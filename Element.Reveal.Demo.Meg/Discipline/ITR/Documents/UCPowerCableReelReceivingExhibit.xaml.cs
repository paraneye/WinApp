using Element.Reveal.Meg.Lib;
using Element.Reveal.Meg.RevealProjectSvc;
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

namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCPowerCableReelReceivingExhibit : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        private new List<WinAppLibrary.UI.ObjectNFCSign> signed;

        public UCPowerCableReelReceivingExhibit()
        {
            this.InitializeComponent();
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
                    new List<FrameworkElement> { inspDate, txtInpectedBy } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtLength, txtUOM } ,
                    new List<FrameworkElement> {chkYes, chkNo}
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { lvNFCSignList2 } 
                },
                new List<List<FrameworkElement>>{                
                    new List<FrameworkElement> {txtAGRND, txtBGRND, txtCGRND, txtNGRND, txtABCNGRND, txtComments},
                    new List<FrameworkElement> {txtEquip, txtSerial, txtCalibrationDueDate, txtByWhom, txtData2}
                }
            };
            lvNFCSignList2.SelectionChanged += lvNFCSignList2_SelectionChanged;

            ProjectName.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            ProjectNumber.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            CWPName.Text = (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            JobNumber.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            FIWPName.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            SystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";
            SystemNumber.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";

            //Header Binding
            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();
            StringVar9.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue1)) ? HeaderDto.StringValue1 : "";
            StringVar15.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue2)) ? HeaderDto.StringValue2 : "";
            StringVar6.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue3)) ? HeaderDto.StringValue3 : "";
            txtConductors.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue4)) ? HeaderDto.StringValue4 : "";
            StringVar4.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue5)) ? HeaderDto.StringValue5 : "";
            txtUOM.Text = "";

            //Grid Binding
            List<QaqcformdetailDTO> grdDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Grid).ToList() ;
            gvCable.ItemsSource = grdDto;
            txtTotalAssigned.Text = grdDto.Sum(x => Convert.ToDecimal(x.StringValue2)).ToString();

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
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (!(bool)chkYes.IsChecked && !(bool)chkNo.IsChecked) checkdata = false;
                if (txtAGRND.Text == "") checkdata = false;
                if (txtBGRND.Text == "") checkdata = false;
                if (txtCGRND.Text == "") checkdata = false;
                if (txtNGRND.Text == "") checkdata = false;
                if (txtEquip.Text == "") checkdata = false;
                if (txtSerial.Text == "") checkdata = false;
                if (txtByWhom.Text == "") checkdata = false;
            });
            return checkdata;
        }

        public bool Validate()
        {
            if (!(bool)chkYes.IsChecked && !(bool)chkNo.IsChecked) return false;
            if (txtAGRND.Text == "") return false;
            if (txtBGRND.Text == "") return false;
            if (txtCGRND.Text == "") return false;
            if (txtNGRND.Text == "") return false;
            if (txtEquip.Text == "") return false;
            if (txtSerial.Text == "") return false;
            if (txtByWhom.Text == "") return false;
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
            List<RevealProjectSvc.QaqcformdetailDTO> tmp = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

        public bool isSigned
        {
            get
            {
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
