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

using Element.Reveal.Crew.Lib.DataSource;
using WinAppLibrary.ServiceModels;

using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenu : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _foremanPersonnelId = 0;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbPopupON, _sbPopupOFF;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        List<RevealProjectSvc.DailybrasssignDTO> _brasssignList = new List<RevealProjectSvc.DailybrasssignDTO>();
        List<RevealProjectSvc.ToolboxsignDTO> _toolboxList = new List<RevealProjectSvc.ToolboxsignDTO>(); 
        public MainMenu()
        {
            this.InitializeComponent();
            Login.MasterPage.ShowTopBanner = true;
            Login.MasterPage.ShowBackButton = false;
            Login.MasterPage.SetPageTitle("Reveal Crew");

            InitControls();
        }

        private void InitControls()
        {
            GrdPopBase.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ucSelectTimeSheet.Event_SheetNotExist += ucSelectTimeSheet_Event_SheetNotExist;
            ucSelectTimeSheet.Event_SelectTimeSheet += ucSelectTimeSheet_SelectTimeSheet;
            ucSelectTimeSheet.Event_CloseThis += ucSelectTimeSheet_Event_CloseThis;
            ucSelectTimeSheet.Event_SheetExist += ucSelectTimeSheet_Event_SheetExist;
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
            this.Frame.Navigate(typeof(Element.Reveal.Crew.Discipline.Progress.SubmitTimeProgress), ucSelectTimeSheet.SelectedTimeSheet);
            GrdPopBase.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            LoadStoryBoardSwitch();
            //Login.MasterPage.SetPageTitle(null);
            
            if(navigationParameter.GetType() != typeof(Boolean))
                _foremanPersonnelId = Convert.ToInt32(navigationParameter.ToString());
            Login.MasterPage.SetUserTitle(Login.UserAccount.UserName);
            Login.MasterPage.ShowUserStatus = true;

            Login.MasterPage.Loading(true, this);
            MainMenuDataSource menusource = new MainMenuDataSource();
            this.DefaultViewModel["Groups"] = menusource.DataSource.GetGroups("AllGroups");
           

            //Online 시에만.
            if (pageState == null)
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    LoadUpdateOffData();

                    if (!Login.NotShowSelectTimeSheet)
                        ShowSelectTimeSheetPop();
                }
            }
        }

        private void LoadStoryBoardSwitch()
        {
            _sbPopupON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbPopupON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(offDataUpdatePanelScale, 1, ANIMATION_SPEED));
            _sbPopupON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(offDataUpdatePanelScale, 1, ANIMATION_SPEED));

            _sbPopupOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbPopupOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(offDataUpdatePanelScale, 0, ANIMATION_SPEED));
            _sbPopupOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(offDataUpdatePanelScale, 0, ANIMATION_SPEED));
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
                Login.MasterPage.ShowBackButton = true;
                //this.Frame.Navigate(typeof(Discipline.Administrator.CrewSignIn), _foremanPersonnelId);
                switch (item.UniqueId)
                {
                    //case Lib.MainMenuList.FilloutITR:
                    //    this.Frame.Navigate(MainMenuDataSource.CurrentMenu, "1");
                    //    break;
                    //case Lib.MainMenuList.SubmitITR:
                    //    this.Frame.Navigate(MainMenuDataSource.CurrentMenu, "2");
                    //    break;
                    default :
                        this.Frame.Navigate(MainMenuDataSource.CurrentMenu, _foremanPersonnelId);
                        break;
                }                
                //this.Frame.Navigate(typeof(MainPage), _foremanPersonnelId);
                //this.Frame.Navigate(typeof(GroupedItemsPage));
            }
        }
        #endregion

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = sender as Image;
            Grid title = (img.Parent as Grid).FindName("grTitle") as Grid;
            img.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + Lib.ContentPath.DefaultDrawing));

            title.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void btnPopupClose_Click(object sender, RoutedEventArgs e)
        {
            disablePanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbPopupOFF.Begin();
        }

        private async Task<Dictionary<string, List<RevealProjectSvc.DailybrasssignDTO>>> GetSigninList()
        {
            Dictionary<string, List<RevealProjectSvc.DailybrasssignDTO>> retValue = new Dictionary<string, List<RevealProjectSvc.DailybrasssignDTO>>();
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                //Get Crew BrassSignIn List
                var stream = await helper.GetFileStream(Lib.ContentPath.OffModeUserFolder, Lib.ContentPath.BrassSignIn);
                var list = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealProjectSvc.DailybrasssignDTO>>(stream);
                retValue.Add(Lib.HashKey.Key_CrewBrassIn, list);

            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "GetGrouping-BrassSignIn");
                throw e;
            }

            return retValue;
        }

        private async Task<Dictionary<string, List<RevealProjectSvc.ToolboxsignDTO>>> GetToolboxinList()
        {
            Dictionary<string, List<RevealProjectSvc.ToolboxsignDTO>> retValue = new Dictionary<string, List<RevealProjectSvc.ToolboxsignDTO>>();
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                //Get ToolboxIn List
                var stream = await helper.GetFileStream(Lib.ContentPath.OffModeUserFolder, Lib.ContentPath.ToolBoxTalk);
                var list = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealProjectSvc.ToolboxsignDTO>>(stream);
                retValue.Add(Lib.HashKey.Key_ToolboxIn, list);
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "GetGrouping-ToolboxIn");
                throw e;
            }

            return retValue;
        }


        


        private async void LoadUpdateOffData()
        {
            bool flag = false;

            //BassSign File Access
            var optionBassSign = await GetSigninList();
            if (optionBassSign[Lib.HashKey.Key_CrewBrassIn] != null)
            {
                _brasssignList = optionBassSign[Lib.HashKey.Key_CrewBrassIn];

                if (_brasssignList != null && _brasssignList.Count > 0)
                {
                    grBrass.Visibility = Visibility.Visible;
                    flag = true;
                }
                else
                {
                    grBrass.Visibility = Visibility.Collapsed;
                }
            }

            //ToolBox Talk File Access
            var optionToolbox = await GetToolboxinList();

            if (optionToolbox[Lib.HashKey.Key_ToolboxIn] != null)
            {
                _toolboxList = optionToolbox[Lib.HashKey.Key_ToolboxIn];

                if (_toolboxList != null && _toolboxList.Count > 0)
                {
                    grToolBox.Visibility = Visibility.Visible;
                    flag = true;
                }
                else
                {
                    grToolBox.Visibility = Visibility.Collapsed;
                }
            }
            //Time & Progress File Access

            //brassin&out,toolbox talk, time&progress에 update할 offline data가 한개라도 있을시에만 offline data update popup open.
            if (flag)
            {
                disablePanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _sbPopupON.Begin();
            }
            else
            {
                disablePanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                _sbPopupOFF.Begin();
            }
        }

        private async void btnBrassSend_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            //Offline Data Update
            await new Lib.ServiceModel.ProjectModel().SaveDailybrasssignOffLine(_brasssignList);
            //기존파일 삭제
            List<RevealProjectSvc.DailybrasssignDTO> del_list = new List<RevealProjectSvc.DailybrasssignDTO>();
            await (new Lib.DataSource.BrassInOutDataSource()).SaveFileDayilyBrassSign(del_list, "BrassSignIn");
            
            //ReBind
            LoadUpdateOffData();

            Login.MasterPage.Loading(false, this);
        }

        private async void btnToolBoxSend_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            //Offline Data Update
            await new Lib.ServiceModel.ProjectModel().SaveToolBoxSignOffline(_toolboxList);

            //기존파일 삭제
            List<RevealProjectSvc.ToolboxsignDTO> del_list = new List<RevealProjectSvc.ToolboxsignDTO>();
            await (new Lib.DataSource.BrassInOutDataSource()).SaveFileToolbox(del_list, "ToolBoxTalk");
            //ReBind
            LoadUpdateOffData();

            Login.MasterPage.Loading(false, this);
        }

        private void btnProgressSend_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            //Offline Data Update

            //기존파일 삭제

            //ReBind
            LoadUpdateOffData();

            Login.MasterPage.Loading(false, this);
        }

    }
}
