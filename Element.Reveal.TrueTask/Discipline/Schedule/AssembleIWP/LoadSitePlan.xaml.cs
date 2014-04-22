using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using WinAppLibrary.Utilities;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using System.Net.Http;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadSitePlan : WinAppLibrary.Controls.LayoutAwarePage
    {
        private List<DataLibrary.DocumentDTO> _orgCoverPage = new List<DataLibrary.DocumentDTO>();
        private List<DataLibrary.DocumentDTO> _trgIWPCoverPage = new List<DataLibrary.DocumentDTO>();

        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbSortON, _sbSortOFF;

        private int _projectid, _fiwpid;
        
        public LoadSitePlan()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _fiwpid = Lib.IWPDataSource.selectedIWP;

            LoadStoryBoardSwitch();
            LoadTemplate();

            Lib.WizardDataSource.SetTargetMenu(DataLibrary.Utilities.AssembleStep.COVER, Lib.CommonDataSource.selPackageTypeLUID, true);

            this.ButtonBar.CurrentMenu = DataLibrary.Utilities.AssembleStep.COVER;
            this.ButtonBar.Load();
        }

        private void LoadStoryBoardSwitch()
        {
            _sbSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 0, ANIMATION_SPEED));


            _sbSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 1, ANIMATION_SPEED));

        }

        private async void LoadTemplate()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();


            List<DataLibrary.CwpDTO> source = new List<DataLibrary.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    // 전체 Cover Page 리스트
                    _orgCoverPage = await (new Lib.ServiceModel.ProjectModel()).GetUploadedFileInfoByProjectFileType(_projectid, DataLibrary.Utilities.FileType.COVER, DataLibrary.Utilities.FileCategory.DOCUMENT);
                    lvCoverfile.ItemsSource = _orgCoverPage;
                    
                    // 선택된 Cover Page
                    List<DataLibrary.DocumentDTO> result;
                    result = await (new Lib.ServiceModel.ProjectModel()).GetIwpDocumentByIwpProjectFileType(_fiwpid, _projectid, DataLibrary.Utilities.FileType.COVER, "N", DataLibrary.Utilities.FileCategory.DOCUMENT, "0");
                    if (result != null)
                    {
                        _trgIWPCoverPage = result;
                        lvFiwpCoverfile.ItemsSource = _trgIWPCoverPage;
                        string url = _trgIWPCoverPage[0].LocationURL;    

                        //이미지 바인딩 : 경로 확인
                        try
                        {
                            imgCoverPage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(url));
                        }
                        catch (Exception)
                        {
                            imgCoverPage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Default.png"));
                        }

                        //전체 리스트 중 선택값 세팅
                        var selected = _orgCoverPage.Where(x => (x as DataLibrary.DocumentDTO).SPCollectionID == _trgIWPCoverPage[0].SPCollectionID).FirstOrDefault();
                        lvCoverfile.SelectedItem = selected;
                        
                        txtNoImg.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        imgCoverPage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else
                    {
                        txtNoImg.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        imgCoverPage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                }
                else
                {
                    //오프라인모드
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Load Cover Page", "There is a problem loading the cover page - Please try again later", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }


        #region "Event Handler"


        //리스트 선택 시 해당 이미지 세팅
        private void lvCoverfile_ItemClick(object sender, ItemClickEventArgs e)
        {
            lvCoverfile.SelectedItem = (DataLibrary.DocumentDTO)e.ClickedItem;
            AddItem();
        }

        //버튼: 공통 uc 사용
        private void Button_Clicked(object sender, object e)
        {
            string tag = e != null ? e.ToString() : string.Empty;

            switch (tag)
            {
                case "Save":
                    SaveDocument();
                    break;
                case "Next":
                    SaveDocument();
                    break;
            }
        }

        private async void SaveDocument()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                //if (lvCoverfile.SelectedItems.Count > 0)
                //{
                    //if (_trgIWPCoverPage == null)
                    //{
                    //    Login.MasterPage.Loading(false, this);
                    //    return;

                    //}

                    List<DataLibrary.DocumentDTO> _savedto = new List<DataLibrary.DocumentDTO>();
                    foreach (DataLibrary.DocumentDTO _dto in _trgIWPCoverPage)
                    {
                        _savedto.Add(_dto);
                    }

                    if (Lib.IWPDataSource.iwplist == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;
                    }

                    List<DataLibrary.FiwpDTO> fiwpdto = new List<DataLibrary.FiwpDTO>();
                    fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == _fiwpid).ToList();

                    fiwpdto[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;

                    //현재 단계 저장 (첫 단계이므로 null 체크 추가)
                    if (string.IsNullOrEmpty(fiwpdto[0].DocEstablishedLUID) || !fiwpdto[0].DocEstablishedLUID.Equals(DataLibrary.Utilities.AssembleStep.APPROVER))
                        fiwpdto[0].DocEstablishedLUID = DataLibrary.Utilities.AssembleStep.COVER;

                    await (new Lib.ServiceModel.ProjectModel()).SaveDocumentForAssembleIWP(fiwpdto, _savedto, DataLibrary.Utilities.AssembleStep.COVER, Login.UserAccount.PersonnelId);

                    Lib.WizardDataSource.SetTargetMenu(DataLibrary.Utilities.AssembleStep.COVER, Lib.CommonDataSource.selPackageTypeLUID, true);

                    //다음 메뉴 IWPSummary, AssembleReport 공통 파일에 파라미터로 구분됨
                    if (Lib.WizardDataSource.NextMenu != null)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SUMMARY);
                //}
                //else
                //{
                //    WinAppLibrary.Utilities.Helper.SimpleMessage("Please select cover Page", "Caution!");
                //}
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Save Cover Page", "There is a problem saving the cover page - Please try again later", "Error");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransConsumableSort.ScaleY > 0)
            {
                _sbSortOFF.Begin();
                btnFilter.Content = "∨";
            }
            else
            {
                _sbSortON.Begin();
                btnFilter.Content = "∧";
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (_orgCoverPage != null && _trgIWPCoverPage != null)
                lvCoverfile.ItemsSource = _orgCoverPage.Where(x => x.Description.ToUpper().Contains((txtSearch.Text).ToUpper()));
        }

        #endregion

        #region "Private Method"

        private async void AddItem()
        {
            //if (lvCoverfile.SelectedItem != null)
            //{                
            Login.MasterPage.Loading(true, this);

            try
            {

                if (lvCoverfile.SelectedItems.Count > 0)
                {
                    if (_trgIWPCoverPage == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;

                    }

                    foreach (DataLibrary.DocumentDTO dto in lvCoverfile.SelectedItems)
                    {
                        //수정
                        if (_trgIWPCoverPage.Where(x => x.SPCollectionID == dto.SPCollectionID).Count() > 0)
                        {
                            //기존 _trgIWPCoverPage를 update 처리
                            _trgIWPCoverPage[0].FIWPID = _fiwpid;
                            _trgIWPCoverPage[0].SPCollectionID = dto.SPCollectionID;
                            _trgIWPCoverPage[0].FileStoreId = dto.FileStoreId;
                            _trgIWPCoverPage[0].LocationURL = dto.LocationURL;
                            _trgIWPCoverPage[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                            _trgIWPCoverPage[0].UpdatedBy = Login.UserAccount.UserName;
                            _trgIWPCoverPage[0].UpdatedDate = DateTime.Now;
                            _trgIWPCoverPage[0].IsDisplayable = "N";
                        }
                        //신규
                        else
                        {
                            if (_trgIWPCoverPage.Count > 0)
                                _trgIWPCoverPage.Clear();

                            DataLibrary.DocumentDTO newdto = new DataLibrary.DocumentDTO();
                            newdto = dto;
                            newdto.FIWPID = _fiwpid;
                            newdto.LocationURL = dto.LocationURL;
                            newdto.FileStoreId = dto.FileStoreId;
                            newdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                            newdto.UpdatedBy = Login.UserAccount.UserName;
                            newdto.UpdatedDate = DateTime.Now;
                            newdto.IsDisplayable = "N";
                            _trgIWPCoverPage.Add(newdto);
                        }
                    }

                    lvFiwpCoverfile.ItemsSource = null;
                    lvFiwpCoverfile.ItemsSource = _trgIWPCoverPage;
                    
                    //이미지 바인딩
                    try
                    {
                        string Url = (lvFiwpCoverfile.Items[0] as DataLibrary.DocumentDTO).LocationURL.ToString();
                        Uri pathCheckUri = new Uri(Url);
                        var client = new HttpClient();
                        //파일 있는지 확인
                        string page = await client.GetStringAsync(pathCheckUri);

                        //선택 이미지 세팅
                        imgCoverPage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(Url));

                    }
                    catch (Exception ex)
                    {
                        imgCoverPage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Default.png"));
                    }
                }
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Load cover page", "There is a problem adding the selected item - Please try again later", "Error!");
                imgCoverPage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Default.png"));
            }

            txtNoImg.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            imgCoverPage.Visibility = Windows.UI.Xaml.Visibility.Visible;

            Login.MasterPage.Loading(false, this);
            //}

        }

        #endregion

    }
}
