using Element.Reveal.TrueVue.Lib;
using WinAppLibrary.ServiceModels;

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

namespace Element.Reveal.TrueVue
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : WinAppLibrary.Controls.LayoutAwarePage
    {
        private bool _listloaded = false;

        public GroupedItemsPage()
        {
            this.InitializeComponent();
            Login.MasterPage.ShowTopBanner = true;
        }
       
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.Loading(true, this);
            _listloaded = false;

            try
            {
                if (ProjectModuleSource.RequestUpdate)
                {
                    ProjectModuleSource projectmodule = new ProjectModuleSource();
                    projectmodule.InitiateProjectModule();
                    this.DefaultViewModel["Groups"] = await projectmodule.GetProjectModule(Login.LoginMode);
                    ProjectModuleSource.RequestUpdate = false;
                }
                else
                    this.DefaultViewModel["Groups"] = ProjectModuleSource.DataSource.GetGroups("AllGroups");

                itemGridView.SelectedItems.Clear();
                _listloaded = true;

                var selecteds = itemGridView.Items.Where(x => (x as DataItem).Selected);
                foreach (var item in selecteds)
                    itemGridView.SelectedItems.Add(item);

                itemGridView.IsEnabled = itemListView.IsEnabled = Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode ? true : false;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TrueVue LoadState");
            }
            Login.MasterPage.Loading(false, this);
        }

        #region "EventHandler"
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;
        }

       
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((DataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }

        private void itemGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.Count > 0 ? e.AddedItems[0] as DataItem : null;
            if (_listloaded && item != null)
            {
                item.Selected = true;
                var groups = itemGridView.SelectedItems.Where(x => x != item && (x as DataItem).Group == item.Group).ToList();
                if (groups != null && groups.Count > 0)
                {
                    (groups[0] as DataItem).Selected = false;
                    itemGridView.SelectedItems.Remove(groups[0]);
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (itemGridView.SelectedItems.Count == 2)
            {
                this.Frame.Navigate(typeof(Discipline.Viewer.DrawingGridViewer));
            }
            else if(Login.LoginMode == WinAppLibrary.UI.LogMode.OffMode && !WinAppLibrary.Utilities.Helper.DownloadedData)
            {
                this.Frame.Navigate(typeof(Discipline.Viewer.DrawingGridViewer));
            }
            else
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please select Project and Module", "Caution!");
        }
        #endregion
    }
}
