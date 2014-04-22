using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Meg.Lib
{
    public class CWPDataSource
    {
        private List<RevealProjectSvc.CwpDTO> _cwp = new List<RevealProjectSvc.CwpDTO>();
        public List<RevealProjectSvc.CwpDTO> CWP { get { return _cwp; } }
        public static int selectedCWP { get; set; }
        public static string selectedCWPName { get; set; }


        public async Task<bool> GetCWPsByProjectIDOnMode(int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetCWPsByProjectID(projectId, moduleId);
                _cwp = result;

                if (_cwp != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetCwpByProjectOnMode(int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetCwpByProject(projectId, moduleId);
                _cwp = result;

                if (_cwp != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetCwpByProjectPackageTypeOnMode(int projectId, int moduleId, int packagetypeLuid)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetCwpByProjectPackageType(projectId, moduleId, packagetypeLuid);
                _cwp = result;

                if (_cwp != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public List<RevealProjectSvc.CwpDTO> GetCWPs()
        {
            List<RevealProjectSvc.CwpDTO> list = new List<RevealProjectSvc.CwpDTO>();
            list = _cwp; 

            return list;
        }
    }
}
