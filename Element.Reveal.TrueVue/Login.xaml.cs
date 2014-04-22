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
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using WinAppLibrary.Utilities;
using WinAppLibrary.ServiceModels;

namespace Element.Reveal.TrueVue
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        static RevealUserSvc.MobileLoginDTO _useraccount = new RevealUserSvc.MobileLoginDTO();
        public static RevealUserSvc.MobileLoginDTO UserAccount { get { return _useraccount; } }
        static WinAppLibrary.UI.LogMode _loginmode;
        public static WinAppLibrary.UI.LogMode LoginMode { get { return _loginmode; } }
        public static WinAppLibrary.UI.MasterPage MasterPage
        {
            get
            {
                var master = Window.Current.Content as WinAppLibrary.UI.MasterPage;
                if (master != null)
                    return master;
                else
                    return new WinAppLibrary.UI.MasterPage();
            }
        }

        public Login()
        {
            this.InitializeComponent();
            AppLogin.LoginStarted += AppLogin_LoginStarted;
            AppLogin.RegquestLoginInfo += AppLogin_RegquestLoginInfo;
            AppLogin.PageCompleted += Login_PageCompleted;
        }

        // -> Override OnNavigatedTo() and check QueryString
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MasterPage.ShowTopBanner = false;
            MasterPage.LoginSuccessful = false;
            AppLogin.OnNavigated(e);
        }

        #region "Event Handler"
        private async void AppLogin_LoginStarted(object sender, object e)
        {
            try
            {
                //first, check if Domain user or reveal                
                RevealUserSvc.MobileLoginDTO login = await (new Lib.ServiceModel.UserModel()).MobileGetUserLogin(sender.ToString(), e.ToString());

                if (login != null && login.PersonnelID > 0)
                {
                    _useraccount = login;
                    AppLogin.CompleteLogin(true);
                }
                else
                    AppLogin.CompleteLogin(false);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "LoginWithAccount");
                AppLogin.CompleteLogin(false);
            }
        }

        private async void AppLogin_RegquestLoginInfo(object sender, object e)
        {
            try
            {
                //first, check if Domain user or reveal                
                RevealUserSvc.LoginaccountDTO login = await (new Lib.ServiceModel.UserModel()).GetLoginaccountByPesonnelID(Convert.ToInt32(e.ToString()));

                if (login != null)
                {
                    AppLogin.SetLoignInfo(login.LoginName);
                }

            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Crew LoginWithAccount");
            }
        }

        private void Login_PageCompleted(object sender, object e)
        {
            _loginmode = AppLogin.LoginMode;
            MasterPage.LoginSuccessful = true;
            Lib.ProjectModuleSource.RequestUpdate = true;
            this.Frame.Navigate(typeof(GroupedItemsPage), _useraccount.PersonnelID);
        }
        #endregion
    }
}
