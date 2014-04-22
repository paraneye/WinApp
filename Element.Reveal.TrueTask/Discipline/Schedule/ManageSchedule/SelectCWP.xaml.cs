using System;
using System.Collections;
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
using WinAppLibrary.ServiceModels;
using WinAppLibrary.Converters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.ManageSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectCWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        private int _projectid; private string _disciplineCode;

        public SelectCWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadCWP();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        private void gvCWP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cwp = e.AddedItems[0] as DataLibrary.CwpDTO;
            Lib.CWPDataSource.selectedCWP = cwp.CWPID;
            this.Frame.Navigate(typeof(Discipline.Schedule.ManageSchedule.SelectSchedule));
        }

        #endregion

        #region "Private Method"

        private async void LoadCWP()
        {
            Login.MasterPage.Loading(true, this);
            //StretchingPanel.Stretch(false);
            /*
            List<DataLibrary.CwpDTO> source = new List<DataLibrary.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    //bool login = WinAppLibrary.Utilities.SPDocument.IsLogin ? true :
                    //            await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                    //if (login)
                    //{
                    //var item = _iwpoption.SelectredIWP as RevealCommonSvc.ComboBoxDTO;
                    //if (item != null)
                    //{
                    //if (!WinAppLibrary.Utilities.SPDocument.IsLogin)
                    //    await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                    await _cwp.GetCWPsByProjectIDOnMode(_projectid, _disciplineCode);
                    source = _cwp.GetCWPs();
                    //}
                    //}
                    //else
                    //    WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "We couldn't sign in Sharepoint Server. Please check your authentication.");
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SelectCWP LoadCWP", "There was an error load CWP. Pleae contact administrator", "Error!");
            }

            this.DefaultViewModel["CWPs"] = source;
            this.gvCWP.SelectedItem = null;

            //this.gvViewType.SelectedIndex = 0;
            Login.MasterPage.Loading(false, this);*/
            List<DataLibrary.CwpDTO> source = new List<DataLibrary.CwpDTO>();
            try
            {
                await _cwp.GetCWPsByProjectIDOnMode(_projectid, _disciplineCode);
                source = _cwp.GetCWPs();
                cwplist.LoadCWP(source);
            }
            catch (Exception ex)
            {
            }
            Login.MasterPage.Loading(false, this);
        }


        private void cwplist_BackClick(object sender, object e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        private void cwplist_SelectionChanged(object sender, ListViewBase e)
        {
            Lib.CWPDataSource.selectedCWP = ((DataLibrary.CwpDTO)e.SelectedItem).CWPID;
            this.Frame.Navigate(typeof(SelectSchedule));
        }
        #endregion
    }
}
