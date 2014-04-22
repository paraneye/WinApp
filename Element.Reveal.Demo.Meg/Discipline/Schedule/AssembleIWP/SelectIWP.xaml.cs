using System;
using System.Collections;
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
using WinAppLibrary.ServiceModels;
using WinAppLibrary.Converters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectIWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
        private int _projectid, _moduleid;

        public SelectIWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            LoadIWP();

            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
            {
                tbpageTitle.Text = "Select Installation Work Package";
            }
            else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
            {
                tbpageTitle.Text = "Select System Installation Work Package";
            }
            else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.HydroTest)
            {
                tbpageTitle.Text = "Select Hydro Test Work Package";
            }
            
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                this.Frame.Navigate(typeof(MainMenu));
            else
                this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectSchedule));
        }

        private void gvIWP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var iwp = e.AddedItems[0] as RevealProjectSvc.FiwpDTO;
            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
            {
                Lib.IWPDataSource.selectedIWP = iwp.FiwpID;
                Lib.IWPDataSource.selectedIWPName = iwp.FiwpName;
                Lib.IWPDataSource.isWizard = iwp.DocEstablishedLUID == WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage ? false : true;
            }
            else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
            {
                Lib.IWPDataSource.selectedSIWP = iwp.FiwpID;
                Lib.IWPDataSource.selectedSIWPName = iwp.FiwpName;
                Lib.IWPDataSource.isWizard = iwp.DocEstablishedLUID == WinAppLibrary.Utilities.DocEstablishedForApp.ITR ? false : true;
            }
            else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.HydroTest)
            {
                Lib.IWPDataSource.selectedHydro = iwp.FiwpID;
                Lib.IWPDataSource.selectedHydroName = iwp.FiwpName;
                Lib.IWPDataSource.isWizard = iwp.DocEstablishedLUID == WinAppLibrary.Utilities.DocEstablishedForApp.SafetyDocument ? false : true;
            }

            //siwp인경우 schedule이 있는지 없는지
            bool isExistSch = Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP && iwp.ProjectScheduleID == 0 ? false : true;

            Lib.WizardDataSource.SetTargetMenu(iwp.DocEstablishedLUID, Lib.CommonDataSource.selPackageTypeLUID, isExistSch);

            if (Lib.WizardDataSource.NextMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
        }

        #endregion

        #region "Private Method"

        private async void LoadIWP()
        {
            Login.MasterPage.Loading(true, this);

            List<RevealProjectSvc.FiwpDTO> source = new List<RevealProjectSvc.FiwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                    {
                        await _iwp.GetFiwpByCwpSchedulePackageTypeOnMode(0, Lib.ScheduleDataSource.selectedSchedule, Lib.CommonDataSource.selPackageTypeLUID);
                    }
                    else
                    {
                        await _iwp.GetFiwpByCwpSchedulePackageTypeOnMode(Lib.CWPDataSource.selectedCWP, Lib.ScheduleDataSource.selectedSchedule, Lib.CommonDataSource.selPackageTypeLUID);
                    }
                    source = _iwp.GetFiwpByProjectScheduleID();
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Select Work Package", "There was an error Work Package. Pleae contact administrator", "Error!");
            }

            this.DefaultViewModel["IWPs"] = source;
            this.gvIWP.SelectedItem = null;

            Login.MasterPage.Loading(false, this);
        }

        #endregion
    }
}
