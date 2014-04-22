using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using WinAppLibrary.Converters;

namespace Element.Reveal.Crew.Discipline.IWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IWPGridViewer : WinAppLibrary.Controls.LayoutAwarePage
    {
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Lib.UI.IWPOption _iwpoption = new Lib.UI.IWPOption();
        Lib.DataSource.IWPDataSource _iwpsource = new Lib.DataSource.IWPDataSource();

        //itr
        List<RevealProjectSvc.DocumentDTO> _documentdto = new List<RevealProjectSvc.DocumentDTO>();
        private int _documentNo;

        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbFlipView, _sbItrViewON, _sbItrViewOFF;
        private int _projectid, _moduleid;

        public IWPGridViewer()
        {
            Login.MasterPage.ShowTopBanner = false;
            Login.MasterPage.ShowUserStatus = false;
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            LoadOption();
            LoadStoryBoardSwitch();
        }

        #region "Event Handler"

        #region "Drawing Viewer"
        private async void gvDrawing_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gvViewType.SelectedIndex == 1)
            {
                if (gvDrawing.SelectedItem == e.ClickedItem)
                {
                    DataItem item = e.ClickedItem as DataItem;

                    string[] addrs = ExtractAddress(item.ImagePath);

                    if (!await DrawingEditor.LoadDrawingWithSharepoint(addrs[0], addrs[1], UriKind.Absolute))
                        await DrawingEditor.LoadDefault();

                    DrawingEditor.ShowWithEdit();
                }
                else
                {
                    gvDrawing.SelectedItem = e.ClickedItem;
                    //StretchingPanel.Stretch(true);
                }
            }
        }

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

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScaleTransFlip.ScaleX > 0 && e.AddedItems != null && e.AddedItems.Count > 0)
            {
                try
                {
                    var id = (e.AddedItems[0] as DataItem).UniqueId;
                    var selected = gvDrawing.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();

                    if (selected != null)
                        gvDrawing.SelectedItem = selected;
                    //var sources = GetDrawingInfo(id);
                    //_drawinginfo.BindInfo(sources);
                    ////_stickynote.BindInfo(_drawingsource.DocumentNote.Where(x => x.DrawingID == id));
                    //_stickynote.BindInfo(_drawingsource.DocumentNote.ToList().GetRange(0, Math.Min(10, _drawingsource.DocumentNote.Count)));
                }
                catch { }
                //StretchingPanel.NavigateTo(_drawinginfo.Title);
            }
        }

        private async void FlipViewItem_Clicked(object sender, object e)
        {
            var item = e as DataItem;
            if (item != null)
            {
                string[] addrs = ExtractAddress(item.ImagePath);

                if (!await DrawingEditor.LoadDrawingWithSharepoint(addrs[0], addrs[1], UriKind.Absolute))
                    await DrawingEditor.LoadDefault();

                DrawingEditor.ShowWithEdit();
            }
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
        }

        private void DrawingEditor_Closed(object sender, object e)
        {
            //Login.MasterPage.ShowTopBanner = true;
        }
        #endregion

        #region "Left Panel"
        private async void _iwpoption_CWPClicked(object sender, object e)
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                var item = e as RevealCommonSvc.ComboBoxDTO;
                var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByCwp_Combo(item.DataID, _projectid, _moduleid);
                _iwpoption.BindIwp(result);

            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "_iwpoption_CWPClicked", "There was an error load IWP drawing. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void _iwpoption_ViewClicked(object sender, object e)
        {
            try
            {
                Loaddrawing();
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "_iwpoption_ViewClicked", "There was an error load IWP drawing. Pleae contact administrator", "Error!");
            }
        }

        private void StretchingPanel_StretchOpened(object sender, object e)
        {
            if (this.ScrollViewer.HorizontalOffset == 0)
                ShiftOver((bool)e);
        }
        #endregion

        private async void LoadDocument()
        {
            try
            {
                var item = _iwpoption.SelectredIWP as RevealCommonSvc.ComboBoxDTO;
                if (item != null)
                {
                    if (!WinAppLibrary.Utilities.SPDocument.IsLogin)
                        await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                }
                _documentdto = await _iwpsource.GetItrDocumentForFIWPByFIWPID(0, item.DataID, _projectid, _moduleid);
                if (_documentdto.Count > 0 && !string.IsNullOrEmpty(_documentdto[0].LocationURL))
                {
                    byte[] bytes = await (new WinAppLibrary.Utilities.SPDocument()).GetDocument("https://elements.sharepoint.com/RevealDev/SafetyDocuments/EQUIPMENT-Battery_Care_And_Tips.pdf");
                    Stream retValue = null;
                    retValue = new MemoryStream(bytes);
                    PdfViewer.LoadDocument(retValue);
                    _documentNo = 0;
                    btnPrev.IsEnabled = false;
                    if (_documentdto.Count == 0)
                        btnNext.IsEnabled = false;
                       
                }
                else
                {
                    btnNext.IsEnabled = false;
                    btnPrev.IsEnabled = false;
                }
               
            }
            catch
            {
            }
           
        }

        private void gvViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string id;
            object selected;

            switch (gvViewType.SelectedIndex)
            {
                case 0:
                    grItr.Visibility = Windows.UI.Xaml.Visibility.Visible;
                     _sbItrViewON.Begin();
                    break;
                //Grid
                case 1:
                    if (this.FlipView.SelectedItem != null)
                    {
                        id = (this.FlipView.SelectedItem as DataItem).UniqueId;
                        selected = gvDrawing.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
                        if (selected != null)
                            gvDrawing.SelectedItem = selected;
                    }
                    grItr.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    _sbItrViewOFF.Begin();
                    _sbGridView.Pause();
                    TranTransScroll.X = LayoutRoot.ActualWidth / 2;
                    _sbGridView.Begin();
                    ShiftOver(false);
                    break;
                //"Flip"
                case 2:
                    if (gvDrawing.SelectedItem != null)
                    {
                        id = (gvDrawing.SelectedItem as DataItem).UniqueId;
                        selected = this.FlipView.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
                        if (selected != null)
                            this.FlipView.SelectedItem = selected;
                    }
                    grItr.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    _sbItrViewOFF.Begin();
                    _sbFlipView.Pause();
                    TranTransFlip.X = LayoutRoot.ActualWidth / 2;
                    _sbFlipView.Begin();
                    break;
            }

            StretchingPanel.Stretch(false);
        }

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var top = (LayoutRoot.RowDefinitions[1].ActualHeight - 470) / 2;
            ScrollViewer.Padding = new Thickness(0, top, 0, 0);
        }
        #endregion

        #region "Private Method"
        private async void LoadOption()
        {
            Login.MasterPage.Loading(true, this);

            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            _iwpoption.CWPClicked += _iwpoption_CWPClicked;
            _iwpoption.ViewClicked += _iwpoption_ViewClicked;
            _iwpoption.ActiveOpacity = 1;
            this.StretchingPanel.AddPanel(_iwpoption);
            this.StretchingPanel.NavigateTo(_iwpoption.Title);
            this.StretchingPanel.StretchOpened += StretchingPanel_StretchOpened;
            this.DrawingSlider.Minnumber = 1;
            this.DrawingEditor.EnableEdit(Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode ? true : false);

            switch (Login.LoginMode)
            {
                case WinAppLibrary.UI.LogMode.OnMode:
                    await LoadOptionOnMode();
                    break;
                case WinAppLibrary.UI.LogMode.OffMode:
                    break;
            }

            StretchingPanel.Stretch(true);
            Login.MasterPage.Loading(false, this);
        }

        private async Task<bool> LoadOptionOnMode()
        {
            bool retValue = false;

            var common = new Lib.ServiceModel.CommonModel();
            try
            {
                var result = await common.GetCWPByProject_Combo_Mobile(_projectid, _moduleid);
                _iwpoption.BindCwp(result);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Sigma LoadOptionOnMode", "There is a problem loading the drawings - Please try again later", "Error!");
            }

            //Login.MasterPage.SetBottomAppbar(WinAppLibrary.UI.MasterPage.BottomAppBarButton.Download, Visibility.Visible);
            //Login.MasterPage.AppbarClicked += MasterPage_AppbarClicked;

            return retValue;
        }

        private async void Loaddrawing()
        {
            Login.MasterPage.Loading(true, this);
            StretchingPanel.Stretch(false);

            List<DataGroup> source = null;

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    bool login = WinAppLibrary.Utilities.SPDocument.IsLogin ? true :
                                await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint("https://elements.sharepoint.com/RevealDev", Login.UserAccount.SPUser, Login.UserAccount.SPPassword);//await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                    if (login)
                    {
                        var item = _iwpoption.SelectredIWP as RevealCommonSvc.ComboBoxDTO;
                        if (item != null)
                        {
                            if (!WinAppLibrary.Utilities.SPDocument.IsLogin)
                                await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint("https://elements.sharepoint.com/RevealDev", Login.UserAccount.SPUser, Login.UserAccount.SPPassword);//await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                            await _iwpsource.GetIwpDrawingOnMode(item.DataID, _projectid, _moduleid);
                            source = _iwpsource.GetGroupedDocument();
                        }
                        LoadDocument();
                    }
                    else
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "We couldn't sign in Sharepoint Server. Please check your authentication.");
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "IWPGridViewer Loaddrawing", "There is a problem loading the drawings - Please try again later", "Error!");
            }

            this.DefaultViewModel["Drawings"] = source;
            this.FlipView.SelectedItem = this.gvDrawing.SelectedItem = null;
            this.gvViewType.SelectedIndex = 1;
            Login.MasterPage.Loading(false, this);
        }

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbGridView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransScroll, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(ScrollViewer, 1, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransScroll, 1, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransScroll, 1, ANIMATION_SPEED));

            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, -LayoutRoot.ActualWidth / 2, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.FlipView, 0, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransFlip, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransFlip, 0, ANIMATION_SPEED));


            //To FlipView 
            _sbFlipView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransScroll, -LayoutRoot.ActualWidth / 2, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(ScrollViewer, 0, 0, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransScroll, 0, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransScroll, 0, ANIMATION_SPEED));

            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, 0, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.FlipView, 1, 0, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransFlip, 1, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransFlip, 1, ANIMATION_SPEED));


            //To ITRView
            _sbItrViewOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbItrViewOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(detailPanelTrans, Window.Current.Bounds.Width, ANIMATION_SPEED));

            _sbItrViewON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(detailPanelScale, 1, 0));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(detailPanelTrans, 0, ANIMATION_SPEED));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, -LayoutRoot.ActualWidth / 2, ANIMATION_SPEED));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.FlipView, 0, 0, ANIMATION_SPEED));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransFlip, 0, ANIMATION_SPEED));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransFlip, 0, ANIMATION_SPEED));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransScroll, -LayoutRoot.ActualWidth / 2, ANIMATION_SPEED));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(ScrollViewer, 0, 0, ANIMATION_SPEED));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransScroll, 0, ANIMATION_SPEED));
            _sbItrViewON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransScroll, 0, ANIMATION_SPEED));
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

        private string[] ExtractAddress(string url)
        {
            string[] retValue = new string[2];
            try
            {
                int index = url.LastIndexOf('/');
                retValue[0] = url.Substring(0, index + 1);
                retValue[1] = url.Substring(index + 1, url.Length - index - 1);
            }
            catch
            {
                retValue = new string[2];
            }

            return retValue;
        }

        private async void SaveDrawing()
        {
            bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

            if (islogin)
            {
                try
                {
                    var writableImage = await DrawingEditor.RenderDrawing();
                    var stream = await (new WinAppLibrary.Utilities.Helper()).GetJpegStreamFromWriteableBitmap(writableImage);

                    await (new WinAppLibrary.Utilities.SPDocument()).SaveJpegContent(DrawingEditor.FileServer, DrawingEditor.FileName, stream);

                    DrawingEditor.UpdateImage(writableImage);
                    Login.MasterPage.Loading(false, this);
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Successfully Saved", "Completed!");
                }
                catch (Exception e)
                {
                    Login.MasterPage.Loading(false, this);
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveImage", "Failed to save the modified drawing - Please try again later.", "Caution!");
                }
            }
            else
            {
                Login.MasterPage.Loading(false, this);
                WinAppLibrary.Utilities.Helper.SimpleMessage("Your account for sharepoint is not valid. Please login again.", "Caution!");
            }
        }
        #endregion

       

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _documentNo = 0;
            grItr.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbItrViewOFF.Begin();
        }

        private async void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _documentNo--;
                if (_documentNo == 0)
                    btnPrev.IsEnabled = false;
                if (_documentNo < _documentdto.Count)
                    btnNext.IsEnabled = true;
                ChangeDocument();
            });
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _documentNo++;
                if (_documentNo + 1 == _documentdto.Count)
                    btnNext.IsEnabled = false;
                if (_documentNo > 0)
                    btnPrev.IsEnabled = true;
                ChangeDocument();
            });
        }

        private async void ChangeDocument()
        {
            try
            {
                if (_documentdto.Count > 0 && !string.IsNullOrEmpty(_documentdto[_documentNo].LocationURL))
                {
                    //byte[] bytes = await (new WinAppLibrary.Utilities.SPDocument()).GetDocument("https://elements.sharepoint.com/RevealDev/SafetyDocuments/EQUIPMENT-Battery_Care_And_Tips.pdf");
                    byte[] bytes = await (new WinAppLibrary.Utilities.SPDocument()).GetDocument(_documentdto[_documentNo].LocationURL);
                    Stream retValue = null;
                    retValue = new MemoryStream(bytes);
                    PdfViewer.LoadDocument(retValue);
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
