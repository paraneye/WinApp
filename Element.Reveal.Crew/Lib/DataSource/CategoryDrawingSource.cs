using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;

namespace Element.Reveal.Crew.Lib.DataSource
{
    public class CategoryDrawingSource
    {
        #region "Properties"
        static ObservableCollection<RevealProjectSvc.CollectionDTO> _categories = new ObservableCollection<RevealProjectSvc.CollectionDTO>();
        public static ObservableCollection<RevealProjectSvc.CollectionDTO> Categories { get { return _categories; } }

        static ObservableCollection<RevealProjectSvc.MTODTO> _mtos = new ObservableCollection<RevealProjectSvc.MTODTO>();
        static ObservableCollection<DataItem> _drawings = new ObservableCollection<DataItem>();
        public static ObservableCollection<DataItem> Drawings { get { return _drawings; } }
        static ObservableCollection<RevealCommonSvc.ComboBoxDTO> _fiwps = new ObservableCollection<RevealCommonSvc.ComboBoxDTO>();
        public static ObservableCollection<RevealCommonSvc.ComboBoxDTO> IWPs { get { return _fiwps; } }
        static ObservableCollection<RevealCommonSvc.ComboBoxDTO> _materials = new ObservableCollection<RevealCommonSvc.ComboBoxDTO>();
        public static ObservableCollection<RevealCommonSvc.ComboBoxDTO> Materials { get { return _materials; } }
        static ObservableCollection<RevealCommonSvc.ComboBoxDTO> _rulecredit = new ObservableCollection<RevealCommonSvc.ComboBoxDTO>();
        public static ObservableCollection<RevealCommonSvc.ComboBoxDTO> RuleCredits { get { return _rulecredit; } }
        
        ServiceModel.ProjectModel _projectmodel = new ServiceModel.ProjectModel();
        ServiceModel.CommonModel _commonmodel = new ServiceModel.CommonModel();

        static RevealCommonSvc.ComboBoxDTO _selectedIwp, _selectedMaterial, _selectedRulecredit;
        static DataItem _selectedDrawing;

        public static int SelectedCWPID()
        {
            if (_selectedIwp == null)
                return 0;
            else
            {
                var collecton = _categories.Where(x => x.FIWPID == _selectedIwp.DataID).FirstOrDefault();
                if (collecton != null)
                    return collecton.CWPID;
                else
                    return 0;
            }
        }

        public static RevealCommonSvc.ComboBoxDTO SelectedIWP 
        { 
            get 
            {
                if (_selectedIwp == null)
                    return new RevealCommonSvc.ComboBoxDTO();
                else
                    return _selectedIwp; 
            } 
        }
        public static RevealCommonSvc.ComboBoxDTO SelectedMaterial
        {
            get
            {
                if (_selectedMaterial == null)
                    return new RevealCommonSvc.ComboBoxDTO();
                else
                    return _selectedMaterial;
            }
        }
        public static RevealCommonSvc.ComboBoxDTO SelectedRuleOfCredit
        {
            get
            {
                if (_selectedRulecredit == null)
                    return new RevealCommonSvc.ComboBoxDTO();
                else
                    return _selectedRulecredit;
            }
        }
        public static DataItem SelectedDrawing
        {
            get
            {
                if (_selectedDrawing == null)
                    return new DataItem("0", "", "", "", null);
                else
                    return _selectedDrawing;
            }
        }

        public int CountOfMaterials { get { return _materials.Count; } }
        public int CountOfRuleOfCredit { get { return _rulecredit.Count; } }
        public int CountOfDrawing { get { return _drawings.Count; } }
        #endregion

        #region "Public Method"

        #region "Load"
        public async Task<bool> LoadCategories(int personnelId, int projectId, int moduleId)
        {
            bool retValue = false;

            try
            {
                var result = await _projectmodel.GetAvailableCollectionForForemanUncompleted(personnelId, projectId, moduleId);
                _categories = result.ToObservableCollection();
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "CategoryDrawingSource LoadCategories");
            }

            return retValue;
        }

        public void LoadFiwps()
        {
            try
            {
                _fiwps.Clear();

                var result = _categories.Select(x => new  
                                        {
                                            DataID = x.FIWPID,
                                            DataName = x.FIWPName,
                                            ExtraValue1 = x.CWPName
                                        }).Distinct().Select(x => new RevealCommonSvc.ComboBoxDTO()
                                        {
                                            DataID = x.DataID,
                                            DataName = x.DataName,
                                            ExtraValue1 = x.ExtraValue1
                                        }).ToList();

                foreach (var item in result)
                    _fiwps.Add(item);
                   
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "CategoryDrawingSource LoadFiwps");
            }
        }

        public void LoadMaterials()
        {
            try
            {
                _materials.Clear();
                var result = _categories.Where(x => x.FIWPID == SelectedIWP.DataID).Select(x => new  
                                        {
                                            DataID = x.MaterialCategoryID,
                                            DataName = x.MaterialCategory
                                        }).Distinct().Select(x => new RevealCommonSvc.ComboBoxDTO() 
                                        {
                                            DataID = x.DataID,
                                            DataName = x.DataName
                                        }).ToList();

                foreach (var item in result)
                    _materials.Add(item);

            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "CategoryDrawingSource LoadMaterials");
            }
        }

        public void LoadRuleOfCredits()
        {
            try
            {
                _rulecredit.Clear();

                var result = _categories.Where(x => x.FIWPID == SelectedIWP.DataID && x.MaterialCategoryID == SelectedMaterial.DataID).Select(x => new 
                   { 
                       DataID = x.RuleOfCreditWeightID,
                       DataName = x.RuleOfCredit
                   }).Distinct().Select(x => new RevealCommonSvc.ComboBoxDTO()
                   {
                       DataID = x.DataID,
                       DataName = x.DataName
                   }).ToList();

                foreach (var item in result)
                    _rulecredit.Add(item);

            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "CategoryDrawingSource LoadMaterials");
            }
        }

        public async Task<bool> LoadDrawings(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, DateTime workDate, int projectId, int moduleId)
        {
            bool retValue = false;

            try
            {
                _drawings.Clear();
                _mtos.Clear();

                var result = await _projectmodel.GetComponentProgressByFIWPUncompleted(cwpId, fiwpId, materialcategoryId, ruleofcreditweightId, workDate, projectId, moduleId);
                var drawing = result.drawing.Select(x => new DataItem(x.DataID.ToString(), x.DataName, x.ExtraValue1 + x.ExtraValue2, "", null) { });

                foreach (var item in drawing)
                    _drawings.Add(item);

                foreach (var item in result.mto)
                    _mtos.Add(item);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgressDataSource GetComponent");
            }

            return retValue;
        }

        public RevealCommonSvc.ComboBoxDTO GetMaterialByIndex(int index)
        {
            RevealCommonSvc.ComboBoxDTO retValue = null;

            try
            {
                if (_materials.Count > index)
                    retValue = _materials[index];
            }
            catch { }

            return retValue;
        }

        public RevealCommonSvc.ComboBoxDTO GetRuleOfCreditByIndex(int index)
        {
            RevealCommonSvc.ComboBoxDTO retValue = null;

            try
            {
                if (_rulecredit.Count > index)
                    retValue = _rulecredit[index];
            }
            catch { }

            return retValue;
        }

        public DataItem GetDrawingByIndex(int index)
        {
            DataItem retValue = null;

            try
            {
                if (_materials.Count > index)
                    retValue = _drawings[index];
            }
            catch { }

            return retValue;
        }
        #endregion

        #region "Get"
        public List<RevealProjectSvc.MTODTO> GetMTOByDrawingID(int drawingId)
        {
            return _mtos.Where(x => x.DrawingID == drawingId).ToList();
        }
        #endregion

        #region "Selection"
        public void SelectIWP(object select)
        {
            try
            {
                _selectedIwp = null;
                if (select != null)
                    _selectedIwp = select as RevealCommonSvc.ComboBoxDTO;
                
            }
            catch { }
        }

        public void SelectMaterial(object select)
        {
            try
            {
                _selectedMaterial = null;
                if (select != null)
                    _selectedMaterial = select as RevealCommonSvc.ComboBoxDTO;

            }
            catch { }
        }

        public void SelectRuleOfCredit(object select)
        {
            try
            {
                _selectedRulecredit = null;
                if (select != null)
                    _selectedRulecredit = select as RevealCommonSvc.ComboBoxDTO;

            }
            catch { }
        }

        public void SelectDrawing(object select)
        {
            try
            {
                _selectedDrawing = null;
                if (select != null)
                    _selectedDrawing = select as DataItem;

            }
            catch { }
        }

        public int SelectedCWPID(int fiwpId)
        {
            if (_selectedIwp == null)
                return 0;
            else
            {
                var collecton = _categories.Where(x => x.FIWPID == fiwpId).FirstOrDefault();
                if (collecton != null)
                    return collecton.CWPID;
                else
                    return 0;
            }
        }
        #endregion

        #region "Clear"
        public void ClearIwps()
        {
            _fiwps.Clear();
            _selectedIwp = null;
        }

        public void ClearMaterials()
        {
            _materials.Clear();
            _selectedMaterial = null;
        }

        public void ClearRuleOfCredit()
        {
            _rulecredit.Clear();
            _selectedRulecredit = null;
        }

        public void ClearDrawing()
        {
            _drawings.Clear();
            _selectedDrawing = null;
        }

        public void InitiateSource()
        {
            _fiwps.Clear();
            _materials.Clear();
            _rulecredit.Clear();
            _drawings.Clear();
            
            _selectedMaterial = null;
            _selectedRulecredit = null;
            _selectedDrawing = null;
        }
        #endregion

        #endregion
    }
}
