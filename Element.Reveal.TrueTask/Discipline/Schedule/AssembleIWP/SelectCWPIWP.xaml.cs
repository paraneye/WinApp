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

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectCWPIWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        List<DataLibrary.ComboBoxDTO> _iwp = new List<DataLibrary.ComboBoxDTO>();
        private int _projectid; private string _disciplineCode;

        public SelectCWPIWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadCWP();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        #endregion

        #region "Private Method"

        private async void LoadCWP()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _cwp.GetCWPsByProjectIDOnMode(_projectid, _disciplineCode);

                    _iwp = await (new Lib.ServiceModel.CommonModel()).GetFIWPByCwp_Combo(0, _projectid, _disciplineCode);
                    
                    lvCWP.ItemsSource = _cwp.GetCWPs();

                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }

            Login.MasterPage.Loading(false, this);
        }

        private void LoadIWP()
        {
            if (_iwp != null)
                lvIWP.ItemsSource = _iwp.Where(x => Convert.ToInt32(x.ExtraValue4.Split('/')[0]) == Lib.CWPDataSource.selectedCWP).ToList();
        }

        #endregion

        private void lvIWP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var iwp = e.AddedItems[0] as DataLibrary.ComboBoxDTO;
            Lib.IWPDataSource.selectedIWP = iwp.DataID;
            Lib.IWPDataSource.selectedIWPName = iwp.DataName;
            Lib.ScheduleDataSource.selectedSchedule = Convert.ToInt32(iwp.ExtraValue4.Split('/')[1]);
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleIWP));
        }

        private void lvCWP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cwp = e.AddedItems[0] as DataLibrary.CwpDTO;
            Lib.CWPDataSource.selectedCWP = cwp.CWPID;
            Lib.CWPDataSource.selectedCWPName = cwp.CWPName;
            LoadIWP();
        }
    }
}
