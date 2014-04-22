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

namespace Element.Reveal.TrueTask.Discipline.Schedule.BuildCSU
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectCSU : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ScheduleDataSource _schedule = new Lib.ScheduleDataSource();
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.IWPDataSource _siwp = new Lib.IWPDataSource();
       // Lib.UI.ScheduleDetail _scheduleDetail = new Lib.UI.ScheduleDetail();
        private int _projectid; private string _disciplineCode;
        const double ANIMATION_SPEED =  WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbDetailON, _sbDetailOFF;
        public SelectCSU()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {   
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID; 
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadSystem();
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

        private void gvSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                detailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _sbDetailON.Begin();

                var system = e.AddedItems[0] as DataLibrary.SystemDTO;
                Lib.CommonDataSource.selectedSystemID = system.SystemID;
                Lib.CommonDataSource.selectedSystemName = system.SystemName;
                LoadCSU(Lib.CommonDataSource.selectedSystemID);

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
                    txtIwpTitle.Text = "Edit CSU Work Package";
                    txtuTitle.Text = siwp.FiwpName;
                    
                    grUpdateIWP.Visibility = Visibility.Visible;
                }
            }
            else
            {
                var source = await (new Lib.ServiceModel.ProjectModel()).GetFiwpBySystemPackageType(_projectid, Lib.CommonDataSource.selectedSystemID, Lib.PackageType.CSU);
                if (source.Count > 0)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "Please Select CSU Work Package.");
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "There is no CSU Work Package data. Please Create CSU Work Package.");
                }
            }
        }

        private async void _scheduleDetail_btnNextClicked(object sender, object e)
        {
            if (Lib.IWPDataSource.selectedHydro != null && Lib.IWPDataSource.selectedHydro > 0)
            {
                this.Frame.Navigate(typeof(Discipline.Schedule.BuildCSU.SelectDrawing));
            }
            else
            {
                var source = await (new Lib.ServiceModel.ProjectModel()).GetFiwpBySystemPackageType(_projectid, Lib.CommonDataSource.selectedSystemID, Lib.PackageType.CSU);
                if (source.Count > 0)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "Please Select CSU Work Package.");
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "There is no CSU Work Package data. Please Create CSU Work Package.");
                }
            }
        }

        private void _iwpDetail_btnNewIWPClicked(object sender, object e)
        {
            txtTitle.Text = string.Empty;
            grNewIWP.Visibility = Visibility.Visible;
        }
        

        private void _scheduleDetail_btnPanelCollapseClicked(object sender, object e)
        {
            Login.MasterPage.ShowUserStatus();
            this.gvSystem.SelectedItem = null;
            detailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbDetailOFF.Begin();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        #endregion

        #region Private Method"

        private async void LoadSystem()
        {
            Login.MasterPage.Loading(true, this);
            
            ucIWPDetail.btnLoadClicked += _scheduleDetail_btnLoadClicked;
            ucIWPDetail.btnNextClicked += _scheduleDetail_btnNextClicked;
            ucIWPDetail.btnPanelCollapseClicked += _scheduleDetail_btnPanelCollapseClicked;
            ucIWPDetail.btnNewIWPClicked += _iwpDetail_btnNewIWPClicked;

            List<DataLibrary.SystemDTO> source = new List<DataLibrary.SystemDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    List<DataLibrary.SystemDTO> list =  await (new Lib.ServiceModel.ProjectModel()).GetSystemByProjectID(_projectid);
                    source = list;
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SelectSchedule LoadSchedule", "There was an error load System. Pleae contact administrator", "Error!");
            }

            this.DefaultViewModel["Systems"] = source;
            this.gvSystem.SelectedItem = null;

            Login.MasterPage.Loading(false, this);
        }

        public async void LoadCSU(int systemId)
        {
            
            bool result;

            if (systemId > 0)
            {
                result = await ucIWPDetail.BindIWPDetail(_projectid, systemId);
            }
        }


        #endregion

        private async void btnNSave_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            List<DataLibrary.FiwpDTO> newFiwps = new List<DataLibrary.FiwpDTO>();
            DataLibrary.FiwpDTO newFiwp = new DataLibrary.FiwpDTO();

            newFiwp.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
            newFiwp.DisciplineCode = _disciplineCode;
            newFiwp.ProjectID = _projectid;
            newFiwp.P6ActivityID = "0";
            newFiwp.StartDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;
            newFiwp.FinishDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;
            newFiwp.FiwpName = txtTitle.Text;
            newFiwp.UpdatedBy = Login.UserAccount.UserName;
            newFiwp.UpdatedDate = DateTime.Now;
            newFiwp.CreatedBy = Login.UserAccount.UserName;
            newFiwp.CreatedDate = DateTime.Now;

            newFiwp.SystemID = Lib.CommonDataSource.selectedSystemID;
            newFiwp.PackageTypeLUID = Lib.PackageType.CSU;

            newFiwps.Add(newFiwp);

            var result = await _siwp.SaveSIWP(newFiwps);


            LoadCSU(Lib.CommonDataSource.selectedSystemID);

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
                iwps[0].FiwpName = txtuTitle.Text;
                iwps[0].UpdatedBy = Login.UserAccount.UserName;
                iwps[0].UpdatedDate = DateTime.Now;
                iwps[0].CreatedDate = iwps[0].CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : iwps[0].CreatedDate;
                iwps[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
            }

            var result = await _siwp.SaveSIWP(iwps);

            LoadCSU(Lib.CommonDataSource.selectedSystemID);

            Login.MasterPage.Loading(false, this);
            grUpdateIWP.Visibility = Visibility.Collapsed;
        }

        private void btnUCancel_Click(object sender, RoutedEventArgs e)
        {
            grUpdateIWP.Visibility = Visibility.Collapsed;
        }

        

    }
}
