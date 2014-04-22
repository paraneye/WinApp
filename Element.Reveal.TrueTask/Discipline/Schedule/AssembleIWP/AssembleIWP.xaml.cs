using System;
using System.Collections;
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
using WinAppLibrary.Converters;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Element.Reveal.DataLibrary;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssembleIWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        protected int _fiwpid;
        protected List<DataLibrary.FiwpDTO> fiwpdto = new List<DataLibrary.FiwpDTO>();
        Lib.WorkFlowDataSource _Workflow = new Lib.WorkFlowDataSource();

        public AssembleIWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _fiwpid = Lib.IWPDataSource.selectedIWP;
            fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == _fiwpid).ToList();

            btnLoadSitePlan.Visibility = Visibility.Visible;
            btnScope.Visibility = Visibility.Visible;
            btnSafetyChecklist.Visibility = Visibility.Visible;
            btnSafetyDocument.Visibility = Visibility.Visible;
            btnInstallationTestRecord.Visibility = Visibility.Visible;
            btnFieldEquipment.Visibility = Visibility.Visible;
            btnConsumableMaterial.Visibility = Visibility.Visible;
            btnScaffoldChecklist.Visibility = Visibility.Visible;
            btnIWPSignOff.Visibility = Visibility.Visible;
            btnSpecsDetails.Visibility = Visibility.Visible;
            btnMOC.Visibility = Visibility.Visible;
            tbpageTitle.Text = "Assemble Intallation Work Package";
            lblSubTitle.Text = Lib.IWPDataSource.selectedIWPName;

            this.ButtonBar.CurrentMenu = DataLibrary.Utilities.AssembleStep.APPROVER;
            this.ButtonBar.Load();

            Login.MasterPage.ShowUserStatus();

            ////데모 전에 Sign off가 완료되지 않을 경우 대비
            ////1. Sign Off 아이콘 감추기
            //this.btnIWPSignOff.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ////2. AssembleButtonBar에서도 처리 필요
            ////3. MOC에서 위자드 완료 처리
            ////4. Select IWP 에서 위자드 처리
        }

        #region button event
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectIWP));
        }
        
        private void btnLoadSitePlan_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.LoadSitePlan));
        }

        private void btnFieldEquipment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.FieldEquipment));
        }

        private void btnConsumableMaterial_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.ConsumableMaterial));
        }

        private void btnIWPSignOff_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.IWPSignoff.SelectApprover));
        }

        //Document 형식 : SafetyForm, ITR, MOC, Spec
        private void btnSafetyDocument_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleDocument), DataLibrary.Utilities.AssembleStep.SAFETY_FORM);
        }

        private void btnInstallationTestRecord_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleDocument), DataLibrary.Utilities.AssembleStep.ITR);
        }

        private void btnSpecsDetails_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleDocument), DataLibrary.Utilities.AssembleStep.SPEC);
        }

        private void btnMOC_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleDocument), DataLibrary.Utilities.AssembleStep.MOC);
        }

        //e-form 형식 : SafetyChecklist, Scope, ScaffoldChecklist
        private void btnScope_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleReport), DataLibrary.Utilities.AssembleStep.SUMMARY);
        }
        private void btnSafetyChecklist_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleReport), DataLibrary.Utilities.AssembleStep.SAFETY_CHECK);
        }

        private void btnScaffoldChecklist_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleReport), DataLibrary.Utilities.AssembleStep.SCAFFOLD_CHECK);
        }

        private void Button_Clicked(object sender, object e)
        {
            string tag = e != null ? e.ToString() : string.Empty;

            switch (tag)
            {
                case "Complete":
                    CompleteAssemble();
                    break;
                case "Viewer":
                    IWPViewer();
                    break;
            }
        }

        private async void CompleteAssemble()
        {
            //테스트 시 fiwpid 세팅
            //_fiwpid = 5;
            bool IsModifiedYN = false;
            await _Workflow.GetPendingWorkflowByPackageTypeCode(DataLibrary.Utilities.WorklowTypeCode.IWP, _fiwpid);

            //IsModified가 Y여야 상신 가능
            List<PendingWorkflow> _dto = _Workflow.GetPendingWorkflow();
            if (_dto.Count > 0)
                IsModifiedYN = _dto[0].IsModified == "Y" ? true : false;

            if (IsModifiedYN)
            {
                if (await (WinAppLibrary.Utilities.Helper.TrueFalseMessage("You are about to complete the assemble IWP and send the signoff request to approvers", "Complete and Send Request", "OK", "Cancel")))
                {
                    //1. SignOff Request : 서비스 확인
                    try
                    {
                        bool result = await (new Lib.ServiceModel.WorkflowModel()).SaveWorkflowForEasy(DataLibrary.Utilities.WorklowTypeCode.IWP, _fiwpid, 0, true, Login.UserAccount.PersonnelId, "", "");
                        
                        //2. IWP 추가할 건지 확인
                        if (result)
                        {
                            if (await (WinAppLibrary.Utilities.Helper.YesOrNoMessage("If you would like to assemble IWP more, please select ‘Yes’, and exit this menu, please select ‘No’", "")))
                                this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectIWP));
                        }
                        else
                        {
                            WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem complete assemble iwp - Please try again later", "Error");
                        }
                    }
                    catch (Exception ex)
                    {
                        (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Complete Assemble IWP", "There is a problem complete assemble iwp - Please try again later", "Error!");
                    }
                }
            }
            else
            {
                if (await (WinAppLibrary.Utilities.Helper.TrueFalseMessage("Please assign all approvers before completing assemble IWP. Signoff request cannot be sent with this condition", "Cannot Complete", "Go to Select Approvers", "Cancel")))
                    this.Frame.Navigate(typeof(Discipline.IWPSignoff.SelectApprover));
            }
        }
        
        //작성된 문서들 rdl, e-form 등 확인하도록 뷰어 이동
        private void IWPViewer()
        {
            //일단 APPROVER 코드 사용
            this.Frame.Navigate(typeof(Discipline.Viewer.IWPGridViewer), DataLibrary.Utilities.AssembleStep.APPROVER);
        }


        #endregion
    }
}
