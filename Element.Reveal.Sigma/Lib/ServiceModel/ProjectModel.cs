using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Sigma.Lib.ServiceModel
{
    public class ProjectModel
    {
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

        public async Task<List<RevealProjectSvc.MTODTO>> GetComponentProgressByFIWPDone(int cwpId, int materialcategoryId, int fiwpId, int projectId, int moduleId, DateTime workDate, int ruleofcreditweightId)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.GetComponentProgressByFIWPDoneAsync(Helper.DBInstance, cwpId, materialcategoryId, fiwpId, projectId, moduleId, workDate, ruleofcreditweightId);
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
        #endregion
    }
}
