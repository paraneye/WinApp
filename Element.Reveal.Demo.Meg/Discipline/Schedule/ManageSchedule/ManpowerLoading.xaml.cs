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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Schedule.ManageSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManpowerLoading : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid;

        public ManpowerLoading()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            
            LoadScheduleInfo();
            LoadManpowerLoading();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.ManageSchedule.SubMenu));
        }
        
        #endregion

        #region "Private Method"

        private void LoadManpowerLoading()
        {
            string sourceUrl = WinAppLibrary.Utilities.Helper.ServiceUrl.Substring(0, WinAppLibrary.Utilities.Helper.ServiceUrl.LastIndexOf('/'));
            if (WinAppLibrary.Utilities.Helper.DBInstance.ToLower().Contains("_dev_"))
            {
                sourceUrl = sourceUrl.Substring(0, sourceUrl.LastIndexOf('/') + 1) + WinAppLibrary.Utilities.Helper.DBInstance.ToLower().Replace("_dev_", ".");
            }
            else
            {
                sourceUrl = sourceUrl.Substring(0, sourceUrl.LastIndexOf('/') + 1) + WinAppLibrary.Utilities.Helper.DBInstance.Replace('_', '.');
            }
            string test = sourceUrl + "/Discipline/MobileReport.aspx?pageno=4&pjid=" + _projectid + "&mdid=" + _moduleid + "&cwid=" + Lib.CWPDataSource.selectedCWP + "&dbname=" + WinAppLibrary.Utilities.Helper.DBInstance;
            Uri source = new Uri(sourceUrl + "/Discipline/MobileReport.aspx?pageno=4&pjid=" + _projectid + "&mdid=" + _moduleid + "&cwid=" + Lib.CWPDataSource.selectedCWP + "&dbname=" + WinAppLibrary.Utilities.Helper.DBInstance);

            //Uri source = new Uri("http://reveal.elementindustrial.com/reveal.demo/Discipline/MobileReport.aspx?pageno=4&pjid=" + _projectid + "&mdid=" + _moduleid + "&cwid=" + Lib.CWPDataSource.selectedCWP + "&dbname=" + WinAppLibrary.Utilities.Helper.DBInstance);
            //Uri source = new Uri("http://dev.elementindustrial.com/Reveal.PreDemo/Discipline/MobileReport.aspx?pageno=4&pjid=" + _projectid + "&mdid=" + _moduleid + "&cwid=" + Lib.CWPDataSource.selectedCWP + "&dbname=" + WinAppLibrary.Utilities.Helper.DBInstance);
            
            wvManpowerLoading.Source = source;
        }

        private void LoadScheduleInfo()
        {
            tbScheduleName.Text = Lib.ScheduleDataSource.selectedScheduleName;
            tbSchedulePeriod.Text = " Start: " + Lib.ScheduleDataSource.selectedScheduleStartDate + " ~ End: " + Lib.ScheduleDataSource.selectedScheduleEndDate;
        }

        #endregion
    }
}
