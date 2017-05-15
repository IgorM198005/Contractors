using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contractors
{
    internal sealed class ExpandedValidationMessageAttribute : Attribute
    {
        public ExpandedValidationMessageAttribute(string message, Type apply)
        {
            this.Message = message;

            this.Apply = apply; 
        }

        public string Message { get; set; }

        public Type Apply { get; set; }
    }
}
