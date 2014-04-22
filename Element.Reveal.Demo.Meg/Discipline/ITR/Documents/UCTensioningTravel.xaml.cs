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
using Element.Reveal.Meg.Lib.ServiceModel;
using Element.Reveal.Meg.RevealProjectSvc;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCTensioningTravel : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }

        public UCTensioningTravel()
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
            FormSerialize.GenDTO(QAQCGroup.GROUP01, controls[0], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP02, controls[1], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP03, controls[2], QAQCDTOList);
            FormSerialize.GenDTO(QAQCGroup.GROUP04, controls[3], QAQCDTOList);
        }

        public void DoAfter(RevealProjectSvc.QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>>
            {
                new List<List<FrameworkElement>> {
                    new List<FrameworkElement> { txtTpn, txtBu },
                    new List<FrameworkElement> { rdoCbuYes, rdoCbuNo, chkHand, chkHydraulic, chkTension },
                    new List<FrameworkElement> { txtFtv, txt30tv, txt60tv },
                    new List<FrameworkElement> { txtTsn, txtTcDt, txtG2mr, txtG3hr }
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> {nfcT1},
                    new List<FrameworkElement> {nfcT2},
                    new List<FrameworkElement> {nfcT3},
                    new List<FrameworkElement> {nfcT4},
                    new List<FrameworkElement> {nfcT5},
                    new List<FrameworkElement> {nfcQ1},
                    new List<FrameworkElement> {nfcQ2},
                    new List<FrameworkElement> {nfcQ3},
                    new List<FrameworkElement> {nfcQ4},
                    new List<FrameworkElement> {nfcQ5},
                    new List<FrameworkElement> {nfcM1},
                    new List<FrameworkElement> {nfcM2},
                    new List<FrameworkElement> {nfcM3},
                    new List<FrameworkElement> {nfcM4},
                    new List<FrameworkElement> {nfcM5}
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtTnPn, txtTnI, txtTn1p, txtTn2p, txtTnfp, txtTnDt },
                    new List<FrameworkElement> { txtQnPn, txtQnI, txtQn1p, txtQn2p, txtQnfp, txtQnDt },
                    new List<FrameworkElement> { txtMnPn, txtMnI, txtMn1p, txtMn2p, txtMnfp, txtMnDt }
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtBlsl1, txtUisl1 },
                    new List<FrameworkElement> { txtBlsl2, txtUisl2 },
                    new List<FrameworkElement> { txtBlsl3, txtUisl3 },
                    new List<FrameworkElement> { txtBlsl4, txtUisl4 },
                    new List<FrameworkElement> { txtBlsl5, txtUisl5 },
                    new List<FrameworkElement> { txtBlsl6, txtUisl6 },
                    new List<FrameworkElement> { txtBlsl7, txtUisl7 },
                    new List<FrameworkElement> { txtBlsl8, txtUisl8 },
                    new List<FrameworkElement> { txtBlsl9, txtUisl9 },
                    new List<FrameworkElement> { txtBlsl10, txtUisl10 },
                    new List<FrameworkElement> { txtBlsl11, txtUisl11 },
                    new List<FrameworkElement> { txtBlsl12, txtUisl12 },
                    new List<FrameworkElement> { txtBlsl13, txtUisl13 },
                    new List<FrameworkElement> { txtBlsl14, txtUisl14 },
                    new List<FrameworkElement> { txtBlsl15, txtUisl15 },
                    new List<FrameworkElement> { txtBlsl16, txtUisl16 },
                    new List<FrameworkElement> { txtBlsl17, txtUisl17 },
                    new List<FrameworkElement> { txtBlsl18, txtUisl18 },
                    new List<FrameworkElement> { txtBlsl19, txtUisl19 },
                    new List<FrameworkElement> { txtBlsl20, txtUisl20 },
                    new List<FrameworkElement> { txtBlsl21, txtUisl21 },
                    new List<FrameworkElement> { txtBlsl22, txtUisl22 },
                    new List<FrameworkElement> { txtBlsl23, txtUisl23 },
                    new List<FrameworkElement> { txtBlsl24, txtUisl24 },
                    new List<FrameworkElement> { txtBlsl25, txtUisl25 },
                    new List<FrameworkElement> { txtBlsl26, txtUisl26 },
                    new List<FrameworkElement> { txtBlsl27, txtUisl27 },
                    new List<FrameworkElement> { txtBlsl28, txtUisl28 },
                    new List<FrameworkElement> { txtBlsl29, txtUisl29 },
                    new List<FrameworkElement> { txtBlsl30, txtUisl30 }
                }
            };
            QaqcformdetailDTO header = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Header select lv).FirstOrDefault<QaqcformdetailDTO>();
            txtSystemNumber.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            txtIsoDwgNoEquipNo.Text = (!string.IsNullOrEmpty(header.StringValue1)) ? header.StringValue1 : "";
            txtBu.Text = (!string.IsNullOrEmpty(header.StringValue2)) ? header.StringValue2 : "";
            txtNumberOfStuds.Text = (!string.IsNullOrEmpty(header.StringValue3)) ? header.StringValue3 : "";
            txtFlangeSizeNRating.Text = (!string.IsNullOrEmpty(header.StringValue4)) ? header.StringValue4 : "";
            txtFStudBoltSize.Text = (!string.IsNullOrEmpty(header.StringValue5)) ? header.StringValue5 : "";
            txtGaketType.Text = (!string.IsNullOrEmpty(header.StringValue6)) ? header.StringValue6 : "";
            txtJointNo.Text = (!string.IsNullOrEmpty(header.StringValue7)) ? header.StringValue7 : "";
            rdoCbuYes.IsChecked = (!string.IsNullOrEmpty(header.StringValue8)) ? (header.StringValue8 == "Y" ? true : false) : false;
            rdoCbuNo.IsChecked = (!string.IsNullOrEmpty(header.StringValue9)) ? (header.StringValue9 == "Y" ? true : false) : false;
            txtArea.Text = (!string.IsNullOrEmpty(header.StringValue10)) ? header.StringValue10 : "";
            txtUnitNo.Text = (!string.IsNullOrEmpty(header.StringValue11)) ? header.StringValue11 : "";
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
            return true;
            bool checkdata = true;
            try
            {
                if (txtTpn.Text.Trim() == "") checkdata = false;
                if (txtBu.Text.Trim() == "") checkdata = false;
                if (!(bool)rdoCbuYes.IsChecked && !(bool)rdoCbuNo.IsChecked) checkdata = false;
                if (!(bool)chkHand.IsChecked && !(bool)chkHydraulic.IsChecked && !(bool)chkTension.IsChecked) checkdata = false;
                if (txtFtv.Text.Trim() == "") checkdata = false;
                if (txt30tv.Text.Trim() == "") checkdata = false;
                if (txt60tv.Text.Trim() == "") checkdata = false;
                if (txtTsn.Text.Trim() == "") checkdata = false;
                if (txtTcDt.Text.Trim() == "") checkdata = false;
                if (txtTnPn.Text.Trim() == "") checkdata = false;
                if (txtTnI.Text.Trim() == "") checkdata = false;
                if (txtTn1p.Text.Trim() == "") checkdata = false;
                if (txtTn2p.Text.Trim() == "") checkdata = false;
                if (txtTnfp.Text.Trim() == "") checkdata = false;
                if (txtTnDt.Text.Trim() == "") checkdata = false;
                if (txtQnPn.Text.Trim() == "") checkdata = false;
                if (txtQnI.Text.Trim() == "") checkdata = false;
                if (txtQn1p.Text.Trim() == "") checkdata = false;
                if (txtQn2p.Text.Trim() == "") checkdata = false;
                if (txtQnfp.Text.Trim() == "") checkdata = false;
                if (txtQnDt.Text.Trim() == "") checkdata = false;
                if (txtMnPn.Text.Trim() == "") checkdata = false;
                if (txtMnI.Text.Trim() == "") checkdata = false;
                if (txtMn1p.Text.Trim() == "") checkdata = false;
                if (txtMn2p.Text.Trim() == "") checkdata = false;
                if (txtMnfp.Text.Trim() == "") checkdata = false;
                if (txtMnDt.Text.Trim() == "") checkdata = false;
                if (txtBlsl1.Text.Trim() == "") checkdata = false;
                if (txtUisl1.Text.Trim() == "") checkdata = false;
                if (txtBlsl2.Text.Trim() == "") checkdata = false;
                if (txtUisl2.Text.Trim() == "") checkdata = false;
                if (txtBlsl3.Text.Trim() == "") checkdata = false;
                if (txtUisl3.Text.Trim() == "") checkdata = false;
                if (txtBlsl4.Text.Trim() == "") checkdata = false;
                if (txtUisl4.Text.Trim() == "") checkdata = false;
                if (txtBlsl5.Text.Trim() == "") checkdata = false;
                if (txtUisl5.Text.Trim() == "") checkdata = false;
                if (txtBlsl6.Text.Trim() == "") checkdata = false;
                if (txtUisl6.Text.Trim() == "") checkdata = false;
                if (txtBlsl7.Text.Trim() == "") checkdata = false;
                if (txtUisl7.Text.Trim() == "") checkdata = false;
                if (txtBlsl8.Text.Trim() == "") checkdata = false;
                if (txtUisl8.Text.Trim() == "") checkdata = false;
                if (txtBlsl9.Text.Trim() == "") checkdata = false;
                if (txtUisl9.Text.Trim() == "") checkdata = false;
                if (txtBlsl10.Text.Trim() == "") checkdata = false;
                if (txtUisl10.Text.Trim() == "") checkdata = false;
                if (txtBlsl11.Text.Trim() == "") checkdata = false;
                if (txtUisl11.Text.Trim() == "") checkdata = false;
                if (txtBlsl12.Text.Trim() == "") checkdata = false;
                if (txtUisl12.Text.Trim() == "") checkdata = false;
                if (txtBlsl13.Text.Trim() == "") checkdata = false;
                if (txtUisl13.Text.Trim() == "") checkdata = false;
                if (txtBlsl14.Text.Trim() == "") checkdata = false;
                if (txtUisl14.Text.Trim() == "") checkdata = false;
                if (txtBlsl15.Text.Trim() == "") checkdata = false;
                if (txtUisl15.Text.Trim() == "") checkdata = false;
                if (txtBlsl16.Text.Trim() == "") checkdata = false;
                if (txtUisl16.Text.Trim() == "") checkdata = false;
                if (txtBlsl17.Text.Trim() == "") checkdata = false;
                if (txtUisl17.Text.Trim() == "") checkdata = false;
                if (txtBlsl18.Text.Trim() == "") checkdata = false;
                if (txtUisl18.Text.Trim() == "") checkdata = false;
                if (txtBlsl19.Text.Trim() == "") checkdata = false;
                if (txtUisl19.Text.Trim() == "") checkdata = false;
                if (txtBlsl20.Text.Trim() == "") checkdata = false;
                if (txtUisl20.Text.Trim() == "") checkdata = false;
                if (txtBlsl21.Text.Trim() == "") checkdata = false;
                if (txtUisl21.Text.Trim() == "") checkdata = false;
                if (txtBlsl22.Text.Trim() == "") checkdata = false;
                if (txtUisl22.Text.Trim() == "") checkdata = false;
                if (txtBlsl23.Text.Trim() == "") checkdata = false;
                if (txtUisl23.Text.Trim() == "") checkdata = false;
                if (txtBlsl24.Text.Trim() == "") checkdata = false;
                if (txtUisl24.Text.Trim() == "") checkdata = false;
                if (txtBlsl25.Text.Trim() == "") checkdata = false;
                if (txtUisl25.Text.Trim() == "") checkdata = false;
                if (txtBlsl26.Text.Trim() == "") checkdata = false;
                if (txtUisl26.Text.Trim() == "") checkdata = false;
                if (txtBlsl27.Text.Trim() == "") checkdata = false;
                if (txtUisl27.Text.Trim() == "") checkdata = false;
                if (txtBlsl28.Text.Trim() == "") checkdata = false;
                if (txtUisl28.Text.Trim() == "") checkdata = false;
                if (txtBlsl29.Text.Trim() == "") checkdata = false;
                if (txtUisl29.Text.Trim() == "") checkdata = false;
                if (txtBlsl30.Text.Trim() == "") checkdata = false;
                if (txtUisl30.Text.Trim() == "") checkdata = false;
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
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (txtTpn.Text.Trim() == "") checkdata = false;
                    if (txtBu.Text.Trim() == "") checkdata = false;
                    if (!(bool)rdoCbuYes.IsChecked && !(bool)rdoCbuNo.IsChecked) checkdata = false;
                    if (!(bool)chkHand.IsChecked && !(bool)chkHydraulic.IsChecked && !(bool)chkTension.IsChecked) checkdata = false;
                    if (txtFtv.Text.Trim() == "") checkdata = false;
                    if (txt30tv.Text.Trim() == "") checkdata = false;
                    if (txt60tv.Text.Trim() == "") checkdata = false;
                    if (txtTsn.Text.Trim() == "") checkdata = false;
                    if (txtTcDt.Text.Trim() == "") checkdata = false;
                    if (txtTnPn.Text.Trim() == "") checkdata = false;
                    if (txtTnI.Text.Trim() == "") checkdata = false;
                    if (txtTn1p.Text.Trim() == "") checkdata = false;
                    if (txtTn2p.Text.Trim() == "") checkdata = false;
                    if (txtTnfp.Text.Trim() == "") checkdata = false;
                    if (txtTnDt.Text.Trim() == "") checkdata = false;
                    if (txtQnPn.Text.Trim() == "") checkdata = false;
                    if (txtQnI.Text.Trim() == "") checkdata = false;
                    if (txtQn1p.Text.Trim() == "") checkdata = false;
                    if (txtQn2p.Text.Trim() == "") checkdata = false;
                    if (txtQnfp.Text.Trim() == "") checkdata = false;
                    if (txtQnDt.Text.Trim() == "") checkdata = false;
                    if (txtMnPn.Text.Trim() == "") checkdata = false;
                    if (txtMnI.Text.Trim() == "") checkdata = false;
                    if (txtMn1p.Text.Trim() == "") checkdata = false;
                    if (txtMn2p.Text.Trim() == "") checkdata = false;
                    if (txtMnfp.Text.Trim() == "") checkdata = false;
                    if (txtMnDt.Text.Trim() == "") checkdata = false;
                    if (txtBlsl1.Text.Trim() == "") checkdata = false;
                    if (txtUisl1.Text.Trim() == "") checkdata = false;
                    if (txtBlsl2.Text.Trim() == "") checkdata = false;
                    if (txtUisl2.Text.Trim() == "") checkdata = false;
                    if (txtBlsl3.Text.Trim() == "") checkdata = false;
                    if (txtUisl3.Text.Trim() == "") checkdata = false;
                    if (txtBlsl4.Text.Trim() == "") checkdata = false;
                    if (txtUisl4.Text.Trim() == "") checkdata = false;
                    if (txtBlsl5.Text.Trim() == "") checkdata = false;
                    if (txtUisl5.Text.Trim() == "") checkdata = false;
                    if (txtBlsl6.Text.Trim() == "") checkdata = false;
                    if (txtUisl6.Text.Trim() == "") checkdata = false;
                    if (txtBlsl7.Text.Trim() == "") checkdata = false;
                    if (txtUisl7.Text.Trim() == "") checkdata = false;
                    if (txtBlsl8.Text.Trim() == "") checkdata = false;
                    if (txtUisl8.Text.Trim() == "") checkdata = false;
                    if (txtBlsl9.Text.Trim() == "") checkdata = false;
                    if (txtUisl9.Text.Trim() == "") checkdata = false;
                    if (txtBlsl10.Text.Trim() == "") checkdata = false;
                    if (txtUisl10.Text.Trim() == "") checkdata = false;
                    if (txtBlsl11.Text.Trim() == "") checkdata = false;
                    if (txtUisl11.Text.Trim() == "") checkdata = false;
                    if (txtBlsl12.Text.Trim() == "") checkdata = false;
                    if (txtUisl12.Text.Trim() == "") checkdata = false;
                    if (txtBlsl13.Text.Trim() == "") checkdata = false;
                    if (txtUisl13.Text.Trim() == "") checkdata = false;
                    if (txtBlsl14.Text.Trim() == "") checkdata = false;
                    if (txtUisl14.Text.Trim() == "") checkdata = false;
                    if (txtBlsl15.Text.Trim() == "") checkdata = false;
                    if (txtUisl15.Text.Trim() == "") checkdata = false;
                    if (txtBlsl16.Text.Trim() == "") checkdata = false;
                    if (txtUisl16.Text.Trim() == "") checkdata = false;
                    if (txtBlsl17.Text.Trim() == "") checkdata = false;
                    if (txtUisl17.Text.Trim() == "") checkdata = false;
                    if (txtBlsl18.Text.Trim() == "") checkdata = false;
                    if (txtUisl18.Text.Trim() == "") checkdata = false;
                    if (txtBlsl19.Text.Trim() == "") checkdata = false;
                    if (txtUisl19.Text.Trim() == "") checkdata = false;
                    if (txtBlsl20.Text.Trim() == "") checkdata = false;
                    if (txtUisl20.Text.Trim() == "") checkdata = false;
                    if (txtBlsl21.Text.Trim() == "") checkdata = false;
                    if (txtUisl21.Text.Trim() == "") checkdata = false;
                    if (txtBlsl22.Text.Trim() == "") checkdata = false;
                    if (txtUisl22.Text.Trim() == "") checkdata = false;
                    if (txtBlsl23.Text.Trim() == "") checkdata = false;
                    if (txtUisl23.Text.Trim() == "") checkdata = false;
                    if (txtBlsl24.Text.Trim() == "") checkdata = false;
                    if (txtUisl24.Text.Trim() == "") checkdata = false;
                    if (txtBlsl25.Text.Trim() == "") checkdata = false;
                    if (txtUisl25.Text.Trim() == "") checkdata = false;
                    if (txtBlsl26.Text.Trim() == "") checkdata = false;
                    if (txtUisl26.Text.Trim() == "") checkdata = false;
                    if (txtBlsl27.Text.Trim() == "") checkdata = false;
                    if (txtUisl27.Text.Trim() == "") checkdata = false;
                    if (txtBlsl28.Text.Trim() == "") checkdata = false;
                    if (txtUisl28.Text.Trim() == "") checkdata = false;
                    if (txtBlsl29.Text.Trim() == "") checkdata = false;
                    if (txtUisl29.Text.Trim() == "") checkdata = false;
                    if (txtBlsl30.Text.Trim() == "") checkdata = false;
                    if (txtUisl30.Text.Trim() == "") checkdata = false;
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
