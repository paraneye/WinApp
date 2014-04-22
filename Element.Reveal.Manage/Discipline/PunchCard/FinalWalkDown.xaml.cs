using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WinAppLibrary.UI;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Element.Reveal.Manage.Discipline.ITR;
using Element.Reveal.Manage.Lib;
using Element.Reveal.Manage.RevealCommonSvc;
using Element.Reveal.Manage.RevealProjectSvc;
using WinAppLibrary.Utilities;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Manage.Discipline.PunchCard
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

        public FinalWalkDown()
        {
            this.InitializeComponent();

            Login.MasterPage.ShowTopBanner = true;
            //Login.MasterPage.ShowBackButton = true;
            Login.MasterPage.ShowBackButton = false;
            Login.MasterPage.SetPageTitle("Final Walk Down");

            btnOpenPopUp.Content = "NO DATA";

            btnAdd.Click += btnAdd_Click;
            btnCapture.Click += btnCapture_Click;
            btnLoad.Click += btnLoad_Click;
            btnCancel.Click += btnCancel_Click;

            cboContractor.SelectionChanged += cboContractor_SelectionChanged;
            cboSystemNo.SelectionChanged += cboSystemNo_SelectionChanged;
            btnOpenPopUp.Click += btnOpenPopUp_Click;
            btnPanelCancel.Click += btnPanelCancel_Click;
            lvDisciplines.SelectionChanged += lvDisciplines_SelectionChanged;
            lvPunchList.SelectionChanged += lvPunchList_SelectionChanged;

            btnSave.Click += btnSave_Click;

            ProximityHandler = new Lib.ProximityHandler();
            ProximityHandler.OnException += ProximityHandler_OnException;
            ProximityHandler.OnMessage += ProximityHandler_OnMessage;

            LoadData();
        }
                
        private async void LoadData()
        {
            txtOriginator.Text = Login.UserAccount.UserName;

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
        }

        #region ComboBox Event

        private async void cboContractor_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private async void cboSystemNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSystemNo.SelectedIndex > 0)
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
                                   select new TurnOverDetailData { PunchNo = To.InspectedKeyValue, 
                                                                   ELTData = To.StringValue4, 
                                                                   ELTDQaqcFormID = To.StringValue5, 
                                                                   Description = To.StringValue6, 
                                                                   Category = To.StringValue7, 
                                                                   CategoryName = To.StringValue8, 
                                                                   Responsible = To.StringValue9, 
                                                                   FileName = To.StringValue10,
                                   }).ToList <TurnOverDetailData>();

                    // lvPunchList.ItemsSource = lstTempList;
                    // For Demo
                    lvPunchList.ItemsSource = null;
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

                    foreach(var obj in cboInsEquLinTag.Items)
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
                    cboInsEquLinTag.SelectionChanged += cboInsEquLinTag_SelectionChanged;
                    
                    popLineAdd.HorizontalOffset = Window.Current.Bounds.Width - 850;

                    popLineAdd.IsOpen = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = e.Parameter as ShareTargetActivatedEventArgs;

            if (args != null)
            {
                soShareOp = args.ShareOperation;

                if (soShareOp.Data.Contains(StandardDataFormats.Bitmap))
                {
                    rasrReference = await soShareOp.Data.GetBitmapAsync();
                    await ProcessBitmap();
                }
                else if (soShareOp.Data.Contains(StandardDataFormats.StorageItems))
                {
                    iroReadonlyList = await soShareOp.Data.GetStorageItemsAsync();
                    await ProcessStorageItems();
                }
                else soShareOp.ReportError("It was unable to find a valid bitmap.");
            }
        }

        #region POP UP Part

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SelectedDetail = null;

            if (lvDisciplines.Items.Count() > 0)
            {
                if (lvDisciplines.SelectedIndex > -1)
                {
                    LoadStoryBoardSwitch();

                    grDetailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    _sbDetailON.Begin();

                    btnPanelCollapse.Click += btnPanelCollapse_Click;
                    cboInsEquLinTag.SelectionChanged += cboInsEquLinTag_SelectionChanged;

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

        private void cboInsEquLinTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            if (isNewer)
            {
                intLastNo++;
            }

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

                if (cboInsEquLinTag.SelectedIndex > -1)
                {
                    tddTEmp.ELTData = ((QaqcformDTO)cboInsEquLinTag.SelectedItem).KeyValue.ToString();
                    tddTEmp.ELTDQaqcFormID = ((QaqcformDTO)cboInsEquLinTag.SelectedItem).QAQCFormID.ToString();
                    isData = true;
                }

                if (!string.IsNullOrEmpty(txtInsDescription.Text))
                {
                    tddTEmp.Description = txtInsDescription.Text;
                    isData = true;
                }

                if (cboInsCategory.SelectedIndex > -1)
                {
                    tddTEmp.Category = ((RevealCommonSvc.ComboBoxDTO)cboInsCategory.SelectedItem).DataID.ToString();
                    tddTEmp.CategoryName = ((RevealCommonSvc.ComboBoxDTO)cboInsCategory.SelectedItem).DataName;
                    isData = true;
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
                    }
                }
                else
                {
                    // 2. 이미지가 존재하지 않을 때 : 저장하고 팝업 닫음
                    tddTEmp.Appendix = "";
                    tddTEmp.FileName = "";
                    popLineAdd.IsOpen = false;
                    isSaveOK = true;
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
                    txtInsPunchNo.Text = "";
                    txtInsDescription.Text = "";
                    txtInsResponsible.Text = "";
                    cboInsCategory.SelectedIndex = -1;
                    cboInsEquLinTag.SelectedIndex = -1;

                    isNewer = true;
                }
            }
        }

        private void btnPanelCancel_Click(object sender, RoutedEventArgs e)
        {
            CheckAndClearShareOperation();

            imgPicture.Source = null;
            txtInsPunchNo.Text = "";
            txtInsDescription.Text = "";
            txtInsResponsible.Text = "";
            cboInsCategory.SelectedIndex = -1;
            cboInsEquLinTag.SelectedIndex = -1;

            grDetailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbDetailOFF.Begin();

            popLineAdd.IsOpen = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TurnoverDTO toList = new TurnoverDTO();
            List<TurnoverattendeeDTO> lstToaList = new List<TurnoverattendeeDTO>();
            List<QaqcformdetailDTO> lstQfList = new List<QaqcformdetailDTO>();

            #region 데이터 전송

            string[] strArrSysInfo = cboSystemNo.SelectedValue.ToString().Split('_');
            string strSysNm = strArrSysInfo[0];
            int intSysNo = Int32.Parse(strArrSysInfo[1]);

            //toList.OwnerID = txtOriginator.Text;
            toList.ProjectID = Login.UserAccount.CurProjectID;
            toList.ContractorID = Int32.Parse(cboContractor.SelectedValue.ToString());
            toList.SystemID = intSysNo;
            toList.FWD_Date = Convert.ToDateTime(txtDate.Text);
            // Shawn Yang : discipline 추가할 것

            // NFC카드 추가할 것.

            // 
            foreach (var lvi in lvPunchList.Items)
            {
                QaqcformdetailDTO qfdList = new QaqcformdetailDTO();
                TurnOverDetailData item = (TurnOverDetailData)lvi;

                qfdList.InspectedKeyValue = item.PunchNo;
                qfdList.StringValue4 = item.ELTData;
                qfdList.StringValue5 = item.ELTDQaqcFormID;
                qfdList.StringValue6 = item.Description;
                qfdList.StringValue7 = item.Category;
                qfdList.StringValue8 = item.CategoryName;
                qfdList.StringValue9 = item.Responsible;
                qfdList.StringValue10 = item.FileName;

                if (!string.IsNullOrEmpty(item.Appendix))
                {
                    SubmitImages(item.Appendix, item.PunchNo + ".jpg");
                }

                lstQfList.Add(qfdList);
            }

            WalkdownDTOSet wdResult = new WalkdownDTOSet();
            List<TurnoverDTO> lstToList = new List<TurnoverDTO>();

            foreach (ObjectNFCSign s in signed)
            {
                lstToaList.Add(new TurnoverattendeeDTO
                {
                    PeronnelName = s.PersonnelName,
                    SignedDate = DateTime.Parse(s.SignedTime),
                    ModuleID = Login.UserAccount.CurModuleID,
                    ProjectID = Login.UserAccount.CurProjectID
                });
            }

            wdResult.qaqcformdetailDTOS = lstQfList;
            wdResult.turnoverDTOS = lstToList;
            wdResult.turnoverattendeeDTOS = lstToaList;

            SaveResult(wdResult);            

            #endregion
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
            WalkdownDTOSet wdReturn = await (new Lib.ServiceModel.ProjectModel()).SaveQaqcformWithSharepoint(wdSet);
        }

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

        private async void AlertMessage(string strTitle, string strCaution)
        {
            await Helper.OkMessage(strTitle, strCaution);
        }

        #endregion

        #region Picture Part

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
        
        #endregion

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
            #region
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
                            personname = temptagmsg[1].ToString();

                            if (temptagmsg.Length > 2)
                            {
                                _pinno = temptagmsg[2].ToString();
                            }
                        }

                        signed.Add( 
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = "Signed",
                                PersonnelName = personname,
                                MemberGrade = "Contractor",
                                SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                SignedTime = DateTime.Now.ToString()
                            });

                        lvNFCSignList.ItemsSource = signed;
                        //if (ucDoc.isExistNFC)
                        //{

                        //    bool blsign = IsSelectedSign();
                        //    if (blsign)
                        //    {
                        //        SetNFCData(personname, "MM", DateTime.Now.ToString(), _pinno);
                        //        _onHandling = false;
                        //        return;
                        //    }
                        //}
                        //bool result = ControlValidate();
                        //if (!result)
                        //{
                        //    _onHandling = false;
                        //    return;
                        //}

                        //Setsigne(personId, personname, _pinno);
                        //Doc.Status = EStatusType.ReadyToSubmit;
                        //Doc.ReadyToSubmit();

                        //_onHandling = false;
                    }
                    catch (Exception e)
                    {
                        (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "AssignProcedure");
                        this.NotifyMessage("We had a problem to update signing. Please contact Administrator", "Error!");
                        _onHandling = false;
                    }
                }
                else
                {
                    _onHandling = false;
                    this.NotifyMessage("This tag doesn't have crew information", "Alert");
                }

            }

            #endregion
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

        private async void SetNFCData(string personname, string grade, string Date, string pin)
        {
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    ucDoc.SetNFCData(personname, "MM");
            //    //    ((UCInstrumentCableReelReceivingExhibit)ucDoc).SetNFCData(personname, "MM");
            //});
        }

        public async void NotifyMessage(string msg, string title)
        {
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    //    MessageDialog.DialogTitle = title;
            //    //    MessageDialog.Content = msg;
            //    //    MessageDialog.Show(this);
            //});
        }


        //private async void Setsigne(int personId, string personname, string _pinno)
        //{
        //    try
        //    {
        //        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //        {
        //            WinAppLibrary.UI.ObjectNFCSign tmp = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[lvNFCSignList.SelectedIndex == 0 ? 1 : 0];
        //            if (lvNFCSignList.SelectedIndex == 0)
        //            {
        //                Doc.DTO.ContractorSignOffBy = personname;
        //                Doc.DTO.ContractorSignOffDate = DateTime.Now;
        //                Doc.DTO.ContractorTitle = _pinno;

        //                signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
        //                    new WinAppLibrary.UI.ObjectNFCSign{
        //                    isSigned = "Signed",
        //                    PersonnelName = personname,
        //                    MemberGrade = "Contractor",
        //                    SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
        //                    SignedTime = DateTime.Now.ToString()
        //                    },
        //                    new WinAppLibrary.UI.ObjectNFCSign{
        //                    isSigned = tmp.isSigned,
        //                    PersonnelName = tmp.PersonnelName,
        //                    MemberGrade = "Client",
        //                    SignedColor = tmp.SignedColor,
        //                    SignedTime = tmp.SignedTime
        //                    } 
        //                };
        //            }
        //            else
        //            {
        //                Doc.DTO.ClientSignOffBy = personname;
        //                Doc.DTO.ClientSignOffDate = DateTime.Now;
        //                Doc.DTO.ClientTitle = _pinno;

        //                signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
        //                    new WinAppLibrary.UI.ObjectNFCSign{
        //                    isSigned = tmp.isSigned,
        //                    PersonnelName = tmp.PersonnelName,
        //                    MemberGrade = "Contractor",
        //                    SignedColor = tmp.SignedColor,
        //                    SignedTime =  tmp.SignedTime
        //                    },
        //                    new WinAppLibrary.UI.ObjectNFCSign{
        //                    isSigned = "Signed",
        //                    PersonnelName = personname,
        //                    MemberGrade = "Client",
        //                    SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
        //                    SignedTime = DateTime.Now.ToString()
        //                    } 
        //                };
        //            }
        //        });

        //        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //        {
        //            ucDoc.Save();
        //        });


        //        bool blSaved = await Doc.Save(ucDoc.QAQCDTOList, BaseFolder, _savedFileName, (hasUpdateGrid) ? ((UCMIReceivingReport)ucDoc).UpdateGrid : null);
        //        if (!blSaved) return;


        //        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //        {
        //            lvNFCSignList.DataContext = signed;
        //        });

        //        for (int i = 0; i < _ofiles.Count; i++)
        //        {
        //            if (_ofiles[i].QAQCFormTemplateID == Convert.ToInt32(arrParameter[0]))
        //                _ofiles[i].QAQCFormCode = "3";
        //        }
        //        await SaveToQaqcformtemplate(_ofiles, BaseFolder, Lib.ITRList.DownloadList);
        //    }
        //    catch (Exception ex)
        //    {
        //        (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "AssignCrew");
        //        this.NotifyMessage("We had a problem to update signing. Please contact Administrator", "Error!");
        //        _onHandling = false;
        //    }
        //}

    }

    public class TurnOverData
    {
        public int ContractNo { get; set; }
        public string ContractName { get; set; }
        public string SystemNumber { get; set; }
        public string SystemName { get; set; }
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }

        public TurnOverData()
        {
        }

        public TurnOverData(int m_ContNo, string m_ContNm, string m_SysNo, string m_SysNm, int m_ModNo, string m_ModNm)
        {
            ContractNo = m_ContNo;
            ContractName = m_ContNm;
            SystemNumber = m_SysNo;
            SystemName = m_SysNm;
            ModuleID = m_ModNo;
            ModuleName = m_ModNm;
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

        public TurnOverDetailData()
        {
        }

        public TurnOverDetailData(string m_PunchNo, string m_EltData, int m_EltDataID, string m_Descript, int m_Cate, string m_CateNm, string m_Respon, string m_Apped, string m_FileNm)
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
        }
    }
}
