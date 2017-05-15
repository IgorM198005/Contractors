using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Contractors
{
    public class ImagePreviewPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Size mAvailableSize = availableSize;

            Button btn = (Button)this.FindName("btn");

            btn.Measure(availableSize);

            mAvailableSize.Height -= btn.DesiredSize.Height;

            Viewbox vbox = (Viewbox)this.FindName("vbx");

            vbox.Measure(new Size(availableSize.Width, availableSize.Height - btn.DesiredSize.Height));

            return availableSize; 
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Viewbox vbox = (Viewbox)this.FindName("vbx");

            Button btn = (Button)this.FindName("btn");

            double vBoxX = Math.Floor((finalSize.Width - vbox.DesiredSize.Width) / 2);  

            double vBoxY= Math.Floor((finalSize.Height - vbox.DesiredSize.Height - btn.DesiredSize.Height) / 2);

            vbox.Arrange(new Rect(new Point(vBoxX, vBoxY), vbox.DesiredSize));            

            double vBtnX = (vBoxX + vbox.DesiredSize.Width) - btn.DesiredSize.Width;

            if(vBtnX < vBoxX)
            {
                vBtnX = Math.Floor((finalSize.Width - btn.DesiredSize.Width) / 2);
            }

            Point btnPoint = new Point
            {
                X = vBtnX,

                Y = vBoxY + vbox.DesiredSize.Height
            };

            btn.Arrange(new Rect(btnPoint, btn.DesiredSize));

            return finalSize; 
        }

        private Flyout Flyout;

        public static void Flyout_Opened(object sender, object e)
        {
            var flyout = (Flyout)sender;

            flyout.Opened -= Flyout_Opened;

            FrameworkElement content = flyout.GetValue(Flyout.ContentProperty) as FrameworkElement;

            ImagePreviewPanel panel = (ImagePreviewPanel)content;

            panel.Flyout = flyout;

            panel.Target = (Image)flyout.Target;  

            panel.Tapped += panel.ImagePreviewPanel_Tapped;

            Button button = (Button)content.FindName("btn");

            panel.CloseFlyoutButton = button;

            button.Click += panel.CloseFlyoutButton_Click;

            flyout.Closed += panel.Flyout_Closed;

            button.Focus(FocusState.Programmatic);
        }

        private Button CloseFlyoutButton;

        private Image Target;

        private void Flyout_Closed(object sender, object e)
        {
            this.Tapped -= ImagePreviewPanel_Tapped;

            this.CloseFlyoutButton.Click -= CloseFlyoutButton_Click;

            var flyout = (Flyout)sender;

            flyout.Closed -= this.Flyout_Closed;            
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

        public static void Img_Tapped(object sender)
        {
            ToolTip toolTip = (ToolTip)ToolTipService.GetToolTip((Image)sender);

            if (toolTip != null)
            {
                if (toolTip.IsOpen) toolTip.IsOpen = false;

                toolTip.IsEnabled = false;
            }
             
            var flyout = Flyout.GetAttachedFlyout((FrameworkElement)sender);

            flyout.Opened += ImagePreviewPanel.Flyout_Opened;

            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
