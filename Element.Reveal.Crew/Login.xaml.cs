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

namespace Element.Reveal.Crew
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
                    WinAppLibrary.Utilities.Helper.LoginID = login.LoginName;
                    await (new Lib.DataSource.UserDataSource()).SaveFileLoginAccount(login, Lib.HashKey.Key_LoginAccount);
                    _useraccount = login;

                    #region Setting에 옵션 만들기
                    List<int> prjs = new List<int>();
                    List<RevealProjectSvc.ProjectDTO> projectList = await (new Lib.ServiceModel.ProjectModel()).GetAllProject();
                    List<RevealProjectSvc.ProjectmoduleDTO> moduleInProject = await (new Lib.ServiceModel.ProjectModel()).GetAllProjectModule();

                    List<WinAppLibrary.UI.DataProject> dataProjectList = new List<WinAppLibrary.UI.DataProject>();
                    foreach (RevealProjectSvc.ProjectDTO pDto in projectList)
                    {
                        if (pDto.ProjectName.Trim() != "")
                            dataProjectList.Add(new WinAppLibrary.UI.DataProject { ProjectId = pDto.ProjectID, ProjectName = pDto.ProjectName });
                    }

                    foreach (WinAppLibrary.UI.DataProject prj in dataProjectList)
                    {
                        List<RevealProjectSvc.ProjectmoduleDTO> add = (from m in moduleInProject where m.ProjectID == prj.ProjectId select m).ToList<RevealProjectSvc.ProjectmoduleDTO>();
                        foreach (RevealProjectSvc.ProjectmoduleDTO m in add)
                        {
                            if ((from mdl in prj.Modules where mdl.ModuleId == m.ModuleID select mdl).Count() == 0)
                                prj.Modules.Add(new WinAppLibrary.UI.DataModule { ModuleId = m.ModuleID, ModuleName = m.ModuleName });
                        }
                    }

                    Login.MasterPage.CurProjectId = Login.UserAccount.CurProjectID;
                    Login.MasterPage.CurModuleId = Login.UserAccount.CurModuleID;
                    Login.MasterPage.ProjectList = dataProjectList;
                    #endregion

                    AppLogin.CompleteLogin(true);
                }
                else
                    AppLogin.CompleteLogin(false);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Crew AppLogin_LoginStarted");
                AppLogin.CompleteLogin(false);
            }
        }

        private async void AppLogin_RegquestLoginInfo(object sender, object e)
        {
            try
            {
                //first, check if Domain user or reveal                
                RevealUserSvc.LoginaccountDTO login = await (new Lib.ServiceModel.UserModel()).GetLoginaccountByPesonnelID(Convert.ToInt32(e.ToString()));

                if (login != null && login.PersonnelID > 0)
                {
                    AppLogin.SetLoignInfo(login.LoginName);
                }
                else
                    AppLogin.CompleteLogin(false);
               
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Crew AppLogin_RegquestLoginInfo");
                AppLogin.CompleteLogin(false);
            }
        }

        private async void Login_PageCompleted(object sender, object e)
        {
            _loginmode = AppLogin.LoginMode;
            MasterPage.LoginSuccessful = true;



            if (_loginmode == WinAppLibrary.UI.LogMode.OffMode)
            {
                var options = await GetLoginUserInfo();
                if (options[Lib.HashKey.Key_LoginAccount] != null)
                {
                    _useraccount = options[Lib.HashKey.Key_LoginAccount];
                }
            }

            //this.Frame.Navigate(typeof(MainPage), _useraccount.PersonnelID);
            DateTime sdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 30, 00);
            DateTime edate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 00, 00);

            if (sdate.CompareTo(DateTime.Now) < 0 && edate.CompareTo(DateTime.Now) > 0)
            {
                this.Frame.Navigate(typeof(Discipline.Administrator.CrewBrassIn), _useraccount.PersonnelID);
            }
            else
            {
                this.Frame.Navigate(typeof(MainMenu), _useraccount.PersonnelID);
                //this.Frame.Navigate(typeof(Element.Reveal.Crew.Discipline.ITR.ITR_Master));
            }
        }

        private async Task<Dictionary<string, RevealUserSvc.MobileLoginDTO>> GetLoginUserInfo()
        {
            Dictionary<string, RevealUserSvc.MobileLoginDTO> retValue = new Dictionary<string, RevealUserSvc.MobileLoginDTO>();
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                //Get Crew BrassSignIn List
                var stream = await helper.GetFileStream(Lib.ContentPath.OffModeLoginFolder, Lib.ContentPath.LoginAccount);
                var list = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<RevealUserSvc.MobileLoginDTO>(stream);
                retValue.Add(Lib.HashKey.Key_LoginAccount, list);
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "LoginInfo");
                throw e;
            }

            return retValue;
        }


        #endregion
    }
}
