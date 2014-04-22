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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.PunchCard
{
    //현재 오프라인모드 방식 아님. MEG시연 후 수정 예정.
    public sealed partial class DownloadPunchList : WinAppLibrary.Controls.LayoutAwarePage
    {
        string fmode = "";
        Lib.DataSource.QaqcDataSource _punchList = new Lib.DataSource.QaqcDataSource();
        private int _projectid, _moduleid, _personnelid;

        public DownloadPunchList()
        {
            this.InitializeComponent();
        }

        #region "Loading Events"
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            _personnelid = Login.UserAccount.PersonnelID;

            try
            {
                fmode = navigationParameter != null ? navigationParameter.ToString() : "1";
                //FillOut Mode
                if (fmode == "1")
                {
                    btnFillout.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    btnSubmit.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                //Submit Mode : 리스트에서 한꺼번에 선택하여 submit할 경우(오프라인모드 사용 시 확인)
                else
                {
                    btnFillout.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    btnSubmit.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                BindList();
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "LoadState");
            }
        }

        //Grid Bind : all punch tickets assigned to the foreman (submit된 문서는 제외)
        //approval관련 일단 제외(reject된 문서 등)
        private async void BindList()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                //if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                //{
                    //personnelId, departmentId 확인 필요(현재는 0으로 넘김, 다 조회)
                    await _punchList.GetPunchListByPersonnelDepartment(_projectid, _moduleid, 0, 0);
                    lvPunchList.ItemsSource = _punchList.GetPunchListByPersonnelDepartment();
                //}
                //else
                //{

                //}
            }
            catch (Exception e)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Cannot get Punch List", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        #endregion


        #region "Button Event"
        private void btnFillout_Click(object sender, RoutedEventArgs e)
        {
            FilloutPunchTicket();
        }

        private void lvPunchList_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FilloutPunchTicket();
        }

        //submit기능 일단 제외
        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //_ofiles = await LoadToQaqcformtemplate();

            //SubmitPunchTicket();
        }

        private void FilloutPunchTicket()
        {
            string QaqcformID = "";
            try
            {
                if (lvPunchList.SelectedItems.Count > 0)
                {
                    //QaqcformId 확인
                    RevealProjectSvc.QaqcformdetailDTO dto = (RevealProjectSvc.QaqcformdetailDTO)lvPunchList.SelectedItems.ToList().FirstOrDefault();
                    QaqcformID = dto.QAQCFormID.ToString();//.StringValue5.ToString();

                    this.Frame.Navigate(typeof(FilloutPunchCard), QaqcformID);
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Please select Punch Card", "Caution!");
                }
            }
            catch (Exception ex)
            {

                WinAppLibrary.Utilities.Helper.SimpleMessage("Fill Out Error!", "Error!");
            }
        }

        private async void SubmitPunchTicket()
        {
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        #endregion
    }

}
