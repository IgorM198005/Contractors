using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Contractors
{
    public abstract class BoolStateTrigger : StateTriggerBase
    {
        public abstract bool WaitValue { get; }

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(BoolStateTrigger),
            new PropertyMetadata(true, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (BoolStateTrigger)d;

            var val = (bool)e.NewValue;

            obj.SetActive(val == obj.WaitValue);
        }
    }
}
