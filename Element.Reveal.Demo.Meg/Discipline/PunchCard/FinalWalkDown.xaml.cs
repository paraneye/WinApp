using Element.Reveal.Meg.Discipline.ITR;
using Element.Reveal.Meg.Lib;
using Element.Reveal.Meg.RevealProjectSvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WinAppLibrary.ServiceModels;
using WinAppLibrary.UI;
using WinAppLibrary.Utilities;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Networking.Proximity;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

using System.Text;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.PunchCard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinalWalkDown : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ProximityHandler ProximityHandler;
        private bool _onHandling = true;
        string _pinno = "1234";
        private IItrDoc ucDoc;

        // FLAG
        public bool isNewer = true;
        int intDTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;

        // Login Infomation
        RevealUserSvc.LoginaccountDTO lstUserInfo = new RevealUserSvc.LoginaccountDTO();

        // For Online
        List<ProjectDTO> lstContactList = new List<ProjectDTO>();
        List<SystemDTO> lstSystemList = new List<SystemDTO>();
        List<RevealProjectSvc.ModuleDTO> lstModuleList = new List<RevealProjectSvc.ModuleDTO>();
        List<TurnOverDetailData> lstTempList = new List<TurnOverDetailData>();
        TurnOverDetailData SelectedDetail = null;
        List<WinAppLibrary.UI.ObjectNFCSign> signed = new List<WinAppLibrary.UI.ObjectNFCSign>();

        WalkdownDTOSet lstTotalList = new WalkdownDTOSet();
        List<QaqcformdetailDTO> lstTurnoverList = new List<QaqcformdetailDTO>();
        List<QaqcformDTO> lstTurnoverComboList = new List<QaqcformDTO>();

        // Common Combo
        List<RevealCommonSvc.ComboBoxDTO> lstCommonList = new List<RevealCommonSvc.ComboBoxDTO>();

        // Popup
        Storyboard _sbDetailON, _sbDetailOFF;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;

        // Bitmap
        private WriteableBitmap wbBitmap;
        private ShareOperation soShareOp;
        RandomAccessStreamReference rasrReference;
        IReadOnlyList<IStorageItem> iroReadonlyList;

        // Drawing
        List<RevealProjectSvc.DrawingDTO> lstEntireDrawingList = new List<DrawingDTO>();
        private string strSaveFileName = string.Empty;
        
        //List<DownloadedImage> lstImageList = new List<DownloadedImage>();
        List<string> lstImageNameList = new List<string>();

        // SharePoint 정보
        private string SPURL = "https://elementindustrial.sharepoint.com/sites/clients/DevSigmaDemo/";
        private string SPUser = "test@elementindustrial.onmicrosoft.com";
        private string SPPassword = "ou812123!";

        public FinalWalkDown()
        {
            this.InitializeComponent();

            //Login.MasterPage.ShowTopBanner = true;
            //Login.MasterPage.ShowBackButton = true;
            //Login.MasterPage.ShowBackButton = false;
            //Login.MasterPage.SetPageTitle("Final Walk Down");
            
            btnOpenPopUp.Content = "NO DATA";

            // 기본 이벤트 처리 - 기본 정보
            cboContractor.SelectionChanged += cboContractor_SelectionChanged;
            cboSystemNo.SelectionChanged += cboSystemNo_SelectionChanged;
            btnOpenPopUp.Click += btnOpenPopUp_Click;
            lvDisciplines.SelectionChanged += lvDisciplines_SelectionChanged;

            // 기본 이벤트 처리 - attandee 정보
            ProximityHandler = new Lib.ProximityHandler();
            ProximityHandler.OnException += ProximityHandler_OnException;
            ProximityHandler.OnMessage += ProximityHandler_OnMessage;

            // 기본 이벤트 처리 - 도면 관련 정보
            lvDrawingList.SelectionChanged += lvDrawingList_SelectionChanged;
            lvDownList.ItemClick += lvDownList_ItemClick;
            btnImgDelete.Click += btnImgDelete_Click;

            // 기본 이벤트 처리 - Discipline 관련 정보
            lvPunchList.SelectionChanged += lvPunchList_SelectionChanged;
            btnAdd.Click += btnAdd_Click;

            // 기본 이벤트 처리 - 저장
            btnSave.Click += btnSave_Click;

            // 서브 이벤트 처리 - 이미지 캡쳐
            btnCapture.Click += btnCapture_Click;

            // 서브 이벤트 처리 - 저장된 이미지 로드
            btnLoad.Click += btnLoad_Click;

            // 서브 이벤트 처리 - 이미지 로딩 취소
            btnCancel.Click += btnCancel_Click;

            // 서브 이벤트 처리 - Equip. / Line / Tag 변경
            cboInsEquLinTag.SelectionChanged += cboInsEquLinTag_SelectionChanged;

            // 서브 이벤트 처리 - 팝업처리 완료
            // 팝업 로딩 위치로 옮김
            // 1. 새 데이터 등록 ( btnAdd_Click )
            // 2. 기존 데이터 수정 ( lvDisciplines_SelectionChanged )

            // 서브 이벤트 처리 - 팝업취소
            btnPanelCancel.Click += btnPanelCancel_Click;

            // 각 컨트롤 생성
            LoadData();

            // 도면 목록 생성
            LoadDrawingList();

            // 다운된 도면 목록 생성
            DownLoadList();
        }

        /// <summary>
        /// 페이지 로드
        /// </summary>
        /// <param name="navigationParameter"></param>
        /// <param name="pageState"></param>
        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            ProximityHandler.SetProximityDevice(ProximityDevice.GetDefault());
            _onHandling = false;

            //var args = e.Parameter as ShareTargetActivatedEventArgs;

            //if (args != null)
            //{
            //    soShareOp = args.ShareOperation;

            //    if (soShareOp.Data.Contains(StandardDataFormats.Bitmap))
            //    {
            //        rasrReference = await soShareOp.Data.GetBitmapAsync();
            //        await ProcessBitmap();
            //    }
            //    else if (soShareOp.Data.Contains(StandardDataFormats.StorageItems))
            //    {
            //        iroReadonlyList = await soShareOp.Data.GetStorageItemsAsync();
            //        await ProcessStorageItems();
            //    }
            //    else soShareOp.ReportError("It was unable to find a valid bitmap.");
            //}
        }

        /// <summary>
        /// 기본 정보 로드
        /// </summary>
        private async void LoadData()
        {
            //lstUserInfo = await (new Lib.ServiceModel.UserModel()).GetLoginaccountByTempOwnerPesonnelID(0);
            
            txtOriginator.Text = Login.UserAccount.LoginName;

            // Select Categories for PunchCard
            lstCommonList = await (new Lib.ServiceModel.CommonModel()).GetLookup_Combo("PunchCategory", "");

            // Select ContactList
            lstContactList = await (new Lib.ServiceModel.ProjectModel()).GetContractorProejctByOwnerProject(Login.UserAccount.CurProjectID);

            var ContList = from C in lstContactList
                           select new TurnOverData { ContractNo = C.ProjectID, ContractName = C.ProjectName };

            if (ContList.Count() > 0)
            {
                cboContractor.ItemsSource = ContList;
                cboContractor.DisplayMemberPath = "ContractName";
                cboContractor.SelectedValuePath = "ContractNo";
            }

            _onHandling = false;
        }

        /// <summary>
        /// Sharepoint에 저장된 도면 데이터 로드
        /// </summary>
        private async void LoadDrawingList()
        {
            // get List<RevealProjectSvc.DrawingDTO>
            lstEntireDrawingList = await (new Lib.ServiceModel.ProjectModel()).GetDrawingByProjectID(Login.UserAccount.CurProjectID, 0);

            if (lstEntireDrawingList != null && lstEntireDrawingList.Count > 0)
            {
                lvDrawingList.ItemsSource = lstEntireDrawingList;
            }
        }

        /// <summary>
        /// 다운받은 도면 목록 로드
        /// </summary>
        private async void DownLoadList()
        {
            int intDownListCnt = 0;

            StorageFolder picturesFolder = KnownFolders.PicturesLibrary;
            IReadOnlyList<StorageFile> fileList = await picturesFolder.GetFilesAsync();

            DataGroup group = new DataGroup("Drawing", "", "");
            List<DataGroup> source = new List<DataGroup>();

            foreach (StorageFile file in fileList)
            {
                if (file != null)
                {
                    // Document의 이미지 폴더에 있는 이미지들 중에서 FWD로 시작하면서 Jpeg형태의 이미지만 로드함.
                    if (file.Name.Substring(0, 3) == "FWD" && file.Name.Substring(file.Name.Length - 3) == "jpg")
                    {
                        group.Items.Add(new DataItem(MakeFileName() + intDownListCnt.ToString(), file.DisplayName, file.Path, "", group));
                    }
                }
                else
                {
                    AlertMessage("Please, check this file format.", "");
                }

                intDownListCnt++;
            }

            source.Add(group);

            this.DefaultViewModel["Drawings"] = source;  // Data binding
        }

        /// <summary>
        /// 뒤로 가기 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        #region | 기본 이벤트 - 기본 정보 관련 |

        private async void cboContractor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cboContractor.SelectedIndex > -1)
                {
                    lstSystemList = await (new Lib.ServiceModel.ProjectModel()).GetSystemByTurnoverProject(Login.UserAccount.CurProjectID, Int32.Parse(cboContractor.SelectedValue.ToString()));

                    var SystemList = from S in lstSystemList
                                     select new TurnOverData { SystemNumber = S.SystemNumber, SystemName = S.SystemName + "_" + S.SystemID };

                    if (SystemList.Count() > 0)
                    {
                        cboSystemNo.ItemsSource = SystemList;
                        cboSystemNo.DisplayMemberPath = "SystemNumber";
                        cboSystemNo.SelectedValuePath = "SystemName";
                    }
                }
            }
            catch (Exception ex)
            {
                AlertMessage("There isn't a Contactor's information.", "");
            }
        }

        private async void cboSystemNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cboSystemNo.SelectedIndex > -1)
                {
                    string strSysInfo = cboSystemNo.SelectedValue.ToString();
                    int intSysNo;

                    string[] strArrSysInfo = strSysInfo.Split('_');

                    txtSystemNm.Text = strArrSysInfo[0];
                    intSysNo = Int32.Parse(strArrSysInfo[1]);

                    lstModuleList = await (new Lib.ServiceModel.ProjectModel()).GetModuleByTurnoverSystem(Login.UserAccount.CurProjectID, Int32.Parse(cboContractor.SelectedValue.ToString()), intSysNo);

                    var ModuleList = from M in lstModuleList
                                     select new TurnOverData { ModuleID = M.ModuleID, ModuleName = M.ModuleName, SystemName = strSysInfo };

                    if (ModuleList.Count() > 0)
                    {
                        lvDisciplines.ItemsSource = ModuleList;
                        btnOpenPopUp.Content = "SELECT";
                    }
                    else
                    {
                        lvDisciplines.ItemsSource = null;
                        btnOpenPopUp.Content = "NO DATA";
                    }
                }
                else
                {
                    txtSystemNm.Text = "";
                    lvDisciplines.ItemsSource = null;
                    btnOpenPopUp.Content = "NO DATA";
                }
            }
            catch (Exception ex)
            {
                AlertMessage("There is no system information.", "");
            }
        }

        private void btnOpenPopUp_Click(object sender, RoutedEventArgs e)
        {
            if (popDisciplines.IsOpen)
            {
                popDisciplines.IsOpen = false;
            }
            else
            {
                popDisciplines.HorizontalOffset = -5;
                popDisciplines.IsOpen = true;
            }
        }

        private async void lvDisciplines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string strSysInfo = string.Empty;

                int intSysNo = 0;
                string strModuleName = string.Empty;
                List<int> lstTempModule = new List<int>();

                if (e.AddedItems.Count > 0)
                {
                    for (int intTmpI = 0; intTmpI < e.AddedItems.Count; intTmpI++)
                    {
                        lstTempModule.Add(((TurnOverData)e.AddedItems[intTmpI]).ModuleID);
                    }

                    strSysInfo = ((TurnOverData)e.AddedItems[0]).SystemName;
                    string[] strArrSysInfo = strSysInfo.Split('_');
                    intSysNo = Int32.Parse(strArrSysInfo[1]);

                    lstTotalList = await (new Lib.ServiceModel.ProjectModel()).GetWalkDownBySystem(Login.UserAccount.CurProjectID, Int32.Parse(cboContractor.SelectedValue.ToString()), intSysNo, lstTempModule);

                    lstTurnoverList = lstTotalList.qaqcformdetailDTOS;
                    lstTurnoverComboList = lstTotalList.qaqcformDTOS;

                    lstTempList = (from To in lstTurnoverList
                                   select new TurnOverDetailData
                                   {
                                       PunchNo = To.InspectedKeyValue,
                                       ELTData = To.StringValue4,
                                       ELTDQaqcFormID = To.StringValue5,
                                       Description = To.StringValue6,
                                       Category = To.StringValue7,
                                       CategoryName = To.StringValue8,
                                       Responsible = To.StringValue9,
                                       FileName = To.StringValue10,
                                       Status = intDTOStatus
                                   }).ToList<TurnOverDetailData>();

                    lvPunchList.ItemsSource = lstTempList;
                    // For Demo
                    //lvPunchList.ItemsSource = null;
                }
                else
                {
                    lvPunchList.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                AlertMessage("There is no data.", "");
            }
        }

        #endregion
        
        #region | 기본 이벤트 처리 - attandee 정보(NFC카드) |

        private void ProximityHandler_OnException(object sender, object e)
        {
            // uiSlideButton.Hide();
            //  Loading(false);
        }

        private void ProximityHandler_OnMessage(object sender, object e)
        {
            var type = (NotifyType)sender;
            switch (type)
            {
                case NotifyType.NdefMessage:
                    AssignProcedureIn(e.ToString());
                    break;
                default:
                    break;
            }
        }

        private void AssignProcedureIn(string tagmsg)
        {
            #region NFC TAG Default Code

            if (!_onHandling)
            {
                if (!string.IsNullOrEmpty(tagmsg))
                {
                    _onHandling = true;
                    int personId = 0;
                    string personname = "";
                    try
                    {
                        string[] temptagmsg = tagmsg.Split('*');

                        if (temptagmsg.Length > 1)
                        {
                            personId = Convert.ToInt32(temptagmsg[0]);
                            personname = temptagmsg[1];

                            if (temptagmsg.Length > 2)
                            {
                                _pinno = temptagmsg[2];
                            }

                            SetSign(personId, personname, _pinno);

                        }

                        _onHandling = false;
                    }
                    catch (Exception e)
                    {
                        (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "AssignProcedure");

                        AlertMessage("We had a problem to update signing. Please contact Administrator", "Error!");

                        _onHandling = false;
                    }
                }
                else
                {
                    _onHandling = false;
                    AlertMessage("This tag doesn't have crew information", "Alert");
                }
            }

            #endregion
        }

        private async void SetSign(int personId, string personname, string _pinno)
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    lvNFCSignList.Items.Clear();
                    signed.Add(new WinAppLibrary.UI.ObjectNFCSign { isSigned = "Signed", MemberGrade = "", PersonnelName = personname, SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue), SignedTime = DateTime.Now.ToString() });

                    foreach (var item in signed)
                    {
                        lvNFCSignList.Items.Add(item);
                    }
                    //lvNFCSignList.DataContext = signed;
                });
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "AssignCrew");
                _onHandling = false;
            }
        }

        private async void log(string s)
        {
            Windows.Storage.StorageFolder BaseFolder = Lib.ContentPath.OffModeUserFolder;
            bool x = await (new WinAppLibrary.Utilities.Helper()).SaveFileStream(BaseFolder, "ddd.log", new MemoryStream(Encoding.UTF8.GetBytes(s)));
        }

        private bool IsSelectedSign()
        {
            bool bl = false;
            ucDoc.checkSelectSign();
            bl = ucDoc.isSelectedSign;
            return bl;
        }

        private bool ControlValidate()
        {
            ucDoc.checkValidate();
            bool bl = ucDoc.isValidate;
            return bl;
        }
        
        #endregion
        
        #region | 기본 이벤트 - 도면 정보 관련 |

        private async void lvDrawingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lvDrawingList.SelectedItems.Count() > 0)
                {
                    // Case 1
                    // For Sharepoint
                    // 

                    string strImagePath = ((DrawingDTO)(e.AddedItems[0])).DrawingFilePath;
                    string strImageName = ((DrawingDTO)(e.AddedItems[0])).DrawingName;
                    //string strImageName = ((DrawingDTO)(e.AddedItems[0])).DrawingFileURL;
                    string strImageUrl = strImagePath + strImageName;
                    //string strImageUrl = strImagePath + strImageName + ".jpg";
                    //string strImageUrl = "https://elementindustrial.sharepoint.com/sites/clients/DevSigmaDemo/" + strImageName;

                    strImageUrl = "https://elementindustrial.sharepoint.com/sites/clients/DevSigmaDemo/Turnover/260KV-12231-TurnoverPackage-5_0.jpg";

                    string strDesination = "";

                    strDesination = "FWD" + strImageName + ".jpg";

                    // 저장할 폴더를 지정함. - 이미지 라이브러리 폴더
                    StorageFolder folder = KnownFolders.PicturesLibrary;

                    if (!(await FileExistsAsync(folder, strDesination)))
                    {

                        if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                        {
                            // Sharepoint의 인증여부 확인
                            bool login = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(SPURL, SPUser, SPPassword);

                            // 인증된 경우 처리
                            if (login)
                            {
                                // 재인증 확인
                                if (!WinAppLibrary.Utilities.SPDocument.IsLogin)
                                {
                                    await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(SPURL, SPUser, SPPassword);
                                }

                                // 로딩바 시작
                                Login.MasterPage.Loading(true, this);

                                // 해당주소의 이미지를 바이트로 받아옴.
                                byte[] btyOriginal = await (new WinAppLibrary.Utilities.SPDocument()).GetDocument(strImageUrl);

                                // 받아온 바이틀 비트맵에 배치함.
                                BitmapImage biOriginal = await (new WinAppLibrary.Utilities.Helper()).GetBitmapImageFromBytes(btyOriginal);

                                // 받아온 이미지 미리보기 처리
                                // imgDrawing.Source = biOriginal;

                                // 해당 폴더에 빈 이미지를 생성함.
                                StorageFile file = await folder.CreateFileAsync(strDesination, CreationCollisionOption.ReplaceExisting);

                                // 빈 이미지에 파일 스트림으로 쓸 준비를 하고 쓰기 시작
                                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                                {
                                    using (IOutputStream outputStream = fileStream.GetOutputStreamAt(0))
                                    {
                                        using (DataWriter dwWriter = new DataWriter(outputStream))
                                        {
                                            dwWriter.WriteBytes(btyOriginal);
                                            await dwWriter.StoreAsync();
                                            dwWriter.DetachStream();
                                        }

                                        await outputStream.FlushAsync();
                                    }
                                }

                                // 로딩바 종료
                                Login.MasterPage.Loading(false, this);

                                // 다운로드 대상 목록
                                DownLoadList();

                                // 다운완료
                                AlertMessage("It was finished.", "");
                            }
                            else
                            {
                                // 인증실패시 경고 메세지 띄움.
                                AlertMessage("We couldn't sign in Sharepoint Server. Please check your authentication.", "Alert!");
                            }

                            //// CASE 2
                            //// For General Case
                            ////
                            //string strImagePath = ((DrawingDTO)(e.AddedItems[0])).DrawingFilePath;
                            //string strImageName = ((DrawingDTO)(e.AddedItems[0])).DrawingName;
                            //string strImageUrl = strImagePath + strImageName;
                            //string strDesination = "";

                            //Uri uSource;

                            //if (Uri.TryCreate(strImageUrl, UriKind.Absolute, out uSource))
                            //{
                            //    Login.MasterPage.Loading(true, this);

                            //    StorageFile sfDestinationFile;

                            //    //strDesination = MakeFileName() + ".jpg";
                            //    strDesination = "FWD" + strImageName;

                            //    sfDestinationFile = await KnownFolders.PicturesLibrary.CreateFileAsync(strDesination, CreationCollisionOption.GenerateUniqueName);

                            //    BackgroundDownloader downloader = new BackgroundDownloader();
                            //    DownloadOperation download = downloader.CreateDownload(uSource, sfDestinationFile);

                            //    await download.StartAsync();

                            //    Login.MasterPage.Loading(false, this);

                            //    AlertMessage("It was finished.", "");

                            //    ResponseInformation response = download.GetResponseInformation();

                            //    DownLoadList();
                            //}
                            //else
                            //{
                            //    AlertMessage("There isn't the drawing, Please check it.", "Sorry!!");
                            //}
                        }
                    }
                    else
                    {
                        AlertMessage("There is the file in your system.", "");
                    }
                }
            }
            catch (Exception ex)
            {
                Login.MasterPage.Loading(false, this);

                AlertMessage("Please, check the URL!", "Sorry!!");
            }
        }

        private async void lvDownList_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // When image binding without no resource
                var file = await StorageFile.GetFileFromPathAsync(((DataCommon)e.ClickedItem).ImagePath);
                var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                var image = new BitmapImage();

                image.SetSource(fileStream);
                imgDrawing.Source = image;

                if (image != null)
                {
                    btnImgDelete.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    strSaveFileName = ((DataCommon)e.ClickedItem).Title + ".jpg";
                }
                else
                {
                    btnImgDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    strSaveFileName = string.Empty;
                }
            }
            catch (Exception ex)
            {
                AlertMessage("Please, check the image file. It must be fake file!", "Waning!");
            }
        }

        private async void btnImgDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await Helper.YesOrNoMessage("Do you want to delete this fiel?", "Attention"))
                {
                    StorageFolder sfoSavedFolder = KnownFolders.PicturesLibrary;

                    if (await FileExistsAsync(sfoSavedFolder, strSaveFileName))
                    {
                        StorageFile sfiSavedFile = await sfoSavedFolder.GetFileAsync(strSaveFileName);

                        await sfiSavedFile.DeleteAsync(StorageDeleteOption.PermanentDelete);

                        AlertMessage("It was Deleted!", "");

                        imgDrawing.Source = null;
                        strSaveFileName = string.Empty;
                        btnImgDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                        DownLoadList();

                    }
                    else
                    {
                        AlertMessage("This path is wrong. Please check it!", "");
                    }
                }
            }
            catch (Exception ex)
            {
                AlertMessage(ex.ToString(), "Attention");
            }
        }

        /// <summary>
        /// 파일 존재 여부 체크용
        /// </summary>
        /// <param name="sfFolder">저장된 폴더 위치</param>
        /// <param name="strFileName"></param>
        /// <returns>true/false</returns>
        private async Task<bool> FileExistsAsync(StorageFolder sfFolder, string strFileName)
        {
            try
            {
                await sfFolder.GetFileAsync(strFileName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        #endregion

        #region | 기본 이벤트 - Discipline 정보 관련 |

        private async void lvPunchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                isNewer = false;

                var CommComboList = from CCo in lstCommonList
                                    select CCo;

                cboInsCategory.ItemsSource = CommComboList;
                cboInsCategory.DisplayMemberPath = "DataName";
                cboInsCategory.SelectedValuePath = "DataID";

                var ELTComboList = from ECo in lstTurnoverComboList
                                   select ECo;

                // Shawn Yang : ELTDQaqcFormID가 맞는지 확인
                cboInsEquLinTag.ItemsSource = ELTComboList;
                cboInsEquLinTag.DisplayMemberPath = "KeyValue";
                cboInsEquLinTag.SelectedValuePath = "QAQCFormID";

                SelectedDetail = (TurnOverDetailData)e.AddedItems[0];

                if (!string.IsNullOrEmpty(((TurnOverDetailData)e.AddedItems[0]).PunchNo))
                {
                    txtInsPunchNo.Text = ((TurnOverDetailData)e.AddedItems[0]).PunchNo;
                }
                // Shawn Yang : ELTDQaqcFormID가 맞는지 확인
                if (!string.IsNullOrEmpty(((TurnOverDetailData)e.AddedItems[0]).ELTDQaqcFormID))
                {
                    int intTmpI = 0;

                    // How to use Combobox
                    // Case 1 :
                    // string thumbnailModeName = ((ComboBoxItem)ModeComboBox.SelectedItem).Name;
                    // Case 2 :
                    foreach (var obj in cboInsEquLinTag.Items)
                    {
                        if (((QaqcformDTO)obj).QAQCFormID.ToString() == ((TurnOverDetailData)e.AddedItems[0]).ELTDQaqcFormID)
                        {
                            cboInsEquLinTag.SelectedIndex = intTmpI;
                            break;
                        }

                        intTmpI++;
                    }
                }

                if (!string.IsNullOrEmpty(((TurnOverDetailData)e.AddedItems[0]).Category))
                {
                    int intTmpI = 0;

                    foreach (var obj in cboInsCategory.Items)
                    {
                        if (((RevealCommonSvc.ComboBoxDTO)obj).DataID.ToString() == ((TurnOverDetailData)e.AddedItems[0]).Category)
                        {
                            cboInsCategory.SelectedIndex = intTmpI;
                            break;
                        }

                        intTmpI++;
                    }
                }

                if (!string.IsNullOrEmpty(((TurnOverDetailData)e.AddedItems[0]).Responsible))
                {
                    txtInsResponsible.Text = ((TurnOverDetailData)e.AddedItems[0]).Responsible;
                }

                if (!string.IsNullOrEmpty(((TurnOverDetailData)e.AddedItems[0]).Description))
                {
                    txtInsDescription.Text = ((TurnOverDetailData)e.AddedItems[0]).Description;
                }

                if (!string.IsNullOrEmpty(((TurnOverDetailData)e.AddedItems[0]).Appendix))
                {
                    // When image binding without no resource
                    var file = await StorageFile.GetFileFromPathAsync(((TurnOverDetailData)e.AddedItems[0]).Appendix);
                    var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    var image = new BitmapImage();

                    image.SetSource(fileStream);
                    imgPicture.Source = image;
                }

                if (!popLineAdd.IsOpen)
                {
                    LoadStoryBoardSwitch();

                    grDetailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    _sbDetailON.Begin();

                    btnPanelCollapse.Click += btnPanelCollapse_Click;

                    popLineAdd.HorizontalOffset = Window.Current.Bounds.Width - 850;

                    popLineAdd.IsOpen = true;
                }
            }
        }
        
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SelectedDetail = null;
            isNewer = true;

            if (lvDisciplines.Items.Count() > 0)
            {
                if (lvDisciplines.SelectedIndex > -1)
                {
                    LoadStoryBoardSwitch();

                    grDetailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    _sbDetailON.Begin();

                    btnPanelCollapse.Click += btnPanelCollapse_Click;

                    var CommComboList = from CCo in lstCommonList
                                        select CCo;

                    cboInsCategory.ItemsSource = CommComboList;
                    cboInsCategory.DisplayMemberPath = "DataName";
                    cboInsCategory.SelectedValuePath = "DataID";

                    var ELTComboList = from ECo in lstTurnoverComboList
                                       select ECo;

                    // Shawn Yang : ELTDQaqcFormID가 맞는지 확인
                    cboInsEquLinTag.ItemsSource = ELTComboList;
                    cboInsEquLinTag.DisplayMemberPath = "KeyValue";
                    cboInsEquLinTag.SelectedValuePath = "QAQCFormID";

                    popLineAdd.HorizontalOffset = Window.Current.Bounds.Width - 850;
                    popLineAdd.IsOpen = true;
                }
                else
                {
                    AlertMessage("Please, select a discipline at least.", "");
                }
            }
            else
            {
                AlertMessage("Please, select other informations in Project Information", "");
            }
        }

        #endregion

        #region | 기본 이벤트 - 저장 관련 |
        
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (lvPunchList.Items.Count > 0)
            {
                TurnoverDTO toList = new TurnoverDTO();
                List<TurnoverattendeeDTO> lstToaList = new List<TurnoverattendeeDTO>();
                List<QaqcformdetailDTO> lstQfList = new List<QaqcformdetailDTO>();

                #region 데이터 전송

                string[] strArrSysInfo = cboSystemNo.SelectedValue.ToString().Split('_');
                string strSysNm = strArrSysInfo[0];
                int intSysNo = Int32.Parse(strArrSysInfo[1]);

                WalkdownDTOSet wdResult = new WalkdownDTOSet();
                List<TurnoverDTO> lstToList = new List<TurnoverDTO>();

                // Turnover 정보
                // Disipline 수 만큼 등록처리.

                foreach (var lvt in lvDisciplines.Items)
                {
                    TurnOverData toItem = (TurnOverData)lvt;

                    //toList.OwnerID = txtOriginator.Text;
                    toList.ProjectID = Login.UserAccount.CurProjectID;
                    toList.ContractorID = Int32.Parse(cboContractor.SelectedValue.ToString());
                    toList.SystemID = intSysNo;
                    toList.FWD_Date = Convert.ToDateTime(txtDate.Text);
                    toList.ModuleID = toItem.ModuleID;
                    toList.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;

                    lstToList.Add(toList);
                }

                // NFC카드 추가할 것.
                foreach (ObjectNFCSign s in signed)
                {
                    lstToaList.Add(new TurnoverattendeeDTO
                    {
                        PeronnelName = s.PersonnelName,
                        SignedDate = DateTime.Parse(s.SignedTime),
                        ModuleID = Login.UserAccount.CurModuleID,
                        ProjectID = Login.UserAccount.CurProjectID,
                        DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New
                    });
                }

                // 
                foreach (var lvp in lvPunchList.Items)
                {
                    QaqcformdetailDTO qfdList = new QaqcformdetailDTO();
                    TurnOverDetailData item = (TurnOverDetailData)lvp;

                    qfdList.InspectedKeyValue = item.PunchNo;
                    qfdList.StringValue4 = item.ELTData;
                    qfdList.StringValue5 = item.ELTDQaqcFormID;
                    qfdList.StringValue6 = item.Description;
                    qfdList.StringValue7 = item.Category;
                    qfdList.StringValue8 = item.CategoryName;
                    qfdList.StringValue9 = item.Responsible;
                    qfdList.StringValue10 = item.FileName;
                    qfdList.DTOStatus = item.Status;

                    if (!string.IsNullOrEmpty(item.Appendix))
                    {
                        SubmitImages(item.Appendix, item.PunchNo + ".jpg");
                    }

                    lstQfList.Add(qfdList);
                }

                wdResult.qaqcformdetailDTOS = lstQfList;
                wdResult.turnoverDTOS = lstToList;
                wdResult.turnoverattendeeDTOS = lstToaList;

                SaveResult(wdResult);

                #endregion
            }
            else
            {
                AlertMessage("There is no datum in the list.", "");
            }
        }

        private async void SubmitImages(string stUrl, string strFileNm)
        {
            #region 이미지 전송

            MemoryStream _stream;

            var file = await StorageFile.GetFileFromPathAsync(stUrl);
            //var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            //var image = new BitmapImage();

            //image.SetSource(fileStream);
            //imgPicture.Source = image;

            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                var reader = new DataReader(fileStream.GetInputStreamAt(0));
                var bytes = new byte[fileStream.Size];
                await reader.LoadAsync((uint)fileStream.Size);
                reader.ReadBytes(bytes);
                _stream = new MemoryStream(bytes);
            }

            bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

            if (islogin)
            {
                try
                {
                    var rst = await (new WinAppLibrary.Utilities.SPDocument()).SaveJpegContent(Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.ProjectDoc + "/", strFileNm, _stream);
                }
                catch (Exception ex)
                {
                    var aa = ex.ToString();
                }
            }

            #endregion
        }

        private async void SaveResult(WalkdownDTOSet wdSet)
        {
            try
            {
                WalkdownDTOSet wdReturn = await (new Lib.ServiceModel.ProjectModel()).SaveQaqcformWithSharepoint(wdSet);

                AlertMessage("It was saved.", "");

                this.Frame.Navigate(typeof(MainMenu), Login.UserAccount.PersonnelID);
            }
            catch (Exception ex)
            {
                AlertMessage("Now we are checking our system.", "");
            }
        }
        
        #endregion

        #region | 팝업 이벤트 - 첨부 이미지 관련 |
        
        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            CheckAndClearShareOperation();

            var camera = new CameraCaptureUI();
            var result = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (result != null)
            {
                await LoadBitmap(await result.OpenAsync(FileAccessMode.ReadWrite));
            }
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            CheckAndClearShareOperation();

            FileOpenPicker picker = new FileOpenPicker();

            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");

            StorageFile result;

            result = await picker.PickSingleFileAsync();

            if (result != null)
            {
                await LoadBitmap(await result.OpenAsync(FileAccessMode.Read));
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            imgPicture.Source = null;
            wbBitmap = null;
        }

        private async Task LoadBitmap(IRandomAccessStream stream)
        {
            wbBitmap = new WriteableBitmap(1, 1);
            wbBitmap.SetSource(stream);
            wbBitmap.Invalidate();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => imgPicture.Source = wbBitmap); ;
        }

        private async Task ProcessBitmap()
        {
            if (rasrReference != null)
            {
                await LoadBitmap(await rasrReference.OpenReadAsync());
            }
        }

        private async Task ProcessStorageItems()
        {
            foreach (var item in iroReadonlyList)
            {
                if (item.IsOfType(StorageItemTypes.File))
                {
                    var file = item as StorageFile;
                    if (file.ContentType.StartsWith("image", StringComparison.CurrentCultureIgnoreCase))
                    {
                        await LoadBitmap(await file.OpenReadAsync());
                        break;
                    }
                }
            }
        }

        private void CheckAndClearShareOperation()
        {
            if (soShareOp != null)
            {
                soShareOp.ReportCompleted();
                soShareOp = null;
                //wbBitmap = null;
                //rasrReference = null;
                //iroReadonlyList = null;
                //imgPicture = null;
            }
        }

        #endregion

        #region | 팝업 이벤트 기본 동작 관련 |

        private void cboInsEquLinTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isNewer)
            {
                //맨 마지막 라인 punch#의 값을 가져옴
                string strLastData = lstTempList.Last<TurnOverDetailData>().PunchNo;

                int intLastNo = 0;

                // punch#의 값이 NULL이 아닐 경우
                if (!string.IsNullOrEmpty(strLastData))
                {
                    // punch#의 값에 - 이 존재할 경우
                    if (strLastData.IndexOf('-') > 0)
                    {
                        // 배열로 분리
                        string[] strArrLastNo = strLastData.Split('-');

                        try
                        {
                            // 마지막 값의 상수화해서 옮김
                            intLastNo = Int32.Parse(strArrLastNo[strArrLastNo.Length - 1]);
                        }
                        catch (Exception ex)
                        {
                            // 마지막 값이 숫자형 문자가 아닐 경우 처리
                            intLastNo = 0;
                        }
                    }

                }

                intLastNo++;

                if (cboInsEquLinTag.SelectedIndex > -1)
                {
                    // Shawn Yang : ELTDQaqcFormID가 맞는지 확인
                    if (intLastNo > 0)
                    {
                        txtInsPunchNo.Text = ((QaqcformDTO)cboInsEquLinTag.SelectedItem).KeyValue.ToString() + "-" + intLastNo.ToString();
                    }
                    else
                    {
                        txtInsPunchNo.Text = ((QaqcformDTO)cboInsEquLinTag.SelectedItem).KeyValue.ToString();
                    }
                }
                else
                {
                    txtInsPunchNo.Text = "";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtInsPunchNo.Text))
                {
                    int intTempLastNo = 0;

                    // punch#의 값에 - 이 존재할 경우
                    if (txtInsPunchNo.Text.IndexOf('-') > 0)
                    {
                        // 배열로 분리
                        string[] strArrLastNo = txtInsPunchNo.Text.Split('-');

                        try
                        {
                            // 마지막 값의 상수화해서 옮김
                            intTempLastNo = Int32.Parse(strArrLastNo[strArrLastNo.Length - 1]);
                        }
                        catch (Exception ex)
                        {
                            // 마지막 값이 숫자형 문자가 아닐 경우 처리
                            intTempLastNo = 0;
                        }
                    }

                    if (cboInsEquLinTag.SelectedIndex > -1)
                    {
                        // Shawn Yang : ELTDQaqcFormID가 맞는지 확인
                        if (intTempLastNo > 0)
                        {
                            txtInsPunchNo.Text = ((QaqcformDTO)cboInsEquLinTag.SelectedItem).KeyValue.ToString() + "-" + intTempLastNo.ToString();
                        }
                        else
                        {
                            txtInsPunchNo.Text = ((QaqcformDTO)cboInsEquLinTag.SelectedItem).KeyValue.ToString();
                        }
                    }
                    else
                    {
                        txtInsPunchNo.Text = "";
                    }
                }
            }
        }

        private async void btnPanelCollapse_Click(object sender, RoutedEventArgs e)
        {
            // Temporary Save Path
            // Lib.ContentPath.OffModeFolder
            string strSavedPath = string.Empty;
            string strSavedName = string.Empty;
            bool isSaveOK = true;
            bool isData = false;

            if (popLineAdd.IsOpen)
            {
                grDetailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                _sbDetailOFF.Begin();

                #region Image Save

                if (wbBitmap != null)
                {
                    try
                    {
                        var picker = new FileSavePicker
                        {
                            SuggestedStartLocation = PickerLocationId.PicturesLibrary
                        };
                        picker.FileTypeChoices.Add("Image", new List<string>() { ".jpg" });
                        picker.DefaultFileExtension = ".jpg";
                        picker.SuggestedFileName = MakeFileName();

                        var savedFile = await picker.PickSaveFileAsync();

                        if (savedFile != null)
                        {
                            strSavedPath = savedFile.Path;
                            strSavedName = savedFile.Name;

                            using (var output = await savedFile.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, output);

                                byte[] pixels;

                                using (var stream = wbBitmap.PixelBuffer.AsStream())
                                {
                                    pixels = new byte[stream.Length];
                                    await stream.ReadAsync(pixels, 0, pixels.Length);
                                }

                                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)wbBitmap.PixelWidth, (uint)wbBitmap.PixelHeight, 96.0, 96.0, pixels);

                                await encoder.FlushAsync();
                                await output.FlushAsync();

                                savedFile = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var s = ex.ToString();
                    }
                    finally
                    {
                        CheckAndClearShareOperation();
                    }
                }

                #endregion

                #region Validation Check

                TurnOverDetailData tddTEmp;

                if (SelectedDetail != null)
                {
                    tddTEmp = SelectedDetail;
                }
                else
                {
                    tddTEmp = new TurnOverDetailData();
                }


                if (!string.IsNullOrEmpty(txtInsPunchNo.Text))
                {
                    tddTEmp.PunchNo = txtInsPunchNo.Text;
                    isData = true;
                }
                else
                {
                    AlertMessage("Please, select one of 'Equip/Line/Tag'.", "Warning!");
                    isData = false;
                    return;
                }

                if (cboInsEquLinTag.SelectedIndex > -1)
                {
                    tddTEmp.ELTData = ((QaqcformDTO)cboInsEquLinTag.SelectedItem).KeyValue.ToString();
                    tddTEmp.ELTDQaqcFormID = ((QaqcformDTO)cboInsEquLinTag.SelectedItem).QAQCFormID.ToString();
                    isData = true;
                }
                else
                {
                    AlertMessage("Please, select one of 'Equip/Line/Tag'.", "Warning!");
                    isData = false;
                    return;
                }

                if (cboInsCategory.SelectedIndex > -1)
                {
                    tddTEmp.Category = ((RevealCommonSvc.ComboBoxDTO)cboInsCategory.SelectedItem).DataID.ToString();
                    tddTEmp.CategoryName = ((RevealCommonSvc.ComboBoxDTO)cboInsCategory.SelectedItem).DataName;
                    isData = true;
                }
                else
                {
                    AlertMessage("Please, select one of Categories.", "Warning!");
                    isData = false;
                    return;
                }

                if (!string.IsNullOrEmpty(txtInsDescription.Text))
                {
                    tddTEmp.Description = txtInsDescription.Text;
                    isData = true;
                }
                else
                {
                    AlertMessage("Please, insert description into the blank ", "Warning!");
                    isData = false;
                    return;
                }
                
                if (!string.IsNullOrEmpty(txtInsResponsible.Text))
                {
                    tddTEmp.Responsible = txtInsResponsible.Text;
                    isData = true;
                }
                #endregion

                // 1. 이미지가 존재할 때
                if (wbBitmap != null)
                {
                    if (string.IsNullOrEmpty(strSavedPath))
                    {
                        // 1-1. 이미지를 중간에 취소했을 때 : 저장하지 않고 팝업 열어 놓음
                        tddTEmp.Appendix = "";
                        tddTEmp.FileName = "";
                        popLineAdd.IsOpen = true;
                        isSaveOK = false;
                        AlertMessage("You canceled saving the image. Please clear image if you don't want to save it.", "");
                    }
                    else
                    {
                        // 1-2. 이미지를 정상저장했을 때 : 저장하고 팝업 닫음
                        tddTEmp.Appendix = strSavedPath;
                        tddTEmp.FileName = strSavedName;
                        popLineAdd.IsOpen = false;
                        isSaveOK = true;

                        if (tddTEmp.Status == 0)
                        {
                            tddTEmp.Status = (int)WinAppLibrary.Utilities.RowStatus.New;
                        }
                    }
                }
                else
                {
                    // 2. 이미지가 존재하지 않을 때 : 저장하고 팝업 닫음
                    tddTEmp.Appendix = "";
                    tddTEmp.FileName = "";
                    popLineAdd.IsOpen = false;
                    isSaveOK = true;

                    if (tddTEmp.Status == 0)
                    {
                        tddTEmp.Status = (int)WinAppLibrary.Utilities.RowStatus.New;
                    }
                }

                if (isSaveOK && isData)
                {

                    if (isNewer)
                    {
                        lstTempList.Add(tddTEmp);
                    }

                    lvPunchList.ItemsSource = null;
                    lvPunchList.ItemsSource = lstTempList;

                    imgPicture.Source = null;
                    wbBitmap = null;

                    txtInsPunchNo.Text = "";
                    txtInsDescription.Text = "";
                    txtInsResponsible.Text = "";
                    cboInsCategory.SelectedIndex = -1;

                    cboInsEquLinTag.SelectionChanged -= cboInsEquLinTag_SelectionChanged;
                    cboInsEquLinTag.SelectedIndex = -1;
                    cboInsEquLinTag.SelectionChanged += cboInsEquLinTag_SelectionChanged;

                    isNewer = true;
                }
            }
        }

        private void btnPanelCancel_Click(object sender, RoutedEventArgs e)
        {
            CheckAndClearShareOperation();

            imgPicture.Source = null;
            wbBitmap = null;
            txtInsPunchNo.Text = "";
            txtInsDescription.Text = "";
            txtInsResponsible.Text = "";
            cboInsCategory.SelectedIndex = -1;
            cboInsEquLinTag.SelectionChanged -= cboInsEquLinTag_SelectionChanged;
            cboInsEquLinTag.SelectedIndex = -1;
            cboInsEquLinTag.SelectionChanged += cboInsEquLinTag_SelectionChanged;

            grDetailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbDetailOFF.Begin();

            popLineAdd.IsOpen = false;
        }

        #endregion

        /// <summary>
        /// 팝업 처리시 부모창 모달 처리
        /// </summary>
        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbDetailOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbDetailOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(ttfDetailPanelTrans, Window.Current.Bounds.Width, ANIMATION_SPEED));
            _sbDetailOFF.Begin();
            _sbDetailON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbDetailON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(stfDetailPanelScale, 1, 0));
            _sbDetailON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(ttfDetailPanelTrans, 0, ANIMATION_SPEED));
        }

        /// <summary>
        /// 경고 메세지 처리
        /// </summary>
        /// <param name="strTitle"></param>
        /// <param name="strCaution"></param>
        private async void AlertMessage(string strTitle, string strCaution)
        {
            await Helper.OkMessage(strTitle, strCaution);
        }
        
        /// <summary>
        /// 고유 파일명 처리
        /// </summary>
        /// <returns></returns>
        private string MakeFileName()
        {
            string strFileName = string.Empty;

            DateTime dtNow = DateTime.Now;

            string strThisMonth = dtNow.Month.ToString();
            string strThisDay = dtNow.Day.ToString();
            string strThisHour = dtNow.Hour.ToString();
            string strThisMinute = dtNow.Minute.ToString();
            string strThisSecond = dtNow.Second.ToString();

            if (dtNow.Month < 10)
            {
                strThisMonth = "0" + dtNow.Month.ToString();
            }

            if (dtNow.Day < 10)
            {
                strThisDay = "0" + dtNow.Day.ToString();
            }

            if (dtNow.Minute < 10)
            {
                strThisMinute = "0" + dtNow.Minute.ToString();
            }

            if (dtNow.Second < 10)
            {
                strThisSecond = "0" + dtNow.Second.ToString();
            }

            strFileName = "FWD" + dtNow.Year.ToString() + strThisMonth + strThisDay + strThisHour + strThisMinute + strThisSecond;

            return strFileName;
        }
    }

    public class TurnOverData
    {
        public int ContractNo { get; set; }
        public string ContractName { get; set; }
        public string SystemNumber { get; set; }
        public string SystemName { get; set; }
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public int Status { get; set; }

        public TurnOverData()
        {
        }

        public TurnOverData(int m_ContNo, string m_ContNm, string m_SysNo, string m_SysNm, int m_ModNo, string m_ModNm, int m_Status)
        {
            ContractNo = m_ContNo;
            ContractName = m_ContNm;
            SystemNumber = m_SysNo;
            SystemName = m_SysNm;
            ModuleID = m_ModNo;
            ModuleName = m_ModNm;
            Status = m_Status;
        }
    }

    public class TurnOverDetailData
    {
        public string PunchNo { get; set; }
        public string ELTData { get; set; }
        public string ELTDQaqcFormID { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public string Responsible { get; set; }
        public string Appendix { get; set; }
        public string FileName { get; set; }
        public int Status { get; set; }

        public TurnOverDetailData()
        {
        }

        public TurnOverDetailData(string m_PunchNo, string m_EltData, int m_EltDataID, string m_Descript, int m_Cate, string m_CateNm, string m_Respon, string m_Apped, string m_FileNm, int m_Status)
        {
            PunchNo = m_PunchNo;
            ELTData = m_EltData;
            ELTDQaqcFormID = m_EltDataID.ToString();
            Description = m_Descript;
            Category = m_Cate.ToString();
            CategoryName = m_CateNm;
            Responsible = m_Respon;
            Appendix = m_Apped;
            FileName = m_FileNm;
            Status = m_Status;
        }
    }

    public class TurnOverDrawing
    {
        public int ProjectID { get; set; }
        public string DrawingName { get; set; }
        public string DrawingFilePath { get; set; }
        public string DrawingFileURL { get; set; }
        public string DrawingNo { get; set; }

        public TurnOverDrawing()
        {
        }

        public TurnOverDrawing(int m_ProjID, string m_DrawingNm, string m_DrawingPath, string m_DrawingFileUrl, string m_DrawingNo)
        {
            ProjectID = m_ProjID;
            DrawingName = m_DrawingNm;
            DrawingFilePath = m_DrawingPath;
            DrawingFileURL = m_DrawingFileUrl;
            DrawingNo = m_DrawingNo;
        }
    }

    public class DownloadedImage
    {
        public Image DownedImage { get; set; }
        public Uri ImagePath { get; set; }

        public DownloadedImage()
        {
        }

        public DownloadedImage(Image m_DownedImage, Uri m_ImagePath)
        {
            DownedImage = m_DownedImage;
            ImagePath = m_ImagePath;
        }
    }
}