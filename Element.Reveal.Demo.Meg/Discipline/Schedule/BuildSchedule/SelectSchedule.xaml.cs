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

namespace Element.Reveal.Meg.Discipline.Schedule.BuildSchedule 
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectSchedule : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ScheduleDataSource _schedule = new Lib.ScheduleDataSource();
        private int _projectid, _moduleid;
        const double ANIMATION_SPEED =  WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbDetailON, _sbDetailOFF;
        public SelectSchedule()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {   
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            LoadSchedule();
            LoadStoryBoardSwitch();
        }

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbDetailOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbDetailOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(detailPanelTrans, Window.Current.Bounds.Width, ANIMATION_SPEED));
            _sbDetailOFF.Begin();

            _sbDetailON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbDetailON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(detailPanelScale, 1, 0));
            _sbDetailON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(detailPanelTrans, 0, ANIMATION_SPEED));
            
           
        }

        #region "Event Handler"

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var top = (LayoutRoot.RowDefinitions[1].ActualHeight - 470) / 2;
        }

        private void gvSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var schedule = e.AddedItems[0] as WinAppLibrary.ServiceModels.DataItem;
                Lib.ScheduleDataSource.selectedSchedule = int.Parse(schedule.UniqueId);
                Lib.ScheduleDataSource.selectedScheduleName = schedule.Title;                
                LoadGeneralForeman(Lib.ScheduleDataSource.selectedSchedule);

                Login.MasterPage.HideUserStatus();
                detailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _sbDetailON.Begin();
            }
        }

        private async void _scheduleDetail_btnLoadClicked(object sender, object e)
        {
            Login.MasterPage.Loading(true, this);

            try
            {                 
                var result = await (new Lib.ServiceModel.CommonModel()).GetGeneralForemanByProject_Combo(_projectid, _moduleid);

                ucScheduleDetail.LoadGeneralForemanList(result);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "_scheduleDetail_GFClicked", "There was an error load GeneralForeman. Pleae contact administrator", "Error!");
            }
            
            Login.MasterPage.Loading(false, this);
        }

        private void _scheduleDetail_btnNextClicked(object sender, object e)
        {   
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildSchedule.ComponentGrouping));
        }

        private void _scheduleDetail_btnPanelCollapseClicked(object sender, object e)
        {
            Login.MasterPage.ShowUserStatus();
            this.gvSchedule.SelectedItem = null;
            detailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbDetailOFF.Begin();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildSchedule.SelectCWP));
        }

        #endregion

        #region Private Method"

        private async void LoadSchedule()
        {
            Login.MasterPage.Loading(true, this);

            List<DataGroup> source = null;

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    //await _schedule.GetProjectScheduleByProjectWithWBSOnMode(_projectid, _moduleid);
                    await _schedule.GetProjectScheduleByCwpProjectIDWithWBSOnMode(Lib.CWPDataSource.selectedCWP, _projectid, _moduleid);
                    source = _schedule.GetGroupedSchedule();
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SelectSchedule LoadSchedule", "There was an error load schedule. Pleae contact administrator", "Error!");
            }

            this.DefaultViewModel["Schedules"] = source;
            this.gvSchedule.SelectedItem = null;
            Login.MasterPage.Loading(false, this);
        }

        public void LoadGeneralForeman(int projectScheduleId)
        {
            Login.MasterPage.Loading(true, this);

            ucScheduleDetail.btnLoadClicked += _scheduleDetail_btnLoadClicked;
            ucScheduleDetail.btnNextClicked += _scheduleDetail_btnNextClicked;
            ucScheduleDetail.btnPanelCollapseClicked += _scheduleDetail_btnPanelCollapseClicked;

            if (projectScheduleId > 0)
                ucScheduleDetail.BindScheduleDetail(projectScheduleId);

            Login.MasterPage.Loading(false, this);  
        }

        #endregion

        

    }
}
