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
    public class CWPDataSource
    {
        private List<DataLibrary.CwpDTO> _cwp = new List<DataLibrary.CwpDTO>();
        public List<DataLibrary.CwpDTO> CWP { get { return _cwp; } }
        public static int selectedCWP { get; set; }
        public static string selectedCWPName { get; set; }


        public async Task<bool> GetCWPsByProjectIDOnMode(int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetCWPsByProjectID(projectId, disciplineCode, Login.UserAccount.LoginName);
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

        public async Task<bool> GetCwpByProjectOnMode(int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetCwpByProject(projectId, disciplineCode);
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

        public async Task<bool> GetCwpByProjectPackageTypeOnMode(int projectId, string disciplineCode, string packagetypeLuid)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetCwpByProjectPackageType(projectId, disciplineCode, packagetypeLuid, Login.UserAccount.LoginName);
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

        public List<DataLibrary.CwpDTO> GetCWPs()
        {
            List<DataLibrary.CwpDTO> list = new List<DataLibrary.CwpDTO>();
            list = _cwp; 

            return list;
        }
    }
}
