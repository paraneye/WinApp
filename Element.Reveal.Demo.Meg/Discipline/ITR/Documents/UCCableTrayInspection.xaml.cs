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
using Windows.UI.Core;
using Element.Reveal.Meg.RevealProjectSvc;

namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCCableTrayInspection : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public UCCableTrayInspection()
        {
            this.InitializeComponent();

            chkVIC1.Checked += chkVIC1_Checked;
            chkVIC2.Checked += chkVIC2_Checked;
            chkVIC3.Checked += chkVIC3_Checked;
            chkVIC4.Checked += chkVIC4_Checked;
            chkVIC5.Checked += chkVIC5_Checked;
            chkVIC6.Checked += chkVIC6_Checked;
            chkVIC7.Checked += chkVIC7_Checked;
            chkVIC8.Checked += chkVIC8_Checked;
            chkVIC9.Checked += chkVIC9_Checked;
            chkVIC10.Checked += chkVIC10_Checked;
            chkVIC11.Checked += chkVIC11_Checked;
            chkVIC12.Checked += chkVIC12_Checked;
            chkVIC13.Checked += chkVIC13_Checked;
            chkVIC14.Checked += chkVIC14_Checked;
            chkVIC15.Checked += chkVIC15_Checked;
            chkVIC16.Checked += chkVIC16_Checked;
            chkVIC17.Checked += chkVIC17_Checked;
            chkVIC18.Checked += chkVIC18_Checked;
            chkVIC19.Checked += chkVIC19_Checked;
            chkVIC20.Checked += chkVIC20_Checked;

            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

        void chkVIC1_Checked(object sender, RoutedEventArgs e) { rdoVIC1Yes.IsChecked = rdoVIC1No.IsChecked = !(bool)chkVIC1.IsChecked; }
        void chkVIC2_Checked(object sender, RoutedEventArgs e) { rdoVIC2Yes.IsChecked = rdoVIC2No.IsChecked = !(bool)chkVIC2.IsChecked; }
        void chkVIC3_Checked(object sender, RoutedEventArgs e) { rdoVIC3Yes.IsChecked = rdoVIC3No.IsChecked = !(bool)chkVIC3.IsChecked; }
        void chkVIC4_Checked(object sender, RoutedEventArgs e) { rdoVIC4Yes.IsChecked = rdoVIC4No.IsChecked = !(bool)chkVIC4.IsChecked; }
        void chkVIC5_Checked(object sender, RoutedEventArgs e) { rdoVIC5Yes.IsChecked = rdoVIC5No.IsChecked = !(bool)chkVIC5.IsChecked; }
        void chkVIC6_Checked(object sender, RoutedEventArgs e) { rdoVIC6Yes.IsChecked = rdoVIC6No.IsChecked = !(bool)chkVIC6.IsChecked; }
        void chkVIC7_Checked(object sender, RoutedEventArgs e) { rdoVIC7Yes.IsChecked = rdoVIC7No.IsChecked = !(bool)chkVIC7.IsChecked; }
        void chkVIC8_Checked(object sender, RoutedEventArgs e) { rdoVIC8Yes.IsChecked = rdoVIC8No.IsChecked = !(bool)chkVIC8.IsChecked; }
        void chkVIC9_Checked(object sender, RoutedEventArgs e) { rdoVIC9Yes.IsChecked = rdoVIC9No.IsChecked = !(bool)chkVIC9.IsChecked; }
        void chkVIC10_Checked(object sender, RoutedEventArgs e) { rdoVIC10Yes.IsChecked = rdoVIC10No.IsChecked = !(bool)chkVIC10.IsChecked; }
        void chkVIC11_Checked(object sender, RoutedEventArgs e) { rdoVIC11Yes.IsChecked = rdoVIC11No.IsChecked = !(bool)chkVIC11.IsChecked; }
        void chkVIC12_Checked(object sender, RoutedEventArgs e) { rdoVIC12Yes.IsChecked = rdoVIC12No.IsChecked = !(bool)chkVIC12.IsChecked; }
        void chkVIC13_Checked(object sender, RoutedEventArgs e) { rdoVIC13Yes.IsChecked = rdoVIC13No.IsChecked = !(bool)chkVIC13.IsChecked; }
        void chkVIC14_Checked(object sender, RoutedEventArgs e) { rdoVIC14Yes.IsChecked = rdoVIC14No.IsChecked = !(bool)chkVIC14.IsChecked; }
        void chkVIC15_Checked(object sender, RoutedEventArgs e) { rdoVIC15Yes.IsChecked = rdoVIC15No.IsChecked = !(bool)chkVIC15.IsChecked; }
        void chkVIC16_Checked(object sender, RoutedEventArgs e) { rdoVIC16Yes.IsChecked = rdoVIC16No.IsChecked = !(bool)chkVIC16.IsChecked; }
        void chkVIC17_Checked(object sender, RoutedEventArgs e) { rdoVIC17Yes.IsChecked = rdoVIC17No.IsChecked = !(bool)chkVIC17.IsChecked; }
        void chkVIC18_Checked(object sender, RoutedEventArgs e) { rdoVIC18Yes.IsChecked = rdoVIC18No.IsChecked = !(bool)chkVIC18.IsChecked; }
        void chkVIC19_Checked(object sender, RoutedEventArgs e) { rdoVIC19Yes.IsChecked = rdoVIC19No.IsChecked = !(bool)chkVIC19.IsChecked; }
        void chkVIC20_Checked(object sender, RoutedEventArgs e) { rdoVIC20Yes.IsChecked = rdoVIC20No.IsChecked = !(bool)chkVIC20.IsChecked; }

        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { dtpInspectionDate, txtInspectionBy } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { chkVIC1, rdoVIC1Yes, rdoVIC1No },
                    new List<FrameworkElement> { chkVIC2, rdoVIC2Yes, rdoVIC2No },
                    new List<FrameworkElement> { chkVIC3, rdoVIC3Yes, rdoVIC3No },
                    new List<FrameworkElement> { chkVIC4, rdoVIC4Yes, rdoVIC4No },
                    new List<FrameworkElement> { chkVIC5, rdoVIC5Yes, rdoVIC5No },
                    new List<FrameworkElement> { chkVIC6, rdoVIC6Yes, rdoVIC6No },
                    new List<FrameworkElement> { chkVIC7, rdoVIC7Yes, rdoVIC7No },
                    new List<FrameworkElement> { chkVIC8, rdoVIC8Yes, rdoVIC8No },
                    new List<FrameworkElement> { chkVIC9, rdoVIC9Yes, rdoVIC9No },
                    new List<FrameworkElement> { chkVIC10, rdoVIC10Yes, rdoVIC10No },
                    new List<FrameworkElement> { chkVIC11, rdoVIC11Yes, rdoVIC11No },
                    new List<FrameworkElement> { chkVIC12, rdoVIC12Yes, rdoVIC12No },
                    new List<FrameworkElement> { chkVIC13, rdoVIC13Yes, rdoVIC13No },
                    new List<FrameworkElement> { chkVIC14, rdoVIC14Yes, rdoVIC14No },
                    new List<FrameworkElement> { chkVIC15, rdoVIC15Yes, rdoVIC15No },
                    new List<FrameworkElement> { chkVIC16, rdoVIC16Yes, rdoVIC16No },
                    new List<FrameworkElement> { chkVIC17, rdoVIC17Yes, rdoVIC17No },
                    new List<FrameworkElement> { chkVIC18, rdoVIC18Yes, rdoVIC18No },
                    new List<FrameworkElement> { chkVIC19, rdoVIC19Yes, rdoVIC19No },
                    new List<FrameworkElement> { chkVIC20, rdoVIC20Yes, rdoVIC20No },
                    new List<FrameworkElement> { txtVIC }
                }
            };
            QaqcformdetailDTO header = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Header select lv).FirstOrDefault<QaqcformdetailDTO>();

            txtCableTrayTagNo.Text = (!string.IsNullOrEmpty(_dto.TagNumber)) ? _dto.TagNumber : "";
            txtProjectName.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            txtProjectNo.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            txtJobNo.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            txtContractNo.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";

            txtRefDrwg.Text = (!string.IsNullOrEmpty(header.StringValue1)) ? header.StringValue1 : "";
            txtRefSpec.Text = (!string.IsNullOrEmpty(header.StringValue2)) ? header.StringValue2 : "";
            txtCableTrayFrom.Text = (!string.IsNullOrEmpty(header.StringValue3)) ? header.StringValue3 : "";
            txtCableTrayTo.Text = (!string.IsNullOrEmpty(header.StringValue4)) ? header.StringValue4 : "";
            txtIWPNo.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            txtSystemNo.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            txtSystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";

            txtInspectionBy.Text = Login.UserAccount.UserName;
        }
        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }
        public bool Validate()
        {
            if (txtInspectionBy.Text.Trim() == "") return false;
            if (!(bool)chkVIC1.IsChecked && !(bool)rdoVIC1Yes.IsChecked && !(bool)rdoVIC1No.IsChecked) return false;
            if (!(bool)chkVIC2.IsChecked && !(bool)rdoVIC2Yes.IsChecked && !(bool)rdoVIC2No.IsChecked) return false;
            if (!(bool)chkVIC3.IsChecked && !(bool)rdoVIC3Yes.IsChecked && !(bool)rdoVIC3No.IsChecked) return false;
            if (!(bool)chkVIC4.IsChecked && !(bool)rdoVIC4Yes.IsChecked && !(bool)rdoVIC4No.IsChecked) return false;
            if (!(bool)chkVIC5.IsChecked && !(bool)rdoVIC5Yes.IsChecked && !(bool)rdoVIC5No.IsChecked) return false;
            if (!(bool)chkVIC6.IsChecked && !(bool)rdoVIC6Yes.IsChecked && !(bool)rdoVIC6No.IsChecked) return false;
            if (!(bool)chkVIC7.IsChecked && !(bool)rdoVIC7Yes.IsChecked && !(bool)rdoVIC7No.IsChecked) return false;
            if (!(bool)chkVIC8.IsChecked && !(bool)rdoVIC8Yes.IsChecked && !(bool)rdoVIC8No.IsChecked) return false;
            if (!(bool)chkVIC9.IsChecked && !(bool)rdoVIC9Yes.IsChecked && !(bool)rdoVIC9No.IsChecked) return false;
            if (!(bool)chkVIC10.IsChecked && !(bool)rdoVIC10Yes.IsChecked && !(bool)rdoVIC10No.IsChecked) return false;
            if (!(bool)chkVIC11.IsChecked && !(bool)rdoVIC11Yes.IsChecked && !(bool)rdoVIC11No.IsChecked) return false;
            if (!(bool)chkVIC12.IsChecked && !(bool)rdoVIC12Yes.IsChecked && !(bool)rdoVIC12No.IsChecked) return false;
            if (!(bool)chkVIC13.IsChecked && !(bool)rdoVIC13Yes.IsChecked && !(bool)rdoVIC13No.IsChecked) return false;
            if (!(bool)chkVIC14.IsChecked && !(bool)rdoVIC14Yes.IsChecked && !(bool)rdoVIC14No.IsChecked) return false;
            if (!(bool)chkVIC15.IsChecked && !(bool)rdoVIC15Yes.IsChecked && !(bool)rdoVIC15No.IsChecked) return false;
            if (!(bool)chkVIC16.IsChecked && !(bool)rdoVIC16Yes.IsChecked && !(bool)rdoVIC16No.IsChecked) return false;
            if (!(bool)chkVIC17.IsChecked && !(bool)rdoVIC17Yes.IsChecked && !(bool)rdoVIC17No.IsChecked) return false;
            if (!(bool)chkVIC18.IsChecked && !(bool)rdoVIC18Yes.IsChecked && !(bool)rdoVIC18No.IsChecked) return false;
            if (!(bool)chkVIC19.IsChecked && !(bool)rdoVIC19Yes.IsChecked && !(bool)rdoVIC19No.IsChecked) return false;
            if (!(bool)chkVIC20.IsChecked && !(bool)rdoVIC20Yes.IsChecked && !(bool)rdoVIC20No.IsChecked) return false;
            return true;
        }

        public async Task<bool> Validate2()
        {
            bool checkdata = true;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (txtInspectionBy.Text.Trim() == "") checkdata = false;
                if (!(bool)chkVIC1.IsChecked && !(bool)rdoVIC1Yes.IsChecked && !(bool)rdoVIC1No.IsChecked) checkdata = false;
                if (!(bool)chkVIC2.IsChecked && !(bool)rdoVIC2Yes.IsChecked && !(bool)rdoVIC2No.IsChecked) checkdata = false;
                if (!(bool)chkVIC3.IsChecked && !(bool)rdoVIC3Yes.IsChecked && !(bool)rdoVIC3No.IsChecked) checkdata = false;
                if (!(bool)chkVIC4.IsChecked && !(bool)rdoVIC4Yes.IsChecked && !(bool)rdoVIC4No.IsChecked) checkdata = false;
                if (!(bool)chkVIC5.IsChecked && !(bool)rdoVIC5Yes.IsChecked && !(bool)rdoVIC5No.IsChecked) checkdata = false;
                if (!(bool)chkVIC6.IsChecked && !(bool)rdoVIC6Yes.IsChecked && !(bool)rdoVIC6No.IsChecked) checkdata = false;
                if (!(bool)chkVIC7.IsChecked && !(bool)rdoVIC7Yes.IsChecked && !(bool)rdoVIC7No.IsChecked) checkdata = false;
                if (!(bool)chkVIC8.IsChecked && !(bool)rdoVIC8Yes.IsChecked && !(bool)rdoVIC8No.IsChecked) checkdata = false;
                if (!(bool)chkVIC9.IsChecked && !(bool)rdoVIC9Yes.IsChecked && !(bool)rdoVIC9No.IsChecked) checkdata = false;
                if (!(bool)chkVIC10.IsChecked && !(bool)rdoVIC10Yes.IsChecked && !(bool)rdoVIC10No.IsChecked) checkdata = false;
                if (!(bool)chkVIC11.IsChecked && !(bool)rdoVIC11Yes.IsChecked && !(bool)rdoVIC11No.IsChecked) checkdata = false;
                if (!(bool)chkVIC12.IsChecked && !(bool)rdoVIC12Yes.IsChecked && !(bool)rdoVIC12No.IsChecked) checkdata = false;
                if (!(bool)chkVIC13.IsChecked && !(bool)rdoVIC13Yes.IsChecked && !(bool)rdoVIC13No.IsChecked) checkdata = false;
                if (!(bool)chkVIC14.IsChecked && !(bool)rdoVIC14Yes.IsChecked && !(bool)rdoVIC14No.IsChecked) checkdata = false;
                if (!(bool)chkVIC15.IsChecked && !(bool)rdoVIC15Yes.IsChecked && !(bool)rdoVIC15No.IsChecked) checkdata = false;
                if (!(bool)chkVIC16.IsChecked && !(bool)rdoVIC16Yes.IsChecked && !(bool)rdoVIC16No.IsChecked) checkdata = false;
                if (!(bool)chkVIC17.IsChecked && !(bool)rdoVIC17Yes.IsChecked && !(bool)rdoVIC17No.IsChecked) checkdata = false;
                if (!(bool)chkVIC18.IsChecked && !(bool)rdoVIC18Yes.IsChecked && !(bool)rdoVIC18No.IsChecked) checkdata = false;
                if (!(bool)chkVIC19.IsChecked && !(bool)rdoVIC19Yes.IsChecked && !(bool)rdoVIC19No.IsChecked) checkdata = false;
                if (!(bool)chkVIC20.IsChecked && !(bool)rdoVIC20Yes.IsChecked && !(bool)rdoVIC20No.IsChecked) checkdata = false;
                
            });
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
