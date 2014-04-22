using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Extensions;

namespace Element.Reveal.Sigma.Lib
{
    public class TimeProgressDataSource
    {
        SortedObservableCollection<RevealProjectSvc.MTODTO> _components = new SortedObservableCollection<RevealProjectSvc.MTODTO>(x => x.TagNumber);
        public SortedObservableCollection<RevealProjectSvc.MTODTO> Component { get { return _components; } }
        SortedObservableCollection<RevealCommonSvc.ComboBoxDTO> _crews = new SortedObservableCollection<RevealCommonSvc.ComboBoxDTO>(x => x.DataID);
        public SortedObservableCollection<RevealCommonSvc.ComboBoxDTO> ForemanCrew { get { return _crews; } }
        
        ObservableCollection<RevealCommonSvc.ComboBoxDTO> _selectedcrews = new ObservableCollection<RevealCommonSvc.ComboBoxDTO>();

        ServiceModel.ProjectModel _projectmodel = new ServiceModel.ProjectModel();
        ServiceModel.CommonModel _commonmodel = new ServiceModel.CommonModel();

        public async Task<bool> GetComponent(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, DateTime workDate, int projectId, int moduleId)
        {
            bool retValue = false;

            try
            {
                _components.Clear();

                var result = await _projectmodel.GetComponentProgressByFIWPUncompleted(cwpId, fiwpId, materialcategoryId, ruleofcreditweightId, workDate, projectId, moduleId);

                if (result != null)
                {
                    foreach (var child in result)
                        _components.Add(child);
                   
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgressDataSource GetComponent");
            }

            return retValue;
        }

        public async Task<List<RevealProjectSvc.MTODTO>> GetComponentProgressByFIWPDone(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, DateTime workDate, int projectId, int moduleId)
        {
            List<RevealProjectSvc.MTODTO> retValue = null;

            if (cwpId > 0 && fiwpId > 0 && materialcategoryId > 0)
            {
                try
                {
                    var result = await _projectmodel.GetComponentProgressByFIWPDone(cwpId, materialcategoryId, fiwpId, projectId, moduleId, workDate, ruleofcreditweightId);
                    retValue = result;
                }
                catch (Exception e)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgressDataSource GetTimeallocationForGroup");
                }
            }

            return retValue;
        }

        public async Task<List<RevealProjectSvc.TimesheetTaskDTO>> GetTimesheetByCrewForMultiPool(int cwpId, int fiwpId, int materialcategoryId, DateTime workDate, int projectId, int moduleId)
        {
            List<RevealProjectSvc.TimesheetTaskDTO> retValue = null;

            if (cwpId > 0 && fiwpId > 0 && materialcategoryId > 0)
            {
                try
                {
                    var result = await _projectmodel.GetTimesheetByCrewForMultiPool(cwpId, fiwpId, materialcategoryId, workDate, projectId, moduleId);
                    retValue = result.TaskList;
                }
                catch (Exception e)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgressDataSource GetTimeallocationForGroup");
                }
            }

            return retValue;
        }

        public async Task<bool> GetCrewAndForemanByFiwpWorkDate_Combo(int cwpId, int fiwpId, int projectId, int moduleId, DateTime workDate)
        {
            bool retValue = false;

            if (cwpId > 0 && fiwpId > 0 && projectId > 0 && workDate > WinAppLibrary.Utilities.Helper.DateTimeMinValue)
            {
                _crews.Clear();
                _selectedcrews.Clear();

                try
                {
                    var result = await _commonmodel.GetCrewAndForemanByFiwpWorkDate_Combo(cwpId, fiwpId, projectId, moduleId, workDate);

                    if (result != null)
                    {
                        int id = result.Min(x => x.DataID);
                        var list = result.Select(x => new RevealCommonSvc.ComboBoxDTO
                                {
                                    DataID = x.DataID,
                                    DataName = x.DataName,
                                    ExtraValue1 = x.ExtraValue1,
                                    ExtraValue2 = !string.IsNullOrEmpty(x.ExtraValue2) ? x.ExtraValue2.Split('/')[1] : "",
                                    ExtraValue3 = x.DataID == id ? "Foreman" : "Crew"
                                }).ToList();
                        
                        foreach (var child in list)
                            _crews.Add(child);

                        retValue = true;
                    }
                }
                catch (Exception e)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgressDataSource GetCrewAndForemanByFiwpWorkDate_Combo");
                }
            }

            return retValue;
        }

        public void AddComponent(object item)
        {
            try
            {
                var progress = item as RevealProjectSvc.MTODTO;
                if (progress != null)
                    _components.Add(progress);
            }
            catch { }
        }

        public void AddComponent(RevealProjectSvc.MTODTO progress)
        {
            if (progress != null)
                _components.Add(progress);
        }

        public void RemoveComponent(object item)
        {
            if (item != null)
            {
                var comp = item as RevealProjectSvc.MTODTO;
                if (comp != null)
                    _components.Remove(comp);
            }
        }

        public void RemoveComponent(RevealProjectSvc.MTODTO item)
        {
            if (item != null)
                _components.Remove(item);
        }

        public void AddCrew(object item)
        {
            try
            {
                var timesheet = item as RevealProjectSvc.TimesheetDTO;
                if (timesheet != null)
                {
                    var crew = _selectedcrews.Where(x => x.DataID == timesheet.DepartStructureID).FirstOrDefault();
                    if (crew != null)
                    {
                        _selectedcrews.Remove(crew);
                        _crews.Add(crew);
                    }
                }
            }
            catch { }
        }

        public void AddCrew(RevealProjectSvc.TimesheetDTO item)
        {
            if (item != null)
            {
                var crew = _selectedcrews.Where(x => x.DataID == item.DepartStructureID).FirstOrDefault();
                if (crew != null)
                {
                    _selectedcrews.Remove(crew);
                    _crews.Add(crew);
                }
            }
        }

        public void AddCrew(RevealCommonSvc.ComboBoxDTO item)
        {
            if (item != null)
                _crews.Add(item);
        }

        public RevealProjectSvc.TimesheetDTO RemoveCrew(object item)
        {
            RevealProjectSvc.TimesheetDTO retValue = new RevealProjectSvc.TimesheetDTO();
            if (item != null)
            {
                var crew = item as RevealCommonSvc.ComboBoxDTO;
                if (crew != null)
                {
                    _crews.Remove(crew);
                    _selectedcrews.Add(crew);
                    retValue.PersonnelID = !string.IsNullOrEmpty(crew.ExtraValue2) ? Convert.ToInt32(crew.ExtraValue2) : 0;
                    retValue.DepartStructureID = crew.DataID;
                    retValue.EmployeeFullName = crew.DataName;
                }
            }

            return retValue;
        }

        public RevealProjectSvc.TimesheetDTO RemoveCrew(RevealCommonSvc.ComboBoxDTO item)
        {
            RevealProjectSvc.TimesheetDTO retValue = new RevealProjectSvc.TimesheetDTO();

            if (item != null)
            {
                _crews.Remove(item);
                _selectedcrews.Add(item);
                retValue.PersonnelID = !string.IsNullOrEmpty(item.ExtraValue2) ? Convert.ToInt32(item.ExtraValue2) : 0;
                retValue.DepartStructureID = item.DataID;
                retValue.EmployeeFullName = item.DataName;
            }

            return retValue;
        }

        public async Task<bool> SaveTimeSheet(List<RevealProjectSvc.TimesheetDTO> updates, List<RevealProjectSvc.MTODTO> progresses, decimal workhour)
        {
            bool retValue = false;

            try
            {
                await _projectmodel.SaveTimesheet(updates, progresses, workhour);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveTimeSheet");
            }

            return retValue;
        }

        public void InitiateSource()
        {
            _components.Clear();
            _crews.Clear();
        }
    }
}
