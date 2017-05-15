using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contractors
{
    internal sealed class DataTypeFactory : IDataTypeFactory
    {
        private DataTypeFactory() { }

        private Dictionary<Type, TupleRef<ManualResetEventSlim, DataTypeInfo>> mInfos = new Dictionary<Type, TupleRef<ManualResetEventSlim, DataTypeInfo>>();  

        private ReaderWriterLockSlim mRwLock = new ReaderWriterLockSlim(); 

        private static DataTypeInfo ReflectInfo(Type type)
        {
            DataTypeInfo info = new DataTypeInfo();

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach(var property in properties)
            {
               var attributes = property.GetCustomAttributes(true);

                var validations = new List<ValidationAttribute>();
                
                var expandedMessages = new Dictionary<Type, string>();

                bool isrequired = false; 

                foreach (var attribute in attributes)
                {
                    if (attribute is ClonableFieldAttribute)
                    {
                        info.Clonable.Add(property);

                        continue;  
                    }

                    var notificationField = attribute as NotificationFieldAttribute;

                    if (notificationField != null)
                    {
                        if (string.IsNullOrEmpty(notificationField.Dependency))
                            throw new ArgumentNullException();

                        info.Notifications.Add(notificationField.Dependency, property);

                        continue; 
                    }

                    var maxLengtn = attribute as MaxLengthAttribute; 

                    if (maxLengtn != null)
                    {
                        info.MaxLength[property.Name] = maxLengtn.Length;

                        continue;   
                    }

                    var required = attribute as RequiredAttribute;

                    if (required != null)
                    {
                        if (string.IsNullOrEmpty(required.ErrorMessage))
                            throw new ArgumentNullException();

                        validations.Add(required);

                        isrequired = true; 

                        continue;
                    }

                    var validation = attribute as ValidationAttribute;

                    if (validation != null)
                    {
                        if (string.IsNullOrEmpty(validation.ErrorMessage))
                            throw new ArgumentNullException();

                        validations.Add(validation);  

                        continue; 
                    }

                    var expandedMessage = attribute as ExpandedValidationMessageAttribute;
                    
                    if (expandedMessage != null)
                    {
                        if (string.IsNullOrEmpty(expandedMessage.Message))
                            throw new ArgumentNullException(); 

                        if (expandedMessage.Apply == null)
                            throw new ArgumentNullException();

                        if (!expandedMessages.ContainsKey(expandedMessage.Apply))
                            expandedMessages[expandedMessage.Apply] = expandedMessage.Message;
                    }
                }

                foreach(var rule in validations)
                {
                    string expandedMessage = null;

                    expandedMessages.TryGetValue(rule.GetType(), out expandedMessage);

                    info.ValidationRules.Add(new Tuple<PropertyInfo, ValidationAttribute, string>(property, rule, expandedMessage));    
                }

                if (isrequired) info.Required.Add(property.Name); 
            }

            return info; 
        }

        public Tuple<string, string>[] IsInvalid(Type type, object instance)
        {
            var warnings = new List<Tuple<string, string>>(); 

            var info = this.GetReflctedInfo(type);
            
            foreach(var rule in info.ValidationRules)
            {
                PropertyInfo notificationProperty;

                bool inotify = info.Notifications.TryGetValue(rule.Item1.Name, out notificationProperty);

                object value = rule.Item1.GetValue(instance);

                if (!info.Required.Contains(rule.Item1.Name))
                {
                    if (DataTypeFactory.IsEmpty(rule.Item1.PropertyType, value))
                    {
                        if (inotify) notificationProperty.SetValue(instance, null);

                        continue;
                    }
                }

                if (rule.Item2.IsValid(value))
                {
                    if (inotify) notificationProperty.SetValue(instance, null);

                    continue;
                }

                if (inotify) notificationProperty.SetValue(instance, "*" + rule.Item2.ErrorMessage);

                string message = string.IsNullOrEmpty(rule.Item3) ? rule.Item2.ErrorMessage : rule.Item3;

                warnings.Add(new Tuple<string, string>(rule.Item1.Name, message)); 
            }

            return warnings.ToArray();                                   
        }

        private void ReflectFast(Type type)
        {
            this.mInfos.Add(type, new TupleRef<ManualResetEventSlim, DataTypeInfo>(null, ReflectInfo(type)));  
        }

        public DataTypeInfo GetReflctedInfo(Type type)
        {
            TupleRef<ManualResetEventSlim, DataTypeInfo> slot = null;

            ManualResetEventSlim waitEvent = null;

            bool sign = false;

            this.mRwLock.EnterReadLock();  

            if (sign = this.mInfos.TryGetValue(type, out slot)) waitEvent = slot.Item1;

            this.mRwLock.ExitReadLock();

            if (sign && (waitEvent == null)) return slot.Item2; 
            
            if (waitEvent == null)
            {
                this.mRwLock.EnterWriteLock();

                if (sign = !this.mInfos.TryGetValue(type, out slot))
                {
                    this.mInfos.Add(type,
                            slot = new TupleRef<ManualResetEventSlim, DataTypeInfo>(
                                new ManualResetEventSlim(false), null));
                }
                else
                {
                    waitEvent = slot.Item1;
                }

                this.mRwLock.ExitWriteLock();
            }            

            if (sign)
            {
                slot.Item2 = ReflectInfo(type);

                this.mRwLock.EnterWriteLock();

                slot.Item1.Set();

                slot.Item1 = null;

                this.mRwLock.ExitWriteLock();
            }
            else if (waitEvent != null)
            {
                waitEvent.Wait();                
            }

            return slot.Item2;
        }

        public IDictionary<string, PropertyInfo> GetNotifications(Type type)
        {
            return this.GetReflctedInfo(type).Notifications;  
        }

        public IDictionary<string, int> MaxLength(Type type)
        {
            return this.GetReflctedInfo(type).MaxLength;
        }

        private static bool IsEmpty(Type type, object value)
        {
            if (type == typeof(string))
            {
                return string.IsNullOrEmpty((string)value); 
            }
            if ((type == typeof(object)) || (type.GetTypeInfo().IsSubclassOf(typeof(object))))
            {
                return object.ReferenceEquals(null, value);
            }
            else throw new NotImplementedException(); 
        }

        private static DataTypeFactory mCurrent;
       
        public static IDataTypeFactory Current
        {
            get
            {
                if (mCurrent == null) throw new ArgumentNullException("Текущий контекст не существует, прежде выполните SetCurrent однократно");

                return mCurrent;  
            }
        }

        public object Clone(Type type, object arg)
        {
            var instance = Activator.CreateInstance(type);

            this.CloneTo(type, arg, instance); 

            return instance; 
        }

        public void CloneTo(Type type, object arg0, object arg1)
        {
            foreach (var property in DataTypeFactory.ReflectInfo(type).Clonable)
            {
                property.SetValue(arg1, property.GetValue(arg0));
            }
        }

        private static object mCurrentLock = new object();

        public static async Task SetCurrentAsync(Type[] types = null)
        {
            await Task.Factory.StartNew(() => SetCurrent(types));  
        }

        public static void SetCurrent(Type[] types = null)
        {
            Monitor.Enter(mCurrentLock);

            if (mCurrent == null)
            {
                mCurrent = new DataTypeFactory();

                if (types != null)
                {
                    foreach (var type in types) mCurrent.ReflectFast(type);
                }
            }
                       
            Monitor.Exit(mCurrentLock);  
        }
    }
}
