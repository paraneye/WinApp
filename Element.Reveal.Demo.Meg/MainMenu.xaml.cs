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

using Element.Reveal.Meg.Lib;
using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Meg
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainMenu : WinAppLibrary.Controls.LayoutAwarePage
    {
        public MainMenu()
        {
            this.InitializeComponent();

            InitControls();
            Login.MasterPage.SetPageTitle("");
            Login.MasterPage.ShowBackButton(false);
        }

        private void InitControls()
        {
            GrdPopBase.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ucSelectTimeSheet.Event_SheetNotExist += ucSelectTimeSheet_Event_SheetNotExist;
            ucSelectTimeSheet.Event_SelectTimeSheet += ucSelectTimeSheet_SelectTimeSheet;
            ucSelectTimeSheet.Event_SheetExist += ucSelectTimeSheet_Event_SheetExist;
            ucSelectTimeSheet.Event_CloseThis += ucSelectTimeSheet_Event_CloseThis;
        }

        void ucSelectTimeSheet_Event_SheetExist(object sender, RoutedEventArgs e)
        {
            GrdPopBase.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void ShowSelectTimeSheetPop()
        {
            ucSelectTimeSheet.GetTimeSheet(Login.UserAccount.PersonnelID);
        }

        void ucSelectTimeSheet_Event_CloseThis(object sender, RoutedEventArgs e)
        {
            GrdPopBase.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        void ucSelectTimeSheet_Event_SheetNotExist(object sender, RoutedEventArgs e)
        {
            GrdPopBase.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        void ucSelectTimeSheet_SelectTimeSheet(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Element.Reveal.Meg.Discipline.TimeProgress.TimeSheet), ucSelectTimeSheet.SelectedTimeSheet);
            GrdPopBase.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.SetUserTitle(Login.UserAccount.UserName);
            Login.MasterPage.ShowUserStatus();
            //if (pageState != null)
            //{
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    if (!Login.NotShowSelectTimeSheet)
                        ShowSelectTimeSheetPop();
                }
            //}

            DoNext();
        }

        private void DoNext()
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
                 
                 if (MainMenuDataSource.CurrentMenu != null)
                     this.Frame.Navigate(MainMenuDataSource.CurrentMenu);
             }
        }

        #endregion

        #region "Private Method"

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = sender as Image;
            Grid title = (img.Parent as Grid).FindName("grTitle") as Grid;
            img.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + Lib.ContentPath.DefaultDrawing));

            title.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        #endregion
    }
}
