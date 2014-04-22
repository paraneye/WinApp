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
    public sealed partial class UCPipeCleaning : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public UCPipeCleaning()
        {
            this.InitializeComponent();
            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();

            nfcCon1.MakeSmall(); nfcCon2.MakeSmall(); nfcCon3.MakeSmall(); nfcCon4.MakeSmall(); nfcCon5.MakeSmall(); nfcCon6.MakeSmall(); nfcCon7.MakeSmall();
            nfcMeg1.MakeSmall(); nfcMeg2.MakeSmall(); nfcMeg3.MakeSmall(); nfcMeg4.MakeSmall(); nfcMeg5.MakeSmall(); nfcMeg6.MakeSmall(); nfcMeg7.MakeSmall(); 
        }

        public void Load()
        {
            FormSerialize.Load(controls, QAQCDTOList);
            //chkOther.Clicked += chkOther_Click;
        }

        public void Save()
        {
            QAQCDTOList.Clear();
            //TODO : Group에 따라 작성
            FormSerialize.GenDTO(QAQCGroup.GROUP01, controls[0], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP02, controls[1], QAQCDTOList);
        }

        public void DoAfter(RevealProjectSvc.QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>>
            {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtEqDesc, txtTagNumber, txtTestPackNum },
                    new List<FrameworkElement> { rdoShop, rdoField }
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { nfcCon1, nfcMeg1 },
                    new List<FrameworkElement> { nfcCon2, nfcMeg2 },
                    new List<FrameworkElement> { nfcCon3, nfcMeg3, chkMech, chkDeter, chkAcid, chkOther, txtCom5 },
                    new List<FrameworkElement> { nfcCon4, nfcMeg4 },
                    new List<FrameworkElement> { nfcCon5, nfcMeg5, txtCom5 },
                    new List<FrameworkElement> { nfcCon6, nfcMeg6 },
                    new List<FrameworkElement> { nfcCon7, nfcMeg7 }
                }
            };
        }

        private void chkOther_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (!(bool)chkOther.IsChecked)
            {
                txtDCSpecify.Text = string.Empty;
                txtComOther.Enabled = false;
            }
            else
            {
                txtComOther.Enabled = true;
            }
            */
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
                if (!(bool)chkMech.IsChecked && !(bool)chkDeter.IsChecked && !(bool)chkAcid.IsChecked && !(bool)chkOther.IsChecked) checkdata = false;
                if (!(bool)rdoShop.IsChecked && !(bool)rdoField.IsChecked) checkdata = false;

                if (txtEqDesc.Text == "" || txtTagNumber.Text == "" || txtTestPackNum.Text == "") checkdata = false;
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
                    if (!(bool)chkMech.IsChecked && !(bool)chkDeter.IsChecked && !(bool)chkAcid.IsChecked && !(bool)chkOther.IsChecked) checkdata = false;
                    if (!(bool)rdoShop.IsChecked && !(bool)rdoField.IsChecked) checkdata = false;

                    if (txtEqDesc.Text == "" || txtTagNumber.Text == "" || txtTestPackNum.Text == "") checkdata = false;
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
