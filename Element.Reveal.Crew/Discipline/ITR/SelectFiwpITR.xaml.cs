using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using Windows.Networking.Proximity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Core;

using WinAppLibrary;
using Element.Reveal.Crew.RevealCommonSvc;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Utilities;
using WinAppLibrary.Extensions;

using System.Reflection;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.ITR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectFiwpITR : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.DataSource.SelectFiwpITRDataSource _FiwpList = new Lib.DataSource.SelectFiwpITRDataSource();

        public SelectFiwpITR()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("Select Installation Work Package");
            Login.MasterPage.DoBeforeBack += MasterPage_DoBeforeBack;

            BindFiwpList();
        }

        void MasterPage_DoBeforeBack(object sender, object e)
        {
            this.Frame.Navigate(typeof(ITRMenu), Login.UserAccount.PersonnelID);
        }
        
        private async void BindFiwpList()
        {
            try
            {
                List<ComboBoxDTO> source = null;
                await _FiwpList.GetFiwpList(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, Login.UserAccount.PersonnelID, Department.Foreman); //test: Login.UserAccount.PersonnelID = 3

                source = _FiwpList.ReturnFiwpList();
                if (source == null)
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Not Found Data", "Not Found Data!");
                else 
                this.DefaultViewModel["FiwpList"] = source;
            }
            catch (Exception ex)
            {

            }
        }

        private void Text_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var item = sender as TextBlock;

            string FiwpName = item.Text + "/" + item.Tag.ToString();

            this.Frame.Navigate(typeof(DownloadITR), FiwpName);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiwpList.SelectedItems.Count > 0)
            {
                ComboBoxDTO item = (ComboBoxDTO)lvFiwpList.SelectedItem;
                string FiwpName = item.DataName + "/" + item.DataID.ToString();

                this.Frame.Navigate(typeof(DownloadITR), FiwpName);
            }
            else
                WinAppLibrary.Utilities.Helper.SimpleMessage("Do Not Select Fiwp", "Caution!");
        }
        
    }
}
