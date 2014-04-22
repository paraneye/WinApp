using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using Element.Reveal.Manage.RevealProjectSvc;
namespace Element.Reveal.Manage.Discipline.Survey
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TEST_Verify : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid, _cwpId = 0, _projectscheduleId = 0, _fiwpId = 0, _drawingId = 0;
        private string _drawingNumber = "";
        Lib.UI.FailReason uiReason;
        private List<RevealCommonSvc.ComboBoxDTO> reasons = new List<RevealCommonSvc.ComboBoxDTO>();
        Popup modal;

        public TEST_Verify()
        {
            this.InitializeComponent();

            Login.MasterPage.SetPageTitle("Quantity Survey - Components Test");
            btn.Click += btn_Click;
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            uiVerify.Save();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            // QS_SelectScheduleLineItemNIWP로부터 Objct받고 이를 기반으로 정보 추출
            _cwpId = 1;
            _projectscheduleId = 55;
            _fiwpId = 39;
            _drawingId = 42;
            _drawingNumber = " Test numbers ";

            InitControls();
        }

        private void InitControls()
        {
            uiVerify.AddEditComment += uiVerify_AddEditComment;
            uiVerify.DTOChanged += uiVerify_DTOChanged;
            uiVerify.LoadData(_cwpId, _projectscheduleId, _fiwpId, _drawingId, _drawingNumber);

            GetPredefined();
        }

        private async void GetPredefined()
        {
            reasons = await (new Element.Reveal.Manage.Lib.ServiceModel.CommonModel()).GetFailReason_Combo();
        }

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
            btn.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void uiVerify_AddEditComment(object sender, EventArgs e)
        {
            OpenModal(((Lib.Common.QuantitySurveyEventArg)e).EventDTO);
        }

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
    }
}
