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
    public sealed partial class ITR_LightingandDeviceInstallation : WinAppLibrary.Controls.LayoutAwarePage
    {
        public ITR_LightingandDeviceInstallation()
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
        private void chkVIfLaC1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfLaC1, rdoVIfLaC1Yes, rdoVIfLaC1No);
        }
        private void chkVIfLaC2_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfLaC2, rdoVIfLaC2Yes, rdoVIfLaC2No);
        }
        private void chkVIfLaC3_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfLaC3, rdoVIfLaC3Yes, rdoVIfLaC3No);
        }
        private void chkVIfLaC4_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfLaC4, rdoVIfLaC4Yes, rdoVIfLaC4No);
        }

        private void chkVIfRaC1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfRaC1, rdoVIfRaC1Yes, rdoVIfRaC1No);
        }
        private void chkVIfRaC2_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfRaC2, rdoVIfRaC2Yes, rdoVIfRaC2No);
        }
        private void chkVIfRaC3_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfRaC3, rdoVIfRaC3Yes, rdoVIfRaC3No);
        }
        private void chkVIfRaC4_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfRaC4, rdoVIfRaC4Yes, rdoVIfRaC4No);
        }

        private void chkVIfHaC1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfHaC1, rdoVIfHaC1Yes, rdoVIfHaC1No);
        }
        private void chkVIfHaC2_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfHaC2, rdoVIfHaC2Yes, rdoVIfHaC2No);
        }
        private void chkVIfHaC3_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfHaC3, rdoVIfHaC3Yes, rdoVIfHaC3No);
        }
        private void chkVIfHaC4_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfHaC4, rdoVIfHaC4Yes, rdoVIfHaC4No);
        }

        private void chkVIfDaC1_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfDaC1, rdoVIfDaC1Yes, rdoVIfDaC1No);
        }
        private void chkVIfDaC2_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfDaC2, rdoVIfDaC2Yes, rdoVIfDaC2No);
        }
        private void chkVIfDaC3_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfDaC3, rdoVIfDaC3Yes, rdoVIfDaC3No);
        }
        private void chkVIfDaC4_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxControl(chkVIfDaC4, rdoVIfDaC4Yes, rdoVIfDaC4No);
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
