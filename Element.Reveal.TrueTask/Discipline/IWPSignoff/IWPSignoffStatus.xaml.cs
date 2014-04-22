using Element.Reveal.DataLibrary;
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

namespace Element.Reveal.TrueTask.Discipline.IWPSignoff
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IWPSignoffStatus : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _fiwpID = 0;
        
        private string btnsentstatus = "N";
        private int btnmonthstatus = 1;

        Lib.WorkFlowDataSource _Workflow = new Lib.WorkFlowDataSource();
        IWPWorkflowStatusBypersonnelid_type_term IWPWorkflowStatusdto = new IWPWorkflowStatusBypersonnelid_type_term();

        public IWPSignoffStatus()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            LoadStatus();
        }

        private void LoadStatus()
        {
            GridBind(btnsentstatus, btnmonthstatus); 
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        private void btnContent_Click(object sender, RoutedEventArgs e)
        {
            if (IWPWorkflowStatusdto != null && IWPWorkflowStatusdto.IwpId > 0)
                this.Frame.Navigate(typeof(SignoffTitle));
            else
                WinAppLibrary.Utilities.Helper.SimpleMessage("An item is not selected. Select an item first!", "Warning!");
        }
       
        //status : Inbox="N" Outbox="Y"
        private async void GridBind(string status, int month)
        {
            try
            {                
                string enddate= DateTime.Now.ToString("yyyyMMdd");
                string startdate = DateTime.Now.AddMonths(month * -1).ToString("yyyyMMdd");

                ucWorkFlowBanner.GridClear();

                await _Workflow.GetIWPWorkflowStatusBypersonnelid_type_term(Login.UserAccount.PersonnelId, startdate, enddate, status);
                
                ucSignoffStatus.LoadSignoffStatus(_Workflow.GetIWPWorkflowStatus());

                int totalCount = await (new Lib.ServiceModel.WorkflowModel()).GetDocumentTransitionStatusTotalCount(Login.UserAccount.PersonnelId);

                ucSignoffStatus.SetButtonText(totalCount.ToString());
            }
            catch (Exception ex)
            {

            }
        }
        
        //SignoffStatus Grid Select Event : Load WorkFlowBar 
        private async void lvListSelectionChanged_Changed(object sender, ListViewBase e)
        {
            try
            {
                ucWorkFlowBanner.GridClear();
                IWPWorkflowStatusdto = (IWPWorkflowStatusBypersonnelid_type_term)e.SelectedItem;
                List<WorkflowDetailByIWPID> workflowbannerdto = new List<WorkflowDetailByIWPID>();
                await _Workflow.GetWorkflowDetailByProcessID(IWPWorkflowStatusdto.ProcessId);
                workflowbannerdto = _Workflow.GetWorkflowDetail();
                ucWorkFlowBanner.LoadWorkFlow(workflowbannerdto);

                Lib.WorkFlowDataSource.PackageTypeCode = IWPWorkflowStatusdto.PackageTypeCode;

                Lib.WorkFlowDataSource.selectedTypeName = IWPWorkflowStatusdto.PackageTypeName;
                Lib.WorkFlowDataSource.selectedDocumentID = IWPWorkflowStatusdto.TargetId;
                Lib.WorkFlowDataSource.selectedIwpID = IWPWorkflowStatusdto.IwpId;
                
                if (btnsentstatus != "")
                    Lib.WorkFlowDataSource.sentyn = btnsentstatus;
            }
            catch (Exception ex)
            {

            }
        }

        //SignoffStatus Inbox Button
        private void btnInboxClick_Click(object sender, ButtonBase e)
        {
            btnsentstatus = "N";

            GridBind(btnsentstatus, btnmonthstatus); 
        }

        //SignoffStatus Sent Button
        private void btnSentClick_Click(object sender, ButtonBase e)
        {
            btnsentstatus = "Y";

            GridBind(btnsentstatus, btnmonthstatus); 
        }

        //SignoffStatus 1Month Button
        private void btn1MonthClick_Click(object sender, ButtonBase e)
        {
            btnmonthstatus = 1;

            GridBind(btnsentstatus, btnmonthstatus); 
        }

        //SignoffStatus 2Month Button
        private void btn2MonthClick_Click(object sender, ButtonBase e)
        {
            btnmonthstatus = 2;

            GridBind(btnsentstatus, btnmonthstatus); 
        }

        //SignoffStatus 3Month Button
        private void btn3MonthClick_Click(object sender, ButtonBase e)
        {
            btnmonthstatus = 3;

            GridBind(btnsentstatus, btnmonthstatus); 
        }

    }
}
