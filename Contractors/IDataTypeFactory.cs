using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Contractors
{
    internal interface IDataTypeFactory
    {
        IDictionary<string, PropertyInfo> GetNotifications(Type type);

        Tuple<string, string>[] IsInvalid(Type type, object instance);

        IDictionary<string, int> MaxLength(Type type);

        object Clone(Type type, object arg);

        void CloneTo(Type type, object arg0, object arg1);
    }
}
