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

using System.Reflection;

//using MuPDFWinRT;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.Administrator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
   
    public sealed partial class DailyToolBoxTalk : WinAppLibrary.Controls.LayoutAwarePage
    {
        //Lib.DocumentPage currentPage;
        //Document pdfDocument;
        //ScrollViewer scrollViewer;
        //readonly List<Lib.DocumentPage> pages = new List<Lib.DocumentPage>();
        Lib.ProximityHandler ProximityHandler;
        private bool _onHandling = true;
        private bool _showBottombar = true;
        private bool _pdfread = false;

        Lib.ServiceModel.ProjectModel _projectSM = new Lib.ServiceModel.ProjectModel();
    
        int _brassid = 0;
        string _pinno = "1234";

        ToolboxsignDTO toolboxSingIn = new ToolboxsignDTO();
        List<RevealProjectSvc.ToolboxsignDTO> _toolboxsign = Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign;
        //private bool _onHandling = true;

        public DailyToolBoxTalk()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("Toolbox talk topic");
            ProximityHandler = new Lib.ProximityHandler();
            ProximityHandler.OnException += ProximityHandler_OnException;
            ProximityHandler.OnMessage += ProximityHandler_OnMessage;
        }
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.Loading(true, this);
            _onHandling = true;

            _brassid = Convert.ToInt32(navigationParameter.ToString());

            switch (Login.LoginMode)
            {
                case WinAppLibrary.UI.LogMode.OnMode:
                    btnSubmit.IsEnabled = true;
                    break;
                case WinAppLibrary.UI.LogMode.OffMode:
                    btnSubmit.IsEnabled = false;
                    break;
            }

            DownloadImg();

            BindToolboxtalk();

            ProximityHandler.SetProximityDevice(ProximityDevice.GetDefault());

            _onHandling = false;
            Login.MasterPage.Loading(false, this);
        }


        #region "ProximityHandler"

        private void ProximityHandler_OnException(object sender, object e)
        {
            //Loading(false);
            this.NotifyMessage(e.ToString(), "Caution!");
        }

        private void ProximityHandler_OnMessage(object sender, object e)
        {
            var type = (NotifyType)sender;
            switch (type)
            {
                case NotifyType.NdefMessage:
                    //if (_pdfread)
                    //{
                        AssignProcedure(e.ToString());
                    //    _pdfread = false;
                    //}
                    //else
                    //    this.NotifyMessage("Toolboxtalks read", "Caution!");
                    break;
                case NotifyType.PublishMessage:
                    //Loading(false);
                    //this.NotifyMessage(e.ToString(), "Success!");
                    break;
                default:
                    break;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ProximityHandler.DesetProximityDevice();
            base.OnNavigatedFrom(e);
        }

        private async void AssignProcedure(string tagmsg)
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
                            if(temptagmsg.Length > 2)
                            {
                                _pinno = temptagmsg[2].ToString();
                            }
                        }

                        //this.NotifyUser("AssignProcedure : " + personId.ToString(), NotifyType.PublishMessage);

                        var item = Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign.Where(x => (x as ToolboxsignDTO).MyPersonnelID == personId).ToList();

                        if (item.Count == 0)
                        {
                            //Save Event
                            toolboxSingIn = new ToolboxsignDTO();
                            toolboxSingIn.DailyBrassID = _brassid;
                            toolboxSingIn.MyPersonnelID = personId;
                            toolboxSingIn.SignTimestamp = DateTime.Now;
                            toolboxSingIn.NFCUID = 1;
                            toolboxSingIn.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                            toolboxSingIn.CreatedDate = DateTime.Now;
                            toolboxSingIn.CreatedBy = personname;

                            //this.NotifyUser("dtoinsert : " + personId.ToString(), NotifyType.PublishMessage);
                            ufn_PopupInputPin("OPEN", personname);
                            //this.NotifyUser("end : " + personId.ToString(), NotifyType.PublishMessage);
                        }
                        else
                        {
                            this.NotifyMessage("This crew is already assigned.", "Alert");
                        }

                        _onHandling = false;
                    }
                    catch (Exception e)
                    {
                        //Loading(false);
                        (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "AssignProcedure");
                        this.NotifyMessage("We had a problem to update signing. Please contact Administrator", "Error!");
                        _onHandling = false;
                    }
                }
                else
                    this.NotifyMessage("This tag doesn't have crew information", "Alert");

            }

            #endregion
        }

        //온라인 크루 리스트
        private async void BindToolboxtalk()
        {
            try
            {
                if (Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign.Count() > 0)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        lvCrewList.Items.Clear();

                        foreach (var dto in Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign.OrderBy(x => x.SignTimestamp).ToList())
                        {
                            lvCrewList.Items.Add(dto);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "BindToolboxtalk");
                this.NotifyMessage("We have difficulty to access to server. Pleae contact administrator", "Error");
            }

            // return retValue;
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _projectSM.SaveToolBoxSign(Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign);
                await _projectSM.SaveToolboxTalks(_brassid, Login.UserAccount.CurProjectID);

                Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign.Clear();
                lvCrewList.Items.Clear();

                this.NotifyMessage("Completion of the transfer", "Caution!");
            }
            catch (Exception ex)
            {
                this.NotifyMessage("We have difficulty to access to server. Pleae contact administrator", "Error");
            }
        }

        private void btnAddCrew_Click(object sender, RoutedEventArgs e)
        {
            ufn_Publish("E");

            int personId = Convert.ToInt32(txtPublishid.Text);
            string personname = txtPublishname.Text;

            switch (Login.LoginMode)
            {
                case WinAppLibrary.UI.LogMode.OnMode:
                    AssignProcedure(personId + "*" + personname);
                    break;
                case WinAppLibrary.UI.LogMode.OffMode:
                    AssignProcedure(personId + "*" + personname);
                    break;
            }
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

        private async void ufn_PopupInputPin(string mode, string name)
        {
            //this.NotifyUser("popupstart : " , NotifyType.PublishMessage);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    txtPin.Text = "";
                    txtForeman.Text = name;
                    txtTime.Text = DateTime.Now.ToString();
                    switch (mode)
                    {
                        case "OPEN":                            
                            pgrPin.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            grPin.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            break;
                        case "CLOSE":
                            pgrPin.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            grPin.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            break;
                    }
                }
            );
            //this.NotifyUser("popupend : ", NotifyType.PublishMessage);
        }

        private async void btnInputPIN_Click(object sender, RoutedEventArgs e)
        {
            ufn_PopupInputPin((sender as Button).Tag.ToString(),"");

            if (_pinno == txtPin.Text)
            {
                //_toolbox.SetToolboxsign(toolboxSingIn);
                _toolboxsign.Add(toolboxSingIn);
                Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign = _toolboxsign;

                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OffMode)
                {
                    await (new Lib.DataSource.BrassInOutDataSource()).SaveFileToolbox(_toolboxsign, "ToolBoxTalk");
                }

                BindToolboxtalk();
            }
            else
            {
                this.NotifyMessage("The Pin number you entered is incorrect", "Alter!");
            }
        }

        #endregion "ProximityHandler"


        private async void DownloadImg()
        {
            try
            {
                if (Lib.DataSource.ToolBoxTalkSelectionDataSource.strFileName == string.Empty)
                {
                    List<DailytoolboxDTO> dailytoolboxdto = await (new Lib.ServiceModel.ProjectModel()).GetDailytoolboxByDailyBras(Lib.DataSource.BrassInOutDataSource.BrassList[0].DailyBrassID);

                    Lib.DataSource.ToolBoxTalkSelectionDataSource.strFileName = dailytoolboxdto[0].DocumentName;

                }

                var stream = await (new WinAppLibrary.Utilities.Helper()).GetFileStream(Windows.Storage.ApplicationData.Current.LocalFolder, Lib.DataSource.ToolBoxTalkSelectionDataSource.strFileName);
                PdfViewer.LoadDocument(stream);

            }
            catch (Exception ex)
            {
                //imgPopup.Source = img;
            }
        }

        private void lvCrewList_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;

            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
            {
                var source = e.OriginalSource as Grid;
                if (source != null)
                {
                    RemoveCrewOn(source.DataContext as ToolboxsignDTO);
                }
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var source = (sender as Image).Source as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            source.UriSource = new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Crew/default_crew.png");
        }

        //크루 사인인 삭제
        private void RemoveCrewOn(ToolboxsignDTO dto)
        {
            #region "RemoveCrew"
            try
            {
                //Loading(true);
                //DailybrasssignDTO item = lvCrewList.Items.Where(x => (x as DailybrasssignDTO) == dto).FirstOrDefault() as DailybrasssignDTO;
                var item = lvCrewList.Items.Where(x => (x as ToolboxsignDTO) == dto).FirstOrDefault();
                var crew = Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign.Where(AlignmentX => AlignmentX.MyPersonnelID == dto.MyPersonnelID && AlignmentX.SignTimestamp.ToString("yyyyMMdd") == dto.SignTimestamp.ToString("yyyyMMdd")).FirstOrDefault();

                if (item != null)
                {
                    dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;

                    Lib.DataSource.ToolBoxTalkSelectionDataSource.ToolBoxSign.Remove(crew);
                    lvCrewList.Items.Remove(item);
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "RemoveCrew");
                this.NotifyMessage("There was an error to connect to server", "Caution!");
            }
            #endregion "RemoveCrew"
        }

        private void MessageDialog_OkClick(object sender, object e)
        {
            //This semaphore is aimed for waiting until user accepts the result.
            _onHandling = false;
            MessageDialog.Hide(this);
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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CrewBrassIn), Login.UserAccount.PersonnelID);
        }
        
        private void svPdf_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            _pdfread = true;
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
    }
}
