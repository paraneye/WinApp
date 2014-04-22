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

namespace Element.Reveal.TrueTask.Discipline.Schedule.BuildHydro 
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectHydro : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ScheduleDataSource _schedule = new Lib.ScheduleDataSource();
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.IWPDataSource _siwp = new Lib.IWPDataSource();
       // Lib.UI.ScheduleDetail _scheduleDetail = new Lib.UI.ScheduleDetail();
        private int _projectid; private string _disciplineCode;
        const double ANIMATION_SPEED =  WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbDetailON, _sbDetailOFF;
        public SelectHydro()
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
            GetTestType();
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
                LoadIWP(Lib.ScheduleDataSource.selectedSchedule);
                GetForemanList(Lib.ScheduleDataSource.selectedGeneralForeman);

                Login.MasterPage.HideUserStatus();
            }
        }

        private async void _scheduleDetail_btnLoadClicked(object sender, object e)
        {
            if (Lib.IWPDataSource.selectedHydro != null && Lib.IWPDataSource.selectedHydro > 0)
            {
                await _siwp.GetHydroByIDOnMode(Lib.IWPDataSource.selectedHydro);
                List<DataLibrary.FiwpDTO> siwps = _siwp.GetHydroByID();

                if (siwps.Count > 0)
                {
                    DataLibrary.FiwpDTO siwp = siwps[0];
                    txtIwpTitle.Text = "Edit<" + siwp.FiwpName + ">";
                    txtUDescription.Text = string.IsNullOrEmpty(siwp.Description) ? string.Empty : siwp.Description;
                    txtStartDate.Text = siwp.StartDate == null ? string.Empty : siwp.StartDate.ToString("MMM dd, yyyy");
                    txtEndDate.Text = siwp.FinishDate == null ? string.Empty : siwp.FinishDate.ToString("MMM dd, yyyy");
                    txtAssignedCrews.Text = siwp.CrewMembersAssigned.ToString();
                    lvUForeman.ItemsSource = _siwp.GetForemanByGeneralForeman();
                    txtTotalManhours.Text = siwp.TotalManhours.ToString();
                    foreach (DataLibrary.ComboCodeBoxDTO dto in lvUForeman.Items)
                    {
                        if (dto.DataID == siwp.LeaderId)
                        {
                            lvUForeman.SelectedValue = dto;
                            break;
                        }
                    }
                    cbUTestType.ItemsSource = _siwp.GetTestType();

                    foreach (DataLibrary.LookupDTO dto in cbUTestType.Items)
                    {
                        if (dto.LookupID == siwp.TestTypeLUID)
                        {
                            cbUTestType.SelectedValue = dto;
                            break;
                        }
                    }

                    grUpdateIWP.Visibility = Visibility.Visible;
                }
            }
            else
            {
                await _siwp.GetHydroTestByProjectScheduleOnMode(Lib.ScheduleDataSource.selectedSchedule);

                if (Lib.IWPDataSource.hydros.Count > 0)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "Please Select Hydro Test Work Package.");
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "There is no Hydro Test Work Package data. Please Create Hydro Test Work Package.");
                }
            }
        }

        private async void _scheduleDetail_btnNextClicked(object sender, object e)
        {
            if (Lib.IWPDataSource.selectedHydro != null && Lib.IWPDataSource.selectedHydro > 0)
            {
                this.Frame.Navigate(typeof(Discipline.Schedule.BuildHydro.ComponentGrouping));
            }
            else
            {
                await _siwp.GetHydroTestByProjectScheduleOnMode(Lib.ScheduleDataSource.selectedSchedule);
                if (Lib.IWPDataSource.hydros.Count > 0)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "Please Select Hydro Test Work Package.");
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "There is no Hydro Test Work Package data. Please Create Hydro Test Work Package.");
                }
            }
        }

        private void _iwpDetail_btnNewIWPClicked(object sender, object e)
        {
            txtIWPName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            grNewIWP.Visibility = Visibility.Visible;
            lvNForeman.ItemsSource = _siwp.GetForemanByGeneralForeman();
            cbNTestType.ItemsSource = _siwp.GetTestType();
            foreach (DataLibrary.LookupDTO dto in cbNTestType.Items)
            {
                cbNTestType.SelectedValue = dto;
                break;
            }
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
            this.Frame.Navigate(typeof(MainMenu));
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

                    //cbCWP.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo_Mobile(_projectid, _disciplineCode);

                    //foreach (DataLibrary.ComboBoxDTO dto in cbCWP.Items)
                    //{
                    //    if (dto.DataID == Lib.CWPDataSource.selectedCWP)
                    //    {
                    //        cbCWP.SelectedValue = dto;
                    //        txtCWP.Text = dto.DataName;
                    //        break;
                    //    }
                    //}

                    //cbCWP.IsEnabled = false;
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
            this.gvSchedule.SelectedItem = null;

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
            //Login.MasterPage.Loading(true, _iwp);
            //await _siwp.GetForemanByGeneralForemanOnMode(generalForemanId, _projectid, _disciplineCode);
            //await _siwp.GetForemanByGeneralForemanOnMode(generalForemanId, WinAppLibrary.Utilities.SigmaRoleType.GeneralForeman, _projectid.ToString());
            await _siwp.GetForemanByGeneralForemanOnMode(DataLibrary.Utilities.ROLE_TYPE.ROLE_FOREMAN, generalForemanId, DataLibrary.Utilities.ROLE_TYPE.ROLE_GENERALFOREMAN, _projectid.ToString());
            //Login.MasterPage.Loading(false, _iwp);
        }


        public async void GetTestType()
        {
            await _siwp.GetTestTypeOnMode();
        }

        #endregion

        private async void btnNSave_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            List<DataLibrary.FiwpDTO> iwplist = new List<DataLibrary.FiwpDTO>();
            List<DataLibrary.FiwpDTO> newFiwps = new List<DataLibrary.FiwpDTO>();
            DataLibrary.FiwpDTO newFiwp = new DataLibrary.FiwpDTO();
            DataLibrary.ProjectscheduleDTO psdto = _commonsource.ProjectSchedule.Where(x => x.ProjectScheduleID == Lib.ScheduleDataSource.selectedSchedule).SingleOrDefault();

            newFiwp.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
            newFiwp.DisciplineCode = psdto.DisciplineCode; 
            newFiwp.ProjectID = psdto.ProjectID; 
            if (lvNForeman.SelectedItems.Count > 0)
            {
                newFiwp.LeaderId = (lvNForeman.SelectedItems[0] as DataLibrary.ComboCodeBoxDTO).DataID;
            }

            newFiwp.CWPID = psdto.CWPID;
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
            newFiwp.TestTypeLUID = (cbNTestType.SelectedValue as DataLibrary.LookupDTO).LookupID;

            newFiwp.PackageTypeLUID = Lib.PackageType.HydroTest;


            newFiwps.Add(newFiwp);

            var result = await _siwp.SaveHydro(newFiwps);


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

            List<DataLibrary.FiwpDTO> iwps = _siwp.GetHydroByID();

            if (iwps.Count > 0)
            {
                if (lvUForeman.SelectedItems.Count > 0)
                    iwps[0].LeaderId = (lvUForeman.SelectedItems[0] as DataLibrary.ComboCodeBoxDTO).DataID;
                else
                    iwps[0].LeaderId = string.Empty;

                iwps[0].TestTypeLUID = (cbUTestType.SelectedValue as DataLibrary.LookupDTO).LookupID;

                iwps[0].Description = txtUDescription.Text;
                iwps[0].UpdatedBy = Login.UserAccount.UserName;
                iwps[0].UpdatedDate = DateTime.Now;
                iwps[0].CreatedDate = iwps[0].CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : iwps[0].CreatedDate;
                iwps[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
            }

            var result = await _siwp.SaveHydro(iwps);

            LoadIWP(Lib.ScheduleDataSource.selectedSchedule);

            Login.MasterPage.Loading(false, this);
            grUpdateIWP.Visibility = Visibility.Collapsed;
        }

        private void btnUCancel_Click(object sender, RoutedEventArgs e)
        {
            grUpdateIWP.Visibility = Visibility.Collapsed;
        }

        

    }
}
