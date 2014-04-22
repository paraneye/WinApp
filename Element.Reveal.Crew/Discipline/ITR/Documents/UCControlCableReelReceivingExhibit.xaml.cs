using Element.Reveal.Crew.Lib;
using Element.Reveal.Crew.RevealProjectSvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Element.Reveal.Crew.Discipline.ITR
{
    public sealed partial class UCControlCableReelReceivingExhibit : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        private new List<WinAppLibrary.UI.ObjectNFCSign> signed;
        public UCControlCableReelReceivingExhibit()
        {
            this.InitializeComponent();
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

        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { dtpInspectionDate, txtInpectedBy } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtLength, txtUOM },
                    new List<FrameworkElement> { chkYes, chkNo}               
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> {lvNFCSignList2}           
                },
                new List<List<FrameworkElement>>{     
                    new List<FrameworkElement> { txtCOND, txtGRND, txtComments},
                    new List<FrameworkElement> { txtEquip, txtSerial, dtpCalibrationDueDate, txtByWhom, dtpDate}
                }
            };
            txtProjectName.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            txtProjectNumber.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            txtCwpEwpNo.Text = (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            txtContractorJobNo.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            txtIwpNo.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            txtSystemNo.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            txtSystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";

            List<QaqcformdetailDTO> grid = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Grid select lv).ToList<QaqcformdetailDTO>();
            QaqcformdetailDTO header = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Header select lv).FirstOrDefault<QaqcformdetailDTO>();

            lvList.ItemsSource = grid;
            txtReelNo.Text =  (!string.IsNullOrEmpty(header.StringValue1)) ? header.StringValue1 : "";
            txtCableType.Text = (!string.IsNullOrEmpty(header.StringValue2)) ? header.StringValue2 : "";
            txtVoltageRating.Text = (!string.IsNullOrEmpty(header.StringValue3)) ? header.StringValue3 : "";
            txtConductors.Text = (!string.IsNullOrEmpty(header.StringValue4)) ? header.StringValue4 : "";
            txtCableSize.Text = (!string.IsNullOrEmpty(header.StringValue5)) ? header.StringValue5 : "";
            txtTotalAssigned.Text = grid.Sum(o => Convert.ToDecimal(o.StringValue2)).ToString();

            this.txtInpectedBy.Text = Login.UserAccount.UserName;
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
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>{
                    if (txtInpectedBy.Text == "" || txtLength.Text == "" || txtUOM.Text == "" || txtCOND.Text == "" || txtGRND.Text == "" || txtComments.Text == "" || txtEquip.Text == "" || txtSerial.Text == "" ||
                       txtByWhom.Text == "")
                    {
                        checkdata = false;
                    }

                    if (chkNo.IsChecked == false && chkYes.IsChecked == false)
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
            bool checkdata = true;
            try
            {
                if(txtInpectedBy.Text == "" || txtLength.Text == "" || txtUOM.Text == "" || txtCOND.Text == "" || txtGRND.Text == "" || txtComments.Text == "" || txtEquip.Text == "" || txtSerial.Text == "" ||
                   txtByWhom.Text == "" )
                {
                    checkdata = false;
                }
                
                if(chkNo.IsChecked == false && chkYes.IsChecked == false)
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
            get { return true; }
        }
        public bool isSigned
        {
            get
            {
                return (((WinAppLibrary.UI.ObjectNFCSign)lvNFCSignList2.Items[0]).PersonnelName == null) ? false : true;
            }
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
