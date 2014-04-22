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

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConsumableMaterial : WinAppLibrary.Controls.LayoutAwarePage
    {
        private List<DataLibrary.LibconsumableDTO> _orgConsumable = new List<DataLibrary.LibconsumableDTO>();
        private List<DataLibrary.LibconsumableDTO> _libConsumable;
        private List<DataLibrary.FiwpmaterialDTO> _trgFiwpmaterial = new List<DataLibrary.FiwpmaterialDTO>();
        List<DataLibrary.FiwpmaterialDTO> _delFiwpmaterial = new List<DataLibrary.FiwpmaterialDTO>();
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();

        private int _projectid, _fiwpid;
        private string _disciplineCode;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbSortON, _sbSortOFF;
        //
        public ConsumableMaterial()
        {
            this.InitializeComponent();

            this.C1nboxQty.Value = 1;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;

            _fiwpid = Lib.IWPDataSource.selectedIWP;

            LoadStoryBoardSwitch();
            LoadLibrary();

            Lib.WizardDataSource.SetTargetMenu(DataLibrary.Utilities.AssembleStep.CONSUMABLE, Lib.CommonDataSource.selPackageTypeLUID, true);


            this.ButtonBar.CurrentMenu = DataLibrary.Utilities.AssembleStep.CONSUMABLE;
            this.ButtonBar.Load();
        }

        private void LoadStoryBoardSwitch()
        {
            _sbSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 0, ANIMATION_SPEED));


            _sbSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 1, ANIMATION_SPEED));

            //////펼치는 방식 변경 : 테스트
            ////_sbSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            ////_sbSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransConsumableSort, 1, ANIMATION_SPEED));
            ////_sbSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 1, ANIMATION_SPEED));

            ////_sbSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            ////_sbSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScaleTransConsumableSort, 0, ANIMATION_SPEED));
            ////_sbSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransConsumableSort, 0, ANIMATION_SPEED));

        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            
            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
        }


        private void Button_Clicked(object sender, object e)
        {
            string tag = e != null ? e.ToString() : string.Empty;

            switch (tag)
            {
                case "Add":
                    AddItem();
                    break;
                case "Remove":
                    RemoveItem();
                    break;
                case "Save":
                    SaveDocument();
                    break;
                case "Next":
                    SaveDocument();
                    break;
            }
        }


        private void AddItem()
        {
            if (lvConsumable.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                DataLibrary.FiwpmaterialDTO newdto = new DataLibrary.FiwpmaterialDTO();

                foreach (DataLibrary.LibconsumableDTO dto in lvConsumable.SelectedItems)
                {
                    
                    newdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                    newdto.Description = dto.Description;
                    newdto.UOMLUID = dto.UOMLUID;
                    newdto.EstMHLibID = dto.LibConsumableID;
                    newdto.PartNo = dto.PartNumber;
                    newdto.FIWPID = _fiwpid;
                    newdto.UpdatedBy = Login.UserAccount.PersonnelId;
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

        private void RemoveItem()
        {
            if (lvFiwpMaterial.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                foreach (DataLibrary.FiwpmaterialDTO dto in lvFiwpMaterial.SelectedItems)
                {
                    if (dto.FIWPMaterialID > 0)
                    {
                        dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Delete;
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

        private async void SaveDocument()
        {
            Login.MasterPage.Loading(true, this);

            List<DataLibrary.FiwpmaterialDTO> updatelist = new List<DataLibrary.FiwpmaterialDTO>();

           
            try
            {
                if (_trgFiwpmaterial == null)
                {
                    Login.MasterPage.Loading(false, this);
                    return;
                }

                updatelist.AddRange(_trgFiwpmaterial.Where(x => x.DTOStatus == (int)DataLibrary.Utilities.RowStatus.Update));
                updatelist.AddRange(_trgFiwpmaterial.Where(x => x.DTOStatus == (int)DataLibrary.Utilities.RowStatus.New));
                updatelist.AddRange(_delFiwpmaterial);


                DataLibrary.DocumentDTO document = new DataLibrary.DocumentDTO();
                document.DocumentTypeLUID = Lib.DocType.ModelView;
                document.CWPID = Lib.CWPDataSource.selectedCWP;
                document.ProjectID = _projectid;
                document.DisciplineCode = _disciplineCode;
                document.ProjectScheduleID = Lib.ScheduleDataSource.selectedSchedule;
                document.FIWPID = _fiwpid;
                document.UpdatedBy = Login.UserAccount.UserName;
                document.UpdatedDate = DateTime.Now;

                List<DataLibrary.DocumentDTO> listdocument = new List<DataLibrary.DocumentDTO>();
                listdocument.Add(document);

                List<DataLibrary.FiwpDTO> fiwpdto = new List<DataLibrary.FiwpDTO>();
                if (Lib.IWPDataSource.iwplist == null)
                {
                    Login.MasterPage.Loading(false, this);
                    return;
                }
                fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == _fiwpid).ToList();

                fiwpdto[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;

                //현재 단계 저장
                if (!fiwpdto[0].DocEstablishedLUID.Equals(DataLibrary.Utilities.AssembleStep.APPROVER))
                    fiwpdto[0].DocEstablishedLUID = DataLibrary.Utilities.AssembleStep.CONSUMABLE;

                List<DataLibrary.FiwpmaterialDTO> result;
                result = await(new Lib.ServiceModel.ProjectModel()).SaveFiwpMaterialForAssembleIWP(updatelist, fiwpdto, Login.UserAccount.PersonnelId);

                Lib.WizardDataSource.SetTargetMenu(DataLibrary.Utilities.AssembleStep.CONSUMABLE, Lib.CommonDataSource.selPackageTypeLUID, true);

                if (Lib.WizardDataSource.NextMenu != null)
                    this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SCAFFOLD_CHECK);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Save consumable material", "There is a problem saving the selected item - Please try again later", "Error");
            }

            Login.MasterPage.Loading(false, this);
        }
        private void lvConsumable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvConsumable.SelectedItem != null)
            {
                txtR_Description.Text = !string.IsNullOrEmpty((lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).Description) ? (lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).Description : " ";
                txtR_Type.Text = !string.IsNullOrEmpty((lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).Type) ? (lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).Type : " ";
                txtR_Vendor.Text = !string.IsNullOrEmpty((lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).Vendor) ? (lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).Vendor : " ";
                txtR_UOM.Text = !string.IsNullOrEmpty((lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).UOM) ? (lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).UOM : " ";
                //txtR_Manhour.Text = (lvConsumable.SelectedItem as DataLibrary.LibconsumableDTO).Manhours.ToString();
            }
            else
            {
                txtR_Description.Text = string.Empty;
                txtR_Type.Text = string.Empty;
                txtR_Vendor.Text = string.Empty;
                txtR_UOM.Text = string.Empty;
                //txtR_Manhour.Text = string.Empty;
            }
            //trgFiwpequip
        }

        #endregion

        #region "Private Method"

        private void InitLibConsumable()
        {
            _libConsumable = new List<DataLibrary.LibconsumableDTO>();

            if (_orgConsumable == null || _trgFiwpmaterial == null)
                return;

            foreach (DataLibrary.LibconsumableDTO dto in _orgConsumable)
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
            

            List<DataLibrary.CwpDTO> source = new List<DataLibrary.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    List<DataLibrary.LibconsumableDTO> result;
                    result = await (new Lib.ServiceModel.ProjectModel()).GetLibconsumableAll();
                    if (result != null)
                        _orgConsumable = result;

                    List<DataLibrary.FiwpmaterialDTO> trgresult;
                    trgresult = await (new Lib.ServiceModel.ProjectModel()).GetFiwpMaterialByFIWP(_fiwpid, _projectid, _disciplineCode);
                    if (trgresult != null)
                        _trgFiwpmaterial = trgresult;

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
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Load consumable material", "There is a problem loading consumable material data - Please try again later", "Loading Error");
            }

            Login.MasterPage.Loading(false, this);
        }



        #endregion

        private void lvFiwpMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvFiwpMaterial.SelectedItem != null)
            {
                txtA_Description.Text = (lvFiwpMaterial.SelectedItem as DataLibrary.FiwpmaterialDTO).Description == null ? "" : (lvFiwpMaterial.SelectedItem as DataLibrary.FiwpmaterialDTO).Description;
                foreach (DataLibrary.ComboCodeBoxDTO dto in cbUOM.Items)
                {
                    //기본값 세팅 필요?
                    if ((lvFiwpMaterial.SelectedItem as DataLibrary.FiwpmaterialDTO).UOMLUID == dto.DataID)
                    {
                        cbUOM.SelectedValue = dto;
                        break;
                    }
                }
                C1nboxQty.Value = Convert.ToDouble((lvFiwpMaterial.SelectedItem as DataLibrary.FiwpmaterialDTO).Qty);
            }
            else
            {
                txtA_Description.Text = string.Empty;
                C1nboxQty.Value = 1;

                //기본값 세팅 필요?
                foreach (DataLibrary.ComboCodeBoxDTO dto in cbUOM.Items)
                {
                    cbUOM.SelectedValue = dto;
                    break;
                }
            }
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _libConsumable = await (new Lib.ServiceModel.ProjectModel()).GetLibconsumableAll();
            if (_libConsumable == null)
                return;

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
            {
                _sbSortOFF.Begin();
                btnSort.Content = "∨";
            }
            else
            {
                _sbSortON.Begin();
                btnSort.Content = "∧";
            }
        }

        private void C1nboxQty_ValueChanged(object sender, C1.Xaml.PropertyChangedEventArgs<double> e)
        {
            if (lvFiwpMaterial.SelectedItem != null)
            {
                foreach (DataLibrary.FiwpmaterialDTO dto in _trgFiwpmaterial)
                {
                    if (dto == lvFiwpMaterial.SelectedItem as DataLibrary.FiwpmaterialDTO)
                    {
                        if ((lvFiwpMaterial.SelectedItem as DataLibrary.FiwpmaterialDTO).FIWPMaterialID > 0)
                        {
                            dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                            dto.Qty = Convert.ToDecimal(C1nboxQty.Value);
                        }
                        else
                        {
                            dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                            dto.Qty = Convert.ToDecimal(C1nboxQty.Value);
                            //dto.QtySum = Convert.ToDecimal(C1nboxQty.Value);
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
                foreach (DataLibrary.FiwpmaterialDTO dto in _trgFiwpmaterial)
                {
                    if (dto == lvFiwpMaterial.SelectedItem as DataLibrary.FiwpmaterialDTO)
                    {
                        if ((lvFiwpMaterial.SelectedItem as DataLibrary.FiwpmaterialDTO).FIWPMaterialID > 0)
                            dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                        else
                            dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;

                        dto.UOMLUID = (cbUOM.SelectedValue as DataLibrary.ComboCodeBoxDTO).DataID;
                        dto.UpdatedDate = DateTime.Now;
                    }
                }
            }
        }
    }
}
