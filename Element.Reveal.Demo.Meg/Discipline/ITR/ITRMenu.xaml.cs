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

using WinAppLibrary;

using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.ITR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ITRMenu : WinAppLibrary.Controls.LayoutAwarePage
    {
        public ITRMenu()
        {
            Login.MasterPage.SetPageTitle("Inspection & Test Record(ITR)");
            Login.MasterPage.ShowBackButton(false);
            this.InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu), Login.UserAccount.PersonnelID);
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.ShowBackButton(true);

            Login.MasterPage.Loading(true, this);
            await LoadSelectITR();
            Login.MasterPage.Loading(false, this);
        }

        private async Task<bool> LoadSelectITR()
        {
            bool retvalue = true;
            try
            {
                this.Frame.Navigate(typeof(SelectFiwpITR), Login.UserAccount.PersonnelID);
            }
            catch (Exception ex)
            {
                retvalue = false;
            }

            return retvalue;
        }

        private void btnFillOut_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.ShowBackButton(true);
            this.Frame.Navigate(typeof(FillOutSubmitITR), "1");   //"1" : Fillout Mode  "2" : Submit Mode
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.ShowBackButton(true);
            this.Frame.Navigate(typeof(FillOutSubmitITR), "2");   //"1" : Fillout Mode  "2" : Submit Mode
        }

        private void btnApproval_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.ShowBackButton(true);
            this.Frame.Navigate(typeof(FillOutSubmitITR), "3");  // Approval Mode
        }
    }
}
