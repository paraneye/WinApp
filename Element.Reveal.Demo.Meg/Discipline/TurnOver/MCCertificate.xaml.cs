using Element.Reveal.Meg.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.Networking.Proximity;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.TurnOver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MCCertificate : UserControl
    {

        private new List<WinAppLibrary.UI.ObjectNFCSign> signed;
        Popup popup;

        Lib.ProximityHandler ProximityHandler;
        private bool _onHandling = true;
        string _pinno = "";


        public static int systemId { get; set; }
        public static int _projectid { get; set; }
        public static int _moduleid { get; set; }

        RevealProjectSvc.DocumentDTO document = new RevealProjectSvc.DocumentDTO();
        RevealProjectSvc.QaqcformDTO _dtoform = new RevealProjectSvc.QaqcformDTO();
        RevealProjectSvc.QaqcformdetailDTO _dtodetailform = new RevealProjectSvc.QaqcformdetailDTO();
        RevealProjectSvc.QaqcformDTO result = new RevealProjectSvc.QaqcformDTO();

        public MCCertificate()
        {
            this.InitializeComponent();

            signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                new WinAppLibrary.UI.ObjectNFCSign{
                isSigned = "Unsigned",
                MemberGrade = "CR",
                PersonnelName = "",
                SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                SignedTime = null//DateTime.Now.ToString()
            }};
            

            lvNFCSignListConstractor.DataContext = signed;
            lvNFCSignListMegTurnover.DataContext = signed;
            lvNFCSignListMegConstruction.DataContext = signed;
            LoadStoryboad();

            ProximityHandler = new Lib.ProximityHandler();
            ProximityHandler.OnException += ProximityHandler_OnException;
            ProximityHandler.OnMessage += ProximityHandler_OnMessage;

            _onHandling = false;

            ProximityHandler.SetProximityDevice(ProximityDevice.GetDefault());
            lvNFCSignListConstractor.SelectionChanged += lvNFCSignListConstractor_SelectionChanged;
            lvNFCSignListMegConstruction.SelectionChanged += lvNFCSignListMegConstruction_SelectionChanged;
            lvNFCSignListMegTurnover.SelectionChanged += lvNFCSignListMegTurnover_SelectionChanged;            
        }

       

        private void LoadStoryboad()
        {
            sbShow.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(SpinnerScale, 1, 0.3));
            sbShow.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(SpinnerScale, 1, 0.3));
            sbHide.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(SpinnerScale, 0, 0.3));
            sbHide.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(SpinnerScale, 0, 0.3));

            this.SetValue(HorizontalAlignmentProperty, Windows.UI.Xaml.HorizontalAlignment.Stretch);
            this.SetValue(VerticalAlignmentProperty, Windows.UI.Xaml.VerticalAlignment.Stretch);
            this.popup = GetPopup();
        }

        

        private Popup GetPopup()
        {
            Popup retValue = new Popup();
            retValue.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
            retValue.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
            return retValue;
        }

        public void Load()
        {
            Login.MasterPage.HideUserStatus();
            LoadData(false);
            sbHide.Stop();
            sbShow.Begin();
          //  LayoutRoot.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        public void Hide()
        {
            txtContens.Text = "";
            txtDate.Text = "";
            txtSir.Text = "";
            RevealProjectSvc.DocumentDTO document = new RevealProjectSvc.DocumentDTO();
            RevealProjectSvc.QaqcformDTO _dtoform = new RevealProjectSvc.QaqcformDTO();
            RevealProjectSvc.QaqcformdetailDTO _dtodetailform = new RevealProjectSvc.QaqcformdetailDTO();
            RevealProjectSvc.QaqcformDTO result = new RevealProjectSvc.QaqcformDTO();
           
            Clear(null, "");

            Login.MasterPage.ShowUserStatus();
            sbShow.Stop();
            sbHide.Begin();
        }

        private async void LoadData(bool savemode)
        {
            try
            {
                _dtoform = new RevealProjectSvc.QaqcformDTO();
                _dtoform = await (new Lib.ServiceModel.ProjectModel()).GetTurnoverCertificateForMC(_projectid, systemId);
                if (!savemode && _dtoform != null)
                {
                    if (_dtoform.QaqcfromDetails.Count > 0)
                    {
                        _dtodetailform = _dtoform.QaqcfromDetails[0];

                        txtSir.Text = _dtoform.QaqcfromDetails[0].StringValue1;
                        txtContens.Text = _dtoform.QaqcfromDetails[0].StringValue5;

                        if (_dtoform.QaqcfromDetails[0].DateValue1 != null)
                            txtDate.Text = _dtoform.QaqcfromDetails[0].DateValue1.ToString();


                        lvNFCSignListConstractor.DataContext = new List<WinAppLibrary.UI.ObjectNFCSign>{
                        new WinAppLibrary.UI.ObjectNFCSign{
                        PersonnelName = !string.IsNullOrEmpty(_dtoform.ClientSignOffBy) ? _dtoform.ClientSignOffBy+"\r/"+ _dtoform.QaqcfromDetails[0].StringValue2 : "",
                        SignedPinNumber = _dtoform.QaqcfromDetails[0].StringValue2,
                        SignedTime = _dtoform.ClientSignOffDate != null ? _dtoform.ClientSignOffDate.ToString() : "",
                        isSigned = string.IsNullOrEmpty(_dtoform.ClientSignOffBy) ? "Unsigned" : "Signed",
                        MemberGrade = "Contractor",
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                        }};

                            lvNFCSignListMegTurnover.DataContext = new List<WinAppLibrary.UI.ObjectNFCSign>{
                        new WinAppLibrary.UI.ObjectNFCSign{
                        PersonnelName =!string.IsNullOrEmpty(_dtoform.TechnicianSignOffBy) ?_dtoform.TechnicianSignOffBy +"\r/"+ _dtoform.QaqcfromDetails[0].StringValue3:"",
                        SignedPinNumber = _dtoform.QaqcfromDetails[0].StringValue3,
                        SignedTime = _dtoform.TechnicianOffDate != null ? _dtoform.TechnicianOffDate.ToString() : "",
                        isSigned = string.IsNullOrEmpty(_dtoform.ClientSignOffBy) ? "Unsigned" : "Signed",
                        MemberGrade = "Turnover Manager",
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                        }};

                            lvNFCSignListMegConstruction.DataContext = new List<WinAppLibrary.UI.ObjectNFCSign>{
                        new WinAppLibrary.UI.ObjectNFCSign{
                        PersonnelName = !string.IsNullOrEmpty(_dtoform.ContractorSignOffBy)?_dtoform.ContractorSignOffBy+"\r/"+ _dtoform.QaqcfromDetails[0].StringValue4:"",
                        SignedPinNumber = _dtoform.QaqcfromDetails[0].StringValue4,
                        SignedTime = _dtoform.ClientSignOffDate != null ? _dtoform.ClientSignOffDate.ToString() : "",
                        isSigned = string.IsNullOrEmpty(_dtoform.ClientSignOffBy) ? "Unsigned" : "Signed",
                        MemberGrade = "Construction",
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                        }};
                    }
                }
            }
            catch (Exception ex)
            {
            }
            //_dtoform.QAQCFormID = 759;
            //_dtodetailform.QAQCFormDetailID = 2200;
        }

        

        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
      //      PDFViewer.Show();
        //    PDFViewer.LoadPDF("");
            Login.MasterPage.Loading(true, this);
          
            try
            {
                var project = new Lib.ServiceModel.ProjectModel();
                List<RevealProjectSvc.DocumentDTO> _documentlist = new List<RevealProjectSvc.DocumentDTO>();
                document.UpdatedBy = Login.UserAccount.UserName;
                document.UpdatedDate = DateTime.Now;
                document.ProjectID = _projectid;
                _documentlist = await project.SaveTurnoverCertificatePDFForMC(document, _dtoform, systemId);

                PDFViewer.Show();
                if (_documentlist.Count > 0)
                    PDFViewer.LoadPDF(_documentlist[0].LocationURL);
            }
            catch (Exception ex)
            {
            }
            Login.MasterPage.Loading(false, this);
        }

        private async void SaveData()
        {
            Login.MasterPage.Loading(true, this);
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _dtoform.DTOStatus = _dtoform.QAQCFormID > 0 ? (int)WinAppLibrary.Utilities.RowStatus.Update : (int)WinAppLibrary.Utilities.RowStatus.New;
                    _dtoform.SystemID = systemId;
                    _dtoform.QAQCFormTemplateID = Lib.TURNOVERSYSTEM.MCQaqcformtype;
                    _dtoform.QAQCDataTypeLUID = Lib.TURNOVERSYSTEM.QAQCDataTypeLuid;
                    _dtoform.UpdatedBy = Login.UserAccount.UserName;
                    _dtoform.UpdatedDate = DateTime.Now;
                    _dtoform.ProjectID = _projectid;
                    _dtoform.ModuleID = _moduleid;
                    _dtodetailform.InspectionLUID = QAQCGroup.GROUP01;
                    _dtodetailform.InspectedValue = 1;
                    _dtodetailform.StringValue1 = txtSir.Text;
                    _dtodetailform.StringValue5 = txtContens.Text;
                    _dtodetailform.DTOStatus = _dtodetailform.QAQCFormDetailID > 0 ? (int)WinAppLibrary.Utilities.RowStatus.Update : (int)WinAppLibrary.Utilities.RowStatus.New;
                    _dtodetailform.DateValue1 = string.IsNullOrEmpty(txtDate.Text) ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : DateTime.Parse(txtDate.Text);



                    if (lvNFCSignListConstractor.Items.Count > 0 && !string.IsNullOrEmpty(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListConstractor.Items[0]).PersonnelName))
                    {
                        _dtoform.ClientSignOffBy = ((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListConstractor.Items[0]).PersonnelName.Split('\r')[0];
                        if (!string.IsNullOrEmpty(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListConstractor.Items[0]).SignedTime))
                            _dtoform.ClientSignOffDate = DateTime.Parse(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListConstractor.Items[0]).SignedTime);
                        _dtodetailform.StringValue2 = ((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListConstractor.Items[0]).SignedPinNumber;
                    }

                    if (lvNFCSignListMegTurnover.Items.Count > 0 && !string.IsNullOrEmpty(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegTurnover.Items[0]).PersonnelName))
                    {
                        _dtoform.TechnicianSignOffBy = ((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegTurnover.Items[0]).PersonnelName.Split('\r')[0];

                        if (!string.IsNullOrEmpty(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegTurnover.Items[0]).SignedTime))
                            _dtoform.TechnicianOffDate = DateTime.Parse(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegTurnover.Items[0]).SignedTime);
                        _dtodetailform.StringValue3 = ((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegTurnover.Items[0]).SignedPinNumber;
                    }

                    if (lvNFCSignListMegConstruction.Items.Count > 0 && !string.IsNullOrEmpty(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegConstruction.Items[0]).PersonnelName))
                    {
                        _dtoform.ContractorSignOffBy = ((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegConstruction.Items[0]).PersonnelName.Split('\r')[0];
                        if (!string.IsNullOrEmpty(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegConstruction.Items[0]).SignedTime))
                            _dtoform.ContractorSignOffDate = DateTime.Parse(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegConstruction.Items[0]).SignedTime);
                        _dtodetailform.StringValue4 = ((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegConstruction.Items[0]).SignedPinNumber;
                    }
                    _dtoform.QaqcfromDetails = new List<RevealProjectSvc.QaqcformdetailDTO>();
                    _dtoform.QaqcfromDetails.Add(_dtodetailform);

                    document.UpdatedBy = Login.UserAccount.UserName;
                    document.UpdatedDate = DateTime.Now;
                    document.ProjectID = _projectid;
                    foreach (RevealProjectSvc.QaqcformdetailDTO dto in _dtoform.QaqcfromDetails)
                    {
                        if (dto.DateValue1 == DateTime.MinValue) dto.DateValue1 = WinAppLibrary.Utilities.Helper.DateTimeMinValue; // DateTime.Now;
                        if (dto.DateValue2 == DateTime.MinValue) dto.DateValue2 = WinAppLibrary.Utilities.Helper.DateTimeMinValue; // DateTime.Now;
                    }

                    if (_dtoform.AssembledDate.Equals(DateTime.MinValue))
                        _dtoform.AssembledDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;
                    if (_dtoform.ClientSignOffDate.Equals(DateTime.MinValue))
                        _dtoform.ClientSignOffDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;
                    if (_dtoform.ContractorSignOffDate.Equals(DateTime.MinValue))
                        _dtoform.ContractorSignOffDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;
                    if (_dtoform.TechnicianOffDate.Equals(DateTime.MinValue))
                        _dtoform.TechnicianOffDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;
                    if (_dtoform.PartialDate.Equals(DateTime.MinValue))
                        _dtoform.PartialDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;

                });
                    var project = new Lib.ServiceModel.ProjectModel();
                    result = await project.SaveTurnoverCertificateForMC(document, _dtoform, systemId);
               

            }
            catch (Exception ex)
            {
            }
            LoadData(true);
            Login.MasterPage.Loading(false, this);
            _onHandling = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Clear(null,"");
            txtContens.Text = "";
            txtDate.Text = "";
            txtSir.Text = "";
            Login.MasterPage.ShowUserStatus();
            sbShow.Stop();
            sbHide.Begin();
        }

        #region sign 
        private void lvNFCSignListConstractor_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
                Clear(lvNFCSignListConstractor,"");
        }

        private void lvNFCSignListMegTurnover_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
                Clear(lvNFCSignListMegTurnover,"");
        }


        private void lvNFCSignListMegConstruction_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
                Clear(lvNFCSignListMegConstruction,"");
        }

        private void Clear(ListView lv, string mode)
        {
            signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                new WinAppLibrary.UI.ObjectNFCSign{
                isSigned = "UnSigned",
                PersonnelName = "",
                MemberGrade = "",
                SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                SignedTime = "",
                SignedPinNumber = ""
                }};
            if (mode.Equals("All"))
            {
                lvNFCSignListConstractor.DataContext = signed;
                lvNFCSignListMegConstruction.DataContext = signed;
                lvNFCSignListMegTurnover.DataContext = signed;
            }
            else
                lv.DataContext = signed;
        }

        
        void lvNFCSignListConstractor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvNFCSignListConstractor.SelectedItems.Count > 0)
            {
                lvNFCSignListMegTurnover.SelectedItem = null;
                lvNFCSignListMegConstruction.SelectedItem = null;
              //  if (SelectedSign != null)
                 //   SelectedSign(this, null);
            }
        }

        void lvNFCSignListMegTurnover_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvNFCSignListMegTurnover.SelectedItems.Count > 0)
            {
                lvNFCSignListConstractor.SelectedItem = null;
                lvNFCSignListMegConstruction.SelectedItem = null;
            }
        }

        void lvNFCSignListMegConstruction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvNFCSignListMegConstruction.SelectedItems.Count > 0)
            {
                lvNFCSignListConstractor.SelectedItem = null;
                lvNFCSignListMegTurnover.SelectedItem = null;
            }
        }
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
                        Setsigne(personId, personname, _pinno);
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

        public async void NotifyMessage(string msg, string title)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //    MessageDialog.DialogTitle = title;
                //    MessageDialog.Content = msg;
                //    MessageDialog.Show(this);
            });
        }
       

        private async void Setsigne(int personId, string personname, string _pinno)
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (lvNFCSignListConstractor.SelectedItems.Count > 0)
                    {
                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                            new WinAppLibrary.UI.ObjectNFCSign{
                            isSigned = "Signed",
                            PersonnelName = personname +"\r/"+_pinno,
                            SignedPinNumber = _pinno,
                            MemberGrade = "Contractor",
                            SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                            SignedTime = DateTime.Now.ToString()
                            }
                        };
                        lvNFCSignListConstractor.DataContext = signed;
                        
                    }
                    else if (lvNFCSignListMegTurnover.SelectedItems.Count > 0)
                    {
                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                            new WinAppLibrary.UI.ObjectNFCSign{
                            isSigned = "Signed",
                            PersonnelName = personname +"\r/"+_pinno,
                            SignedPinNumber = _pinno,
                            MemberGrade = "TurnoverManager",
                            SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                            SignedTime = DateTime.Now.ToString()
                            }
                        };
                        lvNFCSignListMegTurnover.DataContext = signed;
                    }
                    else if (lvNFCSignListMegConstruction.SelectedItems.Count > 0)
                    {
                        signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                            new WinAppLibrary.UI.ObjectNFCSign{
                            isSigned = "Signed",
                            PersonnelName = personname +"\r/"+_pinno,
                            SignedPinNumber = _pinno,
                            MemberGrade = "Construction",
                            SignedColor = new SolidColorBrush(Windows.UI.Colors.Blue),
                            SignedTime = DateTime.Now.ToString()
                            }
                        };
                        lvNFCSignListMegConstruction.DataContext = signed;    
                    }

                    if (!string.IsNullOrEmpty(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegTurnover.Items[0]).PersonnelName) &&
                        !string.IsNullOrEmpty(((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListMegConstruction.Items[0]).PersonnelName))
                        txtDate.Text = DateTime.Now.ToString();
                    SaveData();
                });                
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
