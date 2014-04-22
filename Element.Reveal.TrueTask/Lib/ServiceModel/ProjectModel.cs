using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;
using Element.Reveal.DataLibrary;

namespace Element.Reveal.TrueTask.Lib.ServiceModel
{
    public class ProjectModel
    {
        Helper _helper = new Helper();

        #region "FiwpManonsite"
        public async Task<List<DataLibrary.FiwpmanonsiteDTO>> GetFiwpManonsiteByForeman(int foremanStructureId, DateTime workdate)
        {
            List<DataLibrary.FiwpmanonsiteDTO> retValue = new List<DataLibrary.FiwpmanonsiteDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(foremanStructureId);
            param.Add(workdate);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpmanonsiteDTO>>("JsonGetFiwpManonsiteByForeman", param, JsonHelper.ProjectService);

            return retValue;

        }

        public async Task<List<DataLibrary.FiwpmanonsiteDTO>> SaveFiwpManonsite(List<DataLibrary.FiwpmanonsiteDTO> fiwpmaonsite)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwpManonsite", fiwpmaonsite);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpmanonsiteDTO>>("JsonSaveFiwpManonsite", dParams, JsonHelper.ProjectService);
        }
        #endregion

        #region "Project"

        //public async Task<List<DataLibrary.DocumentnoteDTO>> GetDocumentNoteByFiwpDocumentDrawing(int fiwpId, int projectId, string disciplineCode)
        //{
        //    List<DataLibrary.DocumentnoteDTO> retValue = new List<DataLibrary.DocumentnoteDTO>();

        //    List<dynamic> param = new List<dynamic>();
        //    param.Add(fiwpId);
        //    param.Add(0);
        //    param.Add(0);
        //    param.Add(projectId);
        //    param.Add(disciplineCode);
        //    retValue = await JsonHelper.GetDataAsync<List<DataLibrary.DocumentnoteDTO>>("JsonGetDocumentNoteByFiwpDocumentDrawing", param, JsonHelper.ProjectService);

        //    return retValue;
        //}

        public async Task<List<DataLibrary.DocumentmarkupDTO>> GetDocumentmarkupByDrawingPersonnel(int drawingId, string personnelId)
        {
            List<DataLibrary.DocumentmarkupDTO> retValue = new List<DataLibrary.DocumentmarkupDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(drawingId);
            param.Add(personnelId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.DocumentmarkupDTO>>("JsonGetDocumentmarkupByDrawingPersonnel", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<DataLibrary.DocumentmarkupDTO> SaveDocumentmarkupWithMarkupImage(DataLibrary.DocumentmarkupDTO documentmarkup, DataLibrary.UpfileDTOS upFileCollection)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("documentmarkup", documentmarkup);
            dParams.Add("upFileCollection", upFileCollection);

            return await JsonHelper.PutDataAsync<DataLibrary.DocumentmarkupDTO>("JsonSaveDocumentmarkupWithMarkupImage", dParams, JsonHelper.ProjectService);
        }

        //Json 메서드에 pagesize 파라미터 추가되어 있음, 일단 10으로 넘김.
        public async Task<DataLibrary.DrawingPageTotal> GetDrawingForDrawingViewer(int projectId, List<string> cwpIds, List<string> fiwpIds, List<string> drawingtypes, string enTag, string title, string sortoption, int curpage)
        {
            DataLibrary.DrawingPageTotal retValue = new DataLibrary.DrawingPageTotal();
            try
            {
                List<dynamic> param = new List<dynamic>();
                param.Add(projectId);
                param.Add(cwpIds);
                param.Add(fiwpIds);
                param.Add(drawingtypes);
                param.Add(enTag);
                param.Add(title);
                param.Add(sortoption);
                param.Add(curpage);
                param.Add(20);   //pagesize
                var result = await JsonHelper.GetDataAsync<DataLibrary.DrawingPageTotal>("JsonGetDrawingForDrawingViewer", param, JsonHelper.ProjectService);

                retValue = result;
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetDrawingForDrawingViewer");
            }
            return retValue;
        }

        public async Task<List<DataLibrary.ProjectDTO>> GetAllProject()
        {
            List<DataLibrary.ProjectDTO> retValue = new List<DataLibrary.ProjectDTO>();

            List<dynamic> param = new List<dynamic>();
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectDTO>>("JsonGetProjectAll", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.ProjectDTO>> GetProjectBySigmauser(string sigmaUserId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(sigmaUserId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ProjectDTO>>("JsonGetProjectBySigmauser", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.ProjectDTO> GetProjectByID(int projectId)
        {
            DataLibrary.ProjectDTO retValue = new DataLibrary.ProjectDTO();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            var result = await JsonHelper.GetDataAsync<DataLibrary.ProjectDTO>("JsonGetProjectById", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<DataLibrary.rptProjectCwaIwpDTO> JsonGetProjectCwaIwpByIwp(string iwpId)
        {
            DataLibrary.rptProjectCwaIwpDTO retValue = new DataLibrary.rptProjectCwaIwpDTO();

            List<dynamic> param = new List<dynamic>();
            param.Add(iwpId);
            var result = await JsonHelper.GetDataAsync<DataLibrary.rptProjectCwaIwpDTO>("JsonGetProjectCwaIwpByIwp", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<List<DataLibrary.ProjectmoduleDTO>> GetAllProjectModule()
        {
            List<DataLibrary.ProjectmoduleDTO> retValue = new List<DataLibrary.ProjectmoduleDTO>();

            List<dynamic> param = new List<dynamic>();
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectmoduleDTO>>("JsonGetProjectAllModule", param, JsonHelper.ProjectService);

            return retValue;
        }

        #endregion

        #region "IWP"

        //public async Task<List<DataLibrary.DocumentDTO>> GetSafetyDocumentsList(int projectId, string fileTypeCode)
        //{
        //    List<DataLibrary.DocumentDTO> retValue = new List<DataLibrary.DocumentDTO>();

        //    List<dynamic> param = new List<dynamic>();
        //    param.Add(projectId);
        //    param.Add(fileTypeCode);

        //    return await JsonHelper.GetDataAsync<List<DataLibrary.DocumentDTO>>("JsonGetUploadedFileInfoByProjectFileType", param, JsonHelper.ProjectService);
        //}

        public async Task<List<DataLibrary.DocumentDTO>> SaveDocument(List<DataLibrary.DocumentDTO> document)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("document", document);
            return await JsonHelper.PutDataAsync<List<DataLibrary.DocumentDTO>>("JsonSaveDocument", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.DocumentDTO>> SaveSafetyDocumentForAssembleIWP(List<DataLibrary.DocumentDTO> safety, List<DataLibrary.FiwpDTO> fiwps, List<DataLibrary.DocumentDTO> documents)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("safetys", safety);
            dParams.Add("fiwps", fiwps);
            dParams.Add("documents", documents);
            return await JsonHelper.PutDataAsync<List<DataLibrary.DocumentDTO>>("JsonSaveSafetyDocumentForAssembleIWP", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.SigmacueDTO>> GetSigmacueByPersonnel(string personnelId)
        {
            List<DataLibrary.SigmacueDTO> retValue = new List<DataLibrary.SigmacueDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(personnelId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.SigmacueDTO>>("JsonGetSigmacueByPersonnel", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.DocumentDTO>> GetDocumentForFIWPByFIWPID(int fiwpId, int projectId, string disciplineCode)
        {
            List<DataLibrary.DocumentDTO> retValue = new List<DataLibrary.DocumentDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(0);
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.DocumentDTO>>("JsonGetDocumentForFIWPByDocType", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<DataLibrary.DocumentAndDrawing> GetFIWPDocDrawingsByFIWP(int fiwpId, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<DataLibrary.DocumentAndDrawing>("JsonGetFIWPDocDrawingsByFIWP", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.FiwpDocument> GetFIWPDocDrawingsBySIWP(int siwpId, int projectId, string disciplineCode)
        {
            DataLibrary.FiwpDocument retValue = new DataLibrary.FiwpDocument();

            List<dynamic> param = new List<dynamic>();
            param.Add(siwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            var result = await JsonHelper.GetDataAsync<DataLibrary.FiwpDocument>("JsonGetFIWPDocDrawingsBySIWP", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<DataLibrary.FiwpDocument> GetFIWPDocDrawingsByHydro(int siwpId, int projectId, string disciplineCode)
        {
            DataLibrary.FiwpDocument retValue = new DataLibrary.FiwpDocument();

            List<dynamic> param = new List<dynamic>();
            param.Add(siwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            var result = await JsonHelper.GetDataAsync<DataLibrary.FiwpDocument>("JsonGetFIWPDocDrawingsByHydro", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<List<DataLibrary.FiwpDTO>> SaveFIWP(List<DataLibrary.FiwpDTO> fiwp)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwp", fiwp);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpDTO>>("JsonSaveFIWP", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpDTO>> SaveHydro(List<DataLibrary.FiwpDTO> fiwp)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwp", fiwp);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpDTO>>("JsonSaveHydro", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpDTO>> SaveSIWP(List<DataLibrary.FiwpDTO> fiwp)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwp", fiwp);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpDTO>>("JsonSaveHydro", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpDTO>> GetFiwpByID(int iwpId)
        {
            List<DataLibrary.FiwpDTO> retValue = new List<DataLibrary.FiwpDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(iwpId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpDTO>>("JsonGetFiwpByID", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<DataLibrary.MTOAndDrawing> GetComponentProgressForFIWPWithList(int cwpId,
                                                                 int projectscheduleId, int drawingId,
                                                                 List<string> taskCategoryIdList, List<string> taskTypeLUIDList,
                                                                 List<string> materialIDList, List<string> progressIDList,
                                                                 string searchValue,
                                                                 int projectId, string disciplineCode, int grouppage)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("cwpId", cwpId);
            dParams.Add("projectscheduleId", projectscheduleId);
            dParams.Add("drawingId", drawingId);
            dParams.Add("taskCategoryCodeList", taskCategoryIdList);
            dParams.Add("taskTypeIdList", taskTypeLUIDList);
            dParams.Add("materialIdList", materialIDList);
            dParams.Add("progressIdList", progressIDList);
            dParams.Add("searchValue", searchValue);
            dParams.Add("projectId", projectId);
            dParams.Add("disciplineCode", disciplineCode);
            dParams.Add("grouppage", grouppage);

            return await JsonHelper.PutDataAsync<DataLibrary.MTOAndDrawing>("JsonGetComponentProgressForFIWPWithList", dParams, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.MTOAndDrawing> GetComponentProgressForHydroSchedulingWithList( int drawingId,
                                                                List<int> systemIdList, List<int> projectscheduleIdList, List<int> costcodeIdList, List<string> isolinenoList,
                                                                List<DataLibrary.ComboBoxDTO> searchstringList, List<DataLibrary.ComboBoxDTO> locationList,
                                                                string searchcolumn, List<string> searchvalueList,
                                                                int projectId, string disciplineCode, int grouppage)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(drawingId);
            param.Add(systemIdList);
            param.Add(projectscheduleIdList);
            param.Add(costcodeIdList);
            param.Add(isolinenoList);
            param.Add(searchstringList);
            param.Add(locationList);
            param.Add(searchcolumn);
            param.Add(searchvalueList);
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(grouppage);

            return await JsonHelper.GetDataAsync<DataLibrary.MTOAndDrawing>("JsonGetComponentProgressForHydroSchedulingWithList", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.MTOAndDrawing> GetComponentProgressForHydroSchedulingWithListApps(int cwpId, int drawingId,
                                                                List<DataLibrary.ComboBoxDTO> matrsearchstringList, List<DataLibrary.ComboBoxDTO> matrsearchstringList2,
                                                                List<DataLibrary.ComboBoxDTO> compsearchstringList, List<DataLibrary.ComboBoxDTO> pnidsearchstringList,
                                                                List<int> systemIdList, List<int> projectscheduleIdList, List<int> costcodeIdList,
                                                                List<DataLibrary.ComboBoxDTO> locationList, string searchcolumn, List<string> searchvalueList,
                                                                int projectId, string disciplineCode, int grouppage)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(drawingId);
            param.Add(matrsearchstringList);
            param.Add(matrsearchstringList2);
            param.Add(compsearchstringList);
            param.Add(pnidsearchstringList);
            param.Add(systemIdList);
            param.Add(projectscheduleIdList);
            param.Add(costcodeIdList);
            param.Add(locationList);
            param.Add(searchcolumn);
            param.Add(searchvalueList);
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(grouppage);

            return await JsonHelper.GetDataAsync<DataLibrary.MTOAndDrawing>("JsonGetComponentProgressForHydroSchedulingWithListApps", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.ExtSchedulerDTO> UpdateFiwpScheduler(DataLibrary.ExtSchedulerDTO extScheduler, string startDate, string finishDate)
        {
            DataLibrary.ExtSchedulerDTO fiwp = new DataLibrary.ExtSchedulerDTO();

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

                Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
                dParams.Add("extscheduler", fiwp);
                dParams.Add("userName", string.Empty);
                dParams.Add("password", string.Empty);
                var retValue = await JsonHelper.PutDataAsync<DataLibrary.ExtSchedulerDTO>("JsonUpdateFiwpScheduler", dParams, JsonHelper.ProjectService);

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

        public async Task<List<DataLibrary.SigmacueDTO>> SaveSigmacue(List<DataLibrary.SigmacueDTO> cue, int fiwpId, string type)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("sigmacue", cue);
            dParams.Add("dataId", fiwpId);
            dParams.Add("type", type);
            return await JsonHelper.PutDataAsync<List<DataLibrary.SigmacueDTO>>("JsonSaveSigmacue", dParams, JsonHelper.ProjectService);
        }
        
        public async Task<DataLibrary.ProgressAssignment> UpdateFIWPProgressAssignment(DataLibrary.ProgressAssignment progress)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("progress", progress);
            return await JsonHelper.PutDataAsync<DataLibrary.ProgressAssignment>("JsonUpdateFIWPProgressAssignment", dParams, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.ProgressAssignment> UpdateSIWPProgressAssignmentByScope(DataLibrary.ProgressAssignment progress,
            int startdrawingId, int enddrawingId, int startidfseq, int endidfseq, List<int> withindrawingList)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("progress", progress);
            dParams.Add("startdrawingId", startdrawingId);
            dParams.Add("enddrawingId", enddrawingId);
            dParams.Add("startidfseq", startidfseq);
            dParams.Add("endidfseq", endidfseq);
            dParams.Add("withindrawingList", withindrawingList);

            return await JsonHelper.PutDataAsync<DataLibrary.ProgressAssignment>("JsonUpdateSIWPProgressAssignmentByScope", dParams, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.ProgressAssignment> UpdateHydroProgressAssignmentByStartPoint(DataLibrary.ProgressAssignment progress,
            int drawingId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("progress", progress);
            dParams.Add("drawingId", drawingId);

            return await JsonHelper.PutDataAsync<DataLibrary.ProgressAssignment>("JsonUpdateHydroProgressAssignmentByStartPoint", dParams, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.FiwpDTO> UpdateIwpPeriod(FiwpDTO iwp, string totalManhours)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("iwp", iwp);
            dParams.Add("totalManhours", totalManhours);

            return await JsonHelper.PutDataAsync<DataLibrary.FiwpDTO>("JsonUpdateIwpPeriod", dParams, JsonHelper.ProjectService);
        }

        #endregion

        #region "Progress"
        public async Task<List<DataLibrary.MTODTO>> GetComponentProgressByFIWPUncompleted(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, DateTime workDate, int projectId, string disciplineCode)
        {
            List<DataLibrary.MTODTO> retValue = new List<DataLibrary.MTODTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(materialcategoryId);
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(workDate);
            param.Add(ruleofcreditweightId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.MTODTO>>("JsonGetComponentProgressByFIWPUncompleted", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<DataLibrary.TimesheetTaskAndValue> GetTimesheetByCrewForMultiPool(int cwpId, int fiwpId, int materialcategoryId, DateTime workDate, int projectId, string disciplineCode)
        {
            DataLibrary.TimesheetTaskAndValue retValue = new DataLibrary.TimesheetTaskAndValue();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(fiwpId);
            param.Add(materialcategoryId);
            param.Add(workDate);
            param.Add(projectId);
            param.Add(disciplineCode);
            var result = await JsonHelper.GetDataAsync<DataLibrary.TimesheetTaskAndValue>("JsonGetTimesheetByCrewForMultiPool", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<List<DataLibrary.TimeallocationDTO>> GetTimeallocationForGroup(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, DateTime installedDate)
        {
            List<DataLibrary.TimeallocationDTO> retValue = new List<DataLibrary.TimeallocationDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(fiwpId);
            param.Add(materialcategoryId);
            param.Add(installedDate);
            param.Add(ruleofcreditweightId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.TimeallocationDTO>>("JsonGetTimeallocationForGroup", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.TimesheetDTO>> GetTimesheetByWorkdateDailyTimeSheet(int costcodeId, int departstructureId, int dailytimesheetId, DateTime workDate, int projectId, string disciplineCode)
        {
            List<DataLibrary.TimesheetDTO> retValue = new List<DataLibrary.TimesheetDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(workDate);
            param.Add(costcodeId);
            param.Add(departstructureId);
            param.Add(dailytimesheetId);
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.TimesheetDTO>>("JsonGetTimesheetByWorkdateDailyTimeSheet", param, JsonHelper.ProjectService);

            return retValue;
        }

        //기존에 서비스호출 주석되어 있음 => 동일하게 추석처리
        //Json 메서드에 파라미터 ishistory 추가되어 있음
        public async Task<List<DataLibrary.TimesheetDTO>> GetTimesheetByWorkdateCostcodeDepartstructure(int costcodeId, int departstructureId, DateTime workDate, int projectId, string disciplineCode)
        {
            List<DataLibrary.TimesheetDTO> retValue = new List<DataLibrary.TimesheetDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(workDate);
            param.Add(costcodeId);
            param.Add(departstructureId);
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add("");  //ishistory
            //retValue = await JsonHelper.GetDataAsync<List<DataLibrary.TimesheetDTO>>("JsonGetTimesheetByWorkdateCostcodeDepartstructure", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.DailytimesheetDTO>> GetDailytimesheetByID(int dailytimesheetId)
        {
            List<DataLibrary.DailytimesheetDTO> retValue = new List<DataLibrary.DailytimesheetDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(dailytimesheetId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.DailytimesheetDTO>>("JsonGetDailytimesheetByID", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.MTODTO>> GetComponentProgressByFIWPDone(int cwpId, int materialcategoryId, int fiwpId, int projectId, string disciplineCode, DateTime workDate, int ruleofcreditweightId)
        {
            List<DataLibrary.MTODTO> retValue = new List<DataLibrary.MTODTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(materialcategoryId);
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(workDate);
            param.Add(ruleofcreditweightId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.MTODTO>>("JsonGetComponentProgressByFIWPDone", param, JsonHelper.ProjectService);

            return retValue;
        }

        // Save이기 때문에 PUT으로 되어야 하지만 GET으로 되어 있음
        public async Task<bool> SaveDailyTimehseet(int dataId, int isDirect, int dailyTimesheetId, int status, string updatedBy, DateTime workDate, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(workDate);
            param.Add(dataId);
            param.Add(isDirect);
            param.Add(dailyTimesheetId);
            param.Add(status);
            param.Add(updatedBy);
            param.Add(projectId);
            param.Add(disciplineCode);
            return await JsonHelper.GetDataAsync<bool>("JsonSaveDailyTimehseet", param, JsonHelper.ProjectService);
        }

        // Save이기 때문에 PUT으로 되어야 하지만 GET으로 되어 있음
        public async Task<bool> SaveDailyTimehseet(int departstructueId, string updatedBy, DateTime workDate, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(workDate);
            param.Add(departstructueId);
            param.Add(1);
            param.Add(0);
            param.Add(TrackTimeSheetStatus.Submit);
            param.Add(updatedBy);
            param.Add(projectId);
            param.Add(disciplineCode);
            return await JsonHelper.GetDataAsync<bool>("JsonSaveDailyTimehseet", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.TimesheetDTO>> SaveTimesheet(List<DataLibrary.TimesheetDTO> updates, List<DataLibrary.MTODTO> progresses, decimal workhour)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("timesheet", updates);
            dParams.Add("progresses", progresses);
            dParams.Add("workhour", workhour);
            dParams.Add("timeallocationId", -1);
            return await JsonHelper.PutDataAsync<List<DataLibrary.TimesheetDTO>>("JsonSaveTimesheet", dParams, JsonHelper.ProjectService);
        }
        //todo 노석진:서비스 확정되지 않아 원본 주석처리 서비스 나오면 삭제 예정
        /*
        public async Task<DataLibrary.MTOAndDrawing> GetComponentProgressForSchedulingWithList(int cwpId, int drawingId,
                                                                    List<string> taskCategoryCodeList, List<string> taskCategoryIdList,
                                                                    List<int> systemIdList, List<int> typeLUIdList,
                                                                    List<int> drawingtypeLUIdList, List<int> costcodeIdList,
                                                                    List<DataLibrary.ComboCodeBoxDTO> searchstringList, List<DataLibrary.ComboCodeBoxDTO> compsearchstringList,
                                                                    List<DataLibrary.ComboCodeBoxDTO> locationList, List<string> rfinumberList,
                                                                    string searchcolumn, List<string> searchvalueList,
                                                                    int projectId, string disciplineCode, int grouppage)
        {
            DataLibrary.MTOAndDrawing retValue = new DataLibrary.MTOAndDrawing();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("cwpId", cwpId);
            dParams.Add("drawingId", drawingId);
            dParams.Add("taskCategoryCodeList", taskCategoryCodeList);
            dParams.Add("taskCategoryIdList", taskCategoryIdList);
            dParams.Add("systemIdList", systemIdList);
            dParams.Add("typeLUIdList", typeLUIdList);
            dParams.Add("drawingtypeLUIdList", drawingtypeLUIdList);
            dParams.Add("costcodeIdList", costcodeIdList);
            dParams.Add("searchstringList", searchstringList);
            dParams.Add("compsearchstringList", compsearchstringList);
            dParams.Add("locationList", locationList);
            dParams.Add("rfinumberList", rfinumberList);
            dParams.Add("searchcolumn", searchcolumn);
            dParams.Add("searchvalueList", searchvalueList);
            dParams.Add("projectId", projectId);
            dParams.Add("disciplineCode", disciplineCode);
            dParams.Add("grouppage", grouppage);
            var result = await JsonHelper.PutDataAsync<DataLibrary.MTOAndDrawing>("JsonGetComponentProgressForSchedulingWithList", dParams, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }*/

        public async Task<DataLibrary.MTOAndDrawing> GetComponentProgressForSchedulingWithList(int cwpId, int drawingId,
                                                                    List<string> taskCategoryIdList, List<string> taskTypeLUIDList,
                                                                    List<string> materialIDList, List<string> progressIDList,
                                                                    string searchValue, int projectId, string disciplineCode, int grouppage)
        {
            DataLibrary.MTOAndDrawing retValue = new DataLibrary.MTOAndDrawing();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("cwpId", cwpId);
            dParams.Add("drawingId", drawingId);
            dParams.Add("taskCategoryCodeList", taskCategoryIdList);
            dParams.Add("taskTypeIdList", taskTypeLUIDList);
            dParams.Add("materialIdList", materialIDList);
            dParams.Add("progressIdList", progressIDList);
            dParams.Add("searchValue", searchValue);
            dParams.Add("projectId", projectId);
            dParams.Add("disciplineCode", disciplineCode);
            dParams.Add("grouppage", grouppage);
            var result = await JsonHelper.PutDataAsync<DataLibrary.MTOAndDrawing>("JsonGetComponentProgressForSchedulingWithList", dParams, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<List<DataLibrary.MTODTO>> GetComponentProgressByFIWP(int fiwpId, int projectScheduleID, int projectId, string disciplineCode)
        {
            List<DataLibrary.MTODTO> retValue = new List<DataLibrary.MTODTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);
            param.Add(projectScheduleID);
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.MTODTO>>("JsonGetComponentProgressByFIWP", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.MTODTO>> GetComponentProgressBySIWP(int fiwpId, int projectScheduleID, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);
            param.Add(projectScheduleID);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.MTODTO>>("JsonGetComponentProgressBySIWP", param, JsonHelper.ProjectService);

            //DataLibrary.ProjectServiceClient project = ServiceHelper.GetServiceClient<DataLibrary.ProjectServiceClient>(ServiceHelper.ProjectService);
            //var retValue = await project.GetComponentProgressBySIWPAsync(Helper.DBInstance, fiwpId, projectScheduleID, projectId, disciplineCode);
            //return retValue;
        }

        #endregion

        #region "CWP"
        public async Task<List<DataLibrary.CwpDTO>> GetCWPsByProjectID(int projectId, string disciplineCode, string userId)
        {
            List<DataLibrary.CwpDTO> retValue = new List<DataLibrary.CwpDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(userId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.CwpDTO>>("JsonGetCWPsByProjectID", param, JsonHelper.ProjectService);

            return retValue;
        }





        public async Task<List<DataLibrary.CwpDTO>> GetCwpByProject(int projectId, string disciplineCode)
        {
            List<DataLibrary.CwpDTO> retValue = new List<DataLibrary.CwpDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.CwpDTO>>("JsonGetCwpByProject", param, JsonHelper.ProjectService);

            return retValue;
        }



        public async Task<List<DataLibrary.CwpDTO>> GetCwpByProjectPackageType(int projectId, string disciplineCode, string packagetypeLuid, string userId)
        {
            List<DataLibrary.CwpDTO> retValue = new List<DataLibrary.CwpDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(packagetypeLuid);
            param.Add(userId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.CwpDTO>>("JsonGetCwpByProjectPackageType", param, JsonHelper.ProjectService);

            return retValue;
        }

        #endregion

        #region "Schedule"
        public async Task<List<DataLibrary.ProjectscheduleDTO>> GetProjectScheduleByProjectWithWBS(int projectId, string disciplineCode)
        {
            List<DataLibrary.ProjectscheduleDTO> retValue = new List<DataLibrary.ProjectscheduleDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectscheduleDTO>>("JsonGetProjectScheduleByProjectWithWBS", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.ProjectscheduleDTO>> GetProjectScheduleByCwpProjectIDWithWBS(int cwpId, int projectId)//, string disciplineCode)
        {
            List<DataLibrary.ProjectscheduleDTO> retValue = new List<DataLibrary.ProjectscheduleDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectId);
            //param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectscheduleDTO>>("JsonGetProjectScheduleByCwpProjectIDWithWBS", param, JsonHelper.ProjectService);

            return retValue;
        }
        

        public async Task<List<DataLibrary.ProjectscheduleDTO>> GetProjectScheduleByCwpProjectWithWBS(int cwpId, int projectId)
        {
            List<DataLibrary.ProjectscheduleDTO> retValue = new List<DataLibrary.ProjectscheduleDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectscheduleDTO>>("JsonGetProjectScheduleByCwpProjectWithWBS", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.ProjectscheduleDTO>> GetProjectScheduleByCwpProjectPackageTypeWithWBS(int cwpId, int projectId, string packagetypeLuid)
        {
            List<DataLibrary.ProjectscheduleDTO> retValue = new List<DataLibrary.ProjectscheduleDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectId);
            param.Add(packagetypeLuid);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectscheduleDTO>>("JsonGetProjectScheduleByCwpProjectPackageTypeWithWBS", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<DataLibrary.ProjectscheduleDTO> GetSignleProjscheduleByID(int projectScheduleId)
        {
            DataLibrary.ProjectscheduleDTO retValue = new DataLibrary.ProjectscheduleDTO();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectScheduleId);
            var result = await JsonHelper.GetDataAsync<DataLibrary.ProjectscheduleDTO>("JsonGetSignleProjscheduleByID", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<List<DataLibrary.ProjectscheduleDTO>> GetProjectScheduleByID(int projectScheduleId)
        {
            List<DataLibrary.ProjectscheduleDTO> retValue = new List<DataLibrary.ProjectscheduleDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectScheduleId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectscheduleDTO>>("JsonGetProjectScheduleByID", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.ProjectscheduleDTO>> SaveProjectSchedule(List<DataLibrary.ProjectscheduleDTO> schedules)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("schedule", schedules);
            return await JsonHelper.PutDataAsync<List<DataLibrary.ProjectscheduleDTO>>("JsonSaveProjectSchedule", dParams, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.ProgressAssignment> UpdateProjectScheduleAssignment(DataLibrary.ProgressAssignment progress)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("progress", progress);
            return await JsonHelper.PutDataAsync<DataLibrary.ProgressAssignment>("JsonUpdateProjectScheduleAssignment", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.ProjectscheduleDTO>> GetProjectScheduleByCWPProjectID(int cwpId, int projectId, string disciplineCode)
        {
            List<DataLibrary.ProjectscheduleDTO> retValue = new List<DataLibrary.ProjectscheduleDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectscheduleDTO>>("JsonGetProjectScheduleByCWPProjectID", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.ProjectscheduleDTO>> GetProjectScheduleAllByProjectIDdisciplineCode(int projectId, string disciplineCode)
        {
            List<DataLibrary.ProjectscheduleDTO> retValue = new List<DataLibrary.ProjectscheduleDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ProjectscheduleDTO>>("JsonGetProjectScheduleByProjectID", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.SystemDTO>> GetSystemByProjectID(int projectId)
        {
            List<DataLibrary.SystemDTO> retValue = new List<DataLibrary.SystemDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.SystemDTO>>("JsonGetSystemByProjectID", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<DataLibrary.ProjectscheduleDTO> UpdateProjectSchedulePeriod(ProjectscheduleDTO schedule, string totalManhours)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("schedule", schedule);
            dParams.Add("totalManhours", totalManhours);

             return await JsonHelper.PutDataAsync<DataLibrary.ProjectscheduleDTO>("JsonUpdateProjectSchedulePeriod", dParams, JsonHelper.ProjectService);
        }
        #endregion

        #region Assemble

        public async Task<List<DataLibrary.CollectionDTO>> GetAvailableCollectionForScheduling(int cwpId, int projectScheduleId, int projectId, string disciplineCode, int iwpId)
        {
            List<DataLibrary.CollectionDTO> retValue = new List<DataLibrary.CollectionDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectScheduleId);
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(iwpId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.CollectionDTO>>("JsonGetAvailableCollectionForScheduling", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.CollectionDTO>> GetAvailableCollectionForHydroScheduling(int cwpId, int projectScheduleId, int projectId, string disciplineCode)
        {
            List<DataLibrary.CollectionDTO> retValue = new List<DataLibrary.CollectionDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectScheduleId);
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.CollectionDTO>>("JsonGetAvailableCollectionForHydroScheduling", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.CollectionDTO>> GetAvailableCollectionForSchedulingApp(int cwpId, int projectScheduleId, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectScheduleId);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.CollectionDTO>>("JsonGetAvailableCollectionForSchedulingApp", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpequipDTO>> GetFiwpEquipByFIWP(int id)
        {
            List<DataLibrary.FiwpequipDTO> retValue = new List<DataLibrary.FiwpequipDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(id);
            //param.Add(projectId);
            //param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpequipDTO>>("JsonGetFiwpEquipByFIWP", param, JsonHelper.ProjectService);
            
            return retValue;
        }

        public async Task<List<DataLibrary.FiwpequipDTO>> SaveFiwpequip(List<DataLibrary.FiwpequipDTO> fiwpequip)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwpequip", fiwpequip);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpequipDTO>>("JsonSaveFiwpequip", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpequipDTO>> SaveFiwpequipForAssembleIWP(List<DataLibrary.FiwpequipDTO> fiwpequips, List<DataLibrary.FiwpDTO> fiwps, string userId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwpequips", fiwpequips);
            dParams.Add("fiwps", fiwps);
            dParams.Add("userId", userId);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpequipDTO>>("JsonSaveFiwpequipForAssembleIWP", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpmaterialDTO>> SaveFiwpmaterial(List<DataLibrary.FiwpmaterialDTO> fiwpmaterial)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwpmaterial", fiwpmaterial);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpmaterialDTO>>("JsonSaveFiwpMaterial", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpmaterialDTO>> SaveFiwpMaterialForAssembleIWP(List<DataLibrary.FiwpmaterialDTO> fiwpmaterial, List<DataLibrary.FiwpDTO> fiwps, string userId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwpmaterials", fiwpmaterial);
            dParams.Add("fiwps", fiwps);
            dParams.Add("userId", userId);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpmaterialDTO>>("JsonSaveFiwpMaterialForAssembleIWP", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.LibconsumableDTO>> GetLibconsumableAll()
        {
            List<DataLibrary.LibconsumableDTO> retValue = new List<DataLibrary.LibconsumableDTO>();

            List<dynamic> param = new List<dynamic>();
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.LibconsumableDTO>>("JsonGetLibconsumableAll", param, JsonHelper.ProjectService);
            
            return retValue;
        }

        public async Task<List<DataLibrary.FiwpmaterialDTO>> GetFiwpMaterialByFIWP(int fiwpId, int projectId, string disciplineCode)
        {
            List<DataLibrary.FiwpmaterialDTO> retValue = new List<DataLibrary.FiwpmaterialDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpmaterialDTO>>("JsonGetFiwpMaterialByFIWP", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.FiwpwfpDTO>> GetFiwpwfpByFiwp(int fiwpId, int projectId, string disciplineCode)
        {
            List<DataLibrary.FiwpwfpDTO> retValue = new List<DataLibrary.FiwpwfpDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpwfpDTO>>("JsonGetFiwpWFPByFIWP", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.FiwpwfpDTO>> SaveFiwpwfp(List<DataLibrary.FiwpwfpDTO> fiwpwfpdto)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwpwfp", fiwpwfpdto);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpwfpDTO>>("JsonSaveFiwpwfp", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpwfpDTO>> SaveFiwpwfpForAssembleIWP(List<DataLibrary.FiwpwfpDTO> fiwpwfpdto, List<DataLibrary.FiwpDTO> fiwps, List<DataLibrary.DocumentDTO> documents)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwpwfps", fiwpwfpdto);
            dParams.Add("fiwps", fiwps);
            dParams.Add("documents", documents);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpwfpDTO>>("JsonSaveFiwpwfpForAssembleIWP", dParams, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.DocumentDTO> SaveProjectDocumentWithSharePointForModelView(List<DataLibrary.FiwpDTO> fiwps,
            DataLibrary.DocumentDTO document, string docName, string cwpName, string fiwpName)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwps", fiwps);
            dParams.Add("document", document);
            dParams.Add("docName", docName);
            dParams.Add("cwpName", cwpName);
            dParams.Add("fiwpName", fiwpName);
            return await JsonHelper.PutDataAsync<DataLibrary.DocumentDTO>("JsonSaveProjectDocumentWithSharePointForModelView", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.DocumentDTO>> GetDocumentForFIWPByDocType(string doctypeId, int fiwpId, int projectId, string disciplineCode)
        {
            List<DataLibrary.DocumentDTO> retValue = new List<DataLibrary.DocumentDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(doctypeId);
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.DocumentDTO>>("JsonGetDocumentForFIWPByDocType", param, JsonHelper.ProjectService);
            
            return retValue;
        }


        public async Task<List<DataLibrary.DocumentDTO>> SaveProjectDocumentWithSharePointForWFP(
           DataLibrary.DocumentDTO document, string cwpName, string fiwpName, int siteimage)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("document", document);
            dParams.Add("cwpName", cwpName);
            dParams.Add("fiwpName", fiwpName);
            dParams.Add("packagetypeLuid", siteimage);
            dParams.Add("currentStep", 0);
            return await JsonHelper.PutDataAsync<List<DataLibrary.DocumentDTO>>("JsonSaveProjectDocumentWithSharePointForWFP", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpqaqcDTO>> SaveFiwpqaqc(List<DataLibrary.FiwpqaqcDTO> fiwpqaqc)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwpqaqc", fiwpqaqc);
            return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpqaqcDTO>>("JsonSaveFiwpqaqc", dParams, JsonHelper.ProjectService);
        }

        //public async Task<List<DataLibrary.FiwpqaqcDTO>> SaveFiwpqaqcForAssembleIWP(List<DataLibrary.FiwpqaqcDTO> fiwpqaqc, List<DataLibrary.FiwpDTO> fiwps, List<DataLibrary.DocumentDTO> documents)
        //{
        //    Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
        //    dParams.Add("fiwpqaqcs", fiwpqaqc);
        //    dParams.Add("fiwps", fiwps);
        //    dParams.Add("documents", documents);
        //    return await JsonHelper.PutDataAsync<List<DataLibrary.FiwpqaqcDTO>>("JsonSaveFiwpqaqcForAssembleIWP", dParams, JsonHelper.ProjectService);
        //}

        public async Task<List<DataLibrary.DocumentDTO>> SaveDocumentForAssembleIWP(List<DataLibrary.FiwpDTO> fiwps, List<DataLibrary.DocumentDTO> documents, string curStepCode, string userId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwps", fiwps);
            dParams.Add("documents", documents);
            dParams.Add("curStepCode", curStepCode);
            dParams.Add("userId", userId);
            return await JsonHelper.PutDataAsync<List<DataLibrary.DocumentDTO>>("JsonSaveDocumentForAssembleIWP", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.DocumentDTO>> SaveDocumentWithOZformForAssembleIWP(List<DataLibrary.FiwpDTO> fiwps, UpfileDTOS upFileCollection, string curStepCode, string userId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("fiwps", fiwps);
            dParams.Add("upFileCollection", upFileCollection);
            dParams.Add("curStepCode", curStepCode);
            dParams.Add("userId", userId);
            return await JsonHelper.PutDataAsync<List<DataLibrary.DocumentDTO>>("JsonSaveDocumentWithOZformForAssembleIWP", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.QaqcformtemplateDTO>> GetQaqcformtemplateByNameProject(string templetename, int projectId)
        {
            List<DataLibrary.QaqcformtemplateDTO> retValue = new List<DataLibrary.QaqcformtemplateDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(templetename);
            param.Add(projectId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.QaqcformtemplateDTO>>("JsonGetQaqcformtemplateByNameProject", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.FiwpqaqcDTO>> GetFiwpqaqcByFIWP(int fiwpId)
        {
            List<DataLibrary.FiwpqaqcDTO> retValue = new List<DataLibrary.FiwpqaqcDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpqaqcDTO>>("JsonGetFiwpqaqcByFIWP", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.DocumentDTO>> GetIwpDocumentByIwpProjectFileType(int iwpId, int projectId, string fileTypeCode, string isDisplayable, string fileCategory, string iwpDocumentId)
        {
            List<DataLibrary.DocumentDTO> retValue = new List<DataLibrary.DocumentDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(iwpId);
            param.Add(projectId);
            param.Add(fileTypeCode);
            param.Add(isDisplayable);
            param.Add(fileCategory);
            param.Add(iwpDocumentId);

            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.DocumentDTO>>("JsonGetIwpDocumentByIwpProjectFileType", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.DocumentDTO>> GetUploadedFileInfoByProjectFileType(int projectId, string fileTypeCode, string fileCategory)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(fileTypeCode);
            param.Add(fileCategory);

            return await JsonHelper.GetDataAsync<List<DataLibrary.DocumentDTO>>("JsonGetUploadedFileInfoByProjectFileType", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.FiwpDTO>> GetFiwpByScheduleID(int projectscheduleId)
        {
            List<DataLibrary.FiwpDTO> retValue = new List<DataLibrary.FiwpDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(projectscheduleId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpDTO>>("JsonGetFiwpByScheduleID", param, JsonHelper.ProjectService);

            return retValue;
        }

        public async Task<List<DataLibrary.FiwpDTO>> GetFiwpByCwpSchedule(int cwpId, int projectscheduleId)
        {
            List<DataLibrary.FiwpDTO> retValue = new List<DataLibrary.FiwpDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectscheduleId);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpDTO>>("JsonGetFiwpByCwpSchedule", param, JsonHelper.ProjectService);
            
            return retValue;
        }

        public async Task<List<DataLibrary.FiwpDTO>> GetFiwpByCwpSchedulePackageType(int cwpId, int projectscheduleId, string packagetypeLuid)
        {
            List<DataLibrary.FiwpDTO> retValue = new List<DataLibrary.FiwpDTO>();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(projectscheduleId);
            param.Add(packagetypeLuid);
            retValue = await JsonHelper.GetDataAsync<List<DataLibrary.FiwpDTO>>("JsonGetFiwpByCwpSchedulePackageType", param, JsonHelper.ProjectService);
            
            return retValue;
        }

        public async Task<List<DataLibrary.DrawingDTO>> test(DrawingDTO drawing)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("drawing", drawing);
            
            return await JsonHelper.PutDataAsync<List<DataLibrary.DrawingDTO>>("UploadTestImage", dParams, JsonHelper.ProjectService);

        }

        #endregion

        #region MTO

        public async Task<DataLibrary.MTOPageTotal> GetMaterialTakeOff(int cwpId, int drawingId, int materialCategyrId, int projectId, string disciplineCode)
        {
            DataLibrary.MTOPageTotal retValue = new DataLibrary.MTOPageTotal();

            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(drawingId);
            param.Add(materialCategyrId);
            param.Add(0);
            param.Add(0);
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(1);
            param.Add(10000);
            var result = await JsonHelper.GetDataAsync<DataLibrary.MTOPageTotal>("JsonGetMaterialTakeOff", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<List<DataLibrary.ProgressDTO>> DeleteMto(List<DataLibrary.MTODTO> mtodto, string updatedBy, string userName, string password, int rfiid)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("progress", mtodto);
            dParams.Add("updatedBy", updatedBy);
            dParams.Add("userName", userName);
            dParams.Add("password", password);
            dParams.Add("rfiid", rfiid);

            return await JsonHelper.PutDataAsync<List<DataLibrary.ProgressDTO>>("JsonDeleteMTO", dParams, JsonHelper.ProjectService);
        }

        #endregion

        #region Library

        public async Task<DataLibrary.LibgroundingmanhourPageTotal> GetLibGroundingManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            DataLibrary.LibgroundingmanhourPageTotal retValue = new DataLibrary.LibgroundingmanhourPageTotal();

            List<dynamic> param = new List<dynamic>();
            param.Add(selectedPage);
            param.Add(pageSize);
            param.Add(taskType);
            param.Add(partNumber);
            param.Add(Vender);
            var result = await JsonHelper.GetDataAsync<DataLibrary.LibgroundingmanhourPageTotal>("JsonGetLibGroundingManhourForPaging", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<DataLibrary.LibequipmanhourPageTotal> GetLibEquipManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(selectedPage);
            param.Add(pageSize);
            param.Add(taskType);
            param.Add(partNumber);
            param.Add(Vender);

            return await JsonHelper.GetDataAsync<DataLibrary.LibequipmanhourPageTotal>("JsonGetLibEquipManhourForPaging", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.LiblightingmanhourPageTotal> GetLibLightingManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(selectedPage);
            param.Add(pageSize);
            param.Add(taskType);
            param.Add(partNumber);
            param.Add(Vender);

            return await JsonHelper.GetDataAsync<DataLibrary.LiblightingmanhourPageTotal>("JsonGetLibLightingManhourForPaging", param, JsonHelper.ProjectService);

        }

        public async Task<DataLibrary.LibracewaymanhourPageTotal> GetLibRacewayManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            DataLibrary.LibracewaymanhourPageTotal retValue = new DataLibrary.LibracewaymanhourPageTotal();

            List<dynamic> param = new List<dynamic>();
            param.Add(selectedPage);
            param.Add(pageSize);
            param.Add(taskType);
            param.Add(partNumber);
            param.Add(Vender);
            var result = await JsonHelper.GetDataAsync<DataLibrary.LibracewaymanhourPageTotal>("JsonGetLibRacewayManhourForPaging", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<DataLibrary.LibinstrmanhourPageTotal> GetLibInstrManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            DataLibrary.LibinstrmanhourPageTotal retValue = new DataLibrary.LibinstrmanhourPageTotal();

            List<dynamic> param = new List<dynamic>();
            param.Add(selectedPage);
            param.Add(pageSize);
            param.Add(taskType);
            param.Add(partNumber);
            param.Add(Vender);
            var result = await JsonHelper.GetDataAsync<DataLibrary.LibinstrmanhourPageTotal>("JsonGetLibInstrManhourForPaging", param, JsonHelper.ProjectService);

            retValue = result;

            return retValue;
        }

        public async Task<DataLibrary.LibehtmanhourPageTotal> GetLibEhtManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(selectedPage);
            param.Add(pageSize);
            param.Add(taskType);
            param.Add(partNumber);
            param.Add(Vender);

            return await JsonHelper.GetDataAsync<DataLibrary.LibehtmanhourPageTotal>("JsonGetLibEhtManhourForPaging", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.LibcablemanhourPageTotal> GetLibCableManhourForPaging(int selectedPage, int pageSize, string taskType, string partNumber, string Vender)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(selectedPage);
            param.Add(pageSize);
            param.Add(taskType);
            param.Add(partNumber);
            param.Add(Vender);

            return await JsonHelper.GetDataAsync<DataLibrary.LibcablemanhourPageTotal>("JsonGetLibCableManhourForPaging", param, JsonHelper.ProjectService);
        }

        #endregion

        #region "Qaqc" 
        public async Task<List<DataLibrary.FiwpqaqcDTO>> GetITRListByFiwp(int fiwpId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.FiwpqaqcDTO>>("JsonGetFiwpqaqcByFIWP", param, JsonHelper.ProjectService);
        }
        
        public async Task<List<DataLibrary.QaqcformDTO>> SaveQaqcformForDownload(int projectId, string disciplineCode, int cwpId, int fiwpId, List<DataLibrary.QaqcformtemplateDTO> qaqcformtemplate, string updatedBy, int qaqcType )
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("projectId", projectId);
            dParams.Add("disciplineCode", disciplineCode);
            dParams.Add("cwpId", cwpId);
            dParams.Add("fiwpId", fiwpId);
            dParams.Add("qaqcformtemplate", qaqcformtemplate);
            dParams.Add("updatedBy", updatedBy);
            dParams.Add("QAQCDataTypeLUID", qaqcType);
            return await JsonHelper.PutDataAsync<List<DataLibrary.QaqcformDTO>>("JsonSaveQaqcformForDownload", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.QaqcformDTO>> SaveQaqcformForSubmit(List<DataLibrary.QaqcformDTO> qaqcforms)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("qaqcforms", qaqcforms);
            return await JsonHelper.PutDataAsync<List<DataLibrary.QaqcformDTO>>("JsonSaveQaqcformForSubmit", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.QaqcformDTO>> GetQaqcformByQcManager(int projectid, string disciplineCode, int loginId)
        {
            //DataLibrary.ProjectServiceClient project = ServiceHelper.GetServiceClient<DataLibrary.ProjectServiceClient>(ServiceHelper.ProjectService);
            //var retValue = await project.GetQaqcformByQcManagerAsync(Helper.DBInstance, projectid, disciplineCode, loginId, WinAppLibrary.Utilities.QAQCDataType.ITR);
            //await project.CloseAsync();
            //return retValue;

            List<dynamic> param = new List<dynamic>();
            param.Add(projectid);
            param.Add(disciplineCode);
            param.Add(loginId);
            param.Add(DataLibrary.Utilities.QAQCDataType.ITR);
            return await JsonHelper.GetDataAsync<List<DataLibrary.QaqcformDTO>>("JsonGetQaqcformByQcManager", param, JsonHelper.ProjectService);
        }

        // Download Punch Ticket List
        public async Task<List<DataLibrary.QaqcformdetailDTO>> GetQaqcformByPersonnelDepartment(int projectId, string disciplineCode, string personnelId, int departmentId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(personnelId);
            param.Add(departmentId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.QaqcformdetailDTO>>("JsonGetQaqcformByPersonnelDepartment", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.QaqcformDTO> GetQaqcformbyID(int qaqcformId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(qaqcformId);

            return await JsonHelper.GetDataAsync<DataLibrary.QaqcformDTO>("JsonGetQaqcformByID", param, JsonHelper.ProjectService);
        }

        // Get Punch Ticket
        public async Task<DataLibrary.PunchDTOSet> GetPunchListByQaqcform(int qaqcformId)
        {
            //DataLibrary.ProjectServiceClient project = ServiceHelper.GetServiceClient<DataLibrary.ProjectServiceClient>(ServiceHelper.ProjectService);
            //var retValue = await project.GetPunchListByQaqcformAsync(Helper.DBInstance, qaqcformId);
            //await project.CloseAsync();
            //return retValue;

            List<dynamic> param = new List<dynamic>();
            param.Add(qaqcformId);

            return await JsonHelper.GetDataAsync<DataLibrary.PunchDTOSet>("JsonGetPunchListByQaqcform", param, JsonHelper.ProjectService);
        }

        // Save Punch Ticket
        public async Task<DataLibrary.WalkdownDTOSet> SaveQaqcformWithSharePoint(DataLibrary.WalkdownDTOSet qaqcforms)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("walkdownDs", qaqcforms);
            return await JsonHelper.PutDataAsync<DataLibrary.WalkdownDTOSet>("JsonSaveQaqcformWithSharepoint", dParams, JsonHelper.ProjectService);
        }
        #endregion

        #region "Tuen Over"

        public async Task<List<DataLibrary.ProjectDTO>> GetContractorProejctByOwnerProject(int projectId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ProjectDTO>>("JsonGetContractorProejctByOwnerProject", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.SystemDTO>> GetSystemByTurnoverProject(int projectId, int constractorId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(constractorId);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.SystemDTO>>("JsonGetSystemByTurnoverProject", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.ModuleDTO>> GetModuleByTurnoverSystem(int projectId, int constractorId, int systemId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(constractorId);
            param.Add(systemId);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.ModuleDTO>>("JsonGetModuleByTurnoverSystem", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.WalkdownDTOSet> GetWalkDownBySystem(int projectId, int constractorId, int systemId, List<int> disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(constractorId);
            param.Add(systemId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<DataLibrary.WalkdownDTOSet>("JsonGetWalkDownBySystem", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.QaqcformDTO>> GetQaqcformBySystem(int systemId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(systemId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.QaqcformDTO>>("JsonGetQaqcformBySystem", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.WalkdownDTOSet> SaveQaqcformWithSharepoint(DataLibrary.WalkdownDTOSet walkdownDs)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("walkdownDs", walkdownDs);

            return await JsonHelper.PutDataAsync<DataLibrary.WalkdownDTOSet>("JsonSaveQaqcformWithSharepoint", dParams, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.QaqcformDTO> SaveTurnoverCertificateForMC(DataLibrary.DocumentDTO documentdto, DataLibrary.QaqcformDTO qaqcformdto, int systemId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("documentDTO", documentdto);
            dParams.Add("qaqcformDTO", qaqcformdto);
            dParams.Add("systemId", systemId);

            return await JsonHelper.PutDataAsync<DataLibrary.QaqcformDTO>("JsonSaveTurnoverCertificateForMC", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.DocumentDTO>> SaveTurnoverCertificatePDFForMC(DataLibrary.DocumentDTO documentdto, DataLibrary.QaqcformDTO qaqcformdto, int systemId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("documentDTO", documentdto);
            dParams.Add("qaqcformDTO", qaqcformdto);
            dParams.Add("systemId", systemId);

            return await JsonHelper.PutDataAsync<List<DataLibrary.DocumentDTO>>("JsonSaveTurnoverCertificatePDFForMC", dParams, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.QaqcformDTO> SaveTurnoverCertificateForTCCC(DataLibrary.DocumentDTO documentdto, DataLibrary.QaqcformDTO qaqcformdto, int systemId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("documentDTO", documentdto);
            dParams.Add("qaqcformDTO", qaqcformdto);
            dParams.Add("systemId", systemId);

            return await JsonHelper.PutDataAsync<DataLibrary.QaqcformDTO>("JsonSaveTurnoverCertificateForTCCC", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.DocumentDTO>> SaveTurnoverCertificatePDFForTCCC(DataLibrary.DocumentDTO documentdto, DataLibrary.QaqcformDTO qaqcformdto, int systemId)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("documentDTO", documentdto);
            dParams.Add("qaqcformDTO", qaqcformdto);
            dParams.Add("systemId", systemId);

            return await JsonHelper.PutDataAsync<List<DataLibrary.DocumentDTO>>("JsonSaveTurnoverCertificatePDFForTCCC", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.DocumentDTO>> GetDocumentByTurnoverPackage(string lookupValue, int fiwpId, int projectId, string disciplineCode)
        {
            //DataLibrary.ProjectServiceClient project = ServiceHelper.GetServiceClient<DataLibrary.ProjectServiceClient>(ServiceHelper.ProjectService);
            //var retValue = await project.GetDocumentByTurnoverPackageAsync(Helper.DBInstance, lookupValue, fiwpId, projectId, disciplineCode);
            //await project.CloseAsync();
            //return retValue.ToList();

            List<dynamic> param = new List<dynamic>();
            param.Add(lookupValue);
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.DocumentDTO>>("JsonGetDocumentByTurnoverPackage", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.QaqcformDTO> GetTurnoverCertificateForMC(int projectId, int systemId)
        {
            //DataLibrary.ProjectServiceClient project = ServiceHelper.GetServiceClient<DataLibrary.ProjectServiceClient>(ServiceHelper.ProjectService);
            //var retValue = await project.GetTurnoverCertificateForMCAsync(Helper.DBInstance, projectId, systemId);
            //await project.CloseAsync();
            //return retValue;

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(systemId);

            return await JsonHelper.GetDataAsync<DataLibrary.QaqcformDTO>("JsonGetTurnoverCertificateForMC", param, JsonHelper.ProjectService);
        }

        public async Task<DataLibrary.QaqcformDTO> GetTurnoverCertificateForTCCC(int projectId, int systemId)
        {
            //DataLibrary.ProjectServiceClient project = ServiceHelper.GetServiceClient<DataLibrary.ProjectServiceClient>(ServiceHelper.ProjectService);
            //var retValue = await project.GetTurnoverCertificateForTCCCAsync(Helper.DBInstance, projectId, systemId);
            //await project.CloseAsync();
            //return retValue;

            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(systemId);

            return await JsonHelper.GetDataAsync<DataLibrary.QaqcformDTO>("JsonGetTurnoverCertificateForTCCC", param, JsonHelper.ProjectService);
        }

        #endregion

        #region Report
        public async Task<List<DataLibrary.rptQAQCformDTO>> GetITRReportBySystem(int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.rptQAQCformDTO>>("JsonGetITRChartbySystem", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.rptQAQCformDTO>> GetITRReportByCWP(int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.rptQAQCformDTO>>("JsonGetITRChartbyCWP", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.rptPunchDTO>> GetPunchReportByDisc(int projectId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.rptPunchDTO>>("JsonGetPunchChartbyDiscipline", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.rptPunchDTO>> GetPunchReportByCat(int projectId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.rptPunchDTO>>("JsonGetPunchChartbyCAT", param, JsonHelper.ProjectService);
        }
        #endregion

        #region "Drawing"

        public async Task<List<DataLibrary.DrawingDTO>> GetDrawingByProjectID(int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.DrawingDTO>>("JsonGetDrawingsByProjectID", param, JsonHelper.ProjectService);
        }

        #endregion

        #region csu

        public async Task<List<DataLibrary.FiwpDTO>> GetFiwpBySystemPackageType(int projectId, int systemId, string type)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(systemId);
            param.Add(type);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.FiwpDTO>>("JsonGetFiwpBySystemPackageType", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.DrawingDTO>> GetDrawingByDrawingType(int drawingtypeLuid, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(drawingtypeLuid);
            param.Add(projectId);
            param.Add(disciplineCode);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.DrawingDTO>>("JsonGetDrawingByDrawingType", param, JsonHelper.ProjectService);;
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> SavePnIDDrawingForBuildCSU(List<DataLibrary.ComboBoxDTO> drawings, List<DataLibrary.FiwpDTO> fiwps)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("drawings", drawings);
            dParams.Add("fiwps", fiwps);
            return await JsonHelper.PutDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonSavePnIDDrawingForBuildCSU", dParams, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.DocumentDTO>> SaveAssociatedDocumentForBuildCSU(List<DataLibrary.DocumentDTO> AssociatedDocs, List<DataLibrary.FiwpDTO> fiwps)
        {
            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("associatedDocs", AssociatedDocs);
            dParams.Add("fiwps", fiwps);
            dParams.Add("currentStep", 0);
            return await JsonHelper.PutDataAsync<List<DataLibrary.DocumentDTO>>("JsonSaveAssociatedDocumentForBuildCSU", dParams, JsonHelper.ProjectService);
        }

        // Json Method와 uriTemplate이 다름
        public async Task<DataLibrary.FiwpDocument> GetFIWPDocDrawingsByCSU(int fiwpId, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<DataLibrary.FiwpDocument>("JsonGetCSUDocDrawingsByFIWP", param, JsonHelper.ProjectService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetDrawingByDrawingType_Combo(int drawingtypeLuid, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(drawingtypeLuid);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetDrawingByDrawingType_Combo", param, JsonHelper.ProjectService);
        }

        #endregion
    }
}
