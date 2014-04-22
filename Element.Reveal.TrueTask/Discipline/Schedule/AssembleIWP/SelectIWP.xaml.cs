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

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectIWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
        private int _projectid; private string _disciplineCode;

        public SelectIWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadIWP();

            tbpageTitle.Text = "Select Installation Work Package";
            
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.SelectSchedule));
        }

        private void gvIWP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var iwp = e.AddedItems[0] as DataLibrary.FiwpDTO;
            Lib.IWPDataSource.selectedIWP = iwp.FiwpID;
            Lib.IWPDataSource.selectedIWPName = iwp.FiwpName;

            //원래 Sign Off 여부로 위자드 설정
            Lib.IWPDataSource.isWizard = iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.APPROVER ? false : true;
            ////데모 전에 Sign off가 완료되지 않을 경우 대비
            //Lib.IWPDataSource.isWizard = iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.MOC ? false : true;
            
            //siwp인경우 schedule이 있는지 없는지
            bool isExistSch = Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP && iwp.ProjectScheduleID == 0 ? false : true;

            Lib.WizardDataSource.SetTargetMenu(iwp.DocEstablishedLUID, Lib.CommonDataSource.selPackageTypeLUID, isExistSch);

            //위자드 모드인데 공통 메뉴( Document/Report)인 경우 파라미터 추가
            if (Lib.WizardDataSource.NextMenu != null)
            {
                if (Lib.IWPDataSource.isWizard)
                {
                    //Document, Report 메뉴일 떄
                    if (iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.COVER)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SUMMARY);
                    else if (iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.SUMMARY)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SAFETY_CHECK);
                    else if (iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.SAFETY_CHECK)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SAFETY_FORM);
                    else if (iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.SAFETY_FORM)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.ITR);
                    else if (iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.CONSUMABLE)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SCAFFOLD_CHECK);
                    else if (iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.SCAFFOLD_CHECK)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SPEC);
                    else if (iwp.DocEstablishedLUID == DataLibrary.Utilities.AssembleStep.SPEC)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.MOC);
                    else
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
                }
                else
                {
                    this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
                }
            }
        }

        #endregion

        #region "Private Method"

        private async void LoadIWP()
        {
            Login.MasterPage.Loading(true, this);

            List<DataLibrary.FiwpDTO> source = new List<DataLibrary.FiwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _iwp.GetFiwpByCwpSchedulePackageTypeOnMode(Lib.CWPDataSource.selectedCWP, Lib.ScheduleDataSource.selectedSchedule, Lib.CommonDataSource.selPackageTypeLUID);
                    
                    source = _iwp.GetFiwpByProjectScheduleID();
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Load Installation Work Package", "There is a problem loading the Installation Work Package - Please try again later", "Loading Error");
            }

            this.DefaultViewModel["IWPs"] = source;
            this.gvIWP.SelectedItem = null;

            Login.MasterPage.Loading(false, this);
        }

        #endregion
    }
}
