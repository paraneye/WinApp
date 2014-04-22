using Element.Reveal.Meg.Lib;
using Element.Reveal.Meg.RevealProjectSvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class UCMIInspectionReport : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        private new List<WinAppLibrary.UI.ObjectNFCSign> signed;
        public UCMIInspectionReport()
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
                    new List<FrameworkElement> { txtA_CT, txtA_IRT, txtA_Comments },
                    new List<FrameworkElement> { txtAT_SerialNo, txtAT_CalibrationDueDate, txtAT_ByWhom, txtAT_Date },
                    new List<FrameworkElement> { txtAG_SerialNo, txtAG_CalibrationDueDate, txtAG_ByWhom, txtAG_Date},
                    new List<FrameworkElement> {lvNFCSignListA}  
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { rgb1Yes, rgb1No, cspVI1, txtVI1},
                    new List<FrameworkElement> { rgb2Yes, rgb2No, cspVI2,  txtVI2},
                    new List<FrameworkElement> { rgb3Na, rgb3Yes, rgb3No, cspVI3, txtVI3 },
                    new List<FrameworkElement> { rgb4Na, rgb4Yes, rgb4No, cspVI4, txtVI4 },
                    new List<FrameworkElement>{ txtB_CT, txtB_IRT, txtB_Comments},
                    new List<FrameworkElement>{ txtBT_SerialNo, txtBT_CalibrationDueDate, txtBT_ByWhom, txtBT_Date},
                    new List<FrameworkElement>{ txtBG_SerialNo, txtBG_CalibrationDueDate, txtBG_ByWhom, txtBG_Date},
                    new List<FrameworkElement> {lvNFCSignListB}  
                },
                new List<List<FrameworkElement>>{
                    new List<FrameworkElement>{ rgc1Yes, rgc1No, cspVP1, txtVP1},
                    new List<FrameworkElement>{ rgc2Yes, rgc2No, cspVP2, txtVP2},
                    new List<FrameworkElement>{ rgc3Yes, rgc3No, cspVP3, txtVP3},
                    new List<FrameworkElement>{ rgc4Na, rgc4Yes, rgc4No, cspVP4, txtVP4},
                    new List<FrameworkElement>{ rgc5Na, rgc5Yes, rgc5No, cspVP5, txtVP5},
                    new List<FrameworkElement>{ txtC_CT, txtC_IRT, txtC_Comments},
                    new List<FrameworkElement>{ txtCT_SerialNo, txtCT_CalibrationDueDate, txtCT_ByWhom, txtCT_Date},
                    new List<FrameworkElement>{ txtCG_SerialNo, txtCG_CalibrationDueDate, txtCG_ByWhom, txtCG_Date},
                    new List<FrameworkElement> {lvNFCSignListC}  
                },
            };

            lvNFCSignListA.SelectionChanged += lvNFCSignListA_SelectionChanged;
            lvNFCSignListB.SelectionChanged += lvNFCSignListB_SelectionChanged;
            lvNFCSignListC.SelectionChanged += lvNFCSignListC_SelectionChanged;

            //Header Binding
            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();

            lblProjectName.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            lblProjectNumber.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            lblCWPNo.Text = (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            lblContractorJobNo.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";

            //lblEHTZoneDRWG.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue1)) ? HeaderDto.StringValue1 : "";
            //lblEHTZoneDRWGrev.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue2)) ? HeaderDto.StringValue2 : "";
            //lblLocationDRWG.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue3)) ? HeaderDto.StringValue3 : "";
            //lblLocationDRWGrev.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue4)) ? HeaderDto.StringValue4 : "";

            lblIWPNo.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            lblSystemNo.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            lblSystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";

            cbEHTTagNo.Text = HeaderDto.StringValue5;
            lblPanelNo.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue6)) ? HeaderDto.StringValue6 : "";
            lblCCT.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue7)) ? HeaderDto.StringValue7 : "";
            lblControllerNo.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue8)) ? HeaderDto.StringValue8 : "";
            lblHTCNo.Text = (!string.IsNullOrEmpty(HeaderDto.StringValue9)) ? HeaderDto.StringValue9 : "";

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

            try
            {                
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (!(bool)rgb3Na.IsChecked && !(bool)rgb3Yes.IsChecked && !(bool)rgb3No.IsChecked && !(bool)string.IsNullOrEmpty(txtVI3.Text) && !(bool)string.IsNullOrEmpty(cspVI3.SelectedDate.ToString())) checkdata = false;
                    if (!(bool)rgb4Na.IsChecked && !(bool)rgb4Yes.IsChecked && !(bool)rgb4No.IsChecked && !(bool)string.IsNullOrEmpty(txtVI4.Text) && !(bool)string.IsNullOrEmpty(cspVI4.SelectedDate.ToString())) checkdata = false;
                    if (!(bool)rgc4Na.IsChecked && !(bool)rgc4Yes.IsChecked && !(bool)rgc4No.IsChecked && !(bool)string.IsNullOrEmpty(txtVP4.Text) && !(bool)string.IsNullOrEmpty(cspVP4.SelectedDate.ToString())) checkdata = false;
                    if (!(bool)rgc5Na.IsChecked && !(bool)rgc5Yes.IsChecked && !(bool)rgc5No.IsChecked && !(bool)string.IsNullOrEmpty(txtVP5.Text) && !(bool)string.IsNullOrEmpty(cspVP5.SelectedDate.ToString())) checkdata = false;
                    if (!(bool)rgb1Yes.IsChecked && !(bool)rgb1No.IsChecked) checkdata = false;
                    if (!(bool)rgb2Yes.IsChecked && !(bool)rgb2No.IsChecked) checkdata = false;
                    if (!(bool)rgc1Yes.IsChecked && !(bool)rgc1No.IsChecked) checkdata = false;
                    if (!(bool)rgc2Yes.IsChecked && !(bool)rgc2No.IsChecked) checkdata = false;
                    if (!(bool)rgc3Yes.IsChecked && !(bool)rgc3No.IsChecked) checkdata = false;
                    if (!string.IsNullOrEmpty(txtA_Comments.Text) && !string.IsNullOrEmpty(txtA_CT.Text) && !string.IsNullOrEmpty(txtA_IRT.Text) && !string.IsNullOrEmpty(txtAG_ByWhom.Text)
                       && !string.IsNullOrEmpty(txtAG_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtAG_Date.Text) && !string.IsNullOrEmpty(txtAG_SerialNo.Text)
                       && !string.IsNullOrEmpty(txtAT_ByWhom.Text) && !string.IsNullOrEmpty(txtAT_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtAT_Date.Text)
                       && !string.IsNullOrEmpty(txtAT_SerialNo.Text) && !string.IsNullOrEmpty(txtB_Comments.Text) && !string.IsNullOrEmpty(txtB_CT.Text)
                       && !string.IsNullOrEmpty(txtB_IRT.Text) && !string.IsNullOrEmpty(txtBG_ByWhom.Text) && !string.IsNullOrEmpty(txtBG_CalibrationDueDate.Text)
                       && !string.IsNullOrEmpty(txtBG_Date.Text) && !string.IsNullOrEmpty(txtBG_SerialNo.Text) && !string.IsNullOrEmpty(txtBT_ByWhom.Text)
                       && !string.IsNullOrEmpty(txtBT_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtBT_Date.Text) && !string.IsNullOrEmpty(txtBT_SerialNo.Text)
                       && !string.IsNullOrEmpty(txtC_Comments.Text) && !string.IsNullOrEmpty(txtC_CT.Text) && !string.IsNullOrEmpty(txtC_IRT.Text) && !string.IsNullOrEmpty(txtCG_ByWhom.Text)
                       && !string.IsNullOrEmpty(txtCG_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtCG_Date.Text) && !string.IsNullOrEmpty(txtCG_SerialNo.Text)
                       && !string.IsNullOrEmpty(txtCT_ByWhom.Text) && !string.IsNullOrEmpty(txtCT_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtCT_Date.Text)
                       && !string.IsNullOrEmpty(txtCT_SerialNo.Text) && !string.IsNullOrEmpty(txtVI1.Text) && !string.IsNullOrEmpty(txtVI2.Text)
                       && !string.IsNullOrEmpty(txtVP1.Text) && !string.IsNullOrEmpty(txtVP2.Text) && !string.IsNullOrEmpty(txtVP3.Text))
                        checkdata = true;
                    else
                        checkdata = false;
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
            try
            {
                if (!(bool)rgb3Na.IsChecked && !(bool)rgb3Yes.IsChecked && !(bool)rgb3No.IsChecked && !(bool)string.IsNullOrEmpty(txtVI3.Text) && !(bool)string.IsNullOrEmpty(cspVI3.SelectedDate.ToString())) return false;
                if (!(bool)rgb4Na.IsChecked && !(bool)rgb4Yes.IsChecked && !(bool)rgb4No.IsChecked && !(bool)string.IsNullOrEmpty(txtVI4.Text) && !(bool)string.IsNullOrEmpty(cspVI4.SelectedDate.ToString())) return false;
                if (!(bool)rgc4Na.IsChecked && !(bool)rgc4Yes.IsChecked && !(bool)rgc4No.IsChecked && !(bool)string.IsNullOrEmpty(txtVP4.Text) && !(bool)string.IsNullOrEmpty(cspVP4.SelectedDate.ToString())) return false;
                if (!(bool)rgc5Na.IsChecked && !(bool)rgc5Yes.IsChecked && !(bool)rgc5No.IsChecked && !(bool)string.IsNullOrEmpty(txtVP5.Text) && !(bool)string.IsNullOrEmpty(cspVP5.SelectedDate.ToString())) return false;
                if (!(bool)rgb1Yes.IsChecked && !(bool)rgb1No.IsChecked) return false;
                if (!(bool)rgb2Yes.IsChecked && !(bool)rgb2No.IsChecked) return false;
                if (!(bool)rgc1Yes.IsChecked && !(bool)rgc1No.IsChecked) return false;
                if (!(bool)rgc2Yes.IsChecked && !(bool)rgc2No.IsChecked) return false;
                if (!(bool)rgc3Yes.IsChecked && !(bool)rgc3No.IsChecked) return false;
                if (!string.IsNullOrEmpty(txtA_Comments.Text) && !string.IsNullOrEmpty(txtA_CT.Text) && !string.IsNullOrEmpty(txtA_IRT.Text) && !string.IsNullOrEmpty(txtAG_ByWhom.Text)
                   && !string.IsNullOrEmpty(txtAG_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtAG_Date.Text) && !string.IsNullOrEmpty(txtAG_SerialNo.Text)
                   && !string.IsNullOrEmpty(txtAT_ByWhom.Text) && !string.IsNullOrEmpty(txtAT_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtAT_Date.Text)
                   && !string.IsNullOrEmpty(txtAT_SerialNo.Text) && !string.IsNullOrEmpty(txtB_Comments.Text) && !string.IsNullOrEmpty(txtB_CT.Text)
                   && !string.IsNullOrEmpty(txtB_IRT.Text) && !string.IsNullOrEmpty(txtBG_ByWhom.Text) && !string.IsNullOrEmpty(txtBG_CalibrationDueDate.Text)
                   && !string.IsNullOrEmpty(txtBG_Date.Text) && !string.IsNullOrEmpty(txtBG_SerialNo.Text) && !string.IsNullOrEmpty(txtBT_ByWhom.Text)
                   && !string.IsNullOrEmpty(txtBT_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtBT_Date.Text) && !string.IsNullOrEmpty(txtBT_SerialNo.Text)
                   && !string.IsNullOrEmpty(txtC_Comments.Text) && !string.IsNullOrEmpty(txtC_CT.Text) && !string.IsNullOrEmpty(txtC_IRT.Text) && !string.IsNullOrEmpty(txtCG_ByWhom.Text)
                   && !string.IsNullOrEmpty(txtCG_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtCG_Date.Text) && !string.IsNullOrEmpty(txtCG_SerialNo.Text)
                   && !string.IsNullOrEmpty(txtCT_ByWhom.Text) && !string.IsNullOrEmpty(txtCT_CalibrationDueDate.Text) && !string.IsNullOrEmpty(txtCT_Date.Text)
                   && !string.IsNullOrEmpty(txtCT_SerialNo.Text) && !string.IsNullOrEmpty(txtVI1.Text) && !string.IsNullOrEmpty(txtVI2.Text)
                   && !string.IsNullOrEmpty(txtVP1.Text) && !string.IsNullOrEmpty(txtVP2.Text) && !string.IsNullOrEmpty(txtVP3.Text))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
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

        public bool isSelectedSign { get; set; }

        public event EventHandler SelectedSign;

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
