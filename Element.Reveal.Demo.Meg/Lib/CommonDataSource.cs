using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Meg.Lib
{
    public class ObjectParam
    {
        public List<int> materialCategoryIdList { get; set; }
        public List<int> taskCategoryIdList { get; set; }
        public List<int> systemIdList { get; set; }
        public List<int> typeLUIdList { get; set; }
        public List<int> drawingtypeLUIdList { get; set; }
        public List<int> costcodeIdList { get; set; }
        public List<RevealProjectSvc.ComboBoxDTO> searchstringList { get; set; }
        public List<RevealProjectSvc.ComboBoxDTO> compsearchstringList { get; set; }
        public List<RevealProjectSvc.ComboBoxDTO> locationList { get; set; }
        public List<string> rfinumberList { get; set; }
        public string searchcolumn { get; set; }
        public List<string> searchvalueList { get; set; }
        public List<string> isolinenoList { get; set; }
        public string fiwpName { get; set; }
        public string cwpName { get; set; }
        public int projectscheduleId { get; set; }
        public List<RevealProjectSvc.ComboBoxDTO> moduleList { get; set; }
        public List<RevealProjectSvc.ComboBoxDTO> pidList { get; set; }
        public List<RevealProjectSvc.ComboBoxDTO> processsystemList { get; set; }
        public List<RevealProjectSvc.ComboBoxDTO> lineList { get; set; }
    }
    public class LibTemplate
    {
        public string PartNumber { get; set; }
        public string UOM { get; set; }
        public string Description { get; set; }
        public decimal Manhours { get; set; }
    }
    public class CommonDataSource
    {
        private List<RevealProjectSvc.ComboBoxDTO> _drawing = new List<RevealProjectSvc.ComboBoxDTO>();
        public List<RevealProjectSvc.ComboBoxDTO> Drawing { get { return _drawing; } }
        private List<RevealProjectSvc.MTODTO> _unassignedcomponent = new List<RevealProjectSvc.MTODTO>();
        private List<RevealProjectSvc.MTODTO> _assignedcomponent = new List<RevealProjectSvc.MTODTO>();
        private List<RevealProjectSvc.ProjectscheduleDTO> _projectschedule = new List<RevealProjectSvc.ProjectscheduleDTO>();
        public List<RevealProjectSvc.ProjectscheduleDTO> ProjectSchedule { get { return _projectschedule; } }
        private List<RevealProjectSvc.CollectionDTO> _collection = new List<RevealProjectSvc.CollectionDTO>();
        private List<RevealProjectSvc.CollectionDTO> _collectionforsiwp = new List<RevealProjectSvc.CollectionDTO>();
        public List<RevealProjectSvc.CollectionDTO> orgCollection { get { return _collection; } }
        public List<RevealProjectSvc.CollectionDTO> orgCollectionforsiwp { get { return _collectionforsiwp; } }
        public static List<RevealProjectSvc.CollectionDTO> curCollection { get; set; }
        public static List<RevealProjectSvc.CollectionDTO> selectedTaskCategory { get; set; }
        public static List<RevealProjectSvc.CollectionDTO> selectedComponentType { get; set; }
        public static List<RevealProjectSvc.CollectionDTO> selectedSystem { get; set; }
        public static List<RevealProjectSvc.CollectionDTO> selectedDrawingType { get; set; }
        public static List<RevealProjectSvc.CollectionDTO> selectedCostCode { get; set; }
        public static int selectedMaterialCategory { get; set; }
        private List<RevealCommonSvc.ComboBoxDTO> _uomlist = new List<RevealCommonSvc.ComboBoxDTO>();
        

        public static List<int> selMaterialCategoryIDList { get; set; }
        public static List<int> selTaskCategoryIDList { get; set; }
        public static List<int> selComponentTypeIDList { get; set; }
        public static List<int> selSystemIDList { get; set; }
        public static List<int> selDrawingTypeIDList { get; set; }
        public static List<int> selCostCodeIDList { get; set; }

        public static List<string> selModuleList { get; set; }
        public static List<string> selPIDList { get; set; }
        public static List<string> selProcessSystemList { get; set; }
        public static List<string> selLineList { get; set; }
        
        public static int selectedDrawing { get; set; }
        public static int selPackageTypeLUID { get; set; }
        private decimal _unassignedmanhour = 0;
        private decimal _assignedmanhour = 0;
        private int _unassignedcnt = 0;
        private int _assignedcnt = 0;

        public decimal UnAssignedManhour { get { return _unassignedmanhour; } }
        public decimal AssignedManhour { get { return _assignedmanhour; } }
        public int UnAssignedCnt { get { return _unassignedcnt; } }
        public int AssignedCnt { get { return _assignedcnt; } }

        public static int startPoint { get; set; }
        public static int endPoint { get; set; }
        public static int startDrawingID { get; set; }
        public static int endDrawingID { get; set; }

        public static int selectedSystemID { get; set; }
        public static string selectedSystemName { get; set; }

        public async Task<bool> GetDrawingOnMode(int cwpId, int drawingId,
                                                                    List<int> materialCategoryIdList, List<int> taskCategoryIdList,
                                                                    List<int> systemIdList, List<int> typeLUIdList,
                                                                    List<int> drawingtypeLUIdList, List<int> costcodeIdList,
                                                                    List<RevealProjectSvc.ComboBoxDTO> searchstringList, List<RevealProjectSvc.ComboBoxDTO> compsearchstringList,
                                                                    List<RevealProjectSvc.ComboBoxDTO> locationList, List<string> rfinumberList,
                                                                    string searchcolumn, List<string> searchvalueList,
                                                                    int projectId, int moduleId, int grouppage)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForSchedulingWithList(cwpId, drawingId, materialCategoryIdList, taskCategoryIdList, 
                    systemIdList, typeLUIdList, drawingtypeLUIdList, costcodeIdList, searchstringList, compsearchstringList, locationList, rfinumberList, searchcolumn, searchvalueList, 
                    projectId, moduleId, grouppage);

                _unassignedcomponent = result.mto;
                
                _drawing = result.drawing;

                if (_drawing != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetDrawingForAssignIWPOnMode(int cwpId, int projectscheduleId, int drawingId,
                                                                   List<int> materialCategoryIdList, List<int> taskCategoryIdList,
                                                                   List<int> systemIdList, List<int> typeLUIdList,
                                                                   List<int> drawingtypeLUIdList, List<int> costcodeIdList,
                                                                   List<RevealProjectSvc.ComboBoxDTO> searchstringList, List<RevealProjectSvc.ComboBoxDTO> compsearchstringList,
                                                                   List<RevealProjectSvc.ComboBoxDTO> locationList, List<string> rfinumberList,
                                                                   string searchcolumn, List<string> searchvalueList,
                                                                   int projectId, int moduleId, int grouppage)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForFIWPWithList(cwpId, projectscheduleId, drawingId, materialCategoryIdList, taskCategoryIdList,
                    systemIdList, typeLUIdList, drawingtypeLUIdList, costcodeIdList, searchstringList, compsearchstringList, locationList, rfinumberList, searchcolumn, searchvalueList,
                    projectId, moduleId, grouppage);

                _unassignedcomponent = result.mto;

                _drawing = result.drawing;

                if (_drawing != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetDrawingForCSUOnMode(int drawingtypeLuid, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetDrawingByDrawingType_Combo(drawingtypeLuid, projectId, moduleId);

                _drawing = result;

                if (_drawing != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }


        public async Task<bool> GetDrawingForAssignSIWPOnMode(int drawingId, List<int> systemIdList, List<int> projectscheduleIdList, List<int> costcodeIdList, List<string> isolinenoList,
                                                                 List<RevealProjectSvc.ComboBoxDTO> searchstringList, List<RevealProjectSvc.ComboBoxDTO> locationList, 
                                                                 string searchcolumn, List<string> searchvalueList, int projectId, int moduleId, int grouppage)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForHydroSchedulingWithList(drawingId, systemIdList, projectscheduleIdList,
                    costcodeIdList, isolinenoList, searchstringList, locationList, searchcolumn, searchvalueList, projectId, moduleId, grouppage);

                //idf seq (integervar1) asend sort
                _unassignedcomponent = result.mto.OrderBy(x=> x.IntegerVar1).ToList();

                //drawingnumber asend sort
                _drawing = result.drawing.OrderBy(x=> x.DataName).ToList();

                if (_drawing != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetDrawingForAssignHydroOnMode(int cwpId, int drawingId,
                                                                List<RevealProjectSvc.ComboBoxDTO> matrsearchstringList, List<RevealProjectSvc.ComboBoxDTO> matrsearchstringList2,
                                                                List<RevealProjectSvc.ComboBoxDTO> compsearchstringList, List<RevealProjectSvc.ComboBoxDTO> pnidsearchstringList,
                                                                List<int> systemIdList, List<int> projectscheduleIdList, List<int> costcodeIdList,
                                                                List<RevealProjectSvc.ComboBoxDTO> locationList, string searchcolumn, List<string> searchvalueList,
                                                                int projectId, int moduleId, int grouppage)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForHydroSchedulingWithListApps(cwpId, drawingId, matrsearchstringList, matrsearchstringList2,
                compsearchstringList, pnidsearchstringList, systemIdList, projectscheduleIdList, costcodeIdList, locationList, searchcolumn, searchvalueList, projectId, moduleId, grouppage);

                _unassignedcomponent = result.mto.ToList();

                _drawing = result.drawing != null ? result.drawing.ToList() : null;

                if (_drawing != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetComponentProgressByFIWP(int fiwpId, int projectScheduleID, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressByFIWP(fiwpId, projectScheduleID, projectId, moduleId);
                _assignedcomponent = result;
               
                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetComponentProgressBySIWP(int fiwpId, int projectScheduleID, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressBySIWP(fiwpId, projectScheduleID, projectId, moduleId);
                _assignedcomponent = result;

                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public List<DataGroup> GetGroupedDrawing()
        {
            List<DataGroup> grouplist = new List<DataGroup>();
            DataGroup group;
            group = new DataGroup("Drawing", "", "");
            if (_drawing !=null && _drawing.Count > 0)
            {
                group.Items = _drawing.Select(x =>
                    new DataItem(x.DataID.ToString(), x.DataName,
                        Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.Drawing + "/" + x.ExtraValue2, "", group) { }).ToObservableCollection();
            }
            grouplist.Add(group);

            return grouplist;
        }

        public List<RevealProjectSvc.MTODTO> GetGroupedUnAssignedComponent(int drawingId)
        {
            List<RevealProjectSvc.MTODTO> list = new List<RevealProjectSvc.MTODTO>();

            list = _unassignedcomponent.Where(x => x.DrawingID == drawingId).ToList();

            _unassignedmanhour = list.Sum(x => x.ManhoursEstimate);
            _unassignedcnt = list.Count;
            return list;
        }

        public List<RevealProjectSvc.MTODTO> GetGroupedAssignedComponent()
        {
            List<RevealProjectSvc.MTODTO> list = new List<RevealProjectSvc.MTODTO>();

            list = _assignedcomponent;

            _assignedmanhour = list.Sum(x => x.ManhoursEstimate);
            _assignedcnt = list.Count;
            return list;
        }

        public async Task<bool> GetProjectScheduleAllByProjectIDModuleID(int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleAllByProjectIDModuleID(projectId, moduleId);
                _projectschedule = result;

                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetAvailableCollectionForScheduling(int cwpId, int projectscheduleId, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetAvailableCollectionForScheduling(cwpId, projectscheduleId, projectId, moduleId);
                _collection = result;
                _collectionforsiwp = result;

                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetAvailableCollectionForHydroScheduling(int cwpId, int projectscheduleId, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetAvailableCollectionForHydroScheduling(cwpId, projectscheduleId, projectId, moduleId);
                _collection = result;
                _collectionforsiwp = result;

                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetAvailableCollectionForSchedulingApp(int cwpId, int projectscheduleId, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetAvailableCollectionForSchedulingApp(cwpId, projectscheduleId, projectId, moduleId);
                _collectionforsiwp = result;

                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }


        public async Task<bool> UpdateProjectScheduleAssignment(RevealProjectSvc.ProgressAssignment progress)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).UpdateProjectScheduleAssignment(progress);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "UpdateProjectScheduleAssignment");
            }

            return retValue;
        }

        public async Task<bool> UpdateFIWPProgressAssignment(RevealProjectSvc.ProgressAssignment progress)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).UpdateFIWPProgressAssignment(progress);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "UpdateFIWPProgressAssignment");
            }

            return retValue;
        }

        public async Task<bool> UpdateSIWPProgressAssignmentByScope(RevealProjectSvc.ProgressAssignment progress, 
            int startdrawingId, int enddrawingId, int startidfseq, int endidfseq, List<int> withindrawingList)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).UpdateSIWPProgressAssignmentByScope(progress, startdrawingId, enddrawingId, startidfseq, endidfseq, withindrawingList);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "UpdateSIWPProgressAssignmentByScope");
            }

            return retValue;
        }

        public async Task<bool> UpdateHydroProgressAssignmentByStartPoint(RevealProjectSvc.ProgressAssignment progress, int drawingId)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).UpdateHydroProgressAssignmentByStartPoint(progress, drawingId);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "UpdateHydroProgressAssignmentByStartPoint");
            }

            return retValue;
        }


        

        public List<RevealCommonSvc.ComboBoxDTO> GetSortingItems()
        {

            List<RevealCommonSvc.ComboBoxDTO> list = new List<RevealCommonSvc.ComboBoxDTO>();
            list.Add(new RevealCommonSvc.ComboBoxDTO() { DataName = "Reveal Tag Number", ExtraValue1 = "" });
            list.Add(new RevealCommonSvc.ComboBoxDTO() { DataName = "Man Hours", ExtraValue1 = "" });
            list.Add(new RevealCommonSvc.ComboBoxDTO() { DataName = "Task Type", ExtraValue1 = "" });
            list.Add(new RevealCommonSvc.ComboBoxDTO() { DataName = "Progress Type", ExtraValue1 = "" });

            return list;
        }

        public async Task<bool> GetUOMOnMode()
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.CommonModel()).GetUOM_Combo();
                _uomlist = result.ToList();

                if (_uomlist != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public List<RevealCommonSvc.ComboBoxDTO> GetUOM()
        {
            RevealCommonSvc.ComboBoxDTO dto = new RevealCommonSvc.ComboBoxDTO();
            List<RevealCommonSvc.ComboBoxDTO> rslt = new List<RevealCommonSvc.ComboBoxDTO>();
            rslt.Add(dto);
            rslt.AddRange(_uomlist);
            return rslt;
        }
    }
}
