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

using Element.Reveal.Meg.RevealCommonSvc;
using Element.Reveal.Meg.RevealProjectSvc;
using Element.Reveal.Meg.Lib.DataSource;
using WinAppLibrary.Utilities;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.PunchCard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssignmentPunch : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid, _personnelid;
        private int _qaqcformDetailID;
        private string _foremanName = "";
        private string  _foremanID = "";

        Lib.DataSource.QaqcDataSource _punchList = new Lib.DataSource.QaqcDataSource();
        private PunchDoc Doc;

        RevealProjectSvc.WalkdownDTOSet _wdDTO = new RevealProjectSvc.WalkdownDTOSet();
        List<RevealProjectSvc.QaqcformdetailDTO> _qaqcformDetailDTO = new List<RevealProjectSvc.QaqcformdetailDTO>();
        List<RevealCommonSvc.ComboBoxDTO> lstForemanList = new List<RevealCommonSvc.ComboBoxDTO>();
        
        public AssignmentPunch()
        {
            this.InitializeComponent();
            
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            _personnelid = Login.UserAccount.PersonnelID;

            BindList();
        }

        private async void BindList()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                //if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                //{
                    //personnelId, departmentId 확인 필요(현재는 0으로 넘김, 다 조회)
                await _punchList.GetPunchListByPersonnelDepartment(_projectid, _moduleid, 0, 0);
                _qaqcformDetailDTO = _punchList.GetPunchListByPersonnelDepartment();
                lvPunchList.ItemsSource = _qaqcformDetailDTO;

                _wdDTO.qaqcformdetailDTOS = _qaqcformDetailDTO;
                
                //}
                
                //foreman popup
                lstForemanList = await (new Lib.ServiceModel.CommonModel()).GetForemanGeneralForemanNameByPersonnelDepartment_Combo(Login.UserAccount.PersonnelID, Department.Foreman, Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);
                
                //스크롤 테스트
                //RevealCommonSvc.ComboBoxDTO testList = new RevealCommonSvc.ComboBoxDTO();
                //for (int i = 0; i < 30; i++)
                //{
                //    testList.DataName = "1";
                //    testList.ExtraValue1 = "1";
                //    lstForemanList.Add(testList);
                //}

                lvForeman.ItemsSource = lstForemanList;
            }
            catch (Exception e)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Cannot get Punch List", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(DownloadPunchList));
            Submit();
        }
        

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        ////리스트에서 foreman선택 버튼 클릭 시 해당 row 저장
        //private void lvPunchList_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    //_rowIndex = lvPunchList.SelectedIndex;
        //}

        //private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    //Foreman지정 팝업
        //    SettingForemanPopup.IsOpen = true;

        //    //e.Key
        //}

        //foreman 텍스트박스 클릭 시 팝업
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource as TextBox;
            QaqcformdetailDTO dto = source.DataContext as QaqcformdetailDTO;
            _qaqcformDetailID = dto.QAQCFormDetailID;
            SettingForemanPopup.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SettingForemanPopupBD.Visibility = Windows.UI.Xaml.Visibility.Visible;
            
            //Login.MasterPage.Loading(true, this);
        }

        //팝업에서 foreman 선택
        private void lvForeman_ItemClick(object sender, ItemClickEventArgs e)
        {
            _foremanName = ((RevealCommonSvc.ComboBoxDTO)e.ClickedItem).DataName;
            _foremanID = ((RevealCommonSvc.ComboBoxDTO)e.ClickedItem).DataID.ToString();
        }
        
        //선택한 foreman으로 저장
        private void btnAssign_Click(object sender, RoutedEventArgs e)
        {
            if(_foremanID != "")
            {
                Submit();
                SettingForemanPopup.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SettingForemanPopupBD.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                _foremanName = "";
                _foremanID = "";
            }
            else{
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please select foreman", "Caution!");
            }
            //Login.MasterPage.Loading(false, this);
        }

        //팝업 닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            SettingForemanPopup.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            SettingForemanPopupBD.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _foremanName = "";
            _foremanID = "";
            //Login.MasterPage.Loading(false, this);
        }

        private async void Submit()
        {
            bool saveresult = false;

            //팝업에서 foreman지정 후 저장
            
            List<RevealProjectSvc.QaqcformDTO> qaqcform = new List<RevealProjectSvc.QaqcformDTO>();
            //선택된 행의 qaqcformdetailDTO
            RevealProjectSvc.QaqcformdetailDTO dto = new RevealProjectSvc.QaqcformdetailDTO();
            for (int i = 0; i < lvPunchList.Items.Count; i++)
            {
                dto = (RevealProjectSvc.QaqcformdetailDTO)lvPunchList.Items[i];
                if (dto.QAQCFormDetailID == _qaqcformDetailID)
                {
                    _wdDTO.qaqcformdetailDTOS[i].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                    _wdDTO.qaqcformdetailDTOS[i].StringValue9 = _foremanID.ToString();
                    _wdDTO.qaqcformdetailDTOS[i].StringValue10 = _foremanName;
                    _wdDTO.qaqcformdetailDTOS[i].DateValue1 = DateTime.Now;
                    _wdDTO.qaqcformdetailDTOS[i].DateValue2 = DateTime.Now;
                }
                else
                {
                    _wdDTO.qaqcformdetailDTOS[i].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                }
            }

            //선택된 행의 qaqcformdetailDTO정보로 qaqcform 가져옴 
            await _punchList.GetPunchTicketByQaqcform(Convert.ToInt32(dto.QAQCFormID));
            qaqcform = _punchList.GetPunchTicketByQaqcform().qaqcformDTOS;
            
            //저장할 qaqcformDTO의 DTOStatus만 변경 ==> 변경 내용 없음?
            qaqcform[0].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
            _wdDTO.qaqcformDTOS = qaqcform;
            _wdDTO.qaqcformDTOS[0].QaqcfromDetails = _wdDTO.qaqcformdetailDTOS;


            saveresult = await _punchList.SaveQaqcformWithSharePoint(_wdDTO);

            if (saveresult)
                WinAppLibrary.Utilities.Helper.SimpleMessage("Save Completed", "Saved");
            else
                WinAppLibrary.Utilities.Helper.SimpleMessage("Save Failed", "Failed");




            /*
            List<RevealProjectSvc.QaqcformDTO> qaqcform = new List<RevealProjectSvc.QaqcformDTO>();
            for (int i = 0; i < lvPunchList.Items.Count; i++)
            {
                RevealProjectSvc.QaqcformdetailDTO dto = (RevealProjectSvc.QaqcformdetailDTO)lvPunchList.Items[i];

                await _punchList.GetPunchTicketByQaqcform(Convert.ToInt32(dto.QAQCFormID));
                qaqcform = _punchList.GetPunchTicketByQaqcform().qaqcformDTOS;

                if (!string.IsNullOrEmpty(dto.StringValue16))
                {
                    _wdDTO.qaqcformdetailDTOS[i].StringValue16 = dto.StringValue16.ToString();
                    qaqcform[0].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                    _wdDTO.qaqcformdetailDTOS[i].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                }
                else
                {
                    qaqcform[0].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                    _wdDTO.qaqcformdetailDTOS[i].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                }
                _wdDTO.qaqcformdetailDTOS[i].DateValue1 = DateTime.Now;
                _wdDTO.qaqcformdetailDTOS[i].DateValue2 = DateTime.Now;
            }
            _wdDTO.qaqcformDTOS = qaqcform;
            */

            //}
        }

    }
}
