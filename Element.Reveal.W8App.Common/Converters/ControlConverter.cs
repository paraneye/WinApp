using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using WinAppLibrary.Utilities;

namespace WinAppLibrary.Converters
{
    public sealed class SelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            try
            {
                var image = (bool)value;
                if (image)
                    return parameter;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, string culture)
        {
            throw new NotImplementedException();
        }

        private async Task<BitmapImage> ImageStreamAsyn(string url)
        {
            var response = await (new SPDocument()).GetDocument(url);

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(response);
                await writer.StoreAsync();
                BitmapImage b = new BitmapImage();
                b.SetSource(stream);
                return b;
            }
        }
    }

    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            return String.Format("{0:MMM/dd/yyyy}", (DateTime)value);
        }

        public object ConvertBack(object value, Type targetType,
                          object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListViewItemBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            var backcolor = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black);

            ListViewItem item = value as ListViewItem;

            if (item != null)
            {
                ListView lv = ItemsControl.ItemsControlFromItemContainer(item) as ListView;

                int index = lv.ItemContainerGenerator.IndexFromContainer(item);

                if (index % 2 > 0)
                    backcolor = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);
            }

            backcolor.Opacity = 0.25;

            return backcolor;
        }

        public object ConvertBack(object value, Type targetType,
                          object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }


}
