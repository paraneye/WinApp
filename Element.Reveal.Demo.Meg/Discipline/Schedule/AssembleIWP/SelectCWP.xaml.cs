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
    public sealed partial class SelectCWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        private int _projectid, _moduleid, _pakagetypeLuid;

        public SelectCWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            _pakagetypeLuid = Lib.CommonDataSource.selPackageTypeLUID;

            Lib.ScheduleDataSource.selectedSchedule = 0;

            //if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.HydroTest)
            //    _moduleid = 1;

            LoadCWP();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        private void gvCWP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cwp = e.AddedItems[0] as RevealProjectSvc.CwpDTO;
            Lib.CWPDataSource.selectedCWP = cwp.CWPID;
            Lib.CWPDataSource.selectedCWPName = cwp.CWPName;

            //Lib.ScheduleDataSource.selectedSchedule = 188;
            //this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectIWP));

            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectIWP));
            else
                this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectSchedule));
        }

        #endregion

        #region "Private Method"

        private async void LoadCWP()
        {
            Login.MasterPage.Loading(true, this);

            List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _cwp.GetCwpByProjectPackageTypeOnMode(_projectid, _moduleid, _pakagetypeLuid);
                    source = _cwp.GetCWPs();
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SelectCWP LoadCWP", "There was an error load CWP. Pleae contact administrator", "Error!");
            }

            this.DefaultViewModel["CWPs"] = source;
            this.gvCWP.SelectedItem = null;

            Login.MasterPage.Loading(false, this);
        }

        #endregion
    }
}
