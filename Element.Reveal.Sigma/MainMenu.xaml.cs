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

using Element.Reveal.Sigma.Lib;
using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Sigma
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainMenu : WinAppLibrary.Controls.LayoutAwarePage
    {
        public MainMenu()
        {
            this.InitializeComponent();
            Login.MasterPage.ShowTopBanner = true;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.Loading(true, this);
            MainMenuDataSource menusource = new MainMenuDataSource();
            this.DefaultViewModel["Groups"] = menusource.DataSource.GetGroups("AllGroups");
        }

        #region "EventHandler"
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;
        }

        private void itemGridView_Loaded(object sender, RoutedEventArgs e)
        {
            itemListView.SelectedItem = itemGridView.SelectedItem = null;
            itemGridView.SelectionMode = itemListView.SelectionMode = ListViewSelectionMode.Single;
            Login.MasterPage.Loading(false, this);
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
             var item = e.ClickedItem as DataItem;
             if (item != null)
             {
                 MainMenuDataSource.SetCurrentMenu(item.UniqueId);
                 this.Frame.Navigate(typeof(GroupedItemsPage));
             }
        }
        #endregion
    }
}
