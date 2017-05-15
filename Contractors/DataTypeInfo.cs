using System;
using System.ComponentModel.DataAnnotations; 
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Contractors
{
    internal sealed class DataTypeInfo
    {
        public DataTypeInfo()
        {
            this.Notifications = new Dictionary<string, PropertyInfo>();

            this.ValidationRules = new List<Tuple<PropertyInfo, ValidationAttribute, string>>();

            this.Required = new HashSet<string>();

            this.MaxLength = new Dictionary<string, int>();

            this.Clonable = new List<PropertyInfo>();  
        }

        public Dictionary<string, PropertyInfo> Notifications { get; private set; }

        public List<Tuple<PropertyInfo, ValidationAttribute, string>> ValidationRules { get; private set; }

        public HashSet<string> Required { get; private set; }

        public Dictionary<string, int> MaxLength { get; private set; }

        public List<PropertyInfo> Clonable { get; private set; }
    }
}
