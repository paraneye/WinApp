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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using Element.Reveal.Meg.Lib;
using C1.Xaml.DateTimeEditors;
using System.Threading.Tasks;
using Element.Reveal.Meg.RevealProjectSvc;
namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCLightingAndDeviceCircuit : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public UCLightingAndDeviceCircuit()
        {
            this.InitializeComponent();

            chkVIoC1.Checked += chkVIoC1_Checked;
            chkVIoC2.Checked += chkVIoC2_Checked;
            chkVIoC3.Checked += chkVIoC3_Checked;
            chkVIoC4.Checked += chkVIoC4_Checked;
            chkVIoC5.Checked += chkVIoC5_Checked;
            chkVIoC6.Checked += chkVIoC6_Checked;
            chkVIoC7.Checked += chkVIoC7_Checked;
            chkVIoC8.Checked += chkVIoC8_Checked;
            chkVIoC9.Checked += chkVIoC9_Checked;
            chkVIoC10.Checked += chkVIoC10_Checked;

            chkFTFCaC1.Checked += chkFTFCaC1_Checked;
            chkFTFCaC2.Checked += chkFTFCaC2_Checked;
            chkFTFCaC3.Checked += chkFTFCaC3_Checked;
            chkFTFCaC4.Checked += chkFTFCaC4_Checked;
            chkFTFCaC5.Checked += chkFTFCaC5_Checked;

            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

        void chkFTFCaC1_Checked(object sender, RoutedEventArgs e) { rdoFTFCaC1Yes.IsChecked = rdoFTFCaC1No.IsChecked = !(bool)chkFTFCaC1.IsChecked; }
        void chkFTFCaC2_Checked(object sender, RoutedEventArgs e) { rdoFTFCaC2Yes.IsChecked = rdoFTFCaC2No.IsChecked = !(bool)chkFTFCaC2.IsChecked; }
        void chkFTFCaC3_Checked(object sender, RoutedEventArgs e) { rdoFTFCaC3Yes.IsChecked = rdoFTFCaC3No.IsChecked = !(bool)chkFTFCaC3.IsChecked; }
        void chkFTFCaC4_Checked(object sender, RoutedEventArgs e) { rdoFTFCaC4Yes.IsChecked = rdoFTFCaC4No.IsChecked = !(bool)chkFTFCaC4.IsChecked; }
        void chkFTFCaC5_Checked(object sender, RoutedEventArgs e) { rdoFTFCaC5Yes.IsChecked = rdoFTFCaC5No.IsChecked = !(bool)chkFTFCaC5.IsChecked; }

        void chkVIoC1_Checked(object sender, RoutedEventArgs e) { rdoVIoC1Yes.IsChecked = rdoVIoC1No.IsChecked = !(bool)chkVIoC1.IsChecked; }
        void chkVIoC2_Checked(object sender, RoutedEventArgs e) { rdoVIoC2Yes.IsChecked = rdoVIoC2No.IsChecked = !(bool)chkVIoC2.IsChecked; }
        void chkVIoC3_Checked(object sender, RoutedEventArgs e) { rdoVIoC3Yes.IsChecked = rdoVIoC3No.IsChecked = !(bool)chkVIoC3.IsChecked; }
        void chkVIoC4_Checked(object sender, RoutedEventArgs e) { rdoVIoC4Yes.IsChecked = rdoVIoC4No.IsChecked = !(bool)chkVIoC4.IsChecked; }
        void chkVIoC5_Checked(object sender, RoutedEventArgs e) { rdoVIoC5Yes.IsChecked = rdoVIoC5No.IsChecked = !(bool)chkVIoC5.IsChecked; }
        void chkVIoC6_Checked(object sender, RoutedEventArgs e) { rdoVIoC6Yes.IsChecked = rdoVIoC6No.IsChecked = !(bool)chkVIoC6.IsChecked; }
        void chkVIoC7_Checked(object sender, RoutedEventArgs e) { rdoVIoC7Yes.IsChecked = rdoVIoC7No.IsChecked = !(bool)chkVIoC7.IsChecked; }
        void chkVIoC8_Checked(object sender, RoutedEventArgs e) { rdoVIoC8Yes.IsChecked = rdoVIoC8No.IsChecked = !(bool)chkVIoC8.IsChecked; }
        void chkVIoC9_Checked(object sender, RoutedEventArgs e) { rdoVIoC9Yes.IsChecked = rdoVIoC9No.IsChecked = !(bool)chkVIoC9.IsChecked; }
        void chkVIoC10_Checked(object sender, RoutedEventArgs e) { rdoVIoC10Yes.IsChecked = rdoVIoC10No.IsChecked = !(bool)chkVIoC10.IsChecked; }

        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }

        public async Task<bool> Validate2()
        {
            bool checkdata = true;

            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    if (txtLtgDev.Text.Trim() == "") checkdata = false;
                    if (txtTtlLts.Text.Trim() == "") checkdata = false;
                    if (txtVIoC.Text.Trim() == "") checkdata = false;
                    if (txtFTFCaC.Text.Trim() == "") checkdata = false;
                    if (txtTestInfo.Text.Trim() == "") checkdata = false;
                    if (txtEquipType.Text.Trim() == "") checkdata = false;
                    if (txtVoltage.Text.Trim() == "") checkdata = false;
                    if (txtAMPS.Text.Trim() == "") checkdata = false;
                    if (txtSerialNo.Text.Trim() == "") checkdata = false;
                    if (txtCalibrationDueDate.Text.Trim() == "") checkdata = false;

                    if (!(bool)chkVIoC1.IsChecked && !(bool)rdoVIoC1Yes.IsChecked && !(bool)rdoVIoC1No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC2.IsChecked && !(bool)rdoVIoC2Yes.IsChecked && !(bool)rdoVIoC2No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC3.IsChecked && !(bool)rdoVIoC3Yes.IsChecked && !(bool)rdoVIoC3No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC4.IsChecked && !(bool)rdoVIoC4Yes.IsChecked && !(bool)rdoVIoC4No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC5.IsChecked && !(bool)rdoVIoC5Yes.IsChecked && !(bool)rdoVIoC5No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC6.IsChecked && !(bool)rdoVIoC6Yes.IsChecked && !(bool)rdoVIoC6No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC7.IsChecked && !(bool)rdoVIoC7Yes.IsChecked && !(bool)rdoVIoC7No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC8.IsChecked && !(bool)rdoVIoC8Yes.IsChecked && !(bool)rdoVIoC8No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC9.IsChecked && !(bool)rdoVIoC9Yes.IsChecked && !(bool)rdoVIoC9No.IsChecked) checkdata = false;
                    if (!(bool)chkVIoC10.IsChecked && !(bool)rdoVIoC10Yes.IsChecked && !(bool)rdoVIoC10No.IsChecked) checkdata = false;

                    if (!(bool)chkFTFCaC1.IsChecked && !(bool)rdoFTFCaC1Yes.IsChecked && !(bool)rdoFTFCaC1No.IsChecked) checkdata = false;
                    if (!(bool)chkFTFCaC2.IsChecked && !(bool)rdoFTFCaC2Yes.IsChecked && !(bool)rdoFTFCaC2No.IsChecked) checkdata = false;
                    if (!(bool)chkFTFCaC3.IsChecked && !(bool)rdoFTFCaC3Yes.IsChecked && !(bool)rdoFTFCaC3No.IsChecked) checkdata = false;
                    if (!(bool)chkFTFCaC4.IsChecked && !(bool)rdoFTFCaC4Yes.IsChecked && !(bool)rdoFTFCaC4No.IsChecked) checkdata = false;
                    if (!(bool)chkFTFCaC5.IsChecked && !(bool)rdoFTFCaC5Yes.IsChecked && !(bool)rdoFTFCaC5No.IsChecked) checkdata = false;
                });
            }
            catch (Exception ex)
            { 

            }


            return checkdata;
        }
        
        public bool Validate()
        {
            if (txtLtgDev.Text.Trim() == "") return false;
            if (txtTtlLts.Text.Trim() == "") return false;
            if (txtVIoC.Text.Trim() == "") return false;
            if (txtFTFCaC.Text.Trim() == "") return false;
            if (txtTestInfo.Text.Trim() == "") return false;
            if (txtEquipType.Text.Trim() == "") return false;
            if (txtVoltage.Text.Trim() == "") return false;
            if (txtAMPS.Text.Trim() == "") return false;
            if (txtSerialNo.Text.Trim() == "") return false;
            if (txtCalibrationDueDate.Text.Trim() == "") return false;

            if (!(bool)chkVIoC1.IsChecked && !(bool)rdoVIoC1Yes.IsChecked && !(bool)rdoVIoC1No.IsChecked) return false;
            if (!(bool)chkVIoC2.IsChecked && !(bool)rdoVIoC2Yes.IsChecked && !(bool)rdoVIoC2No.IsChecked) return false;
            if (!(bool)chkVIoC3.IsChecked && !(bool)rdoVIoC3Yes.IsChecked && !(bool)rdoVIoC3No.IsChecked) return false;
            if (!(bool)chkVIoC4.IsChecked && !(bool)rdoVIoC4Yes.IsChecked && !(bool)rdoVIoC4No.IsChecked) return false;
            if (!(bool)chkVIoC5.IsChecked && !(bool)rdoVIoC5Yes.IsChecked && !(bool)rdoVIoC5No.IsChecked) return false;
            if (!(bool)chkVIoC6.IsChecked && !(bool)rdoVIoC6Yes.IsChecked && !(bool)rdoVIoC6No.IsChecked) return false;
            if (!(bool)chkVIoC7.IsChecked && !(bool)rdoVIoC7Yes.IsChecked && !(bool)rdoVIoC7No.IsChecked) return false;
            if (!(bool)chkVIoC8.IsChecked && !(bool)rdoVIoC8Yes.IsChecked && !(bool)rdoVIoC8No.IsChecked) return false;
            if (!(bool)chkVIoC9.IsChecked && !(bool)rdoVIoC9Yes.IsChecked && !(bool)rdoVIoC9No.IsChecked) return false;
            if (!(bool)chkVIoC10.IsChecked && !(bool)rdoVIoC10Yes.IsChecked && !(bool)rdoVIoC10No.IsChecked) return false;

            if (!(bool)chkFTFCaC1.IsChecked && !(bool)rdoFTFCaC1Yes.IsChecked && !(bool)rdoFTFCaC1No.IsChecked) return false;
            if (!(bool)chkFTFCaC2.IsChecked && !(bool)rdoFTFCaC2Yes.IsChecked && !(bool)rdoFTFCaC2No.IsChecked) return false;
            if (!(bool)chkFTFCaC3.IsChecked && !(bool)rdoFTFCaC3Yes.IsChecked && !(bool)rdoFTFCaC3No.IsChecked) return false;
            if (!(bool)chkFTFCaC4.IsChecked && !(bool)rdoFTFCaC4Yes.IsChecked && !(bool)rdoFTFCaC4No.IsChecked) return false;
            if (!(bool)chkFTFCaC5.IsChecked && !(bool)rdoFTFCaC5Yes.IsChecked && !(bool)rdoFTFCaC5No.IsChecked) return false;
            return true;
        }

        /*
         * 
         * 요롷게만 하면 됨 - 시작!!!
         * 
         * */
        public void Save()
        {
            QAQCDTOList.Clear();
            FormSerialize.GenDTO(QAQCGroup.GROUP01, controls[0], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP02, controls[1], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP03, controls[2], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP04, controls[3], QAQCDTOList);
        }

        public void Load()
        {
            FormSerialize.Load(controls, QAQCDTOList);
        }

        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtLtgDev, txtTtlLts } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { chkVIoC1, rdoVIoC1Yes, rdoVIoC1No },
                    new List<FrameworkElement> { chkVIoC2, rdoVIoC2Yes, rdoVIoC2No },
                    new List<FrameworkElement> { chkVIoC3, rdoVIoC3Yes, rdoVIoC3No },
                    new List<FrameworkElement> { chkVIoC4, rdoVIoC4Yes, rdoVIoC4No },
                    new List<FrameworkElement> { chkVIoC5, rdoVIoC5Yes, rdoVIoC5No },
                    new List<FrameworkElement> { chkVIoC6, rdoVIoC6Yes, rdoVIoC6No },
                    new List<FrameworkElement> { chkVIoC7, rdoVIoC7Yes, rdoVIoC7No },
                    new List<FrameworkElement> { chkVIoC8, rdoVIoC8Yes, rdoVIoC8No },
                    new List<FrameworkElement> { chkVIoC9, rdoVIoC9Yes, rdoVIoC9No },
                    new List<FrameworkElement> { chkVIoC10, rdoVIoC10Yes, rdoVIoC10No } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { chkFTFCaC1, rdoFTFCaC1Yes, rdoFTFCaC1No },
                    new List<FrameworkElement> { chkFTFCaC2, rdoFTFCaC2Yes, rdoFTFCaC2No },
                    new List<FrameworkElement> { chkFTFCaC3, rdoFTFCaC3Yes, rdoFTFCaC3No },
                    new List<FrameworkElement> { chkFTFCaC4, rdoFTFCaC4Yes, rdoFTFCaC4No },
                    new List<FrameworkElement> { chkFTFCaC5, rdoFTFCaC5Yes, rdoFTFCaC5No }
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtVoltage, txtEquipType, txtAMPS, txtSerialNo, txtCalibrationDueDate,txtByWhorm,txtDate }
                }
            };

            txtProjectName.Text =  (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            txtProjectNumber.Text =  (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            txtCwpEwpNo.Text =  (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            txtContractorJobNo.Text =  (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";

            //Header Binding
            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();

            //txtLocationDrwg.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue1)) ? HeaderDto.StringValue1 : "";
            //txtLocationRev.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue2)) ? HeaderDto.StringValue2 : "";
            //txtDetailDrwg.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue3)) ? HeaderDto.StringValue3 : "";
            //txtDetailRev.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue4)) ? HeaderDto.StringValue4 : "";
            txtPanelSchedule.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue5)) ? HeaderDto.StringValue5 : "";
            txtPanelRev.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue6)) ? HeaderDto.StringValue6 : "";
            //txtLocationDrwgForPanel.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue7)) ? HeaderDto.StringValue7 : "";
            //txtLocationForPanelRev.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue8)) ? HeaderDto.StringValue8 : "";

            txtIwpNo.Text =  (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            txtSystemNo.Text =  (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            txtSystemName.Text =  (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";

            cboCircuit.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue9)) ? HeaderDto.StringValue9 : "";
            txtPanelNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue10)) ? HeaderDto.StringValue10 : "";

            //Drawing            
            List<DrawingDTO> drawingDTO = _dto.QaqcrefDrawing;
            lvDrawing.ItemsSource = drawingDTO;

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
