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

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuildIWPFieldEquipment : WinAppLibrary.Controls.LayoutAwarePage
    {
        private List<RevealCommonSvc.FieldequipmentDTO> orgFieldequipment;
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        private int _projectid, _moduleid;
        private int _totalselectedcnt;
        private bool _chk = true;
        public BuildIWPFieldEquipment()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            LoadLibrary();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildIWP.SelectIWP));
        }

        private void lvCategory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<RevealCommonSvc.FieldequipmentDTO> result = null;
            List<RevealProjectSvc.ComboBoxDTO> tmp = null;

            if (lvCategory1.SelectedItem != null)
            {
                result = orgFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName && !string.IsNullOrEmpty(x.Category2)).ToList();

                tmp = result.GroupBy(x => new { x.Category2 }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.Category2,
                    DataID = 0
                }).ToList();

            }

            lvCategory2.ItemsSource = tmp;
        }

        private void lvCategory2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            List<RevealCommonSvc.FieldequipmentDTO> result = null;
            List<RevealProjectSvc.ComboBoxDTO> tmp = null;
            if (lvCategory2.SelectedItem != null)
            {
                result = orgFieldequipment.Where(x => x.Category2 == (lvCategory2.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName && !string.IsNullOrEmpty(x.Category3)).ToList();

                tmp = result.GroupBy(x => new { x.Category3 }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.Category3,
                    DataID = 0
                }).ToList();
            }


            lvCategory3.ItemsSource = tmp;
        }

        private void lvCategory3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            List<RevealCommonSvc.FieldequipmentDTO> result = null;
            List<RevealProjectSvc.ComboBoxDTO> tmp = null;

            if (lvCategory3.SelectedItem != null)
            {
                result = orgFieldequipment.Where(x => x.Category3 == (lvCategory3.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName).ToList();

                tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.Spec + (string.IsNullOrEmpty(x.Key.SystemType) ? "" : ", " + x.Key.SystemType),
                    DataID = x.Key.FieldEquipmentID
                }).ToList();
            }


            lvSpec.ItemsSource = tmp;
        }

        private void lvSpec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAddIWP_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemoveIWP_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region "Private Method"

        private async void LoadLibrary()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();
            

            List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    orgFieldequipment = await (new Lib.ServiceModel.CommonModel()).GetFieldequipmentByType(string.Empty);

                    List<RevealProjectSvc.ComboBoxDTO> lstCategory1 = orgFieldequipment.GroupBy(x => new { x.Category1 }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = x.Key.Category1,
                        DataID = 0
                    }).ToList();

                    lvCategory1.ItemsSource = lstCategory1;//.Where(x => x.DataID > 0).ToList();

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "ComponentGrouping LoadGrouping", "There was an error load Grouping. Pleae contact administrator", "Error!");
            }

            //this.DefaultViewModel["CWPs"] = source;
            //this.gvCWP.SelectedItem = null;

            //this.gvViewType.SelectedIndex = 0;
            Login.MasterPage.Loading(false, this);
        }

        #endregion
    }
}
