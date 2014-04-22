using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Element.Reveal.Crew.Lib.DataSource;
using WinAppLibrary.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.Progress
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubmitTimeProgress : WinAppLibrary.Controls.LayoutAwarePage
    {
        #region "Public Method"
        public event EventHandler<object> CancelClick;
        public event EventHandler<object> Completed;

        ObservableCollection<TimesheetAndProgress> _timeprogrsses = new ObservableCollection<TimesheetAndProgress>();
        public ObservableCollection<TimesheetAndProgress> TimeProgressList { get { return _timeprogrsses; } }
        #endregion

        #region "Private Properties"

        bool _bounded = false;
        int _status = 0;
        int _departstructure = 0, _projectId = 0, _moduleId = 0;
        string _updatedBy = string.Empty;
        DateTime _workdate = DateTime.Now;
        RevealProjectSvc.SigmacueDTO _approve = new RevealProjectSvc.SigmacueDTO();

        Lib.UI.InputTemplate.OperationType OperationType;
        Lib.UI.InputTemplate _selectedgroup;
        int SelectedGroupIndex = -1;
        #endregion

        public SubmitTimeProgress()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("TimeSheet");
            Login.MasterPage.ShowBackButton = true;
        }

        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            RevealProjectSvc.SigmacueDTO param = navigationParameter as RevealProjectSvc.SigmacueDTO;
            await LoadTotalTimeSheet(param);
        }

        #region "Public Method"
        public async Task<bool> LoadTotalTimeSheet(RevealProjectSvc.SigmacueDTO param)
        {
            bool retValue = false;

            try
            {
                if (param != null)
                {
                    Login.MasterPage.Loading(true, this);
                    
                    btnSubmit.IsEnabled = true;
                    _approve = param;

                    var daily = await (new Lib.ServiceModel.ProjectModel()).GetDailytimesheetByID(_approve.DataID);

                    if (daily != null && daily.Count > 0)
                    {
                        if (daily[0].StatusLUID == WinAppLibrary.Utilities.DailyTimesheetStatus.GF_Rejected)
                            await LoadTotalTimeSheet_Reject(_approve);
                        else
                            await LoadTotalTimeSheet_Submit(_approve);
                    }
                    else
                    {
                        _status = WinAppLibrary.Utilities.TrackTimeSheetStatus.Submit;
                        this.TimeSheetTotal.SetOperationType(_status, _workdate, "Submitted by");
                    }
                     
                    Login.MasterPage.Loading(false, this);
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeSheetTotal LoadTotalTimeSheet");
            }

            return retValue;
        }
        #endregion

        #region "Event Handler"
        private async void InputTemplate_OperationClicked(object sender, object e)
        {
            var type = (Lib.UI.InputTemplate.OperationType)e;
            var selectedgroup = sender as Lib.UI.InputTemplate;

            switch (type)
            {
                case Lib.UI.InputTemplate.OperationType.Edit:
                    EditTimeSheetAndProgress(selectedgroup, type);
                    break;
                case Lib.UI.InputTemplate.OperationType.Add:
                    //OpenAddingTimeSheetAndProgress(type);
                    break;
                case Lib.UI.InputTemplate.OperationType.DeleteAll:
                    if (SelectedGroupIndex > -1)
                    {
                        if (await WinAppLibrary.Utilities.Helper.YesOrNoMessage("You can't recover once removing all data. Would you like to proceed?", "Caution!") == true)
                            DeleteTimeSheetAndProgress(SelectedGroupIndex);
                    }
                    break;
                case Lib.UI.InputTemplate.OperationType.Save:
                    if (SelectedGroupIndex > -1)
                    {
                        OperationType = type;
                        //SaveTimeSheetAndProgress(SelectedGroupIndex);
                    }
                    break;
                case Lib.UI.InputTemplate.OperationType.Cancel:
                    if (selectedgroup != null && !(selectedgroup.DataContext as TimesheetAndProgress).Updated)
                    {
                        //_datasource.RollBack(SelectedGroupIndex);
                        selectedgroup.EnableInputTemplate(false);
                        SelectedGroupIndex = -1;
                        OperationType = Lib.UI.InputTemplate.OperationType.Normal;
                    }
                    break;
            }
        }

        private void InputTemplate_DeleteItemClicked(object sender, object e)
        {
            OperationType = Lib.UI.InputTemplate.OperationType.DeleteItem;

            try
            {
                switch (sender.ToString())
                {
                    case "Component":
                        //_datasource.DeleteProgress(SelectedGroupIndex, e as RevealProjectSvc.MTODTO);
                        break;
                    case "Crew":
                        //_datasource.DeleteTimeSheet(SelectedGroupIndex, e as RevealProjectSvc.TimesheetDTO);
                        break;
                }
            }
            catch (Exception ee)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage(ee.Message, "Caution!");
            }
        }

        void TimeSheetTotal_OperationClick(object sencer, object e)
        {
            if (_bounded)
            {
                switch (e.ToString())
                {
                    case "Submit":
                        SubmitOperation();
                        break;
                    case "Approve":
                        ApproveTimeSheet(WinAppLibrary.Utilities.DailyTimesheetStatus.GF_Approved);
                        break;
                    case "Reject":
                        ApproveTimeSheet(WinAppLibrary.Utilities.DailyTimesheetStatus.GF_Rejected);
                        break;
                    case "Cancel":
                        if (CancelClick != null)
                        {
                            _status = 0;
                            CancelClick(null, null);
                        }
                        break;
                }
            }
        }

        void SubmitOperation()
        {
            if (_bounded)
            {
                switch (_status)
                {
                    case WinAppLibrary.Utilities.TrackTimeSheetStatus.Submit:
                        SubmitTimeSheet();
                        break;
                    case WinAppLibrary.Utilities.TrackTimeSheetStatus.Reject:
                        ReSubmitTimeSheet();
                        break;
                }
            }
        }
        #endregion

        #region "Private Method"
        async Task<bool> LoadTotalTimeSheet_Reject(RevealProjectSvc.SigmacueDTO param)
        {
            bool retValue = false;

            this.TimeSheetTotal.Visibility = Visibility.Collapsed;
            this.gvInputTimeProgress.Visibility = Visibility.Visible;
            this.BottomAppBar.IsEnabled = true;

            _status = WinAppLibrary.Utilities.TrackTimeSheetStatus.Reject;
            _timeprogrsses.Clear();

            var result = await (new Lib.ServiceModel.ProjectModel()).GetTimesheetAndProgressByWorkdateDepartStructure(param.SentDate, 0, param.DataID,
                                  Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, 0);
            foreach (var ts in result)
            {
                TimesheetAndProgress child = new TimesheetAndProgress();
                child.DataID = ts.DataID;
                child.Updated = ts.Updated;
                child.progresseslist = ts.progresseslist.ToObservableCollection();
                child.TimesheetList = ts.TimesheetList.ToObservableCollection();
                _timeprogrsses.Add(child);
            }

            return retValue;
        }

        async Task<bool> LoadTotalTimeSheet_Submit(RevealProjectSvc.SigmacueDTO param)
        {
            bool retValue = false;

            this.TimeSheetTotal.Visibility = Visibility.Visible;
            gvInputTimeProgress.Visibility = Visibility.Collapsed;
            this.BottomAppBar.IsEnabled = false;

            _status = WinAppLibrary.Utilities.TrackTimeSheetStatus.Approve;

            var result = await (new Lib.ServiceModel.ProjectModel()).GetTimesheetByWorkdateDailyTimeSheet(0, 0, param.DataID,
                                  param.SentDate, Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);

            result = result.OrderBy(x => x.DepartStructureID)
                                    .GroupBy(g => new { g.DepartStructureID, g.PersonnelID, g.EmployeeFullName })
                                    .Select(y => new RevealProjectSvc.TimesheetDTO
                                    {
                                        DepartStructureID = y.Key.DepartStructureID,
                                        EmployeeFullName = y.Key.EmployeeFullName,
                                        PersonnelID = y.Key.PersonnelID,
                                        StraightTime = y.Sum(z => z.StraightTime),
                                        DoubleTime = y.Sum(z => z.DoubleTime),
                                        TimeAndHalf = y.Sum(z => z.TimeAndHalf)
                                    }).ToList();

            this.TimeSheetTotal.BindTimeSheetTotal(result);
            this.TimeSheetTotal.SetOperationType(_status, _approve.SentDate, string.Format("Submitted by {0}", _approve.SentBy));

            _bounded = result.Count > 0 ? true : false;
            retValue = true;

            return retValue;
        }

        private async void EditTimeSheetAndProgress(Lib.UI.InputTemplate selectedgroup, Lib.UI.InputTemplate.OperationType type)
        {
            if (OperationType == Lib.UI.InputTemplate.OperationType.Normal)
            {
                _selectedgroup = selectedgroup;

                if (_selectedgroup != null)
                {
                    SelectedGroupIndex = (_selectedgroup.DataContext as TimesheetAndProgress).DataID;
                    _selectedgroup.EnableInputTemplate(true);
                    OperationType = type;
                }
                else
                    SelectedGroupIndex = -1;
            }
            else
            {
                bool excute = false;
                if (_selectedgroup == null)
                    excute = true;
                else if (await WinAppLibrary.Utilities.Helper.YesOrNoMessage("If you don't save previous work you will lose all of them. Would you like to proceed?", "Caution!") == true)
                {
                    //_datasource.RollBack(SelectedGroupIndex);
                    _selectedgroup.EnableInputTemplate(false);
                    excute = true;
                }

                if (excute)
                {
                    _selectedgroup = selectedgroup;
                    if (_selectedgroup != null)
                    {
                        SelectedGroupIndex = (_selectedgroup.DataContext as TimesheetAndProgress).DataID;
                        _selectedgroup.EnableInputTemplate(true);
                        OperationType = type;
                    }
                    else
                        SelectedGroupIndex = -1;
                }
            }
        }

        private void DeleteTimeSheetAndProgress(int groupindex)
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                //await _datasource.DeleteTimeSheetAndProgress(groupindex);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgress DeleteTimeSheetAndProgress",
                    "There was an error to update time and progress. \n Please contact Administrator.", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        async void SubmitTimeSheet()
        {
            try
            {
                Login.MasterPage.Loading(true, this);
                Lib.ServiceModel.ProjectModel project = new Lib.ServiceModel.ProjectModel();
                await project.SaveDailyTimehseet(_departstructure, _updatedBy, _workdate, _projectId, _moduleId);
                Initiate("Submit was successful!", "Success");
            }
            catch (Exception ee)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ee, "TimeSheetTotal SubmitTimeSheet",
                    "There was an error on submitting timesheet. Please contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        async void ReSubmitTimeSheet()
        {
            try
            {
                Login.MasterPage.Loading(true, this);
                Lib.ServiceModel.ProjectModel project = new Lib.ServiceModel.ProjectModel();
                await project.SaveDailyTimehseet(_departstructure, _updatedBy, _workdate, _projectId, _moduleId);
                Initiate("Submit was successful!", "Success");
            }
            catch (Exception ee)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ee, "TimeSheetTotal SubmitTimeSheet",
                    "There was an error on submitting timesheet. Please contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        async void ApproveTimeSheet(int dailytimesheetstatus)
        {
            try
            {
                Login.MasterPage.Loading(true, this);
                Lib.ServiceModel.ProjectModel project = new Lib.ServiceModel.ProjectModel();

                _approve.IsActive = 0;
                _approve.StatusID = dailytimesheetstatus;
                _approve.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;

                await project.SaveSigmacue(new List<RevealProjectSvc.SigmacueDTO>() { _approve }, _approve.DataID, WinAppLibrary.Utilities.SigmaCueTaskType.Timesheet);
                Initiate(dailytimesheetstatus == WinAppLibrary.Utilities.DailyTimesheetStatus.GF_Rejected ? "Reject was sent" : "Approve was successful!", "Success");
                _status = 0;
                if (Completed != null)
                    Completed(WinAppLibrary.Utilities.TrackTimeSheetStatus.Approve, dailytimesheetstatus);
            }
            catch (Exception ee)
            {
                string msg = dailytimesheetstatus == WinAppLibrary.Utilities.DailyTimesheetStatus.GF_Rejected ? "rejecting" : "approving";

                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ee, "TimeSheetTotal ApproveTimeSheet",
                    "There was an error on " + msg + " timesheet. Please contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        void Initiate(string msg, string title)
        {
            _timeprogrsses.Clear();
            this.TimeSheetTotal.Initiate();
            WinAppLibrary.Utilities.Helper.SimpleMessage(msg, title);
        }
        #endregion
    }
}
