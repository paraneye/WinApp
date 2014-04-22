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

namespace Element.Reveal.Meg.Discipline.Schedule.BuildSIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComponentGrouping : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        private int _projectid, _moduleid;
        private int _totalselectedcnt;
        private bool _chk = true;
        public ComponentGrouping()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            LoadGrouping();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildSIWP.SelectSIWP));
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (lvSystem.SelectedItems.Count > 0)
            {

                if (lvSystemLine.SelectedItems.Count > 0)
                {
                    _objectparam.materialCategoryIdList = new List<int>();
                    _objectparam.taskCategoryIdList = new List<int>();
                    _objectparam.typeLUIdList = new List<int>();
                    _objectparam.systemIdList = new List<int>();
                    _objectparam.drawingtypeLUIdList = new List<int>();
                    _objectparam.costcodeIdList = new List<int>();
                    _objectparam.searchstringList = new List<RevealProjectSvc.ComboBoxDTO>();
                    _objectparam.compsearchstringList = new List<RevealProjectSvc.ComboBoxDTO>();
                    _objectparam.locationList = new List<RevealProjectSvc.ComboBoxDTO>();
                    _objectparam.rfinumberList = new List<string>();
                    _objectparam.searchcolumn = string.Empty;
                    _objectparam.searchvalueList = new List<string>();
                    _objectparam.isolinenoList = new List<string>();


                    foreach (RevealProjectSvc.ComboBoxDTO dto in lvSystem.SelectedItems)
                    {
                        _objectparam.systemIdList.Add(dto.DataID);
                    }

                    foreach (RevealProjectSvc.ComboBoxDTO dto in lvSystemLine.SelectedItems)
                    {
                        _objectparam.isolinenoList.Add(dto.DataName);
                    }

                    this.Frame.Navigate(typeof(Discipline.Schedule.BuildSIWP.ScheduleComponents), _objectparam);
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "Please Select System Line.");  
                }
            }
            else
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "Please Select System.");
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lvSystem.SelectedItems.Clear();
        }

        #endregion

        #region "Private Method"

        private async void LoadGrouping()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();

            List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _commonsource.GetAvailableCollectionForScheduling(0, -1, _projectid, _moduleid);

                    List<RevealProjectSvc.ComboBoxDTO> lstSystem = _commonsource.orgCollectionforsiwp.GroupBy(x => new { x.SystemID, x.SystemName }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = x.Key.SystemName,
                        DataID = x.Key.SystemID
                    }).ToList();

                    lvSystem.ItemsSource = lstSystem.Where(x=> x.DataID > 0 ).ToList();

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "ComponentGrouping LoadGrouping", "There was an error load Grouping. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        #endregion

        private void lvSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<RevealProjectSvc.CollectionDTO> list = _commonsource.orgCollectionforsiwp;
            string pars = "";


            foreach (RevealProjectSvc.ComboBoxDTO dto in lvSystem.SelectedItems)
            {
                pars += dto.DataID + ";";
            }

            var result = list.Where(x => pars.Contains(x.SystemID.ToString())).ToList();

            List<RevealProjectSvc.ComboBoxDTO> tmp = result.GroupBy(x => new { x.ISOLineNo }).Select(x => new RevealProjectSvc.ComboBoxDTO()
            {
                DataName = x.Key.ISOLineNo
            }).ToList();


            lvSystemLine.ItemsSource = tmp.Where(x => !string.IsNullOrEmpty(x.DataName)).ToList();
        }

        private void lvSystemLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}
