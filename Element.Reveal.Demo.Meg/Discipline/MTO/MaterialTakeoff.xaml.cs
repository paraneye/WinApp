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
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.MTO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MaterialTakeoff : WinAppLibrary.Controls.LayoutAwarePage
    {
        private Lib.ObjectParam _obj;
        private int _projectid, _moduleid;

        public MaterialTakeoff()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            Load_Data();
        }

        private void Load_Data()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();


            Login.MasterPage.Loading(false, this);
        }

        #region button event
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.MTO.SelectCWP));
        }

        #endregion
    }
}
