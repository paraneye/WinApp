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
using Windows.UI.Xaml.Media.Imaging;
using Element.Reveal.Meg.Lib;
using System.Text;
using System.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Viewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TurnoverbinderGridViewer : WinAppLibrary.Controls.LayoutAwarePage
    {
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Lib.UI.TurnoverbinderOption _turnoverbinderoption = new Lib.UI.TurnoverbinderOption();
        Lib.IWPDataSource _iwpsource = new Lib.IWPDataSource();

        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbFlipView;
        private int _projectid, _moduleid;

        int backtype = 0;
        ////////쉐어포인트 정보 : 바뀐 것 임시 하드코딩
        //////private string SPURL = "https://elementindustrial.sharepoint.com/sites/clients/DevSigmaDemo/";
        //////private string SPUser = "test@elementindustrial.onmicrosoft.com";
        //////private string SPPassword = "ou812123!";

        public TurnoverbinderGridViewer()
        {
            this.InitializeComponent();           
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            backtype = navigationParameter != null ? 1 : 0;
            // TODO: Create an appropriate data model 
            LoadOption();
            LoadStoryBoardSwitch();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (backtype.Equals(1))
                this.Frame.Navigate(typeof(Discipline.TurnOver.TurnoverSystem));
            else
                this.Frame.Navigate(typeof(MainMenu));
        }

        #region "Drawing Viewer"
        //썸네일 클릭 시
        private async void gvDrawing_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gvViewType.SelectedIndex == 0)
            {
                if (gvDrawing.SelectedItem == e.ClickedItem)
                {
                    DataItem item = e.ClickedItem as DataItem;

                    //Login.MasterPage.ShowTopBanner = false;
                    //string[] addrs = ExtractAddress(item.ImagePath);
                    /*
                    if (!await DrawingEditor.LoadDrawingWithSharepoint(addrs[0], addrs[1], UriKind.Absolute))
                        await DrawingEditor.LoadDefault();

                    DrawingEditor.ShowWithEdit();*/
                }
                else
                {
                    gvDrawing.SelectedItem = e.ClickedItem;
                    StretchingPanel.Stretch(true);
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

        //썸네일 클릭 -> 스크린뷰 > 이미지 클릭 시
        private async void FlipViewItem_Clicked(object sender, object e)
        {
            //var item = e as DataItem; 

           // ImgDrawing.Source =   ((RevealCommonSvc.ComboBoxDTO)FlipView.SelectedItem).ExtraValue3;
           // Uri aa = new Uri(((RevealCommonSvc.ComboBoxDTO)FlipView.SelectedItem).ExtraValue3);
      //      Uri aa = new Uri(((RevealCommonSvc.ComboBoxDTO)e).ExtraValue3);
        //    WriteableBitmap source = await (new WinAppLibrary.Utilities.Helper()).GetWriteableBitmapFromUri(aa);
      //     

           //ratio = source.PixelWidth / LayoutRoot.ActualWidth; //무조건 현재의 Layout의 해상도에서 그리기를 시작한다.
                //((RevealCommonSvc.ComboBoxDTO)FlipView.SelectedItem).ExtraValue3;
           
            /*
            if (item != null)
            {
                //Login.MasterPage.ShowTopBanner = false;
                string[] addrs = ExtractAddress(item.ImagePath);
                /*
                if (!await DrawingEditor.LoadDrawingWithSharepoint(addrs[0], addrs[1], UriKind.Absolute))
                    await DrawingEditor.LoadDefault();

                DrawingEditor.ShowWithEdit();*/
            //}

            //    //Login.MasterPage.ShowTopBanner = false;
            
            ////쉐어포인트 이미지 추가 확인
            //////string[] addrs = ExtractAddress(((RevealCommonSvc.ComboBoxDTO)e).ExtraValue3);

            //////DrawingEditor.LoadDrawing(addrs[0], addrs[1], UriKind.Absolute);
            //////DrawingEditor.Show();
        }

        private void DrawingEditor_Closed(object sender, object e)
        {
            //Login.MasterPage.ShowTopBanner = true;
        }
        #endregion

        #region "Left Panel"
        private async void _iwpoption_CWPClicked(object sender, object e)
        {
            //Login.MasterPage.Loading(true, this);

            //try
            //{
            //    var item = e as RevealCommonSvc.ComboBoxDTO;
            //    var result = await (new Lib.ServiceModel.CommonModel()).GetFIWPByCwp_Combo(item.DataID, _projectid, _moduleid);
            //    result = result.Where(x => string.IsNullOrEmpty(x.ExtraValue3)).ToList();
            //    _turnoverbinderoption.BindList(result);

            //}
            //catch (Exception ex)
            //{
            //    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "_iwpoption_CWPClicked", "There was an error load IWP drawing. Pleae contact administrator", "Error!");
            //}

            //Login.MasterPage.Loading(false, this);
        }

        //드로잉 리스트 보기
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

        //보기 방식 변경(크리드, 스크린 FlipView)
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
                    //    id = (this.FlipView.SelectedItem as DataItem).UniqueId;
                     //   selected = gvDrawing.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
                        id = ((RevealCommonSvc.ComboBoxDTO)gvDrawing.SelectedItem).DataID.ToString();
                        selected = this.gvDrawing.Items.Where(x => ((RevealCommonSvc.ComboBoxDTO)x).DataID.ToString() == id).FirstOrDefault();
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
                        /*
                        id = (gvDrawing.SelectedItem as DataItem).UniqueId;
                        selected = this.FlipView.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
                        if (selected != null)
                            this.FlipView.SelectedItem = selected;*/
                        id = ((RevealCommonSvc.ComboBoxDTO)gvDrawing.SelectedItem).DataID.ToString();
                        selected = this.FlipView.Items.Where(x => ((RevealCommonSvc.ComboBoxDTO)x).DataID.ToString() == id).FirstOrDefault();
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
            _moduleid = Login.UserAccount.CurModuleID;

            _turnoverbinderoption.CWPClicked += _iwpoption_CWPClicked;
            _turnoverbinderoption.ViewClicked += _iwpoption_ViewClicked;
            _turnoverbinderoption.ActiveOpacity = 1;
            this.StretchingPanel.AddPanel(_turnoverbinderoption);
            this.StretchingPanel.NavigateTo(_turnoverbinderoption.Title);
            this.StretchingPanel.StretchOpened += StretchingPanel_StretchOpened;
       //     this.DrawingEditor.EnableEdit(Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode ? true : false);

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

        //턴오버 바인더 목차 바인딩
        private async Task<bool> LoadOptionOnMode()
        {
            bool retValue = false;

            var common = new Lib.ServiceModel.CommonModel();
            try
            {
                var result = await common.GetTurnoverPackageByBinderPage_Combo(_projectid, _moduleid);
                    //await common.GetCWPByProject_Combo_Mobile(_projectid, _moduleid);
                _turnoverbinderoption.BindList(result);

                //var result = await common.GetCWPByProject_Combo_Mobile(_projectid, _moduleid);
                //_turnoverbinderoption.BindCwp(result);
                //retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Sigma LoadOptionOnMode", "There was an error load drawing. Pleae contact administrator", "Error!");
            }

            //Login.MasterPage.SetBottomAppbar(WinAppLibrary.UI.MasterPage.BottomAppBarButton.Download, Visibility.Visible);
            //Login.MasterPage.AppbarClicked += MasterPage_AppbarClicked;

            return retValue;
        }

        //드로잉 바인딩
        private async void Loaddrawing()
        {
            Login.MasterPage.Loading(true, this);
            StretchingPanel.Stretch(false);

            List<DataGroup> source = null;
            List<RevealProjectSvc.DocumentDTO> _documentDTO = new List<RevealProjectSvc.DocumentDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    bool login = WinAppLibrary.Utilities.SPDocument.IsLogin ? true :
                                await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                    if (login)
                    {
                        
                        var item = _turnoverbinderoption.SelectredTurnover as RevealCommonSvc.ComboBoxDTO;
                        if (item != null)
                        {
                            
                            if (!WinAppLibrary.Utilities.SPDocument.IsLogin)
                                await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);
                                //await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(SPURL, SPUser, SPPassword);

                            _documentDTO = await (new Lib.ServiceModel.ProjectModel()).GetDocumentByTurnoverPackage(LOOKUPTYPE.TurnoverPackage, item.DataID, _projectid, 0);

                            List<RevealCommonSvc.ComboBoxDTO> thumbnailviewrList = new List<RevealCommonSvc.ComboBoxDTO>();
                            
                            for (int i = 0; i < _documentDTO.Count; i++)
                            {
                                RevealCommonSvc.ComboBoxDTO thumbnailviewr = new RevealCommonSvc.ComboBoxDTO();
                                string Url = _documentDTO[i].LocationURL;
                                string[] addrs = ExtractAddress(Url);

                                thumbnailviewr.DataID = _documentDTO[i].DocumentID;
                                thumbnailviewr.ExtraValue1 = addrs[0];
                                thumbnailviewr.ExtraValue2 = addrs[1];
                                thumbnailviewr.ExtraValue3 = Url;
                                thumbnailviewrList.Add(thumbnailviewr);
                            }

                            gvDrawing.ItemsSource = thumbnailviewrList;
                            FlipView.ItemsSource = thumbnailviewrList;

                            ////////////테스트
                            ////////List<RevealCommonSvc.ComboBoxDTO> thumbnailviewr = new List<RevealCommonSvc.ComboBoxDTO>();
                            ////////thumbnailviewr = await (new Lib.ServiceModel.CommonModel()).GetDrawingByCWP(1, 1, "", _projectid, _moduleid);
                            ////////thumbnailviewr[0].ExtraValue1 = "http://reveal.elementindustrial.com/Element.Reveal.Server.WS/IFC_Images/";
                            ////////thumbnailviewr[0].ExtraValue2 = "PL21-P-8391-1%20REV%201B.jpg";
                            ////////thumbnailviewr[0].ExtraValue3 = "http://reveal.elementindustrial.com/Element.Reveal.Server.WS/IFC_Images/PL21-P-8391-1%20REV%201B.jpg";

                            ////////thumbnailviewr.Add(thumbnailviewr[0]);
                            ////////for (int i = 0; i < 30; i++)
                            ////////{
                            ////////    thumbnailviewr.Add(thumbnailviewr[0]);
                            ////////}
                            ////////gvDrawing.ItemsSource = thumbnailviewr;
                            ////////FlipView.ItemsSource = thumbnailviewr;
                              
                        }
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
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "IWPGridViewer Loaddrawing", "There was an error load drawing. Pleae contact administrator", "Error!");
            }

         //   this.DefaultViewModel["Drawings"] = source;
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
        #endregion


        #region "Drawing Supporter"
        private void DrawingEditor_EditRequested(object sender, object e)
        {
          //  ButtonDialog.Show(this, "Select edit mode", new string[] { "RFI", "Markup" });
        }

        private void DrawingEditor_SaveRequested(object sender, object e)
        {
         //   Login.MasterPage.Loading(true, this);
          //  SaveDrawing();
        }

        private void DrawingEditor_SaveCompleted(object sender, object e)
        {
            /*
            Login.MasterPage.Loading(false, this);
            WinAppLibrary.Utilities.Helper.SimpleMessage(e.ToString(), sender.ToString());

            if (this.DrawingEditor.IsNew)
            {
                this.DrawingEditor.Hide();
                //Login.MasterPage.ShowTopBanner = true;
            }*/
        }

        
        #endregion
     
    }
}
