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
    public sealed partial class UCPowerCableInspectionExhibit : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public UCPowerCableInspectionExhibit()
        {
            this.InitializeComponent();
            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { dtpInspectionDate, txtInspectedBy } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtD_InitialMTRMark, txtD_EndMTRMark, txtD_ActualLength },
                    new List<FrameworkElement> { txtD_Print, txtD_Sign, dtpD_Date},
                },
                new List<List<FrameworkElement>> {                    
                    new List<FrameworkElement> { txtGT_From, txtGT_To, txtGT_PrintName, dtpGT_Date},
                    new List<FrameworkElement> { txtGS_From, txtGS_To, txtGS_PrintName, dtpGS_Date},
                    new List<FrameworkElement> { txtGC_From, txtGC_To, txtGC_PrintName, dtpGC_Date},
                    new List<FrameworkElement> { txtP_AGND, txtP_BGND, txtP_CGND, txtP_NGND, txtP_ABCNGND, txtP_Comment},
                    new List<FrameworkElement> { txtTestEquipType, txtTestSerial, txtTestCalibrationDueDate, txtTestByWhom, txtTestDate}
                },
                new List<List<FrameworkElement>>{
                    new List<FrameworkElement> { txtTT_From, txtTT_To, txtTT_PrintName, dtpTT_Date},
                    new List<FrameworkElement> { txtTS_From, txtTS_To, txtTS_PrintName, dtpTS_Date},
                    new List<FrameworkElement> { txtTC_From, txtTC_To, txtTC_PrintName, dtpTC_Date}
                }
            };
            //Header Binding
            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();

            lblProjectName.Text =  (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            lblProjectNumber.Text =  (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            lblCWPNo.Text =  (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            lblContractorJobNo.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";

            //lblCableSCHEDNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue1)) ? HeaderDto.StringValue1 : "";
            //lblCableSCHEDrev.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue2)) ? HeaderDto.StringValue2 : "";
            //lblRFIDRWGNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue3)) ? HeaderDto.StringValue3 : "";
            //lblRFIDRWGrev.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue4)) ? HeaderDto.StringValue4 : "";
            //lblFromLocationDRWGNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue5)) ? HeaderDto.StringValue5 : "";
            //lblFromLocationDRWGrev.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue6)) ? HeaderDto.StringValue6 : "";
            //lblToLocationDRWGNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue7)) ? HeaderDto.StringValue7 : "";
            //lblToLocationDRWGrev.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue8)) ? HeaderDto.StringValue8 : "";

            lblIWPNo.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            lblSystemNo.Text =  (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            lblSystemName.Text =  (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";

            if (HeaderDto != null)
            {
                txtCableTagNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue9)) ? HeaderDto.StringValue9 : "";
                lblEstLength.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue10)) ? HeaderDto.StringValue10 : "";
                lblReelNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue11)) ? HeaderDto.StringValue11 : "";
                lblCableType.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue12)) ? HeaderDto.StringValue12 : "";
                lblShield.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue13)) ? HeaderDto.StringValue13 : "";
                lblVoltageRating.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue14)) ? HeaderDto.StringValue14 : "";
                lblConductors.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue15)) ? HeaderDto.StringValue15 : "";
                lblInsul.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue16)) ? HeaderDto.StringValue16 : "";
                lblCableSizeAWG.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue17)) ? HeaderDto.StringValue17 : "";
            }

            lblD_From.Text = "";
            lblD_To.Text = "";
            lblD_TrayRouting.Text = "";

            this.txtInspectedBy.Text = Login.UserAccount.UserName;

            //Drawing
            List<DrawingDTO> drawingDto = _dto.QaqcrefDrawing;
            if(drawingDto != null)
                lvDrawing.ItemsSource = drawingDto;
        }

        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }
        public async Task<bool> Validate2()
        {
            return true;
            bool checkdata = true;
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (txtInspectedBy.Text == "" || txtD_InitialMTRMark.Text == "" || txtD_EndMTRMark.Text == "" || txtD_ActualLength.Text == "" || txtGC_From.Text == "" || txtGC_PrintName.Text == "" ||
                        txtGC_To.Text == "" || txtGS_From.Text == "" || txtGS_PrintName.Text == "" || txtGS_To.Text == "" || txtGT_From.Text == "" || txtGT_PrintName.Text == "" || txtGT_To.Text == "" ||
                        txtP_ABCNGND.Text == "" || txtP_AGND.Text == "" || txtP_BGND.Text == "" || txtP_CGND.Text == "" || txtP_Comment.Text == "" || txtP_NGND.Text == "" || txtTC_From.Text == "" ||
                        txtTC_PrintName.Text == "" || txtTC_To.Text == "" || txtTestByWhom.Text == "" || txtTestCalibrationDueDate.Text == "" || txtTestDate.Text == "" || txtTestEquipType.Text == "" ||
                        txtTestSerial.Text == "" || txtTS_From.Text == "" || txtTS_PrintName.Text == "" || txtTS_To.Text == "" || txtTT_From.Text == "" || txtTT_PrintName.Text == "" || txtTT_To.Text == ""
                        )
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

        public bool Validate()
        {
            return true;
            bool checkdata = true;
            try
            {
                if (txtInspectedBy.Text == "" || txtD_InitialMTRMark.Text == "" || txtD_EndMTRMark.Text == "" || txtD_ActualLength.Text == "" || txtGC_From.Text == "" || txtGC_PrintName.Text == "" ||
                    txtGC_To.Text == "" || txtGS_From.Text == "" || txtGS_PrintName.Text == "" || txtGS_To.Text == "" || txtGT_From.Text == "" || txtGT_PrintName.Text == "" || txtGT_To.Text == "" ||
                    txtP_ABCNGND.Text == "" || txtP_AGND.Text == "" || txtP_BGND.Text == "" || txtP_CGND.Text == "" || txtP_Comment.Text == "" || txtP_NGND.Text == "" || txtTC_From.Text == "" ||
                    txtTC_PrintName.Text == "" || txtTC_To.Text == "" || txtTestByWhom.Text == "" || txtTestCalibrationDueDate.Text == "" || txtTestDate.Text == "" || txtTestEquipType.Text == "" ||
                    txtTestSerial.Text == "" || txtTS_From.Text == "" || txtTS_PrintName.Text == "" || txtTS_To.Text == "" || txtTT_From.Text == "" || txtTT_PrintName.Text == "" || txtTT_To.Text == ""
                    )
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

        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public void Load()
        {
            FormSerialize.Load(controls, QAQCDTOList);
        }

        public void Save()
        {
            QAQCDTOList.Clear();
            FormSerialize.GenDTO(QAQCGroup.GROUP01, controls[0], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP02, controls[1], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP03, controls[2], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP04, controls[3], QAQCDTOList);
        }

        public bool isExistNFC
        {
            get { return false; }
        }
        public bool isSigned
        {
            get { return false; }
        }

        public void SetNFCData(string _personmane, string _grade)
        {
        }

        public void ClearSelect()
        {
            
        }

        public event EventHandler SelectedSign;

        public bool isSelectedSign
        {
            get { return false; }
            set { }
        }

        public void checkSelectSign()
        {
        }
    }
}
