using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using Windows.Networking.Proximity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Core;

using WinAppLibrary;
using Element.Reveal.Crew.RevealProjectSvc;
using Element.Reveal.Crew.RevealCommonSvc;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Utilities;
using WinAppLibrary.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.Administrator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CrewBrassOut : WinAppLibrary.Controls.LayoutAwarePage
    {

        Lib.ServiceModel.CommonModel _commonSM = new Lib.ServiceModel.CommonModel();
        Lib.ServiceModel.ProjectModel _projectSM = new Lib.ServiceModel.ProjectModel();
        Lib.ProximityHandler ProximityHandler;

        #region "Properties"
        private bool _onHandling = true;

        RevealUserSvc.MobileLoginDTO _foremanDto;
        RevealUserSvc.MobileLoginDTO ForemanDTO
        {
            get
            {
                return _foremanDto;
            }

            set
            {
                _foremanDto = value;
                this.DefaultViewModel["Foreman"] = new List<RevealUserSvc.MobileLoginDTO>() { value };
            }
        }

        int foremanPersonnelId = 0;
        //string _pinno = Lib.DataSource.BrassInOutDataSource.BrassList[0].DailyBrassID.ToString();
        string _pinno = "1234";

        //This list is for preventing direct access to ListView from Thread 
        List<PersonnelDTO> _foremanList = new List<PersonnelDTO>();
        List<DailybrassDTO> _brassList = new List<DailybrassDTO>();
        List<DailybrasssignDTO> _brasssignList = new List<DailybrasssignDTO>();
        #endregion

        #region "PageLoad"
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            try
            {
                foremanPersonnelId = Login.UserAccount.PersonnelID;
                ForemanDTO = Login.UserAccount;
                switch (Login.LoginMode)
                {
                    case WinAppLibrary.UI.LogMode.OnMode:
                        btnSubmit.IsEnabled = true;
                        InitiatePageOn();
                        break;
                    case WinAppLibrary.UI.LogMode.OffMode:
                        //foremanPersonnelId = 8;
                        //txtUserID.Text = "pkim";
                        //ForemanDTO = new RevealUserSvc.MobileLoginDTO();
                        //ForemanDTO.CurModuleID = 2;
                        //ForemanDTO.CurProjectID = 1;
                        //ForemanDTO.PersonnelID = 8;
                        btnSubmit.IsEnabled = false;
                        InitiatePageOff();
                        break;
                }

                uiSlideButton.Hide();
                //txtUserID.Text = Login.UserAccount.UserName == null ? "" : Login.UserAccount.UserName;
            }
            catch (Exception ex)
            {
                Loading(false);
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "LoadState");
                this.NotifyMessage("We couldn't get Foreman information", "Warning");
            }
        }

        public CrewBrassOut()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("Crew Brass Out");
            ProximityHandler = new Lib.ProximityHandler();
            ProximityHandler.OnException += ProximityHandler_OnException;
            ProximityHandler.OnMessage += ProximityHandler_OnMessage;
            uiSlideButton.SetImage(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/stop.png");
            uiSlideButton.ContentClick += uiSlideButton_ContentClick;
        }

        #endregion "PageLoad"

        #region "OnLineMode" ----------------------------------------------------------------------------------------------------------

        private async void InitiatePageOn()
        {
            Loading(true);
            _onHandling = true;

            //foremanPersonnelId = 3;
            //Login.MasterPage.Loading(true, this);
            try
            {
                if (Login.UserAccount.IsDailyBrass == 1)
                {
                    _brassList = await (new Lib.ServiceModel.ProjectModel()).GetDailybrassByForemanPersonnelWorkDate(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, foremanPersonnelId, DateTime.Now);
                }
                else
                {
                    //DailyBass 정보가 없다면 등록
                    _brassList = await InputDailyBass();
                    Login.UserAccount.IsDailyBrass = 1;
                }

                await (new Lib.DataSource.BrassInOutDataSource()).SaveFileDailyBrass(_brassList, Lib.HashKey.Key_ForemanBrassIn);
                _brasssignList.Clear();
                BindCrewListOn();

                //txtCurrent.Text = _brasssignList.Count().ToString();

                ProximityHandler.SetProximityDevice(ProximityDevice.GetDefault());
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "InitiatePage");
                this.NotifyMessage("We have difficulty to access to server4. Pleae contact administrator", "Error");
            }

            _onHandling = false;
            Loading(false);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ProximityHandler.DesetProximityDevice();
            base.OnNavigatedFrom(e);
        }

        //온라인 크루 리스트
        private async void BindCrewListOn()
        {
            try
            {
                _brasssignList.Clear();
                _brasssignList = await _projectSM.GetDailybrasssignByDailyBrass(_brassList[0].DailyBrassID);
                
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    txtCurrent.Text = _brasssignList.Count.ToString();
                    txtSignoutCount.Text = _brasssignList.Where(x => x.GateNo == Lib.GateNo.BRASSOUT).ToList().Count.ToString();
                    lvCrewList.Items.Clear();

                    foreach (var item in _brasssignList)
                    {
                        lvCrewList.Items.Add(item);
                    }
                });
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "BindCrewListOn");
                this.NotifyMessage("We have difficulty to access to server. Pleae contact administrator", "Error");
            }
        }

        //온라인 DailyBass 마스터 저장
        private async Task<List<RevealProjectSvc.DailybrassDTO>> InputDailyBass()
        {
            DailybrassDTO dto = new DailybrassDTO();

            dto.ForemanPersonnelID = foremanPersonnelId;
            dto.ModuleID = Login.UserAccount.CurModuleID;
            dto.ProjectID = Login.UserAccount.CurProjectID;
            dto.WorkDate = DateTime.Now;
            dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;

            List<DailybrassDTO> dtolist = new List<DailybrassDTO>() { dto };
            var list = await _projectSM.SaveDailybrass(dtolist);

            return list;
        }

        //크루 사인인 저장 시작
        private void AssignCrew(int personnelId, string personname)
        {
            bool retValue = false;
            //this.NotifyUser("AssignCrew : " + personnelId.ToString(), NotifyType.PublishMessage);
            if (_brassList[0] != null)
            {

                if (_brasssignList == null || _brasssignList.Where(x => x.MyPersonnelID == personnelId && x.GateNo == Lib.GateNo.BRASSIN && x.SignTimestamp.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd")).FirstOrDefault() != null)
                {
                    try
                    {
                        DailybrasssignDTO crewout = new DailybrasssignDTO();

                        crewout.ModuleID = _brassList[0].ModuleID;
                        crewout.ProjectID = _brassList[0].ProjectID;
                        crewout.DailyBrassID = _brassList[0].DailyBrassID;
                        crewout.GateNo = Lib.GateNo.BRASSOUT;
                        crewout.MyPersonnelID = personnelId;
                        crewout.NFCUID = 1;
                        crewout.SignTimestamp = DateTime.Now;
                        crewout.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                        crewout.PersonnelName = personname;

                        UpdateCrew(crewout);

                    }
                    catch (Exception ex)
                    {
                        (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "AssignCrew");
                        this.NotifyMessage("We had a problem to update signing. Please contact Administrator", "Error!");
                        _onHandling = false;
                    }
                }
                else
                {
                    this.NotifyMessage("This crew is already assigned or not brass in.", "Alert");
                    _onHandling = false;
                }


            }
            else
            {
                this.NotifyMessage("Not found", "Alert");
                _onHandling = false;
            }

        }

        //크루 사인인 저장 완료
        private void UpdateCrew(DailybrasssignDTO dto)
        {
            #region "UpdateCrew"
            try
            {
                if (dto != null && dto.MyPersonnelID > 0)
                {
                    List<DailybrasssignDTO> crewList_out = new List<DailybrasssignDTO>();
                    crewList_out.Add(dto);

                    _projectSM.SaveDailybrasssign(crewList_out);

                    _brasssignList.Clear();
                    BindCrewListOn();
                    

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion UpdateCrew"
        }

        //크루 사인인 삭제
        private async void RemoveCrewOn(DailybrasssignDTO dtoout)
        {
            #region "RemoveCrew"
            try
            {
                //Loading(true);

                var item = lvCrewList.Items.Where(x => (x as DailybrasssignDTO) == dtoout).FirstOrDefault();
                //var crew = _brasssignList.Where(AlignmentX => AlignmentX.MyPersonnelID == dtoout.MyPersonnelID && AlignmentX.SignTimestamp.ToString("yyyyMMdd") == dtoout.SignTimestamp.ToString("yyyyMMdd")).FirstOrDefault();
                var crew = _brasssignList.Where(AlignmentX => AlignmentX.DailyBrassSignID == dtoout.DailyBrassSignID).FirstOrDefault();
                if (crew.GateNo == Lib.GateNo.BRASSOUT)
                {

                    if (item != null)
                    {
                        dtoout.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;

                        List<DailybrasssignDTO> dtooutlist = new List<DailybrasssignDTO>();

                        dtooutlist.Add(dtoout);

                        await (new Lib.ServiceModel.ProjectModel()).SaveDailybrasssign(dtooutlist);

                        //_brasssignList.Remove(crew);
                        _onHandling = false;
                        _brasssignList.Clear();
                        ProximityHandler.Dispose();
                        BindCrewListOn();
                    }
                }
                else
                {
                    this.NotifyMessage("It will not be deleted", "Caution!");
                    _onHandling = false;
                }

                //Loading(false);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "RemoveCrew");
                this.NotifyMessage("There was an error to connect to server", "Caution!");
            }
            #endregion "RemoveCrew"
        }

        #endregion "OnLineMode" --------------------------------------------------------------------------------------------------------

        #region "OffLineMode" ----------------------------------------------------------------------------------------------------------

        private async void InitiatePageOff()
        {
            Loading(true);
            _onHandling = true;

            try
            {
                //_brassList = await (new Lib.ServiceModel.ProjectModel()).GetDailybrassByForemanPersonnelWorkDate(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, Login.UserAccount.PersonnelID, DateTime.Now);
                var options = await GetDailybass();
                if (options[Lib.HashKey.Key_ForemanBrassIn] != null)
                {
                    _brassList = options[Lib.HashKey.Key_ForemanBrassIn];
                    if (_brassList != null || _brassList.Count > 0)
                    {
                        var options2 = await GetSigninList();
                        if (options2[Lib.HashKey.Key_CrewBrassIn] != null)
                        {
                            _brasssignList = options2[Lib.HashKey.Key_CrewBrassIn];
                            //crewList_new = _crewList;
                            if (_brasssignList != null && _brasssignList.Count > 0)
                            {
                                _brasssignList = options2[Lib.HashKey.Key_CrewBrassIn];
                            }
                        }

                        BindCrewListOff();

                        ProximityHandler.SetProximityDevice(ProximityDevice.GetDefault());
                    }
                    else
                    {
                        this.NotifyMessage("We have difficulty to access to server2. Pleae contact administrator.", "Error");
                    }
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "InitiatePage");
                this.NotifyMessage("We have difficulty to access to server3. Pleae contact administrator.", "Error");
            }

            _onHandling = false;
            Loading(false);
        }

        //크루 리스트
        private async void BindCrewListOff()
        {
            if (_brassList[0] != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    lvCrewList.Items.Clear();
                    
                    var bslist = (
                            from t in _brasssignList
                            group t by new { t.DailyBrassID, t.MyPersonnelID } into g
                            select new
                            {
                                DailyBrassID = g.Key.DailyBrassID,
                                MyPersonnelID = g.Key.MyPersonnelID,
                                DailyBrassSignID = g.Max(t => t.DailyBrassSignID),
                                GateNo = g.Max(t => t.GateNo),
                                ModuleID = g.Max(t => t.ModuleID),
                                MyDepartStructureID = g.Max(t => t.MyDepartStructureID),
                                NFCUID = g.Max(t => t.NFCUID),
                                ParentDepartStructureID = g.Max(t => t.ParentDepartStructureID),
                                ParentPersonnelID = g.Max(t => t.ParentPersonnelID),
                                PersonnelName = g.Max(t => t.PersonnelName),
                                ProjectID = g.Max(t => t.ProjectID),
                                SignTimestamp = g.Max(t => t.SignTimestamp),
                                Status = g.Max(t => t.Status)
                            }
                        );

                    if (bslist != null)
                    {
                        foreach (var item in bslist)
                        {
                            DailybrasssignDTO dto = new DailybrasssignDTO();
                            dto.DailyBrassID = item.DailyBrassID;
                            dto.MyPersonnelID = item.MyPersonnelID;
                            dto.DailyBrassSignID = item.DailyBrassSignID;
                            dto.GateNo = item.GateNo;
                            dto.ModuleID = item.ModuleID;
                            dto.MyDepartStructureID = item.MyDepartStructureID;
                            dto.NFCUID = item.NFCUID;
                            dto.ParentDepartStructureID = item.ParentDepartStructureID;
                            dto.ParentPersonnelID = item.ParentPersonnelID;
                            dto.PersonnelName = item.PersonnelName;
                            dto.ProjectID = item.ProjectID;
                            dto.SignTimestamp = item.SignTimestamp;
                            dto.Status = item.Status;
                            lvCrewList.Items.Add(dto);
                        }
                    }

                    txtCurrent.Text = lvCrewList.Items.Count.ToString();
                    txtSignoutCount.Text = _brasssignList.Where(x => x.GateNo == Lib.GateNo.BRASSOUT).ToList().Count.ToString();
                });
            }
        }

        //크루 사인인 저장 시작
        private void AssignCrewOff(int personnelId, string personname)
        {
            #region "AssignCrewOff"

            try
            {
                if (personnelId > 0)
                {
                    //this.NotifyUser("AssignCrewOff1 : " + personnelId.ToString(), NotifyType.PublishMessage);
                    if (_brasssignList == null || _brasssignList.Where(x => x.MyPersonnelID == personnelId && x.GateNo == Lib.GateNo.BRASSOUT && x.SignTimestamp.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd")).FirstOrDefault() == null)
                    {
                        if (_brasssignList.Where(x => x.MyPersonnelID == personnelId && x.GateNo == Lib.GateNo.BRASSIN && x.SignTimestamp.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd")).FirstOrDefault() != null)
                        {
                            //this.NotifyUser("AssignCrewOff2 : " + personnelId.ToString(), NotifyType.PublishMessage);
                            DailybrasssignDTO crew = new DailybrasssignDTO();

                            crew.ModuleID = _brassList[0].ModuleID;
                            crew.ProjectID = _brassList[0].ProjectID;
                            crew.DailyBrassID = _brassList[0].DailyBrassID;
                            crew.GateNo = Lib.GateNo.BRASSOUT;
                            crew.MyPersonnelID = personnelId;
                            crew.NFCUID = 1;
                            crew.SignTimestamp = DateTime.Now;
                            crew.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                            crew.PersonnelName = personname;
                            crew.Status = Lib.GateNo.BRASSOUT;

                            _brasssignList.Add(crew);
                            //this.NotifyUser("AssignCrewOff3 : " + _brassList[0].DailyBrassID.ToString(), NotifyType.PublishMessage);
                            //crewList_new = _crewList;

                            UpdateCrewOff(crew);
                        }
                        else
                        {
                            _onHandling = false;
                            this.NotifyMessage("This crew is already assigned.", "Alert");
                        }
                    }
                    else
                    {
                        _onHandling = false;
                        this.NotifyMessage("This crew is already assigned.", "Alert");
                    }
                }
            }
            catch (Exception e)
            {
            }

            #endregion
        }

        //크루 사인인 저장 완료
        private async void UpdateCrewOff(DailybrasssignDTO dto)
        {
            #region "UpdateCrew"

            try
            {
                if (dto != null && dto.MyPersonnelID > 0)
                {
                    await (new Lib.DataSource.BrassInOutDataSource()).SaveFileDayilyBrassSign(_brasssignList, "BrassSignIn");

                    if (_brasssignList != null && _brasssignList.Count > 0)
                    {
                        BindCrewListOff();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion UpdateCrew"
        }

        //크루 사인인 삭제
        private async void RemoveCrewOff(DailybrasssignDTO dto)
        {
            #region "RemoveCrew"
            try
            {
                //Loading(true);
                var item = lvCrewList.Items.Where(x => (x as DailybrasssignDTO) == dto).FirstOrDefault();
                //var crew = _brasssignList.Where(x => x.DailyBrassSignID == dto.DailyBrassSignID && x.MyPersonnelID == dto.MyPersonnelID && x.GateNo == dto.GateNo).FirstOrDefault();
                var crew = _brasssignList.Where(x => x.MyPersonnelID == dto.MyPersonnelID && x.DailyBrassSignID == dto.DailyBrassSignID).OrderByDescending(x => x.GateNo).FirstOrDefault();
                //var crew = _brasssignList.Where(x => (x as DailybrasssignDTO) == dto).FirstOrDefault();

                if (crew.GateNo == Lib.GateNo.BRASSOUT)
                {
                    if (item != null)
                    {
                        _brasssignList.Remove(crew);

                        await (new Lib.DataSource.BrassInOutDataSource()).SaveFileDayilyBrassSign(_brasssignList, "BrassSignIn");

                        BindCrewListOff();
                    }
                }
                else
                {
                    _onHandling = false;
                    ProximityHandler.Dispose();
                    this.NotifyMessage("This Crew is no deleted here ", "Caution!");
                }

                //Loading(false);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "RemoveCrew");
                this.NotifyMessage("There was an error to connect to server", "Caution!");
            }
            #endregion "RemoveCrew"
        }

        #endregion "OffLineMode"----------------------------------------------------------------------------------------------------------

        #region "Event Handler"

        #region "ProximityHandler"

        private void ProximityHandler_OnException(object sender, object e)
        {
            uiSlideButton.Hide();
            Loading(false);
            this.NotifyMessage(e.ToString(), "Caution!");
        }

        private void ProximityHandler_OnMessage(object sender, object e)
        {
            var type = (NotifyType)sender;
            switch (type)
            {
                case NotifyType.NdefMessage:
                    AssignProcedure(e.ToString());
                    break;
                case NotifyType.PublishMessage:
                    uiSlideButton.Hide();
                    Loading(false);
                    //this.NotifyMessage(e.ToString(), "Success!");
                    break;
                case NotifyType.StatusMessage:
                    if(_onHandling)
                        this.UpdateStatus(e.ToString());
                    break;
                default :
                    break;
            }
        }

        private void AssignProcedure(string tagmsg)
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

                        switch (Login.LoginMode)
                        {
                            case WinAppLibrary.UI.LogMode.OnMode:
                                //this.NotifyUser("ProcedurepersonId : " + personId.ToString(), NotifyType.PublishMessage);
                                AssignCrew(personId, personname);
                                break;
                            case WinAppLibrary.UI.LogMode.OffMode:
                                AssignCrewOff(personId, personname);
                                break;
                        }

                        _onHandling = false;
                    }
                    catch (Exception e)
                    {
                        Loading(false);
                        (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "AssignProcedure");
                        this.NotifyMessage("We had a problem to update signing. Please contact Administrator", "Error!");
                        _onHandling = false;
                    }
                }
                else
                {
                    this.NotifyMessage("This tag doesn't have crew information", "Alert");
                    _onHandling = false;
                }

            }

            #endregion
        }

        #endregion "ProximityHandler"

        #region "Publish"
        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            Loading(true);
            uiSlideButton.Show();
            _onHandling = true;

            try
            {
                int publishid = Convert.ToInt32(txtPublishid.Text == "" ? "0" : txtPublishid.Text);
                string publishname = txtPublishname.Text;

                if (publishid > 0)
                {

                    this.NotifyMessage("publish", "Caution!");
                    ProximityHandler.SetPublishCrew(publishid.ToString() + "*" + publishname);

                    ufn_Publish("E");
                }
                else
                {
                    ufn_Publish("E");
                    _onHandling = false;
                    uiSlideButton.Hide();
                    Loading(false);
                    this.NotifyMessage("We couldn't find login information. Please try login again.", "Caution!");
                }
            }
            catch (Exception ex)
            {
                //lbMessageBox.Items.Add(ex.Message.ToString());
            }

            //This is for assigning crew to NFC
            //ProximityHandler.SetPublishCrew(86);
        }

        //private void btnPublish1_Click(object sender, RoutedEventArgs e)
        //{
        //    ufn_Publish((sender as Button).Tag.ToString());
        //}

        private void ufn_Publish(string mode)
        {
            switch (mode)
            {
                case "P":
                    grPublish.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    break;
                case "E":
                    grPublish.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
            }
        }
        #endregion  "Publish"

        #region "Button Event Handler"

        private void lvCrewList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //BindCrew(e.ClickedItem);
        }

        private void lvCrewList_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;

            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
            {
                var source = e.OriginalSource as Grid;
                if (source != null)
                {
                    switch (Login.LoginMode)
                    {
                        case WinAppLibrary.UI.LogMode.OnMode:
                            RemoveCrewOn(source.DataContext as DailybrasssignDTO);
                            break;
                        case WinAppLibrary.UI.LogMode.OffMode:
                            RemoveCrewOff(source.DataContext as DailybrasssignDTO);
                            break;
                        default:
                            break;
                    }
                }
            }
            e.Handled = false;
        }

        private void uiSlideButton_ContentClick(object sender, RoutedEventArgs e)
        {
            ProximityHandler.StopPublish();
            uiSlideButton.Hide();
            _onHandling = false;
            Loading(false);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ProximityHandler.Dispose();
            this.Frame.Navigate(typeof(MainMenu), Login.UserAccount.PersonnelID);
        }

        private void MessageDialog_OkClick(object sender, object e)
        {
            //This semaphore is aimed for waiting until user accepts the result.
            _onHandling = false;
            MessageDialog.Hide(this);
        }

        private void btnAddCrew_Click(object sender, RoutedEventArgs e)
        {
            ufn_Publish("E");

            int personId = Convert.ToInt32(txtPublishid.Text);
            string personname = txtPublishname.Text;

            switch (Login.LoginMode)
            {
                case WinAppLibrary.UI.LogMode.OnMode:
                    AssignCrew(personId, personname);
                    break;
                case WinAppLibrary.UI.LogMode.OffMode:
                    AssignCrewOff(personId, personname);
                    break;
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int check = await _projectSM.SaveCrewAttendance(Lib.DataSource.BrassInOutDataSource.BrassList[0].DailyBrassID ,Login.UserAccount.CurProjectID);

                if (check > 0)
                {
                    this.NotifyMessage("Completion of the transfer", "Caution!");
                }
            }
            catch (Exception ex)
            {
                this.NotifyMessage("We have difficulty to access to server. Pleae contact administrator", "Error");
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var source = (sender as Image).Source as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            source.UriSource = new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Crew/default_crew.png");
        }

        private void Image_ImageFailed_Foreman(object sender, ExceptionRoutedEventArgs e)
        {
            var source = (sender as Image).Source as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            source.UriSource = new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Foreman/default_foreman.png");
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ForemanInfoPanel.Width = (e.NewSize.Width - 70);
            CrewListPanel.Width = ForemanInfoPanel.Width;
            lvCrewList.Height = CrewListPanel.ActualHeight;// -grCrewDetailPanel.ActualHeight - btnSns.Height;// btnPublish.Height;
        }

        #endregion

        #endregion

        #region "GetFile"
        private async Task<Dictionary<string, List<RevealProjectSvc.DailybrassDTO>>> GetDailybass()
        {
            Dictionary<string, List<RevealProjectSvc.DailybrassDTO>> retValue = new Dictionary<string, List<RevealProjectSvc.DailybrassDTO>>();
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                //Get Crew BrassSignIn List
                var stream = await helper.GetFileStream(Lib.ContentPath.OffModeUserFolder, Lib.ContentPath.BrassIn);
                var list = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealProjectSvc.DailybrassDTO>>(stream);
                retValue.Add(Lib.HashKey.Key_ForemanBrassIn, list);
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "GetGrouping-ForemanBrassIn");
                throw e;
            }

            return retValue;
        }

        private async Task<Dictionary<string, List<RevealProjectSvc.DailybrasssignDTO>>> GetSigninList()
        {
            Dictionary<string, List<RevealProjectSvc.DailybrasssignDTO>> retValue = new Dictionary<string, List<RevealProjectSvc.DailybrasssignDTO>>();
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                //Get Crew BrassSignIn List
                var stream = await helper.GetFileStream(Lib.ContentPath.OffModeUserFolder, Lib.ContentPath.BrassSignIn);
                var list = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealProjectSvc.DailybrasssignDTO>>(stream);
                retValue.Add(Lib.HashKey.Key_CrewBrassIn, list);

            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "GetGrouping-BrassSignIn");
                throw e;
            }

            return retValue;
        }

        #endregion "GetFile"

        #region "Private Method"

        private async void Loading(bool isshow)
        {
            if(isshow)
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => 
                { 
                    if(!Loader.IsLoading(this)) 
                        Loader.Show(this); 
                });
            else
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { Loader.Hide(this); });
        }
        
        private async void UpdateStatus(string status)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Loader.UpdateStatus(this, status);
                });
        }

        public async void NotifyMessage(string msg, string title)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MessageDialog.DialogTitle = title;
                    MessageDialog.Content = msg;
                    MessageDialog.Show(this);
                });
        }

        //public async void NotifyUser(string strMessage, NotifyType type)
        //{
        //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        TextBlock tbmsg = new TextBlock();
        //        switch (type)
        //        {
        //            // Use the status message style.
        //            case NotifyType.StatusMessage:
        //                tbmsg.Style = Resources["StatusStyle"] as Style;
        //                break;
        //            // Use the error message style.
        //            case NotifyType.ErrorMessage:
        //                tbmsg.Style = Resources["ErrorStyle"] as Style;
        //                break;
        //            case NotifyType.ClearMessage:
        //                //lbMessageBox.Items.Clear();
        //                break;
        //        }
        //        if (!string.IsNullOrEmpty(strMessage))
        //        {
        //            //tbmsg.Text = strMessage;
        //            lbMessageBox.Items.Add(strMessage);
        //        }
        //    });
        //}

        #endregion
    }


}
