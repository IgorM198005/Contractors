using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента диалогового окна содержимого см. в разделе http://go.microsoft.com/fwlink/?LinkId=234238

namespace Contractors
{
    public sealed partial class ContentDialog1 : ContentDialog
    {
        public ContentDialog1(BitmapImage bitmapImage)
        {
            this.InitializeComponent();
        }
      
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
        
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".jpeg");
            fileOpenPicker.FileTypeFilter.Add(".bmp");
            fileOpenPicker.FileTypeFilter.Add(".tiff");
            fileOpenPicker.FileTypeFilter.Add(".ico");

            var storageFile = await fileOpenPicker.PickSingleFileAsync();

            if (storageFile == null) return;

            BitmapImage bitmapImage = new BitmapImage();

            using (IRandomAccessStream stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                await bitmapImage.SetSourceAsync(stream);
            }
            

            this.img1.Source = bitmapImage;
        }

        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
