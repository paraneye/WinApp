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

namespace Element.Reveal.Meg
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
        public static bool NotShowSelectTimeSheet { get; set; }

        public static WinAppLibrary.UI.TrueTaskMasterPage MasterPage
        {
            get
            {
                var master = Window.Current.Content as WinAppLibrary.UI.TrueTaskMasterPage;
                if (master != null)
                    return master;
                else
                    return new WinAppLibrary.UI.TrueTaskMasterPage();
            }
        }

        public Login()
        {
            this.InitializeComponent();
            AppLogin.LoginStarted += AppLogin_LoginStarted;
            AppLogin.RegquestLoginInfo += AppLogin_RegquestLoginInfo;
            AppLogin.PageCompleted += Login_PageCompleted;

            Login.MasterPage.SetPageTitle("");
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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

                    //Setting에 옵션 만들기 - 시작
                    List<int> prjs = new List<int>();
                    List<RevealProjectSvc.ProjectDTO> projectList = await (new Lib.ServiceModel.ProjectModel()).GetAllProject();
                    List<RevealProjectSvc.ProjectmoduleDTO> moduleInProject = await (new Lib.ServiceModel.ProjectModel()).GetAllProjectModule();

                    List<WinAppLibrary.UI.DataProject> dataProjectList = new List<WinAppLibrary.UI.DataProject>();
                    foreach (RevealProjectSvc.ProjectDTO pDto in projectList)
                    {
                        if(pDto.ProjectName.Trim() != "")
                            dataProjectList.Add(new WinAppLibrary.UI.DataProject { ProjectId = pDto.ProjectID, ProjectName = pDto.ProjectName });
                    }

                    foreach (WinAppLibrary.UI.DataProject prj in dataProjectList)
                    {
                        List<RevealProjectSvc.ProjectmoduleDTO> add = (from m in moduleInProject where m.ProjectID == prj.ProjectId select m).ToList<RevealProjectSvc.ProjectmoduleDTO>();
                        foreach (RevealProjectSvc.ProjectmoduleDTO m in add)
                        {
                            if((from mdl in prj.Modules where mdl.ModuleId == m.ModuleID select mdl).Count() == 0)
                                prj.Modules.Add(new WinAppLibrary.UI.DataModule { ModuleId = m.ModuleID, ModuleName = m.ModuleName });
                        }
                    }

                    Login.MasterPage.CurProjectId = Login.UserAccount.CurProjectID;
                    Login.MasterPage.CurModuleId = Login.UserAccount.CurModuleID;
                    Login.MasterPage.ProjectList = dataProjectList;
                    //Setting에 옵션 만들기 - 끝

                    AppLogin.CompleteLogin(true);
                }
                else
                    AppLogin.CompleteLogin(false);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Sigma LoginWithAccount");
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
            //Lib.ProjectModuleSource.RequestUpdate = true;
            this.Frame.Navigate(typeof(MainMenu), _useraccount.PersonnelID);
        }
        #endregion
    }
}
