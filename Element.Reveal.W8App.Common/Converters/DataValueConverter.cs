using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace WinAppLibrary.Converters
{
    public sealed class NumberToInputTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int no = 0;

            try
            {
                no = System.Convert.ToInt32(value);
            }
            catch { }

            return no > 0 ? no.ToString() : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    public class AvailabilityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //DTOStatus가 New인 항목 배경색 구분
            var Availability = (int)value;
            
            var color = Availability == (int)Element.Reveal.DataLibrary.Utilities.RowStatus.New ? new SolidColorBrush(Colors.LightGray) : null;
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }


    public class IsVisibleDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var FileType = (string)value;

            //ITR 타입일 때 보이기
            var ReturnValue = Visibility.Collapsed;
            if (FileType == Element.Reveal.DataLibrary.Utilities.FileType.ITR)
                ReturnValue = Visibility.Visible;
            return ReturnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
