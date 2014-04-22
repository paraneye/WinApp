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
    public class IWPDataSource
    {
        private DataLibrary.DocumentAndDrawing _iwpdrawing = new DataLibrary.DocumentAndDrawing();
        public DataLibrary.DocumentAndDrawing IWPDrawing { get { return _iwpdrawing; } }

        private List<DataLibrary.DocumentDTO> _iwpDocumnet = new List<DataLibrary.DocumentDTO>();

        private List<DataLibrary.ComboBoxDTO> _iwplist = new List<DataLibrary.ComboBoxDTO>();
        private List<DataLibrary.ComboBoxDTO> _siwplist = new List<DataLibrary.ComboBoxDTO>();
        private List<DataLibrary.ComboBoxDTO> _hydrolist = new List<DataLibrary.ComboBoxDTO>();
        private List<DataLibrary.FiwpDTO> _csulist = new List<DataLibrary.FiwpDTO>();
        private List<DataLibrary.FiwpDTO> _iwps = new List<DataLibrary.FiwpDTO>();
        private List<DataLibrary.FiwpDTO> _fiwps = new List<DataLibrary.FiwpDTO>();
        private List<DataLibrary.FiwpDTO> _siwps = new List<DataLibrary.FiwpDTO>();
        private List<DataLibrary.FiwpDTO> _hydros = new List<DataLibrary.FiwpDTO>();
        private List<DataLibrary.FiwpDTO> _cus = new List<DataLibrary.FiwpDTO>();
        private List<DataLibrary.ComboCodeBoxDTO> _foremanlist = new List<DataLibrary.ComboCodeBoxDTO>();
        private List<DataLibrary.LookupDTO> _testtypelist = new List<DataLibrary.LookupDTO>();
        
        public static int selectedIWP { get; set; }
        public static int selectedSIWP { get; set; }
        public static int selectedHydro { get; set; }
        public static int selectedCSU { get; set; }
        public static string selectedIWPName { get; set; }
        public static string selectedSIWPName { get; set; }
        public static string selectedHydroName { get; set; }
        public static string selectedCSUName { get; set; }

        public static string selectIwpStart { get; set; }
        public static string selectIWpEnd { get; set; }
        public static string selectEstimateManHour { get; set; }

        public static bool isWizard { get; set; }
        public static List<DataLibrary.FiwpDTO> iwplist { get; set; }
        public static List<DataLibrary.ComboBoxDTO> iwps { get; set; }
        public static List<DataLibrary.ComboBoxDTO> hydros { get; set; }   


        public async Task<bool> GetIwpDrawingOnMode(int fiwpId, int projectId, string disciplineCode, string currentMenu)
        {
            bool retValue = false;
            try
            {
                DataLibrary.DocumentAndDrawing result = new DataLibrary.DocumentAndDrawing();
                //IWP or SIWP
                if (currentMenu == Lib.MainMenuList.IWPViewer)
                    result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByFIWP(fiwpId, projectId, disciplineCode);
                //else if (currentMenu == Lib.MainMenuList.SIWPViewer)
                //    result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsBySIWP(fiwpId, projectId, disciplineCode);
                //else if (currentMenu == Lib.MainMenuList.HydroViewer)
                //    result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByHydro(fiwpId, projectId, disciplineCode);
                //else if (currentMenu == Lib.MainMenuList.CSUViewer)
                //    result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByCSU(fiwpId, projectId, disciplineCode);
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
                _csulist = result;
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

        public async Task<bool> GetFiwpByCwpSchedulePackageTypeOnMode(int cwpId, int projectscheduleId, string packagetypeLuid)
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

        public async Task<bool> GetForemanByGeneralForemanOnMode(string rolltype, string nextuserid, string nextrolltype, string projectId)
        {
            bool retValue = false;
            try
            {
                //var result = await (new Lib.ServiceModel.CommonModel()).GetForemanByGeneralForeman_Combo(sigmaUserId, sigmaRoleId, projectId);
                var result = await (new Lib.ServiceModel.CommonModel()).GetGeneralForemanByProject_Combo(rolltype, nextuserid, nextrolltype, projectId);//, _disciplineCode);
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
        
        public List<DataLibrary.ComboBoxDTO> GetFiwpByProjectSchedule()
        {
            return _iwplist;
        }

        public List<DataLibrary.ComboBoxDTO> GetSiwpByProjectSchedule()
        {
            return _siwplist;
        }

        public List<DataLibrary.ComboBoxDTO> GetHydroByProjectSchedule()
        {
            return _hydrolist;
        }

        public List<DataLibrary.FiwpDTO> GetCsuByProjectSchedule()
        {
            return _csulist;
        }


        public List<DataLibrary.FiwpDTO> GetFiwpByID()
        {
            return _iwps;
        }

        public List<DataLibrary.FiwpDTO> GetFiwpByProjectScheduleID()
        {
            return _fiwps;
        }

        public List<DataLibrary.FiwpDTO> GetSiwpByID()
        {
            return _siwps;
        }

        public List<DataLibrary.FiwpDTO> GetHydroByID()
        {
            return _hydros;
        }

        public async Task<bool> SaveFIWP(List<DataLibrary.FiwpDTO> fiwp)
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
        public async Task<bool> SaveHydro(List<DataLibrary.FiwpDTO> fiwp)
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

        public async Task<bool> SaveSIWP(List<DataLibrary.FiwpDTO> fiwp)
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

        public List<DataLibrary.ComboCodeBoxDTO> GetForemanByGeneralForeman()
        {
            List<DataLibrary.ComboCodeBoxDTO> rslt = new List<DataLibrary.ComboCodeBoxDTO>();
            if(_foremanlist != null && _foremanlist.Count >0)
                rslt.AddRange(_foremanlist);
            return rslt;
        }

        public List<DataLibrary.LookupDTO> GetTestType()
        {
            List<DataLibrary.LookupDTO> rslt = new List<DataLibrary.LookupDTO>();
            rslt.AddRange(_testtypelist);
            return rslt;
        }

        public List<DataGroup> GetGroupedDocument(string wfpTitle)
        {
            List<DataGroup> grouplist = new List<DataGroup>();
            
            DataGroup group;
            if (_iwpdrawing.documents != null && _iwpdrawing.documents.Count > 0)
            {
                group = new DataGroup("Goup1", wfpTitle, "");
                group.Items = _iwpdrawing.documents.Where(x => x.FileCategory == DataLibrary.Utilities.FileCategory.REPORT && x.LocationURL.ToString().LastIndexOf(".jpg") >-1).OrderBy(x=> Convert.ToInt32(x.SortOrder))
                    .Select(x => new DataItem(x.DocumentID + "WFP",  x.Description, x.LocationURL.Replace("\\\\", "/").Replace("\\", "/"), "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if (_iwpdrawing.drawings != null && _iwpdrawing.drawings.Count > 0)
            {
                 group = new DataGroup("Group2", DataLibrary.Utilities.CollectionName.DRAWING, "");
                 group.Items = _iwpdrawing.drawings.Where(x=> x.DrawingFileURL.ToString().LastIndexOf(".jpg")> -1).Select(x => new DataItem(x.DrawingID + "Drawing", x.DrawingName, x.DrawingFileURL.Replace("\\\\", "/").Replace("\\", "/"), "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if (_iwpdrawing.documents != null && _iwpdrawing.documents.Count > 0)
            {
                group = new DataGroup("Goup3", DataLibrary.Utilities.CollectionName.SAFETY, "");
                group.Items = _iwpdrawing.documents.Where(x => x.FileCategory == DataLibrary.Utilities.FileCategory.DOCUMENT && x.DocumentTypeLUID == DataLibrary.Utilities.FileType.SAFETY_FORM && x.LocationURL.ToString().LastIndexOf(".jpg") > -1)
                    .Select(x => new DataItem(x.DocumentID + "Safety", x.Description, x.LocationURL.Replace("\\\\", "/").Replace("\\", "/"), "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if (_iwpdrawing.documents != null && _iwpdrawing.documents.Count > 0)
            {
                group = new DataGroup("Goup4", DataLibrary.Utilities.CollectionName.ITR, "");
                group.Items = _iwpdrawing.documents.Where(x => x.FileCategory == DataLibrary.Utilities.FileCategory.DOCUMENT && x.DocumentTypeLUID == DataLibrary.Utilities.FileType.ITR && x.LocationURL.ToString().LastIndexOf(".jpg") > -1)
                    .Select(x => new DataItem(x.DocumentID + "ITR", x.Description, x.LocationURL.Replace("\\\\", "/").Replace("\\", "/"), "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if (_iwpdrawing.documents != null && _iwpdrawing.documents.Count > 0)
            {
                group = new DataGroup("Goup5", DataLibrary.Utilities.CollectionName.SPEC, "");
                group.Items = _iwpdrawing.documents.Where(x => x.FileCategory == DataLibrary.Utilities.FileCategory.DOCUMENT && x.DocumentTypeLUID == DataLibrary.Utilities.FileType.SPEC )
                    .Select(x => new DataItem(x.DocumentID + "Spec", x.Description, x.LocationURL.Replace("\\\\", "/").Replace("\\", "/"), "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            if (_iwpdrawing.documents != null && _iwpdrawing.documents.Count > 0)
            {
                group = new DataGroup("Goup6", DataLibrary.Utilities.CollectionName.MOC, "");
                group.Items = _iwpdrawing.documents.Where(x => x.FileCategory == DataLibrary.Utilities.FileCategory.DOCUMENT && x.DocumentTypeLUID == DataLibrary.Utilities.FileType.MOC)
                    .Select(x => new DataItem(x.DocumentID + "MOC", x.Description, x.LocationURL.Replace("\\\\", "/").Replace("\\", "/"), "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            return grouplist;
        }

        public async Task<List<DataLibrary.DocumentDTO>> GetItrDocumentForFIWPByFIWPID(string doctypeId, int fiwpId, int projectId, string disciplineCode)
        {
            try
            {
                List<DataLibrary.DocumentDTO> result = await (new Lib.ServiceModel.ProjectModel()).GetDocumentForFIWPByDocType(doctypeId, fiwpId, projectId, disciplineCode);
                _iwpDocumnet = result;

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<DataLibrary.FiwpDTO> UpdateIwpPeriod(DataLibrary.FiwpDTO iwp, string totalManhours)
        {
            try
            {
                DataLibrary.FiwpDTO result = await (new Lib.ServiceModel.ProjectModel()).UpdateIwpPeriod(iwp, totalManhours);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }    
    }
}
