using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Utilities;

using WinAppLibrary.Extensions;


namespace Element.Reveal.Crew.Lib.DataSource
{
    public class SelectFiwpITRDataSource
    {
        private static List<RevealCommonSvc.ComboBoxDTO> _FiwpList;
        public List<RevealCommonSvc.ComboBoxDTO> FiwpList { get { return _FiwpList; } }


        public async Task<bool> GetFiwpList(int projectId, int moduleId, int personnelId, int departmentId)
        {
            bool retValue = false;
            try
            {
                //var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByPersonnelDepartment_Combo(projectId, moduleId, personnelId, departmentId);
                var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByPersonnelDepartmentFiwpqaqc_Combo(projectId, moduleId, personnelId, departmentId);
                _FiwpList = result;

                if (_FiwpList != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<RevealCommonSvc.ComboBoxDTO> ReturnFiwpList()
        {
            return _FiwpList;
        }
        
    }
}
