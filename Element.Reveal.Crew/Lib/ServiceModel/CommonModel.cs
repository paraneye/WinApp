using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Crew.Lib.ServiceModel
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
        //todo 추가중
        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetUOMByProject_Combo_Mobile(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetUOM_ComboAsync(Helper.DBInstance);
            await common.CloseAsync();
            return retValue.GetUOM_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetCWPByProject_Combo_Mobile(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetCWPByProject_Combo_MobileAsync(Helper.DBInstance, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetCWPByProject_Combo_MobileResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetFIWPByProject_Combo(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetFIWPByProject_ComboAsync(Helper.DBInstance, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetFIWPByProject_ComboResult.ToList();
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

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetRuleofCreditByMaterialCategory_Combo(int projectId, int moduleId, int materialcategoryId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetRuleofCreditByMaterialCategory_ComboAsync(Helper.DBInstance, projectId, moduleId, materialcategoryId);
            await common.CloseAsync();
            return retValue.GetRuleofCreditByMaterialCategory_ComboResult.ToList();
        }

        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetCrewAndForemanByFiwpWorkDate_Combo(int cwpId, int fiwpId, int projectId, int moduleId, DateTime workDate)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetCrewAndForemanByFiwpWorkDate_ComboAsync(Helper.DBInstance, fiwpId, cwpId, projectId, moduleId, workDate);
            await common.CloseAsync();
            return retValue.GetCrewAndForemanByFiwpWorkDate_ComboResult.ToList();
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
