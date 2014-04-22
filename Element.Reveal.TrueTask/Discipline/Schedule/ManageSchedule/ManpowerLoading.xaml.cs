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
using oz.api;
using Element.Reveal.TrueTask.Lib.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.ManageSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManpowerLoading : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid; private string _disciplineCode;
        OZReportViewer ozViewer = null;
        public ManpowerLoading()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            
            LoadScheduleInfo();
            LoadServerFile();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.ManageSchedule.SubMenu));
        }
        
        #endregion

        #region "Private Method"

        private void LoadServerFile()
        {
            Login.MasterPage.Loading(true, this);

            ReportDS dsReport = new ReportDS();
            object[] objParam = new object[2];

            objParam[0] = "CwpID="+Lib.CWPDataSource.selectedCWP;
            objParam[1] = "ScheduleWorkItemID= " + Lib.ScheduleDataSource.selectedSchedule;
            dsReport.Params = objParam;

            dsReport.ToolBarUseYn = "N";
            try
            {
                if (ozViewer != null)
                {
                    ozViewer.Dispose();
                }
                dsReport.ServerYn = "Y";
                dsReport.ProjectCode = "LedCore";

                dsReport.ReportName = "/TureTaskManPower.ozr";
                dsReport.OdiName = "TrueTaskManPower";
                string strParam = ReportUtil.MakeParameterForOnline(dsReport);

                ozViewer = ReportUtil.RunReport(brdViewer, strParam);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            Login.MasterPage.Loading(false, this);
        }

        private void LoadScheduleInfo()
        {
            tbScheduleName.Text = Lib.ScheduleDataSource.selectedScheduleName;
            tbSchedulePeriod.Text = " Start: " + Lib.ScheduleDataSource.selectedScheduleStartDate + " ~ End: " + Lib.ScheduleDataSource.selectedScheduleEndDate;
        }

        #endregion
    }
}
