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
using Element.Reveal.Crew.Lib;
using C1.Xaml.DateTimeEditors;
using System.Threading.Tasks;
using Element.Reveal.Crew.RevealProjectSvc;

namespace Element.Reveal.Crew.Discipline.ITR
{
    public sealed partial class UCSR_PLReceivingReport : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }
        public UCSR_PLReceivingReport()
        {
            this.InitializeComponent();
            QAQCDTOList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        }
        
        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { dtpInspectionDate, txtInpectedBy, txtLocation, txtRow, txtShelf}
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtManufacturer, txtPowerOutputRating, txtHeatingCableFamily, txtVoltage, txtOuterJacket,
                        txtTemperaturRating,txtMEGAOHMS500VDC ,txtMEGAOHMS1000VDC,txtMEGAOHMS2500VDC,
                        txtSerialNo, dtpCalibrationDueDate, txtTestInfoByWhom, dtpTestInfoDate
                    }
                }
            };
            //Header Binding
            QaqcformdetailDTO HeaderDto = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Header).FirstOrDefault();

            ProjectName.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            ProjectNumber.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            CWPName.Text =  (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            JobNumber.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            txtReelNo.Text =  (!string.IsNullOrEmpty(HeaderDto.StringValue1)) ? HeaderDto.StringValue1 : "";

            List<QaqcformdetailDTO> grid = _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Grid).ToList<QaqcformdetailDTO>();
            lvList.ItemsSource = grid;

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
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (txtInpectedBy.Text == "" || txtLocation.Text == "" || txtRow.Text == "" || txtShelf.Text == "" || txtManufacturer.Text == "" || txtPowerOutputRating.Text == "" ||
                        txtHeatingCableFamily.Text == "" || txtVoltage.Text == "" || txtOuterJacket.Text == "" || txtTemperaturRating.Text == "" || txtMEGAOHMS1000VDC.Text == "" || txtMEGAOHMS2500VDC.Text == "" ||
                        txtMEGAOHMS500VDC.Text == "" || txtSerialNo.Text == "" || txtTestInfoByWhom.Text == "")
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
                if (txtInpectedBy.Text == "" || txtLocation.Text == "" || txtRow.Text == "" || txtShelf.Text == "" || txtManufacturer.Text == "" || txtPowerOutputRating.Text == "" ||
                    txtHeatingCableFamily.Text == "" || txtVoltage.Text == "" || txtOuterJacket.Text == "" || txtTemperaturRating.Text == "" || txtMEGAOHMS1000VDC.Text == "" || txtMEGAOHMS2500VDC.Text == "" ||
                    txtMEGAOHMS500VDC.Text == "" || txtSerialNo.Text == "" || txtTestInfoByWhom.Text == "")
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
