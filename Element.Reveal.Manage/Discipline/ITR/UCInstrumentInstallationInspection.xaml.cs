using Element.Reveal.Manage.Lib;
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

namespace Element.Reveal.Manage.Discipline.ITR
{
    public sealed partial class UCInstrumentInstallationInspection : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public UCInstrumentInstallationInspection()
        {
            this.InitializeComponent();

            chkIpC1.Checked += chkIpC1_Checked;
            chkIpC2.Checked += chkIpC2_Checked;
            chkIpC3.Checked += chkIpC3_Checked;
            chkIpC4.Checked += chkIpC4_Checked;
            chkIpC5.Checked += chkIpC5_Checked;
            chkIpC6.Checked += chkIpC6_Checked;
            chkIpC7.Checked += chkIpC7_Checked;
            chkIpC8.Checked += chkIpC8_Checked;
            chkIpC9.Checked += chkIpC9_Checked;
            chkIpC10.Checked += chkIpC10_Checked;
            chkIpC11.Checked += chkIpC11_Checked;
            chkIpC12.Checked += chkIpC12_Checked;
            chkIpC13.Checked += chkIpC13_Checked;

            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }

        void chkIpC1_Checked(object sender, RoutedEventArgs e) { rdoIpC1Yes.IsChecked = rdoIpC1No.IsChecked = !(bool)chkIpC1.IsChecked; }
        void chkIpC2_Checked(object sender, RoutedEventArgs e) { rdoIpC2Yes.IsChecked = rdoIpC2No.IsChecked = !(bool)chkIpC2.IsChecked; }
        void chkIpC3_Checked(object sender, RoutedEventArgs e) { rdoIpC3Yes.IsChecked = rdoIpC3No.IsChecked = !(bool)chkIpC3.IsChecked; }
        void chkIpC4_Checked(object sender, RoutedEventArgs e) { rdoIpC4Yes.IsChecked = rdoIpC4No.IsChecked = !(bool)chkIpC4.IsChecked; }
        void chkIpC5_Checked(object sender, RoutedEventArgs e) { rdoIpC5Yes.IsChecked = rdoIpC5No.IsChecked = !(bool)chkIpC5.IsChecked; }
        void chkIpC6_Checked(object sender, RoutedEventArgs e) { rdoIpC6Yes.IsChecked = rdoIpC6No.IsChecked = !(bool)chkIpC6.IsChecked; }
        void chkIpC7_Checked(object sender, RoutedEventArgs e) { rdoIpC7Yes.IsChecked = rdoIpC7No.IsChecked = !(bool)chkIpC7.IsChecked; }
        void chkIpC8_Checked(object sender, RoutedEventArgs e) { rdoIpC8Yes.IsChecked = rdoIpC8No.IsChecked = !(bool)chkIpC8.IsChecked; }
        void chkIpC9_Checked(object sender, RoutedEventArgs e) { rdoIpC9Yes.IsChecked = rdoIpC9No.IsChecked = !(bool)chkIpC9.IsChecked; }
        void chkIpC10_Checked(object sender, RoutedEventArgs e) { rdoIpC10Yes.IsChecked = rdoIpC10No.IsChecked = !(bool)chkIpC10.IsChecked; }
        void chkIpC11_Checked(object sender, RoutedEventArgs e) { rdoIpC11Yes.IsChecked = rdoIpC11No.IsChecked = !(bool)chkIpC11.IsChecked; }
        void chkIpC12_Checked(object sender, RoutedEventArgs e) { rdoIpC12Yes.IsChecked = rdoIpC12No.IsChecked = !(bool)chkIpC12.IsChecked; }
        void chkIpC13_Checked(object sender, RoutedEventArgs e) { rdoIpC13Yes.IsChecked = rdoIpC13No.IsChecked = !(bool)chkIpC13.IsChecked; }

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

        public void DoAfter(RevealProjectSvc.QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>>
            {
                new List<List<FrameworkElement>> {
                    new List<FrameworkElement> {txtIsptBy, txtIsptDt, txtRg}
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { chkIpC1, rdoIpC1Yes, rdoIpC1No },
                    new List<FrameworkElement> { chkIpC2, rdoIpC2Yes, rdoIpC2No },
                    new List<FrameworkElement> { chkIpC3, rdoIpC3Yes, rdoIpC3No },
                    new List<FrameworkElement> { chkIpC4, rdoIpC4Yes, rdoIpC4No },
                    new List<FrameworkElement> { chkIpC5, rdoIpC5Yes, rdoIpC5No },
                    new List<FrameworkElement> { chkIpC6, rdoIpC6Yes, rdoIpC6No },
                    new List<FrameworkElement> { chkIpC7, rdoIpC7Yes, rdoIpC7No },
                    new List<FrameworkElement> { chkIpC8, rdoIpC8Yes, rdoIpC8No },
                    new List<FrameworkElement> { chkIpC9, rdoIpC9Yes, rdoIpC9No },
                    new List<FrameworkElement> { chkIpC10, rdoIpC10Yes, rdoIpC10No },
                    new List<FrameworkElement> { chkIpC11, rdoIpC11Yes, rdoIpC11No },
                    new List<FrameworkElement> { chkIpC12, rdoIpC12Yes, rdoIpC12No },
                    new List<FrameworkElement> { chkIpC13, rdoIpC13Yes, rdoIpC13No },
                    new List<FrameworkElement> { txtInstrumentTagNo }
                }
            };
        }

        #region NFC Sign
        public bool isExistNFC { get { return false; } }
        public bool isSigned { get { return false; } }
        public bool isSelectedSign { get { return false; } set { } }
        public void SetNFCData(string _personmane, string _grade) { }
        public void ClearSelect() { }
        public event EventHandler SelectedSign;
        public void checkSelectSign() { }
        #endregion

        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }

        public bool Validate()
        {
            bool checkdata = true;
            try
            {
                if (txtIsptBy.Text.Trim() == "") checkdata = false;
                if (txtIsptDt.Text.Trim() == "") checkdata = false;
                if (txtRg.Text.Trim() == "") checkdata = false;

                if (!(bool)chkIpC1.IsChecked && !(bool)rdoIpC1Yes.IsChecked && !(bool)rdoIpC1No.IsChecked) checkdata = false;
                if (!(bool)chkIpC2.IsChecked && !(bool)rdoIpC2Yes.IsChecked && !(bool)rdoIpC2No.IsChecked) checkdata = false;
                if (!(bool)chkIpC3.IsChecked && !(bool)rdoIpC3Yes.IsChecked && !(bool)rdoIpC3No.IsChecked) checkdata = false;
                if (!(bool)chkIpC4.IsChecked && !(bool)rdoIpC4Yes.IsChecked && !(bool)rdoIpC4No.IsChecked) checkdata = false;
                if (!(bool)chkIpC5.IsChecked && !(bool)rdoIpC5Yes.IsChecked && !(bool)rdoIpC5No.IsChecked) checkdata = false;
                if (!(bool)chkIpC6.IsChecked && !(bool)rdoIpC6Yes.IsChecked && !(bool)rdoIpC6No.IsChecked) checkdata = false;
                if (!(bool)chkIpC7.IsChecked && !(bool)rdoIpC7Yes.IsChecked && !(bool)rdoIpC7No.IsChecked) checkdata = false;
                if (!(bool)chkIpC8.IsChecked && !(bool)rdoIpC8Yes.IsChecked && !(bool)rdoIpC8No.IsChecked) checkdata = false;
                if (!(bool)chkIpC9.IsChecked && !(bool)rdoIpC9Yes.IsChecked && !(bool)rdoIpC9No.IsChecked) checkdata = false;
                if (!(bool)chkIpC10.IsChecked && !(bool)rdoIpC10Yes.IsChecked && !(bool)rdoIpC10No.IsChecked) checkdata = false;
                if (!(bool)chkIpC11.IsChecked && !(bool)rdoIpC11Yes.IsChecked && !(bool)rdoIpC11No.IsChecked) checkdata = false;
                if (!(bool)chkIpC12.IsChecked && !(bool)rdoIpC12Yes.IsChecked && !(bool)rdoIpC12No.IsChecked) checkdata = false;
                if (!(bool)chkIpC13.IsChecked && !(bool)rdoIpC13Yes.IsChecked && !(bool)rdoIpC13No.IsChecked) checkdata = false;
                if (txtIpC.Text.Trim() == "") checkdata = false;
            }
            catch (Exception ex)
            {
                checkdata = false;
            }
            return checkdata;
        }

        public async Task<bool> Validate2()
        {
            bool checkdata = true;
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (txtIsptBy.Text.Trim() == "") checkdata = false;
                    if (txtIsptDt.Text.Trim() == "") checkdata = false;
                    if (txtIstmln.Text.Trim() == "") checkdata = false;
                    if (txtRg.Text.Trim() == "") checkdata = false;

                    if (!(bool)chkIpC1.IsChecked && !(bool)rdoIpC1Yes.IsChecked && !(bool)rdoIpC1No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC2.IsChecked && !(bool)rdoIpC2Yes.IsChecked && !(bool)rdoIpC2No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC3.IsChecked && !(bool)rdoIpC3Yes.IsChecked && !(bool)rdoIpC3No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC4.IsChecked && !(bool)rdoIpC4Yes.IsChecked && !(bool)rdoIpC4No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC5.IsChecked && !(bool)rdoIpC5Yes.IsChecked && !(bool)rdoIpC5No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC6.IsChecked && !(bool)rdoIpC6Yes.IsChecked && !(bool)rdoIpC6No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC7.IsChecked && !(bool)rdoIpC7Yes.IsChecked && !(bool)rdoIpC7No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC8.IsChecked && !(bool)rdoIpC8Yes.IsChecked && !(bool)rdoIpC8No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC9.IsChecked && !(bool)rdoIpC9Yes.IsChecked && !(bool)rdoIpC9No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC10.IsChecked && !(bool)rdoIpC10Yes.IsChecked && !(bool)rdoIpC10No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC11.IsChecked && !(bool)rdoIpC11Yes.IsChecked && !(bool)rdoIpC11No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC12.IsChecked && !(bool)rdoIpC12Yes.IsChecked && !(bool)rdoIpC12No.IsChecked) checkdata = false;
                    if (!(bool)chkIpC13.IsChecked && !(bool)rdoIpC13Yes.IsChecked && !(bool)rdoIpC13No.IsChecked) checkdata = false;
                    if (txtIpC.Text.Trim() == "") checkdata = false;
                });
            }
            catch (Exception ex)
            {
                checkdata = false;
            }
            return checkdata;
        }
    }
}
