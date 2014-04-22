using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;
using Element.Reveal.DataLibrary;

namespace Element.Reveal.TrueTask.Lib.ServiceModel
{
    public class CommonModel
    {
        Helper _helper = new Helper();

        #region "ComboBox"
        public async Task<IEnumerable<DataLibrary.ComboCodeBoxDTO>> GetCWPByProjectID_Combo(int projectId, string disciplineCode, string userId)
        {
            // IEnumerable<DataLibrary.ComboBoxDTO> retValue = new List<DataLibrary.ComboBoxDTO>();
            List<DataLibrary.ComboCodeBoxDTO> retValue = new List<DataLibrary.ComboCodeBoxDTO>();
            try
            {
                List<dynamic> param = new List<dynamic>();
                param.Add(projectId);
                param.Add(disciplineCode);
                param.Add(userId);

                retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetCWPByProject_Combo", param, JsonHelper.CommonService);
            }

            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetCWPByProjectID");
            }

            return retValue;
        }

        public async Task<List<DataLibrary.ComboCodeBoxDTO>> GetLookup_Combo(string LookupType, string LookupSubType)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(LookupType);
            param.Add(LookupSubType);

            return  await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetLookupByLookupType_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetCWPByProject_Combo_Mobile(int projectId, string disciplineCode, string userId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(userId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetCWPByProject_Combo_Mobile", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetFIWPByProjectSchedule_Combo(int projectScheduleId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectScheduleId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetFIWPByProjectSchedule_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetFIWPByProjectSchedulePackageType_Combo(int projectScheduleId, string packagetype)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectScheduleId);
            param.Add(packagetype);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetFIWPByProjectSchedulePackageType_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetForemanByGeneralForeman_Combo(string sigmaUserId, string sigmaRoleId, string projectId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(sigmaUserId);
            param.Add(sigmaRoleId);
            param.Add(projectId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetForemanByGeneralForeman_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetFIWPByCwp_Combo(int cwpId, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(0);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetFIWPByCwp_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetMaterialCategoryByModule_Combo(string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetMaterialCategoryByModule_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetCrewAndForemanByFiwpWorkDate_Combo(int cwpId, int fiwpId, int projectId, string disciplineCode, DateTime workDate)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(fiwpId);
            param.Add(cwpId);
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(workDate);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetCrewAndForemanByFiwpWorkDate_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetRuleofCreditByMaterialCategory_Combo(int projectId, string disciplineCode, int materialcategoryId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(materialcategoryId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetRuleofCreditByMaterialCategory_Combo", param, JsonHelper.CommonService);
        }

        //public async Task<List<DataLibrary.ComboCodeBoxDTO>> GetGeneralForemanByProject_Combo(int projectId) //, string disciplineCode)
        //{
        //    List<dynamic> param = new List<dynamic>();
        //    param.Add(projectId);
        //    //param.Add(disciplineCode);

        //    return await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetGeneralForemanByProject_Combo", param, JsonHelper.CommonService);
        //}
        public async Task<List<DataLibrary.ComboCodeBoxDTO>> GetGeneralForemanByProject_Combo(string roleTypeCode, string reportTo, string reportToRoleTypeCode, string projectId) //, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            
            param.Add(roleTypeCode);
            param.Add(reportTo);
            param.Add(reportToRoleTypeCode);
            param.Add(projectId);
            //param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetSigmaUserSigmaRoleBySigmaRoleReportToReportToRoleProject", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetSystemByProject_Combo(int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetSystemByProject_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetCostCodeByProject_Combo(int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetCostCodeByProject_Combo", param, JsonHelper.CommonService);
        }

        public async Task<IEnumerable<DataLibrary.ComboCodeBoxDTO>> GetFIWPByProject_Combo(int projectId, string disciplineCode)
        {
            //IEnumerable<DataLibrary.ComboBoxDTO> retValue = new List<DataLibrary.ComboBoxDTO>();
            List<DataLibrary.ComboCodeBoxDTO> retValue = new List<DataLibrary.ComboCodeBoxDTO>();
            try
            {
                
                List<dynamic> param = new List<dynamic>();
                param.Add(projectId);
                param.Add(disciplineCode);
                retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetFIWPByProject_Combo", param, JsonHelper.CommonService);
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetFIWPyProjectID");
            }

            return retValue;
        }

        public async Task<IEnumerable<DataLibrary.ComboCodeBoxDTO>> GetDrawingType_Combo()
        {
            // IEnumerable<DataLibrary.ComboBoxDTO> retValue = new List<DataLibrary.ComboBoxDTO>();
            List<DataLibrary.ComboCodeBoxDTO> retValue = new List<DataLibrary.ComboCodeBoxDTO>();
            try
            {
                
                List<dynamic> param = new List<dynamic>();
                param.Add(DataLibrary.Utilities.LOOKUPTYPE.DrawingType);
                param.Add("");
                retValue = await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetLookupByLookupType_Combo", param, JsonHelper.CommonService);
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetDrawingType");
            }

            return retValue;
        }

        public async Task<List<DataLibrary.ComboCodeBoxDTO>> GetCWPByProject_Combo(int projectId, string disciplineCode, string userId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(userId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetCWPByProject_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetDrawingByCWP(int cwpId, int materialCategyrId, string engTagNumber, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(cwpId);
            param.Add(materialCategyrId);
            param.Add(engTagNumber);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetDrawingByCWP_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboCodeBoxDTO>> GetForemanGeneralForemanNameByPersonnelDepartment_Combo(int personnelId, int departmentId, int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(personnelId);
            param.Add(departmentId);
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetForemanGeneralForemanNameByPersonnelDepartment_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboBoxDTO>> GetTurnoverPackageByBinderPage_Combo(int projectId, string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetTurnoverPackageByBinderPage_Combo", param, JsonHelper.CommonService);
        }

        #endregion

        #region "Module"
        public async Task<List<DataLibrary.ModuleDTO>> GetAllModule()
        {
            List<dynamic> param = new List<dynamic>();

            return await JsonHelper.GetDataAsync<List<DataLibrary.ModuleDTO>>("JsonGetModuleAll", param, JsonHelper.CommonService);
        }

        public async Task<DataLibrary.ModuleDTO> GetModuleByID(string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<DataLibrary.ModuleDTO>("JsonGetModuleByID", param, JsonHelper.CommonService);
        }
        #endregion

        #region "Qaqc"
        public async Task<List<DataLibrary.ComboBoxDTO>> GetFIWPByPersonnelDepartmentFiwpqaqc_Combo(int projectId, string disciplineCode, int personnelId, int departmentId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectId);
            param.Add(disciplineCode);
            param.Add(personnelId);
            param.Add(departmentId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboBoxDTO>>("JsonGetFIWPByPersonnelDepartmentFiwpqaqc_Combo", param, JsonHelper.CommonService);
        }
        #endregion

        #region "Personnel"
        public async Task<DataLibrary.PersonnelDTO> GetSinglePersonnelByID(int personnelId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(personnelId);

            return await JsonHelper.GetDataAsync<DataLibrary.PersonnelDTO>("JsonGetSinglePersonnelByID", param, JsonHelper.CommonService);
        }
        #endregion

        #region "Schedule"

        public async Task<List<DataLibrary.TaskcategoryDTO>> GetTaskcategoryByMaterialCategory(string disciplineCode, int materialCategoryId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(disciplineCode);
            param.Add(materialCategoryId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.TaskcategoryDTO>>("JsonGetTaskcategoryByMaterialCategory", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ExtSchedulerDTO>> GetFIWPByProjectSchedule_ExtSch(string projectscheduleId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(projectscheduleId);

            return await JsonHelper.GetDataAsync<List<DataLibrary.ExtSchedulerDTO>>("JsonGetFIWPByProjectSchedule_ExtSch", param, JsonHelper.CommonService);
        }

        #endregion

        #region "Lookup"

        public async Task<List<DataLibrary.LookupDTO>> GetLookupByLookupType(string lookuptype)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(lookuptype);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.LookupDTO>>("JsonGetLookupByLookupType", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboCodeBoxDTO>> GetLookupByLookupType_Combo(string lookuptype, string lookupsubtype)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(lookuptype);
            param.Add(lookupsubtype);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetLookupByLookupType_Combo", param, JsonHelper.CommonService);
        }


        public async Task<List<DataLibrary.FieldequipmentDTO>> GetFieldequipmentByType(string search)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(search);
            
            return await JsonHelper.GetDataAsync<List<DataLibrary.FieldequipmentDTO>>("JsonGetFieldequipmentByType", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.ComboCodeBoxDTO>> GetUOM_Combo()
        {
            List<dynamic> param = new List<dynamic>();

            return await JsonHelper.GetDataAsync<List<DataLibrary.ComboCodeBoxDTO>>("JsonGetUOM_Combo", param, JsonHelper.CommonService);
        }

        public async Task<List<DataLibrary.MaterialcategoryDTO>> GetMaterialCategoryByModuleID(string disciplineCode)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(disciplineCode);

            return await JsonHelper.GetDataAsync<List<DataLibrary.MaterialcategoryDTO>>("JsonGetMaterialCategoryByModuleID", param, JsonHelper.CommonService);
        }

        #endregion

        #region "Depart Structure"
        public async Task<List<DataLibrary.DepartstructureDTO>> GetDepartStructureByPersonnelID(int personnelID)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(personnelID);

            return await JsonHelper.GetDataAsync<List<DataLibrary.DepartstructureDTO>>("JsonGetDepartstructureByID", param, JsonHelper.CommonService);
        }
        #endregion
    }

}
