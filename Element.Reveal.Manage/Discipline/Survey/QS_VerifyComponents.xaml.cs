using System;
using System.Collections.Generic;
using System.Linq;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using WinAppLibrary.Extensions;
using WinAppLibrary.ServiceModels;
using WinAppLibrary.Utilities;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Element.Reveal.Manage.Discipline.Survey
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QS_VerifyComponents : WinAppLibrary.Controls.LayoutAwarePage
    {
        private bool _isNotSaved = false;  // result save status
        private int _projectid, _moduleid, _cwpId, _projectscheduleId, _fiwpId, _drawingId;
        private string _drawingNumber;
        private List<RevealProjectSvc.DrawingDTO> drawingList;
        Lib.UI.FailReason uiReason;
        private List<RevealCommonSvc.ComboBoxDTO> reasons = new List<RevealCommonSvc.ComboBoxDTO>();
        Popup modal;

        public QS_VerifyComponents()
        {
            this.InitializeComponent();

            Login.MasterPage.ShowTopBanner = true;
            Login.MasterPage.ShowBackButton = false;
            Login.MasterPage.SetPageTitle("Quantity Survey - Verify Components");
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.Loading(true, this);

            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            ObjectDisc objParam = new ObjectDisc();
            objParam = navigationParameter as ObjectDisc;

            //_cwpId = objParam.CWPID;
            //_fiwpId = objParam.FIWPID;
            //_projectscheduleId = objParam.ProjectScheduleID;

            _cwpId = 1;// objParam.CWPID;
            _fiwpId = 43;// objParam.FIWPID;
            _projectscheduleId = 65;// objParam.ProjectScheduleID;

            // VerifyComponent Event set
            InitControls();

            // data binding & Darwing Thumbnail List set
            DrawingList(_cwpId, _projectscheduleId, _fiwpId);

            Login.MasterPage.Loading(false, this);
        }

        #region private Method
        private void InitControls()
        {
            uiVerify.AddEditComment += uiVerify_AddEditComment;
            uiVerify.DTOChanged += uiVerify_DTOChanged;
            GetPredefined();
        }

        private async void GetPredefined()
        {
            reasons = await (new Element.Reveal.Manage.Lib.ServiceModel.CommonModel()).GetFailReason_Combo();
        }

        private async void DrawingList(int cwpId, int projectscheduleId, int fiwpId)
        {
            // get List<RevealProjectSvc.DrawingDTO>
            drawingList = await (new Lib.ServiceModel.CommonModel()).GetDrawingByFiwpProgressCompleted(_projectid, _moduleid, cwpId, projectscheduleId, fiwpId);

            if (drawingList != null && drawingList.Count > 0)
            {
                DataGroup group = new DataGroup("Drawing", "", "");
                List<DataGroup> source = new List<DataGroup>();
                group.Items = drawingList.Select(x => new DataItem(x.DrawingID.ToString(), x.DrawingName, x.DrawingFilePath + x.DrawingFileURL, x.Description, group) { }).ToObservableCollection();
                source.Add(group);
                this.DefaultViewModel["Drawings"] = source;  // Data binding
            }
        }

        private void ItemClik(DataItem item)
        {
            if (item != null)
            {
                var drawing = drawingList.Where(x => x.DrawingID.ToString() == item.UniqueId).FirstOrDefault();
                DrawingEditor.LoadDrawing(drawing.DrawingFilePath, drawing.DrawingFileURL, UriKind.Absolute);

                _drawingId = drawing.DrawingID;
                _drawingNumber = string.IsNullOrEmpty(drawing.DrawingNo) ? "" : drawing.DrawingNo;

                uiVerify.LoadData(_cwpId, _projectscheduleId, _fiwpId, _drawingId, _drawingNumber);
            }
        }
        #endregion

        private void CloseModal()
        {
            modal.IsOpen = false;
        }

        private void OpenModal(RevealProjectSvc.QuantityserveyDTO _dto)
        {
            uiReason = new Lib.UI.FailReason();
            uiReason.Updated += uiReason_Updated;
            uiReason.Canceled += uiReason_Canceled;
            uiReason.reasons = reasons;
            uiReason.LoadData(_dto);

            modal = new Popup();
            modal.Width = Window.Current.Bounds.Width;
            modal.Height = Window.Current.Bounds.Height;
            modal.Child = uiReason;
            modal.IsOpen = true;
        }

        #region Event Handler
        private async void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isNotSaved == false)
            {
                Login.MasterPage.GoBack();
                return;
            }

            bool result = await Helper.YesOrNoMessage("The curren result is not saved yet. Are you sure to ignore all your progressed status?", "");
            if (result == true)
            {
                Login.MasterPage.GoBack();
            }
        }

        private async void lvDrawing_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DataItem;

            if (_isNotSaved == false)
            {
                ItemClik(item);
                return;
            }

            bool result = await Helper.YesOrNoMessage("The curren result is not saved yet. Are you sure to ignore all your progressed status?", "");
            if (result == true)
            {
                ItemClik(item);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);
            uiVerify.Save();
            Login.MasterPage.Loading(false, this);

            if (uiVerify.SaveResult == true)
            {
                // To enable Toast Notification; you need to enable Toast capable of the application in Application manifest.
                var notificationXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                var toastElements = notificationXml.GetElementsByTagName("text");
                toastElements[0].AppendChild(notificationXml.CreateTextNode("You have successfully saved your data"));

                var toastNotification = new ToastNotification(notificationXml);
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            }

            if (uiVerify.RemainDTOCount == 0)
            {
                Login.MasterPage.GoBack();
            }
        }

        #region VerifyComponent Event
        void uiReason_Canceled(object sender, EventArgs e)
        {
            CloseModal();
        }

        void uiReason_Updated(object sender, EventArgs e)
        {
            uiVerify.UpdateItems();
            CloseModal();
        }

        void uiVerify_DTOChanged(object sender, EventArgs e)
        {
            saveButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void uiVerify_AddEditComment(object sender, EventArgs e)
        {
            OpenModal(((Lib.Common.QuantitySurveyEventArg)e).EventDTO);
        }
        #endregion

        #endregion
    }
}
