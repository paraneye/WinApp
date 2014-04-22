using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.TrueVue.Lib.ServiceModel
{
    public class CommonModel
    {
        Helper _helper = new Helper();

        #region "Personnel"
        public async Task<RevealCommonSvc.PersonnelDTO> GetSinglePersonnelByID(int personnelId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetSinglePersonnelByIDAsync(Helper.DBInstance, personnelId);
            await common.CloseAsync();
            return retValue.GetSinglePersonnelByIDResult;
        }
        #endregion

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

        public static async Task<List<RevealCommonSvc.ComboBoxDTO>> GetForemanGeneralForemanNameByPersonnelDepartment_Combo(int personnelId, int departmentId, int projectId, int moduleId)
        {
            RevealCommonSvc.CommonServiceClient common = ServiceHelper.GetServiceClient<RevealCommonSvc.CommonServiceClient>(ServiceHelper.CommonService);
            var retValue = await common.GetForemanGeneralForemanNameByPersonnelDepartment_ComboAsync(Helper.DBInstance, personnelId, departmentId, projectId, moduleId);
            await common.CloseAsync();
            return retValue.GetForemanGeneralForemanNameByPersonnelDepartment_ComboResult.ToList();
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
