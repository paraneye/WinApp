using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Meg.Lib.ServiceModel
{
    public class CommonModel
    {
        Helper _helper = new Helper();

        #region "ComboBox"
        public async Task<IEnumerable<RevealCommonSvc.ComboBoxDTO>> GetCWPByProjectID_Combo(int projectId, int moduleId)
        {
            IEnumerable<RevealCommonSvc.ComboBoxDTO> retValue = new List<RevealCommonSvc.ComboBoxDTO>();
            try
            {
                RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
                var result = await common.GetCWPByProject_ComboAsync(Helper.DBInstance, projectId, moduleId);
                retValue = result.GetCWPByProject_ComboResult.ToList();
                await common.CloseAsync();
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetCWPByProjectID");
            }

            return retValue;
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetLookup_Combo(string LookupType, string LookupSubType)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetLookupByLookupType_ComboAsync(Helper.DBInstance, LookupType, LookupSubType);
            await common.CloseAsync();
            return retValue.GetLookupByLookupType_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetCWPByProject_Combo_Mobile(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetCWPByProject_Combo_MobileAsync(Helper.DBInstance, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetCWPByProject_Combo_MobileResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetFIWPByProjectSchedule_Combo(int projectScheduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetFIWPByProjectSchedule_ComboAsync(Helper.DBInstance, projectScheduleId);
            await common.CloseAsync();
            return retValue.GetFIWPByProjectSchedule_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetFIWPByProjectSchedulePackageType_Combo(int projectScheduleId, int packagetype)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetFIWPByProjectSchedulePackageType_ComboAsync(Helper.DBInstance, projectScheduleId, packagetype);
            await common.CloseAsync();
            return retValue.GetFIWPByProjectSchedulePackageType_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetForemanByGeneralForeman_Combo(int departStructureID, int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetForemanByGeneralForeman_ComboAsync(Helper.DBInstance, departStructureID, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetForemanByGeneralForeman_ComboResult.ToList();
        }

        

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetFIWPByCwp_Combo(int cwpId, int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetFIWPByCwp_ComboAsync(Helper.DBInstance, cwpId, 0, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetFIWPByCwp_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetMaterialCategoryByModule_Combo(int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetMaterialCategoryByModule_ComboAsync(Helper.DBInstance, moduleId);
            await common.CloseAsync();
            return retValue.GetMaterialCategoryByModule_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetCrewAndForemanByFiwpWorkDate_Combo(int cwpId, int fiwpId, int projectId, int moduleId, DateTime workDate)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetCrewAndForemanByFiwpWorkDate_ComboAsync(Helper.DBInstance, fiwpId, cwpId, projectId, moduleId, workDate);
            await common.CloseAsync();
            return retValue.GetCrewAndForemanByFiwpWorkDate_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetRuleofCreditByMaterialCategory_Combo(int projectId, int moduleId, int materialcategoryId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetRuleofCreditByMaterialCategory_ComboAsync(Helper.DBInstance, projectId, moduleId, materialcategoryId);
            await common.CloseAsync();
            return retValue.GetRuleofCreditByMaterialCategory_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetGeneralForemanByProject_Combo(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetGeneralForemanByProject_ComboAsync(Helper.DBInstance, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetGeneralForemanByProject_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetSystemByProject_Combo(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetSystemByProject_ComboAsync(Helper.DBInstance, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetSystemByProject_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetCostCodeByProject_Combo(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetCostCodeByProject_ComboAsync(Helper.DBInstance, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetCostCodeByProject_ComboResult.ToList();
        }

        public async Task<IEnumerable<RevealCommonSvc.ComboBoxDTO>> GetFIWPByProject_Combo(int projectId, int moduleId)
        {
            IEnumerable<RevealCommonSvc.ComboBoxDTO> retValue = new List<RevealCommonSvc.ComboBoxDTO>();
            try
            {
                RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
                var result = await common.GetFIWPByProject_ComboAsync(Helper.DBInstance, projectId, moduleId);
                retValue = result.GetFIWPByProject_ComboResult.ToList();
                await common.CloseAsync();
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetFIWPyProjectID");
            }

            return retValue;
        }

        public async Task<IEnumerable<RevealCommonSvc.ComboBoxDTO>> GetDrawingType_Combo()
        {
            IEnumerable<RevealCommonSvc.ComboBoxDTO> retValue = new List<RevealCommonSvc.ComboBoxDTO>();
            try
            {
                RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
                var result = await common.GetLookupByLookupType_ComboAsync(Helper.DBInstance, WinAppLibrary.Utilities.LOOKUPTYPE.DrawingType, "");
                retValue = result.GetLookupByLookupType_ComboResult.ToList();
                await common.CloseAsync();
            }
            catch (Exception e)
            {
                _helper.ExceptionHandler(e, "GetDrawingType");
            }

            return retValue;
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetCWPByProject_Combo(int projectId, int moduleId) 
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetCWPByProject_ComboAsync(Helper.DBInstance, projectId, moduleId);
            
            await common.CloseAsync();
            return retValue.GetCWPByProject_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetDrawingByCWP(int cwpId, int materialCategyrId, string engTagNumber, int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetDrawingByCWP_ComboAsync(Helper.DBInstance, cwpId, materialCategyrId, "", projectId, moduleId);

            await common.CloseAsync();
            return retValue.GetDrawingByCWP_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetForemanGeneralForemanNameByPersonnelDepartment_Combo(int personnelId, int departmentId, int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetForemanGeneralForemanNameByPersonnelDepartment_ComboAsync(Helper.DBInstance, personnelId, departmentId, projectId, moduleId);

            await common.CloseAsync();
            return retValue.GetForemanGeneralForemanNameByPersonnelDepartment_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetTurnoverPackageByBinderPage_Combo(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetTurnoverPackageByBinderPage_ComboAsync(Helper.DBInstance,  projectId, moduleId);

            await common.CloseAsync();
            return retValue.GetTurnoverPackageByBinderPage_ComboResult.ToList();
        }

        #endregion

        #region "Module"
        public async Task<List<RevealCommonSvc.ModuleDTO>> GetAllModule()
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetModuleAllAsync(Helper.DBInstance);
            await c.CloseAsync();
            return retValue.GetModuleAllResult.ToList();
        }

        public async Task<RevealCommonSvc.ModuleDTO> GetModuleByID(int moduleId)
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetModuleByIDAsync(Helper.DBInstance, moduleId);
            await c.CloseAsync();
            return retValue.GetModuleByIDResult;
        }
        #endregion

        #region "Qaqc"
        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetFIWPByPersonnelDepartmentFiwpqaqc_Combo(int projectId, int moduleId, int personnelId, int departmentId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetFIWPByPersonnelDepartmentFiwpqaqc_ComboAsync(Helper.DBInstance, projectId, moduleId, personnelId, departmentId);
            await common.CloseAsync();
            return retValue.GetFIWPByPersonnelDepartmentFiwpqaqc_ComboResult.ToList();
        }
        #endregion

        #region "Personnel"
        public async Task<RevealCommonSvc.PersonnelDTO> GetSinglePersonnelByID(int personnelId)
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetSinglePersonnelByIDAsync(Helper.DBInstance, personnelId);
            await c.CloseAsync();
            return retValue.GetSinglePersonnelByIDResult;
        }
        #endregion
        #region "Schedule"

        public async Task<List<RevealCommonSvc.TaskcategoryDTO>> GetTaskcategoryByMaterialCategory(int moduleId, int materialCategoryId)
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetTaskcategoryByMaterialCategoryAsync(Helper.DBInstance, moduleId, materialCategoryId);
            await c.CloseAsync();
            return retValue.GetTaskcategoryByMaterialCategoryResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ExtSchedulerDTO>> GetFIWPByProjectSchedule_ExtSch(int projectscheduleId)
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetFIWPByProjectSchedule_ExtSchAsync(Helper.DBInstance, projectscheduleId);
            await c.CloseAsync();
            return retValue.GetFIWPByProjectSchedule_ExtSchResult.ToList();
        }

        #endregion

        #region "Lookup"

        public async Task<List<RevealCommonSvc.LookupDTO>> GetLookupByLookupType(string lookuptype)
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetLookupByLookupTypeAsync(Helper.DBInstance, lookuptype);
            await c.CloseAsync();
            return retValue.GetLookupByLookupTypeResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetLookupByLookupType_Combo(string lookuptype, string lookupsubtype)
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetLookupByLookupType_ComboAsync(Helper.DBInstance, lookuptype, lookupsubtype);
            await c.CloseAsync();
            return retValue.GetLookupByLookupType_ComboResult.ToList();
        }


        public async Task<List<RevealCommonSvc.FieldequipmentDTO>> GetFieldequipmentByType(string search)
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetFieldequipmentByTypeAsync(Helper.DBInstance, search);
            await c.CloseAsync();
            return retValue.GetFieldequipmentByTypeResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetUOM_Combo()
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetUOM_ComboAsync(Helper.DBInstance);
            await c.CloseAsync();
            return retValue.GetUOM_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.MaterialcategoryDTO>> GetMaterialCategoryByModuleID(int moduleId)
        {
            RevealCommonSvc.CommonServiceClient c = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await c.GetMaterialCategoryByModuleIDAsync(Helper.DBInstance, moduleId);
            await c.CloseAsync();
            return retValue.GetMaterialCategoryByModuleIDResult.ToList();
        }
        

        #endregion

        #region "Depart Structure"
        public async Task<List<RevealCommonSvc.DepartstructureDTO>> GetDepartStructureByPersonnelID(int personnelID)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetDepartStructureByPersonnelIDAsync(Helper.DBInstance, personnelID);
            await common.CloseAsync();

            return retValue.GetDepartStructureByPersonnelIDResult.ToList();
        }
        #endregion
    }

}
