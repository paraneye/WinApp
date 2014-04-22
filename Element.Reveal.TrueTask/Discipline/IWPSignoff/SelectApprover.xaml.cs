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

using Element.Reveal.DataLibrary;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.IWPSignoff
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectApprover : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _fiwpID = Lib.IWPDataSource.selectedIWP;
        private FiwpDTO fiwpDto = new FiwpDTO();
        Lib.WorkFlowDataSource _Workflow = new Lib.WorkFlowDataSource();
        private string PackageTypeCode = DataLibrary.Utilities.WorklowTypeCode.IWP;  //추후 구분타입이 오게되면 재설정 해야함
        private string isNewYN = "Y";

        public SelectApprover()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _fiwpID = Lib.IWPDataSource.selectedIWP;
            if(_fiwpID == null || _fiwpID == 0)
                _fiwpID = Convert.ToInt32(navigationParameter.ToString());

            SetIwp(_fiwpID);

            SetDepartment();

            btnRemove.IsEnabled = false;
        }

        //IWP 정보 세팅
        private async void SetIwp(int iwpId)
        {
            try
            {
                await _Workflow.GetIWPByIwpID(iwpId);
                fiwpDto = _Workflow.GetIWP().FirstOrDefault();

                txbIwpName.Text = fiwpDto.FiwpName;

                //Workflow 바인딩
                List<PendingWorkflow> pendingdto = new List<PendingWorkflow>();
                await _Workflow.GetPendingWorkflowByPackageTypeCode(PackageTypeCode, iwpId);

                if (_Workflow.GetPendingWorkflow().Count < 1)
                {
                    await _Workflow.GetWorkflowRoleTitleByPackageTypeCode(PackageTypeCode, iwpId);
                    isNewYN = "Y"; //신규
                }
                else
                    isNewYN = "N"; //저장된 내역

                pendingdto = _Workflow.GetPendingWorkflow();

                if (pendingdto != null && pendingdto.Count() > 0)
                {
                    ////Grid 자동생성 시
                    //GridLoadBind(pendingdto);  

                    //ListView 사용시
                    lvWorkflow.ItemsSource = pendingdto;
                }
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("There is no workflow - Please create workflow first", "Loading Error");
                
            }
            catch (Exception ex)
            {

            }
        }

        #region 주석
        //private void GridClear()
        //{
        //    grdWorkflow.ColumnDefinitions.Clear();
        //    grdWorkflow.RowDefinitions.Clear();
        //    grdWorkflow.Children.Clear();
        //}

        ////저장된 Workflow 바인딩
        //private void GridLoadBind(List<PendingWorkflow> _dto)
        //{
        //    try
        //    {
        //        GridClear();

        //        _dto = _dto.OrderBy(x => x.TransitionStatusSeq).ThenBy(y => y.UserSeq).ToList();

        //        long groupcnt = _dto[0].TransitionStatusSeq;
        //        int membercount = _dto.Where(y => y.TransitionStatusSeq == _dto[0].TransitionStatusSeq).GroupBy(x => x.TransitionStatusSeq).Count();
        //        int icolumn = 0;

        //        foreach (PendingWorkflow pending in _dto)
        //        {
        //            //동일 레벨
        //            if (groupcnt == pending.TransitionStatusSeq)
        //            {
        //                ColumnLoadDefine(icolumn, pending);
        //            }
        //            else //다음 레벨
        //            {
        //                NextStepColumnDefine(icolumn);
        //                membercount = _dto.Where(y => y.TransitionStatusSeq == pending.TransitionStatusSeq).GroupBy(x => x.TransitionStatusSeq).Count();
        //                icolumn = icolumn + 1;
        //                ColumnLoadDefine(icolumn, pending);
        //            }

        //            icolumn = icolumn + 1;
        //            groupcnt = pending.TransitionStatusSeq;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        ////결재 대상 인원 바인딩
        //private void ColumnLoadDefine(int icolumn, PendingWorkflow dto)
        //{
        //    SolidColorBrush txbcolor = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#FFFFFF");  //white
        //    SolidColorBrush spnlBackGround;
        //    //IsProcessStatus : Y - 접속자 승인, N - 접속자 미승인, X - 접속자 거절  (결재자의 정보)
        //    //dto.WorkflowStatusCd : Y - 결재 승인완료, N - 결재 미승인, X - 결재 거절 (Workflow 전체의 정보)
        //    if (dto.IsProcessStatus == "X")  //Denine
        //    {
        //        spnlBackGround = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#FF0000"); //red
        //        //editcrew = false;
        //    }
        //    else if (dto.IsProcessStatus == "Y") //pending
        //    {
        //        spnlBackGround = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#0054FF"); //blue
        //        //editcrew = false;
        //    }
        //    else // not sent
        //    {
        //        spnlBackGround = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#BDBDBD"); //gray
        //    }

        //    Windows.UI.Xaml.TextWrapping txbwrap = TextWrapping.Wrap;
        //    //Windows.UI.Xaml.Media.FontFamily txbfont = Windows.UI.Xaml.Media.FontFamily("Segoe UI Regular"); // "Segoe UI Regular"  as ;

        //    RowDefinition row1 = new RowDefinition();
        //    RowDefinition row2 = new RowDefinition();
        //    row1.MinHeight = 50;
        //    row1.MinHeight = 30;
        //    grdWorkflow.RowDefinitions.Add(row1);
        //    grdWorkflow.RowDefinitions.Add(row2);

        //    ColumnDefinition column = new ColumnDefinition();
        //    column.MinWidth = 150;
        //    grdWorkflow.ColumnDefinitions.Add(column);

        //    TextBlock txbName = new TextBlock();
        //    txbName.Text = dto.SigmaUserName;
        //    txbName.Foreground = txbcolor;
        //    txbName.TextWrapping = txbwrap;
        //    txbName.FontSize = 16;
        //    txbName.Margin = new Thickness(10,0,10,0);
        //    txbName.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
        //    txbName.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
        //    Grid.SetRow(txbName, 0);
        //    Grid.SetColumn(txbName, icolumn);
        //    grdWorkflow.Children.Add(txbName);

        //    StackPanel spnl = new StackPanel();
        //    spnl.Background = spnlBackGround;
        //    spnl.HorizontalAlignment = HorizontalAlignment.Stretch;
        //    spnl.VerticalAlignment = VerticalAlignment.Stretch;
        //    Grid.SetRow(spnl, 1);
        //    Grid.SetColumn(spnl, icolumn);
        //    grdWorkflow.Children.Add(spnl);

        //}
                
        ////다음 결재 Step
        //private void NextStepColumnDefine(int icolumn)
        //{
        //    RowDefinition row1 = new RowDefinition();
        //    RowDefinition row2 = new RowDefinition();
        //    row1.MinHeight = 50;
        //    row1.MinHeight = 30;
        //    grdWorkflow.RowDefinitions.Add(row1);
        //    grdWorkflow.RowDefinitions.Add(row2);

        //    ColumnDefinition column = new ColumnDefinition();
        //    column.MinWidth = 150;
        //    grdWorkflow.ColumnDefinitions.Add(column);

        //    Image imgStep = new Image();

        //    //x:Name="imgnext2" Source="/Assets/status_arrow.png" Margin="68,0,68,0" Width="30" Height="50" 
        //    imgStep.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/status_arrow.png"));
        //    imgStep.Margin = new Thickness(68, 0, 68, 0);
        //    imgStep.Width = 30;
        //    imgStep.Height = 50;

        //    Grid.SetRowSpan(imgStep, 2);
        //    Grid.SetColumn(imgStep, icolumn);
        //    grdWorkflow.Children.Add(imgStep);

        //}

        #endregion
        //Department 바인딩
        private async void SetDepartment()
        {
            try
            {
                List<Department> DepartmentDto = new List<Department>();
                ucCrewList.LoadDepartment(DepartmentDto);  //Clear

                await _Workflow.GetDepartmentUsed(Login.UserAccount.CurProjectID);
                DepartmentDto = _Workflow.GetDepartment();

                ucCrewList.LoadDepartment(DepartmentDto);
            }
            catch (Exception ex)
            {

            }
        }

        //Crew 바인딩
        private async void SetCrewList(int SigmaRoleId)
        {
            try
            {
                List<CrewByDepartmentID> CrewDto = new List<CrewByDepartmentID>();

                ucCrewList.gvCrewList_Clear();

                await _Workflow.GetCrewByDepartmentID(SigmaRoleId);
                
                foreach (CrewByDepartmentID dto in _Workflow.GetCrewList())
                {
                    var photo = await (new Lib.ServiceModel.UserModel()).GetPersonnelPhoto(dto.SigmaUserId);
                    if (!(photo.DataName != null && (photo.DataName.ToLower().Contains(".jpg") || photo.DataName.ToLower().Contains(".png"))))
                        photo.DataName = WinAppLibrary.Utilities.Helper.BaseUri + "Assets/common_default_user.png";

                    dto.imageurl = new Uri(photo.DataName);
                    CrewDto.Add(dto);
                }

                ucCrewList.LoadCrewList(CrewDto);
            }
            catch (Exception ex)
            {
                
            }
        }

        //Department 선택시 CrewList 바인딩
        private void ucCrewList_lvDepartmentSelectionChanged(object sender, ListViewBase e)
        {
            SetCrewList(((Department)e.SelectedItem).SigmaRoleId);
        }

        //CrewList에서 Crew 선택시 상단 WorkflowBar에 선택한 Crew 바인딩
        private void ucCrewList_gvCrewListSelectionChanged(object sender, ListViewBase e)
        {
            if (lvWorkflow.SelectedItem != null)
            {
                CrewByDepartmentID dto = (CrewByDepartmentID)e.SelectedItem;

                PendingWorkflow Selectworkflowdto = (PendingWorkflow)lvWorkflow.SelectedItem;

                if (dto != null)
                {
                    if (CheckCrewList(dto.SigmaUserId))
                    {
                        if(Selectworkflowdto.SigmaRoleName == "")
                            Selectworkflowdto.SigmaRoleName = Selectworkflowdto.SigmaUserName;
                        Selectworkflowdto.SigmaRoleId = dto.SigmaRoleId;
                        Selectworkflowdto.SigmaUserName = dto.SigmaUserName;
                        Selectworkflowdto.SigmaUserId = dto.SigmaUserId;

                        lvWorkflow.SelectedItem = Selectworkflowdto;

                        lvWorkflowRebind();
                    }
                    else
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Selected Crew Already Assigned.", "Warning");
                }
                //else
                //{
                //    WinAppLibrary.Utilities.Helper.SimpleMessage("Please Select Workflow First.", "Warning");
                //}
            }
        }

        //동일 인물 등록 체크
        private bool CheckCrewList(string userid)
        {
            bool result = true;

            foreach (PendingWorkflow item in lvWorkflow.Items)
            {
                if (item.SigmaUserId == userid)
                    result = false;
            }

            return result;
        }

        //ListView ReBind
        private void lvWorkflowRebind()
        {
            List<PendingWorkflow> dto = new List<PendingWorkflow>();
            foreach (PendingWorkflow item in lvWorkflow.Items)
            {
                dto.Add(item);
            }
            lvWorkflow.ItemsSource = dto;

        }

        //CrewList 
        private void lvWorkflow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvWorkflow.SelectedItem != null)
            {
                PendingWorkflow Selectworkflowdto = (PendingWorkflow)lvWorkflow.SelectedItem;
                if (Selectworkflowdto.SigmaUserId != null && Selectworkflowdto.SigmaUserId != "")
                    btnRemove.IsEnabled = true;
                else
                    btnRemove.IsEnabled = false;
            }
            else
                btnRemove.IsEnabled = false;

        }

        //상단 WorkflowBar에서 선택한 Crew 제거
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lvWorkflow.SelectedItem != null)
            {
                PendingWorkflow Selectworkflowdto = (PendingWorkflow)lvWorkflow.SelectedItem;

                Selectworkflowdto.SigmaUserName = Selectworkflowdto.SigmaRoleName;
                Selectworkflowdto.SigmaRoleId = 0;
                Selectworkflowdto.SigmaUserId = "";

                lvWorkflow.SelectedItem = Selectworkflowdto;

                lvWorkflowRebind();
            }
            else
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please Select Workflow First.", "Warning");
            }  
        }

        //선택된 WorkflowBar의 CrewList 저장 후 화면 빠져나가기
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (CheckWorkflow())
            {
                SaveWorkFlow();                
            }
            else
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please assign all approvers before completing assemble IWP. Signoff request cannot be sent with this condition", "Cannot Complete");
            }
        }

        //Workflow가 다 채워졌는지 검사
        private bool CheckWorkflow()
        {
            bool result = true;

            foreach (PendingWorkflow item in lvWorkflow.Items)
            {
                if (item.SigmaUserName == "" || item.SigmaUserName == null || item.SigmaUserId == "" || item.SigmaUserId == null)
                {
                    result = false;
                }
            }

            return result;
        }

        //WorkFlow 저장
        private async void SaveWorkFlow()
        {
            try
            {
                List<TypeTransition> CrewListDto = new List<TypeTransition>();

                Guid workflowid = new Guid();
                string SaveYN = "Y";  //저장가능 여부

                int oldseq = 0;
                int i = 0;
                int iseq = 0;

               foreach (PendingWorkflow item in lvWorkflow.Items)
                {
                    TypeTransition dto = new TypeTransition();

                    if (i == 0)
                    {
                        oldseq = Convert.ToInt32(item.TransitionStatusSeq);
                    }
                    else if(oldseq != Convert.ToInt32(item.TransitionStatusSeq))
                    {
                        iseq = iseq + 1;
                        oldseq = Convert.ToInt32(item.TransitionStatusSeq);
                    }

                    dto.Row = iseq;  //같은 그룹끼리의 번호 
                    dto.UserId = item.SigmaUserId;
                    dto.Role = item.SigmaRoleId;

                    workflowid = item.WorkFlowId;

                    SaveYN = item.IsModified;

                    CrewListDto.Add(dto);

                    i = i + 1;
                }

                bool result;

                string strTitle = fiwpDto.Description;   //Title = SignoffStatus 화면의 Description 컬럼

                if (isNewYN == "Y") //신규일경우
                    result = await (new Lib.ServiceModel.WorkflowModel()).SaveWorkflowCrew(PackageTypeCode, 0, Login.UserAccount.PersonnelId, CrewListDto, strTitle, string.Empty, string.Empty, _fiwpID, _fiwpID);
                else
                {
                    if (SaveYN == "Y") //업데이트가 가능할 경우 (워크플로우가 진행되지 않은 상태)
                    {
                        result = await (new Lib.ServiceModel.WorkflowModel()).UpdateWorkflowCrew(PackageTypeCode, 0, workflowid, CrewListDto, fiwpDto.FiwpID);
                    }
                    else
                    {
                        result = true;
                        WinAppLibrary.Utilities.Helper.SimpleMessage("This Workflow is alredy processing", "Cannot Saved");
                    }
                }
                                
                if (result)
                {
                    if (isNewYN == "Y")
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Successfully Saved", "Save Complete");
                    }
                    else if (SaveYN == "Y")
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Successfully Updated", "Update Complete");
                    }


                    #region

                    //현재 위자드 모드 확인/저장
                    Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
                    await _iwp.GetFiwpByCwpSchedulePackageTypeOnMode(Lib.CWPDataSource.selectedCWP, Lib.ScheduleDataSource.selectedSchedule, Lib.CommonDataSource.selPackageTypeLUID);
                    List<DataLibrary.FiwpDTO> iwplist = _iwp.GetFiwpByProjectScheduleID();
                    DataLibrary.FiwpDTO iwpdto = new DataLibrary.FiwpDTO();
                    iwpdto = iwplist.Where(x => x.FiwpID == Lib.IWPDataSource.selectedIWP).FirstOrDefault();

                    Lib.IWPDataSource.isWizard = iwpdto.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.APPROVER ? false : true;

                    Lib.WizardDataSource.SetTargetMenu(DataLibrary.Utilities.AssembleStep.APPROVER, Lib.CommonDataSource.selPackageTypeLUID, true);

                    if (Lib.WizardDataSource.NextMenu != null)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu);

                    #endregion
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Failed to save the workflow - Please try again later", "Saving Error");
                }
            }
            catch (Exception ex)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Error to save the workflow", "Error");
            }
            
        }
                        
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //이전 메뉴로 이동
            if (Lib.WizardDataSource.PreviousMenu != null)
            {
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu, DataLibrary.Utilities.AssembleStep.MOC);
            }
        }
        
    }
}

