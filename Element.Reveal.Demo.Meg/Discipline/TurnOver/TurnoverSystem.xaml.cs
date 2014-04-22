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

namespace Element.Reveal.Meg.Discipline.TurnOver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TurnoverSystem : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid;
        public TurnoverSystem()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            LoadSystem();
        }

        private async void LoadSystem()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    List<RevealProjectSvc.SystemDTO> _dto = new List<RevealProjectSvc.SystemDTO>();
                    //lvSystem.ItemsSource = await(new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo(_projectid, _moduleid);
                   // _dto = await (new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo(_projectid, _moduleid);

                    _dto = await (new Lib.ServiceModel.ProjectModel()).GetSystemByTurnoverProject(_projectid, 1);
                   
                    lvSystem.ItemsSource = _dto;
                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }

            Login.MasterPage.Loading(false, this);
            grButton.Visibility = Windows.UI.Xaml.Visibility.Visible;

            
        }


        private void lvSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                /*
                if (lvCWP.SelectedItems.Count > 0)
                {
                    Lib.CWPDataSource.selectedCWP = ((RevealCommonSvc.ComboBoxDTO)lvCWP.SelectedItem).DataID;
                    Lib.CWPDataSource.selectedCWPName = ((RevealCommonSvc.ComboBoxDTO)lvCWP.SelectedItem).DataName;
                    this.Frame.Navigate(typeof(SelectTask));
                }*/
            }
            catch (Exception ex)
            {
            }

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }


        private void btnMC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvSystem.SelectedItems.Count > 0)
                {
                    int systemId = ((RevealProjectSvc.SystemDTO)lvSystem.SelectedItem).SystemID;  // ((RevealCommonSvc.ComboBoxDTO)lvSystem.SelectedItem).DataID;
                    MCCertificate.systemId = systemId;
                    MCCertificate._projectid = _projectid;
                    MCCertificate._moduleid = _moduleid;
                    TCCCCertificate.Hide();
                    Certificate.Load();


                }
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Please select system", "System");
            }
            catch
            {
            }
        }

        private void btnTCCC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvSystem.SelectedItems.Count > 0)
                {
                    int systemId = ((RevealProjectSvc.SystemDTO)lvSystem.SelectedItem).SystemID;
                    TCCCCertificate.systemId = systemId;
                    TCCCCertificate._projectid = _projectid;
                    TCCCCertificate._moduleid = _moduleid;
                    Certificate.Hide();
                    TCCCCertificate.Load();

                }
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Please select system", "System");
            }
            catch
            {
            }
        }

        private void btnItr_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.ITR.ITRReportSummary));
        }

        private void btnPunch_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.PunchCard.PunchSummary),1);
        }

        private void btnBinder_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Viewer.TurnoverbinderGridViewer),1);
        }

    }
}
