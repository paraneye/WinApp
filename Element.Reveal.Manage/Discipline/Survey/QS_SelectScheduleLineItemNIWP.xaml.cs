using Element.Reveal.Manage.RevealProjectSvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WinAppLibrary;
using WinAppLibrary.Extensions;
using WinAppLibrary.ServiceModels;
using WinAppLibrary.Utilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class QS_SelectScheduleLineItemNIWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.ScheduleDataSource _schedule = new Lib.ScheduleDataSource();
        private int _projectid, _moduleid;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbDetailON, _sbDetailOFF;

        ObjectSCPReturn objReturn = new ObjectSCPReturn();
        List<RevealProjectSvc.ProgressruleofcreditCompletedDTO> mlist = new List<ProgressruleofcreditCompletedDTO>();

        private int intType;
        
        public QS_SelectScheduleLineItemNIWP()
        {
            this.InitializeComponent();

            Login.MasterPage.ShowTopBanner = true;
            //Login.MasterPage.ShowBackButton = true;
            Login.MasterPage.ShowBackButton = false;
            Login.MasterPage.SetPageTitle("Select Schedule Line Item & IWP");
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            // mlist = navigationParameter as List<ProgressruleofcreditCompletedDTO>;
            mlist = ((ObjectSCPReturn)navigationParameter).lData;
            intType = ((ObjectSCPReturn)navigationParameter).intType;

            LoadSchedule();
            //Login.MasterPage.DoBeforeBack += MasterPage_DoBeforeBack;
        }

        private void LoadSchedule()
        {
            Login.MasterPage.Loading(true, this);

            List<RevealProjectSvc.ProgressruleofcreditCompletedDTO> titles = new List<RevealProjectSvc.ProgressruleofcreditCompletedDTO>();
            List<DataGroup> grouplist = new List<DataGroup>();

            DataGroup group;
            titles = mlist.Where(x => x.IsWBS == 1).ToList();

            for (int i = 0; i < titles.Count(); i++)
            {
                group = new DataGroup("Group" + i.ToString(), titles[i].ProjectScheduleName, "");

                group.Items = mlist.Where(y => y.IsWBS == 3
                    && titles[i].P6WBSCode == y.P6WBSCode.Substring(0, y.P6WBSCode.LastIndexOf("."))).Select(y =>
                        new DataItem(y.ProjectScheduleID.ToString(), y.P6ActivityID + " - " + y.ProjectScheduleName, y.StartDate + "~" + y.FinishDate, y.DepartStructureID.ToString(), group) { }).ToObservableCollection();

                if (group.Items.Count > 0)
                    grouplist.Add(group);
            }

            this.DefaultViewModel["Schedules"] = grouplist;
            this.gvSchedule.SelectedItem = null;

            Login.MasterPage.Loading(false, this);

            if (grouplist == null)
            {
                MoveBackAsyc("There is no IWP data.", "");
            }
            else
            {
                if (grouplist.Count() == 0)
                {
                    MoveBackAsyc("There is no IWP data.", "");
                }
                else
                {
                    LoadStoryBoardSwitch();
                }
            }
        }

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbDetailOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbDetailOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(detailPanelTrans, Window.Current.Bounds.Width, ANIMATION_SPEED));
            _sbDetailOFF.Begin();

            _sbDetailON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbDetailON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(detailPanelScale, 1, 0));
            _sbDetailON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(detailPanelTrans, 0, ANIMATION_SPEED));
        }

        private void gvSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                detailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _sbDetailON.Begin();

                lvIWP.ItemClick += lvIWP_ItemClick;
                RootPopupBorder.Height = this.Height;
                btnPanelCollapse.Click += btnPanelCollapse_Click;
                SettingsAnimatedPopup.HorizontalOffset = Window.Current.Bounds.Width - 500;
                SettingsAnimatedPopup.IsOpen = true;

                var strList = from m in mlist
                              where 1 == 1
                              select new ObjectDisc
                              {
                                  ModuleID = m.ModuleID,
                                  ModuleName = m.ModuleName,
                                  CWPName = m.CWPName,
                                  CWPID = m.CWPID,
                                  CWAName = m.CWAName,
                                  FIWPID = m.FIWPID,
                                  FIWPName = m.FIWPName,
                                  ProjectScheduleID = m.ProjectScheduleID
                              };

                lvIWP.ItemsSource = strList;
            }
            else
            {
                lvIWP.ItemsSource = null;
            }
        }

        private void btnPanelCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsAnimatedPopup.IsOpen)
            {
                detailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                _sbDetailOFF.Begin();

                SettingsAnimatedPopup.IsOpen = false;   
            }
        }

        private void lvIWP_ItemClick(object sender, ItemClickEventArgs e)
        {
            ObjectDisc objReturn = new ObjectDisc();

            objReturn.CWPID = ((ObjectDisc)e.ClickedItem).CWPID;
            objReturn.FIWPID = ((ObjectDisc)e.ClickedItem).FIWPID;
            objReturn.ProjectScheduleID = ((ObjectDisc)e.ClickedItem).ProjectScheduleID;

            if (SettingsAnimatedPopup.IsOpen)
            {
                detailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                _sbDetailOFF.Begin();

                SettingsAnimatedPopup.IsOpen = false;
            }

            this.Frame.Navigate(typeof(QS_VerifyComponents), objReturn);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MoveBack();
        }

        private void MoveBack()
        {
            if (intType == 2)
            {
                this.Frame.Navigate(typeof(QS_SelectDisplineNCWP));
            }
            else
            {
                this.Frame.Navigate(typeof(MainMenu), Login.UserAccount.PersonnelID);
            }
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
        //    if (intType == 2)
        //    {
        //        this.Frame.Navigate(typeof(QS_SelectDisplineNCWP));
        //    }
        //    else
        //    {
        //        this.Frame.Navigate(typeof(MainMenu), Login.UserAccount.PersonnelID);
        //    }
        //}

        //protected override void GoBack(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
