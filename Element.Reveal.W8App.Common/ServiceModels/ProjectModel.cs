using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace WinAppLibrary.ServiceModels
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
        #endregion

        #region "Drawing"
        public async Task<IEnumerable<RevealProjectSvc.DrawingDTO>> GetDrawingByProjectID(int projectId, int moduleId)
        {
            IEnumerable<RevealProjectSvc.DrawingDTO> retValue = new List<RevealProjectSvc.DrawingDTO>();
            try
            {
                RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
                retValue = await project.JsonGetDrawingsByProjectIDAsync(Helper.DBInstance, project.ToString(), moduleId.ToString());
                await project.CloseAsync();
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetDrawingByProjectID");
            }

            return retValue;
        }

        public async Task<IEnumerable<RevealProjectSvc.DrawingDTO>> GetDrawingByCWP(int cwpId)
        {
            IEnumerable<RevealProjectSvc.DrawingDTO> retValue = new List<RevealProjectSvc.DrawingDTO>();
            try
            {
                RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
                retValue = await project.GetDrawingByCWPAsync(Helper.DBInstance, cwpId);
                await project.CloseAsync();
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetDrawingByCWP");
            }

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
        #endregion

        #region "FIWPManonsite"
        public async Task<List<RevealProjectSvc.FiwpmanonsiteDTO>> GetFiwpManonsiteByForeman(int foremanStructureId, DateTime workdate)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            workdate = workdate == null || workdate.Year < 1753 ? new DateTime(1753, 1, 1) : workdate;
            var retValue = await project.GetFiwpManonsiteByForemanAsync(Helper.DBInstance, foremanStructureId, workdate);
            await project.CloseAsync();
            return retValue.ToList();
        }

        public async Task<List<RevealProjectSvc.FiwpmanonsiteDTO>> SaveFiwpManonsite(List<RevealProjectSvc.FiwpmanonsiteDTO> list)
        {
            RevealProjectSvc.ProjectServiceClient project = ServiceHelper.GetServiceClient<RevealProjectSvc.ProjectServiceClient>(ServiceHelper.ProjectService);
            var retValue = await project.SaveFiwpManonsiteAsync(Helper.DBInstance, list);
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
        #endregion
    }
}
