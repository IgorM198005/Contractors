using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Contractors
{
    internal sealed class ClearButtonVisibilityTrigger : StateTriggerBase
    {
        private TextBox mTextBox;
                
        private bool? mWait;
        
        public bool Wait
        {
            set
            {
                this.mWait = value;

                this.TrySetActive(true);
            }
        } 

        private bool isTextBoxFocused;

        private bool isActive;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.TrySetActive();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.isTextBoxFocused = false;

            this.TrySetActive();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.isTextBoxFocused = true;

            this.TrySetActive();
        }

        private void TrySetActive(bool anycase = false)
        {
            if ((!this.mWait.HasValue) || (this.mTextBox == null)) return;

            bool nIsActive = ((!string.IsNullOrEmpty(this.mTextBox.Text)) && (this.isTextBoxFocused || this.isPointerOverButton)) == this.mWait.Value;

            if (anycase || (nIsActive != this.isActive))
            {
                this.SetActive(nIsActive);

                this.isActive = nIsActive; 
            }
        }

        public TextBox TextBox
        {
            get { return (TextBox)GetValue(TextBoxProperty); }
            set { SetValue(TextBoxProperty, value); }
        }

        public Button Button
        {
            get { return (Button)GetValue(ButtonProperty); }
            set { SetValue(ButtonProperty, value); }
        }

        private void SetTextBox(TextBox value)
        {
            if (value == this.mTextBox) return; 

            this.mTextBox = value;

            this.TextBox.GotFocus += TextBox_GotFocus;

            this.TextBox.LostFocus += TextBox_LostFocus;

            this.TextBox.TextChanged += TextBox_TextChanged;

            this.TrySetActive(true);
        }

        private Button mButton;

        private bool isPointerOverButton;

        private void SetButton(Button button)
        {
            if (button == this.mButton) return;

            this.mButton = button;

            this.mButton.PointerEntered += MButton_PointerEntered;

            this.mButton.PointerExited += MButton_PointerExited;
        }

        private void MButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.isPointerOverButton = false;

            this.TrySetActive();
        }

        private void MButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.isPointerOverButton = true;

            this.TrySetActive();
        }

        private static void TextBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ClearButtonVisibilityTrigger)d).SetTextBox((TextBox)e.NewValue);
        }

        private static void ButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ClearButtonVisibilityTrigger)d).SetButton((Button)e.NewValue);
        }

        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.Register("TextBox", typeof(TextBox), typeof(ClearButtonVisibilityTrigger), new PropertyMetadata(true, TextBoxChanged));

        public static readonly DependencyProperty ButtonProperty =
            DependencyProperty.Register("Button", typeof(Button), typeof(ClearButtonVisibilityTrigger), new PropertyMetadata(true, ButtonChanged));
    }
}
