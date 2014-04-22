using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Meg.Lib.ServiceModel
{
    public class ProjectModel
    {
        Helper _helper = new Helper();

        #region "FiwpManonsite"
        public async Task<List<RevealProjectSvc.FiwpmanonsiteDTO>> GetFiwpManonsiteByForeman(int foremanStructureId, DateTime workdate)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpManonsiteByForemanAsync(Helper.DBInstance, foremanStructureId, workdate);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.FiwpmanonsiteDTO>> SaveFiwpManonsite(List<RevealProjectSvc.FiwpmanonsiteDTO> fiwpmaonsite)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpManonsiteAsync(Helper.DBInstance, fiwpmaonsite);
            await project.CloseAsync();
            return retValue.ToList();
        }
        #endregion

        #region "Project"

        public async Task<List<RevealProjectSvc.DocumentnoteDTO>> GetDocumentNoteByFiwpDocumentDrawing(int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDocumentNoteByFiwpDocumentDrawingAsync(Helper.DBInstance, fiwpId, 0, 0, projectId, moduleId);
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.DocumentmarkupDTO>> GetDocumentmarkupByDrawingPersonnel(int drawingId, int personnelId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDocumentmarkupByDrawingPersonnelAsync(Helper.DBInstance, drawingId, personnelId);
            return retValue.ToList();
        }

        public async Task<RevealProjectSvc.DocumentmarkupDTO> SaveDocumentmarkupWithSharePoint(int projectId, RevealProjectSvc.DocumentmarkupDTO documentmarkup)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveDocumentmarkupWithSharePointAsync(Helper.DBInstance, projectId, documentmarkup);
            return retValue;
        }

        public async Task<RevealProjectSvc.DrawingPageTotal> GetDrawingForDrawingViewer(int projectId, List<int> cwpIds, List<int> fiwpIds, List<int> drawingtypes, string enTag, string title, string sortoption, int curpage)
        {
            RevealProjectSvc.DrawingPageTotal retValue = new RevealProjectSvc.DrawingPageTotal();
            try
            {
                RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
                retValue = await project.GetDrawingForDrawingViewerAsync(Helper.DBInstance, projectId, cwpIds.ToList(), fiwpIds.ToList(), drawingtypes.ToList(), enTag, title, sortoption, curpage);
                await project.CloseAsync();
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetDrawingForDrawingViewer");
            }
            return retValue;
        }

        public async Task<List<RevealProjectSvc.ProjectDTO>> GetAllProject()
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectAllAsync(Helper.DBInstance);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<RevealProjectSvc.ProjectDTO> GetProjectByID(int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectByIdAsync(Helper.DBInstance, projectId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.ProjectmoduleDTO>> GetAllProjectModule()
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetAllProjectAndModulesAsync(Helper.DBInstance);
            await project.CloseAsync();
            return retValue.ToList();
        }
        #endregion

        #region "IWP"

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetSafetyDocumentsList(int projectId, string collection,  string docName)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetSafetyDocumentsListAsync(Helper.DBInstance, projectId, collection, docName);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> SaveDocument(List<RevealProjectSvc.DocumentDTO> document)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveDocumentAsync(Helper.DBInstance, document);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> SaveSafetyDocumentForAssembleIWP(List<RevealProjectSvc.DocumentDTO> safety, List<RevealProjectSvc.FiwpDTO> fiwps, List<RevealProjectSvc.DocumentDTO> documents)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveSafetyDocumentForAssembleIWPAsync(Helper.DBInstance, safety, fiwps, documents);
            await project.CloseAsync();
            return retValue;
        }


        public async Task<List<RevealProjectSvc.SigmacueDTO>> GetSigmacueByPersonnel(int personnelid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetSigmacueByPersonnelAsync(Helper.DBInstance, personnelid);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetDocumentForFIWPByFIWPID(int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDocumentForFIWPByDocTypeAsync(Helper.DBInstance, 0, fiwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<RevealProjectSvc.FiwpDocument> GetFIWPDocDrawingsByFIWP(int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFIWPDocDrawingsByFIWPAsync(Helper.DBInstance, fiwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.FiwpDocument> GetFIWPDocDrawingsBySIWP(int siwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFIWPDocDrawingsBySIWPAsync(Helper.DBInstance, siwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.FiwpDocument> GetFIWPDocDrawingsByHydro(int siwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFIWPDocDrawingsByHydroAsync(Helper.DBInstance, siwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }


        public async Task<List<RevealProjectSvc.FiwpDTO>> SaveFIWP(List<RevealProjectSvc.FiwpDTO> fiwp)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFIWPAsync(Helper.DBInstance, fiwp, "admin", "admin");
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpDTO>> SaveHydro(List<RevealProjectSvc.FiwpDTO> fiwp)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveHydroAsync(Helper.DBInstance, fiwp, "admin", "admin");
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpDTO>> SaveSIWP(List<RevealProjectSvc.FiwpDTO> fiwp)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveSIWPAsync(Helper.DBInstance, fiwp);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpDTO>> GetFiwpByID(int iwpId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpByIDAsync(Helper.DBInstance, iwpId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.MTOAndDrawing> GetComponentProgressForFIWPWithList(int cwpId,
                                                                 int projectscheduleId, int drawingId,
                                                                 List<int> materialCategoryIdList, List<int> taskCategoryIdList,
                                                                 List<int> systemIdList, List<int> typeLUIdList,
                                                                 List<int> drawingtypeLUIdList, List<int> costcodeIdList,
                                                                 List<RevealProjectSvc.ComboBoxDTO> searchstringList, List<RevealProjectSvc.ComboBoxDTO> compsearchstringList,
                                                                 List<RevealProjectSvc.ComboBoxDTO> locationList, List<string> rfinumberList,
                                                                 string searchcolumn, List<string> searchvalueList,
                                                                 int projectId, int moduleId, int grouppage)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressForFIWPWithListAsync(Helper.DBInstance, cwpId, projectscheduleId, drawingId, materialCategoryIdList, taskCategoryIdList,
                systemIdList, typeLUIdList, drawingtypeLUIdList, costcodeIdList, searchstringList, compsearchstringList, locationList, rfinumberList, searchcolumn, searchvalueList,
                projectId, moduleId, grouppage);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.MTOAndDrawing> GetComponentProgressForHydroSchedulingWithList( int drawingId,
                                                                List<int> systemIdList, List<int> projectscheduleIdList, List<int> costcodeIdList, List<string> isolinenoList,
                                                                List<RevealProjectSvc.ComboBoxDTO> searchstringList, List<RevealProjectSvc.ComboBoxDTO> locationList,
                                                                string searchcolumn, List<string> searchvalueList,
                                                                int projectId, int moduleId, int grouppage)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressForHydroSchedulingAppWithListAsync(Helper.DBInstance, drawingId, systemIdList, projectscheduleIdList,
                costcodeIdList, isolinenoList, searchstringList, locationList, searchcolumn, searchvalueList,
                projectId, moduleId, grouppage);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.MTOAndDrawing> GetComponentProgressForHydroSchedulingWithListApps(int cwpId, int drawingId,
                                                                List<RevealProjectSvc.ComboBoxDTO> matrsearchstringList, List<RevealProjectSvc.ComboBoxDTO> matrsearchstringList2,
                                                                List<RevealProjectSvc.ComboBoxDTO> compsearchstringList, List<RevealProjectSvc.ComboBoxDTO> pnidsearchstringList,
                                                                List<int> systemIdList, List<int> projectscheduleIdList, List<int> costcodeIdList,
                                                                List<RevealProjectSvc.ComboBoxDTO> locationList, string searchcolumn, List<string> searchvalueList,
                                                                int projectId, int moduleId, int grouppage)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressForHydroSchedulingWithListAppsAsync(Helper.DBInstance, cwpId, drawingId, matrsearchstringList, matrsearchstringList2, 
                compsearchstringList, pnidsearchstringList, systemIdList, projectscheduleIdList, costcodeIdList, locationList, searchcolumn, searchvalueList, projectId, moduleId, grouppage);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealCommonSvc.ExtSchedulerDTO> UpdateFiwpScheduler(RevealCommonSvc.ExtSchedulerDTO extScheduler, string startDate, string finishDate)
        {
            RevealProjectSvc.ExtSchedulerDTO fiwp = new RevealProjectSvc.ExtSchedulerDTO();

            try
            {
                fiwp.Id = extScheduler.Id;
                fiwp.Name = extScheduler.Name;
                fiwp.FavoriteColor = extScheduler.FavoriteColor;
                fiwp.ResourceId = extScheduler.ResourceId;
                fiwp.Title = extScheduler.Title;
                fiwp.ResourceName = extScheduler.ResourceName;
                fiwp.StartDate = Convert.ToDateTime(startDate + " 07:00:00 AM");
                fiwp.EndDate = Convert.ToDateTime(finishDate + " 05:00:00 PM");
                fiwp.TotalMH = extScheduler.TotalMH;
                fiwp.CrewMembers = extScheduler.CrewMembers;
                fiwp.RowStyle = extScheduler.RowStyle;
                fiwp.Color = extScheduler.Color;
                fiwp.ReferenceID = extScheduler.ReferenceID;
                fiwp.IsProgressed = extScheduler.IsProgressed;

                RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
                var retValue = await project.UpdateFiwpSchedulerAsync(Helper.DBInstance, fiwp, string.Empty, string.Empty);
                await project.CloseAsync();

                if (string.IsNullOrEmpty(retValue.ExceptionMessage))
                {
                    extScheduler.Id = retValue.Id;
                    extScheduler.Name = retValue.Name;
                    extScheduler.FavoriteColor = retValue.FavoriteColor;
                    extScheduler.ResourceId = retValue.ResourceId;
                    extScheduler.Title = retValue.Title;
                    extScheduler.ResourceName = retValue.ResourceName;
                    extScheduler.StartDate = retValue.StartDate;
                    extScheduler.EndDate = retValue.EndDate;
                    extScheduler.TotalMH = retValue.TotalMH;
                    extScheduler.CrewMembers = retValue.CrewMembers;
                    extScheduler.RowStyle = retValue.RowStyle;
                    extScheduler.Color = retValue.Color;
                    extScheduler.ReferenceID = retValue.ReferenceID;
                    extScheduler.IsProgressed = retValue.IsProgressed;
                }
                extScheduler.ExceptionMessage = retValue.ExceptionMessage; 
                return extScheduler;
            }
            catch
            {
                extScheduler.ExceptionMessage = "There was an error at update";
                return extScheduler;
            }


        }

        public async Task<List<RevealProjectSvc.SigmacueDTO>> SaveSigmacue(List<RevealProjectSvc.SigmacueDTO> cue, int fiwpId, string type)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveSigmacueAsync(Helper.DBInstance, cue, fiwpId, type);
            await project.CloseAsync();
            return retValue;
        }
        
        public async Task<RevealProjectSvc.ProgressAssignment> UpdateFIWPProgressAssignment(RevealProjectSvc.ProgressAssignment progress)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.UpdateFIWPProgressAssignmentAsync(Helper.DBInstance, progress, "admin", "admin");
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.ProgressAssignment> UpdateSIWPProgressAssignmentByScope(RevealProjectSvc.ProgressAssignment progress,
            int startdrawingId, int enddrawingId, int startidfseq, int endidfseq, List<int> withindrawingList)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.UpdateSIWPProgressAssignmentByScopeAsync(Helper.DBInstance, progress, startdrawingId, enddrawingId, startidfseq, endidfseq, withindrawingList,"admin", "admin");
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.ProgressAssignment> UpdateHydroProgressAssignmentByStartPoint(RevealProjectSvc.ProgressAssignment progress,
            int drawingId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.UpdateHydroProgressAssignmentByStartPointAsync(Helper.DBInstance, progress, drawingId, "admin", "admin");
            await project.CloseAsync();
            return retValue;
        }

        

        #endregion

        #region "Progress"
        public async Task<List<RevealProjectSvc.MTODTO>> GetComponentProgressByFIWPUncompleted(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, DateTime workDate, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressByFIWPUncompletedAsync(Helper.DBInstance, cwpId, materialcategoryId, fiwpId, projectId, moduleId, workDate, ruleofcreditweightId, new List<int>() { });
            await project.CloseAsync();
            return retValue.mto;
        }

        public async Task<RevealProjectSvc.TimesheetTaskAndValue> GetTimesheetByCrewForMultiPool(int cwpId, int fiwpId, int materialcategoryId, DateTime workDate, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetTimesheetByCrewForMultiPoolAsync(Helper.DBInstance, cwpId, fiwpId, materialcategoryId, workDate, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.TimeallocationDTO>> GetTimeallocationForGroup(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, DateTime installedDate)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetTimeallocationForGroupAsync(Helper.DBInstance, cwpId, fiwpId, materialcategoryId, installedDate, ruleofcreditweightId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.TimesheetDTO>> GetTimesheetByWorkdateDailyTimeSheet(int costcodeId, int departstructureId, int dailytimesheetId, DateTime workDate, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetTimesheetByWorkdateDailyTimeSheetAsync(Helper.DBInstance, workDate, costcodeId, departstructureId, dailytimesheetId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.TimesheetDTO>> GetTimesheetByWorkdateCostcodeDepartstructure(int costcodeId, int departstructureId, DateTime workDate, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = new List<RevealProjectSvc.TimesheetDTO>();
            //var retValue = await project.GetTimesheetByWorkdateCostcodeDepartstructureAsync(Helper.DBInstance, workDate, costcodeId, departstructureId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DailytimesheetDTO>> GetDailytimesheetByID(int dailytimesheetId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDailytimesheetByIDAsync(Helper.DBInstance, dailytimesheetId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.MTODTO>> GetComponentProgressByFIWPDone(int cwpId, int materialcategoryId, int fiwpId, int projectId, int moduleId, DateTime workDate, int ruleofcreditweightId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressByFIWPDoneAsync(Helper.DBInstance, cwpId, materialcategoryId, fiwpId, projectId, moduleId, workDate, ruleofcreditweightId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<bool> SaveDailyTimehseet(int dataId, int isDirect, int dailyTimesheetId, int status, string updatedBy, DateTime workDate, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveDailyTimehseetAsync(Helper.DBInstance, workDate, dataId, isDirect, dailyTimesheetId, status, updatedBy, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<bool> SaveDailyTimehseet(int departstructueId, string updatedBy, DateTime workDate, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveDailyTimehseetAsync(Helper.DBInstance, workDate, departstructueId, 1, 0, TrackTimeSheetStatus.Submit, updatedBy, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.TimesheetDTO>> SaveTimesheet(List<RevealProjectSvc.TimesheetDTO> updates, List<RevealProjectSvc.MTODTO> progresses, decimal workhour)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveTimesheetAsync(Helper.DBInstance, updates, progresses, workhour, -1);
            await project.CloseAsync();
            return retValue;
        }


        public async Task<RevealProjectSvc.MTOAndDrawing> GetComponentProgressForSchedulingWithList(int cwpId, int drawingId,
                                                                    List<int> materialCategoryIdList, List<int> taskCategoryIdList,
                                                                    List<int> systemIdList, List<int> typeLUIdList,
                                                                    List<int> drawingtypeLUIdList, List<int> costcodeIdList,
                                                                    List<RevealProjectSvc.ComboBoxDTO> searchstringList, List<RevealProjectSvc.ComboBoxDTO> compsearchstringList,
                                                                    List<RevealProjectSvc.ComboBoxDTO> locationList, List<string> rfinumberList,
                                                                    string searchcolumn, List<string> searchvalueList,
                                                                    int projectId, int moduleId, int grouppage)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressForSchedulingWithListAsync(Helper.DBInstance, cwpId, drawingId, materialCategoryIdList, taskCategoryIdList, systemIdList, 
                typeLUIdList, drawingtypeLUIdList, costcodeIdList, searchstringList, compsearchstringList, locationList, rfinumberList, searchcolumn, searchvalueList, projectId,
                moduleId, grouppage);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.MTODTO>> GetComponentProgressByFIWP(int fiwpId, int projectScheduleID, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressByFIWPAsync(Helper.DBInstance, fiwpId, projectScheduleID, projectId, moduleId);
            return retValue;
        }

        public async Task<List<RevealProjectSvc.MTODTO>> GetComponentProgressBySIWP(int fiwpId, int projectScheduleID, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressBySIWPAsync(Helper.DBInstance, fiwpId, projectScheduleID, projectId, moduleId);
            return retValue;
        }

        #endregion

        #region "CWP"
        public async Task<List<RevealProjectSvc.CwpDTO>> GetCWPsByProjectID(int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetCWPsByProjectIDAsync(Helper.DBInstance, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.CwpDTO>> GetCwpByProject(int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetCwpByProjectAsync(Helper.DBInstance, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.CwpDTO>> GetCwpByProjectPackageType(int projectId, int moduleId, int packagetypeLuid )
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetCwpByProjectPackageTypeAsync(Helper.DBInstance, projectId, moduleId, packagetypeLuid);
            await project.CloseAsync();
            return retValue.ToList();
        }

        #endregion

        #region "Schedule"
        public async Task<List<RevealProjectSvc.ProjectscheduleDTO>> GetProjectScheduleByProjectWithWBS(int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectScheduleByProjectWithWBSAsync(Helper.DBInstance, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.ProjectscheduleDTO>> GetProjectScheduleByCwpProjectIDWithWBS(int cwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectScheduleByCwpProjectIDWithWBSAsync(Helper.DBInstance, cwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }
        

        public async Task<List<RevealProjectSvc.ProjectscheduleDTO>> GetProjectScheduleByCwpProjectWithWBS(int cwpId, int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectScheduleByCwpProjectWithWBSAsync(Helper.DBInstance, cwpId, projectId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.ProjectscheduleDTO>> GetProjectScheduleByCwpProjectPackageTypeWithWBS(int cwpId, int projectId, int packagetypeLuid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectScheduleByCwpProjectPackageTypeWithWBSAsync(Helper.DBInstance, cwpId, projectId, packagetypeLuid);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<RevealProjectSvc.ProjectscheduleDTO> GetSignleProjscheduleByID(int projectScheduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetSignleProjscheduleByIDAsync(Helper.DBInstance, projectScheduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.ProjectscheduleDTO>> GetProjectScheduleByID(int projectScheduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectScheduleByIDAsync(Helper.DBInstance, projectScheduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.ProjectscheduleDTO>> SaveProjectSchedule(List<RevealProjectSvc.ProjectscheduleDTO> schedules)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveProjectScheduleAsync(Helper.DBInstance, schedules, "admin", "admin");
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.ProgressAssignment> UpdateProjectScheduleAssignment(RevealProjectSvc.ProgressAssignment progress)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.UpdateProjectScheduleAssignmentAsync(Helper.DBInstance, progress, "admin", "admin");
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.ProjectscheduleDTO>> GetProjectScheduleByCWPProjectID(int cwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectScheduleByCWPProjectIDAsync(Helper.DBInstance, cwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.ProjectscheduleDTO>> GetProjectScheduleAllByProjectIDModuleID(int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetProjectScheduleByProjectIDAsync(Helper.DBInstance, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }


        public async Task<List<RevealProjectSvc.SystemDTO>> GetSystemByProjectID(int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetSystemByProjectIDAsync(Helper.DBInstance, projectId);
            await project.CloseAsync();
            return retValue;
        }

        #endregion

        #region Assemble

        public async Task<List<RevealProjectSvc.CollectionDTO>> GetAvailableCollectionForScheduling(int cwpId, int projectScheduleId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetAvailableCollectionForSchedulingAsync(Helper.DBInstance, cwpId, projectScheduleId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.CollectionDTO>> GetAvailableCollectionForHydroScheduling(int cwpId, int projectScheduleId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetAvailableCollectionForHydroSchedulingAsync(Helper.DBInstance, cwpId, projectScheduleId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.CollectionDTO>> GetAvailableCollectionForSchedulingApp(int cwpId, int projectScheduleId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetAvailableCollectionForSchedulingAppAsync(Helper.DBInstance, cwpId, projectScheduleId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpequipDTO>> GetFiwpEquipByFIWP(int id, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpEquipByFIWPAsync(Helper.DBInstance, id, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpequipDTO>> SaveFiwpequip(List<RevealProjectSvc.FiwpequipDTO> fiwpequip)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpequipAsync(Helper.DBInstance, fiwpequip);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpequipDTO>> SaveFiwpequipForAssembleIWP(List<RevealProjectSvc.FiwpequipDTO> fiwpequip, List<RevealProjectSvc.FiwpDTO> fiwps, List<RevealProjectSvc.DocumentDTO> documents)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpequipForAssembleIWPAsync(Helper.DBInstance, fiwpequip, fiwps, documents);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpmaterialDTO>> SaveFiwpmaterial(List<RevealProjectSvc.FiwpmaterialDTO> fiwpmaterial)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpMaterialAsync(Helper.DBInstance, fiwpmaterial);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpmaterialDTO>> SaveFiwpMaterialForAssembleIWP(List<RevealProjectSvc.FiwpmaterialDTO> fiwpmaterial, List<RevealProjectSvc.FiwpDTO> fiwps, List<RevealProjectSvc.DocumentDTO> documents)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpMaterialForAssembleIWPAsync(Helper.DBInstance, fiwpmaterial, fiwps, documents);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.LibconsumableDTO>> GetLibconsumableAll()
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetLibconsumableAllAsync(Helper.DBInstance); 
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpmaterialDTO>> GetFiwpMaterialByFIWP(int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpMaterialByFIWPAsync(Helper.DBInstance, fiwpId, projectId, moduleId); 
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpwfpDTO>> GetFiwpwfpByFiwp(int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpWFPByFIWPAsync(Helper.DBInstance, fiwpId, projectId, moduleId);
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpwfpDTO>> SaveFiwpwfp(List<RevealProjectSvc.FiwpwfpDTO> fiwpwfpdto)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpwfpAsync(Helper.DBInstance, fiwpwfpdto);           
            
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpwfpDTO>> SaveFiwpwfpForAssembleIWP(List<RevealProjectSvc.FiwpwfpDTO> fiwpwfpdto, List<RevealProjectSvc.FiwpDTO> fiwps, List<RevealProjectSvc.DocumentDTO> documents)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpwfpForAssembleIWPAsync(Helper.DBInstance, fiwpwfpdto, fiwps, documents);

            return retValue;
        }

        public async Task<RevealProjectSvc.DocumentDTO> SaveProjectDocumentWithSharePointForModelView(List<RevealProjectSvc.FiwpDTO> fiwps,
            RevealProjectSvc.DocumentDTO document, string docName, string cwpName, string fiwpName)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveProjectDocumentWithSharePointForModelViewAsync(Helper.DBInstance, fiwps, document, docName, cwpName, fiwpName);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetDocumentForFIWPByDocType(int doctypeId, int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDocumentForFIWPByDocTypeAsync(Helper.DBInstance, doctypeId, fiwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }


        public async Task<List<RevealProjectSvc.DocumentDTO>> SaveProjectDocumentWithSharePointForWFP(
           RevealProjectSvc.DocumentDTO document, string cwpName, string fiwpName, int siteimage)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveProjectDocumentWithSharePointForWFPAsync(Helper.DBInstance, document, cwpName, fiwpName, siteimage, 0);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpqaqcDTO>> SaveFiwpqaqc(List<RevealProjectSvc.FiwpqaqcDTO> fiwpqaqc)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpqaqcAsync(Helper.DBInstance, fiwpqaqc);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpqaqcDTO>> SaveFiwpqaqcForAssembleIWP(List<RevealProjectSvc.FiwpqaqcDTO> fiwpqaqc, List<RevealProjectSvc.FiwpDTO> fiwps, List<RevealProjectSvc.DocumentDTO> documents)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpqaqcForAssembleIWPAsync(Helper.DBInstance, fiwpqaqc, fiwps, documents);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.QaqcformtemplateDTO>> GetQaqcformtemplateByNameProject(string templetename, int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetQaqcformtemplateByNameProjectAsync(Helper.DBInstance, templetename, projectId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpqaqcDTO>> GetFiwpqaqcByFIWP(int fiwpId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpqaqcByFIWPAsync(Helper.DBInstance, fiwpId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.QaqcformtemplateDTO>> GetQaqcformtemplateByFiwp(int fiwpId, int qaqcformtype, int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetQaqcformtemplateByFiwpUnassignedAsync(Helper.DBInstance, qaqcformtype, fiwpId, projectId);
            //var retValue = await project.GetQaqcformtemplateByFiwpUnassignedAsync(Helper.DBInstance, 0, fiwpId, projectId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpDTO>> GetFiwpByScheduleID(int projectscheduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpByScheduleIDAsync(Helper.DBInstance, projectscheduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpDTO>> GetFiwpByCwpSchedule(int cwpId, int projectscheduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpByCwpScheduleAsync(Helper.DBInstance, cwpId, projectscheduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.FiwpDTO>> GetFiwpByCwpSchedulePackageType(int cwpId, int projectscheduleId, int packagetypeLuid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpByCwpSchedulePackageTypeAsync(Helper.DBInstance, cwpId, projectscheduleId, packagetypeLuid);
            await project.CloseAsync();
            return retValue;
        }

        #endregion

        #region MTO

        public async Task<RevealProjectSvc.MTOPageTotal> GetMaterialTakeOff(int cwpId, int drawingId, int materialCategyrId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetMaterialTakeOffAsync(Helper.DBInstance, cwpId, drawingId, materialCategyrId, 0, 0, projectId, moduleId, 1, 10000);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.ProgressDTO>> DeleteMto(List<RevealProjectSvc.MTODTO> mtodto, string updatedBy, string userName, string password, int rfiid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.DeleteMTOAsync(Helper.DBInstance, mtodto, updatedBy, userName, password, rfiid);
            await project.CloseAsync();
            return retValue;
        }

        #endregion

        #region Library

        public async Task<RevealProjectSvc.LibgroundingmanhourPageTotal> GetLibGroundingManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetLibGroundingManhourForPagingAsync(Helper.DBInstance, selectedPage, pageSize, taskType, partNumber, Vender);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.LibequipmanhourPageTotal> GetLibEquipManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetLibEquipManhourForPagingAsync(Helper.DBInstance, selectedPage, pageSize, taskType, partNumber, Vender);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.LiblightingmanhourPageTotal> GetLibLightingManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetLibLightingManhourForPagingAsync(Helper.DBInstance, selectedPage, pageSize, taskType, partNumber, Vender);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.LibracewaymanhourPageTotal> GetLibRacewayManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetLibRacewayManhourForPagingAsync(Helper.DBInstance, selectedPage, pageSize, taskType, partNumber, Vender);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.LibinstrmanhourPageTotal> GetLibInstrManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetLibInstrManhourForPagingAsync(Helper.DBInstance, selectedPage, pageSize, taskType, partNumber, Vender);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.LibehtmanhourPageTotal> GetLibEhtManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetLibEhtManhourForPagingAsync(Helper.DBInstance, selectedPage, pageSize, taskType, partNumber, Vender);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.LibcablemanhourPageTotal> GetLibCableManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetLibCableManhourForPagingAsync(Helper.DBInstance, selectedPage, pageSize, taskType, partNumber, Vender);
            await project.CloseAsync();
            return retValue;
        }

        #endregion

        #region "Qaqc" 
        public async Task<List<RevealProjectSvc.FiwpqaqcDTO>> GetITRListByFiwp(int fiwpId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpqaqcByFIWPAsync(Helper.DBInstance, fiwpId);
            await project.CloseAsync();
            return retValue.ToList();
        }
        
        public async Task<List<RevealProjectSvc.QaqcformDTO>> SaveQaqcformForDownload(int projectId, int moduleId, int cwpId, int fiwpId, List<RevealProjectSvc.QaqcformtemplateDTO> qaqcformtemplate, string updatedBy, int qaqcType )
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveQaqcformForDownloadAsync(Helper.DBInstance, projectId, moduleId, cwpId, fiwpId, qaqcformtemplate, updatedBy, qaqcType);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.QaqcformDTO>> SaveQaqcformForSubmit(List<RevealProjectSvc.QaqcformDTO> qaqcforms)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveQaqcformForSubmitAsync(Helper.DBInstance, qaqcforms);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.QaqcformDTO>> GetQaqcformByQcManager(int projectid, int moduleid, int loginId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetQaqcformByQcManagerAsync(Helper.DBInstance, projectid, moduleid, loginId, WinAppLibrary.Utilities.QAQCDataType.ITR);
            await project.CloseAsync();
            return retValue;
        }

        // Download Punch Ticket List
        public async Task<List<RevealProjectSvc.QaqcformdetailDTO>> GetQaqcformByPersonnelDepartment(int projectId, int moduleId, int personnelId, int departmentId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetQaqcformByPersonnelDepartmentAsync(Helper.DBInstance, projectId, moduleId, personnelId, departmentId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<RevealProjectSvc.QaqcformDTO> GetQaqcformbyID(int qaqcformId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetQaqcformByIDAsync(Helper.DBInstance, qaqcformId);
            await project.CloseAsync();
            return retValue;
        }

        // Get Punch Ticket
        public async Task<RevealProjectSvc.PunchDTOSet> GetPunchListByQaqcform(int qaqcformId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetPunchListByQaqcformAsync(Helper.DBInstance, qaqcformId);
            await project.CloseAsync();
            return retValue;
        }

        // Save Punch Ticket
        public async Task<RevealProjectSvc.WalkdownDTOSet> SaveQaqcformWithSharePoint(RevealProjectSvc.WalkdownDTOSet qaqcforms)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveQaqcformWithSharepointAsync(Helper.DBInstance, qaqcforms);
            await project.CloseAsync();
            return retValue;
        }
        #endregion

        #region "Tuen Over"

        public async Task<List<RevealProjectSvc.ProjectDTO>> GetContractorProejctByOwnerProject(int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetContractorProejctByOwnerProjectAsync(Helper.DBInstance, projectId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.SystemDTO>> GetSystemByTurnoverProject(int projectId, int constractorId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetSystemByTurnoverProjectAsync(Helper.DBInstance, projectId, constractorId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.ModuleDTO>> GetModuleByTurnoverSystem(int projectId, int constractorId, int systemId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetModuleByTurnoverSystemAsync(Helper.DBInstance, projectId, constractorId, systemId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<RevealProjectSvc.WalkdownDTOSet> GetWalkDownBySystem(int projectId, int constractorId, int systemId, List<int> moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetWalkDownBySystemAsync(Helper.DBInstance, projectId, constractorId, systemId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.QaqcformDTO>> GetQaqcformBySystem(int systemId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetQaqcformBySystemAsync(Helper.DBInstance, systemId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.WalkdownDTOSet> SaveQaqcformWithSharepoint(Element.Reveal.Meg.RevealProjectSvc.WalkdownDTOSet walkdownDs)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveQaqcformWithSharepointAsync(Helper.DBInstance, walkdownDs);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.QaqcformDTO> SaveTurnoverCertificateForMC(RevealProjectSvc.DocumentDTO documentdto, RevealProjectSvc.QaqcformDTO qaqcformdto, int systemId)
        {
            
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveTurnoverCertificateForMCAsync(Helper.DBInstance, documentdto, qaqcformdto, systemId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> SaveTurnoverCertificatePDFForMC(RevealProjectSvc.DocumentDTO documentdto, RevealProjectSvc.QaqcformDTO qaqcformdto, int systemId)
        {
            
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveTurnoverCertificatePDFForMCAsync(Helper.DBInstance, documentdto, qaqcformdto, systemId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.QaqcformDTO> SaveTurnoverCertificateForTCCC(RevealProjectSvc.DocumentDTO documentdto, RevealProjectSvc.QaqcformDTO qaqcformdto, int systemId)
        {

            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveTurnoverCertificateForTCCCAsync(Helper.DBInstance, documentdto, qaqcformdto, systemId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> SaveTurnoverCertificatePDFForTCCC(RevealProjectSvc.DocumentDTO documentdto, RevealProjectSvc.QaqcformDTO qaqcformdto, int systemId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveTurnoverCertificatePDFForTCCCAsync(Helper.DBInstance, documentdto, qaqcformdto, systemId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetDocumentByTurnoverPackage(string lookupValue, int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDocumentByTurnoverPackageAsync(Helper.DBInstance, lookupValue, fiwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<RevealProjectSvc.QaqcformDTO> GetTurnoverCertificateForMC(int projectId, int systemId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetTurnoverCertificateForMCAsync(Helper.DBInstance, projectId, systemId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.QaqcformDTO> GetTurnoverCertificateForTCCC(int projectId, int systemId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetTurnoverCertificateForTCCCAsync(Helper.DBInstance, projectId, systemId);
            await project.CloseAsync();
            return retValue;
        }

        #endregion

        #region Report
        public async Task<List<RevealProjectSvc.rptQAQCformDTO>> GetITRReportBySystem(int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetITRChartbySystemAsync(Helper.DBInstance, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }
        public async Task<List<RevealProjectSvc.rptQAQCformDTO>> GetITRReportByCWP(int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetITRChartbyCWPAsync(Helper.DBInstance, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.rptPunchDTO>> GetPunchReportByDisc(int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetPunchChartbyDisciplineAsync(Helper.DBInstance, projectId);
            await project.CloseAsync();
            return retValue;
        }
        public async Task<List<RevealProjectSvc.rptPunchDTO>> GetPunchReportByCat(int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetPunchChartbyCATAsync(Helper.DBInstance, projectId);
            await project.CloseAsync();
            return retValue;
        }
        #endregion

        #region "Drawing"

        public async Task<List<RevealProjectSvc.DrawingDTO>> GetDrawingByProjectID(int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDrawingsByProjectIDAsync(Helper.DBInstance, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        #endregion

        #region csu

        public async Task<List<RevealProjectSvc.FiwpDTO>> GetFiwpBySystemPackageType(int projectId, int systemId, int type)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpBySystemPackageTypeAsync(Helper.DBInstance, projectId, systemId, type);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.DrawingDTO>> GetDrawingByDrawingType(int drawingtypeLuid, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDrawingByDrawingTypeAsync(Helper.DBInstance, drawingtypeLuid, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.ComboBoxDTO>> SavePnIDDrawingForBuildCSU(List<RevealProjectSvc.ComboBoxDTO> drawings, List<RevealProjectSvc.FiwpDTO> fiwps)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SavePnIDDrawingForBuildCSUAsync(Helper.DBInstance, drawings, fiwps);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> SaveAssociatedDocumentForBuildCSU(List<RevealProjectSvc.DocumentDTO> AssociatedDocs, List<RevealProjectSvc.FiwpDTO> fiwps, int currentStep)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveAssociatedDocumentForBuildCSUAsync(Helper.DBInstance, AssociatedDocs, fiwps, currentStep);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<RevealProjectSvc.FiwpDocument> GetFIWPDocDrawingsByCSU(int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFIWPDocDrawingsByCSUAsync(Helper.DBInstance, fiwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.ComboBoxDTO>> GetDrawingByDrawingType_Combo(int drawingtypeLuid, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDrawingByDrawingType_ComboAsync(Helper.DBInstance, drawingtypeLuid, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        #endregion
    }
}
