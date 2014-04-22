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
using C1.Xaml.DateTimeEditors;
using System.Threading.Tasks;
using Element.Reveal.Meg.RevealProjectSvc;

namespace Element.Reveal.Meg.Discipline.ITR
{
    public sealed partial class UCMIReceivingReport : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDTOList { get; set; }
        public List<QaqcformdetailDTO> UpdateGrid { get; set; }
        public UCMIReceivingReport()
        {
            this.InitializeComponent();
            UpdateGrid = new List<QaqcformdetailDTO>();
        }
        
        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtInspectionDate, txtInspectedBy, txtLocation, txtRow, txtShelf}
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtAT_SerialNo, txtAT_CalibrationDueDate, txtAT_ByWhom, txtAT_Date},
                    new List<FrameworkElement> { txtAG_SerialNo, txtAT_Date, txtAG_ByWhom, txtAG_Date}
                }
            };

            ProjectName.Text =  (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            ProjectNumber.Text =  (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            CWPName.Text =  (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            JobNumber.Text =  (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            foreach (QaqcformdetailDTO q in _dto.QaqcfromDetails.Where(x => x.InspectionLUID == QAQCGroup.Grid).ToList<QaqcformdetailDTO>())
                 UpdateGrid.Add(q);

            lvCableCatalogue.ItemsSource = UpdateGrid;
            lvCableCatalogue.PointerWheelChanged += lvCableCatalogue_PointerWheelChanged;

            this.txtInspectedBy.Text = Login.UserAccount.UserName;
        }

        void lvCableCatalogue_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            string s = ((QaqcformdetailDTO)lvCableCatalogue.Items[0]).StringValue1;

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
                    if (txtInspectedBy.Text == "" || txtLocation.Text == "" || txtRow.Text == "" || txtShelf.Text == "" || txtAT_SerialNo.Text == "" || txtAT_CalibrationDueDate.Text == "" || txtAG_SerialNo.Text == "" ||
                        txtAG_ByWhom.Text == "")
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
                if (txtInspectedBy.Text == "" || txtLocation.Text == "" || txtRow.Text == "" || txtShelf.Text == "" || txtAT_SerialNo.Text == "" || txtAT_CalibrationDueDate.Text == "" || txtAG_SerialNo.Text == "" ||
                    txtAG_ByWhom.Text == "")
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

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

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
