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

namespace Element.Reveal.TrueTask.Discipline.Schedule.BuildCSU
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PSSRS : WinAppLibrary.Controls.LayoutAwarePage
    {

        private List<DataLibrary.DocumentDTO> _orgSafety;
        private List<DataLibrary.DocumentDTO> _trgSafety;

        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();

        private int _projectid, _fiwpid;
        private string _disciplineCode;
        private string _doctype;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbSortON, _sbSortOFF;

        public PSSRS()
        {
            this.InitializeComponent(); 
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;

            _fiwpid = Lib.IWPDataSource.selectedHydro;
            _doctype = DataLibrary.Utilities.SPCollectionName.PSSR;

            LoadStoryBoardSwitch();
            LoadTemplate();

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
        }

        private void LoadStoryBoardSwitch()
        {
            _sbSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 0, ANIMATION_SPEED));


            _sbSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 1, ANIMATION_SPEED));
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Lib.WizardDataSource.SetTargetMenuForCSU(DataLibrary.Utilities.DocEstablishedForCSU.PSSR);

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
                    foreach (DataLibrary.DocumentDTO dto in lvITRForms.SelectedItems)
                    {
                        if (_trgSafety.Where(x => x.SPCollectionID == dto.SPCollectionID).Count() > 0)
                            _trgSafety.Where(x => x.SPCollectionID == dto.SPCollectionID).FirstOrDefault().DTOStatus = (int)DataLibrary.Utilities.RowStatus.None;
                        else
                        {
                            dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                            _trgSafety.Add(dto);
                        }
                    }
                    lvFiwpITRForms.ItemsSource = null;
                    lvFiwpITRForms.ItemsSource = _trgSafety.Where(x => x.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete);
                }

                lvITRForms.ItemsSource = (from x in _orgSafety where !(from b in _trgSafety where b.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete select b.SPCollectionID).Contains(x.SPCollectionID) select x);
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
                    foreach (DataLibrary.DocumentDTO p in lvFiwpITRForms.SelectedItems)
                    {
                        for (int i = 0; i < _trgSafety.Count; i++)
                        {
                            if (p.SPCollectionID == _trgSafety[i].SPCollectionID)
                            {
                                if (_trgSafety[i].DTOStatus.Equals((int)DataLibrary.Utilities.RowStatus.New))
                                    _trgSafety.RemoveAt(i);
                                else if (_trgSafety[i].DTOStatus.Equals((int)DataLibrary.Utilities.RowStatus.None))
                                    _trgSafety[i].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Delete;
                                break;
                            }
                        }
                    }
                    lvFiwpITRForms.ItemsSource = null;
                    lvFiwpITRForms.ItemsSource = _trgSafety.Where(x => x.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete);

                    lvITRForms.ItemsSource = null;
                    lvITRForms.ItemsSource = (from x in _orgSafety where !(from b in _trgSafety where b.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete select b.SPCollectionID).Contains(x.SPCollectionID) select x);

                   
                }
                catch (Exception ex)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Remove to IWP", "There is a problem removing the selected item - Please try again later", "Error!");
                }             

                Login.MasterPage.Loading(false, this);
            }
        }


        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                List<DataLibrary.DocumentDTO> _result = new List<DataLibrary.DocumentDTO>();

                List<DataLibrary.DocumentDTO> _savedto = new List<DataLibrary.DocumentDTO>();
                foreach (DataLibrary.DocumentDTO _dto in _trgSafety)
                {
                    //_dto.CWPID = Lib.CWPDataSource.selectedCWP;
                    _dto.DocumentTypeLUID = Lib.DocType.PSSR;
                    _dto.FIWPID = _fiwpid;
                    _dto.PersonnelID = Login.UserAccount.PersonnelId;
                    //_dto.ProjectScheduleID = Lib.ScheduleDataSource.selectedSchedule;
                    _dto.ProjectID = _projectid;
                    _dto.DisciplineCode = _disciplineCode;
                    _dto.UpdatedBy = Login.UserAccount.UserName;
                    _dto.UpdatedDate = DateTime.Now;
                    
                    _savedto.Add(_dto);
                }

                DataLibrary.DocumentDTO document = new DataLibrary.DocumentDTO();
                document.DocumentTypeLUID = Lib.DocType.PSSR;
                //document.CWPID = Lib.CWPDataSource.selectedCWP;
                document.ProjectID = _projectid;
                document.DisciplineCode = _disciplineCode;
                //document.ProjectScheduleID = Lib.ScheduleDataSource.selectedSchedule;
                document.FIWPID = _fiwpid;
                document.UpdatedBy = Login.UserAccount.UserName;
                document.UpdatedDate = DateTime.Now;

                List<DataLibrary.DocumentDTO> listdocument = new List<DataLibrary.DocumentDTO>();
                listdocument.Add(document);
                
                List<DataLibrary.FiwpDTO> fiwpdto = new List<DataLibrary.FiwpDTO>();
                fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == _fiwpid).ToList();

                fiwpdto[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;


                if (!fiwpdto[0].DocEstablishedLUID.Equals(DataLibrary.Utilities.DocEstablishedForCSU.AssociatedDoc))
                    fiwpdto[0].DocEstablishedLUID = DataLibrary.Utilities.DocEstablishedForCSU.PSSR;
               
               
                _result = await (new Lib.ServiceModel.ProjectModel()).SaveSafetyDocumentForAssembleIWP(_savedto, fiwpdto, listdocument);

                //_result = await (new Lib.ServiceModel.ProjectModel()).SaveAssociatedDocumentForBuildCSU(listdocument, fiwpdto);

                _trgSafety = await (new Lib.ServiceModel.ProjectModel()).GetDocumentForFIWPByDocType(Lib.DocType.PSSR, _fiwpid, _projectid, _disciplineCode);
                lvFiwpITRForms.ItemsSource = _trgSafety;

                lvITRForms.ItemsSource = null;
                lvITRForms.ItemsSource = (from x in _orgSafety where !(from b in _trgSafety where b.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete select b.SPCollectionID).Contains(x.SPCollectionID) select x);


                Lib.WizardDataSource.SetTargetMenuForCSU(DataLibrary.Utilities.DocEstablishedForCSU.PSSR);

                if (Lib.WizardDataSource.NextMenu != null)
                    this.Frame.Navigate(Lib.WizardDataSource.NextMenu);

            }

            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "PSSR", "There was an error Save to PSSR. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);

        }

       

        #endregion

        #region "Private Method"

        private async void LoadTemplate()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();

            List<DataLibrary.CwpDTO> source = new List<DataLibrary.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    _orgSafety = await (new Lib.ServiceModel.ProjectModel()).GetUploadedFileInfoByProjectFileType(_projectid, DataLibrary.Utilities.FileType.ITR, DataLibrary.Utilities.FileCategory.DOCUMENT);
                    _trgSafety = await (new Lib.ServiceModel.ProjectModel()).GetDocumentForFIWPByDocType(Lib.DocType.PSSR, _fiwpid, _projectid, _disciplineCode);

                    List<DataLibrary.DocumentDTO> _neworgSafety = new List<DataLibrary.DocumentDTO>();
                    List<DataLibrary.DocumentDTO> _newtrgSafety = new List<DataLibrary.DocumentDTO>();
                    foreach (DataLibrary.DocumentDTO _dto in _orgSafety)
                    {
                        _dto.Description = _dto.Description.Replace(".pdf", "");
                        _neworgSafety.Add(_dto);
                    }

                    foreach (DataLibrary.DocumentDTO _dto in _trgSafety)
                    {
                        _dto.Description = _dto.Description.Replace(".pdf", "");
                        _newtrgSafety.Add(_dto);
                    }

                    _orgSafety = _neworgSafety;
                    _trgSafety = _newtrgSafety;

                    lvITRForms.ItemsSource = (from x in _orgSafety where !(from b in _trgSafety select b.SPCollectionID).Contains(x.SPCollectionID) select x);
                    lvFiwpITRForms.ItemsSource = _trgSafety;
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Load PSSR", "There was an error load Data. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        

        #endregion

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            lvITRForms.ItemsSource = _orgSafety.Where(x => x.Description.ToUpper().Contains((txtSearch.Text).ToUpper()) && !(from b in _trgSafety where b.DTOStatus != (int)DataLibrary.Utilities.RowStatus.Delete select b.SPCollectionID).Contains(x.SPCollectionID));
        }


        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransConsumableSort.ScaleY > 0)
                _sbSortOFF.Begin();
            else
                _sbSortON.Begin();
        }

        private void lvITRForms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lvFiwpITRForms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
