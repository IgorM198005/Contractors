using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Contractors
{
    public sealed class IsTrueStateTrigger : BoolStateTrigger
    {
        public override bool WaitValue
        {
            get
            {
                return true;
            }
        }
    }
}
