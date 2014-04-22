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
using Element.Reveal.Meg.RevealProjectSvc;
using Element.Reveal.Meg.RevealCommonSvc;

namespace Element.Reveal.Meg
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ServiceModel.CommonModel _commonSM = new Lib.ServiceModel.CommonModel();
        Lib.ServiceModel.ProjectModel _projectSM = new Lib.ServiceModel.ProjectModel();
        Lib.ProximityHandler ProximityHandler;

        #region "Properties"
        private bool _onHandling = true;

        //This is for keeping foreman in memor
        PersonnelDTO _foremanDto;
        PersonnelDTO ForemanDTO
        {
            get
            {
                return _foremanDto;
            }

            set
            {
                _foremanDto = value;
                this.DefaultViewModel["Foreman"] = new List<PersonnelDTO>() { value };
            }
        }

        FiwpmanonsiteDTO _crewDto;
        FiwpmanonsiteDTO CrewDTO
        {
            get
            {
                if (_crewDto == null)
                    return new FiwpmanonsiteDTO();
                else
                    return _crewDto;
            }

            set
            {
                _crewDto = value;
                spInfoPanel.Visibility = Visibility.Visible;
                this.DefaultViewModel["Crew"] = new List<FiwpmanonsiteDTO>() { value };
            }
        }
        //This list is for preventing direct access to ListView from Thread 
        List<FiwpmanonsiteDTO> _crewList = new List<FiwpmanonsiteDTO>();
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            ProximityHandler = new Lib.ProximityHandler();
            ProximityHandler.OnException += ProximityHandler_OnException;
            ProximityHandler.OnMessage += ProximityHandler_OnMessage;
            uiSlideButton.SetImage(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/stop.png");
            uiSlideButton.ContentClick += uiSlideButton_ContentClick;
        }

        #region "Event Handler"
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            try
            {
                int foremanPersonnelId = Convert.ToInt32(navigationParameter.ToString());
                //foremanPersonnelId = 3;
                InitiatePage(foremanPersonnelId);
            }
            catch (Exception ex)
            {
                Loading(false);
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "LoadState");
                this.NotifyMessage("We couldn't get Foreman information", "Warning");
            }
        }

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
                    this.NotifyMessage(e.ToString(), "Success!");
                    break;
                case NotifyType.StatusMessage:
                    if(_onHandling)
                        this.UpdateStatus(e.ToString());
                    break;
            }
        }

        private void lvCrewList_ItemClick(object sender, ItemClickEventArgs e)
        {
            BindCrew(e.ClickedItem);
        }

        private void lvCrewList_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;

            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
            {
                var source = e.OriginalSource as Grid;
                if (source != null)
                {
                    RemoveCrew(source.DataContext as FiwpmanonsiteDTO);
                }
            }
        }

        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            Loading(true);
            uiSlideButton.Show();
            _onHandling = true;
          
            if (Login.UserAccount.PersonnelID > 0)
                ProximityHandler.SetPublishLaunch(Login.UserAccount.PersonnelID.ToString());
                //ProximityHandler.SetPublishCrew(86);
            else
            {
                _onHandling = false;
                uiSlideButton.Hide();
                Loading(false);
                this.NotifyMessage("We couldn't find login information. Please try login again.", "Caution!");
            }

            //This is for assigning crew to NFC
            //ProximityHandler.SetPublishCrew(86);
        }

        private void uiSlideButton_ContentClick(object sender, RoutedEventArgs e)
        {
            ProximityHandler.StopPublish();
            uiSlideButton.Hide();
            _onHandling = false;
            Loading(false);
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {            
            var source = (sender as Image).Source as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            source.UriSource = new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/DefaultCrew.png");
        }

        private void btInfo_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag.ToString())
            {
                case "Add":
                    break;
                case "Edit":
                    break;
                case "Save":
                    break; ;
                case "Cancel":
                    break;
            }
        }

        private void btnSns_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag.ToString())
            {
                case "Talk":
                    grToolboxTalk.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    /*
                    Windows.UI.Xaml.Media.Animation.Storyboard board = new Windows.UI.Xaml.Media.Animation.Storyboard();
                    Windows.UI.Xaml.Media.Animation.Storyboard board2 = new Windows.UI.Xaml.Media.Animation.Storyboard();
                    TextBlock msg = new TextBlock();
                    msg.Text = "Coming soon~";
                    msg.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                    msg.FontSize = 20;
                    msg.Opacity = 0;
                    cvSns.Children.Add(msg);
                    var height = this.ActualHeight - spLeftPanel.ActualHeight;
                    Canvas.SetLeft(msg, btnSns.ActualWidth + tblMsg.ActualWidth);
                    Canvas.SetTop(msg, height);
          
                    var move = WinAppLibrary.Utilities.AnimationHelper.CreateYAnimation(msg, 0, 2.5);
                    var opacity = WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(msg, 1, 0, 0.5);

                    board.Children.Add(move);
                    board2.Children.Add(opacity);
                    board2.Completed += (s, ee) => { if (opacity.To == 1) { opacity.To = 0; board2.Begin(); } };
                    board.Begin();
                    board2.Begin();
                     */
                    break;
                case "Exit":
                    grToolboxTalk.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
            }
        }

        private void MessageDialog_OkClick(object sender, object e)
        {
            //This semaphore is aimed for waiting until user accepts the result.
            _onHandling = false;
            MessageDialog.Hide(this);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            spLeftPanel.Width = (e.NewSize.Width - 70) / 2;
            spRightPanel.Width = spLeftPanel.Width;
            lvCrewList.Height = spRightPanel.ActualHeight - grCrewDetailPanel.ActualHeight - btnPublish.Height;
        }
        #endregion

        #region "Private Method"
        private async void InitiatePage(int foremanPersonnelId)
        {
            Loading(true);
            
            try
            {
                ForemanDTO = await (new Lib.ServiceModel.CommonModel()).GetSinglePersonnelByID(foremanPersonnelId);
                BindCrewList(ForemanDTO);
                ProximityHandler.SetProximityDevice(ProximityDevice.GetDefault());
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "InitiatePage");
                this.NotifyMessage("We have difficulty to access to server. Pleae contact administrator.", "Error");
            }

            _onHandling = false;
            Loading(false);
        }

        private async void AssignProcedure(string tagmsg)
        {
            if (!_onHandling)
            {
                if (!string.IsNullOrEmpty(tagmsg))
                {
                    _onHandling = true;
                    int personId = 0;

                    try
                    {
                        personId = Convert.ToInt32(tagmsg);
                    }
                    catch { }

                    try
                    {
                        
                        await AssignCrew(personId);
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
                    this.NotifyMessage("This tag doesn't have crew information", "Alert");
                
            }
        }

        private async Task<bool> AssignCrew(int personnelId)
        {
            bool retValue = false;

            if (personnelId > 0)
            {
                if (_crewList.Where(x => x.PersonnelID == personnelId).FirstOrDefault() == null)
                {
                    Loading(true);
                    PersonnelDTO tagmanDto = await _commonSM.GetSinglePersonnelByID(personnelId);

                    if(tagmanDto == null || tagmanDto.PersonnelID == 0)
                        this.NotifyMessage("It doesn't exist in DataBase!", "Alert");
                    else if (tagmanDto != null && tagmanDto.DepartmentID == WinAppLibrary.Utilities.Department.Crew)
                    {

                        FiwpmanonsiteDTO crew = new FiwpmanonsiteDTO();
                        crew.ModuleID = ForemanDTO.CurModuleID;
                        crew.ProjectID = ForemanDTO.CurProjectID;
                        crew.FIWPID = ForemanDTO.CurFIWPID;
                        crew.PersonnelID = tagmanDto.PersonnelID;
                        crew.FullName = tagmanDto.FName ?? "" + ", " + tagmanDto.LName ?? "";
                        crew.DepartStructureID = tagmanDto.CurDepartStructureID;
                        crew.ForemanDepartStructureID = ForemanDTO.CurDepartStructureID;
                        crew.ProjectID = ForemanDTO.CurProjectID;
                        crew.ModuleID = ForemanDTO.CurModuleID;
                        crew.StatusLUID = WinAppLibrary.Utilities.FiwpManonsiteStatus.OnSite;
                        crew.WorkDate = DateTime.Today;
                        crew.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;

                        retValue = await UpdateCrew(crew);
                    }
                    else
                        this.NotifyMessage("It's not crew!. Only crew can be assigned.", "Alert");

                    Loading(false);
                }
                else
                    this.NotifyMessage("This crew is already assigned.", "Alert");
            }

            return retValue;
        }

        private async Task<bool> UpdateCrew(FiwpmanonsiteDTO dto)
        {
            bool retValue = false;
            try
            {
                if (dto != null && dto.PersonnelID > 0)
                {
                    List<FiwpmanonsiteDTO> crewList_new = new List<FiwpmanonsiteDTO>() { dto };
                    //assign crew
                    crewList_new = await _projectSM.SaveFiwpManonsite(crewList_new);

                    if (crewList_new != null && crewList_new.Count > 0)
                    {
                        BindCrewList(ForemanDTO);
                        BindCrew(dto);
                        retValue = true;
                    }
                }
            }
            catch (Exception ex)
            {            
                throw ex;
            }

            return retValue;
        }

        private async void RemoveCrew(FiwpmanonsiteDTO dto)
        {
            try
            {
                //Loading(true);
                var item = lvCrewList.Items.Where(x => (x as FiwpmanonsiteDTO) == dto).FirstOrDefault();
                var crew = _crewList.Where(AlignmentX => AlignmentX.FiwpManOnSiteID == dto.FiwpManOnSiteID).FirstOrDefault();

                if (item != null)
                {
                    dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                    await (new Lib.ServiceModel.ProjectModel()).SaveFiwpManonsite(new List<FiwpmanonsiteDTO>() { dto });                    
                    _crewList.Remove(crew);
                    lvCrewList.Items.Remove(item);
                }

                //Loading(false);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "RemoveCrew");
                this.NotifyMessage("There was an error to connect to server", "Caution!");
            }
        }
                

        private async void BindCrewList(PersonnelDTO foreman)
        {
            if (foreman != null)
            {
                var crewList = await _projectSM.GetFiwpManonsiteByForeman(foreman.CurDepartStructureID, DateTime.Now);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    lvCrewList.Items.Clear();
                    _crewList.Clear();
                    foreach (var item in crewList)
                    {
                        _crewList.Add(item);
                        lvCrewList.Items.Add(item);
                    }
                });
            };
        }

        private async void BindCrew(object item)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (item != null)
                {
                    lvCrewList.SelectedItem = item;
                    CrewDTO = item as FiwpmanonsiteDTO;
                    imgDefaultCrew.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            });
        }

        private async void BindCrew(FiwpmanonsiteDTO dto)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var item = lvCrewList.Items.Where(x => (x as FiwpmanonsiteDTO).PersonnelID == dto.PersonnelID).FirstOrDefault();
                if (item != null)
                {
                    lvCrewList.SelectedItem = item;
                    CrewDTO = item as FiwpmanonsiteDTO;
                }
            });
        }

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
        
        /*
         * This was used for indicating procedure message for development
        public async void NotifyUser(string strMessage, NotifyType type)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TextBlock tbmsg = new TextBlock();
                switch (type)
                {
                    // Use the status message style.
                    case NotifyType.StatusMessage:
                        tbmsg.Style = Resources["StatusStyle"] as Style;
                        break;
                    // Use the error message style.
                    case NotifyType.ErrorMessage:
                        tbmsg.Style = Resources["ErrorStyle"] as Style;
                        break;
                    case NotifyType.ClearMessage:
                        lbMessageBox.Items.Clear();
                        break;
                }
                if (!string.IsNullOrEmpty(strMessage))
                {
                    tbmsg.Text = strMessage;
                    lbMessageBox.Items.Add(tbmsg);
                }
            });
        }
        */
        #endregion
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
}
