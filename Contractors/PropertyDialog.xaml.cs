using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;

// Документацию по шаблону элемента диалогового окна содержимого см. в разделе http://go.microsoft.com/fwlink/?LinkId=234238

namespace Contractors
{
    public sealed partial class PropertyDialog : ContentDialog
    {
        public ICommand ClearTextBox { get; private set; }

        private Contractor iContractor; 

        public PropertyDialog(Contractor contractor)
        {
            this.ImageHint = "HHH";

            this.InitializeComponent();

            this.mResetValidationMap = DataTypeFactory.Current.GetNotifications(typeof(Contractor));  

            this.ClearTextBox = new CommandBridge() { To = this.OnClearTextBox };

            this.DataContext = this.iContractor = contractor;

            this.BuildTextBoxesMap();

            this.SetImagesToolTip();
        }

        private void SetImagesToolTip()
        {
            foreach(var image in new Image[] { this.img1, this.img2})
            {
                ToolTip toolTip = (ToolTip)ToolTipService.GetToolTip(image);

                TextBlock tbl = toolTip.Content as TextBlock;

                tbl.SetBinding(TextBlock.TextProperty, this.GetImageToolTipBinding());
            }
        }

        private Binding GetImageToolTipBinding()
        {
            Binding binding = new Binding();

            binding.Source = this;

            binding.Path = new PropertyPath(nameof(ImageHint));

            binding.Mode = BindingMode.TwoWay;

            return binding; 
        }

        public Viewbox Vbx1 { get { return this.vbx1; } }

        public Viewbox Vbx2 { get { return this.vbx2; } }

        [ResurceStroreField]
        public static double DesignMaxHeight { get; set; }

        [ResurceStroreField]
        public static double DesignMaxWidth { get; set; }

        [ResurceStroreField]
        public static double Image1WidthPie { get; set; }

        [ResurceStroreField]
        public static double Image1HeightPie { get; set; }

        [ResurceStroreField]
        public static double Image2WidthPie { get; set; }

        [ResurceStroreField]
        public static double Image2HeightPie { get; set; }

        [ResurceStroreField]
        public static string ZoomImageHint { get; set; }

        [ResurceStroreField]
        public static string NoImageHint { get; set; }

        [ResurceStroreField]
        public static string MaxLengthReached { get; set; }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender; 

            if ((textBox.Text ?? string.Empty).Length == textBox.MaxLength)
            {
                ToolTip toolTip = (ToolTip)ToolTipService.GetToolTip(textBox);

                if (toolTip == null)
                {                    
                    toolTip = new ToolTip() { Content = MaxLengthReached };

                    ToolTipService.SetToolTip(textBox, toolTip);
                }
                else toolTip.IsEnabled = true;  
                
                if (!toolTip.IsOpen)
                {
                    toolTip.IsOpen = true;

                    await Task.Delay(1500);

                    toolTip.IsOpen = false;

                    toolTip.IsEnabled = false;
                }
            }
        }

        private IDictionary<string, TextBox> mTextBoxesMap; 

        private IDictionary<string, PropertyInfo> mResetValidationMap;

        private void ResetTextBox_InvalidState(object sender, TextChangedEventArgs e)
        {
            this.mResetValidationMap[((TextBox)sender).Name].SetValue(this.DataContext, string.Empty); 
        }

        private void OnClearTextBox(object parameter)
        {
            TextBox textBox = (TextBox)parameter;

            textBox.Text = string.Empty;

            textBox.Focus(FocusState.Programmatic); 
        }

        private void BuildTextBoxesMap()
        {
            this.mTextBoxesMap = new Dictionary<string, TextBox>();

            this.mResetValidationMap = new Dictionary<string, PropertyInfo>();

            var notificationsPropertyMap = DataTypeFactory.Current.GetNotifications(typeof(Contractor));

            var lengthLimits = DataTypeFactory.Current.MaxLength(typeof(Contractor)); 

            var elements = UIFunctions.EnumerateUIElements(this.ucRoot);

            var textBoxesBind = UIFunctions.GetTextBoxesWithBindedText(elements);   

            foreach(var textBoxBind in textBoxesBind)
            {
                this.mTextBoxesMap.Add(textBoxBind.Item2, textBoxBind.Item1);

                int maxLength;

                if (lengthLimits.TryGetValue(textBoxBind.Item2, out maxLength))
                    textBoxBind.Item1.MaxLength = maxLength; 

                PropertyInfo info;

                if (notificationsPropertyMap.TryGetValue(textBoxBind.Item2, out info))
                {
                    if (string.IsNullOrEmpty(textBoxBind.Item1.Name))
                        throw new ArgumentNullException(); 

                    this.mResetValidationMap.Add(textBoxBind.Item1.Name, info);

                    textBoxBind.Item1.TextChanged += this.ResetTextBox_InvalidState;
                } 
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
           var warnings = DataTypeFactory.Current.IsInvalid(typeof(Contractor), this.DataContext);

           if (args.Cancel = (warnings.Length > 0))
           {
                var messageDialog = new MessageDialog(warnings[0].Item2);

                await messageDialog.ShowAsync();

                for(int i = 0; i < warnings.Length; i++)
                {
                    TextBox textBox;

                    if (this.mTextBoxesMap.TryGetValue(warnings[0].Item1, out textBox))
                    {
                        textBox.Focus(FocusState.Programmatic);

                        break; 
                    }                    
                }
            }           
        }

        private void contentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.tblNameVal.Focus(FocusState.Programmatic);
        }

        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.iContractor.PhotoRaw == null)
            {
                this.ChoosePhoto(); 
            }
            else
            {
                CloseFlyoutTrigger.Img_Tapped(sender, null);
            }            
        }

        private void contentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            this.tblNameVal.Focus(FocusState.Programmatic);
        }

        private void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            this.ChoosePhoto();
        }

        private async void ChoosePhoto()
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

            var pointer = Window.Current.CoreWindow.PointerCursor;

            Window.Current.CoreWindow.PointerCursor =
                    new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 1);

            using (IRandomAccessStream stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                byte[] fileBytes = new byte[stream.Size];

                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);

                    reader.ReadBytes(fileBytes);

                    try
                    {
                        this.iContractor.Photo = await UIFunctions.RawToImage(fileBytes);
                    }
                    catch(Exception e)
                    {
                        if (e.HResult == -2003292336)
                        {
                            Window.Current.CoreWindow.PointerCursor = pointer;

                            var dialog = new MessageDialog("То что вы пытаетесь присвоить - технически не совсем изображение");

                            dialog.Commands.Add(new UICommand("Закрыть"));                            
                            dialog.DefaultCommandIndex = 0;
                            dialog.CancelCommandIndex = 0;

                            await dialog.ShowAsync();

                            return; 
                        }
                        else throw;  
                    }

                    this.iContractor.PhotoRaw = fileBytes;

                    this.ImageHint = ZoomImageHint; 
                }
            }

            Window.Current.CoreWindow.PointerCursor = pointer; 
        }

        private void DetachPhoto_Click(object sender, RoutedEventArgs e)
        {
            this.iContractor.Photo = ContractorsContext.Current.NoImage;

            this.iContractor.PhotoRaw = null;

            this.ImageHint = NoImageHint;
        }

        private void contentDialog_Loading(FrameworkElement sender, object args)
        {
            if (this.iContractor.PhotoRaw == null)
            {               
                this.ImageHint = NoImageHint;
            }
            else
            {
                this.ImageHint = ZoomImageHint; 
            }
        }

        public String ImageHint
        {
            get { return (String)GetValue(ImageHintProperty); }
            set { SetValue(ImageHintProperty, value); }
        }

        public static readonly DependencyProperty ImageHintProperty = DependencyProperty.Register(
              "ImageHint",
              typeof(String),
              typeof(PropertyDialog),
              new PropertyMetadata(null));
    }
}
