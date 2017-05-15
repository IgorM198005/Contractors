using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contractors
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple =false, Inherited =true)]
    internal sealed class NotificationFieldAttribute : Attribute
    {
        public string Dependency { get; set; }

        public NotificationFieldAttribute(string dependency)
        {
            this.Dependency = dependency;
        }
    }
}
