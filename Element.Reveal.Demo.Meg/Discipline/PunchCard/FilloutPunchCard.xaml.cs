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
//
using WinAppLibrary.UI;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.PunchCard
{
    //현재 오프라인모드 방식 아님. MEG시연 후 수정 예정.
    public sealed partial class FilloutPunchCard : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid;
        Lib.DataSource.QaqcDataSource _punchTicket = new Lib.DataSource.QaqcDataSource();

        private PunchCardDoc ucDoc;
        private PunchDoc Doc;
        private List<QaqcformdetailDTO> _qaqcFormDetails = new List<QaqcformdetailDTO>();
        private PunchDTOSet WDDTO = new PunchDTOSet();

        //Sign관련
        Lib.ProximityHandler ProximityHandler;
        private bool _onHandling = true;
        string _pinno = "1234";
        private new List<WinAppLibrary.UI.ObjectNFCSign> signed;

        public FilloutPunchCard()
        {
            this.InitializeComponent();
            Doc = new PunchDoc();

            ProximityHandler = new Lib.ProximityHandler();
            ProximityHandler.OnException += ProximityHandler_OnException;
            ProximityHandler.OnMessage += ProximityHandler_OnMessage;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            ProximityHandler.SetProximityDevice(ProximityDevice.GetDefault());
            //btnSave.Visibility = Visibility.Collapsed;

            LoadDoc(navigationParameter.ToString());
            _onHandling = false;
        }

        private async void LoadDoc(string QaqcformID)
        {
            //Data있을 때 : 거르는 장치? 확인
            try
            {
                btnSave.Click += btnSave_Click;
                btnSubmit.Click += btnSubmit_Click;

                //content가 UC로 빠져 있음
                ucDoc = (PunchCardDoc)(new PunchTicket());
                Grid.SetColumn((FrameworkElement)ucDoc, 0);
                BaseGrid.Width = ((FrameworkElement)ucDoc).Width + 280;
                BaseGrid.Children.Add((FrameworkElement)ucDoc);

                await _punchTicket.GetPunchTicketByQaqcform(Convert.ToInt32(QaqcformID));
                WDDTO = _punchTicket.GetPunchTicketByQaqcform();
                Doc.DTO = WDDTO;
                Doc.QAQCDTO = Doc.DTO.qaqcformDTOS[0];
                //UC로 DTO넘김
                ucDoc.DoAfter(WDDTO);

                //Sign : Contractor Foreman / Contractor QC / MEG Coordinator / MEG QC
                signed = new List<WinAppLibrary.UI.ObjectNFCSign>
                {
                    new WinAppLibrary.UI.ObjectNFCSign{

                        isSigned = Doc.QAQCDTO.TechnicianSignOffBy != null ? "Signed" : "Unsigned",
                        MemberGrade = "GF",
                        PersonnelName = Doc.QAQCDTO.TechnicianSignOffBy,
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                        SignedTime = Doc.QAQCDTO.TechnicianSignOffBy == null ? "" : Doc.QAQCDTO.TechnicianOffDate != null ? Doc.QAQCDTO.TechnicianOffDate.ToString() : ""
                    },
                    new WinAppLibrary.UI.ObjectNFCSign{

                        isSigned = Doc.QAQCDTO.ContractorSignOffBy != null ? "Signed" : "Unsigned",
                        MemberGrade = "QAQC",
                        PersonnelName = Doc.QAQCDTO.ContractorSignOffBy,
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                        SignedTime = Doc.QAQCDTO.ContractorSignOffBy == null ? "" : Doc.QAQCDTO.ContractorSignOffDate != null ? Doc.QAQCDTO.ContractorSignOffDate.ToString() : ""
                    },
                    new WinAppLibrary.UI.ObjectNFCSign{

                        isSigned = Doc.QAQCDTO.ClientSignOffBy != null ? "Signed" : "Unsigned",
                        MemberGrade = "Client QC",
                        PersonnelName = Doc.QAQCDTO.ClientSignOffBy,
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                        SignedTime = Doc.QAQCDTO.ClientSignOffBy == null ? "" : Doc.QAQCDTO.ClientSignOffDate != null ? Doc.QAQCDTO.ClientSignOffDate.ToString() : ""
                    },
                };
                lvNFCSignList.DataContext = signed;

            }
            catch (Exception ex)
            {
                //WinAppLibrary.Utilities.Helper.SimpleMessage("Signed Error: " + ex.ToString(), "Error!");
            }
        }

        #region "Event Handler"

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
            //if (_departmentid == WinAppLibrary.Utilities.Department.Foreman)
            //{
            //    Doc.DTO.qaqcformDTOS[0].PartialDate = DateTime.Now;
            //}

            //SaveDoc();

            //if (Doc.Status == EStatusType.ReadyToSubmit)
            //    Submit();
        }

        //오프라인모드 지원 X. (Save에서 별도 폼 저장 없이 입력값 저장)
        void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        protected void Save()
        {
            Login.MasterPage.Loading(true, this);

            //ucDoc.checkValidate();
            if (ucDoc.Validate())
            {
                //Sign이 있으면 Submit, Sign이 없으면 Save
                if (Doc.Status == EStatusType.ReadyToSubmit)
                {
                    //submit
                    WDDTO.qaqcformDTOS[0].UpdatedBy = Login.UserAccount.UserName;
                }
                else
                {
                    //save
                }

                ucDoc.Save();

                //RevealProjectSvc.WalkdownDTOSet _wdDTO = new RevealProjectSvc.WalkdownDTOSet();
                WDDTO.qaqcformDTOS[0].QaqcfromDetails = ucDoc.QAQCDetailDTOList;

                Submit(WDDTO);
            }
            else{
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please Enter the punch ticket", "Alert!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DownloadPunchList));
        }

        private void lvNFCSignList_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (ucDoc.Validate())
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
            else
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please Enter the punch ticket", "Alert!");
            }
        }


        #endregion

        #region "Private Method"
        //사용자가 입력한 데이터의 저장에 대한 처리 (ucDoc의 txtDescription, txtComments, txtLessons는 QaqcformDetailDTO)
        private async void Submit(PunchDTOSet _wdDTO)
        {
            bool saveresult = false;

            //WinAppLibrary.Utilities.Helper.SimpleMessage(Doc.QAQCDTO.TechnicianSignOffBy.ToString() + "|" + Doc.QAQCDTO.ContractorSignOffBy.ToString() + "|" + Doc.QAQCDTO.ClientSignOffBy.ToString(), "Submit : TechnicianSignOffBy");
            
            try
            {
                _wdDTO.qaqcformDTOS[0] = Doc.QAQCDTO;

                //WinAppLibrary.Utilities.Helper.SimpleMessage(_wdDTO.qaqcformDTOS[0].TechnicianSignOffBy, "Setsigne : personname");

                WalkdownDTOSet SaveDTO = new WalkdownDTOSet();
                SaveDTO.qaqcformDTOS = _wdDTO.qaqcformDTOS;
                SaveDTO.qaqcformDTOS[0].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                SaveDTO.qaqcformDTOS[0].QaqcfromDetails = _wdDTO.qaqcformDTOS[0].QaqcfromDetails;
                SaveDTO.qaqcformdetailDTOS = _wdDTO.qaqcformDTOS[0].QaqcfromDetails;

                saveresult = await _punchTicket.SaveQaqcformWithSharePoint(SaveDTO);

                if (saveresult)
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Save Completed", "Saved");
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Save Failed", "Failed");
            }
            catch (Exception ex)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Save Failed", "Failed");
            }
        }

        private async void delsign(int index)
        {
            //WinAppLibrary.Utilities.Helper.SimpleMessage(index.ToString(), "delsign");

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //lvNFCSignList.DataContext = signed;

                WinAppLibrary.UI.ObjectNFCSign tmp = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[0];
                WinAppLibrary.UI.ObjectNFCSign tmp2 = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[1];
                WinAppLibrary.UI.ObjectNFCSign tmp3 = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[2];

                switch (index)
                {
                    case 0:
                        Doc.QAQCDTO.TechnicianSignOffBy = "";
                        Doc.QAQCDTO.TechnicianTitle = "";
                        
                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = "UnSigned",
                                    PersonnelName = "",
                                    MemberGrade = "GF",
                                    SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                    SignedTime = ""
                                    },
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = tmp2.isSigned,
                                    PersonnelName = tmp2.PersonnelName,
                                    MemberGrade = "QAQC",
                                    SignedColor = tmp2.SignedColor,
                                    SignedTime = tmp2.SignedTime
                                    },
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = tmp3.isSigned,
                                    PersonnelName = tmp3.PersonnelName,
                                    MemberGrade = "Client QC",
                                    SignedColor = tmp3.SignedColor,
                                    SignedTime = tmp3.SignedTime
                                    }  
                                };
                        break;
                    case 1:
                        Doc.QAQCDTO.ContractorSignOffBy = "";
                        Doc.QAQCDTO.ContractorTitle = "";

                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = tmp.isSigned,
                                    PersonnelName = tmp.PersonnelName,
                                    MemberGrade = "GF",
                                    SignedColor = tmp.SignedColor,
                                    SignedTime =  tmp.SignedTime
                                    },
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = "UnSigned",
                                    PersonnelName = "",
                                    MemberGrade = "QAQC",
                                    SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                    SignedTime =  ""
                                    },
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = tmp3.isSigned,
                                    PersonnelName = tmp3.PersonnelName,
                                    MemberGrade = "Client QC",
                                    SignedColor = tmp3.SignedColor,
                                    SignedTime = tmp3.SignedTime
                                    } 
                                };
                        break;
                    case 2:
                        Doc.QAQCDTO.ClientSignOffBy = "";
                        Doc.QAQCDTO.ClientTitle = "";

                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = tmp.isSigned,
                                    PersonnelName = tmp.PersonnelName,
                                    MemberGrade = "GF",
                                    SignedColor = tmp.SignedColor,
                                    SignedTime =  tmp.SignedTime
                                    },
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = tmp2.isSigned,
                                    PersonnelName = tmp2.PersonnelName,
                                    MemberGrade = "QAQC",
                                    SignedColor = tmp2.SignedColor,
                                    SignedTime =  tmp2.SignedTime
                                    },
                                    new WinAppLibrary.UI.ObjectNFCSign{
                                    isSigned = "UnSigned",
                                    PersonnelName = "",
                                    MemberGrade = "Client QC",
                                    SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                    SignedTime = ""
                                    } 
                                };
                        break;
                }
                lvNFCSignList.DataContext = signed;
            });
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

        public enum NotifyType
        {
            StatusMessage,
            NdefMessage,
            PublishMessage,
            PeerMessage,
            ErrorMessage,
            ClearMessage
        };

        #endregion


        #region "ProximityHandler"


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
            //WinAppLibrary.Utilities.Helper.SimpleMessage(tagmsg, "AssignProcedureIn : tagmsg");

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
                        //UC에 Sign 있을 경우 체크
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

        private async void Setsigne(int personId, string personname, string _pinno)
        {
            ///WinAppLibrary.Utilities.Helper.SimpleMessage(lvNFCSignList.SelectedIndex.ToString(), "Setsigne 1 : TechnicianSignOffBy");
            
            try
            {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        WinAppLibrary.UI.ObjectNFCSign tmp1 = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[0];
                        WinAppLibrary.UI.ObjectNFCSign tmp2 = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[1];
                        WinAppLibrary.UI.ObjectNFCSign tmp3 = (WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList.Items[2];

                        if (lvNFCSignList.SelectedIndex == 0)
                        {
                            Doc.QAQCDTO.TechnicianSignOffBy = personname;
                            Doc.QAQCDTO.TechnicianOffDate = DateTime.Now;
                            Doc.QAQCDTO.TechnicianTitle = _pinno;

                            signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = "Signed",
                                PersonnelName = personname,
                                MemberGrade = "GF",
                                SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                SignedTime = DateTime.Now.ToString()},
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = tmp2.isSigned,
                                PersonnelName = tmp2.PersonnelName,
                                MemberGrade = "QAQC",
                                SignedColor = tmp2.SignedColor,
                                SignedTime = tmp2.SignedTime},
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = tmp3.isSigned,
                                PersonnelName = tmp3.PersonnelName,
                                MemberGrade = "Client QC",
                                SignedColor = tmp3.SignedColor,
                                SignedTime = tmp3.SignedTime} 
                            };
                        }
                        else if (lvNFCSignList.SelectedIndex == 1)
                        {
                            Doc.QAQCDTO.ContractorSignOffBy = personname;
                            Doc.QAQCDTO.ContractorSignOffDate = DateTime.Now;
                            Doc.QAQCDTO.ContractorTitle = _pinno;

                            signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = tmp1.isSigned,
                                PersonnelName = tmp1.PersonnelName,
                                MemberGrade = "GF",
                                SignedColor = tmp1.SignedColor,
                                SignedTime = tmp1.SignedTime},
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = "Signed",
                                PersonnelName = personname,
                                MemberGrade = "QAQC",
                                SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                SignedTime = DateTime.Now.ToString()},
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = tmp3.isSigned,
                                PersonnelName = tmp3.PersonnelName,
                                MemberGrade = "Client QC",
                                SignedColor = tmp3.SignedColor,
                                SignedTime = tmp3.SignedTime} 
                            };
                        }
                        else
                        {
                            Doc.QAQCDTO.ClientSignOffBy = personname;
                            Doc.QAQCDTO.ClientSignOffDate = DateTime.Now;
                            Doc.QAQCDTO.ClientTitle = _pinno;

                            signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = tmp1.isSigned,
                                PersonnelName = tmp1.PersonnelName,
                                MemberGrade = "GF",
                                SignedColor = tmp1.SignedColor,
                                SignedTime = tmp1.SignedTime},
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = tmp2.isSigned,
                                PersonnelName = tmp2.PersonnelName,
                                MemberGrade = "QAQC",
                                SignedColor = tmp2.SignedColor,
                                SignedTime = tmp2.SignedTime},
                            new WinAppLibrary.UI.ObjectNFCSign{
                                isSigned = "Signed",
                                PersonnelName = personname,
                                MemberGrade = "Client QC",
                                SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                                SignedTime = DateTime.Now.ToString()} 
                            };
                        }
                    });

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ucDoc.Save();
                    });

                    //저장 부분 확인 필요
                    //bool blSaved = await Doc.Save(ucDoc.QAQCDTOList, BaseFolder, _savedFileName, (hasUpdateGrid) ? ((PunchTicket)ucDoc).UpdateGrid : null);
                    //if (!blSaved) return;


                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        lvNFCSignList.DataContext = signed;
                    });


                    //저장 부분 확인 필요(오프라인 모드 시)
                    //for (int i = 0; i < _ofiles.Count; i++)
                    //{
                    //    if (_ofiles[i].QAQCFormTemplateID == Convert.ToInt32(arrParameter[0]))
                    //        _ofiles[i].QAQCFormCode = "3";
                    //}
                    //await SaveToQaqcformtemplate(_ofiles, BaseFolder, Lib.ITRList.DownloadList);

            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "AssignCrew");
                this.NotifyMessage("We had a problem to update signing. Please contact Administrator", "Error!");
                _onHandling = false;
            }
        }

        #endregion

    }
}
