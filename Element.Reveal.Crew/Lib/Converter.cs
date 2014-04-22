using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Element.Reveal.Crew.Lib
{
    public sealed class TimesheetTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            try
            {
                var timsheet = (RevealProjectSvc.TimesheetDTO)value;
                if (timsheet != null)
                    return (timsheet.StraightTime + timsheet.TimeAndHalf + timsheet.DoubleTime);
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
