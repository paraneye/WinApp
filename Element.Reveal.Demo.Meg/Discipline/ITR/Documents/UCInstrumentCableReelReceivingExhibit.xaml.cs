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
using Element.Reveal.Meg.Lib;
using Windows.UI.Core;
using System.Threading.Tasks;
using Element.Reveal.Meg.RevealProjectSvc; 
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCInstrumentCableReelReceivingExhibit : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        private new List<WinAppLibrary.UI.ObjectNFCSign> signed;

        public UCInstrumentCableReelReceivingExhibit()
        {
            this.InitializeComponent();
            rdoYes.Checked += rdoYes_Checked;
            rdoNo.Checked += rdoNo_Checked;

            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();

            signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                new WinAppLibrary.UI.ObjectNFCSign{
                isSigned = "Unsigned",
                MemberGrade = "MM",
                PersonnelName = null,
                SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                SignedTime = null//DateTime.Now.ToString()
            }};

            lvNFCSignList2.DataContext = signed;
        }

        void rdoNo_Checked(object sender, RoutedEventArgs e)
        {
            rdoYes.IsChecked = false;
        }

        void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            rdoNo.IsChecked = false;
        }

        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { dtpInspectionDate, txtInpectedBy } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtLength, txtUOM },
                    new List<FrameworkElement> {rdoYes,rdoNo}                
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> {lvNFCSignList2}             
                },
                new List<List<FrameworkElement>>{                  
                    new List<FrameworkElement> {txtCONDtoCOND, txtCONDtoGRND, txtCONDtoSHIELD, txtSHIELDtoGRND, txtComments},
                    new List<FrameworkElement> {txtEquip, txtSerial, txtCalibrationDueDate, txtByWhom, dtpDate}
                }
            };
            lvNFCSignList2.SelectionChanged += lvNFCSignList2_SelectionChanged;

            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();
            ProjectName.Text =  (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            ProjectNumber.Text =  (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            CWPName.Text = (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            JobNumber.Text =  (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            FIWPName.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            SystemNumber.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            SystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";

            if (HeaderDto != null)
            {
                StringVar9.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue1)) ? HeaderDto.StringValue1 : "";
                StringVar15.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue2)) ? HeaderDto.StringValue2 : "";
                StringVar6.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue3)) ? HeaderDto.StringValue3 : "";
                txtConductors.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue4)) ? HeaderDto.StringValue4 : "";
                StringVar4.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue5)) ? HeaderDto.StringValue5 : "";
            }

            List<QaqcformdetailDTO> grid = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Grid select lv).ToList<QaqcformdetailDTO>();
            if (grid != null)
            {
                lvList.ItemsSource = grid;
                txtTotalAssigned.Text = grid.Sum(o => Convert.ToDecimal(o.StringValue2)).ToString();
            }

            this.txtInpectedBy.Text = Login.UserAccount.UserName;
        }

        public bool isValidate { get; set; }
        public bool Validate()
        {
            return true;
            bool checkdata = true;
            try
            {
                
                if (!string.IsNullOrEmpty(txtByWhom.Text) && !string.IsNullOrEmpty(txtCalibrationDueDate.Text) && !string.IsNullOrEmpty(txtComments.Text)
                    && !string.IsNullOrEmpty(txtCONDtoCOND.Text) && !string.IsNullOrEmpty(txtCONDtoGRND.Text) && !string.IsNullOrEmpty(txtCONDtoSHIELD.Text)
                    && !string.IsNullOrEmpty(txtEquip.Text) && !string.IsNullOrEmpty(txtInpectedBy.Text)
                    && !string.IsNullOrEmpty(txtLength.Text) && !string.IsNullOrEmpty(txtSerial.Text) && !string.IsNullOrEmpty(txtSHIELDtoGRND.Text)
                    && !string.IsNullOrEmpty(txtUOM.Text) 
                    && (rdoYes.IsChecked != false || rdoNo.IsChecked != false))
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

        public async Task<bool> Validate2()
        {
            return true;
            bool checkdata = true;
            
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (!string.IsNullOrEmpty(txtByWhom.Text) && !string.IsNullOrEmpty(txtCalibrationDueDate.Text) && !string.IsNullOrEmpty(txtComments.Text)
                    && !string.IsNullOrEmpty(txtCONDtoCOND.Text) && !string.IsNullOrEmpty(txtCONDtoGRND.Text) && !string.IsNullOrEmpty(txtCONDtoSHIELD.Text)
                    && !string.IsNullOrEmpty(txtEquip.Text) && !string.IsNullOrEmpty(txtInpectedBy.Text)
                    && !string.IsNullOrEmpty(txtLength.Text) && !string.IsNullOrEmpty(txtSerial.Text) && !string.IsNullOrEmpty(txtSHIELDtoGRND.Text)
                    && !string.IsNullOrEmpty(txtUOM.Text)
                    && (rdoYes.IsChecked != false || rdoNo.IsChecked != false))
                    {
                        checkdata = true;
                    }
                    else
                        checkdata = false;
                });
            return checkdata;
        }

        public async void checkValidate()
        {
            isValidate = await Validate2();
           
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
            List<RevealProjectSvc.QaqcformdetailDTO> tmp = new List<RevealProjectSvc.QaqcformdetailDTO>();
           // foreach (RevealProjectSvc.QaqcformdetailDTO dto in QAQCDTOList)
           //     tmp.Add(dto);

          //  QAQCDTOList.Clear();
          //  QAQCDTOList.AddRange(tmp);
        }

        public bool isSigned
        {
            get {
                return (((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList2.Items[0]).PersonnelName == null) ? false : true;
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
                    MemberGrade = _grade,
                    SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                    SignedTime = DateTime.Now.ToString()
                    }};
                    lvNFCSignList2.DataContext = signed;
                });
            }
            catch (Exception e)
            {
            }
        }

        void lvNFCSignList2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvNFCSignList2.SelectedItems.Count > 0)
            {
                if (SelectedSign != null)
                    SelectedSign(this, null);
            }
        }

        public void ClearSelect()
        {
            lvNFCSignList2.SelectedItem = null;
        }

     //   public bool isSelectedSign { get { return (lvNFCSignList2.SelectedItems.Count > 0) ? true : false; } }
        public bool isSelectedSign { get; set; }

        public event EventHandler SelectedSign;

        public async void checkSelectSign()
        {
            bool bl = false;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bl = lvNFCSignList2.SelectedItems.Count > 0 ? true : false;
                isSelectedSign = bl;
            });
           
        }

        private void lvNFCSignList2_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(e.Velocities.Linear.X) > WinAppLibrary.Utilities.AnimationHelper.VelocityThreshold)
            {
                
                signed = new List<WinAppLibrary.UI.ObjectNFCSign>{
                new WinAppLibrary.UI.ObjectNFCSign{
                isSigned = "UnSigned",
                PersonnelName = "",
                MemberGrade = "MM",
                SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen),
                SignedTime = ""
                }};
                lvNFCSignList2.DataContext = signed;
            }
        }        
    }
}
