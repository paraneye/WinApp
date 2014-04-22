using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Crew.Lib.ServiceModel
{
    public class ProjectModel
    {
        Helper _helper = new Helper();

        #region "Project"
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

        #region "ToolBoxTalk"

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetSafetyDocumentsList(int projectid, int moduleid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetSafetyDocumentsListAsync(Helper.DBInstance, projectid, SPCollectionName.SafetyDoc, "Name");
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.DailytoolboxDTO>> GetDailytoolboxByDailyBras(int dailybassid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDailytoolboxByDailyBrassAsync(Helper.DBInstance, dailybassid);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetCrewDocumentsList(int projectId, List<string> docName)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetCrewDocumentsListAsync(Helper.DBInstance, projectId, "CrewPictures", docName);
            await project.CloseAsync();
            return retValue.ToList();
        }


        public async Task<List<RevealProjectSvc.DailytoolboxDTO>> SaveDailyToolBox(List<RevealProjectSvc.DailytoolboxDTO> dailytoolbox)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveDailytoolboxAsync(Helper.DBInstance, dailytoolbox);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.ToolboxsignDTO>> SaveToolBoxSign(List<RevealProjectSvc.ToolboxsignDTO> toolboxsign)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveToolboxsignAsync(Helper.DBInstance, toolboxsign);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.ToolboxsignDTO>> SaveToolBoxSignOffline(List<RevealProjectSvc.ToolboxsignDTO> toolboxsign)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveToolboxsignOfflineAsync(Helper.DBInstance, toolboxsign);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<int> SaveToolboxTalks(int dailybrassid, int projectid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveToolboxTalksAsync(Helper.DBInstance, dailybrassid, projectid);
            await project.CloseAsync();
            return retValue;
        }


        #endregion

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

        #region "Brass In"
        public async Task<List<RevealProjectSvc.DailybrassDTO>> GetDailybrassByForemanPersonnelWorkDate(int projectid, int moduleid, int personnelid, DateTime workdate)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDailybrassByForemanPersonnelWorkDateAsync(Helper.DBInstance, projectid, moduleid, personnelid, workdate);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.DailybrasssignDTO>> GetDailybrasssignByDailyBrass(int dailybrassid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDailybrasssignByDailyBrassAsync(Helper.DBInstance, dailybrassid);
            await project.CloseAsync();
            return retValue.ToList();
        }


        public async Task<List<RevealProjectSvc.DailybrassDTO>> SaveDailybrass(List<RevealProjectSvc.DailybrassDTO> dailybrass)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveDailybrassAsync(Helper.DBInstance, dailybrass);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DailybrasssignDTO>> SaveDailybrasssign(List<RevealProjectSvc.DailybrasssignDTO> dailybrasssign)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveDailybrasssignAsync(Helper.DBInstance, dailybrasssign);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.DailybrasssignDTO>> SaveDailybrasssignOffLine(List<RevealProjectSvc.DailybrasssignDTO> dailybrasssign)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveDailybrasssignOfflineAsync(Helper.DBInstance, dailybrasssign);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<int> SaveCrewAttendance(int dailybrassId, int projectId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveCrewAttendanceAsync(Helper.DBInstance, dailybrassId, projectId);
            await project.CloseAsync();
            return retValue;
        }

        //public async Task<int> SaveTimetable(int projectid, int departmentid)
        //{
        //    RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
        //    var retValue = await project.SaveTimetableAsync(Helper.DBInstance, projectid, departmentid, DateTime.Now);
        //    await project.CloseAsync();
        //    return retValue;
        //}

        #endregion

        #region "Progress"
        public async Task<List<RevealProjectSvc.CollectionDTO>> GetAvailableCollectionForForemanUncompleted(int personnelId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetAvailableCollectionForForemanUncompletedAsync(Helper.DBInstance, personnelId, projectId, moduleId);
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

        public async Task<RevealProjectSvc.MTOAndDrawing> GetComponentProgressByFIWPUncompleted(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, DateTime workDate, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressByFIWPUncompletedAsync(Helper.DBInstance, cwpId, materialcategoryId, fiwpId, projectId, moduleId, workDate, ruleofcreditweightId, new List<int>() { });
            await project.CloseAsync();
            return retValue;
        }

        public async Task<RevealProjectSvc.TimesheetTaskAndValue> GetTimesheetByCrewForMultiPool(int cwpId, int fiwpId, int materialcategoryId, DateTime workDate, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetTimesheetByCrewForMultiPoolAsync(Helper.DBInstance, cwpId, fiwpId, materialcategoryId, workDate, projectId, moduleId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.TimesheetDTO>> GetTimesheetByWorkdateCostcodeDepartstructure(int costcodeId, int departstructureId, int dailytimesheetId, DateTime workDate, int projectId, int moduleId, int ishistory)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetTimesheetByWorkdateCostcodeDepartstructureAsync(Helper.DBInstance, workDate, costcodeId, departstructureId, projectId, moduleId, ishistory);
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

        public async Task<List<RevealProjectSvc.DailytimesheetDTO>> GetDailytimesheetByID(int dailytimesheetId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDailytimesheetByIDAsync(Helper.DBInstance, dailytimesheetId);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.TimesheetAndProgress>> GetTimesheetAndProgressByWorkdateDepartStructure(DateTime workdate, int departstructureId, int dailytimesheetId, int projectId, int moduleId, int ishisotry)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetTimesheetAndProgressByWorkdateDepartStructureAsync(Helper.DBInstance, workdate, departstructureId, dailytimesheetId, projectId, moduleId, ishisotry);
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

        public async Task<List<RevealProjectSvc.TimesheetDTO>> SaveTimesheet(List<RevealProjectSvc.TimesheetDTO> updates, List<RevealProjectSvc.MTODTO> progresses, decimal workhour, int timeallocationId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveTimesheetAsync(Helper.DBInstance, updates, progresses, workhour, timeallocationId);
            await project.CloseAsync();
            return retValue;
        }
        #endregion
        
        #region "Crew"
        public async Task<List<RevealProjectSvc.SigmacueDTO>> GetSigmacueByPersonnel(int personnelid)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetSigmacueByPersonnelAsync(Helper.DBInstance, personnelid);
            await project.CloseAsync();
            return retValue;
        }

        public async Task<List<RevealProjectSvc.SigmacueDTO>> SaveSigmacue(List<RevealProjectSvc.SigmacueDTO> tasks, int dataId, string tasktype)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveSigmacueAsync(Helper.DBInstance, tasks, dataId, tasktype);
            await project.CloseAsync();
            return retValue;
        }
        #endregion

        #region "IWP"
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

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetItrDocumentForFIWPByFIWPID(int doctypeId, int fiwpId, int projectId, int moduleId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetDocumentForFIWPByDocTypeAsync(Helper.DBInstance, doctypeId, fiwpId, projectId, moduleId);
            await project.CloseAsync();
            return retValue.ToList();
        }


        

        #endregion

        #region "Document"
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
        #endregion

        #region "Drawing"
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
        #endregion

        #region "Qaqc"
        public async Task<List<RevealProjectSvc.FiwpqaqcDTO>> GetITRListByFiwp(int fiwpId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetFiwpqaqcByFIWPAsync(Helper.DBInstance, fiwpId);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.QaqcformDTO>> SaveQaqcformForDownload(int projectId, int moduleId, int cwpId, int fiwpId, List<RevealProjectSvc.QaqcformtemplateDTO> qaqcformtemplate, string updatedBy)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveQaqcformForDownloadAsync(Helper.DBInstance, projectId, moduleId, cwpId, fiwpId, qaqcformtemplate, updatedBy, WinAppLibrary.Utilities.QAQCDataType.ITR);
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
        #endregion
    }
}
