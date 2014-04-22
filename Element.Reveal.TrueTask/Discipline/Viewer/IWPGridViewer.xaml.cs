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
using System.Net.Http;
using Element.Reveal.TrueTask.Lib.Common;
using Windows.Storage;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Viewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IWPGridViewer : WinAppLibrary.Controls.LayoutAwarePage
    {
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Lib.UI.IWPOption _iwpoption = new Lib.UI.IWPOption();
        Lib.IWPDataSource _iwpsource = new Lib.IWPDataSource();

        //itr
        List<DataLibrary.DocumentDTO> _documentdto = new List<DataLibrary.DocumentDTO>();
        private int _documentNo;

        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbFlipView, _sbItrViewON, _sbItrViewOFF;
        private int _projectid; private string _disciplineCode;

        protected string BackMenuType = ""; //다른 곳에서 넘어온 메뉴일 때 구분

        public IWPGridViewer()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            LoadOption();

            //AssembleIWP에서 넘어온 경우(일단 APPROVER 코드 사용)
            if (navigationParameter != null && navigationParameter.ToString() == DataLibrary.Utilities.AssembleStep.APPROVER)
            {
                BackMenuType = DataLibrary.Utilities.AssembleStep.APPROVER;
                StretchingPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;  //왼쪽 메뉴 감추기
                Loaddrawing();  //데이터 바로 바인딩
            }
            
            LoadStoryBoardSwitch();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //다른 곳에서 넘어온 메뉴일 때 백버튼 링크 구분
            if(BackMenuType == DataLibrary.Utilities.AssembleStep.APPROVER)
                this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleIWP));
            else
                this.Frame.Navigate(typeof(MainMenu));
        }

        #region "Drawing Viewer"
        private async void gvDrawing_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gvViewType.SelectedIndex == 0)
            {
                if (gvDrawing.SelectedItem == e.ClickedItem)
                {
                    DataItem item = e.ClickedItem as DataItem;

                    //Login.MasterPage.ShowTopBanner = false;
                    string[] addrs = ExtractAddress(item.ImagePath);
                    if (item.ImagePath.ToLower().Contains(".pdf") || item.ImagePath.ToLower().Contains(".ozd") || item.ImagePath.ToLower().Contains(".ozr"))
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Can't Edit", "Alert");
                    else
                    {
                        Login.MasterPage.Loading(true, this);
                        DrawingEditor.EnableEdit(false);
                        if (!await DrawingEditor.LoadDrawingWithServer(item.ImagePath, UriKind.Absolute))
                            await DrawingEditor.LoadDefault();

                        DrawingEditor.ViewMode();
                        Login.MasterPage.Loading(false, this);
                    }
                }
                else
                {
                    gvDrawing.SelectedItem = e.ClickedItem;
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
                //Login.MasterPage.ShowTopBanner = false;
                if (item.ImagePath.ToLower().Contains(".pdf") || item.ImagePath.ToLower().Contains(".ozd") || item.ImagePath.ToLower().Contains(".ozr"))
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Can't Edit", "Alert");
                else
                {
                    string[] addrs = ExtractAddress(item.ImagePath);

                    DrawingEditor.EnableEdit(false);
                    if (!await DrawingEditor.LoadDrawingWithServer(item.ImagePath, UriKind.Absolute))
                        await DrawingEditor.LoadDefault();

                    DrawingEditor.ViewMode();
                }
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
                var item = e as DataLibrary.ComboBoxDTO;
                var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByCwp_Combo(item.DataID, _projectid, _disciplineCode);
                if (result == null)
                {
                    Login.MasterPage.Loading(false, this);
                    return;
                }

                result = result.Where(x => string.IsNullOrEmpty(x.ExtraValue3)).ToList();
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
                        id = (this.FlipView.SelectedItem as DataItem).UniqueId;
                        selected = gvDrawing.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
                        if (selected != null)
                            gvDrawing.SelectedItem = selected;
                    }

                    _sbGridView.Pause();
                    TranTransScroll.X = LayoutRoot.ActualWidth / 2;
                    _sbGridView.Begin();
                    ShiftOver(false);
                    break;
                //"Flip"
                case 1:
                    if (gvDrawing.SelectedItem != null)
                    {
                        
                        id = (gvDrawing.SelectedItem as DataItem).UniqueId;
                        selected = this.FlipView.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
                        if (selected != null)
                            this.FlipView.SelectedItem = selected;
                    }

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
            Login.MasterPage.HideUserStatus();

            Lib.ProjectModuleSource projectmodule = new Lib.ProjectModuleSource();
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;

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
                var result = await common.GetCWPByProject_Combo_Mobile(_projectid, _disciplineCode, Login.UserAccount.LoginName);
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
                    //AssembleIWP에서 넘어온 경우(일단 APPROVER 코드 사용) : 선택된 iwpid로 바로 바인딩
                    if (BackMenuType == DataLibrary.Utilities.AssembleStep.APPROVER && Lib.IWPDataSource.selectedIWP > 0)
                    {
                        await _iwpsource.GetIwpDrawingOnMode(Lib.IWPDataSource.selectedIWP, _projectid, _disciplineCode, Lib.MainMenuList.IWPViewer);
                        source = _iwpsource.GetGroupedDocument("Installation Work Package");
                    }
                    else
                    {
                        var item = _iwpoption.SelectredIWP as DataLibrary.ComboBoxDTO;
                        if (item != null)
                        {
                            await _iwpsource.GetIwpDrawingOnMode(item.DataID, _projectid, _disciplineCode, Lib.MainMenuList.IWPViewer);
                            source = _iwpsource.GetGroupedDocument("Installation Work Package");
                        }
                    }
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
            this.gvViewType.SelectedIndex = 0;
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
            try
            {
                var writableImage = await DrawingEditor.RenderDrawing();
                var stream = await (new WinAppLibrary.Utilities.Helper()).GetJpegStreamFromWriteableBitmap(writableImage);

                // file upload 방식으로 수정해야 함
                //await (new WinAppLibrary.Utilities.FileDocument()).SaveJpegContent(DrawingEditor.FileUrl, DrawingEditor.f, stream);

                DrawingEditor.UpdateImage(writableImage);
                Login.MasterPage.Loading(false, this);
                WinAppLibrary.Utilities.Helper.SimpleMessage("Successfully Saved", "Completed!");
            }
            catch (Exception e)
            {
                Login.MasterPage.Loading(false, this);
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveImage", "Failed to save the modified drawing - Please try again later.", "Saving Error");
            }
        }
        #endregion

    }
}
