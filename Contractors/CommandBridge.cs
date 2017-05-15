using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Contractors
{
    internal sealed class CommandBridge : ICommand
    {
        public Action<object> To { get; set; } 

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true; 
        }

        public void Execute(object parameter)
        {
            this.To(parameter); 
        }
    }
}
