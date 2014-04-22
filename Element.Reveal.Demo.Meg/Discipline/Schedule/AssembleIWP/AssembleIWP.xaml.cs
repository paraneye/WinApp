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

namespace Element.Reveal.Meg.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssembleIWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        private Lib.ObjectParam _obj;
        private int _projectid, _moduleid;

        public AssembleIWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
            {
                btnScope.Visibility = Visibility.Visible;
                btnFieldEquipment.Visibility = Visibility.Visible;
                btnConsumableMaterial.Visibility = Visibility.Visible;
                btnInstallationTestRecord.Visibility = Visibility.Visible;
                btnSafetyDocument.Visibility = Visibility.Visible;
                btnLoadSitePlan.Visibility = Visibility.Visible;
                tbpageTitle.Text = "Assemble Intallation Work Package";
                lblSubTitle.Text = Lib.IWPDataSource.selectedIWPName;
            }
            else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
            {
                btnScope.Visibility = Visibility.Visible;
                btnFieldEquipment.Visibility = Visibility.Visible;
                btnConsumableMaterial.Visibility = Visibility.Collapsed;
                btnInstallationTestRecord.Visibility = Visibility.Visible;
                btnSafetyDocument.Visibility = Visibility.Collapsed;
                btnLoadSitePlan.Visibility = Visibility.Collapsed;
                lblSubTitle.Text = Lib.IWPDataSource.selectedSIWPName;
                tbpageTitle.Text = "Assemble System Intallation Work Package";
            }
            else
            {
                btnScope.Visibility = Visibility.Visible;
                btnFieldEquipment.Visibility = Visibility.Visible;
                btnConsumableMaterial.Visibility = Visibility.Visible;
                btnInstallationTestRecord.Visibility = Visibility.Visible;
                btnSafetyDocument.Visibility = Visibility.Visible;
                btnLoadSitePlan.Visibility = Visibility.Collapsed;
                lblSubTitle.Text = Lib.IWPDataSource.selectedHydroName;
                tbpageTitle.Text = "Assemble Hydro Test Work Package";
            }
        }

        #region button event
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectIWP));
        }

        private void btnScope_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.ScopeReport));
        }

        private void btnFieldEquipment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.FieldEquipment));
        }

        private void btnConsumableMaterial_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.ConsumableMaterial));
        }

        private void btnInstallationTestRecord_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.InstallationTestRecord));
        }

        private void btnSafetyDocument_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SafetyDocument));
        }

        private void btnLoadSitePlan_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.LoadSitePlan));
        }

        
        #endregion

       
    }
}
