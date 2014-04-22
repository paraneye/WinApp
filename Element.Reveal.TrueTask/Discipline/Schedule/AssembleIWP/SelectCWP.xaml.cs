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

using Element.Reveal.DataLibrary;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectCWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        private int _projectid;
        private string _pakagetypeLuid;
        private string _disciplineCode;

        public SelectCWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            _pakagetypeLuid = Lib.CommonDataSource.selPackageTypeLUID;

            Lib.ScheduleDataSource.selectedSchedule = 0;

            //if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.HydroTest)
            //    _disciplineCode = 1;

            LoadCWP();
        }

        #region "Event Handler"

        private void cwplist_BackClick(object sender, object e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        private void cwplist_SelectionChanged(object sender, ListViewBase e)
        {
            Lib.CWPDataSource.selectedCWP = ((CwpDTO)e.SelectedItem).CWPID;
            this.Frame.Navigate(typeof(SelectSchedule));
        }

        #endregion

        #region "Private Method"

        private async void LoadCWP()
        {
            Login.MasterPage.Loading(true, this);

            //List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();
            List<CwpDTO> source = new List<CwpDTO>();
            try
            {
                await _cwp.GetCwpByProjectPackageTypeOnMode(_projectid, _disciplineCode, _pakagetypeLuid);
                source = _cwp.GetCWPs();
                cwplist.LoadCWP(source);
            }
            catch (Exception ex)
            {
            }

            Login.MasterPage.Loading(false, this);
        }

        #endregion
    }
}
