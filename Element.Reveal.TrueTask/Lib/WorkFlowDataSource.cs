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
    public class WorkFlowDataSource
    {
        private List<DataLibrary.WorkflowByIWPID> _workflowdto = new List<DataLibrary.WorkflowByIWPID>();
        public List<DataLibrary.WorkflowByIWPID> workflowdto { get { return _workflowdto; } }

        private List<DataLibrary.FiwpDTO> _fiwpdto = new List<DataLibrary.FiwpDTO>();
        public List<DataLibrary.FiwpDTO> fiwpdto { get { return _fiwpdto; } }

        private List<DataLibrary.Department> _departmentdto = new List<DataLibrary.Department>();
        public List<DataLibrary.Department> departmentdto { get { return _departmentdto; } }

        private List<DataLibrary.CrewByDepartmentID> _crewdto = new List<DataLibrary.CrewByDepartmentID>();
        public List<DataLibrary.CrewByDepartmentID> crewdto { get { return _crewdto; } }

        private List<DataLibrary.IWPWorkflowStatusBypersonnelid_type_term> _singoffstatus = new List<DataLibrary.IWPWorkflowStatusBypersonnelid_type_term>();
        public List<DataLibrary.IWPWorkflowStatusBypersonnelid_type_term> singoffstatus { get { return _singoffstatus; } }

        private List<DataLibrary.WorkflowDetailByIWPID> _workflowdetail = new List<DataLibrary.WorkflowDetailByIWPID>();
        public List<DataLibrary.WorkflowDetailByIWPID> workflowdetail { get { return _workflowdetail; } }

        private List<DataLibrary.PendingWorkflow> _pendginworkflow = new List<DataLibrary.PendingWorkflow>();
        public List<DataLibrary.PendingWorkflow> pendginworkflow { get { return _pendginworkflow; } }

        private DataLibrary.DocumentAndDrawing _documentanddrawing = new DataLibrary.DocumentAndDrawing();
        public DataLibrary.DocumentAndDrawing documentanddrawing { get { return _documentanddrawing; } }

        private List<DataLibrary.DocumentDTO> _document = new List<DataLibrary.DocumentDTO>();
        public List<DataLibrary.DocumentDTO> document { get { return _document; } }


        public static string PackageTypeCode { get; set; }

        public static int selectedDocumentID { get; set; }
        public static int selectedIwpID { get; set; }
        public static string selectedTypeName { get; set; }
        public static string sentyn { get; set; }

        public async Task<bool> GetIWPByIwpID(int iwpID)
        {
            bool retValue = false;
            _fiwpdto = new List<DataLibrary.FiwpDTO>();

            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByID(iwpID);
                _fiwpdto = result;

                if (_fiwpdto != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public List<DataLibrary.FiwpDTO> GetIWP()
        {
            return _fiwpdto;
        }

        public async Task<bool> GetWorkFlowByPackageTypeCode(string PackageTypeCode)
        {

            bool retValue = false;
            _workflowdto = new List<DataLibrary.WorkflowByIWPID>();

            try
            {
                var result = await (new Lib.ServiceModel.WorkflowModel()).GetWorkflowByIWPID(PackageTypeCode);
                _workflowdto = result;

                if (_workflowdto != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<DataLibrary.WorkflowByIWPID> GetWorkFlowType()
        {
            return _workflowdto;
        }
        
        public async Task<bool> GetDepartmentUsed(int projectid)
        {

            bool retValue = false;
            _departmentdto = new List<DataLibrary.Department>();

            try
            {
                var result = await (new Lib.ServiceModel.WorkflowModel()).GetDepartmentUsed(projectid);
                _departmentdto = result;

                if (_departmentdto != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<DataLibrary.Department> GetDepartment()
        {
            return _departmentdto;
        }

        public async Task<bool> GetCrewByDepartmentID(int SigmaRoleId)
        {

            bool retValue = false;
            _crewdto = new List<DataLibrary.CrewByDepartmentID>();

            try
            {
                var result = await (new Lib.ServiceModel.WorkflowModel()).GetCrewByDepartmentID(Login.UserAccount.CurProjectID, SigmaRoleId, Login.UserAccount.PersonnelId);
                _crewdto = result;

                if (_crewdto != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<DataLibrary.CrewByDepartmentID> GetCrewList()
        {
            return _crewdto;
        }

        public async Task<bool> GetIWPWorkflowStatusBypersonnelid_type_term(string userid, string startdate, string enddate, string processstatus)
        {

            bool retValue = false;
            _singoffstatus = new List<DataLibrary.IWPWorkflowStatusBypersonnelid_type_term>();

            try
            {
                var result = await (new Lib.ServiceModel.WorkflowModel()).GetIWPWorkflowStatusBypersonnelid_type_term(userid, startdate, enddate, processstatus);
                _singoffstatus = result;

                if (_singoffstatus != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<DataLibrary.IWPWorkflowStatusBypersonnelid_type_term> GetIWPWorkflowStatus()
        {
            return _singoffstatus;
        }

        public async Task<bool> GetWorkflowDetailByProcessID(Guid processId)
        {

            bool retValue = false;
            _workflowdetail = new List<DataLibrary.WorkflowDetailByIWPID>();

            try
            {
                var result = await (new Lib.ServiceModel.WorkflowModel()).GetWorkflowDetailByIWPID(processId);
                _workflowdetail = result;

                if (_workflowdetail != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<DataLibrary.WorkflowDetailByIWPID> GetWorkflowDetail()
        {
            return _workflowdetail;
        }

        public async Task<bool> GetPendingWorkflowByPackageTypeCode(string PackageTypeCode, int iwpid)
        {

            bool retValue = false;

            _pendginworkflow = new List<DataLibrary.PendingWorkflow>();

            try
            {
                var result = await (new Lib.ServiceModel.WorkflowModel()).GetPendingWorkflow(PackageTypeCode, iwpid);
                _pendginworkflow = result;

                if (_pendginworkflow != null)
                    retValue = true;               
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public async Task<bool> GetWorkflowRoleTitleByPackageTypeCode(string PackageTypeCode, int iwpid)
        {

            bool retValue = false;

            _pendginworkflow = new List<DataLibrary.PendingWorkflow>();

            try
            {
                var result = await (new Lib.ServiceModel.WorkflowModel()).GetWorkflowRoleTitle(PackageTypeCode);
                _pendginworkflow = result;

                if (_pendginworkflow != null)
                    retValue = true;
                
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<DataLibrary.PendingWorkflow> GetPendingWorkflow()
        {
            return _pendginworkflow;
        }

        public async Task<bool> GetFIWPDocDrawingsByFIWP(int iwpid, int projectid)
        {

            bool retValue = false;
            _documentanddrawing = new DataLibrary.DocumentAndDrawing();

            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByFIWP(iwpid, projectid, string.Empty);
                _documentanddrawing = result;

                if (_documentanddrawing != null)
                    retValue = true;

            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public DataLibrary.DocumentAndDrawing GetFIWPDocDrawings()
        {
            return _documentanddrawing;
        }

        public async Task<bool> GetIwpDocumentByIwpProjectFileType(int iwpid, int projectid, int documentid)
        {

            bool retValue = false;
            _document = new List<DataLibrary.DocumentDTO>();

            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetIwpDocumentByIwpProjectFileType(iwpid, projectid, DataLibrary.Utilities.FileType.ITR, "N", DataLibrary.Utilities.FileCategory.REPORT, documentid.ToString());
                _document = result;

                if (_document != null)
                    retValue = true;

            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<DataLibrary.DocumentDTO> GetIwpDocument()
        {
            return _document;
        }

        public async Task<bool> SaveWorkflowForEasy(string PackageTypeCode, int TargetId, int TargetSeq, bool IsAgree, string UserID, string Context, string Comment)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.WorkflowModel()).SaveWorkflowForEasy(PackageTypeCode, TargetId, TargetSeq, IsAgree, UserID, Context, Comment); 
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveWorkflowWithComment");
            }

            return retValue;
        }

    }
}
