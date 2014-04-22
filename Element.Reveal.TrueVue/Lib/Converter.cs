using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Element.Reveal.TrueVue.Lib
{
    public sealed class DrawingConverter : DependencyObject, IValueConverter
    {
        #region Propertyies
        public string PathBase
        {
            get { return (string)GetValue(PathBaseProperty); }
            set { SetValue(PathBaseProperty, value); }
        }

        public static readonly DependencyProperty PathBaseProperty =
            DependencyProperty.Register("PathBase", typeof(string), typeof(ConcatenateConverter), new PropertyMetadata(""));
        #endregion

        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            try
            {
                var dto = value as RevealProjectSvc.DrawingDTO;
                BitmapImage source = new BitmapImage(new Uri(string.Format("{0}{1}", dto.DrawingFilePath, dto.DrawingFileURL)));
                return source;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ConcatenateConverter : DependencyObject, IValueConverter
    {
        #region Propertyies
        public string PathBase
        {
            get { return (string)GetValue(PathBaseProperty); }
            set { SetValue(PathBaseProperty, value); }
        }

        public static readonly DependencyProperty PathBaseProperty =
            DependencyProperty.Register("PathBase", typeof(string), typeof(ConcatenateConverter), new PropertyMetadata(""));
        #endregion

        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            try
            {
                var dto = value as RevealProjectSvc.DrawingDTO;
                return string.Format("{0}{1}", dto.DrawingFilePath, dto.DrawingFileURL);
            }
            catch
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
