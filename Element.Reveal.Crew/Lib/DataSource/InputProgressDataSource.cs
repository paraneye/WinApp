using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;

namespace Element.Reveal.Crew.Lib.DataSource
{
    class InputProgressDataSource
    {
        #region "Properties"
        static SortedObservableCollection<TimesheetAndProgress> _timesheetprogress = new SortedObservableCollection<TimesheetAndProgress>(x => x.DataID);
        public static SortedObservableCollection<TimesheetAndProgress> TimpeSheetProgress { get { return _timesheetprogress; } }

        List<RevealProjectSvc.TimesheetDTO> _del_timesheet = new List<RevealProjectSvc.TimesheetDTO>();
        List<RevealProjectSvc.MTODTO> _del_progress = new List<RevealProjectSvc.MTODTO>();
        int _timeallocationId = -1;
        #endregion

        public async Task<bool> LoadTimeSheetAndProgress(DateTime workdate, int departstructId, int dailytimesheetId, int projectId, int moduleId)
        {
            bool retValue = false;

            try
            {
                _timesheetprogress.Clear();
                var result = await (new Lib.ServiceModel.ProjectModel()).
                    GetTimesheetAndProgressByWorkdateDepartStructure(workdate, departstructId, dailytimesheetId, projectId, moduleId, 0);

                foreach (var item in result)
                {
                    TimesheetAndProgress child = new TimesheetAndProgress();
                    child.DataID = item.DataID;
                    child.Updated = item.Updated;
                    child.progresseslist = item.progresseslist.ToObservableCollection();
                    child.TimesheetList = item.TimesheetList.ToObservableCollection();

                    _timesheetprogress.Add(child);
                }

                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "InputProgressDataSource LoadTimeSheetAndProgress");
            }

            return retValue;
        }

        public void AddTimeSheetAndProgress(int groupId, List<RevealProjectSvc.MTODTO> components, List<RevealProjectSvc.TimesheetDTO> crews)
        {
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                foreach (var progress in components)
                {
                    progress.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                    timeprogress.progresseslist.Add(progress);
                }

                foreach (var timesheet in crews)
                {
                    timesheet.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                    timeprogress.TimesheetList.Add(timesheet);
                }

            }
        }

        public List<RevealProjectSvc.MTODTO> GetAddingComponents(int groupId, List<RevealProjectSvc.MTODTO> originList)
        {
            List<RevealProjectSvc.MTODTO> retValue = new List<RevealProjectSvc.MTODTO>();
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                var ids = timeprogress.progresseslist.Select(x => x.ProgressID).ToList();
                retValue = originList.Where(x => !ids.Contains(x.ProgressID)).ToList();
            }

            return retValue;
        }

        public List<RevealCommonSvc.ComboBoxDTO> GetAddingCrews(int groupId, List<RevealCommonSvc.ComboBoxDTO> originList)
        {
            List<RevealCommonSvc.ComboBoxDTO> retValue = new List<RevealCommonSvc.ComboBoxDTO>();
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                var ids = timeprogress.TimesheetList.Select(x => x.DepartStructureID);
                retValue = originList.Where(x => !ids.Contains(x.DataID)).ToList();
            }

            return retValue;
        }

        public bool IsContainInComponents(int groupId, RevealProjectSvc.MTODTO item)
        {
            bool retValue = false;
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                retValue = timeprogress.progresseslist.Contains(item);
            }

            return retValue;
        }

        public bool IsContainInCrew(int groupId, int departstructureId)
        {
            bool retValue = false;
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                var item = timeprogress.TimesheetList.Where(x => x.DepartStructureID == departstructureId).FirstOrDefault();

                if (item != null)
                    retValue = true;
            }

            return retValue;
        }

        public void Insert(int index, TimesheetAndProgress item)
        {
            if (index != -1 && _timesheetprogress.Count > index - 1)
                _timesheetprogress.Insert(index, item);
            
        }

        public async Task<bool> DeleteTimeSheetAndProgress(int groupId)
        {
            bool retValue = false;
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                if (timeprogress.DataID > 0)
                {
                    List<RevealProjectSvc.MTODTO> components = new List<RevealProjectSvc.MTODTO>();
                    List<RevealProjectSvc.TimesheetDTO> timesheets = new List<RevealProjectSvc.TimesheetDTO>();

                    foreach (var progress in timeprogress.progresseslist)
                    {
                        if (progress.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.New)
                        {
                            progress.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                            components.Add(progress);
                        }
                    }

                    foreach (var progress in _del_progress)
                    {
                        _del_progress.Remove(progress);
                        components.Add(progress);
                    }

                    foreach (var timesheet in timeprogress.TimesheetList)
                    {
                        if (timesheet.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.New)
                        {
                            timesheet.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                            timesheets.Add(timesheet);
                        }
                    }

                    foreach (var timesheet in _del_timesheet)
                    {
                        _del_timesheet.Remove(timesheet);
                        timesheets.Add(timesheet);
                    }

                    await (new Lib.ServiceModel.ProjectModel()).SaveTimesheet(timesheets, components, 0, timeprogress.DataID);
                }

                _timesheetprogress.Remove(timeprogress);
                retValue = true;
            }

            return retValue;
        }

        public void DeleteTimeSheet(int groupId, RevealProjectSvc.TimesheetDTO timesheet)
        {
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                var curItem = timeprogress.TimesheetList.Where(x => x.TimeSheetID > 0 && x.DepartStructureID != timesheet.DepartStructureID).FirstOrDefault();

                if (timeprogress.DataID > 0 && curItem == null)
                    throw new Exception("Timesheet can't be removed all before updating them to server. \n If you wanna remove all items please select 'Delete All'");
                else
                {
                    bool result = timeprogress.TimesheetList.Remove(timesheet);

                    //This is because DTO can not be possibly not same generically as a child of enormerous verctor even though it has same date.
                    if (!result)
                    {
                        var child = timeprogress.TimesheetList.Where(x => x.DepartStructureID == timesheet.DepartStructureID).FirstOrDefault();

                        if (child != null)
                            result = timeprogress.TimesheetList.Remove(child);
                    }

                    if (timeprogress.DataID > 0 && result && timesheet.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.New)
                    {
                        timesheet.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                        _del_timesheet.Add(timesheet);
                    }
                }
            }
        }

        public void DeleteProgress(int groupId, RevealProjectSvc.MTODTO progress)
        {
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                var curItem = timeprogress.progresseslist.Where(x => x.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.New && x.ProgressID != progress.ProgressID).FirstOrDefault();

                if (timeprogress.DataID > 0 && curItem == null)
                    throw new Exception("Progress can't be removed all before updating them to server. \n If you wanna remove all items please select 'Delete All'");
                else
                {
                    bool result = timeprogress.progresseslist.Remove(progress);

                    //This is because DTO can not be possibly not same generically  as a child of enormerous verctor even though it has same date.
                    if (!result)
                    {
                        var child = timeprogress.progresseslist.Where(x => x.ProgressID == progress.ProgressID).FirstOrDefault();

                        if (child != null)
                            result = timeprogress.progresseslist.Remove(child);
                    }

                    if (timeprogress.DataID > 0 && result && progress.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Update)
                    {
                        progress.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                        _del_progress.Add(progress);
                    }
                }
            }
        }

        public void RollBack(int groupId)
        {
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                for (int i = 0; i < timeprogress.TimesheetList.Count;)
                {
                    switch (timeprogress.TimesheetList[i].DTOStatus)
                    {
                        case (int)WinAppLibrary.Utilities.RowStatus.New:
                            timeprogress.TimesheetList.Remove(timeprogress.TimesheetList[i]);
                            break;
                        case (int)WinAppLibrary.Utilities.RowStatus.Update:
                            timeprogress.TimesheetList[i].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                            i++;
                            break;
                        default:
                            i++;
                            break;
                    }
                }

                for (int i = 0; i < timeprogress.progresseslist.Count; )
                {
                    switch (timeprogress.progresseslist[i].DTOStatus)
                    {
                        case (int)WinAppLibrary.Utilities.RowStatus.New:
                            timeprogress.progresseslist.Remove(timeprogress.progresseslist[i]);
                            break;
                        case (int)WinAppLibrary.Utilities.RowStatus.Update:
                            timeprogress.progresseslist[i].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                            i++; 
                            break;
                        default:
                            i++;
                            break;
                    }
                }

                foreach (var timesheet in _del_timesheet)
                {
                    timesheet.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                    _del_timesheet.Remove(timesheet);
                    timeprogress.TimesheetList.Add(timesheet);
                }

                foreach (var progress in _del_progress)
                {
                    progress.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                    _del_progress.Remove(progress);
                    timeprogress.progresseslist.Add(progress);
                }
            }
        }

        public void InitiateSource()
        {
            _timesheetprogress.Clear();
            _del_progress.Clear();
            _del_timesheet.Clear();
            _timeallocationId = -1;
        }

        public async Task<bool> SaveTimeSheet(int groupId, int iwpId, int rulecreditId, int materialcategoryId, int projectId, int moduleId, DateTime date, string createdBy)
        {
            bool retValue = false;
            var timeprogress = _timesheetprogress.Where(x => x.DataID == groupId).FirstOrDefault();

            if (timeprogress != null)
            {
                decimal ManhoursEstimate = 0;
                List<RevealProjectSvc.MTODTO> components = new List<RevealProjectSvc.MTODTO>();
                List<RevealProjectSvc.TimesheetDTO> timesheets = new List<RevealProjectSvc.TimesheetDTO>();

                switch (groupId)
                {
                    case 0:

                        foreach (var item in timeprogress.TimesheetList)
                        {
                            if (item.DoubleTime + item.TimeAndHalf + item.StraightTime <= 0)
                                throw new NotImplementedException("Please check " + item.EmployeeFullName + "'s working time");

                            item.RuleOFCreditWeightID = rulecreditId;
                            item.FiwpID = iwpId;
                            item.MaterialCategoryID = materialcategoryId;
                            item.WorkDate = date;
                            item.ProjectID = projectId;
                            item.ModuleID = moduleId;
                            item.IsComplete = 1;
                            item.AmountInstalled = 0;
                            item.TimeAllocationID = -1;
                            item.CreatedBy = createdBy;
                            item.CreatedDate = date;
                            item.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;

                            timesheets.Add(item);

                        }

                        foreach (var item in timeprogress.progresseslist)
                        {
                            item.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                            ManhoursEstimate += item.ManhoursEstimate * item.InstalledQtyRatio;
                            components.Add(item);
                        }

                        _timeallocationId = -1;
                        break;
                    default:
                        foreach (var item in timeprogress.TimesheetList)
                        {
                            if (item.DoubleTime + item.TimeAndHalf + item.StraightTime <= 0)
                                throw new NotImplementedException("Please check " + item.EmployeeFullName + "'s working time");

                            if (item.DTOStatus == (int)WinAppLibrary.Utilities.RowStatus.New)
                            {
                                item.RuleOFCreditWeightID = rulecreditId;
                                item.FiwpID = iwpId;
                                item.MaterialCategoryID = materialcategoryId;
                                item.WorkDate = date;
                                item.ProjectID = projectId;
                                item.ModuleID = moduleId;
                                item.IsComplete = 1;
                                item.AmountInstalled = 0;
                                item.CreatedBy = createdBy;
                                item.CreatedDate = date;
                                item.TimeAllocationID = timeprogress.DataID;
                            }

                            timesheets.Add(item);
                        }

                        foreach (var timesheet in _del_timesheet)
                        {
                            _del_timesheet.Remove(timesheet);
                            timesheets.Add(timesheet);
                        }

                        foreach (var item in timeprogress.progresseslist)
                        {
                            if (item.DTOStatus == (int)WinAppLibrary.Utilities.RowStatus.New)
                                item.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;

                            ManhoursEstimate += item.ManhoursEstimate * item.InstalledQtyRatio;
                            components.Add(item);
                        }

                        foreach (var progress in _del_progress)
                        {
                            _del_progress.Remove(progress);
                            components.Add(progress);
                        }
                        break;
                }

                await (new Lib.ServiceModel.ProjectModel()).SaveTimesheet(timesheets, components, 0, _timeallocationId);
                retValue = true;
            }

            return retValue;
        }

        public async Task<bool> ReloadTimeSheetProgress(DateTime workdate, int departstructId, int dailytimesheetId, int projectId, int moduleId)
        {
            bool retValue = false;

            _timesheetprogress.Clear();
            var result = await (new Lib.ServiceModel.ProjectModel()).
                GetTimesheetAndProgressByWorkdateDepartStructure(workdate, departstructId, dailytimesheetId, projectId, moduleId, 0);

            foreach (var item in result)
            {
                TimesheetAndProgress child = new TimesheetAndProgress();
                child.DataID = item.DataID;
                child.Updated = item.Updated;
                child.progresseslist = item.progresseslist.ToObservableCollection();
                child.TimesheetList = item.TimesheetList.ToObservableCollection();

                _timesheetprogress.Add(child);
            }

            _timeallocationId = -1;
            return retValue;
        }

        public async Task<List<RevealProjectSvc.TimesheetDTO>> LoadTotalTimeSheet(int departstructureId, DateTime workDate, int projectId, int moduleId)
        {
            List<RevealProjectSvc.TimesheetDTO> retValue = new List<RevealProjectSvc.TimesheetDTO>();

            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetTimesheetByWorkdateCostcodeDepartstructure(0, departstructureId, 0,
                    workDate, projectId, moduleId, 0);

                retValue = result.OrderBy(x => x.DepartStructureID)
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
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "InputProgressDataSource LoadTotalTimeSheet");
            }

            return retValue;
        }
    }

    public class TimesheetAndProgress
    {
        public int DataID { get; set; }
        public bool Updated { get; set; }
        public ObservableCollection<RevealProjectSvc.TimesheetDTO> TimesheetList { get; set; }
        public ObservableCollection<RevealProjectSvc.MTODTO> progresseslist { get; set; }
    }
}
