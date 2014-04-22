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

namespace Element.Reveal.TrueTask.Discipline.Schedule.ManageSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectSchedule : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ScheduleDataSource _schedule = new Lib.ScheduleDataSource();
        // Lib.UI.ScheduleDetail _scheduleDetail = new Lib.UI.ScheduleDetail();
        private int _projectid; private string _disciplineCode;
                
        public SelectSchedule()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadSchedule();            
        }
        
        #region "Event Handler"

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var top = (LayoutRoot.RowDefinitions[1].ActualHeight - 470) / 2;
            //ScrollViewer.Padding = new Thickness(0, top, 0, 0);
        }

        private void gvSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var schedule = e.AddedItems[0] as WinAppLibrary.ServiceModels.DataItem;
                Lib.ScheduleDataSource.selectedSchedule = int.Parse(schedule.UniqueId);
                Lib.ScheduleDataSource.selectedScheduleName = schedule.Title;                
                string[] scheduleDate = schedule.ImagePath.Split('~');
                Lib.ScheduleDataSource.selectedScheduleStartDate = string.IsNullOrEmpty(scheduleDate[0]) ? "" : Convert.ToDateTime(scheduleDate[0]).ToString("MM/dd/yyyy");
                Lib.ScheduleDataSource.selectedScheduleEndDate = string.IsNullOrEmpty(scheduleDate[1]) ? "" : Convert.ToDateTime(scheduleDate[1]).ToString("MM/dd/yyyy");

                this.Frame.Navigate(typeof(Discipline.Schedule.ManageSchedule.SubMenu));      
            }
        }
                     
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.ManageSchedule.SelectCWP));
        }

        #endregion

        #region Private Method"

        private async void LoadSchedule()
        {
            Login.MasterPage.Loading(true, this);
            //StretchingPanel.Stretch(false);

            List<DataGroup> source = null;

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    //bool login = WinAppLibrary.Utilities.SPDocument.IsLogin ? true :
                    //            await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                    //if (login)
                    //{
                    //var item = _iwpoption.SelectredIWP as RevealCommonSvc.ComboBoxDTO;
                    //if (item != null)
                    //{
                    //if (!WinAppLibrary.Utilities.SPDocument.IsLogin)
                    //    await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                    //await _schedule.GetProjectScheduleByProjectWithWBSOnMode(_projectid, _disciplineCode);
                    await _schedule.GetProjectScheduleByCwpProjectIDWithWBSOnMode(Lib.CWPDataSource.selectedCWP, _projectid);//, _disciplineCode);
                    source = _schedule.GetGroupedSchedule();
                    sc.LoadSchedule(source);

                    //}
                    //}
                    //else
                    //    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "We couldn't sign in Sharepoint Server. Please check your authentication.");
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SelectSchedule LoadSchedule", "There is a problem loading the schedule - Please try again later", "Loading Error");
            }

         //   this.DefaultViewModel["Schedules"] = source;
         //   this.gvSchedule.SelectedItem = null;
            // this.StretchingPanel.AddPanel(_scheduleDetail);
            //this.gvViewType.SelectedIndex = 0;
            Login.MasterPage.Loading(false, this);
        }

        private void gvScheduleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             if (e.AddedItems.Count > 0)
            {
                var schedule = e.AddedItems[0] as WinAppLibrary.ServiceModels.DataItem;
                Lib.ScheduleDataSource.selectedSchedule = int.Parse(schedule.UniqueId);
                Lib.ScheduleDataSource.selectedScheduleName = schedule.Title;                
                string[] scheduleDate = schedule.ImagePath.Split('~');
                Lib.ScheduleDataSource.selectedScheduleStartDate = string.IsNullOrEmpty(scheduleDate[0]) ? "" : Convert.ToDateTime(scheduleDate[0]).ToString("MM/dd/yyyy");
                Lib.ScheduleDataSource.selectedScheduleEndDate = string.IsNullOrEmpty(scheduleDate[1]) ? "" : Convert.ToDateTime(scheduleDate[1]).ToString("MM/dd/yyyy");

                this.Frame.Navigate(typeof(SubMenu));      
            }
        }

        private void sc_WrapGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var top = (LayoutRoot.RowDefinitions[1].ActualHeight - 470) / 2;
        }
        #endregion
    }
}
