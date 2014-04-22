using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
using WinAppLibrary.Extensions;

namespace Element.Reveal.Crew.Discipline.Drawing
{
    public sealed partial class DrawingGridViewer : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid, category;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbScreenView;
        Lib.DataSource.DrawingDataSource _drawingsource = new Lib.DataSource.DrawingDataSource();

        Lib.UI.DrawingGrouping _drawinggrouping = new Lib.UI.DrawingGrouping();
        Lib.UI.DrawingInfo _drawinginfo = new Lib.UI.DrawingInfo();
        Lib.UI.DrawingSorting _drawingsort = new Lib.UI.DrawingSorting();
        WinAppLibrary.UI.StickyNote _stickynote = new WinAppLibrary.UI.StickyNote();

        public DrawingGridViewer()
        {
            Login.MasterPage.ShowTopBanner = false;
            Login.MasterPage.ShowUserStatus = false;
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            LoadOption();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Login.MasterPage.AppbarClicked -= MasterPage_AppbarClicked;
            Login.MasterPage.ShowUserStatus = true;
            Login.MasterPage.ShowTopBanner = true;
            base.OnNavigatedFrom(e);
        }

        #region "Private Method"
        private void LoadOption()
        {
            Login.MasterPage.Loading(true, this);
            Lib.DataSource.ProjectModuleSource projectmodule = new Lib.DataSource.ProjectModuleSource();

            _drawinggrouping.HeaderClicked += Grouping_HeaderClicked;
            _drawinggrouping.ItemSelectionChanged += Grouping_ItemSelectionChanged;
            _drawinggrouping.SearchClicked += Grouping_SearchClicked;
            _drawingsort.SortClicked += Grouping_SearchClicked;
            _drawinginfo.InfoViewClicked += _drawinginfo_InfoViewClicked;
            _drawinginfo.ActiveOpacity = 1;
            _drawingsort.ActiveOpacity = 1;

            this.StretchingPanel.AddPanel(_drawinggrouping);
          //  this.StretchingPanel.AddPanel(_drawingsort);
            this.StretchingPanel.AddPanel(_drawinginfo);
          //  this.StretchingPanel.AddPanel(_stickynote);

            this.StretchingPanel.StretchOpened += StretchingPanel_StretchOpened;
            this.DrawingSlider.SlideChanged += Slider_ValueChanged;
            this.DrawingSlider.Minnumber = 1;
            this.DrawingEditor.EnableEdit(Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode ? true : false);

            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

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

            LoadStoryBoardSwitch();
            StretchingPanel.Stretch(true);
        }

        private async void LoadOptionOnMode()
        {
            var common = new Lib.ServiceModel.CommonModel();
            try
            {
                await _drawingsource.LoadOptionOnMode(_projectid, _moduleid);
                _drawingsort.BindInfo(GetSortingCategories());
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadGroup", "There is a problem loading the drawings - Please try again later", "Error!");
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
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadGroup", "There is a problem loading the drawings - Please try again later", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private List<RevealCommonSvc.ComboBoxDTO> GetGroupingCategories()
        {
            List<RevealCommonSvc.ComboBoxDTO> retValue = new List<RevealCommonSvc.ComboBoxDTO>();
            retValue.Add(new RevealCommonSvc.ComboBoxDTO() { DataID = 0, DataName = "Reset All", ExtraValue1 = @"&#xE10F;" });
            retValue.Add(new RevealCommonSvc.ComboBoxDTO() { DataID = 1, DataName = "CWP" });
            retValue.Add(new RevealCommonSvc.ComboBoxDTO() { DataID = 2, DataName = "IWP" });
            retValue.Add(new RevealCommonSvc.ComboBoxDTO() { DataID = 3, DataName = "Drawing Type" });

            return retValue;
        }

        private ObservableCollection<DataGroup> GetSortingCategories()
        {
            ObservableCollection<DataGroup> retValue = new ObservableCollection<DataGroup>();

            var group = new DataGroup("Sotring", "Sorting Drawing", "");
            retValue.Add(group);
            retValue[0].Items.Add(new DataItem("DrawingName", "DrawingName", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingPlantNo", "Plant Number", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingSubject", "Drawing Title", "", "", group));
            retValue[0].Items.Add(new DataItem("CWPName", "CWP", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingTypeLUID", "Drawing Type", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingNo", "Drawing Number", "", "", group));

            return retValue;
        }

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbGridView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransScroll, 0, Lib.Threshold.ANIMATION_TIME));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(gvDrawing, 1, 0, Lib.Threshold.ANIMATION_TIME));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransScroll, 1, Lib.Threshold.ANIMATION_TIME));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransScroll, 1, Lib.Threshold.ANIMATION_TIME));

            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, -LayoutRoot.ActualWidth / 2, Lib.Threshold.ANIMATION_TIME));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(FlipView, 0, 0, Lib.Threshold.ANIMATION_TIME));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransFlip, 0, Lib.Threshold.ANIMATION_TIME));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransFlip, 0, Lib.Threshold.ANIMATION_TIME));

            //To FlipView 
            _sbScreenView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransScroll, -LayoutRoot.ActualWidth / 2, Lib.Threshold.ANIMATION_TIME));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(gvDrawing, 0, 0, Lib.Threshold.ANIMATION_TIME));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransScroll, 0, Lib.Threshold.ANIMATION_TIME));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransScroll, 0, Lib.Threshold.ANIMATION_TIME));

            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, 0, Lib.Threshold.ANIMATION_TIME));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(FlipView, 1, 0, Lib.Threshold.ANIMATION_TIME));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransFlip, 1, Lib.Threshold.ANIMATION_TIME));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransFlip, 1, Lib.Threshold.ANIMATION_TIME));
        }

        #region "Drawing"
        private async void Loaddrawing()
        {
            Login.MasterPage.Loading(true, this);
            StretchingPanel.Stretch(false);

            if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
            {
                await _drawingsource.GetDrawingOnMode(_projectid,
                                _drawingsource.GroupList[Lib.HashKey.Key_CWP].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawingsource.GroupList[Lib.HashKey.Key_FIWP].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawingsource.GroupList[Lib.HashKey.Key_DrawingType].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawinggrouping.EngineerTag, _drawinggrouping.DrawingTitle,
                                _drawingsort.SelectedItem == null ? "" : (_drawingsort.SelectedItem as DataItem).UniqueId,
                                (int)DrawingSlider.Value);
            }
            else
            {
                await _drawingsource.GetDrawingOffMode((int)DrawingSlider.Value);
            }

            GrouingDrawing(3);

            GC.Collect();
            Login.MasterPage.Loading(false, this);
        }

        private void GrouingDrawing(int sortindex)
        {
            List<DataGroup> source = null;
            DataGroup datagroup = null;
            bool isshow = false;

            if (_drawingsource.DrawingPage != null && _drawingsource.DrawingPage.drawing != null)
            {
                switch (sortindex)
                {
                    //Group by CWP
                    case 0:
                        source = _drawingsource.DrawingPage.drawing.GroupBy(x => new { x.CWPName, x.CWPID }).Select(x => datagroup = new DataGroup(x.Key.CWPID.ToString(), x.Key.CWPName, "Assets/semantic_cwp.png")
                        {
                            Items = x.Select(y => new DataItem(y.DrawingID.ToString(), y.DrawingName, y.DrawingFilePath + y.DrawingFileURL, y.Description, datagroup) { }).ToObservableCollection()
                        }).ToList();
                        break;
                    //Group by 
                    case 1:
                        source = _drawingsource.DrawingPage.drawing.GroupBy(x => new { x.CWPName, x.CWPID }).Select(x => datagroup = new DataGroup(x.Key.CWPID.ToString(), x.Key.CWPName, "Assets/semantic_cwp.png")
                        {
                            Items = x.Select(y => new DataItem(y.DrawingID.ToString(), y.DrawingName, y.DrawingFileURL + y.DrawingFilePath, y.Description, datagroup) { }).ToObservableCollection()
                        }).ToList();
                        break;
                    default:
                        datagroup = new DataGroup("Drawing", "", "");
                        datagroup.Items = _drawingsource.DrawingPage.drawing.Select(x =>
                            new DataItem(x.DrawingID.ToString(), x.DrawingName, x.DrawingFilePath + x.DrawingFileURL, x.Description, datagroup) { }).ToObservableCollection();
                        source = new List<DataGroup>();
                        source.Add(datagroup);
                        break;
                }

                isshow = true;
            }
            else
                Helper.SimpleMessage("There is no drawings exist.", "Alert!");

            this.DefaultViewModel["Drawings"] = source;
            this.DrawingSlider.Value = _drawingsource.DrawingPage.CurrentPage;
            this.DrawingSlider.Maxnumber = _drawingsource.DrawingPage.TotalPageCount;
            this.DrawingSlider.SwingShow(isshow);
            this.FlipView.SelectedItem = this.gvDrawing.SelectedItem = null;
            this.gvViewType.SelectedIndex = 0;

            //((GridView)SemanticZoomDrawing.ZoomedOutView).ItemsSource = cvDrawingList.View.CollectionGroups;
        }

        private async Task<bool> LoadMarkupDrawing(int drawingId, List<RevealProjectSvc.DocumentmarkupDTO> markup)
        {
            bool retValue = false;

            try
            {
                if (markup == null)
                    markup = await (new Lib.ServiceModel.ProjectModel()).GetDocumentmarkupByDrawingPersonnel(drawingId, Login.UserAccount.PersonnelID);

                if (markup != null && markup.Count > 0)
                {
                    //string file = selected.Title + WinAppLibrary.Utilities.SPCollectionName.MarkupSuffix + Login.UserAccount.PersonnelID + ".jpg";
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
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "DrawingGridView LoadMarkupDrawing");
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

                await _drawingsource.SaveDrawing(_projectid, _moduleid, _drawinggrouping.EngineerTag, _drawinggrouping.DrawingTitle,
                    _drawingsort.SelectedItem == null ? "" : (_drawingsort.SelectedItem as DataItem).UniqueId);

                await (new Lib.DataSource.ProjectModuleSource()).SaveProjectModuleFull(_projectid, _moduleid);
                WinAppLibrary.Utilities.Helper.DownloadedData = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "DownloadDrawing", "There was an error to download data from server.Please contact administrator.", "Caution!");
            }

            Login.MasterPage.Loading(false, this);
        }
        #endregion

        private RevealProjectSvc.DrawingDTO GetDrawingById(int drawingId)
        {
            var item = _drawingsource.DrawingPage.drawing.Where(x => x.DrawingID == drawingId).FirstOrDefault();
            return item;
        }

        private Dictionary<string, string> GetDrawingInfo(RevealProjectSvc.DrawingDTO drawing)
        {
            Dictionary<string, string> infolist = new Dictionary<string, string>();
            infolist.Add("DrawingName", drawing.DrawingName == null ? "" : drawing.DrawingName);
            infolist.Add("CWA", drawing.CWPName == null ? "" : drawing.CWPName);
            infolist.Add("Title", drawing.Description == null ? "" : drawing.Description);
            infolist.Add("TypeDesc", drawing.TypeDesc == null ? "" : drawing.TypeDesc);
            return infolist;

            //This was dismissed because of faster binding to Drawing Info Panel
            //ObservableCollection<DataGroup> groupedlist = new ObservableCollection<DataGroup>();
            //var group = new DataGroup("DrawingName", "DrawingName", "");
            //groupedlist.Add(group);
            //groupedlist[0].Items.Add(new DataItem(drawing.DrawingID + "DrawingName", drawing.DrawingName, "", "", groupedlist[0]) { });

            //group = new DataGroup("CWA", "CWA", "");
            //groupedlist.Add(group);
            //groupedlist[1].Items.Add(new DataItem(drawing.CWPID + "CWA", drawing.CWPName, "", "", groupedlist[1]) { });

            //group = new DataGroup("Title", "Title", "");
            //groupedlist.Add(group);
            //groupedlist[2].Items.Add(new DataItem(drawing.Description + "Title", drawing.Description, "", "", groupedlist[2]) { });

            //group = new DataGroup("TypeDesc", "Type Description", "");
            //groupedlist.Add(group);
            //groupedlist[3].Items.Add(new DataItem(drawing.TypeDesc, drawing.TypeDesc, "", "", groupedlist[3]) { });
        }
        private void ShiftOver(bool shift)
        {
            if (shift)
            {
                dbaniDrawingViewer.To = StretchingPanel.ActualWidth + LayoutRoot.ColumnDefinitions[0].Width.Value;
                sbShiftLeft.Stop();
                sbShiftRight.Begin();
            }
            else
            {
                sbShiftRight.Stop();
                sbShiftLeft.Begin();
            }
        }

        private async void SaveDrawing()
        {
            bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

            if (islogin)
            {
                try
                {
                    var writableImage = await DrawingEditor.RenderDrawing();
                    if (writableImage != null)
                    {
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
                        }
                        else
                        {
                            DrawingEditor.UpdateImage(writableImage);

                            WinAppLibrary.Utilities.Helper.SimpleMessage("Successfully Saved", "Completed!");
                        }
                    }
                    else
                        WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the drawing board - Please try again later", "Caution!");
                }
                catch (Exception e)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "DawingGridView SaveDrawing",
                        "Failed to save the modified drawing - Please try again later.", "Caution!");

                    if (this.DrawingEditor.IsNew)
                    {
                        this.DrawingEditor.Hide();
                    }
                }
            }
            else
                WinAppLibrary.Utilities.Helper.SimpleMessage("Your account for sharepoint is not valid. Please login again.", "Caution!");

            Login.MasterPage.Loading(false, this);
        }
        #endregion

        #region "Event Handler"

        #region "Drawing Viewer"
        private void gvDrawing_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gvViewType.SelectedIndex == 0)
            {
                if (gvDrawing.SelectedItem == e.ClickedItem)
                {
                    var item = e.ClickedItem as DataItem;
                    if (item != null)
                    {
                        var drawing = _drawingsource.DrawingPage.drawing.Where(x => x.DrawingID.ToString() == item.UniqueId).FirstOrDefault();
                        DrawingEditor.LoadDrawing(drawing.DrawingFilePath, drawing.DrawingFileURL, UriKind.Absolute);
                        DrawingEditor.Show();
                    }
                }
                else
                {
                    gvDrawing.SelectedItem = e.ClickedItem;
                    try
                    {
                        var item = GetDrawingById(Convert.ToInt32((e.ClickedItem as DataItem).UniqueId));
                        var groupedlist = GetDrawingInfo(item);

                        _drawinginfo.BindInfo(groupedlist, item.drawingcwp, item.drawingref, item.mto);
                        //_stickynote.BindInfo(_drawingsource.DocumentNote.Where(x => x.DrawingID == id));
                        _stickynote.BindInfo(_drawingsource.DocumentNote.ToList().GetRange(0, Math.Min(10, _drawingsource.DocumentNote.Count)));
                    }
                    catch { }

                    StretchingPanel.NavigateTo(_drawinginfo.Title);

                    if (gvDrawing.SelectedIndex < 8)
                        ShiftOver(true);
                    else
                        ShiftOver(false);
                }
            }
        }

        #region "FlipView"
        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gvViewType.SelectedIndex == 1 && e.AddedItems != null && e.AddedItems.Count > 0)
            {
                try
                {
                    var id = (e.AddedItems[0] as DataItem).UniqueId;
                    if (id != (gvDrawing.SelectedItem as DataItem).UniqueId)
                    {
                        var item = GetDrawingById(Convert.ToInt32(id));
                        var groupedlist = GetDrawingInfo(item);

                        _drawinginfo.BindInfo(groupedlist, item.drawingcwp, item.drawingref, item.mto);
                        //_stickynote.BindInfo(_drawingsource.DocumentNote.Where(x => x.DrawingID == id));
                        _stickynote.BindInfo(_drawingsource.DocumentNote.ToList().GetRange(0, Math.Min(10, _drawingsource.DocumentNote.Count)));
                    }
                }
                catch { }
                StretchingPanel.NavigateTo(_drawinginfo.Title);
            }
        }

        private void FlipViewItem_Clicked(object sender, object e)
        {
            var item = e as DataItem;
            if (item != null)
            {
                var drawing = _drawingsource.DrawingPage.drawing.Where(x => x.DrawingID.ToString() == item.UniqueId).FirstOrDefault();
                DrawingEditor.LoadDrawing(drawing.DrawingFilePath, drawing.DrawingFileURL, UriKind.Absolute);
                DrawingEditor.Show();
            }
        }
        #endregion

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (this.ScrollViewer.HorizontalOffset == 0)
            {
                StretchingPanel.Stretch(true);
                ShiftOver(true);
            }
            else
            {
                if (TranslateTransSemnatic.X > 0)
                {
                    StretchingPanel.Stretch(false);
                    ShiftOver(false);
                }
            }
        }
        #endregion

        #region "Top & Bottom"
        private void gvViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string id;
            object selected;

            switch (gvViewType.SelectedIndex)
            {
                //Grid
                case 0:
                    if (this.FlipView.SelectedItem != null)
                    {
                        id = (FlipView.SelectedItem as DataItem).UniqueId;
                        selected = gvDrawing.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
                        if (selected != null)
                            gvDrawing.SelectedItem = selected;
                    }
                    _sbGridView.Pause();
                    TranTransScroll.X = LayoutRoot.ActualWidth / 2;
                    _sbGridView.Begin();
                    ShiftOver(false);
                    break;
                //"Screen"
                case 1:
                    if (gvDrawing.SelectedItem != null)
                    {
                        id = (gvDrawing.SelectedItem as DataItem).UniqueId;
                        selected = FlipView.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
                        if (selected != null)
                            FlipView.SelectedItem = selected;
                    }
                    _sbScreenView.Pause();
                    TranTransFlip.X = LayoutRoot.ActualWidth / 2;
                    _sbScreenView.Begin();
                    break;
            }

            StretchingPanel.Stretch(false);
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
                    if (Login.LoginMode == WinAppLibrary.UI.LogMode.OffMode)
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Download is not allowed in OffMode.", "Warning!");
                    else if (!WinAppLibrary.Utilities.Helper.ActivateOffMode)
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Please activate off mode at setting panel.", "Warning!");
                    else if (WinAppLibrary.Utilities.Helper.DownloadedData)
                    {
                        if (await WinAppLibrary.Utilities.Helper.YesOrNoMessage("Stored data already exist. Would you overrite on the data?", "Caution!") == true)
                            DownloadDrawing();
                    }
                    else
                        DownloadDrawing();
                    break;
            }
        }
        #endregion

        #region "Drawing Supporter"
        private void DrawingEditor_EditRequested(object sender, object e)
        {
            ButtonDialog.Show(this, "Select edit mode", new string[] { "RFI", "Markup" });
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
            }
        }

        private void DrawingEditor_Closed(object sender, object e)
        {
            //Login.MasterPage.ShowTopBanner = true;
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
                        var selected = gvViewType.SelectedIndex == 0 ? gvDrawing.SelectedItem as DataItem : FlipView.SelectedItem as DataItem;
                        if (selected != null)
                        {
                            try
                            {
                                DrawingEditor.EditMode = WinAppLibrary.Utilities.DrawingEditMode.MarkUp;

                                int drawingId = Convert.ToInt32(selected.UniqueId);
                                if (!await LoadMarkupDrawing(drawingId, null))
                                {
                                    var markup = new RevealProjectSvc.DocumentmarkupDTO();
                                    markup.DrawingID = drawingId;
                                    markup.PersonnelID = Login.UserAccount.PersonnelID;
                                    markup.DocumentMarkupURL = selected.Title + WinAppLibrary.Utilities.SPCollectionName.MarkupSuffix + Login.UserAccount.PersonnelID + ".jpg";
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
                                WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the markup drawing - Please try again later", "Caution!");
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

        #region "Left Panel"
        private void Grouping_HeaderClicked(object sender, object e)
        {
            if (e != null)
            {
                var item = e.ToString();

                switch (item)
                {
                    case Lib.HashKey.Key_Reset:
                        _drawingsource.ClearSelection();
                        _drawinggrouping.ClearSelection();
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
            DrawingSlider.Value = 1;
            Loaddrawing();
        }

        private async void _drawinginfo_InfoViewClicked(object sender, object e)
        {
            var selected = gvViewType.SelectedIndex == 0 ? gvDrawing.SelectedItem as DataItem : FlipView.SelectedItem as DataItem;

            if (selected != null)
            {
                Login.MasterPage.Loading(true, this);

                bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                if (islogin)
                {
                    int drawingId = Convert.ToInt32(selected.UniqueId);
                    try
                    {
                        if (!await LoadMarkupDrawing(drawingId, null))
                            WinAppLibrary.Utilities.Helper.SimpleMessage("Please create markup first!", "Caution!");
                        else if (this.DrawingEditor.IsNew)
                        {
                            WinAppLibrary.Utilities.Helper.SimpleMessage("We failed to get markup drawing.Please save markup again or try it later!", "Caution!");
                            this.DrawingEditor.Hide();
                        }
                        else
                        {
                            DrawingEditor.ViewMode();
                        }
                    }
                    catch
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the markup drawing - Please try again later", "Error!");
                    }

                }
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Your account for sharepoint is not valid. Please login again.", "Caution!");

                Login.MasterPage.Loading(false, this);
            }
        }

        private void StretchingPanel_StretchOpened(object sender, object e)
        {
            if (this.ScrollViewer.HorizontalOffset == 0 && this.gvDrawing.SelectedIndex < 8)
                ShiftOver((bool)e);
        }
        #endregion

        #endregion
    }
}
