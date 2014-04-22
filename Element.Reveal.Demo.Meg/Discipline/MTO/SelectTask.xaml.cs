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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Element.Reveal.Meg.Discipline.MTO
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SelectTask : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        List<RevealCommonSvc.MaterialcategoryDTO> _materialcategory = new List<RevealCommonSvc.MaterialcategoryDTO>();
        private int _projectid, _moduleid;

        public SelectTask()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            LoadTask();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SelectCWP));
        }

        #endregion

        #region "Private Method"

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
            if (Lib.CWPDataSource.selectedCWP != 0)
                this.Frame.Navigate(typeof(Discipline.MTO.ThumbnailViewer));
            else
                this.Frame.Navigate(typeof(SelectCWP));
        }
    }
}
