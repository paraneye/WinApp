using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Extensions;

namespace Element.Reveal.Crew.Lib.DataSource
{
    class ComponentCrewDataSource
    {
        static ObservableCollection<RevealProjectSvc.MTODTO> _components = new ObservableCollection<RevealProjectSvc.MTODTO>();
        static public ObservableCollection<RevealProjectSvc.MTODTO> Component { get { return _components; } }
        static ObservableCollection<RevealCommonSvc.ComboBoxDTO> _crews = new ObservableCollection<RevealCommonSvc.ComboBoxDTO>();
        static public ObservableCollection<RevealCommonSvc.ComboBoxDTO> ForemanCrew { get { return _crews; } }

        ServiceModel.ProjectModel _projectmodel = new ServiceModel.ProjectModel();
        ServiceModel.CommonModel _commonmodel = new ServiceModel.CommonModel();

        public void SetComponent(List<RevealProjectSvc.MTODTO> source)
        {
            _components.Clear();

            foreach (var item in source)
                _components.Add(item);
        }

        public async Task<bool> LoadComponent(int cwpId, int fiwpId, int materialcategoryId, int ruleofcreditweightId, int drawingId, DateTime workDate, int projectId, int moduleId)
        {
            bool retValue = false;

            try
            {
                _components.Clear();

                var result = await _projectmodel.GetComponentProgressByFIWPUncompleted(cwpId, fiwpId, materialcategoryId, ruleofcreditweightId, workDate, projectId, moduleId);

                if (result != null)
                {
                    foreach (var child in result.mto)
                        _components.Add(child);

                    retValue = true;
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgressDataSource GetComponent");
            }

            return retValue;
        }

        public async Task<bool> LoadCrewAndForemanByFiwpWorkDate_Combo(int cwpId, int fiwpId, int projectId, int moduleId, DateTime workDate)
        {
            bool retValue = false;

            if (fiwpId > 0 && projectId > 0 && workDate > WinAppLibrary.Utilities.Helper.DateTimeMinValue)
            {
                _crews.Clear();

                try
                {
                    var result = await _commonmodel.GetCrewAndForemanByFiwpWorkDate_Combo(cwpId, fiwpId, projectId, moduleId, workDate);

                    if (result != null && result.Count > 0)
                    {
                        int id = result.Min(x => x.DataID);
                        var list = result.OrderBy(x => x.DataID).Select(x => new RevealCommonSvc.ComboBoxDTO
                        {
                            DataID = x.DataID,
                            DataName = x.DataName,
                            ExtraValue1 = x.ExtraValue1,
                            ExtraValue2 = !string.IsNullOrEmpty(x.ExtraValue2) ? x.ExtraValue2.Split('/')[1] : "",
                            ExtraValue3 = x.DataID == id ? "Foreman" : "Crew"
                        }).ToList();

                        foreach (var child in list)
                            _crews.Add(child);

                        retValue = true;
                    }
                    else
                        throw new NullReferenceException("There is no crew for the options. Please change date or iwp.");
                }
                catch (NullReferenceException ne)
                {
                    throw ne;
                }
                catch (Exception e)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgressDataSource GetCrewAndForemanByFiwpWorkDate_Combo");
                    throw new Exception("There was an error to retrieve data from server. Please contact administrator");
                }
            }

            return retValue;
        }

        public RevealCommonSvc.ComboBoxDTO FindCrewByPersonnelID(int personnelID)
        {
            RevealCommonSvc.ComboBoxDTO retValue = new RevealCommonSvc.ComboBoxDTO();

            try
            {
                var crew = _crews.Where(x => Convert.ToInt32(x.ExtraValue2) == personnelID).SingleOrDefault();
                if (crew != null)
                    retValue = crew;
            }
            catch { }

            return retValue;
        }

        public void RemoveSource(List<RevealProjectSvc.MTODTO> components)
        {
            foreach(var com in components)
            {
                var item = _components.Where(x => x.ProgressID == com.ProgressID).SingleOrDefault();

                if(item != null)
                    _components.Remove(item);
            }
        }

        public void ClearComponent()
        {
            _components.Clear();
        }

        public void ClearCrew()
        {
            _crews.Clear();
        }

        public void InitiateSource()
        {
            _components.Clear();
            _crews.Clear();

            (new Lib.DataSource.InputProgressDataSource()).InitiateSource();
        }
    }
}
