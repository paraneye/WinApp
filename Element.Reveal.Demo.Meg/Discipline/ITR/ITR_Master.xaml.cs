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
using Windows.UI.Core;
using Windows.Networking.Proximity;
using System.Threading.Tasks;
using System.Text;
using Element.Reveal.Meg.RevealProjectSvc;
using Element.Reveal.Meg.Lib;
using WinAppLibrary.UI;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.ITR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ITR_Master : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid, _departmentid;
        private string _savedFileName = "123.xml";


        private bool hasUpdateGrid = false;
        private CControlSerializer Serializer;
        private CDocRepository DocRepository;
        private QAQCDoc Doc;
        private Windows.Storage.StorageFolder BaseFolder;
        private WinAppLibrary.Utilities.Helper Helper;
        private IItrDoc ucDoc;
        List<QaqcformtemplateDTO> _ofiles = new List<QaqcformtemplateDTO>();

        string[] arrParameter = new string[2];
        Lib.ProximityHandler ProximityHandler;
        private bool _onHandling = true;
        string _pinno = "1234";
        private new List<WinAppLibrary.UI.ObjectNFCSign> signed;

        private QaqcformDTO _qaqcForm = new QaqcformDTO();
        private List<QaqcformdetailDTO> _qaqcFormDetails = new List<QaqcformdetailDTO>();

        public ITR_Master()
        {
            this.InitializeComponent();
            Serializer = new CControlSerializer();
            DocRepository = new CDocRepository(Windows.Storage.ApplicationData.Current.LocalFolder, "FiwpItrList.xml");
            Doc = new QAQCDoc();

            BaseFolder = Lib.ContentPath.OffModeUserFolder;
            Helper = new WinAppLibrary.Utilities.Helper();

            ProximityHandler = new Lib.ProximityHandler();
            ProximityHandler.OnException += ProximityHandler_OnException;
            ProximityHandler.OnMessage += ProximityHandler_OnMessage;

            lvNFCSignList.SelectionChanged += lvNFCSignList_SelectionChanged;
        }



        void lvNFCSignList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvNFCSignList.SelectedItems.Count > 0)
            {
                //UC에 Sign이 있다면, 선택 해제.
                ucDoc.ClearSelect();
            }
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            ProximityHandler.SetProximityDevice(ProximityDevice.GetDefault());

            Login.MasterPage.DoBeforeBack += MasterPage_DoBeforeBack;
            Login.MasterPage.ShowBackButton(true);

            //arrParameter = navigationParameter.ToString().Split('_');
            Dictionary<string, Object> nParams = (Dictionary<string, Object>)navigationParameter;
            arrParameter = ((string)nParams["filename"]).Split('_');
            _departmentid = (int)nParams["departmentid"];

            if (_departmentid == WinAppLibrary.Utilities.Department.Foreman)
            {
                btnSave.Visibility = Visibility.Visible;
                btnApprove.Visibility = Visibility.Collapsed;
                btnReject.Visibility = Visibility.Collapsed;
            }

            if (_departmentid == WinAppLibrary.Utilities.Department.QualityManagement)
            {
                btnSave.Visibility = Visibility.Collapsed;
                btnApprove.Visibility = Visibility.Visible;
                btnReject.Visibility = Visibility.Visible;
            }

            if (arrParameter.Length > 1)
            {
                LoadControls(arrParameter[1].Replace(".xml", ""));

            }
            LoadDoc((string)nParams["filename"]);
            _onHandling = false;

            btnSave.Click += btnSave_Click;
            btnApprove.Click += btnApprove_Click;
            btnReject.Click += btnReject_Click;
        }

        void MasterPage_DoBeforeBack(object sender, object e)
        {
            //뒤로 가기 눌렀을 경우 화면에 변경된 내역이 있는지 확인
            CheckDef();
        }

        private async void CheckDef()
        {
            bool bcheck = true;
            int i = 0;

            try
            {
                bool blLoaded = await Doc.Load(BaseFolder, _savedFileName);
                if (blLoaded)
                {
                    _qaqcFormDetails = (from q in Doc.DTO.QaqcfromDetails where q.InspectionLUID != QAQCGroup.Grid && q.InspectionLUID != QAQCGroup.Header select q).ToList<QaqcformdetailDTO>();

                    ucDoc.DoAfter(Doc.DTO);
                    ucDoc.Save();
                }

                if (_qaqcFormDetails != null && ucDoc.QAQCDTOList != null & _qaqcFormDetails.Count > 0 && ucDoc.QAQCDTOList.Count > 0)
                {
                    if (_qaqcFormDetails.Count == ucDoc.QAQCDTOList.Count)
                    {
                        _qaqcFormDetails = _qaqcFormDetails.OrderBy(x => x.InspectionLUID).ThenBy(y => y.InspectedValue).ToList();
                        ucDoc.QAQCDTOList = ucDoc.QAQCDTOList.OrderBy(x => x.InspectionLUID).ThenBy(y => y.InspectedValue).ToList();

                        foreach (QaqcformdetailDTO dtos in ucDoc.QAQCDTOList)
                        {
                            if (!((dtos.StringValue1 == _qaqcFormDetails[i].StringValue1) &&
                                 (dtos.StringValue2 == _qaqcFormDetails[i].StringValue2) &&
                                 (dtos.StringValue3 == _qaqcFormDetails[i].StringValue3) &&
                                 (dtos.StringValue4 == _qaqcFormDetails[i].StringValue4) &&
                                 (dtos.StringValue5 == _qaqcFormDetails[i].StringValue5) &&
                                 (dtos.StringValue6 == _qaqcFormDetails[i].StringValue6) &&
                                 (dtos.StringValue7 == _qaqcFormDetails[i].StringValue7) &&
                                 (dtos.StringValue8 == _qaqcFormDetails[i].StringValue8) &&
                                 (dtos.StringValue9 == _qaqcFormDetails[i].StringValue9) &&
                                 (dtos.StringValue10 == _qaqcFormDetails[i].StringValue10) &&
                                 (dtos.StringValue11 == _qaqcFormDetails[i].StringValue11) &&
                                 (dtos.StringValue12 == _qaqcFormDetails[i].StringValue12) &&
                                 (dtos.StringValue13 == _qaqcFormDetails[i].StringValue13) &&
                                 (dtos.StringValue14 == _qaqcFormDetails[i].StringValue14) &&
                                 (dtos.StringValue15 == _qaqcFormDetails[i].StringValue15) &&
                                 (dtos.StringValue16 == _qaqcFormDetails[i].StringValue16) &&
                                 (dtos.StringValue17 == _qaqcFormDetails[i].StringValue17)
                                ))
                            {
                                bcheck = false;
                            }

                            i = i + 1;
                        }
                    }
                    else
                    {
                        bcheck = false;
                    }

                    if (bcheck == false && await WinAppLibrary.Utilities.Helper.YesOrNoMessage("Contents Changed. Save & Exit?", "Save Confirm"))
                    {
                        SaveDoc();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            string param = "1";
            if (_departmentid == WinAppLibrary.Utilities.Department.Foreman)
            {
                param = "1";
            }
            else if (_departmentid == WinAppLibrary.Utilities.Department.QualityManagement)
            {
                param = "3";
            }
            this.Frame.Navigate(typeof(FillOutSubmitITR), param);
            Login.MasterPage.DoBeforeBack -= MasterPage_DoBeforeBack;
        }

        private void LoadControls(string ucName)
        {
            //var doc = DocRepository.GetDoc(_data["itrkey"].ToString());

            /*
            switch(doc["Key"]){
                case "1" :
                    ucDoc= new UCControlCableInspectionExhibit();

            };
            */
            //ucDoc = (IItrDoc)(new UCPowerCableReelReceivingExhibit());

            /*
            ucDoc = (IItrDoc)(new UCMIInspectionReport());
            Grid.SetColumn((FrameworkElement)ucDoc, 0);
            BaseGrid.Width = ((FrameworkElement)ucDoc).Width + 260;
            BaseGrid.Children.Add((FrameworkElement)ucDoc);
            btnSave.Click += btnSave_Click;

            //Signed 정보를 가져와 Binding 한다.
            lvNFCSignList.DataContext = "";
            */
            switch (ucName)
            {
                case Lib.ITRList.CableTrayInspection:
                    ucDoc = (IItrDoc)(new UCCableTrayInspection());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.CableTrayInspectionTitle);
                    break;
                case Lib.ITRList.LightingDeviceInstallation:
                    ucDoc = (IItrDoc)(new UCLightingandDeviceInstallation());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.LightingDeviceInstallationTitle);
                    break;
                case Lib.ITRList.LightingDeviceCircuit:
                    ucDoc = (IItrDoc)(new UCLightingAndDeviceCircuit());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.LightingDeviceCircuitTitle);
                    break;
                case Lib.ITRList.PowerCableReelReceiving:
                    ucDoc = (IItrDoc)(new UCPowerCableReelReceivingExhibit());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.PowerCableReelReceivingTitle);
                    break;
                case Lib.ITRList.ControlCableReelReciving:
                    ucDoc = (IItrDoc)(new UCControlCableReelReceivingExhibit());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.ControlCableReelRecivingTitle);
                    break;
                case Lib.ITRList.InstrumentCableReelReceiving:
                    ucDoc = (IItrDoc)(new UCInstrumentCableReelReceivingExhibit());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.InstrumentCableReelReceivingTitle);
                    break;
                case Lib.ITRList.PowerCableInspection:
                    ucDoc = (IItrDoc)(new UCPowerCableInspectionExhibit());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.PowerCableInspectionTitle);
                    break;
                case Lib.ITRList.ControlCableInspection:
                    ucDoc = (IItrDoc)(new UCControlCableInspectionExhibit());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.ControlCableInspectionTitle);
                    break;
                case Lib.ITRList.InstrumentCableInspection:
                    ucDoc = (IItrDoc)(new UCInstrumentCableInspectionExhibit());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.InstrumentCableInspectionTitle);
                    break;
                case Lib.ITRList.MIReceiving:
                    hasUpdateGrid = true;
                    ucDoc = (IItrDoc)(new UCMIReceivingReport());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.MIReceivingTitle);
                    break;
                case Lib.ITRList.SRPLReceiving:
                    ucDoc = (IItrDoc)(new UCSR_PLReceivingReport());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.SRPLReceivingTitle);
                    break;
                case Lib.ITRList.MIInspection:
                    ucDoc = (IItrDoc)(new UCMIInspectionReport());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.MIInspectionTitle);
                    break;
                case Lib.ITRList.SRPLInspection:
                    ucDoc = (IItrDoc)(new UCSR_PLInspectionReport());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.SRPLInspectionTitle);
                    break;

                case Lib.ITRList.INSTRUMENTATIONReceiving:
                    ucDoc = (IItrDoc)(new UCInstrumentReceivingReport());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.INSTRUMENTATIONReceiving);
                    break;
                case Lib.ITRList.INSTRUMENTATIONInstallation:
                    ucDoc = (IItrDoc)(new UCInstrumentInstallationInspection());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.INSTRUMENTATIONInstallation);
                    break;
                case Lib.ITRList.INSTRUMENTATIONCalibration:
                    ucDoc = (IItrDoc)(new UCInstrumentCalibrationRecord());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.INSTRUMENTATIONCalibration);
                    break;
                case Lib.ITRList.PIPEPressureTestRecord:
                    ucDoc = (IItrDoc)(new UCPipingPressureTestRecord());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.PIPEPressureTestRecord);
                    break;
                case Lib.ITRList.PipeCleaningTravel:
                    ucDoc = (IItrDoc)(new UCPipeCleaning());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.PipeCleaningTravel);
                    break;
                case Lib.ITRList.BoltTorquing:
                    ucDoc = (IItrDoc)(new UCTensioningTravel());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.BoltTorquing);
                    break;
                case Lib.ITRList.PIPAnsa:
                    ucDoc = (IItrDoc)(new UCAbsaTestDataReport());
                    Login.MasterPage.SetPageTitle(Lib.UcTitle.PIPAnsa);
                    break;
            }
            ucDoc.SelectedSign += ucDoc_SelectedSign;
            Grid.SetColumn((FrameworkElement)ucDoc, 0);
            BaseGrid.Width = ((FrameworkElement)ucDoc).Width + 280;
            BaseGrid.Children.Add((FrameworkElement)ucDoc);
        }

        void ucDoc_SelectedSign(object sender, EventArgs e)
        {
            lvNFCSignList.SelectedItem = null;
        }

        private async void LoadDoc(string fileName)
        {
            _savedFileName = fileName; //_data["localSavedFileName"].ToString();
            bool blLoaded = await Doc.Load(BaseFolder, _savedFileName);
            if (blLoaded)
            {
                ucDoc.DoAfter(Doc.DTO);
                ucDoc.QAQCDTOList = (from q in Doc.DTO.QaqcfromDetails where q.InspectionLUID != QAQCGroup.Grid && q.InspectionLUID != QAQCGroup.Header select q).ToList<QaqcformdetailDTO>();
                _qaqcFormDetails = ucDoc.QAQCDTOList;
                ucDoc.Load();

                //Doc.Status
                if (_departmentid == WinAppLibrary.Utilities.Department.QualityManagement)
                {
                    Doc.Status = EStatusType.Saved;
                }
                else
                {
                    _ofiles = new List<QaqcformtemplateDTO>(await LoadToQaqcformtemplate());
                    if (_ofiles == null)
                    {
                        return;
                    }

                    for (int i = 0; i < _ofiles.Count; i++)
                    {
                        if (_ofiles[i].QAQCFormTemplateID == Convert.ToInt32(arrParameter[0]))
                        {
                            Doc.Status = _ofiles[i].QAQCFormCode == "1" ? EStatusType.Downloaded : _ofiles[i].QAQCFormCode == "2" ? EStatusType.Saved : EStatusType.ReadyToSubmit;
                        }
                    }
                }

                signed = new List<WinAppLibrary.UI.ObjectNFCSign>
                {
                    new WinAppLibrary.UI.ObjectNFCSign{
                        isSigned = Doc.DTO.TechnicianSignOffBy != null ? "Signed" : "Unsigned",
                        MemberGrade = "Foreman",
                        PersonnelName = Doc.DTO.TechnicianSignOffBy,
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                        SignedTime = Doc.DTO.TechnicianSignOffBy == null ? "" : Doc.DTO.TechnicianOffDate != null ? Doc.DTO.TechnicianOffDate.ToString() : ""
                    },
                    new WinAppLibrary.UI.ObjectNFCSign{
                        isSigned = Doc.DTO.ContractorSignOffBy != null ? "Signed" : "Unsigned",
                        MemberGrade = "QC Manager",
                        PersonnelName = Doc.DTO.ContractorSignOffBy,
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                        SignedTime = Doc.DTO.ContractorSignOffBy == null ? "" : Doc.DTO.ContractorSignOffDate != null ? Doc.DTO.ContractorSignOffDate.ToString() : ""
                    },
                };
                lvNFCSignList.DataContext = signed;
            }
            else
            {
                Helper.ExceptionHandler(new Exception(), "LoadDoc()", "Load from LocalStorage Error!", "Error");
            }

            //    FrameworkElement ctl = FindControlType(BaseGrid, typeof(WinAppLibrary.UI.UCNFCSign), null);
        }



        /*
        private async void Submit22() {
            //public async Task<List<RevealProjectSvc.QaqcformDTO>> SaveQaqcformForSubmit(List<RevealProjectSvc.QaqcformDTO> qaqcforms)
            List<RevealProjectSvc.QaqcformDTO> save = new List<RevealProjectSvc.QaqcformDTO>();
            save.Add(Doc.DTO);
            List<RevealProjectSvc.QaqcformDTO> result = await (new Lib.ServiceModel.ProjectModel()).SaveQaqcformForSubmit(save);
            
        }*/

        #region "Event Handler"
        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_departmentid == WinAppLibrary.Utilities.Department.Foreman)
            {
                Doc.DTO.PartialDate = DateTime.Now;
            }

            SaveDoc();

            if (Doc.Status == EStatusType.ReadyToSubmit)
                Submit("Save");
        }

        void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (_departmentid == WinAppLibrary.Utilities.Department.QualityManagement)
            {
                Doc.DTO.ApprovalStatusLUID = WinAppLibrary.Utilities.QAQCApprovalStatus.QCManagerApprove;
            }

            SaveDoc();

            if (Doc.Status == EStatusType.ReadyToSubmit)
                Submit("Approve");
        }

        void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (_departmentid == WinAppLibrary.Utilities.Department.QualityManagement)
            {
                Doc.DTO.ApprovalStatusLUID = WinAppLibrary.Utilities.QAQCApprovalStatus.QCManagerReject;
            }

            SaveDoc();

            if (Doc.Status == EStatusType.ReadyToSubmit)
                Submit("Reject");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        private void lvNFCSignList_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
            {
                if (lvNFCSignList.SelectedItems.Count == 0) return;
                signed[lvNFCSignList.SelectedIndex] = new WinAppLibrary.UI.ObjectNFCSign
                {
                    isSigned = "UnSigned",
                    PersonnelName = "",
                    MemberGrade = "MM",
                    SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                    SignedTime = ""
                };
                delsign(lvNFCSignList.SelectedIndex);
            }
        }

        // QC Manager가 Foreman의 sign을 삭제할 권한을 줘야 하는지 문의
        private async void delsign(int index)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //lvNFCSignList.DataContext = signed;

                WinAppLibrary.UI.ObjectNFCSign tmp = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[index == 0 ? 1 : 0];
                switch (index)
                {
                    case 0:
                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = tmp.isSigned,
                                    PersonnelName = tmp.PersonnelName,
                                    MemberGrade = "Foreman",
                                    SignedColor = tmp.SignedColor,
                                    SignedTime = tmp.SignedTime
                                    } ,
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = "UnSigned",
                                    PersonnelName = "",
                                    MemberGrade = "QC Manager",
                                    SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                    SignedTime = ""
                                    }
                                };
                        break;
                    case 1:
                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = "UnSigned",
                                    PersonnelName = "",
                                    MemberGrade = "Foreman",
                                    SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                    SignedTime = ""
                                    } ,
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = tmp.isSigned,
                                    PersonnelName = tmp.PersonnelName,
                                    MemberGrade = "QC Manager",
                                    SignedColor = tmp.SignedColor,
                                    SignedTime =  tmp.SignedTime
                                    }
                                };
                        break;
                }
                lvNFCSignList.DataContext = signed;
            });
        }

        #endregion

        private void Approval(string status)
        {
            if (_qaqcForm == null || _qaqcForm.QAQCFormID <= 0)
            {
                _qaqcForm.ApprovalStatus = "";
            }
        }

        #region "Private Method"
        //사용자가 입력한 데이터의 저장에 대한 처리
        private async void SaveDoc()
        {
            bool saveresult = false;
            if (Doc.Status != EStatusType.ReadyToSubmit)
            {
                if (_departmentid == WinAppLibrary.Utilities.Department.Foreman)
                {
                    NotifyMessage("If you are offline, This will be saved in the Submit ITR Hub and can be submitted when you are online. If you are online, this will be uploaded to Sharepoint.", "");
                }
                ucDoc.Save();

                bool blSaved = await Doc.Save(ucDoc.QAQCDTOList, BaseFolder, _savedFileName, (hasUpdateGrid) ? ((UCMIReceivingReport)ucDoc).UpdateGrid : null);
                if (blSaved)
                {
                    if (_ofiles == null)
                    {
                        return;
                    }
                    for (int i = 0; i < _ofiles.Count; i++)
                    {
                        if (_ofiles[i].QAQCFormTemplateID == Convert.ToInt32(arrParameter[0]))
                            _ofiles[i].QAQCFormCode = "2";
                    }
                    saveresult = await SaveToQaqcformtemplate(_ofiles, BaseFolder, Lib.ITRList.DownloadList);
                    //WinAppLibrary.Utilities.Helper.SimpleMessage("Save Complete", "Saved!");
                    if (saveresult == false)
                    {
                        MessageHide("SaveDoc Failed", "SaveDoc Fail");
                    }
                }
                else
                {
                    Helper.ExceptionHandler(new Exception(), "SaveDoc()", "Save to LocalStorage Error!", "Error");
                    //WinAppLibrary.Utilities.Helper.SimpleMessage("Save Fail", "SaveFaild!");
                    MessageHide("SaveDoc Failed", "SaveDoc Fail");
                }
            }
            else
            {
                this.NotifyMessage("Cannot Save this Doc. on ready to submit status", "Caution!");
            }
        }

        public async Task<List<QaqcformtemplateDTO>> LoadToQaqcformtemplate()
        {
            List<QaqcformtemplateDTO> filedto = new List<QaqcformtemplateDTO>();
            try
            {
                if (BaseFolder.GetFileAsync(Lib.ITRList.DownloadList) != null)
                {
                    WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
                    var stream = await helper.GetFileStream(BaseFolder, Lib.ITRList.DownloadList);
                    filedto = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealProjectSvc.QaqcformtemplateDTO>>(stream);
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("File Download First!", "Error");
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadToQaqcformtemplate");
                throw e;
            }

            return filedto;
        }


        public async Task<bool> SaveToQaqcformtemplate(List<QaqcformtemplateDTO> dto, Windows.Storage.StorageFolder _path, string _filename)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                var xmlstream = FormSerialize.EncryptHashSerializeTo<List<QaqcformtemplateDTO>>(dto);
                retValue = await helper.SaveFileStream(_path, _filename, xmlstream);

            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "SaveToQaqcformtemplate");
                throw e;
            }

            return retValue;
        }

        /// <summary>
        /// NFC Sign에 대한 처리
        /// </summary>
        private void CheckReadyToSubmit()
        {
            // if (!ucDoc.Validate())
            // {
            //TODO : Validate Error or Upate Status Error
            // return;
            // }
            Doc.Status = EStatusType.ReadyToSubmit;

            //Doc.ExtraData를 갱신 -> 저장

        }

        private void UndoReadyToSubmit()
        {
            Doc.Status = EStatusType.Saved;
            //Doc.ExtraData를 갱신 -> 저장
        }

        private async void Submit(string action)
        {
            //TODO : Online Check!
            //Login.LoginMode == WinAppLibrary.UI.LogMode.
            //if (_departmentid == WinAppLibrary.Utilities.Department.Foreman)
            //{
            //    NotifyMessage("If you are offline, This will be saved in the Submit ITR Hub and can be submitted when you are online. If you are online, this will be uploaded to Sharepoint.", "");
            //}
            string param = "1";
            if (_departmentid == WinAppLibrary.Utilities.Department.Foreman)
            {
                param = "1";
            }
            else if (_departmentid == WinAppLibrary.Utilities.Department.QualityManagement)
            {
                param = "3";
            }

            switch (Login.LoginMode)
            {
                case WinAppLibrary.UI.LogMode.OnMode:
                    Doc.DTO.UpdatedBy = Login.UserAccount.UserName;
                    if (await Doc.Submit())
                    {
                        bool res = await DeleteDoc();
                        if (res)
                        {
                            MessageHide(action + " completed!", action + " Complete");
                            this.Frame.Navigate(typeof(FillOutSubmitITR), param);
                        }
                        else
                            MessageHide("Submit for " + action + " Failed!", "Submit for " + action + "Submit Failed");

                    }
                    else
                        MessageHide("Submit for " + action + " Failed!", "Submit for " + action + "Submit Failed");
                    break;
                case WinAppLibrary.UI.LogMode.OffMode:
                    if (_ofiles == null)
                    {
                        return;
                    }
                    for (int i = 0; i < _ofiles.Count; i++)
                    {
                        if (_ofiles[i].QAQCFormTemplateID == Convert.ToInt32(arrParameter[0]))
                            _ofiles[i].QAQCFormCode = "3";
                    }
                    await SaveToQaqcformtemplate(_ofiles, BaseFolder, Lib.ITRList.DownloadList);

                    this.Frame.Navigate(typeof(FillOutSubmitITR), param);
                    break;
            }
        }

        private async Task<bool> DeleteDoc()
        {
            for (int i = 0; i < _ofiles.Count; i++)
            {
                if (_ofiles[i].QAQCFormTemplateID == Convert.ToInt32(arrParameter[0]))
                    _ofiles.RemoveAt(i);
            }
            bool r1 = await SaveToQaqcformtemplate(_ofiles, BaseFolder, Lib.ITRList.DownloadList);
            if (r1)
                r1 = await Doc.Delete(BaseFolder, _savedFileName);

            return r1;
        }

        public async void NotifyMessage(string msg, string title)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //    MessageDialog.DialogTitle = title;
                //    MessageDialog.Content = msg;
                //    MessageDialog.Show(this);
            });
        }

        public async void MessageHide(string message, string title)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _onHandling = false;
                MessageDialog.Hide(this);
                WinAppLibrary.Utilities.Helper.SimpleMessage(message, title);
            });

        }

        #endregion

        #region "ProximityHandler"

        private void ProximityHandler_OnException(object sender, object e)
        {
            // uiSlideButton.Hide();
            //  Loading(false);
        }

        private async void log(NotifyType type)
        {
            Stream s = Doc.EncryptHashSerializeToLog<NotifyType>(type);
            bool x = await (new WinAppLibrary.Utilities.Helper()).SaveFileStream(BaseFolder, "ddd.log", s);
        }

        private async void log(string s)
        {

            bool x = await (new WinAppLibrary.Utilities.Helper()).SaveFileStream(BaseFolder, "ddd.log", new MemoryStream(Encoding.UTF8.GetBytes(s)));
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
                        if (ucDoc.isExistNFC)
                        {

                            bool blsign = IsSelectedSign();
                            if (blsign)
                            {
                                SetNFCData(personname, "MM", DateTime.Now.ToString(), _pinno);
                                _onHandling = false;
                                return;
                            }
                        }
                        bool result = ControlValidate();
                        if (!result)
                        {
                            _onHandling = false;
                            return;
                        }

                        Setsigne(personId, personname, _pinno);
                        Doc.Status = EStatusType.ReadyToSubmit;
                        Doc.ReadyToSubmit();

                        _onHandling = false;
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ucDoc.SetNFCData(personname, "MM");
                //    ((UCInstrumentCableReelReceivingExhibit)ucDoc).SetNFCData(personname, "MM");
            });
        }

        private FrameworkElement FindControlType(FrameworkElement _reference, Type _findType, FrameworkElement _exist)
        {
            if (_reference != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(_reference);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(_reference, i);
                    if (_findType == ((FrameworkElement)child).GetType())
                        _exist = (FrameworkElement)child;
                    else
                    {
                        if (_exist == null)
                            _exist = FindControlType((FrameworkElement)child, _findType, _exist);
                    }
                }
            }
            if (_exist == null)
            {
                return null;
            }
            else
            {
                return _exist;
            }
        }

        private async void SetSignedInControl(WinAppLibrary.UI.UCNFCSign ctl, string _pinno, string personname)
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ctl.DataContext = new WinAppLibrary.UI.ObjectNFCSign
                    {
                        SignedPinNumber = _pinno,
                        PersonnelName = personname,
                        SignedTime = DateTime.Now.ToString(),
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen)
                    };
                });
            }
            catch
            {
            }
        }

        private async void Setsigne(int personId, string personname, string _pinno)
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    WinAppLibrary.UI.ObjectNFCSign tmp = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[lvNFCSignList.SelectedIndex == 0 ? 1 : 0];
                    if (lvNFCSignList.SelectedIndex == 0)
                    {
                        Doc.DTO.TechnicianSignOffBy = personname;
                        Doc.DTO.TechnicianOffDate = DateTime.Now;
                        Doc.DTO.TechnicianTitle = _pinno;

                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                            new WinAppLibrary.UI.ObjectNFCSign{
                            isSigned = "Signed",
                            PersonnelName = personname,
                            MemberGrade = "Foreman",
                            SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                            SignedTime = DateTime.Now.ToString()
                            } ,
                            new WinAppLibrary.UI.ObjectNFCSign{
                            isSigned = tmp.isSigned,
                            PersonnelName = tmp.PersonnelName,
                            MemberGrade = "QC Manager",
                            SignedColor = tmp.SignedColor,
                            SignedTime =  tmp.SignedTime
                            }
                        };
                    }
                    else
                    {
                        Doc.DTO.ContractorSignOffBy = personname;
                        Doc.DTO.ContractorSignOffDate = DateTime.Now;
                        Doc.DTO.ContractorTitle = _pinno;

                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                            new WinAppLibrary.UI.ObjectNFCSign{
                            isSigned = tmp.isSigned,
                            PersonnelName = tmp.PersonnelName,
                            MemberGrade = "Foreman",
                            SignedColor = tmp.SignedColor,
                            SignedTime = tmp.SignedTime
                            } ,
                            new WinAppLibrary.UI.ObjectNFCSign{
                            isSigned = "Signed",
                            PersonnelName = personname,
                            MemberGrade = "QC Manager",
                            SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                            SignedTime = DateTime.Now.ToString()
                            }
                        };
                    }
                });

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ucDoc.Save();
                });


                bool blSaved = await Doc.Save(ucDoc.QAQCDTOList, BaseFolder, _savedFileName, (hasUpdateGrid) ? ((UCMIReceivingReport)ucDoc).UpdateGrid : null);
                if (!blSaved) return;


                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    lvNFCSignList.DataContext = signed;
                });

                if (_ofiles == null)
                {
                    return;
                }
                for (int i = 0; i < _ofiles.Count; i++)
                {
                    if (_ofiles[i].QAQCFormTemplateID == Convert.ToInt32(arrParameter[0]))
                        _ofiles[i].QAQCFormCode = "3";
                }
                await SaveToQaqcformtemplate(_ofiles, BaseFolder, Lib.ITRList.DownloadList);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "AssignCrew");
                this.NotifyMessage("We had a problem to update signing. Please contact Administrator", "Error!");
                _onHandling = false;
            }
        }

        #endregion "ProximityHandler"
    }
}
