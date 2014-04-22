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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using Element.Reveal.Manage.Lib;
using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Manage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenu : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _foremanPersonnelId = 0;
        public MainMenu()
        {
            this.InitializeComponent();
            Login.MasterPage.ShowTopBanner = true;
            Login.MasterPage.ShowBackButton = false;
            Login.MasterPage.SetPageTitle("Manage");
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.SetUserTitle(Login.UserAccount.UserName);
            _foremanPersonnelId = Convert.ToInt32(navigationParameter.ToString());
            Login.MasterPage.ShowUserStatus = true;
            if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
            {
            }

            DoNext();
        }

        private void DoNext()
        {
            Login.MasterPage.Loading(true, this);
            MainMenuDataSource menusource = new MainMenuDataSource(Lib.MainMenuGroup.DIAGNOSTIC_CENTER);  // 사용자에 따라 메뉴 설정 변경
            this.DefaultViewModel["Groups"] = menusource.DataSource.GetGroups("AllGroups");
        }

        private void LoadStoryBoardSwitch()
        {
        }

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
                Login.MasterPage.ShowBackButton = true;

                if (MainMenuDataSource.CurrentMenu != null)
                    this.Frame.Navigate(MainMenuDataSource.CurrentMenu, _foremanPersonnelId);
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = sender as Image;
            Grid title = (img.Parent as Grid).FindName("grTitle") as Grid;
            img.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + Lib.ContentPath.DefaultDrawing));

            title.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
