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

using WinAppLibrary.ServiceModels;
using System.Threading.Tasks;
using Element.Reveal.Meg.Lib;
using Element.Reveal.Meg.Discipline.ITR;
using Element.Reveal.Meg.RevealProjectSvc;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Element.Reveal.Meg.Discipline.PunchCard
{
    public sealed partial class PunchTicket : UserControl, PunchCardDoc
    {
        private PunchDoc Doc;
        public List<RevealProjectSvc.QaqcformdetailDTO> QAQCDetailDTOList { get; set; }
        public PunchTicket()
        {
            this.InitializeComponent();
        }

        public void DoAfter(PunchDTOSet _dto)
        {
            try
            {
                this.lblProjectName.Text = (!string.IsNullOrEmpty(_dto.turnoverDTOS[0].ProjectName)) ? _dto.turnoverDTOS[0].ProjectName : "";
                this.lblOriginator.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue1)) ? _dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue1 : "";
                this.lblContractor.Text = (!string.IsNullOrEmpty(_dto.turnoverDTOS[0].ContractorName)) ? _dto.turnoverDTOS[0].ContractorName : "";
                this.lblDiscipline.Text = "";//!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].ModuleName)) ? _dto.qaqcformDTOS[0].ModuleName : "";
                this.lblPunchTicketNumber.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].KeyValue)) ? _dto.qaqcformDTOS[0].KeyValue : "";
                this.lblCategory.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue8)) ? _dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue8 : "";
                this.lblStatus.Text = (_dto.qaqcformDTOS[0].IsSubmitted == 0) ? "Incomplete" : "Complete";
                this.lblIWPNumber.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].FIWPName)) ? _dto.qaqcformDTOS[0].FIWPName : "";
                this.lblSystemName.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].SystemName)) ? _dto.qaqcformDTOS[0].SystemName : "";
                this.lblSystemNumber.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].SystemNumber)) ? _dto.qaqcformDTOS[0].SystemNumber : "";
                this.lblTagNumber.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue4)) ? _dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue4 : "";

                this.txtComments.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue13)) ? _dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue13 : "";
                this.txtDescription.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue12)) ? _dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue12 : "";
                this.txtLessons.Text = (!string.IsNullOrEmpty(_dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue14)) ? _dto.qaqcformDTOS[0].QaqcfromDetails[0].StringValue14 : "";

                //Drawing 정보
                LoadThumbnail(_dto.qaqcformDTOS[0].QaqcrefDrawing);

                QAQCDetailDTOList = _dto.qaqcformDTOS[0].QaqcfromDetails;

                //서브밋 상태면 수정 불가
                //if(Doc.Status == EStatusType.ReadyToSubmit)
            }
            catch (Exception ex)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please check punch card data", "Error!");
            }

        }


        public void Load()
        {
            //txtDescription, txtComments, txtLessons는 QaqcformDetailDTO
            //FormSerialize.Load(controls, QAQCDetailDTOList);
        }

        private async void LoadThumbnail(List<DrawingDTO> _drwDTO)
        {
            try
            {
                //드로잉 테스트 데이터
                //for (int i = 0; i < 30; i++)
                //{
                //    DrawingDTO testDto = new DrawingDTO();
                //    testDto.DrawingFilePath = "http://reveal.elementindustrial.com/Element.Reveal.Server.WS/IFC_Images/";
                //    testDto.DrawingFileURL = "PL21-P-8391-1%20REV%201B.jpg";
                //    testDto.DrawingName = "PL21-P-8391-1%20REV%201B";
                //    _drwDTO.Add(testDto);
                //}

                for (int i = 0; i < _drwDTO.Count; i++)
                {
                    _drwDTO[i].DrawingFileURL = _drwDTO[i].DrawingFilePath + _drwDTO[i].DrawingFileURL;
                }

                gvDrawing.ItemsSource = _drwDTO;
            }
            catch (Exception ex)
            {

            }
        }

        //썸네일 클릭 시 이미지 경로 변경(뷰어 기능 확인)
        private void gvDrawing_ItemClick(object sender, ItemClickEventArgs e)
        {
            string url = ((RevealProjectSvc.DrawingDTO)e.ClickedItem).DrawingFileURL.ToString();
            imgDrawing.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(url));
        }

        public void Save()
        {
            try
            {

                QAQCDetailDTOList[0].StringValue12 = this.txtDescription.Text;
                QAQCDetailDTOList[0].StringValue13 = this.txtComments.Text;
                QAQCDetailDTOList[0].StringValue14 = this.txtLessons.Text;
                QAQCDetailDTOList[0].DateValue1 = DateTime.Now;
                QAQCDetailDTOList[0].DateValue2 = DateTime.Now;
                QAQCDetailDTOList[0].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;

            }
            catch (Exception ex)
            {
                //WinAppLibrary.Utilities.Helper.SimpleMessage("Please check data", "Alert!");
            }
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
                    if (txtDescription.Text == "" || txtComments.Text == "" || txtLessons.Text == "")
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
                if (txtDescription.Text == "" || txtComments.Text == "" || txtLessons.Text == "")
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
