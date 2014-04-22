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
    public sealed partial class UCLightingandDeviceInstallation : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public UCLightingandDeviceInstallation()
        {
            this.InitializeComponent();

            chkVIfLaC1.Checked += chkVIfLaC1_Checked;
            chkVIfLaC2.Checked += chkVIfLaC2_Checked;
            chkVIfLaC3.Checked += chkVIfLaC3_Checked;
            chkVIfLaC4.Checked += chkVIfLaC4_Checked;

            chkVIfRaC1.Checked += chkVIfRaC1_Checked;
            chkVIfRaC2.Checked += chkVIfRaC2_Checked;
            chkVIfRaC3.Checked += chkVIfRaC3_Checked;
            chkVIfRaC4.Checked += chkVIfRaC4_Checked;

            chkVIfHaC1.Checked += chkVIfHaC1_Checked;
            chkVIfHaC2.Checked += chkVIfHaC2_Checked;
            chkVIfHaC3.Checked += chkVIfHaC3_Checked;
            chkVIfHaC4.Checked += chkVIfHaC4_Checked;

            chkVIfDaC1.Checked += chkVIfDaC1_Checked;
            chkVIfDaC2.Checked += chkVIfDaC2_Checked;
            chkVIfDaC3.Checked += chkVIfDaC3_Checked;
            chkVIfDaC4.Checked += chkVIfDaC4_Checked;

            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

        void chkVIfDaC1_Checked(object sender, RoutedEventArgs e) { rdoVIfDaC1Yes.IsEnabled = rdoVIfDaC1No.IsEnabled = !(bool)chkVIfDaC1.IsChecked; }
        void chkVIfDaC2_Checked(object sender, RoutedEventArgs e) { rdoVIfDaC1Yes.IsEnabled = rdoVIfDaC1No.IsEnabled = !(bool)chkVIfDaC1.IsChecked; }
        void chkVIfDaC3_Checked(object sender, RoutedEventArgs e) { rdoVIfDaC1Yes.IsEnabled = rdoVIfDaC1No.IsEnabled = !(bool)chkVIfDaC1.IsChecked; }
        void chkVIfDaC4_Checked(object sender, RoutedEventArgs e) { rdoVIfDaC1Yes.IsEnabled = rdoVIfDaC1No.IsEnabled = !(bool)chkVIfDaC1.IsChecked; }

        void chkVIfHaC1_Checked(object sender, RoutedEventArgs e) { rdoVIfHaC1Yes.IsEnabled = rdoVIfHaC1No.IsEnabled = !(bool)chkVIfHaC1.IsChecked; }
        void chkVIfHaC2_Checked(object sender, RoutedEventArgs e) { rdoVIfHaC2Yes.IsEnabled = rdoVIfHaC2No.IsEnabled = !(bool)chkVIfHaC2.IsChecked; }
        void chkVIfHaC3_Checked(object sender, RoutedEventArgs e) { rdoVIfHaC3Yes.IsEnabled = rdoVIfHaC3No.IsEnabled = !(bool)chkVIfHaC3.IsChecked; }
        void chkVIfHaC4_Checked(object sender, RoutedEventArgs e) { rdoVIfHaC4Yes.IsEnabled = rdoVIfHaC4No.IsEnabled = !(bool)chkVIfHaC4.IsChecked; }

        void chkVIfRaC1_Checked(object sender, RoutedEventArgs e) { rdoVIfRaC1Yes.IsEnabled = rdoVIfRaC1No.IsEnabled = !(bool)chkVIfRaC1.IsChecked; }
        void chkVIfRaC2_Checked(object sender, RoutedEventArgs e) { rdoVIfRaC2Yes.IsEnabled = rdoVIfRaC2No.IsEnabled = !(bool)chkVIfRaC2.IsChecked; }
        void chkVIfRaC3_Checked(object sender, RoutedEventArgs e) { rdoVIfRaC3Yes.IsEnabled = rdoVIfRaC3No.IsEnabled = !(bool)chkVIfRaC3.IsChecked; }
        void chkVIfRaC4_Checked(object sender, RoutedEventArgs e) { rdoVIfRaC4Yes.IsEnabled = rdoVIfRaC4No.IsEnabled = !(bool)chkVIfRaC4.IsChecked; }

        void chkVIfLaC1_Checked(object sender, RoutedEventArgs e) { rdoVIfLaC1Yes.IsEnabled = rdoVIfLaC1No.IsEnabled = !(bool)chkVIfLaC1.IsChecked; }
        void chkVIfLaC2_Checked(object sender, RoutedEventArgs e) { rdoVIfLaC2Yes.IsEnabled = rdoVIfLaC2No.IsEnabled = !(bool)chkVIfLaC2.IsChecked; }
        void chkVIfLaC3_Checked(object sender, RoutedEventArgs e) { rdoVIfLaC3Yes.IsEnabled = rdoVIfLaC3No.IsEnabled = !(bool)chkVIfLaC3.IsChecked; }
        void chkVIfLaC4_Checked(object sender, RoutedEventArgs e) { rdoVIfLaC4Yes.IsEnabled = rdoVIfLaC4No.IsEnabled = !(bool)chkVIfLaC4.IsChecked; }

        public void DoAfter(QaqcformDTO _dto)
        {
             controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtLtgDev, txtTtlLts }                    
                },
                new List<List<FrameworkElement>> {
                    new List<FrameworkElement> { chkVIfLaC1, rdoVIfLaC1Yes, rdoVIfLaC1No},
                    new List<FrameworkElement> { chkVIfLaC2, rdoVIfLaC2Yes, rdoVIfLaC2No},
                    new List<FrameworkElement> { chkVIfLaC3, rdoVIfLaC3Yes, rdoVIfLaC3No},
                    new List<FrameworkElement> { chkVIfLaC4, rdoVIfLaC4Yes, rdoVIfLaC4No},
                    new List<FrameworkElement> { txtVIfLaC }
                },
                new List<List<FrameworkElement>> {
                    new List<FrameworkElement> { chkVIfRaC1, rdoVIfRaC1Yes, rdoVIfRaC1No},
                    new List<FrameworkElement> { chkVIfRaC2, rdoVIfRaC2Yes, rdoVIfRaC2No},
                    new List<FrameworkElement> { chkVIfRaC3, rdoVIfRaC3Yes, rdoVIfRaC3No},
                    new List<FrameworkElement> { chkVIfRaC4, rdoVIfRaC4Yes, rdoVIfRaC4No},
                    new List<FrameworkElement> { txtVIfRaC }
                },
                new List<List<FrameworkElement>> {
                    new List<FrameworkElement> { chkVIfHaC1, rdoVIfHaC1Yes, rdoVIfHaC1No},
                    new List<FrameworkElement> { chkVIfHaC2, rdoVIfHaC2Yes, rdoVIfHaC2No},
                    new List<FrameworkElement> { chkVIfHaC3, rdoVIfHaC3Yes, rdoVIfHaC3No},
                    new List<FrameworkElement> { chkVIfHaC4, rdoVIfHaC4Yes, rdoVIfHaC4No},
                    new List<FrameworkElement> { txtVIfHaC },
                },
                new List<List<FrameworkElement>> {
                    new List<FrameworkElement> { chkVIfDaC1, rdoVIfDaC1Yes, rdoVIfDaC1No},
                    new List<FrameworkElement> { chkVIfDaC2, rdoVIfDaC2Yes, rdoVIfDaC2No},
                    new List<FrameworkElement> { chkVIfDaC3, rdoVIfDaC3Yes, rdoVIfDaC3No},
                    new List<FrameworkElement> { chkVIfDaC4, rdoVIfDaC4Yes, rdoVIfDaC4No},
                    new List<FrameworkElement> { txtVIfDaC }
                },
            };
            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();

            ProjectName.Text =  (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            ProjectNumber.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            CWPName.Text =  (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            JobNumber.Text =  (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";

            //DrawingName.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue1)) ? HeaderDto.StringValue1 : "";
            //Rev.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue2)) ? HeaderDto.StringValue2 : "";
            //txtDetailDrwg.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue3)) ? HeaderDto.StringValue3 : "";
            //txtDetailRev.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue4)) ? HeaderDto.StringValue4 : "";
            txtPanelSchedule.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue5)) ? HeaderDto.StringValue5 : "";
            txtPanelRev.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue6)) ? HeaderDto.StringValue6 : "";
            //txtLocationDrwgForPanel.Text = _dto.QaqcfromDetails[0].StringValue7;
            //txtLocationForPanelRev.Text = _dto.QaqcfromDetails[0].StringValue8;

            FIWPName.Text =  (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            SystemNumber.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            SystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";
                
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
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (!(bool)chkVIfLaC1.IsChecked && !(bool)rdoVIfLaC1Yes.IsChecked && !(bool)rdoVIfLaC1No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfLaC2.IsChecked && !(bool)rdoVIfLaC2Yes.IsChecked && !(bool)rdoVIfLaC2No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfLaC3.IsChecked && !(bool)rdoVIfLaC3Yes.IsChecked && !(bool)rdoVIfLaC3No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfLaC4.IsChecked && !(bool)rdoVIfLaC4Yes.IsChecked && !(bool)rdoVIfLaC4No.IsChecked) checkdata = false;

                    if (!(bool)chkVIfHaC1.IsChecked && !(bool)rdoVIfHaC1Yes.IsChecked && !(bool)rdoVIfHaC1No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfHaC2.IsChecked && !(bool)rdoVIfHaC2Yes.IsChecked && !(bool)rdoVIfHaC2No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfHaC3.IsChecked && !(bool)rdoVIfHaC3Yes.IsChecked && !(bool)rdoVIfHaC3No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfHaC4.IsChecked && !(bool)rdoVIfHaC4Yes.IsChecked && !(bool)rdoVIfHaC4No.IsChecked) checkdata = false;

                    if (!(bool)chkVIfRaC1.IsChecked && !(bool)rdoVIfRaC1Yes.IsChecked && !(bool)rdoVIfRaC1No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfRaC2.IsChecked && !(bool)rdoVIfRaC2Yes.IsChecked && !(bool)rdoVIfRaC2No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfRaC3.IsChecked && !(bool)rdoVIfRaC3Yes.IsChecked && !(bool)rdoVIfRaC3No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfRaC4.IsChecked && !(bool)rdoVIfRaC4Yes.IsChecked && !(bool)rdoVIfRaC4No.IsChecked) checkdata = false;

                    if (!(bool)chkVIfLaC1.IsChecked && !(bool)rdoVIfLaC1Yes.IsChecked && !(bool)rdoVIfLaC1No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfLaC2.IsChecked && !(bool)rdoVIfLaC2Yes.IsChecked && !(bool)rdoVIfLaC2No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfLaC3.IsChecked && !(bool)rdoVIfLaC3Yes.IsChecked && !(bool)rdoVIfLaC3No.IsChecked) checkdata = false;
                    if (!(bool)chkVIfLaC4.IsChecked && !(bool)rdoVIfLaC4Yes.IsChecked && !(bool)rdoVIfLaC4No.IsChecked) checkdata = false;

                    if (txtLtgDev.Text == "") checkdata = false;
                    if (txtTtlLts.Text == "") checkdata = false;
                    if (txtVIfLaC.Text == "") checkdata = false;
                    if (txtVIfRaC.Text == "") checkdata = false;
                    if (txtVIfHaC.Text == "") checkdata = false;
                    if (txtVIfDaC.Text == "") checkdata = false;
                });
            }
            catch (Exception ex)
            { 
            }
            return checkdata;
        }

        public bool Validate()
        {
            if (!(bool)chkVIfLaC1.IsChecked && !(bool)rdoVIfLaC1Yes.IsChecked && !(bool)rdoVIfLaC1No.IsChecked) return false;
            if (!(bool)chkVIfLaC2.IsChecked && !(bool)rdoVIfLaC2Yes.IsChecked && !(bool)rdoVIfLaC2No.IsChecked) return false;
            if (!(bool)chkVIfLaC3.IsChecked && !(bool)rdoVIfLaC3Yes.IsChecked && !(bool)rdoVIfLaC3No.IsChecked) return false;
            if (!(bool)chkVIfLaC4.IsChecked && !(bool)rdoVIfLaC4Yes.IsChecked && !(bool)rdoVIfLaC4No.IsChecked) return false;

            if (!(bool)chkVIfHaC1.IsChecked && !(bool)rdoVIfHaC1Yes.IsChecked && !(bool)rdoVIfHaC1No.IsChecked) return false;
            if (!(bool)chkVIfHaC2.IsChecked && !(bool)rdoVIfHaC2Yes.IsChecked && !(bool)rdoVIfHaC2No.IsChecked) return false;
            if (!(bool)chkVIfHaC3.IsChecked && !(bool)rdoVIfHaC3Yes.IsChecked && !(bool)rdoVIfHaC3No.IsChecked) return false;
            if (!(bool)chkVIfHaC4.IsChecked && !(bool)rdoVIfHaC4Yes.IsChecked && !(bool)rdoVIfHaC4No.IsChecked) return false;

            if (!(bool)chkVIfRaC1.IsChecked && !(bool)rdoVIfRaC1Yes.IsChecked && !(bool)rdoVIfRaC1No.IsChecked) return false;
            if (!(bool)chkVIfRaC2.IsChecked && !(bool)rdoVIfRaC2Yes.IsChecked && !(bool)rdoVIfRaC2No.IsChecked) return false;
            if (!(bool)chkVIfRaC3.IsChecked && !(bool)rdoVIfRaC3Yes.IsChecked && !(bool)rdoVIfRaC3No.IsChecked) return false;
            if (!(bool)chkVIfRaC4.IsChecked && !(bool)rdoVIfRaC4Yes.IsChecked && !(bool)rdoVIfRaC4No.IsChecked) return false;

            if (!(bool)chkVIfLaC1.IsChecked && !(bool)rdoVIfLaC1Yes.IsChecked && !(bool)rdoVIfLaC1No.IsChecked) return false;
            if (!(bool)chkVIfLaC2.IsChecked && !(bool)rdoVIfLaC2Yes.IsChecked && !(bool)rdoVIfLaC2No.IsChecked) return false;
            if (!(bool)chkVIfLaC3.IsChecked && !(bool)rdoVIfLaC3Yes.IsChecked && !(bool)rdoVIfLaC3No.IsChecked) return false;
            if (!(bool)chkVIfLaC4.IsChecked && !(bool)rdoVIfLaC4Yes.IsChecked && !(bool)rdoVIfLaC4No.IsChecked) return false;

            if (txtLtgDev.Text == "") return false;
            if (txtTtlLts.Text == "") return false;
            if (txtVIfLaC.Text == "") return false;
            if (txtVIfRaC.Text == "") return false;
            if (txtVIfHaC.Text == "") return false;
            if (txtVIfDaC.Text == "") return false;
            return true;
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
            FormSerialize.GenDTO(QAQCGroup.GROUP05, controls[4], QAQCDTOList);
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
