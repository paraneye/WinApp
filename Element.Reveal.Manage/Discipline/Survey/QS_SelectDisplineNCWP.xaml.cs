using Element.Reveal.Manage.RevealProjectSvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using WinAppLibrary;
using WinAppLibrary.Utilities;

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

namespace Element.Reveal.Manage.Discipline.Survey
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QS_SelectDisplineNCWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        List<CwpDTO> _oCWP = new List<CwpDTO>();
        List<RevealProjectSvc.ProgressruleofcreditCompletedDTO> mlist = new List<ProgressruleofcreditCompletedDTO>();
        ObjectSCPReturn objReturn = new ObjectSCPReturn();
        
        public QS_SelectDisplineNCWP()
        {
            this.InitializeComponent();

            Login.MasterPage.ShowTopBanner = true;
            //Login.MasterPage.ShowBackButton = true;
            Login.MasterPage.ShowBackButton = false;
            Login.MasterPage.SetPageTitle("Select Discipline & Construction Work Page");
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.Loading(true, this);

            LoadData();

            //Login.MasterPage.DoBeforeBack += MasterPage_DoBeforeBack;
        }

        private async void LoadData()
        {
            mlist = await (new Lib.ServiceModel.ProjectModel()).GetProgressruleofcreditCompleted(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, Login.UserAccount.PersonnelID);

            Login.MasterPage.Loading(false, this);

            if (Login.UserAccount.DepartmentName != WinAppLibrary.Utilities.Department.GeneralForeman.ToString())
            {
                LoadDataForNotGF();
            }
            else
            {
                LoadDataForGF();
            }
        }

        private void LoadDataForGF()
        {
            List<RevealProjectSvc.ProgressruleofcreditCompletedDTO> strList = (from m in mlist
                                                                               where 1 == 1
                                                                               select m).ToList<RevealProjectSvc.ProgressruleofcreditCompletedDTO>();
            objReturn.lData = strList;
            objReturn.intType = 1;

            if (strList == null)
            {
                MoveBackAsyc("There is no Discipline data.", "");
            }
            else
            {
                if (strList.Count() == 0)
                {
                    MoveBackAsyc("There is no Discipline data.", "");
                }
                else
                {
                    this.Frame.Navigate(typeof(QS_SelectScheduleLineItemNIWP), objReturn);
                }
            }            
        }

        private void LoadDataForNotGF()
        {
            /*
            // Origin Sample ( Shawn )
            var strList = from m in mlist
                          where 1 == 1
                          group m by new { m.ModuleID, m.ModuleName, m.CWPID, m.CWPName, m.CWAName } into g
                          select new ObjectDisc { ModuleID = g.Key.ModuleID, ModuleName = g.Key.ModuleName, CWPName = g.Key.CWPName, CWPID = g.Key.CWPID, CWAName = g.Key.CWAName };
            */
             
            // Real Sample ( Shawn )
            var strList = from m in mlist
                          where 1 == 1
                          group m by new { m.ModuleID, m.ModuleName } into g
                          select new ObjectDisc { ModuleID = g.Key.ModuleID, ModuleName = g.Key.ModuleName };

            /*
            // Test Sample ( Shawn )
            var strList = from m in mlist
                          where 1 == 1
                          select new ObjectDisc { ModuleID = m.ModuleID, ModuleName = m.ModuleName };
            */

            if (strList == null)
            {
                MoveBackAsyc("There is no CWP data.", "");
            }
            else
            {
                if (strList.Count() == 0)
                {
                    MoveBackAsyc("There is no CWP data.", "");
                }
                else
                {
                    lvDiscipline.ItemsSource = strList;
                }
            } 
        }

        /// <summary>
        /// ListView SelectionChanged Reference ( Shawn )
        /// SelectionChanged Need the following
        /// SelectionChanged="lvDiscipline_SelectionChanged"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intId = 0;
            string strName = string.Empty;

            if (e.AddedItems.Count > 0)
            {
                intId = ((ObjectDisc)e.AddedItems[0]).ModuleID;
                strName = ((ObjectDisc)e.AddedItems[0]).ModuleName;

                var strList = from m in mlist
                              where 1 == 1
                                 && m.ModuleID == intId
                              group m by new { m.ModuleID, m.CWPID, m.CWPName, m.CWAName } into g
                              select new ObjectDisc { ModuleID = intId, ModuleName = strName, CWPID = g.Key.CWPID, CWPName = g.Key.CWPName, CWAName = g.Key.CWAName };

                lvCWP.ItemsSource = strList;
            }
            else
            {
                lvCWP.ItemsSource = null;
            }
        }
        
        /// <summary>
        /// ListView ItemClick Reference ( Shawn )
        /// ItemClick Need the followings
        /// IsItemClickEnabled="True" ItemClick="lvCWP_ItemClick"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvCWP_ItemClick(object sender, ItemClickEventArgs e)
        {
            int intMId = ((ObjectDisc)e.ClickedItem).ModuleID;
            int intCId = ((ObjectDisc)e.ClickedItem).CWPID;

            // It conveters Linq List for ProgressruleofcreditCompletedDTO ( Shawn )
            List<RevealProjectSvc.ProgressruleofcreditCompletedDTO> strList = (from m in mlist 
                                                                               where 1 == 1
                                                                                  && m.ModuleID == intMId
                                                                                  && m.CWPID == intCId
                                                                               select m).ToList<RevealProjectSvc.ProgressruleofcreditCompletedDTO>();

            //this.Frame.Navigate(typeof(QS_SelectScheduleLineItemNIWP), strList);

            objReturn.lData = strList;
            objReturn.intType = 2;

            this.Frame.Navigate(typeof(QS_SelectScheduleLineItemNIWP), objReturn);
        }

        private void MoveBack()
        {
            //this.Frame.Navigate(typeof(MainMenu), Login.UserAccount.PersonnelID);
            this.Frame.Navigate(typeof(Discipline.PunchCard.FinalWalkDown), Login.UserAccount.PersonnelID);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private async void MoveBackAsyc(string strTitle, string strCaution)
        {
            bool result = await Helper.OkMessage(strTitle, strCaution);

            if (result == true)
            {
                MoveBack();
            }
        }
        //private void MasterPage_DoBeforeBack(object sender, object e)
        //{
            
        //}
    }

    public class ObjectDisc
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public int CWPID { get; set; }
        public string CWPName { get; set; }
        public int CWAID { get; set; }
        public string CWAName { get; set; }
        public int FIWPID { get; set; }
        public string FIWPName { get; set; }
        public int ProjectScheduleID { get; set; }

        public ObjectDisc()
        {
        }

        public ObjectDisc(int m_CWPID, int m_FIWPID, int m_ProjectScheduleID)
        {
            CWPID = m_CWPID;
            FIWPID = m_FIWPID;
            ProjectScheduleID = m_ProjectScheduleID;
        }
    }

    public class ObjectSCPReturn
    {
        public List<RevealProjectSvc.ProgressruleofcreditCompletedDTO> lData { get; set; }
        public int intType { get; set; }

        public ObjectSCPReturn()
        {
        }

        public ObjectSCPReturn(List<RevealProjectSvc.ProgressruleofcreditCompletedDTO> m_Data, int m_Type)
        {
            lData = m_Data;
            intType = m_Type;
        }
    }
}
