using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinAppLibrary.Extensions
{
    public static class TextBoxEx
    {
        public static string GetRealTimeText(TextBox obj)
        {
            return (string)obj.GetValue(RealTimeTextProperty);
        }

        public static void SetRealTimeText(TextBox obj, string value)
        {
            obj.SetValue(RealTimeTextProperty, value);
        }

        public static readonly DependencyProperty RealTimeTextProperty =
            DependencyProperty.RegisterAttached("RealTimeText", typeof(string), typeof(TextBoxEx), null);

        public static bool GetIsAutoUpdate(TextBox obj)
        {
            return (bool)obj.GetValue(IsAutoUpdateProperty);
        }

        public static void SetIsAutoUpdate(TextBox obj, bool value)
        {
            obj.SetValue(IsAutoUpdateProperty, value);
        }

        public static readonly DependencyProperty IsAutoUpdateProperty =
            DependencyProperty.RegisterAttached("IsAutoUpdate", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(false, OnIsAutoUpdateChanged));

        private static void OnIsAutoUpdateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var value = (bool)e.NewValue;
            var textbox = (TextBox)sender;

            if (value)
            {
                //This is based on http://www.microsoft.com/en-us/download/details.aspx?id=28568
                //which still needs more research.
                //Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                //              o => textbox.TextChanged += o,
                //              o => textbox.TextChanged -= o)
                //          .Do(_ => textbox.SetValue(TextBoxEx.RealTimeTextProperty, textbox.Text))
                //          .Subscribe();
            }
        }
    }

}
