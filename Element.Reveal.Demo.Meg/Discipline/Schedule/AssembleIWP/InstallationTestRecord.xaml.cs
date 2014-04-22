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
using WinAppLibrary.Utilities;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallationTestRecord : WinAppLibrary.Controls.LayoutAwarePage
    {

        private List<RevealProjectSvc.QaqcformtemplateDTO> _orgITR;
        private List<RevealProjectSvc.FiwpqaqcDTO> _trgIWPITR;

        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();

        private int _projectid, _moduleid, _fiwpid, _formtype;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbSortON, _sbSortOFF;

        public InstallationTestRecord()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (Lib.IWPDataSource.isWizard)
            {
                btnNext.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnNext.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Visible;
            }

            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
            {
                _fiwpid = Lib.IWPDataSource.selectedIWP;
                _formtype = QAQCFormType.AssembleIWP;
            }
            else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
            {
                _fiwpid = Lib.IWPDataSource.selectedSIWP;
                _formtype = QAQCFormType.AssembleSIWP;
            }
            else
            {
                _fiwpid = Lib.IWPDataSource.selectedHydro;
                _formtype = QAQCFormType.AssembleHydroTest;
            }


            LoadStoryBoardSwitch();
            LoadTemplate();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.ITR, Lib.CommonDataSource.selPackageTypeLUID, true);

            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
        }

        private void btnAddIWP_Click(object sender, RoutedEventArgs e)
        {
            if (lvITRForms.SelectedItem != null)
            {                
                Login.MasterPage.Loading(true, this);

                if (lvITRForms.SelectedItems.Count > 0)
                {
                    foreach (RevealProjectSvc.QaqcformtemplateDTO dto in lvITRForms.SelectedItems)
                    {
                        if (_trgIWPITR.Where(x => x.QAQCFormTemplateID == dto.QAQCFormTemplateID).Count() > 0)
                            _trgIWPITR.Where(x => x.QAQCFormTemplateID == dto.QAQCFormTemplateID).FirstOrDefault().DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.None;
                        else
                        {
                            RevealProjectSvc.FiwpqaqcDTO newdto = new RevealProjectSvc.FiwpqaqcDTO();
                            newdto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                            newdto.FIWPID = _fiwpid;
                            newdto.CWPID = Lib.CWPDataSource.selectedCWP;
                            newdto.ProjectID = _projectid;
                            newdto.ModuleID = _moduleid;
                            newdto.QAQCFormTemplateID = dto.QAQCFormTemplateID;
                            newdto.QAQCFormTypeLUID = _formtype;
                            newdto.UpdatedBy = Login.UserAccount.UserName;
                            newdto.UpdatedDate = DateTime.Now;
                            newdto.QAQCFormTemplateName = dto.QAQCFormTemplateName;// +" " + dto.Description;
                            _trgIWPITR.Add(newdto);
                        }
                    }
                    KeepQTY();

                    lvFiwpITRForms.ItemsSource = null;
                    lvFiwpITRForms.ItemsSource = _trgIWPITR.Where(x => x.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Delete);
                }

                lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR where b.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Delete select b.QAQCFormTemplateID).Contains(x.QAQCFormTemplateID) select x);
                
                /*
                List<RevealProjectSvc.FiwpqaqcDTO> newlist = new List<RevealProjectSvc.FiwpqaqcDTO>();                
                
                foreach (RevealProjectSvc.QaqcformtemplateDTO dto in lvITRForms.SelectedItems)
                {
                    RevealProjectSvc.FiwpqaqcDTO newdto = new RevealProjectSvc.FiwpqaqcDTO();
                    newdto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                    newdto.FIWPID = _fiwpid;
                    newdto.CWPID = Lib.CWPDataSource.selectedCWP;
                    newdto.ProjectID = _projectid;
                    newdto.ModuleID = _moduleid;
                    newdto.QAQCFormTemplateID = dto.QAQCFormTemplateID;
                    newdto.QAQCFormTypeLUID = _formtype;
                    newdto.UpdatedBy = Login.UserAccount.UserName;
                    newdto.UpdatedDate = DateTime.Now;
                    newdto.QAQCFormTemplateName = dto.QAQCFormTemplateName + " " + dto.Description;
                    newlist.Add(newdto);
                }

                try
                {
                    var result = await (new Lib.ServiceModel.ProjectModel()).SaveFiwpqaqcForAssembleIWP(newlist);

                    WinAppLibrary.Utilities.Helper.SimpleMessage("Added successfully.", "Completed!");
                }
                catch (Exception ex)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Add to IWP", "There was an error Add to IWP. Pleae contact administrator", "Error!");
                }

                _trgIWPITR = await (new Lib.ServiceModel.ProjectModel()).GetFiwpqaqcByFIWP(Lib.IWPDataSource.selectedIWP);
                _orgITR = await (new Lib.ServiceModel.ProjectModel()).GetQaqcformtemplateByFiwp(Lib.IWPDataSource.selectedIWP, _projectid);

                lvFiwpITRForms.ItemsSource = null;
                lvFiwpITRForms.ItemsSource = _trgIWPITR;
                
                lvITRForms.ItemsSource = null;
                lvITRForms.ItemsSource = _orgITR;*/

                Login.MasterPage.Loading(false, this);
            }
            
        }

        private void btnRemoveIWP_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiwpITRForms.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                try
                {
                    foreach (RevealProjectSvc.FiwpqaqcDTO p in lvFiwpITRForms.SelectedItems)
                    {
                        for (int i = 0; i < _trgIWPITR.Count; i++)
                        {
                            if (p.QAQCFormTemplateID == _trgIWPITR[i].QAQCFormTemplateID)
                            {
                                if (_trgIWPITR[i].DTOStatus.Equals((int)WinAppLibrary.Utilities.RowStatus.New))
                                    _trgIWPITR.RemoveAt(i);
                                else if (_trgIWPITR[i].DTOStatus.Equals((int)WinAppLibrary.Utilities.RowStatus.None))
                                    _trgIWPITR[i].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                                break;
                            }
                        }
                    }
                    lvFiwpITRForms.ItemsSource = null;
                    lvFiwpITRForms.ItemsSource = _trgIWPITR.Where(x => x.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Delete);

                    lvITRForms.ItemsSource = null;
                    lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR where b.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Delete select b.QAQCFormTemplateID).Contains(x.QAQCFormTemplateID) select x);


                }
                catch (Exception ex)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Remove to IWP", "There was an error Remove to IWP. Pleae contact administrator", "Error!");
                }

                Login.MasterPage.Loading(false, this);
            }
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            try
            {

                List<RevealProjectSvc.FiwpqaqcDTO> _result = new List<RevealProjectSvc.FiwpqaqcDTO>();

                List<RevealProjectSvc.FiwpqaqcDTO> _savedto = new List<RevealProjectSvc.FiwpqaqcDTO>();
                foreach (RevealProjectSvc.FiwpqaqcDTO _dto in _trgIWPITR)
                {
                    _savedto.Add(_dto);
                }

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
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.ITR;
                }
                else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                {
                    if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.ITR))
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.ITR;
                }
                else
                {
                    if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.SafetyDocument))
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.ITR;
                }

                _result = await (new Lib.ServiceModel.ProjectModel()).SaveFiwpqaqcForAssembleIWP(_savedto, fiwpdto, listdocument);

                _trgIWPITR = await (new Lib.ServiceModel.ProjectModel()).GetFiwpqaqcByFIWP(_fiwpid);
                lvFiwpITRForms.ItemsSource = _trgIWPITR;

                lvITRForms.ItemsSource = null;
                lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR where b.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Delete select b.QAQCFormTemplateID).Contains(x.QAQCFormTemplateID) select x);

                if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                {
                    Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
                    await _iwp.GetFiwpByScheduleIDOnMode(Lib.ScheduleDataSource.selectedSchedule);
                    RevealProjectSvc.FiwpDTO iwpdto = new RevealProjectSvc.FiwpDTO();
                    iwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == _fiwpid).FirstOrDefault();

                    Lib.IWPDataSource.selectedSIWP = iwpdto.FiwpID;
                    Lib.IWPDataSource.selectedSIWPName = iwpdto.FiwpName;
                    Lib.IWPDataSource.isWizard = iwpdto.DocEstablishedLUID == WinAppLibrary.Utilities.DocEstablishedForApp.ITR ? false : true;

                }

                Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.ITR, Lib.CommonDataSource.selPackageTypeLUID, true);

                if (Lib.WizardDataSource.NextMenu != null)
                    this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
            }

            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Safety Document", "There was an error Save to SafetyDocument. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
           // _orgITR = await (new Lib.ServiceModel.ProjectModel()).GetQaqcformtemplateByFiwp(0, _projectid);

            //List<RevealProjectSvc.QaqcformtemplateDTO> tmpITR = new List<RevealProjectSvc.QaqcformtemplateDTO>();
            //tmpITR = (lvITRForms.Items as List<RevealProjectSvc.QaqcformtemplateDTO>).Where(x => x.QAQCFormTemplateName.ToUpper().Contains((txtSearch.Text).ToUpper())
            //    || (x.Description != null ? x.Description.ToUpper() : string.Empty).Contains((txtSearch.Text).ToUpper())).ToList();

            //lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR select b.QAQCFormTemplateID).Contains(x.QAQCFormTemplateID) select x);

            lvITRForms.ItemsSource = _orgITR.Where(x => x.QAQCFormTemplateName.ToUpper().Contains((txtSearch.Text).ToUpper()) && !(from b in _trgIWPITR where b.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Delete select b.QAQCFormTemplateID).Contains(x.QAQCFormTemplateID));
        }

        private void KeepQTY()
        {
            foreach(RevealProjectSvc.FiwpqaqcDTO dto in lvFiwpITRForms.Items){
                foreach(RevealProjectSvc.FiwpqaqcDTO tdto in _trgIWPITR){
                    if (tdto.QAQCFormTemplateName == dto.QAQCFormTemplateName)
                    {
                        tdto.Qty = dto.Qty;
                    }
                }
            }
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransConsumableSort.ScaleY > 0)
                _sbSortOFF.Begin();
            else
                _sbSortON.Begin();
        }

        #endregion

        #region "Private Method"

        private void LoadStoryBoardSwitch()
        {
            _sbSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 0, ANIMATION_SPEED));


            _sbSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 1, ANIMATION_SPEED));
        }

        private async void LoadTemplate()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();
            

            List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    _orgITR = await (new Lib.ServiceModel.ProjectModel()).GetQaqcformtemplateByFiwp(0, _formtype, _projectid);
                    _trgIWPITR = await (new Lib.ServiceModel.ProjectModel()).GetFiwpqaqcByFIWP(_fiwpid);

                    lvITRForms.ItemsSource = (from x in _orgITR where !(from b in _trgIWPITR select b.QAQCFormTemplateID).Contains(x.QAQCFormTemplateID) select x);
                    lvFiwpITRForms.ItemsSource = _trgIWPITR;
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "ComponentGrouping LoadGrouping", "There was an error load Grouping. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }


        #endregion

        private void txtQty_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            e.Handled = !Char.IsDigit(Convert.ToChar(e.Key));
        }
    }
}
