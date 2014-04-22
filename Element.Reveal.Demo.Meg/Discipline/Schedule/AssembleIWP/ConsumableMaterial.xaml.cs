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
    public sealed partial class ConsumableMaterial : WinAppLibrary.Controls.LayoutAwarePage
    {
        private List<RevealProjectSvc.LibconsumableDTO> _orgConsumable;
        private List<RevealProjectSvc.LibconsumableDTO> _libConsumable;
        private List<RevealProjectSvc.FiwpmaterialDTO> _trgFiwpmaterial;
        List<RevealProjectSvc.FiwpmaterialDTO> _delFiwpmaterial = new List<RevealProjectSvc.FiwpmaterialDTO>();
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();

        private int _projectid, _moduleid, _fiwpid;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbSortON, _sbSortOFF;
        //
        public ConsumableMaterial()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
                _fiwpid = Lib.IWPDataSource.selectedIWP;
            else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                _fiwpid = Lib.IWPDataSource.selectedSIWP;
            else
                _fiwpid = Lib.IWPDataSource.selectedHydro; 

            LoadStoryBoardSwitch();
            LoadLibrary();

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
            Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.ConsumableMaterial, Lib.CommonDataSource.selPackageTypeLUID, true);

            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
        }

        private void btnAddIWP_Click(object sender, RoutedEventArgs e)
        {
            if (lvConsumable.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                RevealProjectSvc.FiwpmaterialDTO newdto = new RevealProjectSvc.FiwpmaterialDTO();

                foreach (RevealProjectSvc.LibconsumableDTO dto in lvConsumable.SelectedItems)
                {
                    
                    newdto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                    newdto.Description = dto.Description;
                    newdto.UOMLUID = dto.UOMLUID;
                    newdto.EstMHLibID = dto.LibConsumableID;
                    newdto.PartNo = dto.PartNumber;
                    newdto.FIWPID = _fiwpid;
                    newdto.UpdatedDate = DateTime.Now;

                    if (newdto.PromisedDeliveryDate < WinAppLibrary.Utilities.Helper.DateTimeMinValue)
                        newdto.PromisedDeliveryDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;

                    if (newdto.RevisedDeliveryDate < WinAppLibrary.Utilities.Helper.DateTimeMinValue)
                        newdto.RevisedDeliveryDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;

                    if (newdto.DelieveryDate < WinAppLibrary.Utilities.Helper.DateTimeMinValue)
                        newdto.DelieveryDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;

                    if (newdto.ETADate < WinAppLibrary.Utilities.Helper.DateTimeMinValue)
                        newdto.ETADate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;

                    if (newdto.ROSDate < WinAppLibrary.Utilities.Helper.DateTimeMinValue)
                        newdto.ROSDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;

                    _trgFiwpmaterial.Add(newdto);
                }

                lvFiwpMaterial.ItemsSource = null;
                lvFiwpMaterial.ItemsSource = _trgFiwpmaterial;
                lvFiwpMaterial.SelectedValue = newdto;

                InitLibConsumable();

                Login.MasterPage.Loading(false, this);
            }
            
        }

        private void btnRemoveIWP_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiwpMaterial.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                foreach (RevealProjectSvc.FiwpmaterialDTO dto in lvFiwpMaterial.SelectedItems)
                {
                    if (dto.FIWPMaterialID > 0)
                    {
                        dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                        _delFiwpmaterial.Add(dto);
                    }
                    _trgFiwpmaterial.Remove(dto);
                }

                lvFiwpMaterial.ItemsSource = null;
                lvFiwpMaterial.ItemsSource = _trgFiwpmaterial;

                InitLibConsumable();

                Login.MasterPage.Loading(false, this);
            }
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            List<RevealProjectSvc.FiwpmaterialDTO> updatelist = new List<RevealProjectSvc.FiwpmaterialDTO>();

           
            try
            {
                updatelist.AddRange(_trgFiwpmaterial.Where(x => x.DTOStatus == (int)WinAppLibrary.Utilities.RowStatus.Update));
                updatelist.AddRange(_trgFiwpmaterial.Where(x => x.DTOStatus == (int)WinAppLibrary.Utilities.RowStatus.New));
                updatelist.AddRange(_delFiwpmaterial);


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
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.ConsumableMaterial;
                }
                else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                {
                    if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.ITR))
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.ConsumableMaterial;
                }
                else
                {
                    if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.SafetyDocument))
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.ConsumableMaterial;
                }


                List<RevealProjectSvc.FiwpmaterialDTO> result = await (new Lib.ServiceModel.ProjectModel()).SaveFiwpMaterialForAssembleIWP(updatelist, fiwpdto, listdocument);

                Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.ConsumableMaterial, Lib.CommonDataSource.selPackageTypeLUID, true);

                if (Lib.WizardDataSource.NextMenu != null)
                    this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Save to IWP", "There was an error Save to IWP. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }
        private void lvConsumable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvConsumable.SelectedItem != null)
            {
                txtR_Description.Text = (lvConsumable.SelectedItem as RevealProjectSvc.LibconsumableDTO).Description;
                txtR_Type.Text = (lvConsumable.SelectedItem as RevealProjectSvc.LibconsumableDTO).Type;
                txtR_Vendor.Text = (lvConsumable.SelectedItem as RevealProjectSvc.LibconsumableDTO).Vendor;
                txtR_UOM.Text = (lvConsumable.SelectedItem as RevealProjectSvc.LibconsumableDTO).UOM;
                txtR_Manhour.Text = (lvConsumable.SelectedItem as RevealProjectSvc.LibconsumableDTO).Manhours.ToString();
            }
            else
            {
                txtR_Description.Text = string.Empty;
                txtR_Type.Text = string.Empty;
                txtR_Vendor.Text = string.Empty;
                txtR_UOM.Text = string.Empty;
                txtR_Manhour.Text = string.Empty;
            }
            //trgFiwpequip
        }

        #endregion

        #region "Private Method"

        private void InitLibConsumable()
        {
            _libConsumable = new List<RevealProjectSvc.LibconsumableDTO>();

            foreach (RevealProjectSvc.LibconsumableDTO dto in _orgConsumable)
            {
                if (_trgFiwpmaterial.Where(x => x.EstMHLibID == dto.LibConsumableID).Count() < 1)
                    _libConsumable.Add(dto);
            }

            lvConsumable.ItemsSource = null;
            lvConsumable.ItemsSource = _libConsumable;
        }
        private async void LoadLibrary()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();
            

            List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    _orgConsumable = await (new Lib.ServiceModel.ProjectModel()).GetLibconsumableAll();

                    _trgFiwpmaterial = await (new Lib.ServiceModel.ProjectModel()).GetFiwpMaterialByFIWP(_fiwpid, _projectid, _moduleid);

                    InitLibConsumable();

                    lvFiwpMaterial.ItemsSource = _trgFiwpmaterial;

                    await _commonsource.GetUOMOnMode();
                    cbUOM.ItemsSource = _commonsource.GetUOM();
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

        private void lvFiwpMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvFiwpMaterial.SelectedItem != null)
            {
                txtA_Description.Text = (lvFiwpMaterial.SelectedItem as RevealProjectSvc.FiwpmaterialDTO).Description == null ? "" : (lvFiwpMaterial.SelectedItem as RevealProjectSvc.FiwpmaterialDTO).Description;
                foreach (RevealCommonSvc.ComboBoxDTO dto in cbUOM.Items)
                {
                    if ((lvFiwpMaterial.SelectedItem as RevealProjectSvc.FiwpmaterialDTO).UOMLUID == dto.DataID)
                    {
                        cbUOM.SelectedValue = dto;
                        break;
                    }
                }
                C1nboxQty.Value = Convert.ToDouble((lvFiwpMaterial.SelectedItem as RevealProjectSvc.FiwpmaterialDTO).QtySum);
            }
            else
            {
                txtA_Description.Text = string.Empty;
                C1nboxQty.Value = 0;

                foreach (RevealCommonSvc.ComboBoxDTO dto in cbUOM.Items)
                {
                    cbUOM.SelectedValue = dto;
                    break;
                }
            }
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _libConsumable = await (new Lib.ServiceModel.ProjectModel()).GetLibconsumableAll();
            _libConsumable = _libConsumable.Where(x => x.PartNumber.ToUpper().Contains((txtSearch.Text).ToUpper())).ToList();
            lvConsumable.ItemsSource = _libConsumable;
        }

        private void lvSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvSort.SelectedItem != null)
            {
                string sortby = (lvSort.SelectedItem as TextBlock).Text;

                switch (sortby)
                {
                    case "Name":
                        _libConsumable = _libConsumable.OrderBy(x => x.PartNumber).ToList();
                        break;
                    case "Type":
                        _libConsumable = _libConsumable.OrderBy(x => x.Type).ToList();
                        break;
                    case "Vender":
                        _libConsumable = _libConsumable.OrderBy(x => x.Vendor).ToList();
                        break;
                }
                lvConsumable.ItemsSource = _libConsumable;
            }
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransConsumableSort.ScaleY > 0)
                _sbSortOFF.Begin();
            else
                _sbSortON.Begin();
        }

        private void C1nboxQty_ValueChanged(object sender, C1.Xaml.PropertyChangedEventArgs<double> e)
        {
            if (lvFiwpMaterial.SelectedItem != null)
            {
                foreach (RevealProjectSvc.FiwpmaterialDTO dto in _trgFiwpmaterial)
                {
                    if (dto == lvFiwpMaterial.SelectedItem as RevealProjectSvc.FiwpmaterialDTO)
                    {
                        if ((lvFiwpMaterial.SelectedItem as RevealProjectSvc.FiwpmaterialDTO).FIWPMaterialID > 0)
                        {
                            dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                            dto.QtySum = Convert.ToDecimal(C1nboxQty.Value);
                        }
                        else
                        {
                            dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                            dto.Qty = Convert.ToDecimal(C1nboxQty.Value);
                            dto.QtySum = Convert.ToDecimal(C1nboxQty.Value);
                        }

                       // dto.Qty = Convert.ToDecimal(C1nboxQty.Value);
                        dto.UpdatedDate = DateTime.Now;
                    }
                }
            }
        }

        private void cbUOM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvFiwpMaterial.SelectedItem != null)
            {
                foreach (RevealProjectSvc.FiwpmaterialDTO dto in _trgFiwpmaterial)
                {
                    if (dto == lvFiwpMaterial.SelectedItem as RevealProjectSvc.FiwpmaterialDTO)
                    {
                        if ((lvFiwpMaterial.SelectedItem as RevealProjectSvc.FiwpmaterialDTO).FIWPMaterialID > 0)
                            dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                        else
                            dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;

                        dto.UOMLUID = (cbUOM.SelectedValue as RevealCommonSvc.ComboBoxDTO).DataID;
                        dto.UpdatedDate = DateTime.Now;
                    }
                }
            }
        }
    }
}
