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

namespace Element.Reveal.TrueTask.Discipline.MTO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectCWPTask : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        List<RevealCommonSvc.MaterialcategoryDTO> _materialcategory = new List<RevealCommonSvc.MaterialcategoryDTO>();
        private int _projectid, _moduleid;

        public SelectCWPTask()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            LoadCWP();
            LoadTask();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        #endregion

        #region "Private Method"

        private async void LoadCWP()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _cwp.GetCWPsByProjectIDOnMode(_projectid, _moduleid);

                    //_iwp = await (new Lib.ServiceModel.CommonModel()).GetFIWPByCwp_Combo(0, _projectid, _moduleid);
                    lvCWP.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo(_projectid, _moduleid); 
                    
                  //  lvCWP.ItemsSource = _cwp.GetCWPs();

                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }

            Login.MasterPage.Loading(false, this);
        }

        private async void LoadTask()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    _materialcategory = await (new Lib.ServiceModel.CommonModel()).GetMaterialCategoryByModuleID(_moduleid);
                    lvTask.ItemsSource = _materialcategory;
                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }

            Login.MasterPage.Loading(false, this);
            
        }

        #endregion

        private void lvTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lib.CommonDataSource.selectedMaterialCategory = ((RevealCommonSvc.MaterialcategoryDTO)lvTask.SelectedItem).MaterialCategoryID;
            if(lvCWP.SelectedItems.Count > 0)
                this.Frame.Navigate(typeof(Discipline.MTO.ThumbnailViewer));
        }

        private void lvCWP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Lib.CWPDataSource.selectedCWP = ((RevealCommonSvc.ComboBoxDTO)lvCWP.SelectedItem).DataID;
                Lib.CWPDataSource.selectedCWPName = ((RevealCommonSvc.ComboBoxDTO)lvCWP.SelectedItem).DataName;
                if(lvTask.SelectedItems.Count > 0)
                    this.Frame.Navigate(typeof(Discipline.MTO.ThumbnailViewer));
            }
            catch (Exception ex)
            { 
            }
             
            /*
            var cwp = e.AddedItems[0] as RevealProjectSvc.CwpDTO;
            Lib.CWPDataSource.selectedCWP = cwp.CWPID;
            Lib.CWPDataSource.selectedCWPName = cwp.CWPName;*/
            
        }
    }
}
