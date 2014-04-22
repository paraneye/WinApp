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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Schedule.ManageSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubMenu : WinAppLibrary.Controls.LayoutAwarePage
    {
        List<RevealCommonSvc.ExtSchedulerDTO> source = new List<RevealCommonSvc.ExtSchedulerDTO>();

        public SubMenu()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.Loading(true, this);
            SubMenuDataSource menusource = new SubMenuDataSource();
            this.DefaultViewModel["Groups"] = menusource.DataSource.GetGroups("AllGroups");
            source = navigationParameter as List<RevealCommonSvc.ExtSchedulerDTO>;
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

        private async void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DataItem;
            if (item != null)
            {
                SubMenuDataSource.SetCurrentMenu(item.UniqueId);

                if (SubMenuDataSource.CurrentMenu != null)
                {
                    var source = await (new Lib.ServiceModel.CommonModel()).GetFIWPByProjectSchedule_ExtSch(Lib.ScheduleDataSource.selectedSchedule);

                    if (source != null && source.Count > 0)
                        this.Frame.Navigate(SubMenuDataSource.CurrentMenu, source);
                    else
                        WinAppLibrary.Utilities.Helper.SimpleMessage("This schedule has no IWP. Please select again.", "Alert!");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.ManageSchedule.SelectSchedule));
        }

        #endregion

        #region "Private Method"

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = sender as Image;
            //Grid title = (img.Parent as Grid).FindName("grTitle") as Grid;
            img.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + Lib.ContentPath.DefaultDrawing));

            //title.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        #endregion
    }
}
