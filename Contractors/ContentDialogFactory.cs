using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Contractors
{
    public class ContentDialogFactory
    {
        private PropertyDialog contentDialog;

        private Grid[] overlays;

        public void Suspend()
        {
        }

        public void Resume()
        {
        }

        private ContentDialogFactory(PropertyDialog dialog)
        {
            this.contentDialog = dialog;            
        }

        public static PropertyDialog Create(Contractor contractor, Grid[] overlayGrids)
        {
            PropertyDialog dialog = new PropertyDialog(contractor);

            var trigger = BindTrigger(dialog, overlayGrids);

            trigger.SetBounds();

            return dialog;             
        }

        private static ContentDialogFactory BindTrigger(PropertyDialog dialog, Grid[] overlayGrids)
        {
            ContentDialogFactory trigger = new ContentDialogFactory(dialog);

            Window.Current.CoreWindow.SizeChanged += trigger.CoreWindow_SizeChanged;

            dialog.Closed += trigger.Dialog_Closed;

            trigger.overlays = overlayGrids;

            foreach (var overlay in overlayGrids)
            {             
                overlay.Visibility = Visibility.Visible;
            }

            return trigger; 
        }

        private void Dialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {             
            Window.Current.CoreWindow.SizeChanged -= this.CoreWindow_SizeChanged;

            foreach (var overlay in this.overlays)
            {             
                overlay.Visibility = Visibility.Collapsed;
            }
        }

        
        private void CoreWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            this.SetBounds();

            this.contentDialog.UpdateLayout();  
        }

        private void SetBounds()
        {
            var dialog = this.contentDialog;

            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;

            double dialogMaxWidth = bounds.Width;

            if (dialogMaxWidth > PropertyDialog.DesignMaxWidth) dialogMaxWidth = PropertyDialog.DesignMaxWidth;

            dialog.MinWidth = dialog.MaxWidth = dialogMaxWidth;

            double dialogMaxHeight = bounds.Height;

            if (dialogMaxHeight > PropertyDialog.DesignMaxHeight) dialogMaxHeight = PropertyDialog.DesignMaxHeight;

            dialog.MinHeight = dialog.MaxHeight = dialogMaxHeight;

            dialog.Vbx1.MaxWidth = dialogMaxWidth * PropertyDialog.Image1WidthPie;

            dialog.Vbx1.MaxHeight = dialogMaxHeight * PropertyDialog.Image1HeightPie;

            dialog.Vbx2.MaxWidth = dialogMaxWidth * PropertyDialog.Image2WidthPie;

            dialog.Vbx2.MaxHeight = dialogMaxHeight * PropertyDialog.Image2HeightPie;
        }
    }
}
