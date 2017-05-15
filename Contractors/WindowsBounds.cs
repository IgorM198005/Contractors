using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Contractors
{
    internal static class WindowsBounds
    {
        public static Size GetMindowsBounds()
        {
            var view = CoreApplication.GetCurrentView();

            if (view == null) return new Size(0, 0);

            var window = view.CoreWindow;

            if (window == null) return new Size(0, 0);

            return new Size(window.Bounds.Width, window.Bounds.Height); 
        }

        private static double? mPropertyDialogMaxWidth;

        private static double PropertyDialogMaxWidth
        {
            get
            {
                if (!mPropertyDialogMaxWidth.HasValue)

                    mPropertyDialogMaxWidth = (double)App.Current.Resources["PropertyDialogMaxWidth"];

                return mPropertyDialogMaxWidth.Value; 

            }
        }

        private static double? mPropertyDialogMargin;

        private static double PropertyDialogMargin
        {
            get
            {
                if (!mPropertyDialogMargin.HasValue)

                    mPropertyDialogMargin = (double)App.Current.Resources["PropertyDialogMargin"];

                return mPropertyDialogMargin.Value;

            }
        }

        public static void ResizeContentDialog(ContentDialog dialog, Page page, Size newSize)
        {
            double newWidth = newSize.Width > PropertyDialogMaxWidth ? PropertyDialogMaxWidth : newSize.Width;

            newWidth -= 2*PropertyDialogMargin;

            if (dialog.ActualWidth != newWidth)
            {
                dialog.MaxWidth = newWidth;

                dialog.MinWidth = newWidth;  
            }
        }
    }
}
