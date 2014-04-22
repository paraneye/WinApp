using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Utilities;
using WinAppLibrary.Converters;

namespace Element.Reveal.TrueVue.Discipline.Viewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DrawingViewer : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid;
        Lib.DrawingDataSource _drawingsource = new Lib.DrawingDataSource();
        Lib.UI.DrawingGrouping _drawinggrouping = new Lib.UI.DrawingGrouping();
        Lib.UI.DrawingInfo _drawinginfo = new Lib.UI.DrawingInfo();
        Lib.UI.DrawingSorting _drawingsort = new Lib.UI.DrawingSorting();
        WinAppLibrary.UI.StickyNote _stickynote = new WinAppLibrary.UI.StickyNote();

        public DrawingViewer()
        {
            this.InitializeComponent();
            Loaded += DrawingViewer_Loaded;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            LoadOption();
            Login.MasterPage.ShowTopBanner = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Login.MasterPage.AppbarClicked -= MasterPage_AppbarClicked;
            base.OnNavigatedFrom(e);
        }

        #region "Private Method"
        private void LoadOption()
        {
            Login.MasterPage.Loading(true, this);
            Lib.ProjectModuleSource projectmodule = new Lib.ProjectModuleSource();

            _drawinggrouping.HeaderClicked += Grouping_HeaderClicked;
            _drawinggrouping.ItemSelectionChanged += Grouping_ItemSelectionChanged;
            _drawinggrouping.SearchClicked += Grouping_SearchClicked;
            _drawingsort.SortClicked += Grouping_SearchClicked;
            _drawinginfo.InfoViewClicked += _drawinginfo_InfoViewClicked;
            _drawinginfo.ActiveOpacity = 1;
            _drawingsort.ActiveOpacity = 1;

            this.StretchingPanel.AddPanel(_drawinggrouping);
            this.StretchingPanel.AddPanel(_drawinginfo);
            this.StretchingPanel.AddPanel(_stickynote);
            this.StretchingPanel.AddPanel(_drawingsort);            
            this.DrawingSlider.SlideChanged += Slider_ValueChanged;
            DrawingEditor.EnableEdit(Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode ? true : false);

            _projectid = projectmodule.GetProjectID();
            _moduleid = projectmodule.GetModuleID();

            switch (Login.LoginMode)
            {
                case WinAppLibrary.UI.LogMode.OnMode:
                    LoadOptionOnMode();
                    break;
                case WinAppLibrary.UI.LogMode.OffMode:
                    if (WinAppLibrary.Utilities.Helper.DownloadedData)
                        LoadOptionOffMode();
                    else
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Please download data from server first.", "Warning!");
                        Login.MasterPage.Loading(false, this);
                    }
                    break;
            }

            StretchingPanel.Stretch(true);
        }

        private async void LoadOptionOnMode()
        {
            try
            {
                await _drawingsource.LoadOptionOnMode(_projectid, _moduleid);
                _drawingsort.BindInfo(GetSortingCategories());
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadGroup", "There was an error load drawing. Please contact administrator", "Error!");
            }

            Login.MasterPage.SetBottomAppbar(WinAppLibrary.UI.MasterPage.BottomAppBarButton.Download, Visibility.Visible);
            Login.MasterPage.AppbarClicked += MasterPage_AppbarClicked;
            Login.MasterPage.Loading(false, this);
        }

        private async void LoadOptionOffMode()
        {
            try
            {
                await _drawingsource.LoadOptionOffMode();

                _drawinggrouping.SetGroupingText(_drawingsource.DrawingTitle, _drawingsource.EngineerTag);

                var collections = GetSortingCategories();               
                _drawingsort.BindInfo<DataItem>(collections, collections[0].Items.Where(x => x.Title == _drawingsource.SortOption).FirstOrDefault());

                _drawinggrouping.SetUserEnable(false);
                _drawingsort.SetUserEnable(false);

                //DrawingGrouping.BindHeader<RevealCommonSvc.ComboBoxDTO>(categories);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadOptionOffMode", "There was an error load drawing. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private ObservableCollection<DataGroup> GetSortingCategories()
        {
            ObservableCollection<DataGroup> retValue = new ObservableCollection<DataGroup>();

            var group = new DataGroup("", "Drawing Sorting", "");
            retValue.Add(group);
            retValue[0].Items.Add(new DataItem("DrawingName", "DrawingName", "", "", group));
            retValue[0].Items.Add(new DataItem("CWPID", "CWP", "", "", group));
            retValue[0].Items.Add(new DataItem("FIWPID", "IWP", "", "", group));
            retValue[0].Items.Add(new DataItem("TypeDesc", "Drawing Type", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingSubject", "Drawing Title", "", "", group));
            retValue[0].Items.Add(new DataItem("EngTagNumber", "Engineer Tag", "", "", group));

            return retValue;
        }

        private async void Loaddrawing()
        {
            bool result = false;
            Login.MasterPage.Loading(true, this);
            StretchingPanel.Stretch(false);
            this.czDrawingViewer.ItemsSource = null;
            GC.Collect();

            if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
            {
                result = await _drawingsource.GetDrawingOnMode(_projectid, 
                                _drawingsource.GroupList[Lib.HashKey.Key_CWP].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawingsource.GroupList[Lib.HashKey.Key_FIWP].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawingsource.GroupList[Lib.HashKey.Key_DrawingType].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawinggrouping.EngineerTag, _drawinggrouping.DrawingTitle,
                                _drawingsort.SelectedItem == null ? "" : (_drawingsort.SelectedItem as DataItem).UniqueId,
                                (int)DrawingSlider.Value);
            }
            else
            {
                result = await _drawingsource.GetDrawingOffMode((int)DrawingSlider.Value);
            }

            if (result)
            {
                //IEnumerable<IGrouping<IComparable, RevealProjectSvc.DrawingDTO>> groups = result.drawing.GroupBy(x => x.CWPName);
                this.czDrawingViewer.ItemsSource = _drawingsource.DrawingPage.drawing;
            }
            else
                Helper.SimpleMessage("There were no drawing.", "Alert!");

            this.DrawingSlider.Value = _drawingsource.DrawingPage.CurrentPage;
            this.DrawingSlider.Maxnumber = _drawingsource.DrawingPage.TotalPageCount;
            this.DrawingSlider.SwingShow(result);

            Login.MasterPage.Loading(false, this);
        }

        private async System.Threading.Tasks.Task<bool> LoadMarkupDrawing(int drawingId, List<RevealProjectSvc.DocumentmarkupDTO> markup)
        {
            bool retValue = false;

            try
            {
                if (markup == null)
                    markup = await (new Lib.ServiceModel.ProjectModel()).GetDocumentmarkupByDrawingPersonnel(drawingId, Login.UserAccount.PersonnelID);

                if (markup != null && markup.Count > 0)
                {
                    string file = markup[0].DocumentMarkupURL;

                    if (!await DrawingEditor.LoadDrawingWithSharepoint(Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.Markup + "/",
                        file, UriKind.Absolute))
                        DrawingEditor.LoadDrawingPath(Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.Markup + "/", file);

                    DrawingEditor.EditMode = WinAppLibrary.Utilities.DrawingEditMode.MarkUp;
                    retValue = true;
                }

                return retValue;
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "DrawingGridView _drawinginfo_InfoViewClicked");
                throw ex;
            }
        }

        private async void DownloadDrawing()
        {
            Login.MasterPage.Loading(true, this);
            
            try
            {
                await (new WinAppLibrary.Utilities.Helper()).DeleteAllUnder(Lib.ContentPath.OffModeFolder);
                WinAppLibrary.Utilities.Helper.DownloadedData = false;

                await _drawingsource.SaveDrawing(_projectid, _moduleid,
                        _drawinggrouping.EngineerTag, _drawinggrouping.DrawingTitle,
                        _drawingsort.SelectedItem == null ? "" : (_drawingsort.SelectedItem as DataItem).UniqueId);

                await (new Lib.ProjectModuleSource()).SaveProjectModuleFull(_projectid, _moduleid);
                WinAppLibrary.Utilities.Helper.DownloadedData = true;
                WinAppLibrary.Utilities.Helper.SimpleMessage("Completed to download drawing", "Successs!");
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "DownloadDrawing", "There was an error to download data from server.Please contact administrator.", "Caution!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private async void SaveDrawing()
        {
            bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : 
                await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, 
                        Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

            if (islogin)
            {
                try
                {
                    var writableImage = await DrawingEditor.RenderDrawing();
                    var stream = await (new WinAppLibrary.Utilities.Helper()).GetJpegStreamFromWriteableBitmap(writableImage);

                    if (DrawingEditor.EditMode == DrawingEditMode.RFI)
                    {
                        MemoryStream clone = new MemoryStream();
                        await stream.CopyToAsync(clone);

                        await (new WinAppLibrary.Utilities.WebDavClient_Old("", Login.UserAccount.WDUser, Login.UserAccount.WDPassword, "")).
                            UploadImageContent(DrawingEditor.FileServer + DrawingEditor.FileName, clone);

                        await (new WinAppLibrary.Utilities.SPDocument()).SaveJpegContent(Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.Drawing + "/", DrawingEditor.FileName, stream);

                        if (stream != null)
                            stream.Dispose();
                        if (clone != null)
                            clone.Dispose();
                    }
                    else
                    {
                        await (new WinAppLibrary.Utilities.SPDocument()).SaveJpegContent(Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.Markup + "/", DrawingEditor.FileName, stream);
                        if (stream != null)
                            stream.Dispose();
                    }

                    if (this.DrawingEditor.IsNew)
                    {
                        this.DrawingEditor.Hide();
                        Login.MasterPage.ShowTopBanner = true;
                    }
                    else
                    {
                        DrawingEditor.UpdateImage(writableImage);

                        WinAppLibrary.Utilities.Helper.SimpleMessage("The drawing was saved to server successfully!", "Completed!");
                    }
                }
                catch (Exception e)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "DrawingViewer SaveDrawing", 
                        "We failed to save the modified drawing. Please contact administrator.", "Error!");

                    if (this.DrawingEditor.IsNew)
                    {
                        this.DrawingEditor.Hide();
                        Login.MasterPage.ShowTopBanner = true;
                    }
                }
            }
            else
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Your account for sharepoint is not valid. Please login again.", "Caution!");
            }

            Login.MasterPage.Loading(false, this);
        }
        #endregion

        #region "Event Handler"
        private void DrawingViewer_Loaded(object sender, RoutedEventArgs e)
        {
            czDrawingViewer.ItemScaleChanged += (s, args) =>
            {
                StretchingPanel.Stretch(false);
            };
        }        

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Loaddrawing();
        }

        private async void MasterPage_AppbarClicked(object sender, object e)
        {
            var btn = (WinAppLibrary.UI.MasterPage.BottomAppBarButton)e;

            switch (btn)
            {
                case WinAppLibrary.UI.MasterPage.BottomAppBarButton.Download:
                    if(Login.LoginMode == WinAppLibrary.UI.LogMode.OffMode)
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Download is not allowed in OffMode.", "Warning!");
                    else if (!WinAppLibrary.Utilities.Helper.ActivateOffMode)
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Please activate off mode at setting panel.", "Warning!");
                    else if(WinAppLibrary.Utilities.Helper.DownloadedData)
                    {
                        if(await WinAppLibrary.Utilities.Helper.YesOrNoMessage("Stored data already exist. Would you overrite on the data?", "Caution!") == true)
                            DownloadDrawing();
                    }
                    else
                        DownloadDrawing();
                    break;
            }
        }

        #region "Left Panel"
        private void Grouping_HeaderClicked(object sender, object e)
        {
            if (e != null)
            {
                var item = e.ToString();

                switch (item)
                {
                    case Lib.HashKey.Key_Reset:
                        if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                        {
                            _drawingsource.ClearSelection();
                            _drawinggrouping.ClearSelection();
                        }
                        break;
                }

                _drawinggrouping.BindList<RevealCommonSvc.ComboBoxDTO>(_drawingsource.GroupList[item], _drawingsource.GroupList[item].Where(x => x.ParentID > 0));
            }
        }

        private void Grouping_ItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                foreach (var item in e.AddedItems)
                    (item as RevealCommonSvc.ComboBoxDTO).ParentID = 1;
                foreach (var item in e.RemovedItems)
                    (item as RevealCommonSvc.ComboBoxDTO).ParentID = 0;
            }
            catch (Exception ee)
            {
                (new Helper()).ExceptionHandler(ee, "Grouping_ItemSelectionChanged");
            }
        }

        private void Grouping_SearchClicked(object sender, RoutedEventArgs e)
        {
            Loaddrawing();
        }

        private async void _drawinginfo_InfoViewClicked(object sender, object e)
        {
            var selected = czDrawingViewer.SelectedItem as RevealProjectSvc.DrawingDTO;

            if (selected != null)
            {
                Login.MasterPage.Loading(true, this);

                bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                if (islogin)
                {
                    try
                    {
                        if (!await LoadMarkupDrawing(selected.DrawingID, null))
                            WinAppLibrary.Utilities.Helper.SimpleMessage("Please create markup first!", "Caution!");
                        else if (this.DrawingEditor.IsNew)
                        {
                            WinAppLibrary.Utilities.Helper.SimpleMessage("We failed to get markup drawing.Please save markup again or try it later!", "Caution!");
                            this.DrawingEditor.Hide();
                        }
                        else
                        {
                            Login.MasterPage.ShowTopBanner = false;
                            DrawingEditor.ViewMode();
                        }
                    }
                    catch
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("There was an error to open markup. Please contact administrator!", "Error!");
                    }

                }
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Your account for sharepoint is not valid. Please login again.", "Caution!");

                Login.MasterPage.Loading(false, this);
            }
        }
        #endregion

        #region "Drawing Controller"
        private void ViewerItem_Loaded(object sender, RoutedEventArgs e)
        {
            var item = sender as Lib.UI.ViewerItem;
            czDrawingViewer.ItemScaleChanged += item.Parent_ScaleChanged;
        }

        private void czDrawingViewer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = czDrawingViewer.SelectedItem as RevealProjectSvc.DrawingDTO;
            if (item != null)
            {
                Login.MasterPage.ShowTopBanner = false;
                Login.MasterPage.ShowUserStatus = false;
                DrawingEditor.LoadDrawing(item.DrawingFilePath, item.DrawingFileURL, UriKind.Absolute);
                
                DrawingEditor.Show();
            }
        }

        private void czDrawingViewer_SelectedItemChanged(CoverZoomLib.ObjectModel.CoverEventArgs e)
        {
            var drawing = e.Item as RevealProjectSvc.DrawingDTO;
            Dictionary<string, string> infolist = new Dictionary<string, string>();
            infolist.Add("DrawingName", drawing.DrawingName == null ? "" : drawing.DrawingName);
            infolist.Add("CWA", drawing.CWPName == null ? "" : drawing.CWPName);
            infolist.Add("Title", drawing.Description == null ? "" : drawing.Description);
            infolist.Add("TypeDesc", drawing.TypeDesc == null ? "" : drawing.TypeDesc);

            _drawinginfo.BindInfo(infolist, drawing.drawingcwp, drawing.drawingref, drawing.mto);
            StretchingPanel.NavigateTo(_drawinginfo.Title);
            
            //_stickynote.BindInfo(_drawingsource.DocumentNote.Where(x => x.DrawingID == item.DrawingID));
            _stickynote.BindInfo(_drawingsource.DocumentNote.ToList().GetRange(0, Math.Min(10, _drawingsource.DocumentNote.Count)));
        }

        private void DrawingEditor_EditRequested(object sender, object e)
        {
            ButtonDialog.Show(this, "Select Drawing For Edit", new string[] { "RFI", "Markup" });
        }

        private void DrawingEditor_SaveRequested(object sender, object e)
        {
            Login.MasterPage.Loading(true, this);
            SaveDrawing();
        }

        private void DrawingEditor_SaveCompleted(object sender, object e)
        {
            Login.MasterPage.Loading(false, this);
            WinAppLibrary.Utilities.Helper.SimpleMessage(e.ToString(), sender.ToString());

            if (this.DrawingEditor.IsNew)
            {
                this.DrawingEditor.Hide();
                Login.MasterPage.ShowTopBanner = true;
                Login.MasterPage.ShowUserStatus = true;
            }
        }

        private void DrawingEditor_Closed(object sender, object e)
        {
            if(this.DrawingEditor.IsNew)

            Login.MasterPage.ShowTopBanner = true;
            Login.MasterPage.ShowUserStatus = true;
        }

        private async void ButtonDialog_CommandClick(object sender, object e)
        {
            ButtonDialog.Hide(this);

            switch (e.ToString())
            {
                case "RFI":
                    DrawingEditor.EditMode = WinAppLibrary.Utilities.DrawingEditMode.RFI;
                    DrawingEditor.InitializeDrawing();
                    DrawingEditor.ShowWithEdit();
                    break;
                case "Markup":
                    Login.MasterPage.Loading(true, this);
                    bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                    if (islogin)
                    {
                        var item = czDrawingViewer.SelectedItem as RevealProjectSvc.DrawingDTO;
                        if (item != null)
                        {
                            try
                            {
                                DrawingEditor.EditMode = WinAppLibrary.Utilities.DrawingEditMode.MarkUp;

                                if (!await LoadMarkupDrawing(item.DrawingID, null))
                                {
                                    var markup = new RevealProjectSvc.DocumentmarkupDTO();
                                    markup.DrawingID = item.DrawingID;
                                    markup.PersonnelID = Login.UserAccount.PersonnelID;
                                    markup.DocumentMarkupURL = item.DrawingName + WinAppLibrary.Utilities.SPCollectionName.MarkupSuffix + Login.UserAccount.PersonnelID + ".jpg";
                                    markup.UpdatedBy = Login.UserAccount.UserName;
                                    markup.UpdatedDate = DateTime.Now;
                                    markup.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;

                                    markup = await (new Lib.ServiceModel.ProjectModel()).SaveDocumentmarkupWithSharePoint(_projectid, markup);

                                    DrawingEditor.LoadDrawingPath(Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.Markup + "/", markup.DocumentMarkupURL);
                                    DrawingEditor.ShowWithEdit();
                                }
                                else
                                    DrawingEditor.ShowWithEdit();
                            }
                            catch
                            {
                                WinAppLibrary.Utilities.Helper.SimpleMessage("There was an error to open markup drawing. Please contact administrator", "Caution!");
                                DrawingEditor.Hide();
                            }
                        }
                    }
                    else
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Your account for sharepoint is not valid. Please login again.", "Caution!");

                    Login.MasterPage.Loading(false, this);
                    break;
            }
        }
        #endregion

        #endregion
    }
}
