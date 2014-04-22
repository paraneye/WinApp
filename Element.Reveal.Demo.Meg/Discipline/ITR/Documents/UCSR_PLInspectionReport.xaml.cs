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
using System.Threading.Tasks;
using Element.Reveal.Meg.RevealProjectSvc;
using Windows.UI.Core;
namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCSR_PLInspectionReport : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        private new List<WinAppLibrary.UI.ObjectNFCSign> signed;
        public UCSR_PLInspectionReport()
        {
            this.InitializeComponent();
            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();

            signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                new WinAppLibrary.UI.ObjectNFCSign{
                isSigned = "Unsigned",
                MemberGrade = "CR",
                PersonnelName = null,
                SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                SignedTime = null//DateTime.Now.ToString()
            }};

            lvNFCSignListA.DataContext = signed;
            lvNFCSignListB.DataContext = signed;
            lvNFCSignListC.DataContext = signed;
        }

        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txt500VDC, txt1000VDC, txt2500VDC, txtA_Comments},
                    new List<FrameworkElement> { txtAT_SerialNo, txtAT_CalibrationDueDate, txtAT_ByWhom, txtAT_Date},
                    new List<FrameworkElement> { txtAG_SerialNo, txtAG_CalibrationDueDate, txtAG_ByWhom, txtAG_Date},
                     new List<FrameworkElement> {lvNFCSignListA}  
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { rdob1_1, rdob1_2, cspVI1, txtVI1 },
                    new List<FrameworkElement> { rdob2_1, rdob2_2, cspVI2, txtVI2 },
                    new List<FrameworkElement> { rdob3_1, rdob3_2, rdob3_3, cspVI3, txtVI3 },
                    new List<FrameworkElement> { rdob4_1, rdob4_2, rdob4_3, cspVI4, txtVI4 },
                    new List<FrameworkElement> { txtB_500VDC, txtB_1000VDC, txtB_2500VDC, txtB_Comments},
                    new List<FrameworkElement> { txtBT_SerialNo, txtBT_CalibrationDueDate, txtBT_ByWhom, txtBT_Date},
                    new List<FrameworkElement> { txtBG_SerialNo, txtBG_CalibrationDueDate, txtBG_ByWhom, txtBG_Date},
                     new List<FrameworkElement> {lvNFCSignListB}  
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { rdoc1_1, rdoc1_2, cspVP1, txtVP1 },
                    new List<FrameworkElement> { rdoc2_1, rdoc2_2, cspVP2, txtVP2 },
                    new List<FrameworkElement> { rdoc3_1, rdoc3_2, cspVP3, txtVP3 },
                    new List<FrameworkElement> { rdoc4_1, rdoc4_2, rdoc4_3, cspVP4, txtVP4 },
                    new List<FrameworkElement> { rdoc5_1, rdoc5_2, rdoc5_3, cspVP5, txtVP5 },
                    new List<FrameworkElement> { txtC_500VDC, txtC_1000VDC, txtC_2500VDC, txtC_Comments},
                    new List<FrameworkElement> { txtCT_SerialNo, txtCT_CalibrationDueDate, txtCT_ByWhom, txtCT_Date},
                    new List<FrameworkElement> { txtCG_SerialNo, txtCG_CalibrationDueDate, txtCG_ByWhom, txtCG_Date},
                    new List<FrameworkElement> {lvNFCSignListC}  
                }
            };

            lvNFCSignListA.SelectionChanged += lvNFCSignListA_SelectionChanged;
            lvNFCSignListB.SelectionChanged += lvNFCSignListB_SelectionChanged;
            lvNFCSignListC.SelectionChanged += lvNFCSignListC_SelectionChanged;

            //Header Binding
            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();

            lblProjectName.Text =  (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            lblProjectNumber.Text =  (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            lblCWPNo.Text =  (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            lblContractorJobNo.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            //lblEHTZoneDRWG.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue1)) ? HeaderDto.StringValue1 : "";
            //lblEHTZoneDRWGrev.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue2)) ? HeaderDto.StringValue2 : "";
            //lblLocationDRWG.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue3)) ? HeaderDto.StringValue3 : "";
            //lblLocationDRWGrev.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue4)) ? HeaderDto.StringValue4 : "";
            lblIWPNo.Text =  (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            lblSystemNo.Text =  (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            lblSystemName.Text =  (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";
            cbEHTTagNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue5)) ? HeaderDto.StringValue5 : "";
            lblPanelNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue6)) ? HeaderDto.StringValue6 : "";
            lblCCT.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue7)) ? HeaderDto.StringValue7 : "";
            lblControllerNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue8)) ? HeaderDto.StringValue8 : "";
            lblHTCNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue9)) ? HeaderDto.StringValue9 : "";

            //Drawing
            List<DrawingDTO> drawingDto = _dto.QaqcrefDrawing;
            lvDrawing.ItemsSource = drawingDto;
        }
        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }

        public async Task<bool> Validate2()
        {
            bool checkdata = true;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (txt500VDC.Text.Trim() == "") checkdata = false;
                if (txt1000VDC.Text.Trim() == "") checkdata = false;
                if (txt2500VDC.Text.Trim() == "") checkdata = false;
                if (txtA_Comments.Text.Trim() == "") checkdata = false;
                if (txtAT_SerialNo.Text.Trim() == "") checkdata = false;
                if (txtAT_ByWhom.Text.Trim() == "") checkdata = false;
                if (txtAG_SerialNo.Text.Trim() == "") checkdata = false;
                if (txtAG_ByWhom.Text.Trim() == "") checkdata = false;
                if (txtB_500VDC.Text.Trim() == "") checkdata = false;
                if (txtB_1000VDC.Text.Trim() == "") checkdata = false;
                if (txtB_2500VDC.Text.Trim() == "") checkdata = false;
                if (txtB_Comments.Text.Trim() == "") checkdata = false;
                if (txtBT_SerialNo.Text.Trim() == "") checkdata = false;
                if (txtBT_ByWhom.Text.Trim() == "") checkdata = false;
                if (txtBG_SerialNo.Text.Trim() == "") checkdata = false;
                if (txtBG_ByWhom.Text.Trim() == "") checkdata = false;
                if (txtC_500VDC.Text.Trim() == "") checkdata = false;
                if (txtC_1000VDC.Text.Trim() == "") checkdata = false;
                if (txtC_2500VDC.Text.Trim() == "") checkdata = false;
                if (txtC_Comments.Text.Trim() == "") checkdata = false;
                if (txtCT_SerialNo.Text.Trim() == "") checkdata = false;
                if (txtCT_ByWhom.Text.Trim() == "") checkdata = false;
                if (txtCG_SerialNo.Text.Trim() == "") checkdata = false;
                if (txtCG_ByWhom.Text.Trim() == "") checkdata = false;

                if (!(bool)rdob1_1.IsChecked && !(bool)rdob1_2.IsChecked) checkdata = false;
                if (!(bool)rdob2_1.IsChecked && !(bool)rdob2_2.IsChecked) checkdata = false;
                if (!(bool)rdob3_1.IsChecked && !(bool)rdob3_2.IsChecked && !(bool)rdob3_3.IsChecked) checkdata = false;
                if (!(bool)rdob4_1.IsChecked && !(bool)rdob4_2.IsChecked && !(bool)rdob4_3.IsChecked) checkdata = false;
                if (!(bool)rdob3_1.IsChecked && txtVI3.Text.Trim() == "") checkdata = false;
                if (!(bool)rdob4_1.IsChecked && txtVI4.Text.Trim() == "") checkdata = false;

                if (!(bool)rdoc1_1.IsChecked && !(bool)rdoc1_2.IsChecked) checkdata = false;
                if (!(bool)rdoc2_1.IsChecked && !(bool)rdoc2_2.IsChecked) checkdata = false;
                if (!(bool)rdoc3_1.IsChecked && !(bool)rdoc3_2.IsChecked) checkdata = false;
                if (!(bool)rdoc4_1.IsChecked && !(bool)rdoc4_2.IsChecked && !(bool)rdoc4_3.IsChecked) checkdata = false;
                if (!(bool)rdoc5_1.IsChecked && !(bool)rdoc5_2.IsChecked && !(bool)rdoc5_3.IsChecked) checkdata = false;
                if (!(bool)rdoc4_1.IsChecked && txtVP4.Text.Trim() == "") checkdata = false;
                if (!(bool)rdoc5_1.IsChecked && txtVP5.Text.Trim() == "") checkdata = false;
            });
            return checkdata;
        }

        public bool Validate()
        {
            if (txt500VDC.Text.Trim() == "") return false;
            if (txt1000VDC.Text.Trim() == "") return false;
            if (txt2500VDC.Text.Trim() == "") return false;
            if (txtA_Comments.Text.Trim() == "") return false;
            if (txtAT_SerialNo.Text.Trim() == "") return false;
            if (txtAT_ByWhom.Text.Trim() == "") return false;
            if (txtAG_SerialNo.Text.Trim() == "") return false;
            if (txtAG_ByWhom.Text.Trim() == "") return false;
            if (txtB_500VDC.Text.Trim() == "") return false;
            if (txtB_1000VDC.Text.Trim() == "") return false;
            if (txtB_2500VDC.Text.Trim() == "") return false;
            if (txtB_Comments.Text.Trim() == "") return false;
            if (txtBT_SerialNo.Text.Trim() == "") return false;
            if (txtBT_ByWhom.Text.Trim() == "") return false;
            if (txtBG_SerialNo.Text.Trim() == "") return false;
            if (txtBG_ByWhom.Text.Trim() == "") return false;
            if (txtC_500VDC.Text.Trim() == "") return false;
            if (txtC_1000VDC.Text.Trim() == "") return false;
            if (txtC_2500VDC.Text.Trim() == "") return false;
            if (txtC_Comments.Text.Trim() == "") return false;
            if (txtCT_SerialNo.Text.Trim() == "") return false;
            if (txtCT_ByWhom.Text.Trim() == "") return false;
            if (txtCG_SerialNo.Text.Trim() == "") return false;
            if (txtCG_ByWhom.Text.Trim() == "") return false;

            if (!(bool)rdob1_1.IsChecked && !(bool)rdob1_2.IsChecked) return false;
            if (!(bool)rdob2_1.IsChecked && !(bool)rdob2_2.IsChecked) return false;
            if (!(bool)rdob3_1.IsChecked && !(bool)rdob3_2.IsChecked && !(bool)rdob3_3.IsChecked) return false;
            if (!(bool)rdob4_1.IsChecked && !(bool)rdob4_2.IsChecked && !(bool)rdob4_3.IsChecked) return false;
            if (!(bool)rdob3_1.IsChecked && txtVI3.Text.Trim() == "") return false;
            if (!(bool)rdob4_1.IsChecked && txtVI4.Text.Trim() == "") return false;

            if (!(bool)rdoc1_1.IsChecked && !(bool)rdoc1_2.IsChecked) return false;
            if (!(bool)rdoc2_1.IsChecked && !(bool)rdoc2_2.IsChecked) return false;
            if (!(bool)rdoc3_1.IsChecked && !(bool)rdoc3_2.IsChecked) return false;
            if (!(bool)rdoc4_1.IsChecked && !(bool)rdoc4_2.IsChecked && !(bool)rdoc4_3.IsChecked) return false;
            if (!(bool)rdoc5_1.IsChecked && !(bool)rdoc5_2.IsChecked && !(bool)rdoc5_3.IsChecked) return false;
            if (!(bool)rdoc4_1.IsChecked && txtVP4.Text.Trim() == "") return false;
            if (!(bool)rdoc5_1.IsChecked && txtVP5.Text.Trim() == "") return false;
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
        }

        public bool isSigned
        {
            get
            {
                if (((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListA.Items[0]).PersonnelName == null && ((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListB.Items[0]).PersonnelName == null && ((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignListC.Items[0]).PersonnelName == null)
                    return false;
                return true;
            }
        }

        public bool isExistNFC
        {
            get { return true; }
        }

        public async void SetNFCData(string _personmane, string _grade)
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                    new WinAppLibrary.UI.ObjectNFCSign{
                    isSigned = "Signed",
                    PersonnelName = _personmane,
                    MemberGrade = "CR",
                    SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                    SignedTime = DateTime.Now.ToString()
                    }};
                    //lvNFCSignListA.DataContext = signed;

                    if (lvNFCSignListA.SelectedItems.Count > 0)
                        lvNFCSignListA.DataContext = signed;
                    else if (lvNFCSignListB.SelectedItems.Count > 0)
                        lvNFCSignListB.DataContext = signed;
                    else if (lvNFCSignListC.SelectedItems.Count > 0)
                        lvNFCSignListC.DataContext = signed;
                });
            }
            catch (Exception e)
            {
            }
        }

        void lvNFCSignListA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvNFCSignListA.SelectedItems.Count > 0)
            {
                lvNFCSignListB.SelectedItem = null;
                lvNFCSignListC.SelectedItem = null;
                if (SelectedSign != null)
                    SelectedSign(this, null);
            }
        }

        void lvNFCSignListB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvNFCSignListA.SelectedItem = null;
            lvNFCSignListC.SelectedItem = null;
            if (lvNFCSignListB.SelectedItems.Count > 0)
            {
                if (SelectedSign != null)
                    SelectedSign(this, null);
            }
        }

        void lvNFCSignListC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvNFCSignListA.SelectedItem = null;
            lvNFCSignListB.SelectedItem = null;
            if (lvNFCSignListC.SelectedItems.Count > 0)
            {
                if (SelectedSign != null)
                    SelectedSign(this, null);
            }
        }
        public void ClearSelect()
        {
            lvNFCSignListA.SelectedItem = null;
            lvNFCSignListB.SelectedItem = null;
            lvNFCSignListC.SelectedItem = null;
        }
        

        public event EventHandler SelectedSign;

        public bool isSelectedSign { get; set; }

        public async void checkSelectSign()
        {
            bool bl = false;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bl = lvNFCSignListA.SelectedItems.Count > 0 || lvNFCSignListB.SelectedItems.Count > 0 || lvNFCSignListC.SelectedItems.Count > 0 ? true : false;
                isSelectedSign = bl;
            });

        }

        private void lvNFCSignListA_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
            {

                signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                new WinAppLibrary.UI.ObjectNFCSign{
                isSigned = "UnSigned",
                PersonnelName = "",
                MemberGrade = "CR",
                SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                SignedTime = ""
                }};
                lvNFCSignListA.DataContext = signed;
            }
        }

        private void lvNFCSignListB_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
            {

                signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                new WinAppLibrary.UI.ObjectNFCSign{
                isSigned = "UnSigned",
                PersonnelName = "",
                MemberGrade = "CR",
                SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                SignedTime = ""
                }};
                lvNFCSignListB.DataContext = signed;
            }
        }

        private void lvNFCSignListC_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
            {

                signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                new WinAppLibrary.UI.ObjectNFCSign{
                isSigned = "UnSigned",
                PersonnelName = "",
                MemberGrade = "CR",
                SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                SignedTime = ""
                }};
                lvNFCSignListC.DataContext = signed;
            }
        } 
    }
}
