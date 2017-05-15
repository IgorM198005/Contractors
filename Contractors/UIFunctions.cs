using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Reflection;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Storage.Streams;

namespace Contractors
{
    internal static class UIFunctions
    {
        public static Tuple<TextBox, string>[] GetTextBoxesWithBindedText(UIElement[] elements)
        {
            List<Tuple<TextBox, string>> textboxes = new List<Tuple<TextBox, string>>();

            for (int i = 0; i < elements.Length; i++)
            {
                TextBox textBox = elements[i] as TextBox;

                if (textBox == null) continue;

                var bexp = textBox.GetBindingExpression(TextBox.TextProperty);

                if (bexp == null) continue;

                textboxes.Add(new Tuple<TextBox, string>(textBox, bexp.ParentBinding.Path.Path));
            }

            return textboxes.ToArray(); 
        }

        public static UIElement[] EnumerateUIElements(DependencyObject root)
        {
            List<UIElement> uiElements = new List<UIElement>();

            UIFunctions.EnumerateUIElements(root, uiElements);

            return uiElements.ToArray();
        }

        private static void EnumerateUIElements(DependencyObject root, List<UIElement> uiElements)
        {
            var uiRoot = root as UIElement;

            if (uiRoot != null) uiElements.Add(uiRoot);

            int count = VisualTreeHelper.GetChildrenCount(root);

            for (int i = 0; i < count; i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(root, i);

                UIFunctions.EnumerateUIElements(current, uiElements);
            };
        }

        public static void SetFieldsFromResources(Type type)
        {            
            var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach(var property in properties)
            {
                if (property.GetCustomAttributes(typeof(ResurceStroreFieldAttribute)).Any())
                {
                    string resourceKey = string.Format("{0}{1}", type.Name, property.Name);

                    object value = App.Current.Resources[resourceKey];

                    property.SetValue(null, value); 
                }
            }
        }

        public static async Task<BitmapImage> RawToImage(byte[] bytes)
        {
            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes((byte[])bytes);

                    await writer.StoreAsync();
                }

                var image = new BitmapImage();

                await image.SetSourceAsync(ms);

                return image;
            }
        }
    }
}
