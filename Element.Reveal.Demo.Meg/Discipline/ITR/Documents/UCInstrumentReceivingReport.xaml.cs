using Element.Reveal.Meg.Lib;
using Element.Reveal.Meg.RevealProjectSvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCInstrumentReceivingReport : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public UCInstrumentReceivingReport()
        {
            this.InitializeComponent();
            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

        public void Load()
        {
            FormSerialize.Load(controls, QAQCDTOList);
        }

        public void Save()
        {
            QAQCDTOList.Clear();
            //TODO : Group에 따라 작성
            FormSerialize.GenDTO(QAQCGroup.GROUP01, controls[0], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP02, controls[1], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP03, controls[2], QAQCDTOList);
        }

        public void DoAfter(RevealProjectSvc.QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>>
            {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtTB, dtTestDate, txtRange }
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtMf, txtModel, txtSNo, txtCSize, txtPON }
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { chkNA1, rdY1Yes, rdY1No },
                    new List<FrameworkElement> { chkNA2, rdY2Yes, rdY2No },
                    new List<FrameworkElement> { chkNA3, rdY3Yes, rdY3No },
                    new List<FrameworkElement> { chkNA4, rdY4Yes, rdY4No },
                    new List<FrameworkElement> { chkNA5, rdY5Yes, rdY5No },
                    new List<FrameworkElement> { chkNA6, rdY6Yes, rdY6No },
                    new List<FrameworkElement> { chkNA7, rdY7Yes, rdY7No },
                    new List<FrameworkElement> { chkNA8, rdY8Yes, rdY8No },
                    new List<FrameworkElement> { chkNA9, rdY9Yes, rdY9No },
                    new List<FrameworkElement> { chkNA10, rdY10Yes, rdY10No },
                    new List<FrameworkElement> { chkNA11, rdY11Yes, rdY11No },
                    new List<FrameworkElement> { txtLocs, txtNote1, txtCom }
                }
            };
            QaqcformdetailDTO header = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Header select lv).FirstOrDefault<QaqcformdetailDTO>();
            ProjectName.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            ProjectNumber.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            CWPName.Text = (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            JobNumber.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            txtFIWPName.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            txtSystemNumber.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            txtSystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";
            txtITN.Text = (!string.IsNullOrEmpty(_dto.TagNumber)) ? _dto.TagNumber : "";

            List<DrawingDTO> drawingDto = _dto.QaqcrefDrawing;
            if(drawingDto != null)
                lvDrawingDSheet.ItemsSource = drawingDto;
        }

        #region NFC Sign
        public bool isExistNFC { get { return false; } }
        public bool isSigned { get { return false; } }
        public bool isSelectedSign { get { return false; } set { } }
        public void SetNFCData(string _personmane, string _grade) { }
        public void ClearSelect() { }
        public event EventHandler SelectedSign;
        public void checkSelectSign() { }
        #endregion

        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }

        public bool Validate()
        {
            return true;
            bool checkdata = true;
            try
            {
                if (!(bool)chkNA1.IsChecked && !(bool)rdY1Yes.IsChecked && !(bool)rdY1No.IsChecked) checkdata = false;
                if (!(bool)chkNA2.IsChecked && !(bool)rdY2Yes.IsChecked && !(bool)rdY2No.IsChecked) checkdata = false;
                if (!(bool)chkNA3.IsChecked && !(bool)rdY3Yes.IsChecked && !(bool)rdY3No.IsChecked) checkdata = false;
                if (!(bool)chkNA4.IsChecked && !(bool)rdY4Yes.IsChecked && !(bool)rdY4No.IsChecked) checkdata = false;
                if (!(bool)chkNA5.IsChecked && !(bool)rdY5Yes.IsChecked && !(bool)rdY5No.IsChecked) checkdata = false;
                if (!(bool)chkNA6.IsChecked && !(bool)rdY6Yes.IsChecked && !(bool)rdY6No.IsChecked) checkdata = false;
                if (!(bool)chkNA7.IsChecked && !(bool)rdY7Yes.IsChecked && !(bool)rdY7No.IsChecked) checkdata = false;
                if (!(bool)chkNA8.IsChecked && !(bool)rdY8Yes.IsChecked && !(bool)rdY8No.IsChecked) checkdata = false;
                if (!(bool)chkNA9.IsChecked && !(bool)rdY9Yes.IsChecked && !(bool)rdY9No.IsChecked) checkdata = false;
                if (!(bool)chkNA10.IsChecked && !(bool)rdY10Yes.IsChecked && !(bool)rdY10No.IsChecked) checkdata = false;
                if (!(bool)chkNA11.IsChecked && !(bool)rdY11Yes.IsChecked && !(bool)rdY11No.IsChecked) checkdata = false;

                if (txtFIWPName.Text == "" || txtSystemNumber.Text == "" || txtSystemName.Text == "") checkdata = false;
                if (txtTB.Text == "" ) checkdata = false;
                if (txtRange.Text == "" || txtMf.Text == "" || txtModel.Text == "" || txtSNo.Text == "" || txtCSize.Text == "" || txtPON.Text == "") checkdata = false;
                if (txtLocs.Text == "" || txtNote1.Text == "" || txtCom.Text == "") checkdata = false;
            }
            catch (Exception ex)
            {
                checkdata = false;
            }
            return checkdata;
        }

        public async Task<bool> Validate2()
        {
            return true;
            bool checkdata = true;
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (!(bool)chkNA1.IsChecked && !(bool)rdY1Yes.IsChecked && !(bool)rdY1No.IsChecked) checkdata = false;
                    if (!(bool)chkNA2.IsChecked && !(bool)rdY2Yes.IsChecked && !(bool)rdY2No.IsChecked) checkdata = false;
                    if (!(bool)chkNA3.IsChecked && !(bool)rdY3Yes.IsChecked && !(bool)rdY3No.IsChecked) checkdata = false;
                    if (!(bool)chkNA4.IsChecked && !(bool)rdY4Yes.IsChecked && !(bool)rdY4No.IsChecked) checkdata = false;
                    if (!(bool)chkNA5.IsChecked && !(bool)rdY5Yes.IsChecked && !(bool)rdY5No.IsChecked) checkdata = false;
                    if (!(bool)chkNA6.IsChecked && !(bool)rdY6Yes.IsChecked && !(bool)rdY6No.IsChecked) checkdata = false;
                    if (!(bool)chkNA7.IsChecked && !(bool)rdY7Yes.IsChecked && !(bool)rdY7No.IsChecked) checkdata = false;
                    if (!(bool)chkNA8.IsChecked && !(bool)rdY8Yes.IsChecked && !(bool)rdY8No.IsChecked) checkdata = false;
                    if (!(bool)chkNA9.IsChecked && !(bool)rdY9Yes.IsChecked && !(bool)rdY9No.IsChecked) checkdata = false;
                    if (!(bool)chkNA10.IsChecked && !(bool)rdY10Yes.IsChecked && !(bool)rdY10No.IsChecked) checkdata = false;
                    if (!(bool)chkNA11.IsChecked && !(bool)rdY11Yes.IsChecked && !(bool)rdY11No.IsChecked) checkdata = false;

                    if (txtFIWPName.Text == "" || txtSystemNumber.Text == "" || txtSystemName.Text == "") checkdata = false;
                    if (txtITN.Text == "" || txtTB.Text == "") checkdata = false;
                    if (txtRange.Text == "" || txtMf.Text == "" || txtModel.Text == "" || txtSNo.Text == "" || txtCSize.Text == "" || txtPON.Text == "") checkdata = false;
                    if (txtLocs.Text == "" || txtNote1.Text == "" || txtCom.Text == "") checkdata = false;
                });
            }
            catch (Exception ex)
            {
                checkdata = false;
            }
            return checkdata;
        }
    }
}
