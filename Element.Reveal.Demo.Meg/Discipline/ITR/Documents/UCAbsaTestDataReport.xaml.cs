using Element.Reveal.Meg.Lib;
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
    public sealed partial class UCAbsaTestDataReport : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public UCAbsaTestDataReport()
        {
            this.InitializeComponent();
            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }
        public void Load()
        {
            FormSerialize.Load(controls, QAQCDTOList);
        }

        public void Save()
        {
            QAQCDTOList.Clear();
            //TODO : Group에 따라 작성
            FormSerialize.GenDTO(QAQCGroup.GROUP01, controls[0], QAQCDTOList);
        }

        public void DoAfter(RevealProjectSvc.QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>>
            {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { chkTop1, chkTop2, chkTop3, chkTop4 },
                    new List<FrameworkElement> { txtCBy, txtOJNo, txtCBAddr, CertiAQP, dtEx1 },
                    new List<FrameworkElement> { txtCAP, dtEx2new },
                    new List<FrameworkElement> { txtOwAddr, txtAQP, dtEx3 },
                    new List<FrameworkElement> { chkDR1, chkDR2, txtWP1, txtWPCom, txtWP2, txtWPSused, txtWPSOwn, txtComment }
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { nfcSign1 },
                    new List<FrameworkElement> { txtSignedJobMo },
                    new List<FrameworkElement> { nfcSign2 },
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
                if (!(bool)chkTop1.IsChecked && !(bool)chkTop2.IsChecked && !(bool)chkTop3.IsChecked && !(bool)chkTop4.IsChecked) checkdata = false;
                if (!(bool)chkDR1.IsChecked && !(bool)chkDR2.IsChecked) checkdata = false;

                if (txtCBy.Text == "" || txtOJNo.Text == "" || txtCBAddr.Text == "" || CertiAQP.Text == "" || txtCAP.Text == "" || txtOwAddr.Text == "" || txtAQP.Text == "") checkdata = false;
                if (txtWP1.Text == "" || txtWPCom.Text == "" || txtWP2.Text == "" || txtWPSused.Text == "" || txtWPSOwn.Text == "" || txtComment.Text == "") checkdata = false;
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
                    if (!(bool)chkTop1.IsChecked && !(bool)chkTop2.IsChecked && !(bool)chkTop3.IsChecked && !(bool)chkTop4.IsChecked) checkdata = false;
                    if (!(bool)chkDR1.IsChecked && !(bool)chkDR2.IsChecked) checkdata = false;

                    if (txtCBy.Text == "" || txtOJNo.Text == "" || txtCBAddr.Text == "" || CertiAQP.Text == "" || txtCAP.Text == "" || txtOwAddr.Text == "" || txtAQP.Text == "") checkdata = false;
                    if (txtWP1.Text == "" || txtWPCom.Text == "" || txtWP2.Text == "" || txtWPSused.Text == "" || txtWPSOwn.Text == "" || txtComment.Text == "") checkdata = false;
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
