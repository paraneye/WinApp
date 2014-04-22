using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Crew.Lib.DataSource
{
    class IWPDataSource
    {
        private RevealProjectSvc.FiwpDocument _iwpdrawing = new RevealProjectSvc.FiwpDocument();
        public RevealProjectSvc.FiwpDocument IWPDrawing { get { return _iwpdrawing; } }

        private List<RevealProjectSvc.DocumentDTO> _iwpDocumnet = new List<RevealProjectSvc.DocumentDTO>();

        public async Task<bool> GetIwpDrawingOnMode(int fiwpId, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByFIWP(fiwpId, projectId, moduleId);
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

        public List<DataGroup> GetGroupedDocument()
        {
            List<DataGroup> grouplist = new List<DataGroup>();

            DataGroup group;
            if (_iwpdrawing.WFP != null && _iwpdrawing.WFP.Count > 0)
            {
                group = new DataGroup("Goup1", "Installed Work Package", "");
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

            if (_iwpdrawing.QAQC != null && _iwpdrawing.QAQC.Count > 0)
            {
                group = new DataGroup("Group5", WinAppLibrary.Utilities.SPCollectionName.QAQC, "");
                group.Items = _iwpdrawing.QAQC.Select(x => new DataItem(x.DTOStatus + "QAQC",
                    x.Description, x.LocationURL, "", group) { }).ToObservableCollection();
                grouplist.Add(group);
            }

            return grouplist;
        }

        public async Task<List<RevealProjectSvc.DocumentDTO>> GetItrDocumentForFIWPByFIWPID(int doctypeId, int fiwpId, int projectId, int moduleId)
        {
            try
            {
                List<RevealProjectSvc.DocumentDTO> result = await (new Lib.ServiceModel.ProjectModel()).GetItrDocumentForFIWPByFIWPID(doctypeId, fiwpId, projectId, moduleId);
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
