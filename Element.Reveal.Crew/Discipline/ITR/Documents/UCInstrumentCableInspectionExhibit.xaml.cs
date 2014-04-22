using Element.Reveal.Crew.Lib;
using Element.Reveal.Crew.RevealProjectSvc;
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

namespace Element.Reveal.Crew.Discipline.ITR
{
    public sealed partial class UCInstrumentCableInspectionExhibit : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public UCInstrumentCableInspectionExhibit()
        {
            this.InitializeComponent();
        }

        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtInspectionDate, txtInspectedBy } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtD_InitialMTRMark, txtD_EndMTRMark, txtD_ActualLength },
                    new List<FrameworkElement> { txtD_Print, txtD_Sign, txtD_Date}
                  
                },
                new List<List<FrameworkElement>>{
                    new List<FrameworkElement> { txtGT_From, txtGT_To, txtGT_PrintName, txtGT_Date},
                    new List<FrameworkElement> { txtGS_From, txtGS_To, txtGS_PrintName, txtGS_Date},
                    new List<FrameworkElement> { txtGC_From, txtGC_To, txtGC_PrintName, txtGC_Date},
                    new List<FrameworkElement> { txtP_CC, txtP_CG, txtP_CS, txtP_SG, txtP_Comment},
                    new List<FrameworkElement> { txtTestEquipType, txtTestSerial, txtTestCalibrationDueDate, txtTestByWhom, txtTestDate}
                },
                new List<List<FrameworkElement>>{ 
                    new List<FrameworkElement> { txtTT_From, txtTT_To, txtTT_PrintName, txtTT_Date},
                    new List<FrameworkElement> { txtTS_From, txtTS_To, txtTS_PrintName, txtTS_Date},
                    new List<FrameworkElement> { txtTC_From, txtTC_To, txtTC_PrintName, txtTC_Date},
                }
            };

            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();

            lblProjectName.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            lblProjectNumber.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
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

            lblIWPNo.Text =  (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            lblSystemNo.Text =  (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            lblSystemName.Text =  (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";

            cbCableTagNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue9)) ? HeaderDto.StringValue9 : "";
            //cbCableTagNo.Items.Add(HeaderDto.StringValue9);
            lblEstLength.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue10)) ? HeaderDto.StringValue10 : "";
            lblReelNo.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue11)) ? HeaderDto.StringValue11 : "";
            lblCableType.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue12)) ? HeaderDto.StringValue12 : "";
            lblShield.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue13)) ? HeaderDto.StringValue13 : "";
            lblVoltageRating.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue14)) ? HeaderDto.StringValue14 : "";
            lblConductors.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue15)) ? HeaderDto.StringValue15 : "";
            // = HeaderDto.StringValue16;
            lblCableSizeAWG.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue17)) ? HeaderDto.StringValue17 : "";

            this.txtInspectedBy.Text = Login.UserAccount.UserName;

            //Drawing
            List<DrawingDTO> drawingDTO = _dto.QaqcrefDrawing;
            lvDrawing.ItemsSource = drawingDTO;

        }
        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }

        public async Task<bool> Validate2() 
        {
            bool checkdata = true;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {

                if (!string.IsNullOrEmpty(txtInspectionDate.Text) && !string.IsNullOrEmpty(txtInspectedBy.Text) && !string.IsNullOrEmpty(txtD_InitialMTRMark.Text)
                    && !string.IsNullOrEmpty(txtD_EndMTRMark.Text) && !string.IsNullOrEmpty(txtD_ActualLength.Text) && !string.IsNullOrEmpty(txtD_Print.Text)
                    && !string.IsNullOrEmpty(txtD_Sign.Text) && !string.IsNullOrEmpty(txtD_Date.Text)
                    && !string.IsNullOrEmpty(txtGT_From.Text) && !string.IsNullOrEmpty(txtGT_To.Text) && !string.IsNullOrEmpty(txtGT_PrintName.Text) 
                    && !string.IsNullOrEmpty(txtGT_Date.Text)
                    && !string.IsNullOrEmpty(txtGS_From.Text) && !string.IsNullOrEmpty(txtGS_To.Text) && !string.IsNullOrEmpty(txtGS_PrintName.Text) 
                    && !string.IsNullOrEmpty(txtGS_Date.Text) 
                    && !string.IsNullOrEmpty(txtGC_From.Text) && !string.IsNullOrEmpty(txtGC_To.Text) && !string.IsNullOrEmpty(txtGC_PrintName.Text) 
                    && !string.IsNullOrEmpty(txtGC_Date.Text)
                    && !string.IsNullOrEmpty(txtP_CC.Text) && !string.IsNullOrEmpty(txtP_CG.Text) && !string.IsNullOrEmpty(txtP_CS.Text) 
                    && !string.IsNullOrEmpty(txtP_SG.Text) && !string.IsNullOrEmpty(txtP_Comment.Text)
                    && !string.IsNullOrEmpty(txtTestEquipType.Text) && !string.IsNullOrEmpty(txtTestSerial.Text) && !string.IsNullOrEmpty(txtTestCalibrationDueDate.Text) 
                    && !string.IsNullOrEmpty(txtTestByWhom.Text) && !string.IsNullOrEmpty(txtTestDate.Text)

                    && !string.IsNullOrEmpty(txtTT_From.Text) && !string.IsNullOrEmpty(txtTT_To.Text) && !string.IsNullOrEmpty(txtTT_PrintName.Text) && !string.IsNullOrEmpty(txtTT_Date.Text)
                    && !string.IsNullOrEmpty(txtTS_From.Text) && !string.IsNullOrEmpty(txtTS_To.Text) && !string.IsNullOrEmpty(txtTS_PrintName.Text) && !string.IsNullOrEmpty(txtTS_Date.Text)
                    && !string.IsNullOrEmpty(txtTC_From.Text) && !string.IsNullOrEmpty(txtTC_To.Text) && !string.IsNullOrEmpty(txtTC_PrintName.Text) && !string.IsNullOrEmpty(txtTC_Date.Text)
                    )
                {
                    checkdata = true;
                }
                else
                    checkdata = false;
            });
            return checkdata;
        }

        public bool Validate()
        {
            bool checkdata = true;
            try
            {
                if (!string.IsNullOrEmpty(txtInspectionDate.Text) && !string.IsNullOrEmpty(txtInspectedBy.Text) && !string.IsNullOrEmpty(txtD_InitialMTRMark.Text)
                        && !string.IsNullOrEmpty(txtD_EndMTRMark.Text) && !string.IsNullOrEmpty(txtD_ActualLength.Text) && !string.IsNullOrEmpty(txtD_Print.Text)
                        && !string.IsNullOrEmpty(txtD_Sign.Text) && !string.IsNullOrEmpty(txtD_Date.Text)
                        && !string.IsNullOrEmpty(txtGT_From.Text) && !string.IsNullOrEmpty(txtGT_To.Text) && !string.IsNullOrEmpty(txtGT_PrintName.Text)
                        && !string.IsNullOrEmpty(txtGT_Date.Text)
                        && !string.IsNullOrEmpty(txtGS_From.Text) && !string.IsNullOrEmpty(txtGS_To.Text) && !string.IsNullOrEmpty(txtGS_PrintName.Text)
                        && !string.IsNullOrEmpty(txtGS_Date.Text)
                        && !string.IsNullOrEmpty(txtGC_From.Text) && !string.IsNullOrEmpty(txtGC_To.Text) && !string.IsNullOrEmpty(txtGC_PrintName.Text)
                        && !string.IsNullOrEmpty(txtGC_Date.Text)
                        && !string.IsNullOrEmpty(txtP_CC.Text) && !string.IsNullOrEmpty(txtP_CG.Text) && !string.IsNullOrEmpty(txtP_CS.Text)
                        && !string.IsNullOrEmpty(txtP_SG.Text) && !string.IsNullOrEmpty(txtP_Comment.Text)
                        && !string.IsNullOrEmpty(txtTestEquipType.Text) && !string.IsNullOrEmpty(txtTestSerial.Text) && !string.IsNullOrEmpty(txtTestCalibrationDueDate.Text)
                        && !string.IsNullOrEmpty(txtTestByWhom.Text) && !string.IsNullOrEmpty(txtTestDate.Text)

                        && !string.IsNullOrEmpty(txtTT_From.Text) && !string.IsNullOrEmpty(txtTT_To.Text) && !string.IsNullOrEmpty(txtTT_PrintName.Text) && !string.IsNullOrEmpty(txtTT_Date.Text)
                        && !string.IsNullOrEmpty(txtTS_From.Text) && !string.IsNullOrEmpty(txtTS_To.Text) && !string.IsNullOrEmpty(txtTS_PrintName.Text) && !string.IsNullOrEmpty(txtTS_Date.Text)
                        && !string.IsNullOrEmpty(txtTC_From.Text) && !string.IsNullOrEmpty(txtTC_To.Text) && !string.IsNullOrEmpty(txtTC_PrintName.Text) && !string.IsNullOrEmpty(txtTC_Date.Text)
                        )
                {
                    checkdata = true;
                }
                else
                    checkdata = false;
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
