using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

namespace Element.Reveal.TrueTask.Lib
{
    public class ObjectParam
    {
        public List<string> taskCategoryCodeList { get; set; }
        public List<string> taskCategoryIdList { get; set; }
        public List<int> systemIdList { get; set; }
        public List<int> typeLUIdList { get; set; }
        public List<int> drawingtypeLUIdList { get; set; }
        public List<int> costcodeIdList { get; set; }
        public List<string> taskTypeLUIDList { get; set; }
        public List<string> progressIDList { get; set; }
        public List<string> materialIDList { get; set; }
        public List<string> rfinumberList { get; set; }
        public string searchcolumn { get; set; }
        public string searhValue { get; set; }
        public List<string> searchvalueList { get; set; }
        public List<string> isolinenoList { get; set; }
        public string fiwpName { get; set; }
        public string cwpName { get; set; }
        public int projectscheduleId { get; set; }
        public List<DataLibrary.ComboBoxDTO> moduleList { get; set; }
        public List<DataLibrary.ComboBoxDTO> pidList { get; set; }
        public List<DataLibrary.ComboBoxDTO> processsystemList { get; set; }
        public List<DataLibrary.ComboBoxDTO> lineList { get; set; }
        
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
        private List<DataLibrary.ComboBoxDTO> _drawing = new List<DataLibrary.ComboBoxDTO>();
        public List<DataLibrary.ComboBoxDTO> Drawing { get { return _drawing; } }
        private List<DataLibrary.MTODTO> _unassignedcomponent = new List<DataLibrary.MTODTO>();
        private List<DataLibrary.MTODTO> _assignedcomponent = new List<DataLibrary.MTODTO>();
        private List<DataLibrary.ProjectscheduleDTO> _projectschedule = new List<DataLibrary.ProjectscheduleDTO>();
        public List<DataLibrary.ProjectscheduleDTO> ProjectSchedule { get { return _projectschedule; } }
        private List<DataLibrary.CollectionDTO> _collection = new List<DataLibrary.CollectionDTO>();
        private List<DataLibrary.CollectionDTO> _collectionforsiwp = new List<DataLibrary.CollectionDTO>();
        public List<DataLibrary.CollectionDTO> orgCollection { get { return _collection; } }
        public List<DataLibrary.CollectionDTO> orgCollectionforsiwp { get { return _collectionforsiwp; } }
        public static List<DataLibrary.CollectionDTO> curCollection { get; set; }
        public static List<DataLibrary.CollectionDTO> selectedTaskCategory { get; set; }
        public static List<DataLibrary.CollectionDTO> selectedComponentType { get; set; }
        public static List<DataLibrary.CollectionDTO> selectedSystem { get; set; }
        public static List<DataLibrary.CollectionDTO> selectedDrawingType { get; set; }
        public static List<DataLibrary.CollectionDTO> selectedCostCode { get; set; }
        public static int selectedMaterialCategory { get; set; }
        private List<DataLibrary.ComboCodeBoxDTO> _uomlist = new List<DataLibrary.ComboCodeBoxDTO>();


        public static List<string> selTaskCategoryCodeList { get; set; }
        public static List<string> selTaskCategoryIDList { get; set; }
        public static List<int> selComponentTypeIDList { get; set; }
        public static List<int> selSystemIDList { get; set; }
        public static List<int> selDrawingTypeIDList { get; set; }
        public static List<int> selCostCodeIDList { get; set; }
        public static List<string> selTaskTypeIDList { get; set; }
        public static List<string> selMaterialIDList { get; set; }
        public static List<string> selProgressIDList { get; set; }

        public static List<string> selModuleList { get; set; }
        public static List<string> selPIDList { get; set; }
        public static List<string> selProcessSystemList { get; set; }
        public static List<string> selLineList { get; set; }
        
        public static int selectedDrawing { get; set; }
        public static string selPackageTypeLUID { get; set; }
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
        //todo 노석진:서비스 확정되지 않아 원본 주석처리 서비스 나오면 삭제 예정
        /*
       public async Task<bool> GetDrawingOnMode(int cwpId, int drawingId,
                                                                   List<string> taskCategoryCodeList, List<string> taskCategoryIdList,
                                                                   List<int> systemIdList, List<int> typeLUIdList,
                                                                   List<int> drawingtypeLUIdList, List<int> costcodeIdList,
                                                                   List<DataLibrary.ComboCodeBoxDTO> searchstringList, List<DataLibrary.ComboCodeBoxDTO> compsearchstringList,
                                                                   List<DataLibrary.ComboCodeBoxDTO> locationList, List<string> rfinumberList,
                                                                   string searchcolumn, List<string> searchvalueList,
                                                                   int projectId, string disciplineCode, int grouppage)
       {
           bool retValue = false;
           try
           {
               var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForSchedulingWithList(cwpId, drawingId, taskCategoryCodeList, taskCategoryIdList, 
                   systemIdList, typeLUIdList, drawingtypeLUIdList, costcodeIdList, searchstringList, compsearchstringList, locationList, rfinumberList, searchcolumn, searchvalueList, 
                   projectId, disciplineCode, grouppage);

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
       }*/

        public async Task<bool> GetDrawingOnMode(int cwpId, int drawingId,
                                                                   List<string> taskCategoryIdList, List<string> taskTypeLUIDList,
                                                                   List<string> materialIDList, List<string> progressIDList,
                                                                   string searchValue, int projectId, string disciplineCode, int grouppage)
       {
           bool retValue = false;
           try
           {
               var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForSchedulingWithList(cwpId, drawingId, taskCategoryIdList, taskTypeLUIDList,
                   materialIDList, progressIDList, searchValue, projectId, disciplineCode, grouppage);

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
        //todo 노석진:서비스 확정되지 않아 원본 주석처리 서비스 나오면 삭제 예정
        /*
        public async Task<bool> GetDrawingForAssignIWPOnMode(int cwpId, int projectscheduleId, int drawingId,
                                                                   List<string> taskCategoryCodeList, List<string> taskCategoryIdList,
                                                                   List<int> systemIdList, List<int> typeLUIdList,
                                                                   List<int> drawingtypeLUIdList, List<int> costcodeIdList,
                                                                   List<DataLibrary.ComboCodeBoxDTO> searchstringList, List<DataLibrary.ComboCodeBoxDTO> compsearchstringList,
                                                                   List<DataLibrary.ComboCodeBoxDTO> locationList, List<string> rfinumberList,
                                                                   string searchcolumn, List<string> searchvalueList,
                                                                   int projectId, string disciplineCode, int grouppage)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForFIWPWithList(cwpId, projectscheduleId, drawingId, taskCategoryCodeList, taskCategoryIdList,
                    systemIdList, typeLUIdList, drawingtypeLUIdList, costcodeIdList, searchstringList, compsearchstringList, locationList, rfinumberList, searchcolumn, searchvalueList,
                    projectId, disciplineCode, grouppage);

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
        }*/

        public async Task<bool> GetDrawingForAssignIWPOnMode(int cwpId, int projectscheduleId, int drawingId,
                                                                   List<string> taskCategoryIdList, List<string> taskTypeLUIDList,
                                                                   List<string> materialIDList, List<string> progressIDList,
                                                                   string searchValue, int projectId, string disciplineCode, int grouppage)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForFIWPWithList(cwpId, projectscheduleId, drawingId, taskCategoryIdList, taskTypeLUIDList,
                    materialIDList, progressIDList, searchValue, projectId, disciplineCode, grouppage);

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

        public async Task<bool> GetDrawingForCSUOnMode(int drawingtypeLuid, int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetDrawingByDrawingType_Combo(drawingtypeLuid, projectId, disciplineCode);

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
                                                                 List<DataLibrary.ComboBoxDTO> searchstringList, List<DataLibrary.ComboBoxDTO> locationList, 
                                                                 string searchcolumn, List<string> searchvalueList, int projectId, string disciplineCode, int grouppage)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForHydroSchedulingWithList(drawingId, systemIdList, projectscheduleIdList,
                    costcodeIdList, isolinenoList, searchstringList, locationList, searchcolumn, searchvalueList, projectId, disciplineCode, grouppage);

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
                                                                List<DataLibrary.ComboBoxDTO> matrsearchstringList, List<DataLibrary.ComboBoxDTO> matrsearchstringList2,
                                                                List<DataLibrary.ComboBoxDTO> compsearchstringList, List<DataLibrary.ComboBoxDTO> pnidsearchstringList,
                                                                List<int> systemIdList, List<int> projectscheduleIdList, List<int> costcodeIdList,
                                                                List<DataLibrary.ComboBoxDTO> locationList, string searchcolumn, List<string> searchvalueList,
                                                                int projectId, string disciplineCode, int grouppage)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressForHydroSchedulingWithListApps(cwpId, drawingId, matrsearchstringList, matrsearchstringList2,
                compsearchstringList, pnidsearchstringList, systemIdList, projectscheduleIdList, costcodeIdList, locationList, searchcolumn, searchvalueList, projectId, disciplineCode, grouppage);

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

        public async Task<bool> GetComponentProgressByFIWP(int fiwpId, int projectScheduleID, int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressByFIWP(fiwpId, projectScheduleID, projectId, disciplineCode);
                _assignedcomponent = result;
               
                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetComponentProgressBySIWP(int fiwpId, int projectScheduleID, int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressBySIWP(fiwpId, projectScheduleID, projectId, disciplineCode);
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
                    new DataItem(x.DataID.ToString(), x.DataName, x.ExtraValue1.Replace("\\", "/").Replace("//", "/").Replace("http:","http:/") , "", group) { }).ToObservableCollection();
            }
            grouplist.Add(group);

            return grouplist;
        }

        public List<DataLibrary.MTODTO> GetGroupedUnAssignedComponent(int drawingId)
        {
            List<DataLibrary.MTODTO> list = new List<DataLibrary.MTODTO>();
            list = _unassignedcomponent.Where((x => x.DrawingID == drawingId && x.ProjectScheduleID == 0)).ToList();
            _unassignedmanhour = list.Sum(x => x.ManhoursEstimate);
            _unassignedcnt = list.Count;
            return list;
        }

        public List<DataLibrary.MTODTO> GetGroupedUnAssignedComponent(int drawingId, int iwpId)
        {
            List<DataLibrary.MTODTO> list = new List<DataLibrary.MTODTO>();
            if (_unassignedcomponent != null)
            {
                list = _unassignedcomponent.Where((x => x.DrawingID == drawingId && x.FIWPID == 0)).ToList();
                _unassignedmanhour = list.Sum(x => x.ManhoursEstimate);
                _unassignedcnt = list.Count;
            }
            else
            {
                _unassignedmanhour = 0;
                _unassignedcnt = 0;
            }
            return list;
        }


        public List<DataLibrary.MTODTO> GetGroupedAssignedComponent()
        {
            List<DataLibrary.MTODTO> list = new List<DataLibrary.MTODTO>();

            if (_assignedcomponent != null)
            {
                list = _assignedcomponent;

                _assignedmanhour = list.Sum(x => x.ManhoursEstimate);
                _assignedcnt = list.Count;
            }
            else
            {
                _assignedmanhour = 0;
                _assignedcnt = 0;

            }
            return list;
        }

        public async Task<bool> GetProjectScheduleAllByProjectIDdisciplineCode(int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleAllByProjectIDdisciplineCode(projectId, disciplineCode);
                _projectschedule = result;

                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetAvailableCollectionForScheduling(int cwpId, int projectscheduleId, int projectId, string disciplineCode, int iwpId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetAvailableCollectionForScheduling(cwpId, projectscheduleId, projectId, disciplineCode, iwpId);
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

        public async Task<bool> GetAvailableCollectionForHydroScheduling(int cwpId, int projectscheduleId, int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetAvailableCollectionForHydroScheduling(cwpId, projectscheduleId, projectId, disciplineCode);
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

        public async Task<bool> GetAvailableCollectionForSchedulingApp(int cwpId, int projectscheduleId, int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetAvailableCollectionForSchedulingApp(cwpId, projectscheduleId, projectId, disciplineCode);
                _collectionforsiwp = result;

                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }


        public async Task<bool> UpdateProjectScheduleAssignment(DataLibrary.ProgressAssignment progress)
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

        public async Task<bool> UpdateFIWPProgressAssignment(DataLibrary.ProgressAssignment progress)
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

        public async Task<bool> UpdateSIWPProgressAssignmentByScope(DataLibrary.ProgressAssignment progress, 
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

        public async Task<bool> UpdateHydroProgressAssignmentByStartPoint(DataLibrary.ProgressAssignment progress, int drawingId)
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

        public async Task<DataLibrary.ProjectscheduleDTO> UpdateProjectSchedulePeriod(DataLibrary.ProjectscheduleDTO psc, string totalManhours)
        {
            DataLibrary.ProjectscheduleDTO dto = new DataLibrary.ProjectscheduleDTO();
            dto = await (new Lib.ServiceModel.ProjectModel()).UpdateProjectSchedulePeriod(psc, totalManhours);
            return dto;
        }


        

        public List<DataLibrary.ComboBoxDTO> GetSortingItems()
        {

            List<DataLibrary.ComboBoxDTO> list = new List<DataLibrary.ComboBoxDTO>();
            //list.Add(new DataLibrary.ComboBoxDTO() { DataName = "Reveal Tag Number", ExtraValue1 = "" });
            list.Add(new DataLibrary.ComboBoxDTO() { DataName = "Man Hours", ExtraValue1 = "" });
            list.Add(new DataLibrary.ComboBoxDTO() { DataName = "Task Type", ExtraValue1 = "" });
            list.Add(new DataLibrary.ComboBoxDTO() { DataName = "Progress Type", ExtraValue1 = "" });

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

        public List<DataLibrary.ComboCodeBoxDTO> GetUOM()
        {
            DataLibrary.ComboCodeBoxDTO dto = new DataLibrary.ComboCodeBoxDTO();
            List<DataLibrary.ComboCodeBoxDTO> rslt = new List<DataLibrary.ComboCodeBoxDTO>();
            rslt.Add(dto);
            rslt.AddRange(_uomlist);
            return rslt;
        }
    }
}
