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
    public sealed partial class UCInstrumentCalibrationRecord : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public UCInstrumentCalibrationRecord()
        {
            this.InitializeComponent();
            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }


        public void Load()
        {
            FormSerialize.Load(controls, QAQCDTOList);
            InitControls();
        }

        private void InitControls()
        {
            string strCommentBlank = string.Empty;

            string strComment = "THIS IS TO CERIFY THAT INSTRUMENT {0} HAS BEEN CERTIFIED TO WITHIN THE ACCURACY SPECIFICATI0NS OF THE MANUFACTURER AND THE RANGE LISTED IN THE DATA SHEET AND IS ACCEPTED";
            txtCommentTitle.Text = string.Format(strComment, strCommentBlank);
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
            controls = new List<List<List<FrameworkElement>>> {
                //TODO : Assign Control Grups
                // Basic Infomation
                new List<List<FrameworkElement>> { 
                    //new List<FrameworkElement> { txtCb, txtCalibrationDt, txtDs, txtDsr, txtLood, txtLoodr, txtLocd, txtLocdr, txtINo, txtSno, txtSNa }
                    new List<FrameworkElement> { txtCb, txtCalibrationDt }
                },
                // Name Plate Data
                new List<List<FrameworkElement>> {                     
                    new List<FrameworkElement> { txtMa, txtMo, txtIt, txtFp, txtAd, txtRa, txtSp }
                },
                // Calibration Check
                new List<List<FrameworkElement>> { 
                    // TestInfo - Calibration Equpment
                    new List<FrameworkElement> { txtCEEt, txtCESno, txtCECalibrationDueDt, txtCEBw, txtCEDt },
                    // TestInfo - Test Equpment
                    new List<FrameworkElement> { txtTEEt, txtTESno, txtTECalibrationDueDt, txtTEBw, txtTEDt },
                    // AS Found
                    new List<FrameworkElement> { txtAFPlusZeroFInput, txtAFPlusZeroFUnit, txtAFPlusZeroLInput, txtAFPlusZeroLUnit },
                    new List<FrameworkElement> { txtAFPlusAQuartFInput, txtAFPlusAQuartFUnit, txtAFPlusAQuartLInput, txtAFPlusAQuartLUnit },
                    new List<FrameworkElement> { txtAFPlusHalfFInput, txtAFPlusHalfFUnit, txtAFPlusHalfLInput, txtAFPlusHalfLUnit },
                    new List<FrameworkElement> { txtAFPlusQuartsFInput, txtAFPlusQuartsFUnit, txtAFPlusQuartsLInput, txtAFPlusQuartsLUnit },
                    new List<FrameworkElement> { txtAFPlusFullFInput, txtAFPlusFullFUnit, txtAFPlusFullLInput, txtAFPlusFullLUnit },
                    new List<FrameworkElement> { txtAFMinusQuartsFInput, txtAFMinusQuartsFUnit, txtAFMinusQuartsLInput, txtAFMinusQuartsLUnit },
                    new List<FrameworkElement> { txtAFMinusHalfFInput, txtAFMinusHalfFUnit, txtAFMinusHalfLInput, txtAFMinusHalfLUnit },
                    new List<FrameworkElement> { txtAFMinusAQuartFInput, txtAFMinusAQuartFUnit, txtAFMinusAQuartLInput, txtAFMinusAQuartLUnit },
                    new List<FrameworkElement> { txtAFMinusZeroFInput, txtAFMinusZeroFUnit, txtAFMinusZeroLInput, txtAFMinusZeroLUnit },
                    // AS Left
                    new List<FrameworkElement> { txtALPlusZeroFInput, txtALPlusZeroFUnit, txtALPlusZeroLInput, txtALPlusZeroLUnit },
                    new List<FrameworkElement> { txtALPlusAQuartFInput, txtALPlusAQuartFUnit, txtALPlusAQuartLInput, txtALPlusAQuartLUnit },
                    new List<FrameworkElement> { txtALPlusHalfFInput, txtALPlusHalfFUnit, txtALPlusHalfLInput, txtALPlusHalfLUnit },
                    new List<FrameworkElement> { txtALPlusQuartsFInput, txtALPlusQuartsFUnit, txtALPlusQuartsLInput, txtALPlusQuartsLUnit },
                    new List<FrameworkElement> { txtALPlusFullFInput, txtALPlusFullFUnit, txtALPlusFullLInput, txtALPlusFullLUnit },
                    new List<FrameworkElement> { txtALMinusQuartsFInput, txtALMinusQuartsFUnit, txtALMinusQuartsLInput, txtALMinusQuartsLUnit },
                    new List<FrameworkElement> { txtALMinusHalfFInput, txtALMinusHalfFUnit, txtALMinusHalfLInput, txtALMinusHalfLUnit },
                    new List<FrameworkElement> { txtALMinusAQuartFInput, txtALMinusAQuartFUnit, txtALMinusAQuartLInput, txtALMinusAQuartLUnit },
                    new List<FrameworkElement> { txtALMinusZeroFInput, txtALMinusZeroFUnit, txtALMinusZeroLInput, txtALMinusZeroLUnit },
                    // Comments
                    new List<FrameworkElement> { txtCommentTitle }
                }
            };
            QaqcformdetailDTO header = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Header select lv).FirstOrDefault<QaqcformdetailDTO>();
            txtPna.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            txtPno.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            txtCen.Text = (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            txtCjn.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            txtFIWPName.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            txtSystemNumber.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            txtSystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";
            txtItn.Text = (!string.IsNullOrEmpty(_dto.TagNumber)) ? _dto.TagNumber : "";

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
                // Basic Infomation & Name Plate Data & TestInfo
                // Basic InfomatiotxtFIWPName, txtSystemNumber, txtSystemNamen
                if (txtCb.Text == "" || txtCalibrationDt.Text == "" ||
                    // Name Plate Data
                    txtMa.Text == "" || txtMo.Text == "" || txtIt.Text == "" || txtFp.Text == "" || txtSNo.Text == "" || txtAd.Text == "" || txtRa.Text == "" || txtSp.Text == "" ||
                    // TestInfo - Calibration Equpment
                    txtCEEt.Text == "" || txtCESno.Text == "" || txtCECalibrationDueDt.Text == "" || txtCEBw.Text == "" || txtCEDt.Text == "" ||
                    // TestInfo - Test Equpment
                    txtTEEt.Text == "" || txtTESno.Text == "" || txtTECalibrationDueDt.Text == "" || txtTEBw.Text == "" || txtTEDt.Text == "" ||
                    // AS Found
                    txtAFPlusZeroFInput.Text == "" || txtAFPlusZeroFUnit.Text == "" || txtAFPlusZeroLInput.Text == "" || txtAFPlusZeroLUnit.Text == "" ||
                    txtAFPlusAQuartFInput.Text == "" || txtAFPlusAQuartFUnit.Text == "" || txtAFPlusAQuartLInput.Text == "" || txtAFPlusAQuartLUnit.Text == "" ||
                    txtAFPlusHalfFInput.Text == "" || txtAFPlusHalfFUnit.Text == "" || txtAFPlusHalfLInput.Text == "" || txtAFPlusHalfLUnit.Text == "" ||
                    txtAFPlusQuartsFInput.Text == "" || txtAFPlusQuartsFUnit.Text == "" || txtAFPlusQuartsLInput.Text == "" || txtAFPlusQuartsLUnit.Text == "" ||
                    txtAFPlusFullFInput.Text == "" || txtAFPlusFullFUnit.Text == "" || txtAFPlusFullLInput.Text == "" || txtAFPlusFullLUnit.Text == "" ||
                    txtAFMinusQuartsFInput.Text == "" || txtAFMinusQuartsFUnit.Text == "" || txtAFMinusQuartsLInput.Text == "" || txtAFMinusQuartsLUnit.Text == "" ||
                    txtAFMinusHalfFInput.Text == "" || txtAFMinusHalfFUnit.Text == "" || txtAFMinusHalfLInput.Text == "" || txtAFMinusHalfLUnit.Text == "" ||
                    txtAFMinusAQuartFInput.Text == "" || txtAFMinusAQuartFUnit.Text == "" || txtAFMinusAQuartLInput.Text == "" || txtAFMinusAQuartLUnit.Text == "" ||
                    txtAFMinusZeroFInput.Text == "" || txtAFMinusZeroFUnit.Text == "" || txtAFMinusZeroLInput.Text == "" || txtAFMinusZeroLUnit.Text == "" ||
                    // AS Left
                    txtALPlusZeroFInput.Text == "" || txtALPlusZeroFUnit.Text == "" || txtALPlusZeroLInput.Text == "" || txtALPlusZeroLUnit.Text == "" ||
                    txtALPlusAQuartFInput.Text == "" || txtALPlusAQuartFUnit.Text == "" || txtALPlusAQuartLInput.Text == "" || txtALPlusAQuartLUnit.Text == "" ||
                    txtALPlusHalfFInput.Text == "" || txtALPlusHalfFUnit.Text == "" || txtALPlusHalfLInput.Text == "" || txtALPlusHalfLUnit.Text == "" ||
                    txtALPlusQuartsFInput.Text == "" || txtALPlusQuartsFUnit.Text == "" || txtALPlusQuartsLInput.Text == "" || txtALPlusQuartsLUnit.Text == "" ||
                    txtALPlusFullFInput.Text == "" || txtALPlusFullFUnit.Text == "" || txtALPlusFullLInput.Text == "" || txtALPlusFullLUnit.Text == "" ||
                    txtALMinusQuartsFInput.Text == "" || txtALMinusQuartsFUnit.Text == "" || txtALMinusQuartsLInput.Text == "" || txtALMinusQuartsLUnit.Text == "" ||
                    txtALMinusHalfFInput.Text == "" || txtALMinusHalfFUnit.Text == "" || txtALMinusHalfLInput.Text == "" || txtALMinusHalfLUnit.Text == "" ||
                    txtALMinusAQuartFInput.Text == "" || txtALMinusAQuartFUnit.Text == "" || txtALMinusAQuartLInput.Text == "" || txtALMinusAQuartLUnit.Text == "" ||
                    txtALMinusZeroFInput.Text == "" || txtALMinusZeroFUnit.Text == "" || txtALMinusZeroLInput.Text == "" || txtALMinusZeroLUnit.Text == ""
                    ) //TODO : Validation1
                {
                    checkdata = false;
                }
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
                    // Basic Infomation & Name Plate Data & TestInfo
                    // Basic Infomation
                    if (txtCb.Text == "" || txtCalibrationDt.Text == "" || txtFIWPName.Text == "" || txtSystemNumber.Text == "" ||
                        // Name Plate Data
                        txtMa.Text == "" || txtMo.Text == "" || txtIt.Text == "" || txtFp.Text == "" || txtAd.Text == "" || txtRa.Text == "" || txtSp.Text == "" ||
                        // TestInfo - Calibration Equpment
                        txtCEEt.Text == "" || txtCESno.Text == "" || txtCECalibrationDueDt.Text == "" || txtCEBw.Text == "" || txtCEDt.Text == "" ||
                        // TestInfo - Test Equpment
                        txtTEEt.Text == "" || txtTESno.Text == "" || txtTECalibrationDueDt.Text == "" || txtTEBw.Text == "" || txtTEDt.Text == "" ||
                        // AS Found
                        txtAFPlusZeroFInput.Text == "" || txtAFPlusZeroFUnit.Text == "" || txtAFPlusZeroLInput.Text == "" || txtAFPlusZeroLUnit.Text == "" ||
                        txtAFPlusAQuartFInput.Text == "" || txtAFPlusAQuartFUnit.Text == "" || txtAFPlusAQuartLInput.Text == "" || txtAFPlusAQuartLUnit.Text == "" ||
                        txtAFPlusHalfFInput.Text == "" || txtAFPlusHalfFUnit.Text == "" || txtAFPlusHalfLInput.Text == "" || txtAFPlusHalfLUnit.Text == "" ||
                        txtAFPlusQuartsFInput.Text == "" || txtAFPlusQuartsFUnit.Text == "" || txtAFPlusQuartsLInput.Text == "" || txtAFPlusQuartsLUnit.Text == "" ||
                        txtAFPlusFullFInput.Text == "" || txtAFPlusFullFUnit.Text == "" || txtAFPlusFullLInput.Text == "" || txtAFPlusFullLUnit.Text == "" ||
                        txtAFMinusQuartsFInput.Text == "" || txtAFMinusQuartsFUnit.Text == "" || txtAFMinusQuartsLInput.Text == "" || txtAFMinusQuartsLUnit.Text == "" ||
                        txtAFMinusHalfFInput.Text == "" || txtAFMinusHalfFUnit.Text == "" || txtAFMinusHalfLInput.Text == "" || txtAFMinusHalfLUnit.Text == "" ||
                        txtAFMinusAQuartFInput.Text == "" || txtAFMinusAQuartFUnit.Text == "" || txtAFMinusAQuartLInput.Text == "" || txtAFMinusAQuartLUnit.Text == "" ||
                        txtAFMinusZeroFInput.Text == "" || txtAFMinusZeroFUnit.Text == "" || txtAFMinusZeroLInput.Text == "" || txtAFMinusZeroLUnit.Text == "" ||
                        // AS Left
                        txtALPlusZeroFInput.Text == "" || txtALPlusZeroFUnit.Text == "" || txtALPlusZeroLInput.Text == "" || txtALPlusZeroLUnit.Text == "" ||
                        txtALPlusAQuartFInput.Text == "" || txtALPlusAQuartFUnit.Text == "" || txtALPlusAQuartLInput.Text == "" || txtALPlusAQuartLUnit.Text == "" ||
                        txtALPlusHalfFInput.Text == "" || txtALPlusHalfFUnit.Text == "" || txtALPlusHalfLInput.Text == "" || txtALPlusHalfLUnit.Text == "" ||
                        txtALPlusQuartsFInput.Text == "" || txtALPlusQuartsFUnit.Text == "" || txtALPlusQuartsLInput.Text == "" || txtALPlusQuartsLUnit.Text == "" ||
                        txtALPlusFullFInput.Text == "" || txtALPlusFullFUnit.Text == "" || txtALPlusFullLInput.Text == "" || txtALPlusFullLUnit.Text == "" ||
                        txtALMinusQuartsFInput.Text == "" || txtALMinusQuartsFUnit.Text == "" || txtALMinusQuartsLInput.Text == "" || txtALMinusQuartsLUnit.Text == "" ||
                        txtALMinusHalfFInput.Text == "" || txtALMinusHalfFUnit.Text == "" || txtALMinusHalfLInput.Text == "" || txtALMinusHalfLUnit.Text == "" ||
                        txtALMinusAQuartFInput.Text == "" || txtALMinusAQuartFUnit.Text == "" || txtALMinusAQuartLInput.Text == "" || txtALMinusAQuartLUnit.Text == "" ||
                        txtALMinusZeroFInput.Text == "" || txtALMinusZeroFUnit.Text == "" || txtALMinusZeroLInput.Text == "" || txtALMinusZeroLUnit.Text == ""
                        )  //TODO : Validation2
                    {
                        checkdata = false;
                    }
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
