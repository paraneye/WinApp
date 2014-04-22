using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Sigma.Lib.ServiceModel
{
    public class CommonModel
    {
        #region "ComboBox"
        public async Task<List<RevealCommonSvc.ComboBoxDTO>> GetCWPByProject_Combo_Mobile(int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetCWPByProject_Combo_MobileAsync(Helper.DBInstance, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetCWPByProject_Combo_MobileResult.ToList();
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
    }
}
