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
    public sealed partial class ITR_LightingAndDeviceCircuit : WinAppLibrary.Controls.LayoutAwarePage
    {
        public ITR_LightingAndDeviceCircuit()
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
        private void chkVIoC1_Checked(object sender, RoutedEventArgs e)
        {            
            CheckBoxControl(chkVIoC1, rdoVIoC1Yes, rdoVIoC1No);
        }
        private void chkVIoC2_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIoC2, rdoVIoC2Yes, rdoVIoC2No);
        }
        private void chkVIoC3_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIoC3, rdoVIoC3Yes, rdoVIoC3No);
        }
        private void chkVIoC4_Checked(object sender, RoutedEventArgs e)
        {            
            CheckBoxControl(chkVIoC4, rdoVIoC4Yes, rdoVIoC4No);
        }
        private void chkVIoC5_Checked(object sender, RoutedEventArgs e)
        {            
            CheckBoxControl(chkVIoC5, rdoVIoC5Yes, rdoVIoC5No);
        }
        private void chkVIoC6_Checked(object sender, RoutedEventArgs e)
        {            
            CheckBoxControl(chkVIoC6, rdoVIoC6Yes, rdoVIoC6No);
        }
        private void chkVIoC7_Checked(object sender, RoutedEventArgs e)
        {            
            CheckBoxControl(chkVIoC7, rdoVIoC7Yes, rdoVIoC7No);
        }
        private void chkVIoC8_Checked(object sender, RoutedEventArgs e)
        {            
            CheckBoxControl(chkVIoC8, rdoVIoC8Yes, rdoVIoC8No);
        }
        private void chkVIoC9_Checked(object sender, RoutedEventArgs e)
        {            
            CheckBoxControl(chkVIoC9, rdoVIoC9Yes, rdoVIoC9No);
        }
        private void chkVIoC10_Checked(object sender, RoutedEventArgs e)
        {            
            CheckBoxControl(chkVIoC10, rdoVIoC10Yes, rdoVIoC10No);
        }

        private void chkFTFCaC1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkFTFCaC1, rdoFTFCaC1Yes, rdoFTFCaC1No);
        }
        private void chkFTFCaC2_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkFTFCaC2, rdoFTFCaC2Yes, rdoFTFCaC2No);
        }
        private void chkFTFCaC3_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkFTFCaC3, rdoFTFCaC3Yes, rdoFTFCaC3No);
        }
        private void chkFTFCaC4_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkFTFCaC4, rdoFTFCaC4Yes, rdoFTFCaC4No);
        }
        private void chkFTFCaC5_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkFTFCaC5, rdoFTFCaC5Yes, rdoFTFCaC5No);
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
