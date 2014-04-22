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
using Element.Reveal.DataLibrary.Utilities;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Viewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DrawingGridViewer : WinAppLibrary.Controls.LayoutAwarePage
    {
        private const double ANIMATION_SPEED = 0.5;
        private int _projectid, category, _fileStoreID = 0;
        private string _disciplineCode;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbScreenView;
        Lib.DrawingDataSource _drawingsource = new Lib.DrawingDataSource();

        Lib.UI.DrawingGrouping _drawinggrouping = new Lib.UI.DrawingGrouping();
        Lib.UI.DrawingInfo _drawinginfo = new Lib.UI.DrawingInfo();
        Lib.UI.DrawingSorting _drawingsort = new Lib.UI.DrawingSorting();
        //WinAppLibrary.UI.StickyNote _stickynote = new WinAppLibrary.UI.StickyNote();
        

        public DrawingGridViewer()
        {
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
            base.OnNavigatedFrom(e);
        }

        #region "Private Method"
        private void LoadOption()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.HideUserStatus();
            //Lib.ProjectModuleSource projectmodule = new Lib.ProjectModuleSource();

            _drawinggrouping.HeaderClicked += Grouping_HeaderClicked;
            _drawinggrouping.ItemSelectionChanged += Grouping_ItemSelectionChanged;
            _drawinggrouping.SearchClicked += Grouping_SearchClicked;
            _drawingsort.SortClicked += Grouping_SearchClicked;
            _drawinginfo.InfoViewClicked += _drawinginfo_InfoViewClicked;
            _drawinginfo.ActiveOpacity = 1;
            _drawingsort.ActiveOpacity = 1;

            this.StretchingPanel.AddPanel(_drawinggrouping);
            //this.StretchingPanel.AddPanel(_drawingsort);
            this.StretchingPanel.AddPanel(_drawinginfo);
            //this.StretchingPanel.AddPanel(_stickynote);
            
            this.StretchingPanel.StretchOpened += StretchingPanel_StretchOpened;
            this.DrawingSlider.SlideChanged += Slider_ValueChanged;
            this.DrawingSlider.Minnumber = 1;
            this.DrawingEditor.EnableEdit(Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode ? true : false);

            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;

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
                await _drawingsource.LoadOptionOnMode(_projectid, _disciplineCode);
                _drawingsort.BindInfo(GetSortingCategories());
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadGroup", "There is a problem loading the drawings - Please try again later", "Error!");
            }

            //오프라인 모드 빠져 있어 주석 처리 20140316 nohsukjin Login.MasterPage.SetBottomAppbar(WinAppLibrary.UI.TrueTaskMasterPage.BottomAppBarButton.Download, Visibility.Visible);
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

        private List<DataLibrary.ComboBoxDTO> GetGroupingCategories()
        {
            List<DataLibrary.ComboBoxDTO> retValue = new List<DataLibrary.ComboBoxDTO>();
            retValue.Add(new DataLibrary.ComboBoxDTO() { DataID = 0, DataName = "Reset All", ExtraValue1 = @"&#xE10F;" });
            retValue.Add(new DataLibrary.ComboBoxDTO() { DataID = 1, DataName = "CWP" });
            retValue.Add(new DataLibrary.ComboBoxDTO() { DataID = 2, DataName = "IWP" });
            retValue.Add(new DataLibrary.ComboBoxDTO() { DataID = 3, DataName = "Drawing Type" });

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
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransScroll, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(gvDrawing, 1, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransScroll, 1, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransScroll, 1, ANIMATION_SPEED));

            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, -LayoutRoot.ActualWidth / 2, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(FlipView, 0, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransFlip, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransFlip, 0, ANIMATION_SPEED));

            //To FlipView 
            _sbScreenView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransScroll, -LayoutRoot.ActualWidth / 2, ANIMATION_SPEED));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(gvDrawing, 0, 0, ANIMATION_SPEED));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransScroll, 0, ANIMATION_SPEED));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransScroll, 0, ANIMATION_SPEED));

            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, 0, ANIMATION_SPEED));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(FlipView, 1, 0, ANIMATION_SPEED));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransFlip, 1, ANIMATION_SPEED));
            _sbScreenView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransFlip, 1, ANIMATION_SPEED));
        }

        #region "Drawing"
        private async void Loaddrawing()
        {
            Login.MasterPage.Loading(true, this);
            StretchingPanel.Stretch(false);
            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _drawingsource.GetDrawingOnMode(_projectid,
                                    _drawingsource.GroupList[Lib.HashKey.Key_CWP].Where(x => !string.IsNullOrEmpty(x.ParentID)).Select(x => x.DataID).ToList(),
                                    _drawingsource.GroupList[Lib.HashKey.Key_FIWP].Where(x => !string.IsNullOrEmpty(x.ParentID)).Select(x => x.DataID).ToList(),
                                    _drawingsource.GroupList[Lib.HashKey.Key_DrawingType].Where(x => !string.IsNullOrEmpty(x.ParentID)).Select(x => x.DataID).ToList(),
                                    _drawinggrouping.EngineerTag, _drawinggrouping.DrawingTitle,
                                    _drawingsort.SelectedItem == null ? "" : (_drawingsort.SelectedItem as DataItem).UniqueId,
                                    (int)DrawingSlider.Value);
                }
                else
                {
                    await _drawingsource.GetDrawingOffMode((int)DrawingSlider.Value);
                }

                GrouingDrawing(3);
            }
            catch
            {
            }

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
                            Items = x.Select(y => new DataItem(y.DrawingID.ToString(), y.DrawingName, y.DrawingFileURL.Replace("\\\\", "/").Replace("\\", "/"), y.Description, datagroup) { }).ToObservableCollection()
                        }).ToList();
                        break;
                    //Group by 
                    case 1:
                        source = _drawingsource.DrawingPage.drawing.GroupBy(x => new { x.CWPName, x.CWPID }).Select(x => datagroup = new DataGroup(x.Key.CWPID.ToString(), x.Key.CWPName, "Assets/semantic_cwp.png")
                        {
                            Items = x.Select(y => new DataItem(y.DrawingID.ToString(), y.DrawingName, y.DrawingFileURL.Replace("\\\\", "/").Replace("\\", "/"), y.Description, datagroup) { }).ToObservableCollection()
                        }).ToList();
                        break;
                    default:
                        datagroup = new DataGroup("Drawing", "", "");
                        datagroup.Items = _drawingsource.DrawingPage.drawing.Where(x=> x.DrawingFileURL.ToString().LastIndexOf(".jpg")> -1).Select(x =>
                            new DataItem(x.DrawingID.ToString(), x.DrawingName, x.DrawingFileURL.Replace("\\\\", "/").Replace("\\", "/"), x.Description, datagroup) { }).ToObservableCollection();
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
            this.DrawingSlider.Maxnumber = _drawingsource.DrawingPage.TotalPageCount.Equals(0) ? 1 : _drawingsource.DrawingPage.TotalPageCount;
            this.DrawingSlider.SwingShow(isshow);
            this.FlipView.SelectedItem = this.gvDrawing.SelectedItem = null;
            this.gvViewType.SelectedIndex = 0;
            
            //((GridView)SemanticZoomDrawing.ZoomedOutView).ItemsSource = cvDrawingList.View.CollectionGroups;
        }

        private async Task<DataLibrary.DocumentmarkupDTO> LoadMarkupDrawing(int drawingId, List<DataLibrary.DocumentmarkupDTO> markup)
        {
            DataLibrary.DocumentmarkupDTO retValue = null;

            try
            {
                if (markup == null)
                    markup = await (new Lib.ServiceModel.ProjectModel()).GetDocumentmarkupByDrawingPersonnel(drawingId, Login.UserAccount.PersonnelId);

                if (markup != null && markup.Count > 0)
                {
                    //string file = selected.Title + DataLibrary.Utilities.SPCollectionName.MarkupSuffix + Login.UserAccount.PersonnelID + ".jpg";
                    string markupURL = markup[0].DocumentMarkupURL.Replace("\\\\", "/").Replace("\\", "/");
                    if (!await DrawingEditor.LoadDrawingWithServer(markupURL, UriKind.Absolute))
                        DrawingEditor.LoadDrawingPath(markupURL);

                    DrawingEditor.EditMode = DataLibrary.Utilities.DrawingEditMode.MarkUp;
                    retValue = markup[0];
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

                await _drawingsource.SaveDrawing(_projectid, _disciplineCode, _drawinggrouping.EngineerTag, _drawinggrouping.DrawingTitle,
                    _drawingsort.SelectedItem == null ? "" : (_drawingsort.SelectedItem as DataItem).UniqueId);

                await (new Lib.ProjectModuleSource()).SaveProjectModuleFull(_projectid, _disciplineCode);
                WinAppLibrary.Utilities.Helper.DownloadedData = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "DownloadDrawing", "There was an error to download data from server.Please contact administrator.", "Caution!");
            }

            Login.MasterPage.Loading(false, this);
        }
        #endregion

        private DataLibrary.DrawingDTO GetDrawingById(int drawingId)
        {
            var item = _drawingsource.DrawingPage.drawing.Where(x => x.DrawingID == drawingId).FirstOrDefault();
            return item;
        }

        private Dictionary<string, string> GetDrawingInfo(DataLibrary.DrawingDTO drawing)
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
            try
            {
                var writableImage = await DrawingEditor.RenderDrawing();
                if (writableImage != null)
                {
                    using (Stream stream = await (new WinAppLibrary.Utilities.Helper()).GetJpegStreamFromWriteableBitmap(writableImage))
                    {
                        //if (DrawingEditor.EditMode == DrawingEditMode.RFI)
                        //{
                        //    using (Stream webdav = await (new WinAppLibrary.Utilities.Helper()).GetJpegStreamFromWriteableBitmap(writableImage))
                        //    {
                        //        await (new WinAppLibrary.Utilities.WebDavClient_Old(this.DrawingEditor.FileUrl, Login.UserAccount.WDUser, Login.UserAccount.WDPassword, "")).
                        //            UploadImageContent(DrawingEditor.FileUrl, webdav);

                        //        //await (new WinAppLibrary.Utilities.Helper()).SaveFileStream(Windows.Storage.ApplicationData.Current.LocalFolder, DrawingEditor.FileName, webdav);
                        //    }

                        //    await (new WinAppLibrary.Utilities.FileDocument()).SaveJpegContent(Login.UserAccount.SPURL + "/" + DataLibrary.Utilities.SPCollectionName.Drawing + "/", DrawingEditor.FileName, stream);
                        //}
                        var selected = gvViewType.SelectedIndex == 0 ? gvDrawing.SelectedItem as DataItem : FlipView.SelectedItem as DataItem;

                        if (selected != null)
                        {
                            int drawingId = Convert.ToInt32(selected.UniqueId);
                            var markup = await LoadMarkupDrawing(drawingId, null);
                            if (markup ==  null)
                            {
                                markup = new DataLibrary.DocumentmarkupDTO();
                                markup.DrawingID = drawingId;
                                markup.PersonnelID = Login.UserAccount.PersonnelId;
                                markup.UpdatedBy = Login.UserAccount.UserName;
                                markup.UpdatedDate = DateTime.Now;
                                markup.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;

                                _fileStoreID = 0;
                            }
                            else
                            {
                                _fileStoreID = markup.FileStoreId;
                                markup.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                            }

                            //upFileCollectionDTO
                            DataLibrary.UpfileDTOS upFileCollection = new DataLibrary.UpfileDTOS(); ;
                            upFileCollection.fileStoreDTOList = new List<DataLibrary.FileStoreDTO>();
                            upFileCollection.uploadedFileDTOList = new List<DataLibrary.UploadedFileInfoDTO>();

                            #region fileStoreDTO
                            DataLibrary.FileStoreDTO fileStoreDTOList = new DataLibrary.FileStoreDTO();

                            if (_fileStoreID > 0)
                                fileStoreDTOList.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                            else
                                fileStoreDTOList.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                            fileStoreDTOList.CreatedBy = Login.UserAccount.UserName;
                            fileStoreDTOList.CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                            fileStoreDTOList.FileCategory = DataLibrary.Utilities.FileCategory.REPORT;
                            fileStoreDTOList.FileDescription = selected.Title + DataLibrary.Utilities.SPCollectionName.MarkupSuffix + Login.UserAccount.PersonnelId;
                            fileStoreDTOList.FileStoreId = _fileStoreID;
                            fileStoreDTOList.FileTypeCode = DataLibrary.Utilities.FileType.DRAWING_MARKUP;
                            fileStoreDTOList.ProjectId = Login.UserAccount.CurProjectID;
                            fileStoreDTOList.UpdatedBy = Login.UserAccount.UserName;
                            fileStoreDTOList.UpdatedDate = DateTime.Now.ToString("dd-MM-yyyy");

                            upFileCollection.fileStoreDTOList.Add(fileStoreDTOList);
                            #endregion

                            #region uploadFileDTO
                            DataLibrary.UploadedFileInfoDTO uploadedFileDTOList = new DataLibrary.UploadedFileInfoDTO();

                            if (_fileStoreID > 0)
                                uploadedFileDTOList.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                            else
                                uploadedFileDTOList.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;

                            var wImage = await DrawingEditor.RenderDrawing();
                            var jStream = await (new WinAppLibrary.Utilities.Helper()).GetJpegStreamFromWriteableBitmap(wImage);

                            byte[] byteArray = new byte[jStream.Length];
                            jStream.Position = 0;
                            jStream.Read(byteArray, 0, (int)jStream.Length);

                            uploadedFileDTOList.byteFile = byteArray;
                            uploadedFileDTOList.FileExtension = "jpg";
                            uploadedFileDTOList.Size = byteArray.Length;
                            uploadedFileDTOList.CreatedBy = Login.UserAccount.UserName;
                            uploadedFileDTOList.CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                            uploadedFileDTOList.FileStoreId = _fileStoreID;
                            uploadedFileDTOList.UpdatedBy = Login.UserAccount.UserName;
                            uploadedFileDTOList.UpdatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                            uploadedFileDTOList.UploadedBy = Login.UserAccount.UserName;
                            uploadedFileDTOList.UploadedDate = DateTime.Now;

                            upFileCollection.uploadedFileDTOList.Add(uploadedFileDTOList);
                            #endregion

                            markup = await (new Lib.ServiceModel.ProjectModel()).SaveDocumentmarkupWithMarkupImage(markup, upFileCollection);
                        }

                    }

                    if (this.DrawingEditor.IsNew)
                    {
                        this.DrawingEditor.Hide();
                        //Login.MasterPage.ShowTopBanner = true;
                    }
                    else
                    {
                        DrawingEditor.UpdateImage(writableImage);

                        WinAppLibrary.Utilities.Helper.SimpleMessage("Successfully Saved", "-!");
                    }
                }
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the drawing board - Please try again later", "Caution!");
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "DawingGridView SaveDrawing", "Failed to save the modified drawing - Please try again later.", "Saving Error!");

                if (this.DrawingEditor.IsNew)
                {
                    this.DrawingEditor.Hide();
                    //Login.MasterPage.ShowTopBanner = true;
                }
            }

            Login.MasterPage.Loading(false, this);
        }
        #endregion

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

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
                        //Login.MasterPage.ShowTopBanner = false;
                        var drawing = _drawingsource.DrawingPage.drawing.Where(x => x.DrawingID.ToString() == item.UniqueId).FirstOrDefault();
                        //DrawingEditor.LoadDrawing(drawing.DrawingFilePath, drawing.DrawingFileURL, UriKind.Absolute);
                        DrawingEditor.LoadDrawing(drawing.DrawingFileURL.Replace("\\\\", "/").Replace("\\", "/"), UriKind.Absolute);
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
                        //_stickynote.BindInfo(_drawingsource.DocumentNote.ToList().GetRange(0, Math.Min(10, _drawingsource.DocumentNote.Count)));
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
                       // _stickynote.BindInfo(_drawingsource.DocumentNote.ToList().GetRange(0, Math.Min(10, _drawingsource.DocumentNote.Count)));
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
                if (item.ImagePath.ToLower().Contains(".pdf") || item.ImagePath.ToLower().Contains(".ozd") || item.ImagePath.ToLower().Contains(".ozr"))
                    WinAppLibrary.Utilities.Helper.SimpleMessage("PDF file can not be edited", "Alert");
                else
                {
                    //Login.MasterPage.ShowTopBanner = false;
                    var drawing = _drawingsource.DrawingPage.drawing.Where(x => x.DrawingID.ToString() == item.UniqueId).FirstOrDefault();
                    //DrawingEditor.LoadDrawing(drawing.DrawingFilePath, drawing.DrawingFileURL, UriKind.Absolute);
                    DrawingEditor.LoadDrawing(drawing.DrawingFileURL.Replace("\\\\", "/").Replace("\\", "/"), UriKind.Absolute);
                    DrawingEditor.Show();
                }
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
           // ButtonDialog.Show(this, "Select edit mode", new string[] { "RFI", "Markup" });
            Login.MasterPage.Loading(true, this);

            var selected = gvViewType.SelectedIndex == 0 ? gvDrawing.SelectedItem as DataItem : FlipView.SelectedItem as DataItem;
            if (selected != null)
            {
                try
                {
                    DrawingEditor.EditMode = DataLibrary.Utilities.DrawingEditMode.MarkUp;
                    DrawingEditor.ShowWithEdit();

                    //int drawingId = Convert.ToInt32(selected.UniqueId);
                    //if (!await LoadMarkupDrawing(drawingId, null))
                    //{
                    //    var markup = new DataLibrary.DocumentmarkupDTO();
                    //    markup.DrawingID = drawingId;
                    //    markup.PersonnelID = Login.UserAccount.PersonnelId;
                    //    markup.UpdatedBy = Login.UserAccount.UserName;
                    //    markup.UpdatedDate = DateTime.Now;
                    //    markup.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;


                    //    markup = await (new Lib.ServiceModel.ProjectModel()).SaveDocumentmarkupWithMarkupImage(markup, upFileCollection);

                    //    DrawingEditor.LoadDrawingPath(markup.DocumentMarkupURL);  // file full path
                    //    DrawingEditor.ShowWithEdit();
                    //}
                    //else
                    //    DrawingEditor.ShowWithEdit();
                }
                catch
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the markup drawing - Please try again later", "Loading Error");
                    DrawingEditor.Hide();
                }
            }
            Login.MasterPage.Loading(false, this);
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
                //Login.MasterPage.ShowTopBanner = true;
            }
        }

        private void DrawingEditor_Closed(object sender, object e)
        {
            //Login.MasterPage.ShowTopBanner = true;
        }

        private void ButtonDialog_CommandClick(object sender, object e)
        {
            ButtonDialog.Hide(this);

            switch (e.ToString())
            {
                case "RFI":
                    DrawingEditor.EditMode = DataLibrary.Utilities.DrawingEditMode.RFI;
                    DrawingEditor.InitializeDrawing();
                    DrawingEditor.ShowWithEdit();
                    break;
                case "Markup":
                    Login.MasterPage.Loading(true, this);
                    var selected = gvViewType.SelectedIndex == 0 ? gvDrawing.SelectedItem as DataItem : FlipView.SelectedItem as DataItem;
                    if (selected != null)
                    {
                        try
                        {
                            DrawingEditor.EditMode = DataLibrary.Utilities.DrawingEditMode.MarkUp;
                            DrawingEditor.ShowWithEdit();

                            //int drawingId = Convert.ToInt32(selected.UniqueId);
                            //if (!await LoadMarkupDrawing(drawingId, null))
                            //{
                            //    var markup = new DataLibrary.DocumentmarkupDTO();
                            //    markup.DrawingID = drawingId;
                            //    markup.PersonnelID = Login.UserAccount.PersonnelId;
                            //    markup.DocumentMarkupURL = selected.Title + DataLibrary.Utilities.SPCollectionName.MarkupSuffix + Login.UserAccount.PersonnelId + ".jpg";  // file title
                            //    markup.UpdatedBy = Login.UserAccount.UserName;
                            //    markup.UpdatedDate = DateTime.Now;
                            //    markup.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;

                            //    markup = await (new Lib.ServiceModel.ProjectModel()).SaveDocumentmarkupWithMarkupImage(markup, null);

                            //    DrawingEditor.LoadDrawingPath(markup.DocumentMarkupURL);  // file full path
                            //    DrawingEditor.ShowWithEdit();
                            //}
                            //else
                            //    DrawingEditor.ShowWithEdit();
                        }
                        catch
                        {
                            WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the markup drawing - Please try again later", "Loading Error");
                            DrawingEditor.Hide();
                        }
                    }

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

                _drawinggrouping.BindList<DataLibrary.ComboCodeBoxDTO>(_drawingsource.GroupList[item], _drawingsource.GroupList[item].Where(x => !string.IsNullOrEmpty(x.ParentID)));
            }
        }

        private void Grouping_ItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                foreach (var item in e.AddedItems)
                    (item as DataLibrary.ComboCodeBoxDTO).ParentID = "1";
                foreach (var item in e.RemovedItems)
                    (item as DataLibrary.ComboCodeBoxDTO).ParentID = "";
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

        private async  void _drawinginfo_InfoViewClicked(object sender, object e)
        {
            var selected = gvViewType.SelectedIndex == 0 ? gvDrawing.SelectedItem  as DataItem : FlipView.SelectedItem as DataItem;

            if (selected != null)
            {
                Login.MasterPage.Loading(true, this);

                if (selected.ImagePath.ToLower().Contains(".pdf") || selected.ImagePath.ToLower().Contains(".ozd") || selected.ImagePath.ToLower().Contains(".ozr"))
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Can't Edit", "Alert");
                else
                {
                    int drawingId = Convert.ToInt32(selected.UniqueId);
                    try
                    {
                        if (await LoadMarkupDrawing(drawingId, null) == null)
                            WinAppLibrary.Utilities.Helper.SimpleMessage("Please create markup first!", "Warning");
                        else if (this.DrawingEditor.IsNew)
                        {
                            WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the markup drawing - Please try again later", "Loading Error");
                            this.DrawingEditor.Hide();
                        }
                        else
                        {
                            //Login.MasterPage.ShowTopBanner = false;
                            DrawingEditor.ViewMode();
                        }
                    }
                    catch
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the markup drawing - Please try again later", "Loading Error");
                    }
                }

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
