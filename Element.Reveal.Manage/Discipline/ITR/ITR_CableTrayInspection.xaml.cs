using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Manage.Discipline.ITR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ITR_CableTrayInspection : WinAppLibrary.Controls.LayoutAwarePage
    {
        public ITR_CableTrayInspection()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #region checkbox control
        private void chkVIC1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC1, rdoVIC1Yes, rdoVIC1No);
        }
        private void chkVIC2_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC2, rdoVIC2Yes, rdoVIC2No);
        }
        private void chkVIC3_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC3, rdoVIC3Yes, rdoVIC3No);
        }
        private void chkVIC4_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC4, rdoVIC4Yes, rdoVIC4No);
        }
        private void chkVIC5_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC5, rdoVIC5Yes, rdoVIC5No);
        }
        private void chkVIC6_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC6, rdoVIC6Yes, rdoVIC6No);
        }
        private void chkVIC7_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC7, rdoVIC7Yes, rdoVIC7No);
        }
        private void chkVIC8_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC8, rdoVIC8Yes, rdoVIC8No);
        }
        private void chkVIC9_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC9, rdoVIC9Yes, rdoVIC9No);
        }
        private void chkVIC10_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC10, rdoVIC10Yes, rdoVIC10No);
        }
        private void chkVIC11_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC11, rdoVIC11Yes, rdoVIC11No);
        }
        private void chkVIC12_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC12, rdoVIC12Yes, rdoVIC12No);
        }
        private void chkVIC13_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC13, rdoVIC13Yes, rdoVIC13No);
        }
        private void chkVIC14_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC14, rdoVIC14Yes, rdoVIC14No);
        }
        private void chkVIC15_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC15, rdoVIC15Yes, rdoVIC15No);
        }
        private void chkVIC16_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC16, rdoVIC16Yes, rdoVIC16No);
        }
        private void chkVIC17_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC17, rdoVIC17Yes, rdoVIC17No);
        }
        private void chkVIC18_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC18, rdoVIC18Yes, rdoVIC18No);
        }
        private void chkVIC19_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC19, rdoVIC19Yes, rdoVIC19No);
        }
        private void chkVIC20_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIC20, rdoVIC20Yes, rdoVIC20No);
        }
        #endregion checkbox control
        private void CheckBoxControl(object checkbox, object radiobutton1, object radiobutton2)
        {
            var chk = checkbox as CheckBox;
            var rdo1 = radiobutton1 as RadioButton;
            var rdo2 = radiobutton2 as RadioButton;

            if (chk.IsChecked == true)
            {
                rdo1.IsChecked = false;
                rdo1.IsEnabled = false;
                rdo2.IsChecked = false;
                rdo2.IsEnabled = false;
            }
            else
            {
                rdo1.IsEnabled = true;
                rdo2.IsEnabled = true;
            }
        }
    }
}
