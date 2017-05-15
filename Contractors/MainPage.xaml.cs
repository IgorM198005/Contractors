using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Contractors
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.DataContext = this.Items; 

            this.InitializeComponent();

            this.noResultToolTip = (ToolTip)ToolTipService.GetToolTip(this.SearchBox); 
        }

        public ObservableCollection<Contractor> Items
        {
            get
            {
                return ContractorsContext.Current.Items; 
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);            
        }

        private void ItemsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
             this.OpenPopUp((Contractor)e.ClickedItem);
        }

        private async void OpenPopUp(Contractor contractor)
        {
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += this.AcceleratorKeyActivated;

            this.Overlay.Visibility = Visibility.Visible;

            var pointer = Window.Current.CoreWindow.PointerCursor;

            Window.Current.CoreWindow.PointerCursor =
                    new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 1);

            await ContractorsContext.Current.Expand(contractor); 

            this.popUp.DataContext = contractor;

            this.SetPopUpSize();

            Window.Current.CoreWindow.SizeChanged += this.CoreWindow_SizeChanged;

            Window.Current.CoreWindow.PointerCursor = pointer; 

            this.popUp.IsOpen = true;
        }

        private void CoreWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            this.UpdatePopUpSizeAndLocation();
        }

        private void SetPopUpSize()
        {
            var bounds = this.GetMaxPopUpSize();

            double popUpMaxWidth = bounds.Width;

            if (popUpMaxWidth > PopUpMaxWidth) popUpMaxWidth = PopUpMaxWidth;

            this.popUpRoot.MaxWidth = popUpMaxWidth;

            double popupMaxHeight = bounds.Height;

            if (popupMaxHeight > PopUpMaxHeight) popupMaxHeight = PopUpMaxHeight;

            this.popUpRoot.MaxHeight = popupMaxHeight;

            this.vbx1.MaxWidth = popUpMaxWidth * Image1WidthPie;

            this.vbx1.MaxHeight = popupMaxHeight * Image1HeightPie;

            this.vbx2.MaxWidth = popUpMaxWidth * Image2WidthPie;

            this.vbx2.MaxHeight = popupMaxHeight * Image2HeightPie;
        }

        private Size GetMaxPopUpSize()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;

            Size size = new Size(bounds.Width, bounds.Height);

            size.Width = size.Width - gRoot.Margin.Left - gRoot.Margin.Right - gRoot.Padding.Left - gRoot.Padding.Right;

            size.Height = size.Height - gRoot.Margin.Top - gRoot.Margin.Bottom - gRoot.Padding.Top - gRoot.Padding.Bottom;

            return size;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            this.AddContractor(); 
        }

        private async void AddContractor()
        {
            Contractor contractor = ContractorsContext.Current.CreateForEdit();

            PropertyDialog dialog = ContentDialogFactory.Create(contractor, new Grid[] { this.Overlay }); 

            if(await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var pointer = Window.Current.CoreWindow.PointerCursor;

                Window.Current.CoreWindow.PointerCursor =
                        new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 1);

                await ContractorsContext.Current.Add(contractor);

                ContractorsContext.Current.Collapse(contractor);

                Window.Current.CoreWindow.PointerCursor = pointer;
            }
        }

        [ResurceStroreField]
        public static double PopUpMaxWidth { get; set; }

        [ResurceStroreField]
        public static double PopUpMaxHeight { get; set; }

        [ResurceStroreField]
        public static double Image1WidthPie { get; set; }

        [ResurceStroreField]
        public static double Image1HeightPie { get; set; }

        [ResurceStroreField]
        public static double Image2WidthPie { get; set; }

        [ResurceStroreField]
        public static double Image2HeightPie { get; set; }

        [ResurceStroreField]
        public static int AutoSuggestBoxMaxItems { get; set; }

        private void AddButtonLIstView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.AddContractor();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
        }

        private void UpdatePopUpSizeAndLocation()
        {
            this.SetPopUpSize();

            this.popUp.UpdateLayout();

            this.MovePopupRootToCenter();
        }

        private void PopUpRoot_Loaded(object sender, RoutedEventArgs e)
        {
            this.MovePopupRootToCenter();
        }

        private void MovePopupRootToCenter()
        {
            var bounds = this.GetMaxPopUpSize(); 

            this.popUp.HorizontalOffset = Math.Floor((bounds.Width - this.popUpRoot.DesiredSize.Width) / 2);

            this.popUp.VerticalOffset = Math.Floor((bounds.Height - this.popUpRoot.DesiredSize.Height) / 2);
        }

        private void popUp_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            this.TryClosePopUp(e); 
        }

        private void TryClosePopUp(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                if (this.popUp.IsOpen)
                {
                    e.Handled = true;

                    this.popUp.IsOpen = false;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.popUp.IsOpen = false;
        }

        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.escPopUpClosingSuspended = true;

            CloseFlyoutTrigger.Img_Tapped(sender, this.EscPopUpClosingResume); 
        }

        private void popUp_Opened(object sender, object e)
        {
            this.btnBack.Focus(FocusState.Programmatic); 
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Contractor contractor = (Contractor)this.popUp.DataContext;

            var clone = ContractorsContext.Current.CopyForEdit(contractor); 

            PropertyDialog dialog = ContentDialogFactory.Create(clone, new Grid[] { this.OverlayPoup });

            this.escPopUpClosingSuspended = true;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var pointer = Window.Current.CoreWindow.PointerCursor;

                Window.Current.CoreWindow.PointerCursor =
                        new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 1);

                await ContractorsContext.Current.Update(contractor, clone);

                Window.Current.CoreWindow.PointerCursor = pointer; 
            }

            this.escPopUpClosingSuspended = false;

            this.popUp.UpdateLayout();

            this.MovePopupRootToCenter();

            this.btnBack.Focus(FocusState.Programmatic); 
        }

        private void popUpRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
        }

        private void popUp_Closed(object sender, object e)
        {
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= this.AcceleratorKeyActivated;

            Window.Current.CoreWindow.SizeChanged -= this.CoreWindow_SizeChanged;

            this.Overlay.Visibility = Visibility.Collapsed; 
        }

        private bool escPopUpClosingSuspended;

        private void EscPopUpClosingResume()
        {
            this.escPopUpClosingSuspended = false;
        }

        private void AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (this.escPopUpClosingSuspended) return; 

            if (args.EventType.ToString().Contains("Down"))
            {
                if (args.VirtualKey == VirtualKey.Escape)
                {
                    if (this.popUp.IsOpen) this.popUp.IsOpen = false;
                }
            }
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.ItemsSource = null;

            Contractor contractor = (Contractor)args.ChosenSuggestion;

            if (contractor == null) contractor = this.GetFilteredItems(args.QueryText).FirstOrDefault();

            if (contractor != null)
            {                
                this.ItemsListView.ScrollIntoView(contractor, ScrollIntoViewAlignment.Leading);

                StoryboardFindedListViewItem.Stop();

                var lvi = (ListViewItem)this.ItemsListView.ContainerFromItem(contractor);

                Storyboard.SetTarget(StoryboardFindedListViewItem, lvi);

                StoryboardFindedListViewItem.Begin();
            }
            else
            {
                this.ShowNoResultToolTip();
            }                        
        }
       
        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.Length > 0)
                {
                    var items = this.GetFilteredList(sender.Text);

                    if (items.Count > 0)
                    {
                        this.CloseNoResultToolTip();

                        sender.ItemsSource = items;                        
                    }
                    else
                    {
                        sender.ItemsSource = null;

                        this.ShowNoResultToolTip(); 
                    }
                }                
                else
                {
                    sender.ItemsSource = null;

                    this.CloseNoResultToolTip(); 
                }
            }
        }

        private void ShowNoResultToolTip()
        {
            if (noResultToolTipCancel != null)
                noResultToolTipCancel.Cancel();

            this.noResultToolTip.IsEnabled = 

            this.noResultToolTip.IsOpen = true;

            this.CloseNoResultToolTipDelay();
        }

        private ToolTip noResultToolTip;

        private CancellationTokenSource noResultToolTipCancel;

        private void CloseNoResultToolTip()
        {
            if (this.noResultToolTipCancel != null)
            {
                this.noResultToolTipCancel.Cancel();
                
                if (this.noResultToolTip.IsOpen)
                {
                    this.noResultToolTip.IsEnabled =

                    this.noResultToolTip.IsOpen = false;

                    this.noResultToolTipCancel = null;
                }
            }
        }

        private async void CloseNoResultToolTipDelay()
        {
            this.noResultToolTipCancel = new CancellationTokenSource();

            CancellationToken ctx = this.noResultToolTipCancel.Token;

            await Task.Delay(1500);

            if (!ctx.IsCancellationRequested)
            {
                this.noResultToolTip.IsEnabled =

                this.noResultToolTip.IsOpen = false;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.CloseNoResultToolTip(); 
        }

        private List<Contractor> GetFilteredList(string text)
        {
            List<Contractor> filtered = new List<Contractor>();

            foreach (var item in this.GetFilteredItems(text))
            {
                filtered.Add(item);

                if (filtered.Count == AutoSuggestBoxMaxItems) break;
            }

            return filtered;
        }

        private IEnumerable<Contractor> GetFilteredItems(string text)
        {
            foreach (var item in this.Items)
            {
                if (item.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                {
                    yield return item;
                }
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog(
                "Действительно удалить ?");

            dialog.Commands.Add(new UICommand("Да") { Id = (int)0 });
            dialog.Commands.Add(new UICommand("Нет") { Id = (int)1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if ((int)result.Id != 0) return; 

            var pointer = Window.Current.CoreWindow.PointerCursor;

            Window.Current.CoreWindow.PointerCursor =
                    new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 1);

            Contractor contractor = (Contractor)this.popUp.DataContext;

            await ContractorsContext.Current.Delete(contractor);

            this.popUp.IsOpen = false;

            Window.Current.CoreWindow.PointerCursor = pointer; 
        }

        private void Overlay_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.popUp.IsOpen) this.btnBack.Focus(FocusState.Programmatic); 
        }        
    }
}

