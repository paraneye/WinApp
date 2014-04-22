using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.TrueTask.Lib.ServiceModel
{
    public class WorkflowModel
    {
        DataLibrary.SigmaResultTypeDTO returenJson;
        Helper _helper = new Helper();

        public async Task<List<DataLibrary.WorkflowByIWPID>> GetWorkflowByIWPID(string WorkflowTypeCode)
        {
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            List<dynamic> param = new List<dynamic>();
            param.Add(WorkflowTypeCode);

            returenJson = await JsonHelper.GetDataAsync<DataLibrary.SigmaResultTypeDTO>("GetWorkflowByIWPID", param, JsonHelper.WorkflowService);
            List<DataLibrary.WorkflowByIWPID> result = null;
            if (returenJson.IsSuccessful == true)
            {
                result = JsonHelper.Deserialize<List<DataLibrary.WorkflowByIWPID>>(returenJson.JsonDataSet);

                foreach (var item in result)
                {
                    item.AffectedRow = returenJson.AffectedRow;
                }
            }

            return result;
        }

        public async Task<List<DataLibrary.Department>> GetDepartmentUsed(int ProjectId)
        {
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            List<dynamic> param = new List<dynamic>();
            param.Add(ProjectId);

            returenJson = await JsonHelper.GetDataAsync<DataLibrary.SigmaResultTypeDTO>("GetDepartmentUsed", param, JsonHelper.WorkflowService);
            List<DataLibrary.Department> result = null;
            if (returenJson.IsSuccessful == true)
            {
                result = JsonHelper.Deserialize<List<DataLibrary.Department>>(returenJson.JsonDataSet);

                foreach (var item in result)
                {
                    item.AffectedRow = returenJson.AffectedRow;
                }
            }

            return result;
        }

        public async Task<List<DataLibrary.CrewByDepartmentID>> GetCrewByDepartmentID(int ProjectId, int SigmaRoleId, string SigmaUserID)
        {
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            List<dynamic> param = new List<dynamic>();
            
            param.Add(ProjectId);
            param.Add(SigmaRoleId);
            param.Add(SigmaUserID);

            returenJson = await JsonHelper.GetDataAsync<DataLibrary.SigmaResultTypeDTO>("GetCrewByDepartmentID", param, JsonHelper.WorkflowService);
            List<DataLibrary.CrewByDepartmentID> result = null;
            if (returenJson.IsSuccessful == true)
            {
                result = JsonHelper.Deserialize<List<DataLibrary.CrewByDepartmentID>>(returenJson.JsonDataSet);

                foreach (var item in result)
                {
                    item.AffectedRow = returenJson.AffectedRow;
                }
            }

            return result;
        }

        public async Task<bool> SaveWorkflowCrew(string WorkflowTypeCode, int TransitionStatusSeq, string LoginID, List<DataLibrary.TypeTransition> TransitionLst, string Title, string Context, string Comment, int TargetId, int IwpId)
        {
            bool result = false;
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("WorkflowTypeCode", WorkflowTypeCode);
            dParams.Add("TransitionStatusSeq", TransitionStatusSeq);
            dParams.Add("LoginID", LoginID);
            dParams.Add("TransitionLst", TransitionLst);
            dParams.Add("Title", Title);
            dParams.Add("Context", Context);
            dParams.Add("Comment", Comment);
            dParams.Add("TargetId", TargetId);
            dParams.Add("IwpId", IwpId);

            returenJson = await JsonHelper.PutDataAsync<DataLibrary.SigmaResultTypeDTO>("SaveWorkflowCrew", dParams, JsonHelper.WorkflowService);
            result = returenJson.IsSuccessful;

            return result;
        }

        public async Task<List<DataLibrary.IWPWorkflowStatusBypersonnelid_type_term>> GetIWPWorkflowStatusBypersonnelid_type_term(string UserId, string StartDt, string EndDt, string IsProcessStatus)
        {
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            List<dynamic> param = new List<dynamic>();
            param.Add(UserId);
            param.Add(StartDt);
            param.Add(EndDt);
            param.Add(IsProcessStatus);
            //returenJson = await JsonHelper.GetDataAsync<DataLibrary.SigmaResultTypeDTO>("GetIWPWorkflowStatusBypersonnelid_type_term", param, JsonHelper.WorkflowService);
            returenJson = await JsonHelper.GetDataAsync<DataLibrary.SigmaResultTypeDTO>("GetWorkflowTransitionHistoryList", param, JsonHelper.WorkflowService);
            List<DataLibrary.IWPWorkflowStatusBypersonnelid_type_term> result = null;
            if (returenJson.IsSuccessful == true)
            {
                result = JsonHelper.Deserialize<List<DataLibrary.IWPWorkflowStatusBypersonnelid_type_term>>(returenJson.JsonDataSet);

                foreach (var item in result)
                {
                    item.AffectedRow = returenJson.AffectedRow;
                }
            }

            return result;
        }

        public async Task<List<DataLibrary.WorkflowDetailByIWPID>> GetWorkflowDetailByIWPID(Guid processId)
        {
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("processId", processId);
                        
            //returenJson = await JsonHelper.PutDataAsync<DataLibrary.SigmaResultTypeDTO>("GetWorkflowDetailByIWPID", dParams, JsonHelper.WorkflowService);
            returenJson = await JsonHelper.PutDataAsync<DataLibrary.SigmaResultTypeDTO>("GetWorkflowTransitionHistory", dParams, JsonHelper.WorkflowService);
            List<DataLibrary.WorkflowDetailByIWPID> result = null;
            if (returenJson.IsSuccessful == true)
            {
                result = JsonHelper.Deserialize<List<DataLibrary.WorkflowDetailByIWPID>>(returenJson.JsonDataSet);

                foreach (var item in result)
                {
                    item.AffectedRow = returenJson.AffectedRow;
                }
            }

            return result;
        }

        public async Task<List<DataLibrary.PendingWorkflow>> GetPendingWorkflow(string PackageTypeCode, int TargetId)
        {
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            List<dynamic> param = new List<dynamic>();
            param.Add(PackageTypeCode);
            param.Add(TargetId);

            returenJson = await JsonHelper.GetDataAsync<DataLibrary.SigmaResultTypeDTO>("GetPendingWorkflow", param, JsonHelper.WorkflowService);
            List<DataLibrary.PendingWorkflow> result = null;
            if (returenJson.IsSuccessful == true)
            {
                result = JsonHelper.Deserialize<List<DataLibrary.PendingWorkflow>>(returenJson.JsonDataSet);

                foreach (var item in result)
                {
                    item.AffectedRow = returenJson.AffectedRow;
                }
            }

            return result;
        }

        public async Task<bool> SaveWorkflow(string WorkflowTypeCode, Guid ProcessGuid, Guid ApprovalGuid, string CommandName, string CurrentStatus, string Context, int TransitionStatusSeq, string Comment)
        {
            bool result = false;
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("WorkflowTypeCode", WorkflowTypeCode);
            dParams.Add("ProcessGuid", ProcessGuid);
            dParams.Add("ApprovalGuid", ApprovalGuid);
            dParams.Add("CommandName", CommandName);
            dParams.Add("CurrentStatus", CurrentStatus);
            dParams.Add("Context", Context);
            dParams.Add("TransitionStatusSeq", TransitionStatusSeq);
            dParams.Add("Comment", Comment);
            returenJson = await JsonHelper.PutDataAsync<DataLibrary.SigmaResultTypeDTO>("SaveWorkflow", dParams, JsonHelper.WorkflowService);
            result = returenJson.IsSuccessful;

            return result;
        }

        public async Task<List<string>> GetSigmaUserCommand(Guid SigmaUserGuid, Guid ProcessGuId, int Order)
        {
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("SigmaUserGuid", SigmaUserGuid);
            dParams.Add("ProcessGuId", ProcessGuId);
            dParams.Add("Order", Order);

            returenJson = await JsonHelper.PutDataAsync<DataLibrary.SigmaResultTypeDTO>("GetSigmaUserCommand", dParams, JsonHelper.WorkflowService);
            List<string> result = null;
            if (returenJson.IsSuccessful == true)
            {
                result = JsonHelper.Deserialize<List<string>>(returenJson.JsonDataSet);
            }

            return result;
        }

        public async Task<bool> UpdateWorkflowCrew(string WorkflowTypeCode, int TransitionStatusSeq, Guid WorkFlowId, List<DataLibrary.TypeTransition> TransitionLst, int TargetId)
        {
            bool result = false;
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("WorkflowTypeCode", WorkflowTypeCode);
            dParams.Add("TransitionStatusSeq", TransitionStatusSeq);
            dParams.Add("WorkFlowId", WorkFlowId);
            dParams.Add("TransitionLst", TransitionLst);
            dParams.Add("TargetId", TargetId);
            returenJson = await JsonHelper.PutDataAsync<DataLibrary.SigmaResultTypeDTO>("UpdateWorkflowCrew", dParams, JsonHelper.WorkflowService);
            result = returenJson.IsSuccessful;

            return result;
        }

        public async Task<bool> SaveWorkflowForEasy(string WorkflowTypeCode, int TargetId, int TargetSeq, bool IsAgree, string UserID, string Context, string Comment)
        {
            bool result = false;
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("WorkflowTypeCode", WorkflowTypeCode);
            dParams.Add("TargetId", TargetId);
            dParams.Add("TargetSeq", TargetSeq);
            dParams.Add("IsAgree", IsAgree);
            dParams.Add("UserID", UserID);
            dParams.Add("Context", Context);
            dParams.Add("Comment", Comment);
            
            returenJson = await JsonHelper.PutDataAsync<DataLibrary.SigmaResultTypeDTO>("SaveWorkflowForEasy", dParams, JsonHelper.WorkflowService);
            result = returenJson.IsSuccessful;

            return result;
        }

        public async Task<bool> SaveWorkflowForEasyEx(string WorkflowTypeCode, int TargetId, int TargetSeq, string AgreeInfo, string UserID, string Context, string Comment)
        {
            bool result = false;
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            Dictionary<string, dynamic> dParams = new Dictionary<string, dynamic>();
            dParams.Add("WorkflowTypeCode", WorkflowTypeCode);
            dParams.Add("TargetId", TargetId);
            dParams.Add("TargetSeq", TargetSeq);
            dParams.Add("AgreeInfo", AgreeInfo);
            dParams.Add("UserID", UserID);
            dParams.Add("Context", Context);
            dParams.Add("Comment", Comment);

            returenJson = await JsonHelper.PutDataAsync<DataLibrary.SigmaResultTypeDTO>("SaveWorkflowForEasyEx", dParams, JsonHelper.WorkflowService);
            result = returenJson.IsSuccessful;

            return result;
        }

        public async Task<int> GetDocumentTransitionStatusTotalCount(string UserId, string StartDt, string EndDt, string IsProcessStatus)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(UserId);
            param.Add(StartDt);
            param.Add(EndDt);
            param.Add(IsProcessStatus);

            return await JsonHelper.GetDataAsync<int>("GetDocumentTransitionStatusTotalCount", param, JsonHelper.WorkflowService);
        }

        public async Task<int> GetDocumentTransitionStatusTotalCount(string UserId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(UserId);

            return await JsonHelper.GetDataAsync<int>("GetDocumentTransitionStatusTotalCountForEasy", param, JsonHelper.WorkflowService);
        }

        public async Task<List<DataLibrary.PendingWorkflow>> GetWorkflowRoleTitle(string WorkflowTypeCode)
        {
            returenJson = new DataLibrary.SigmaResultTypeDTO();

            List<dynamic> param = new List<dynamic>();
            param.Add(WorkflowTypeCode);

            returenJson = await JsonHelper.GetDataAsync<DataLibrary.SigmaResultTypeDTO>("GetWorkflowRoleTitle", param, JsonHelper.WorkflowService);
            List<DataLibrary.PendingWorkflow> result = null;
            if (returenJson.IsSuccessful == true)
            {
                result = JsonHelper.Deserialize<List<DataLibrary.PendingWorkflow>>(returenJson.JsonDataSet);

                foreach (var item in result)
                {
                    item.AffectedRow = returenJson.AffectedRow;
                }
            }

            return result;
        }
    }
}
