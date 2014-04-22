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
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class ScopeReport : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid, _fiwpid;

        public ScopeReport()
        {
            this.InitializeComponent();
        }

        
        
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (Lib.IWPDataSource.isWizard)
            {
                btnNext.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnSave.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                btnNext.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnSave.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }     

            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            if(Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
                _fiwpid = Lib.IWPDataSource.selectedIWP;
            else if(Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                _fiwpid = Lib.IWPDataSource.selectedSIWP;
            else
                _fiwpid = Lib.IWPDataSource.selectedHydro; 

            Load_Data();
        }

        private async void Load_Data()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();

            List<RevealProjectSvc.FiwpwfpDTO> result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpwfpByFiwp(_fiwpid, _projectid, _moduleid);

            try
            {
                if (result.Count > 0)
                {
                    txtScope.Text = result.FirstOrDefault().Scope.ToString(); 
                    txtSafetyEquipmentTraining.Text = result.FirstOrDefault().SafetyTrain.ToString();
                    txtLessonsLearned.Text = result.FirstOrDefault().Lesson.ToString();
                }
                 
            }
            catch (Exception e)
            {                
            }
            Login.MasterPage.Loading(false, this);
        }

        

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.Scope, Lib.CommonDataSource.selPackageTypeLUID, true);

            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (txtScope.Text.Trim().Length > 0)
            {
                Login.MasterPage.Loading(true, this);

                try
                {

                    List<RevealProjectSvc.FiwpwfpDTO> updatelist = new List<RevealProjectSvc.FiwpwfpDTO>();


                    List<RevealProjectSvc.FiwpwfpDTO> result = await (new Lib.ServiceModel.ProjectModel()).GetFiwpwfpByFiwp(_fiwpid, _projectid, _moduleid);

                    RevealProjectSvc.FiwpwfpDTO dto = new RevealProjectSvc.FiwpwfpDTO();

                    if (result.Count > 0)
                    {
                        dto.FIWPWFPID = result.FirstOrDefault().FIWPWFPID;
                        dto.FIWPID = result.FirstOrDefault().FIWPID;
                        dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                    }
                    else
                    {
                        dto.FIWPWFPID = 0;
                        dto.FIWPID = _fiwpid;
                        dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                    }
                    
                    dto.Scope = txtScope.Text;
                    dto.Lesson = txtLessonsLearned.Text;
                    dto.SafetyTrain = txtSafetyEquipmentTraining.Text;
                    dto.UpdatedBy = 0;
                    dto.UpdatedDate = DateTime.Now;                   

                    updatelist.Add(dto);

                    RevealProjectSvc.DocumentDTO document = new RevealProjectSvc.DocumentDTO();
                    document.DocumentTypeLUID = Lib.DocType.ModelView;
                    document.CWPID = Lib.CWPDataSource.selectedCWP;
                    document.ProjectID = _projectid;
                    document.ModuleID = _moduleid;
                    document.ProjectScheduleID = Lib.ScheduleDataSource.selectedSchedule;
                    document.FIWPID = _fiwpid;
                    document.UpdatedBy = Login.UserAccount.UserName;
                    document.UpdatedDate = DateTime.Now;

                    List<RevealProjectSvc.DocumentDTO> listdocument = new List<RevealProjectSvc.DocumentDTO>();
                    listdocument.Add(document);

                    List<RevealProjectSvc.FiwpDTO> fiwpdto = new List<RevealProjectSvc.FiwpDTO>();
                    fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == _fiwpid).ToList();

                    fiwpdto[0].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                  
                    if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
                    {
                        if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage))
                            fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.Scope;
                    }
                    else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                    {
                        if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.ITR))
                            fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.Scope;
                    }
                    else
                    {
                        if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.SafetyDocument))
                            fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.Scope;
                    }

                    var rst = await (new Lib.ServiceModel.ProjectModel()).SaveFiwpwfpForAssembleIWP(updatelist, fiwpdto, listdocument);


                    Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.Scope, Lib.CommonDataSource.selPackageTypeLUID, true);

                    if (Lib.WizardDataSource.NextMenu != null)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu);

                    /*
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Saved successfully.", "Completed!");

                    Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.Scope);

                    if (Lib.WizardDataSource.NextMenu != null)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
                    */
                }
                catch (Exception ex)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Save", "There was an error. Pleae contact administrator", "Error!");
                }

                Login.MasterPage.Loading(false, this);
            }
        }
    }
}
