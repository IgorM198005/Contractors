using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Contractors
{
    public class CloseFlyoutTrigger
    {
        private Flyout Flyout;

        private Action mCloseCallback;

        public void Flyout_Opened(object sender, object e)
        {
            this.Flyout.Opened -= Flyout_Opened;

            FrameworkElement content = this.Flyout.GetValue(Flyout.ContentProperty) as FrameworkElement;

            ImagePreviewPanel panel = this.Panel = (ImagePreviewPanel)content;

            this.Target = (Image)this.Flyout.Target;

            panel.Tapped += this.ImagePreviewPanel_Tapped;

            Button button = (Button)content.FindName("btn");

            this.CloseFlyoutButton = button;

            button.Click += this.CloseFlyoutButton_Click;

            button.Focus(FocusState.Programmatic);
        }

        private Button CloseFlyoutButton;

        private Image Target;

        private ImagePreviewPanel Panel;

        private void Flyout_Closed(object sender, object e)
        {
            this.Panel.Tapped -= ImagePreviewPanel_Tapped;

            this.CloseFlyoutButton.Click -= CloseFlyoutButton_Click;

            this.Flyout.Closed -= this.Flyout_Closed;

            mCloseCallback?.Invoke();
        }

        private void CloseFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.CloseFlyout();
        }

        private void ImagePreviewPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.CloseFlyout();
        }

        private void CloseFlyout()
        {
            this.Flyout.Hide();

            ToolTip toolTip = (ToolTip)ToolTipService.GetToolTip((Image)this.Target);

            if (toolTip != null) toolTip.IsEnabled = true;
        }

        public static void Img_Tapped(object sender, Action closeCallBack)
        {
            ToolTip toolTip = (ToolTip)ToolTipService.GetToolTip((Image)sender);

            if (toolTip != null)
            {
                if (toolTip.IsOpen) toolTip.IsOpen = false;

                toolTip.IsEnabled = false;
            }

            var flyout = Flyout.GetAttachedFlyout((FrameworkElement)sender);

            var trigger = new CloseFlyoutTrigger()
            {
                Flyout = (Flyout)flyout,

                mCloseCallback = closeCallBack
            }; 

            flyout.Opened += trigger.Flyout_Opened;

            flyout.Closed += trigger.Flyout_Closed;

            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }        
    }
}
