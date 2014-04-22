using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.Extensions;

namespace Element.Reveal.Crew.Lib.DataSource
{
    class DrawingDataSource
    {
        private const int ItemCount = 20;
        private RevealProjectSvc.DrawingPageTotal _drawingpage_off = null;
        private RevealProjectSvc.DrawingPageTotal _drawingpage = new RevealProjectSvc.DrawingPageTotal();
        public RevealProjectSvc.DrawingPageTotal DrawingPage { get { return _drawingpage; } }
        Dictionary<string, ObservableCollection<RevealCommonSvc.ComboBoxDTO>> _grouplist = new Dictionary<string, ObservableCollection<RevealCommonSvc.ComboBoxDTO>>();
        public Dictionary<string, ObservableCollection<RevealCommonSvc.ComboBoxDTO>> GroupList { get { return _grouplist; } }
        ObservableCollection<RevealProjectSvc.DocumentnoteDTO> _documentnote = new ObservableCollection<RevealProjectSvc.DocumentnoteDTO>();
        public ObservableCollection<RevealProjectSvc.DocumentnoteDTO> DocumentNote { get { return _documentnote; } }

        public string DrawingTitle
        {
            get
            {
                return WinAppLibrary.Utilities.Helper.GetValueFromStorage(Lib.HashKey.Key_Title);
            }
        }
        public string EngineerTag
        {
            get
            {
                return WinAppLibrary.Utilities.Helper.GetValueFromStorage(Lib.HashKey.Key_EngTag);
            }
        }
        public string SortOption
        {
            get
            {
                return WinAppLibrary.Utilities.Helper.GetValueFromStorage(Lib.HashKey.Key_Sort);
            }
        }

        public async Task<bool> LoadOptionOnMode(int projectId, int moduleId)
        {
            try
            {
                _grouplist.Clear();
                var common = new ServiceModel.CommonModel();
                var project = new ServiceModel.ProjectModel();

                _grouplist.Add(Lib.HashKey.Key_Reset, new ObservableCollection<RevealCommonSvc.ComboBoxDTO>());
                var result = await common.GetCWPByProjectID_Combo(projectId, moduleId);
                _grouplist.Add(Lib.HashKey.Key_CWP, result.ToObservableCollection());
                result = await common.GetFIWPByProject_Combo(projectId, moduleId);
                _grouplist.Add(Lib.HashKey.Key_FIWP, result.ToObservableCollection());
                result = await common.GetDrawingType_Combo();
                _grouplist.Add(Lib.HashKey.Key_DrawingType, result.ToObservableCollection());

                var notes = await project.GetDocumentNoteByFiwpDocumentDrawing(50, projectId, moduleId);
                _documentnote.Clear();
                _documentnote = notes.ToObservableCollection();
                notes = null;

                return true;

                //DrawingGrouping.BindHeader<RevealCommonSvc.ComboBoxDTO>(categories);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadOptionOnMode");
                throw e;
            }

        }

        public async Task<bool> LoadOptionOffMode()
        {
            try
            {
                var options = await GetGrouping();
                _grouplist.Clear();

                _grouplist.Add(Lib.HashKey.Key_Reset, new ObservableCollection<RevealCommonSvc.ComboBoxDTO>());
                _grouplist.Add(Lib.HashKey.Key_CWP, options[Lib.HashKey.Key_CWP].ToObservableCollection());
                _grouplist.Add(Lib.HashKey.Key_FIWP, options[Lib.HashKey.Key_FIWP].ToObservableCollection());
                _grouplist.Add(Lib.HashKey.Key_DrawingType, options[Lib.HashKey.Key_DrawingType].ToObservableCollection());
                options = null;

                _documentnote = await GetDocumentNote();
                return true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadOptionOffMode");
                throw e;
            }
        }

        private List<RevealCommonSvc.ComboBoxDTO> GetGroupingCategories()
        {
            List<RevealCommonSvc.ComboBoxDTO> retValue = new List<RevealCommonSvc.ComboBoxDTO>();
            retValue.Add(new RevealCommonSvc.ComboBoxDTO() { DataID = 0, DataName = "Reset All", ExtraValue1 = @"&#xE10F;" });
            retValue.Add(new RevealCommonSvc.ComboBoxDTO() { DataID = 1, DataName = "CWP", ExtraValue2 = Lib.HashKey.Key_CWP });
            retValue.Add(new RevealCommonSvc.ComboBoxDTO() { DataID = 2, DataName = "IWP", ExtraValue2 = Lib.HashKey.Key_FIWP });
            retValue.Add(new RevealCommonSvc.ComboBoxDTO() { DataID = 3, DataName = "Drawing Type", ExtraValue2 = Lib.HashKey.Key_DrawingType });

            return retValue;
        }

        public void ClearSelection()
        {
            foreach (var group in _grouplist)
            {
                var selecteds = group.Value.Where(x => x.ParentID > 0);

                foreach (var item in selecteds)
                    item.ParentID = 0;
            }
        }

        public async Task<bool> GetDrawingOnMode(int projectId, List<int> cwpIds, List<int> fiwpIds, List<int> drawingtypes, string enTag, string title, string sortoption, int curpage)
        {
            bool retValue = false;

            _drawingpage = await (new Lib.ServiceModel.ProjectModel()).GetDrawingForDrawingViewer(projectId, cwpIds, fiwpIds, drawingtypes, enTag, title, sortoption, curpage);

            if (_drawingpage != null)
                retValue = true;

            return retValue;
        }

        public async Task<bool> GetDrawingOffMode(int curpage)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                if (_drawingpage_off == null)
                {
                    var stream = await helper.GetFileStream(ContentPath.OffModeFolder, ContentPath.DrawingSource);
                    _drawingpage_off = await helper.EncryptDeserializeFrom<RevealProjectSvc.DrawingPageTotal>(stream);
                }

                if (_drawingpage_off != null && _drawingpage_off.drawing != null)
                {
                    int from = (int)Math.Min(Math.Floor(_drawingpage_off.drawing.Count / (double)ItemCount), curpage - 1);
                    int count = (int)Math.Min(_drawingpage_off.drawing.Count - from * ItemCount, ItemCount);

                    _drawingpage.drawing = _drawingpage_off.drawing.GetRange(from * ItemCount, count);
                    _drawingpage.CurrentPage = from + 1;
                    _drawingpage.TotalPageCount = (int)Math.Ceiling(_drawingpage_off.drawing.Count / (double)ItemCount);
                    retValue = true;
                }
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "GetDrawingOffMode");
                throw e;
            }

            return retValue;
        }

        private async Task<Dictionary<string, List<RevealCommonSvc.ComboBoxDTO>>> GetGrouping()
        {
            Dictionary<string, List<RevealCommonSvc.ComboBoxDTO>> retValue = new Dictionary<string, List<RevealCommonSvc.ComboBoxDTO>>();
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                //Get CWP
                var stream = await helper.GetFileStream(ContentPath.OffModeFolder, ContentPath.GroupingCWP);
                var list = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealCommonSvc.ComboBoxDTO>>(stream);
                retValue.Add(HashKey.Key_CWP, list);
                //Get FIWP
                stream = await helper.GetFileStream(ContentPath.OffModeFolder, ContentPath.GroupingFIWP);
                list = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealCommonSvc.ComboBoxDTO>>(stream);
                retValue.Add(HashKey.Key_FIWP, list);
                //Get Drawing Type
                stream = await helper.GetFileStream(ContentPath.OffModeFolder, ContentPath.GroupingDrawingType);
                list = await helper.EncryptDeserializeFrom<List<RevealCommonSvc.ComboBoxDTO>>(stream);
                retValue.Add(HashKey.Key_DrawingType, list);
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "GetGrouping");
                throw e;
            }

            return retValue;
        }

        private async Task<ObservableCollection<RevealProjectSvc.DocumentnoteDTO>> GetDocumentNote()
        {
            ObservableCollection<RevealProjectSvc.DocumentnoteDTO> retValue = new ObservableCollection<RevealProjectSvc.DocumentnoteDTO>();
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                //Get Sticky Note
                var stream = await helper.GetFileStream(ContentPath.OffModeFolder, ContentPath.DocumentNote);
                retValue = await helper.EncryptDeserializeFrom<ObservableCollection<RevealProjectSvc.DocumentnoteDTO>>(stream);
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "GetStickyNote");
                throw e;
            }

            return retValue;
        }

        public async Task<bool> SaveDrawing(int projectId, int moduleId, string enTag, string title, string sortoption)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                Stream xmlstream;
                var drawingpagetotal = await (new Lib.ServiceModel.ProjectModel()).GetDrawingForDrawingViewer(projectId,
                                _grouplist[Lib.HashKey.Key_CWP].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _grouplist[Lib.HashKey.Key_FIWP].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _grouplist[Lib.HashKey.Key_DrawingType].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                enTag, title, sortoption, 1);

                if (drawingpagetotal != null && drawingpagetotal.drawing != null && drawingpagetotal.drawing.Count > 0)
                {
                    foreach (var d in drawingpagetotal.drawing)
                    {
                        var stream = await helper.GetImageStreamFromUri(new Uri(d.DrawingFilePath + d.DrawingFileURL));
                        if (stream == null)
                            stream = await helper.GetImageStreamFromUri(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + ContentPath.DefaultDrawing));

                        await helper.SaveFileStream(ContentPath.OffModeFolder, d.DrawingFileURL, stream);
                        d.DrawingFilePath = ContentPath.OffModeFolder.Path + "\\";
                    }
                }

                xmlstream = helper.EncryptSerializeTo<RevealProjectSvc.DrawingPageTotal>(drawingpagetotal);
                await helper.SaveFileStream(ContentPath.OffModeFolder, ContentPath.DrawingSource, xmlstream);

                await SaveDrawingOption(_grouplist, _documentnote, enTag, title, sortoption);
                retValue = true;
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "SaveDrawing");
                throw e;
            }

            return retValue;
        }

        public async Task<bool> SaveDrawingOption(Dictionary<string, ObservableCollection<RevealCommonSvc.ComboBoxDTO>> options,
            ObservableCollection<RevealProjectSvc.DocumentnoteDTO> notes,
            string engtag, string title, string sortoption)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                Stream xmlstream = helper.EncryptSerializeTo<List<RevealCommonSvc.ComboBoxDTO>>(options[HashKey.Key_CWP].ToList());
                await helper.SaveFileStream(ContentPath.OffModeFolder, ContentPath.GroupingCWP, xmlstream);

                xmlstream = helper.EncryptSerializeTo<List<RevealCommonSvc.ComboBoxDTO>>(options[HashKey.Key_FIWP].ToList());
                await helper.SaveFileStream(ContentPath.OffModeFolder, ContentPath.GroupingFIWP, xmlstream);

                xmlstream = helper.EncryptSerializeTo<List<RevealCommonSvc.ComboBoxDTO>>(options[HashKey.Key_DrawingType].ToList());
                await helper.SaveFileStream(ContentPath.OffModeFolder, ContentPath.GroupingDrawingType, xmlstream);

                xmlstream = helper.EncryptSerializeTo<ObservableCollection<RevealProjectSvc.DocumentnoteDTO>>(notes);
                await helper.SaveFileStream(ContentPath.OffModeFolder, ContentPath.DocumentNote, xmlstream);

                WinAppLibrary.Utilities.Helper.SetValueInStorage(HashKey.Key_EngTag, engtag);
                WinAppLibrary.Utilities.Helper.SetValueInStorage(HashKey.Key_Title, title);
                WinAppLibrary.Utilities.Helper.SetValueInStorage(HashKey.Key_Sort, sortoption);

                //This was banned for temporary until finding alternative as Windows Apps doesn't support serialize Dictionary with List for value.
                //Stream xmlstream = helper.EncryptHashSerializeTo<Dictionary<string, List<WinAppLibrary.RevealCommonSvc.ComboBoxDTO>>>(options);
                //await helper.SaveFileStream(ContentPath.OffModeFolder, ContentPath.GroupingSource, xmlstream);

                retValue = true;
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "SaveDrawingOption");
                throw e;
            }

            return retValue;
        }
    }
}
