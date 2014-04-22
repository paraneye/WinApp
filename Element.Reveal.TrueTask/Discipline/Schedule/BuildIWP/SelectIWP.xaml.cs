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

namespace Element.Reveal.TrueTask.Discipline.Schedule.BuildIWP 
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectIWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ScheduleDataSource _schedule = new Lib.ScheduleDataSource();
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
       // Lib.UI.ScheduleDetail _scheduleDetail = new Lib.UI.ScheduleDetail();
        private int _projectid; private string _disciplineCode;
        const double ANIMATION_SPEED =  WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbDetailON, _sbDetailOFF;
        public SelectIWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {   
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
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
            //ScrollViewer.Padding = new Thickness(0, top, 0, 0);
        }

        private void gvSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                //Login.MasterPage.Loading(true, this);

                detailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _sbDetailON.Begin();

                var schedule = e.AddedItems[0] as WinAppLibrary.ServiceModels.DataItem;
                Lib.ScheduleDataSource.selectedSchedule = int.Parse(schedule.UniqueId);
                Lib.ScheduleDataSource.selectedGeneralForeman = schedule.Content;
                Lib.ScheduleDataSource.selectedScheduleName = schedule.Title;                
                LoadIWP(Lib.ScheduleDataSource.selectedSchedule);
                GetForemanList(Lib.ScheduleDataSource.selectedGeneralForeman);

                Login.MasterPage.HideUserStatus();
            }
        }

        private async void _scheduleDetail_btnLoadClicked(object sender, object e)
        {
            if (Lib.IWPDataSource.selectedIWP > 0)
            {
                await _iwp.GetFiwpByIDOnMode(Lib.IWPDataSource.selectedIWP);
                List<DataLibrary.FiwpDTO> iwps = _iwp.GetFiwpByID();
                if (iwps == null)
                    return;

                if (iwps.Count > 0)
                {
                    DataLibrary.FiwpDTO iwp = iwps[0];
                   // txtIwpTitle.Text = "Edit <" + iwp.FiwpName + ">";
                    txtUIWPName.Text = iwp.FiwpName;
                    txtUDescription.Text = string.IsNullOrEmpty(iwp.Description) ? string.Empty : iwp.Description;
                    txtStartDate.Text = iwp.StartDate == null ? string.Empty : iwp.StartDate.ToString("MM/dd/yyyy");
                    txtEndDate.Text = iwp.FinishDate == null ? string.Empty : iwp.FinishDate.ToString("MM/dd/yyyy");
                    txtAssignedCrews.Text = iwp.CrewMembersAssigned.ToString();
                    lvUForeman.ItemsSource = _iwp.GetForemanByGeneralForeman();
                    txtTotalManhours.Text = iwp.TotalManhours.ToString();
                    foreach (DataLibrary.ComboCodeBoxDTO dto in lvUForeman.Items)
                    {
                        if (dto.DataID == iwp.LeaderId)
                        {
                            lvUForeman.SelectedValue = dto;
                            break;
                        }
                    }
                    grUpdateIWP.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (Lib.IWPDataSource.iwps != null && Lib.IWPDataSource.iwps.Count > 0)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Please select Installation Work Package first", "Warning");
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Please create Installation Work Package first", "Warning");
                }
            }
        }

        private void _scheduleDetail_btnNextClicked(object sender, object e)
        {
            if (Lib.IWPDataSource.selectedIWP > 0)
            {
                this.Frame.Navigate(typeof(Discipline.Schedule.BuildIWP.ComponentGrouping));
            }
            else
            {
                if (Lib.IWPDataSource.iwps != null && Lib.IWPDataSource.iwps.Count > 0)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Please select Installation Work Package first", "Warning");
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Please create Installation Work Package first", "Warning");
                }
            }
        }

        private void _iwpDetail_btnNewIWPClicked(object sender, object e)
        {
            txtIWPName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            grNewIWP.Visibility = Visibility.Visible;
            lvNForeman.ItemsSource = _iwp.GetForemanByGeneralForeman();
        }

        private void ImageF_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var source = (sender as Image).Source as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            source.UriSource = new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "/Assets/common_default_foreman.png");
        }
        

        private void _scheduleDetail_btnPanelCollapseClicked(object sender, object e)
        {
            Login.MasterPage.ShowUserStatus();
           // this.gvSchedule.SelectedItem = null;
            sc.gvScheduleClear();
            detailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbDetailOFF.Begin();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildIWP.SelectCWP));
        }

        #endregion

        #region Private Method"

        private async void LoadSchedule()
        {
            Login.MasterPage.Loading(true, this);
            //StretchingPanel.Stretch(false);

            ucIWPDetail.btnLoadClicked += _scheduleDetail_btnLoadClicked;
            ucIWPDetail.btnNextClicked += _scheduleDetail_btnNextClicked;
            ucIWPDetail.btnPanelCollapseClicked += _scheduleDetail_btnPanelCollapseClicked;
            ucIWPDetail.btnNewIWPClicked += _iwpDetail_btnNewIWPClicked;

            List<DataGroup> source = null;

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _commonsource.GetProjectScheduleAllByProjectIDdisciplineCode(_projectid, _disciplineCode);
                    //await _schedule.GetProjectScheduleByProjectWithWBSOnMode(_projectid, _disciplineCode);
                    await _schedule.GetProjectScheduleByCwpProjectIDWithWBSOnMode(Lib.CWPDataSource.selectedCWP, _projectid);//, _disciplineCode);
                    source = _schedule.GetGroupedSchedule();
                    sc.LoadSchedule(source);

                    cbCWP.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo_Mobile(_projectid, _disciplineCode, Login.UserAccount.LoginName);

                    foreach (DataLibrary.ComboBoxDTO dto in cbCWP.Items)
                    {
                        if (dto.DataID == Lib.CWPDataSource.selectedCWP)
                        {
                            cbCWP.SelectedValue = dto;
                            txtCWP.Text = dto.DataName;
                            break;
                        }
                    }

                    cbCWP.IsEnabled = false;
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SelectSchedule LoadSchedule", "There is a problem loading the schedule - Please try again later", "Loading Error");
            }

            this.DefaultViewModel["Schedules"] = source;
           // this.gvSchedule.SelectedItem = null;
           

            Login.MasterPage.Loading(false, this);
        }

        public async void LoadIWP(int projectScheduleId)
        {
            
            bool result;

            if (projectScheduleId > 0)
            {
                //Login.MasterPage.Loading(true, ucIWPDetail);
                result = await ucIWPDetail.BindIWPDetail(projectScheduleId);
                //Login.MasterPage.Loading(false, ucIWPDetail);
            }       
        }



        public async void GetForemanList(string generalForemanId)
        {
            //Login.MasterPage.Loading(true, _iwp);string sigmaUserId, string sigmaRoleId, string projectId
            //await _iwp.GetForemanByGeneralForemanOnMode(generalForemanId, WinAppLibrary.Utilities.SigmaRoleType.GeneralForeman, _projectid.ToString());
            await _iwp.GetForemanByGeneralForemanOnMode(DataLibrary.Utilities.ROLE_TYPE.ROLE_FOREMAN, generalForemanId, DataLibrary.Utilities.ROLE_TYPE.ROLE_GENERALFOREMAN, _projectid.ToString());
            //Login.MasterPage.Loading(false, _iwp);
        }

        #endregion

        private async void btnNSave_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            List<DataLibrary.FiwpDTO> iwplist = new List<DataLibrary.FiwpDTO>();
            List<DataLibrary.FiwpDTO> newFiwps = new List<DataLibrary.FiwpDTO>();
            DataLibrary.FiwpDTO newFiwp = new DataLibrary.FiwpDTO();

            if (string.IsNullOrEmpty(txtIWPName.Text.Trim()))
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please Input IWP name.", "Warning");
                Login.MasterPage.Loading(false, this);
                return;
            }

            if (_commonsource.ProjectSchedule == null)
            {
                Login.MasterPage.Loading(false, this);
                return;
            }
            DataLibrary.ProjectscheduleDTO psdto = _commonsource.ProjectSchedule.Where(x => x.ProjectScheduleID == Lib.ScheduleDataSource.selectedSchedule).SingleOrDefault();

            newFiwp.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
            newFiwp.DisciplineCode = psdto.DisciplineCode;
            newFiwp.ProjectID = psdto.ProjectID;

            if (lvNForeman.Items.Count > 0 && lvNForeman.SelectedItems.Count == 0)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Foreman is not selected. Please assign a foreman", "Warning");
                Login.MasterPage.Loading(false, this);
                return;
            }
            if (lvNForeman.SelectedItems.Count > 0)
                newFiwp.LeaderId = (lvNForeman.SelectedItems[0] as DataLibrary.ComboCodeBoxDTO).DataID;

            newFiwp.ExternalScheduleId = psdto.ExternalScheduleId;
            newFiwp.CWPID = Lib.CWPDataSource.selectedCWP;
            newFiwp.P6ActivityObjectID = psdto.P6ProjectObjectID;
            newFiwp.ProjectScheduleID = psdto.ProjectScheduleID;
            newFiwp.P6ParentObjectID = psdto.P6ParentObjectID;
            newFiwp.P6CalendarID = psdto.P6CalendarID;
            newFiwp.OwnerID = psdto.OwnerID;
            newFiwp.StartDate = psdto.StartDate;
            newFiwp.FinishDate = psdto.StartDate;
            newFiwp.FiwpName = txtIWPName.Text;
            newFiwp.Description = txtDescription.Text;
            newFiwp.P6RemainingDuration = 0;
            newFiwp.TotalManhours = 0;
            newFiwp.UpdatedBy = Login.UserAccount.UserName;
            newFiwp.UpdatedDate = DateTime.Now;
            newFiwp.CreatedBy = Login.UserAccount.UserName;
            newFiwp.CreatedDate = DateTime.Now;

            newFiwp.PackageTypeLUID = Lib.PackageType.FIWP;


            newFiwps.Add(newFiwp);
           
            var result = await _iwp.SaveFIWP(newFiwps);


            LoadIWP(Lib.ScheduleDataSource.selectedSchedule);

            Login.MasterPage.Loading(false, this);
            grNewIWP.Visibility = Visibility.Collapsed;
        }

        private void btnNCancel_Click(object sender, RoutedEventArgs e)
        {
            grNewIWP.Visibility = Visibility.Collapsed;
        }

        private async void btnUSave_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            List<DataLibrary.FiwpDTO> iwps = _iwp.GetFiwpByID();
            if (iwps == null)
            {
                Login.MasterPage.Loading(false, this);
                return;
            }

            if (string.IsNullOrEmpty(txtUIWPName.Text.Trim()))
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please Input IWP name.", "Warning");
                Login.MasterPage.Loading(false, this);
                return;
            }

            if (iwps.Count > 0)
            {
                if (lvUForeman.Items.Count > 0 && lvUForeman.SelectedItems.Count ==0 )
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Foreman is not selected. Please assign a foreman", "Warning");
                    Login.MasterPage.Loading(false, this);
                    return;
                }
                else if (lvUForeman.SelectedItems.Count > 0)
                    iwps[0].LeaderId = (lvUForeman.SelectedItems[0] as DataLibrary.ComboCodeBoxDTO).DataID;
                else
                    iwps[0].LeaderId = string.Empty;

                iwps[0].FiwpName = txtUIWPName.Text;
                iwps[0].Description = txtUDescription.Text;
                iwps[0].UpdatedBy = Login.UserAccount.UserName;
                iwps[0].UpdatedDate = DateTime.Now;
                iwps[0].CreatedDate = iwps[0].CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : iwps[0].CreatedDate;
                iwps[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
            }

            var result = await _iwp.SaveFIWP(iwps);

            LoadIWP(Lib.ScheduleDataSource.selectedSchedule);

            Login.MasterPage.Loading(false, this);
            grUpdateIWP.Visibility = Visibility.Collapsed;
        }

        private void btnUCancel_Click(object sender, RoutedEventArgs e)
        {
            grUpdateIWP.Visibility = Visibility.Collapsed;
        }



        private void gvScheduleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {
                //Login.MasterPage.Loading(true, this);

                detailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _sbDetailON.Begin();

                var schedule = e.AddedItems[0] as WinAppLibrary.ServiceModels.DataItem;
                Lib.ScheduleDataSource.selectedSchedule = int.Parse(schedule.UniqueId);
                Lib.ScheduleDataSource.selectedGeneralForeman = schedule.Content;
                Lib.ScheduleDataSource.selectedScheduleName = schedule.Title;
                LoadIWP(Lib.ScheduleDataSource.selectedSchedule);
                GetForemanList(Lib.ScheduleDataSource.selectedGeneralForeman);

                Login.MasterPage.HideUserStatus();
            }
        }

        private void sc_WrapGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var top = (LayoutRoot.RowDefinitions[1].ActualHeight - 470) / 2;
        }   

    }
}
