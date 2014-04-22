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
using Element.Reveal.Crew.Lib;
using Windows.UI.Core;
using System.Threading.Tasks;
using Element.Reveal.Crew.RevealProjectSvc; 

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Element.Reveal.Crew.Discipline.ITR
{
    public sealed partial class UCControlCableInspectionExhibit : UserControl, IItrDoc
    {
        private List<List<List<FrameworkElement>>> controls;

        public UCControlCableInspectionExhibit()
        {
            this.InitializeComponent();                                 
        }

        public void DoAfter(QaqcformDTO _dto)
        {
            controls = new List<List<List<FrameworkElement>>> {
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { dtpInspectionDate, txtInspectedBy } 
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> { txtD_InitialMTRMark, txtD_EndMTRMark, txtD_ActualLength, txtD_Print, txtD_Sign, txtD_Date}                    
                },
                new List<List<FrameworkElement>>{
                    new List<FrameworkElement> {txtGT_From, txtGT_To, txtGT_PrintName, txtGT_Date},
                    new List<FrameworkElement> {txtGS_From, txtGS_To, txtGS_PrintName, txtGS_Date},                    
                    new List<FrameworkElement> {txtGC_From, txtGC_To, txtGC_PrintName, txtGC_Date},
                    new List<FrameworkElement> {txtP_COND, txtP_GRND, txtP_Comment},
                    new List<FrameworkElement> {txtTestEquipType, txtTestSerial, txtTestCalibrationDueDate, txtTestByWhom, txtTestDate}
                },
                new List<List<FrameworkElement>> { 
                    new List<FrameworkElement> {txtTT_From, txtTT_To, txtTT_PrintName, txtTT_Date},
                    new List<FrameworkElement> {txtTS_From, txtTS_To, txtTS_PrintName, txtTS_Date},                    
                    new List<FrameworkElement> {txtTC_From, txtTC_To, txtTC_PrintName, txtTC_Date}                   
                },
            };
            QaqcformdetailDTO header = (from lv in _dto.QaqcfromDetails where lv.InspectionLUID == QAQCGroup.Header select lv).FirstOrDefault<QaqcformdetailDTO>();
            ProjectName.Text = (!string.IsNullOrEmpty(_dto.ProjectName)) ? _dto.ProjectName : "";
            ProjectNumber.Text = (!string.IsNullOrEmpty(_dto.ProjectNumber)) ? _dto.ProjectNumber : "";
            CWPName.Text = (!string.IsNullOrEmpty(_dto.CWPName)) ? _dto.CWPName : "";
            JobNumber.Text = (!string.IsNullOrEmpty(_dto.JobNumber)) ? _dto.JobNumber : "";
            //lblCableSCHEDNo.Text = (!string.IsNullOrEmpty(header.StringValue1)) ? header.StringValue1 : "";
            //lblCableSCHEDrev.Text = (!string.IsNullOrEmpty(header.StringValue2)) ? header.StringValue2 : "";

            //lblRFIDRWGNo.Text = (!string.IsNullOrEmpty(header.StringValue3)) ? header.StringValue3 : "";
            //lblRFIDRWGrev.Text = (!string.IsNullOrEmpty(header.StringValue4)) ? header.StringValue4 : "";
            //lblRFIDRWGNo.Text = (!string.IsNullOrEmpty(header.StringValue5)) ? header.StringValue5 : "";
            //lblRFIDRWGrev.Text = (!string.IsNullOrEmpty(header.StringValue6)) ? header.StringValue6 : "";
            //DrawingName2.Text = (!string.IsNullOrEmpty(header.StringValue7)) ? header.StringValue7 : "";
            //lblToLocationDRWGrev.Text = (!string.IsNullOrEmpty(header.StringValue8)) ? header.StringValue8 : "";

            FIWPName.Text = (!string.IsNullOrEmpty(_dto.FIWPName)) ? _dto.FIWPName : "";
            SystemNumber.Text = (!string.IsNullOrEmpty(_dto.SystemNumber)) ? _dto.SystemNumber : "";
            SystemName.Text = (!string.IsNullOrEmpty(_dto.SystemName)) ? _dto.SystemName : "";
            cbCableTagNo.Text = (!string.IsNullOrEmpty(header.StringValue9)) ? header.StringValue9 : "";
            lblEstLength.Text = (!string.IsNullOrEmpty(header.StringValue10)) ? header.StringValue10 : "";
            lblReelNo.Text = (!string.IsNullOrEmpty(header.StringValue11)) ? header.StringValue11 : "";
            lblCableType.Text = (!string.IsNullOrEmpty(header.StringValue12)) ? header.StringValue12 : "";
            //lbl cbCableTagNo.Text = header.StringValue13; - shield
            lblVoltageRating.Text = (!string.IsNullOrEmpty(header.StringValue14)) ? header.StringValue14 : "";
            lblConductors.Text = (!string.IsNullOrEmpty(header.StringValue15)) ? header.StringValue15 : "";
            //cbCableTagNo.Text = header.StringValue16; - insul
            lblCableSizeAWG.Text = (!string.IsNullOrEmpty(header.StringValue17)) ? header.StringValue17 : "";

            this.txtInspectedBy.Text = Login.UserAccount.UserName;

            //Drawing
            List<DrawingDTO> drawingDto = _dto.QaqcrefDrawing;
            lvDrawing.ItemsSource = drawingDto;
        }
        public bool isValidate { get; set; }
        public async void checkValidate()
        {
            isValidate = await Validate2();
        }
        //bool IItrDoc.Validate()
        public bool Validate()
        {
            try
            {
                if (dtpInspectionDate.Text.Trim() == "") return false;
                if (txtInspectedBy.Text.Trim() == "") return false;
                if (txtD_InitialMTRMark.Text.Trim() == "") return false;
                if (txtD_EndMTRMark.Text.Trim() == "") return false;
                if (txtD_ActualLength.Text.Trim() == "") return false;
                if (txtD_Print.Text.Trim() == "") return false;
                if (txtD_Sign.Text.Trim() == "") return false;
                if (txtGT_From.Text.Trim() == "") return false;
                if (txtGT_To.Text.Trim() == "") return false;
                if (txtGT_PrintName.Text.Trim() == "") return false;
                if (txtGS_From.Text.Trim() == "") return false;
                if (txtGS_To.Text.Trim() == "") return false;
                if (txtGS_PrintName.Text.Trim() == "") return false;
                if (txtGC_From.Text.Trim() == "") return false;
                if (txtGC_To.Text.Trim() == "") return false;
                if (txtGC_PrintName.Text.Trim() == "") return false;
                if (txtP_COND.Text.Trim() == "") return false;
                if (txtP_GRND.Text.Trim() == "") return false;
                if (txtP_Comment.Text.Trim() == "") return false;
                if (txtTestEquipType.Text.Trim() == "") return false;
                if (txtTestSerial.Text.Trim() == "") return false;
                if (txtTestCalibrationDueDate.Text.Trim() == "") return false;
                if (txtTestByWhom.Text.Trim() == "") return false;
                if (txtTT_From.Text.Trim() == "") return false;
                if (txtTT_To.Text.Trim() == "") return false;
                if (txtTT_PrintName.Text.Trim() == "") return false;
                if (txtTS_From.Text.Trim() == "") return false;
                if (txtTS_To.Text.Trim() == "") return false;
                if (txtTS_PrintName.Text.Trim() == "") return false;
                if (txtTC_From.Text.Trim() == "") return false;
                if (txtTC_To.Text.Trim() == "") return false;
                if (txtTC_PrintName.Text.Trim() == "") return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Validate2()
        {
            bool checkdata = true;
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (dtpInspectionDate.Text.Trim() == "") checkdata = false;
                    if (txtInspectedBy.Text.Trim() == "") checkdata = false;
                    if (txtD_InitialMTRMark.Text.Trim() == "") checkdata = false;
                    if (txtD_EndMTRMark.Text.Trim() == "") checkdata = false;
                    if (txtD_ActualLength.Text.Trim() == "") checkdata = false;
                    if (txtD_Print.Text.Trim() == "") checkdata = false;
                    if (txtD_Sign.Text.Trim() == "") checkdata = false;
                    if (txtGT_From.Text.Trim() == "") checkdata = false;
                    if (txtGT_To.Text.Trim() == "") checkdata = false;
                    if (txtGT_PrintName.Text.Trim() == "") checkdata = false;
                    if (txtGS_From.Text.Trim() == "") checkdata = false;
                    if (txtGS_To.Text.Trim() == "") checkdata = false;
                    if (txtGS_PrintName.Text.Trim() == "") checkdata = false;
                    if (txtGC_From.Text.Trim() == "") checkdata = false;
                    if (txtGC_To.Text.Trim() == "") checkdata = false;
                    if (txtGC_PrintName.Text.Trim() == "") checkdata = false;
                    if (txtP_COND.Text.Trim() == "") checkdata = false;
                    if (txtP_GRND.Text.Trim() == "") checkdata = false;
                    if (txtP_Comment.Text.Trim() == "") checkdata = false;
                    if (txtTestEquipType.Text.Trim() == "") checkdata = false;
                    if (txtTestSerial.Text.Trim() == "") checkdata = false;
                    if (txtTestCalibrationDueDate.Text.Trim() == "") checkdata = false;
                    if (txtTestByWhom.Text.Trim() == "") checkdata = false;
                    if (txtTT_From.Text.Trim() == "") checkdata = false;
                    if (txtTT_To.Text.Trim() == "") checkdata = false;
                    if (txtTT_PrintName.Text.Trim() == "") checkdata = false;
                    if (txtTS_From.Text.Trim() == "") checkdata = false;
                    if (txtTS_To.Text.Trim() == "") checkdata = false;
                    if (txtTS_PrintName.Text.Trim() == "") checkdata = false;
                    if (txtTC_From.Text.Trim() == "") checkdata = false;
                    if (txtTC_To.Text.Trim() == "") checkdata = false;
                    if (txtTC_PrintName.Text.Trim() == "") checkdata = false;
                });
                
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
            List<RevealProjectSvc.QaqcformdetailDTO> tmp = new List<RevealProjectSvc.QaqcformdetailDTO>();
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
