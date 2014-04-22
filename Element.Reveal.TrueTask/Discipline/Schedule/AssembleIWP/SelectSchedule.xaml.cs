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
    public sealed partial class SelectSchedule : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ScheduleDataSource _schedule = new Lib.ScheduleDataSource();
        private int _projectid;
        private string _packagetypeLuid;
        private string _disciplineCode;
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
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            _packagetypeLuid = Lib.CommonDataSource.selPackageTypeLUID;
            LoadSchedule();
        }

        #region "Event Handler"

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var top = (LayoutRoot.RowDefinitions[1].ActualHeight - 470) / 2;
        }

        private async void gvSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var schedule = e.AddedItems[0] as WinAppLibrary.ServiceModels.DataItem;
                Lib.ScheduleDataSource.selectedSchedule = int.Parse(schedule.UniqueId);
                Lib.ScheduleDataSource.selectedScheduleName = schedule.Title;

                //iwplist
                if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                {
                    if (Lib.IWPDataSource.iwplist == null || _schedule.Schedule == null)
                        return;

                    List<DataLibrary.FiwpDTO> iwps = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == Lib.IWPDataSource.selectedSIWP).ToList();
                    DataLibrary.ProjectscheduleDTO psdto =  _schedule.Schedule.Where(x => x.ProjectScheduleID == Lib.ScheduleDataSource.selectedSchedule).SingleOrDefault();
                    foreach (DataLibrary.FiwpDTO dto in iwps)
                    {
                        dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                        dto.DisciplineCode = psdto.DisciplineCode;
                        dto.ProjectID = psdto.ProjectID;

                        dto.CWPID = Lib.CWPDataSource.selectedCWP;
                        dto.P6ActivityObjectID = psdto.P6ProjectObjectID;
                        dto.ProjectScheduleID = psdto.ProjectScheduleID;
                        dto.P6ParentObjectID = psdto.P6ParentObjectID;
                        dto.P6CalendarID = psdto.P6CalendarID;
                        dto.OwnerID = psdto.OwnerID;
                        dto.StartDate = psdto.StartDate;
                        dto.FinishDate = psdto.StartDate;
                        dto.UpdatedDate = DateTime.Now;
                        dto.CreatedDate = dto.CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : dto.CreatedDate;
                        dto.UpdatedBy = Login.UserAccount.UserName;
                        dto.CreatedBy = Login.UserAccount.UserName;
                    }
                    var result = await (new Lib.ServiceModel.ProjectModel()).SaveFIWP(iwps);

                    Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
                    await _iwp.GetFiwpByCwpSchedulePackageTypeOnMode(Lib.CWPDataSource.selectedCWP, 0, Lib.CommonDataSource.selPackageTypeLUID);

                    Lib.WizardDataSource.SetTargetMenu(iwps[0].DocEstablishedLUID, Lib.CommonDataSource.selPackageTypeLUID, true);

                    if (Lib.WizardDataSource.NextMenu != null)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
                    
                }
                else
                {
                    this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectIWP));
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectIWP));
            else
                this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectCWP));
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
                    //if (Lib.CommonDataSource.selPackageTypeLUID != Lib.PackageType.SIWP)
                        await _schedule.GetProjectScheduleByCwpProjectPackageTypeWithWBS(Lib.CWPDataSource.selectedCWP, _projectid, _packagetypeLuid);
                    //else
                    //{
                    //    await _schedule.GetProjectScheduleByCwpProjectIDWithWBSOnMode(0, _projectid);//, _disciplineCode);
                    //}

                    source = _schedule.GetGroupedSchedule();
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Load schedule", "There is a problem loading the schedule - Please try again later", "Loading Error");
            }

            this.DefaultViewModel["Schedules"] = source;
            this.gvSchedule.SelectedItem = null;
            Login.MasterPage.Loading(false, this);
        }

        #endregion

        

    }
}
