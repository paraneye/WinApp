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
using Windows.UI.Popups;
using WinAppLibrary.Utilities;
using System.Net.Http;
using oz.api;
using Windows.Storage;
using Element.Reveal.TrueTask.Lib.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssembleDocument : WinAppLibrary.Controls.LayoutAwarePage
    {
        OZReportViewer _viewer = null;
        private List<DataLibrary.DocumentDTO> _orgITR = new List<DataLibrary.DocumentDTO>();
        private List<DataLibrary.DocumentDTO> _trgIWPITR = new List<DataLibrary.DocumentDTO>();
        private int _projectid, _fiwpid;
        public string FileType, AssembleStep;

        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbSortON, _sbSortOFF, _sbItrViewON, _sbItrViewOFF;

        public AssembleDocument()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {

            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _fiwpid = Lib.IWPDataSource.selectedIWP;

            //Report Type 구분
            //새로 타입 추가될 때 1) 각 메뉴의 BackButton / Save/Next 이동메뉴 확인 필요시  파라미터 추가 2)SelectIWP에서도 타입 구분 추가
            AssembleStep = navigationParameter.ToString();
            if (AssembleStep == DataLibrary.Utilities.AssembleStep.SAFETY_FORM)
            {
                FileType = DataLibrary.Utilities.FileType.SAFETY_FORM;
                tbpageTitle.Text = "Safety";
                txtOrgTitle.Text = "Select Safety Forms to load";
                txtTrgTitle.Text = "Selected Safety Forms";
            }
            else if (AssembleStep == DataLibrary.Utilities.AssembleStep.ITR)
            {
                FileType = DataLibrary.Utilities.FileType.ITR;
                tbpageTitle.Text = "ITR";
                txtOrgTitle.Text = "Select ITR Forms to load";
                txtTrgTitle.Text = "Selected ITR Forms";
            }
            else if (AssembleStep == DataLibrary.Utilities.AssembleStep.SPEC)
            {
                FileType = DataLibrary.Utilities.FileType.SPEC;
                tbpageTitle.Text = "Specs & Details";
                txtOrgTitle.Text = "Select Specs & Details to load";
                txtTrgTitle.Text = "Selected Specs & Details";
            }
            else if (AssembleStep == DataLibrary.Utilities.AssembleStep.MOC)
            {
                FileType = DataLibrary.Utilities.FileType.MOC;
                tbpageTitle.Text = "MOC";
                txtOrgTitle.Text = "Select MOC to load";
                txtTrgTitle.Text = "Selected MOC";
            }

            LoadStoryBoardSwitch();
            LoadTemplate();

            Lib.WizardDataSource.SetTargetMenu(AssembleStep, Lib.CommonDataSource.selPackageTypeLUID, true);
            this.ButtonBar.CurrentMenu = AssembleStep;
            this.ButtonBar.Load();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //공통 파일 - AssembleDocument나 AssembleReport로 이동 시 파라미터 추가
            if (Lib.WizardDataSource.PreviousMenu != null)
            {
                //SafetyChecklist <- Safety Form
                if(AssembleStep == DataLibrary.Utilities.AssembleStep.SAFETY_FORM)
                    this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu, DataLibrary.Utilities.AssembleStep.SAFETY_CHECK);
                //Safety  Form <- ITR
                else if(AssembleStep == DataLibrary.Utilities.AssembleStep.ITR)
                    this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu, DataLibrary.Utilities.AssembleStep.SAFETY_FORM);
                //ScaffoldChecklist <- Spes & Details
                else if (AssembleStep == DataLibrary.Utilities.AssembleStep.SPEC)
                    this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu, DataLibrary.Utilities.AssembleStep.SCAFFOLD_CHECK);
                //Spec <- MOC
                else if (AssembleStep == DataLibrary.Utilities.AssembleStep.MOC)
                    this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu, DataLibrary.Utilities.AssembleStep.SPEC);
                else
                    this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
            }
        }

        //버튼: 공통 uc 사용
        private void Button_Clicked(object sender, object e)
        {
            string tag = e != null ? e.ToString() : string.Empty;

            switch (tag)
            {
                case "Add":
                    AddItem();
                    break;
                case "Remove":
                    RemoveItem();
                    break;
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
                //int saveCount = 0;

                List<DataLibrary.DocumentDTO> _result = new List<DataLibrary.DocumentDTO>();
                List<DataLibrary.DocumentDTO> _savedto = new List<DataLibrary.DocumentDTO>();

                if (_trgIWPITR == null)
                {
                    Login.MasterPage.Loading(false, this);
                    return;
                }

                foreach (DataLibrary.DocumentDTO _dto in _trgIWPITR)
                {
                    _savedto.Add(_dto);
                    //if (_dto.DTOStatus != (int)DataLibrary.Utilities.RowStatus.None)
                    //    saveCount++;
                }

                //New, Delete 있을 경우에만
                //if (saveCount > 0)
                //{
                    List<DataLibrary.FiwpDTO> fiwpdto = new List<DataLibrary.FiwpDTO>();

                    if (Lib.IWPDataSource.iwplist == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;
                    }
                    fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == _fiwpid).ToList();

                    fiwpdto[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;

                    //현재단계 저장
                    if (!fiwpdto[0].DocEstablishedLUID.Equals(DataLibrary.Utilities.AssembleStep.APPROVER))
                        fiwpdto[0].DocEstablishedLUID = AssembleStep;

                    _result = await (new Lib.ServiceModel.ProjectModel()).SaveDocumentForAssembleIWP(fiwpdto, _savedto, AssembleStep, Login.UserAccount.PersonnelId);

                    _trgIWPITR = _result;
                    lvFiwpITRForms.ItemsSource = _trgIWPITR;

                    if (_orgITR == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;
                    }
                    lvITRForms.ItemsSource = null;
                    lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR where b.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete select b.SPCollectionID).Contains(x.SPCollectionID) select x);
                //}


                ////데모 전에 Sign off가 완료되지 않을 경우 대비
                ////MOC 면 위자드 모드 완료 상태로
                //if(AssembleStep == DataLibrary.Utilities.AssembleStep.MOC)
                //    Lib.IWPDataSource.isWizard = fiwpdto[0].DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.MOC ? false : true;

                Lib.WizardDataSource.SetTargetMenu(AssembleStep, Lib.CommonDataSource.selPackageTypeLUID, true);

                //공통 파일 - AssembleDocument나 AssembleReport로 이동 시 파라미터 추가
                if (Lib.WizardDataSource.NextMenu != null)
                {
                    //Safety Form -> ITR
                    if (AssembleStep == DataLibrary.Utilities.AssembleStep.SAFETY_FORM)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.ITR);
                    //Spec -> MOC
                    else if (AssembleStep == DataLibrary.Utilities.AssembleStep.SPEC)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.MOC);
                    else
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
                }
            }

            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Save " + tbpageTitle.Text, "There is a problem saving the selected item - Please try again later", "Error");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (_orgITR != null && _trgIWPITR != null)
                lvITRForms.ItemsSource = _orgITR.Where(x => x.Description.ToUpper().Contains((txtSearch.Text).ToUpper()) && !(from b in _trgIWPITR where b.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete select b.SPCollectionID).Contains(x.SPCollectionID));
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransConsumableSort.ScaleY > 0)
            {
                _sbSortOFF.Begin();
                btnSort.Content = "∨";
            }
            else
            {
                _sbSortON.Begin();
                btnSort.Content = "∧";
            }
        }

        //왼쪽 -> 오른쪽
        private void lvITRForms_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            lvITRForms.SelectedItem = e.Items;
        }

        private void lvFiwpITRForms_Drop(object sender, DragEventArgs e)
        {
            AddItem();
        }

        //오른쪽 -> 왼쪽
        private void lvFiwpITRForms_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            lvFiwpITRForms.SelectedItem = e.Items;
        }

        private void lvITRForms_Drop(object sender, DragEventArgs e)
        {
            RemoveItem();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //_documentNo = 0;
            grItr.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Login.MasterPage.ShowUserStatus();
            //_sbItrViewOFF.Begin();
        }

        //홀딩 이벤트 : 해당 문서 보여주기
        private async void lvITRForms_Holding(object sender, HoldingRoutedEventArgs e)
        {
            Login.MasterPage.HideUserStatus();
            
            grItr.Visibility = Windows.UI.Xaml.Visibility.Visible;
            imgITR.UriSource = new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Default_loading.png");
            
            var dto = (sender as ListView).Items[0] as DataLibrary.DocumentDTO;
            string Url = dto.LocationURL;

            if (!string.IsNullOrEmpty(Url))
            {
                try
                {
                    Uri pathCheckUri = new Uri(Url);
                    var client = new HttpClient();
                    //파일 있는지 확인
                    string page = await client.GetStringAsync(pathCheckUri);

                    if (Url.ToLower().Contains(".pdf"))
                    {
                        imgViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        var req = new HttpClient();
                        var message = new HttpRequestMessage(HttpMethod.Get, Url);
                        byte[] response;
                        message.Headers.Add("Accept", "");
                        using (var res = await req.SendAsync(message))
                        {
                            response = await res.Content.ReadAsByteArrayAsync();
                        }
                        System.Type type = response.GetType();
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            Stream retValue = null;
                            retValue = new MemoryStream(response);
                            PdfViewer.LoadDocument(retValue);
                        });
                        PdfViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        imgViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else if (Url.ToLower().Contains(".ozd"))
                    {
                        String str = "connection.openfile=" + Url + "\nviewer.usetoolbar=false\n";
                        RunReport(str);
                        imgViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        PdfViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else if (Url.ToLower().Contains(".jpg") || Url.ToLower().Contains(".jpeg") || Url.ToLower().Contains(".png"))
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            PdfViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            Uri itrUrl = new Uri(Url);
                            imgITR.UriSource = itrUrl;
                            imgViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        });

                    }
                    else
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            PdfViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            Uri itrUrl = new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Default.png");
                            imgITR.UriSource = itrUrl;
                            imgViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        });
                    }

                }
                catch (Exception ex)
                {
                    PdfViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Uri itrUrl = new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Default.png");
                    imgITR.UriSource = itrUrl;
                    imgViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
        }
        
        #endregion

        #region "Private Method"

        private async void LoadTemplate()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();


            List<DataLibrary.CwpDTO> source = new List<DataLibrary.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    List<DataLibrary.DocumentDTO> result;
                    result = await (new Lib.ServiceModel.ProjectModel()).GetUploadedFileInfoByProjectFileType(_projectid, FileType, DataLibrary.Utilities.FileCategory.DOCUMENT);
                    if (result != null)
                        _orgITR = result;
                    result = await (new Lib.ServiceModel.ProjectModel()).GetIwpDocumentByIwpProjectFileType(_fiwpid, _projectid, FileType, "N", DataLibrary.Utilities.FileCategory.DOCUMENT, "0");
                    if (result != null)
                        _trgIWPITR = result;

                    if (_orgITR == null || _trgIWPITR == null)
                        return;

                    //전체 리스트에서 fiwp에 선택된 값 제외
                    lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR select b.SPCollectionID).Contains(x.SPCollectionID) select x);
                    lvFiwpITRForms.ItemsSource = _trgIWPITR;
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Load" + tbpageTitle.Text, "There is a problem loading the " + tbpageTitle.Text + " - Please try again later", "Loading Error");
            }

            Login.MasterPage.Loading(false, this);
        }

        //Search할 때 애니메이션 처리
        private void LoadStoryBoardSwitch()
        {
            _sbSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 0, ANIMATION_SPEED));


            _sbSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 1, ANIMATION_SPEED));
        }

        private void AddItem()
        {
            if (lvITRForms.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                try
                {
                    if (_trgIWPITR == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;
                    }

                    if (lvITRForms.SelectedItems.Count > 0)
                    {
                        foreach (DataLibrary.DocumentDTO dto in lvITRForms.SelectedItems)
                        {
                            if (_trgIWPITR.Where(x => x.SPCollectionID == dto.SPCollectionID).Count() > 0)
                                _trgIWPITR.Where(x => x.SPCollectionID == dto.SPCollectionID).FirstOrDefault().DTOStatus = (int)DataLibrary.Utilities.RowStatus.None;
                            else
                            {
                                DataLibrary.DocumentDTO newdto = new DataLibrary.DocumentDTO();
                                newdto = dto;
                                newdto.FIWPID = _fiwpid;
                                newdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                                newdto.UpdatedBy = Login.UserAccount.UserName;
                                newdto.UpdatedDate = DateTime.Now;
                                newdto.IsDisplayable = "N";
                                _trgIWPITR.Add(newdto);
                            }
                        }

                        lvFiwpITRForms.ItemsSource = null;
                        lvFiwpITRForms.ItemsSource = _trgIWPITR.Where(x => x.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete);
                    }


                    if (_orgITR == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;
                    }
                    lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR where b.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete select b.SPCollectionID).Contains(x.SPCollectionID) select x);

                }
                catch (Exception ex)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Add " + tbpageTitle.Text, "There is a problem adding the selected item - Please try again later", "Error");
                }
                Login.MasterPage.Loading(false, this);
            }

        }

        private void RemoveItem()
        {
            if (lvFiwpITRForms.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                try
                {
                    if (_trgIWPITR == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;
                    }

                    foreach (DataLibrary.DocumentDTO p in lvFiwpITRForms.SelectedItems)
                    {
                        for (int i = 0; i < _trgIWPITR.Count; i++)
                        {
                            if (p.SPCollectionID == _trgIWPITR[i].SPCollectionID)
                            {
                                if (_trgIWPITR[i].DocumentID == 0)
                                    _trgIWPITR.RemoveAt(i);
                                else
                                    _trgIWPITR[i].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Delete;
                                break;
                            }
                        }
                    }
                    lvFiwpITRForms.ItemsSource = null;
                    lvFiwpITRForms.ItemsSource = _trgIWPITR.Where(x => x.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete);

                    if (_orgITR == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;
                    }
                    lvITRForms.ItemsSource = null;
                    lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR where b.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete select b.SPCollectionID).Contains(x.SPCollectionID) select x);


                }
                catch (Exception ex)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Remove " + tbpageTitle.Text, "There is a problem removing the selected item - Please try again later", "Error");
                }

                Login.MasterPage.Loading(false, this);
            }
        }

        public void RunReport(string param)
        {
            try
            {
                if (_viewer != null)
                    _viewer.Dispose();

                _viewer = OZReportAPI.CreateViewer(viewerFrame, param);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        #endregion
    }
}
