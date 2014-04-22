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
    public class IWPDataSource
    {
        private RevealProjectSvc.FiwpDocument _iwpdrawing = new RevealProjectSvc.FiwpDocument();
        public RevealProjectSvc.FiwpDocument IWPDrawing { get { return _iwpdrawing; } }

        private List<RevealProjectSvc.DocumentDTO> _iwpDocumnet = new List<RevealProjectSvc.DocumentDTO>();

        private List<RevealCommonSvc.ComboBoxDTO> _iwplist = new List<RevealCommonSvc.ComboBoxDTO>();
        private List<RevealCommonSvc.ComboBoxDTO> _siwplist = new List<RevealCommonSvc.ComboBoxDTO>();
        private List<RevealCommonSvc.ComboBoxDTO> _hydrolist = new List<RevealCommonSvc.ComboBoxDTO>();
        private List<RevealProjectSvc.FiwpDTO> _iwps = new List<RevealProjectSvc.FiwpDTO>();
        private List<RevealProjectSvc.FiwpDTO> _fiwps = new List<RevealProjectSvc.FiwpDTO>();
        private List<RevealProjectSvc.FiwpDTO> _siwps = new List<RevealProjectSvc.FiwpDTO>();
        private List<RevealProjectSvc.FiwpDTO> _hydros = new List<RevealProjectSvc.FiwpDTO>();
        private List<RevealProjectSvc.FiwpDTO> _cus = new List<RevealProjectSvc.FiwpDTO>();
        private List<RevealCommonSvc.ComboBoxDTO> _foremanlist = new List<RevealCommonSvc.ComboBoxDTO>();
        private List<RevealCommonSvc.LookupDTO> _testtypelist = new List<RevealCommonSvc.LookupDTO>();
        
        public static int selectedIWP { get; set; }
        public static int selectedSIWP { get; set; }
        public static int selectedHydro { get; set; }
        public static int selectedCSU { get; set; }
        public static string selectedIWPName { get; set; }
        public static string selectedSIWPName { get; set; }
        public static string selectedHydroName { get; set; }
        public static string selectedCSUName { get; set; }
        public static bool isWizard { get; set; }
        public static List<RevealProjectSvc.FiwpDTO> iwplist { get; set; }
        public static List<RevealProjectSvc.FiwpDTO> csulist { get; set; }
        public static List<RevealCommonSvc.ComboBoxDTO> iwps { get; set; }
        public static List<RevealCommonSvc.ComboBoxDTO> hydros { get; set; }   


        public async Task<bool> GetIwpDrawingOnMode(int fiwpId, int projectId, int moduleId, string currentMenu)
        {
            bool retValue = false;
            try
            {
                RevealProjectSvc.FiwpDocument result = new RevealProjectSvc.FiwpDocument();
                //IWP or SIWP
                if (currentMenu == Lib.MainMenuList.IWPViewer)
                    result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByFIWP(fiwpId, projectId, moduleId);
                else if (currentMenu == Lib.MainMenuList.SIWPViewer)
                    result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsBySIWP(fiwpId, projectId, moduleId);
                else if (currentMenu == Lib.MainMenuList.HydroViewer)
                    result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByHydro(fiwpId, projectId, moduleId);
                else if (currentMenu == Lib.MainMenuList.CSUViewer)
                    result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByCSU(fiwpId, projectId, moduleId);
                _iwpdrawing = result;

                if (_iwpdrawing != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetFIWPByProjectScheduleOnMode(int projectScheduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByProjectSchedulePackageType_Combo(projectScheduleId, Lib.PackageType.FIWP);
                iwps = result;
                _iwplist = result;

                if (_iwplist != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetSIWPByProjectScheduleOnMode(int projectScheduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByProjectSchedulePackageType_Combo(projectScheduleId, Lib.PackageType.SIWP);
                _siwplist = result;

                if (_siwplist != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetHydroTestByProjectScheduleOnMode(int projectScheduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByProjectSchedulePackageType_Combo(projectScheduleId, Lib.PackageType.HydroTest);
                hydros = result;
                _hydrolist = result;

                if (_hydrolist != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }
/*
        public async Task<bool> GetCsuByProjectScheduleOnMode(int projectScheduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByProjectSchedulePackageType_Combo(projectScheduleId, Lib.PackageType.CSU);
                hydros = result;
                _hydrolist = result;

                if (_hydrolist != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }*/

        public async Task<bool> GetCSUPackageOnMode(int projectId, int systemId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpBySystemPackageType(projectId, systemId, Lib.PackageType.CSU);
                csulist = result;
                _cus = result;

                if (_cus != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetFiwpByIDOnMode(int iwpId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByID(iwpId);

                _iwps = result;

                if (_iwps != null)
                {
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetFiwpByScheduleIDOnMode(int projectscheduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByScheduleID(projectscheduleId);
                iwplist = result;
                _fiwps = result;

                if (_fiwps != null)
                {
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetFiwpByCwpScheduleOnMode(int cwpId, int projectscheduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByCwpSchedule(cwpId, projectscheduleId);
                iwplist = result;
                _fiwps = result;

                if (_fiwps != null)
                {
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetFiwpByCwpSchedulePackageTypeOnMode(int cwpId, int projectscheduleId, int packagetypeLuid)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByCwpSchedulePackageType(cwpId, projectscheduleId, packagetypeLuid);
                iwplist = result;
                _fiwps = result;

                if (_fiwps != null)
                {
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        
        

        public async Task<bool> GetSiwpByIDOnMode(int siwpId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByID(siwpId);

                _siwps = result;

                if (_siwps != null)
                {
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetHydroByIDOnMode(int siwpId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByID(siwpId);

                _hydros = result;

                if (_hydros != null)
                {
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }
        
        public async Task<bool> GetForemanByGeneralForemanOnMode(int departStructureID, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.CommonModel()).GetForemanByGeneralForeman_Combo(departStructureID, projectId, moduleId);
                _foremanlist = result;

                if (_foremanlist != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetTestTypeOnMode()
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.CommonModel()).GetLookupByLookupType(LOOKUPTYPE.ComponentTestType);
                _testtypelist = result.OrderBy(x => x.LookupID).ToList();

                if (_testtypelist != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }
        
        public List<RevealCommonSvc.ComboBoxDTO> GetFiwpByProjectSchedule()
        {
            return _iwplist;
        }

        public List<RevealCommonSvc.ComboBoxDTO> GetSiwpByProjectSchedule()
        {
            return _siwplist;
        }

        public List<RevealCommonSvc.ComboBoxDTO> GetHydroByProjectSchedule()
        {
            return _hydrolist;
        }

        public List<RevealProjectSvc.FiwpDTO> GetCsuByProjectSchedule()
        {
            return _cus;
        }


        public List<RevealProjectSvc.FiwpDTO> GetFiwpByID()
        {
            return _iwps;
        }

        public List<RevealProjectSvc.FiwpDTO> GetFiwpByProjectScheduleID()
        {
            return _fiwps;
        }

        public List<RevealProjectSvc.FiwpDTO> GetSiwpByID()
        {
            return _siwps;
        }

        public List<RevealProjectSvc.FiwpDTO> GetHydroByID()
        {
            return _hydros;
        }

        public async Task<bool> SaveFIWP(List<RevealProjectSvc.FiwpDTO> fiwp)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).SaveFIWP(fiwp);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveFIWP");
            }

            return retValue;
        }
        public async Task<bool> SaveHydro(List<RevealProjectSvc.FiwpDTO> fiwp)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).SaveHydro(fiwp);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveHydro");
            }

            return retValue;
        }

        public async Task<bool> SaveSIWP(List<RevealProjectSvc.FiwpDTO> fiwp)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).SaveSIWP(fiwp);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveSIWP");
            }

            return retValue;
        }

        public List<RevealCommonSvc.ComboBoxDTO> GetForemanByGeneralForeman()
        {
            List<RevealCommonSvc.ComboBoxDTO> rslt = new List<RevealCommonSvc.ComboBoxDTO>();
            rslt.AddRange(_foremanlist);
            return rslt;
        }

        public List<RevealCommonSvc.LookupDTO> GetTestType()
        {
            List<RevealCommonSvc.LookupDTO> rslt = new List<RevealCommonSvc.LookupDTO>();
            rslt.AddRange(_testtypelist);
            return rslt;
        }

        public List<DataGroup> GetGroupedDocument(string wfpTitle)
        {
            List<DataGroup> grouplist = new List<DataGroup>();

            DataGroup group;
            if (_iwpdrawing.WFP != null && _iwpdrawing.WFP.Count > 0)
            {
                group = new DataGroup("Goup1", wfpTitle, "");
                group.Items = _iwpdrawing.WFP.Select(x => new DataItem(x.DocumentID + "WFP", x.Description, x.LocationURL,
                            "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if (_iwpdrawing.drawing != null && _iwpdrawing.drawing.Count > 0)
            {
                group = new DataGroup("Group2", WinAppLibrary.Utilities.SPCollectionName.Drawing, "");
                group.Items = _iwpdrawing.drawing.Select(x => new DataItem(x.DrawingID + "Drawing", x.DrawingName,
                    Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.Drawing + "/" + x.DrawingFileURL,
                    "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if (_iwpdrawing.SafetyDoc != null && _iwpdrawing.SafetyDoc.Count > 0)
            {
                group = new DataGroup("Goup3", WinAppLibrary.Utilities.SPCollectionName.SafetyDoc, "");
                group.Items = _iwpdrawing.SafetyDoc.Select(x => new DataItem(x.DocumentID + "Safety", x.Description, x.LocationURL,
                            "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if (_iwpdrawing.RFIDoc != null && _iwpdrawing.RFIDoc.Count > 0)
            {
                group = new DataGroup("Group4", WinAppLibrary.Utilities.SPCollectionName.RFIDoc, "");
                group.Items = _iwpdrawing.RFIDoc.Select(x => new DataItem(x.DocumentID + "RFI", x.Description, x.LocationURL,
                            "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if(_iwpdrawing.QAQC != null && _iwpdrawing.QAQC.Count > 0)
            {
                group = new DataGroup("Group5", WinAppLibrary.Utilities.SPCollectionName.QAQC, "");
                group.Items = _iwpdrawing.QAQC.Select(x => new DataItem(x.DTOStatus + "QAQC", 
                    x.Description, x.LocationURL, "", group){} ).ToObservableCollection();
                grouplist.Add(group);
            }

            return grouplist;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetItrDocumentForFIWPByFIWPID(int doctypeId, int fiwpId, int projectId, int moduleId)
        {
            try
            {
                List<RevealProjectSvc.DocumentDTO> result = await (new Lib.ServiceModel.ProjectModel()).GetDocumentForFIWPByDocType(doctypeId, fiwpId, projectId, moduleId);
                _iwpDocumnet = result;

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
