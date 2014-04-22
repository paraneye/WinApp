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

namespace Element.Reveal.Meg.Discipline.MTO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectLibrary : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        List<RevealCommonSvc.MaterialcategoryDTO> _materialcategory = new List<RevealCommonSvc.MaterialcategoryDTO>();
        private int _projectid, _moduleid;

        public SelectLibrary()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            LoadLibrary();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.MTO.SelectTask));
        }

        #endregion

        #region "Private Method"

        private async void LoadLibrary()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    List<Lib.LibTemplate> list = new List<Lib.LibTemplate>();
                    switch (Lib.CommonDataSource.selectedMaterialCategory)
                    {
                        case Lib.MaterialCategory.Grounding:
                            var grounding = await (new Lib.ServiceModel.ProjectModel()).GetLibGroundingManhourForPaging(1, 99999, "", "", "");
                            foreach (var dto in grounding.lib)
                            {
                                Lib.LibTemplate tmpdto = new Lib.LibTemplate();
                                tmpdto.PartNumber = dto.PartNumber;
                                tmpdto.Description = dto.Description;
                                tmpdto.UOM = dto.UOM;
                                tmpdto.Manhours = dto.Manhours;
                                list.Add(tmpdto);
                            }
                            break;
                        case Lib.MaterialCategory.Equipment:
                            var equipment = await (new Lib.ServiceModel.ProjectModel()).GetLibEquipManhourForPaging(1, 99999, "", "", "");
                            foreach (var dto in equipment.lib)
                            {
                                Lib.LibTemplate tmpdto = new Lib.LibTemplate();
                                tmpdto.PartNumber = dto.PartNumber;
                                tmpdto.Description = dto.Description;
                                tmpdto.UOM = dto.UOM;
                                tmpdto.Manhours = dto.Manhours;
                                list.Add(tmpdto);
                            }
                            break;
                        case Lib.MaterialCategory.LightingREC:
                            var lighting = await (new Lib.ServiceModel.ProjectModel()).GetLibLightingManhourForPaging(1, 99999, "", "", "");
                            foreach (var dto in lighting.lib)
                            {
                                Lib.LibTemplate tmpdto = new Lib.LibTemplate();
                                tmpdto.PartNumber = dto.PartNumber;
                                tmpdto.Description = dto.Description;
                                tmpdto.UOM = dto.UOM;
                                tmpdto.Manhours = dto.Manhours;
                                list.Add(tmpdto);
                            }
                            break;
                        case Lib.MaterialCategory.Raceway:
                            var raceway = await (new Lib.ServiceModel.ProjectModel()).GetLibRacewayManhourForPaging(1, 99999, "", "", "");
                            foreach (var dto in raceway.lib)
                            {
                                Lib.LibTemplate tmpdto = new Lib.LibTemplate();
                                tmpdto.PartNumber = dto.PartNo;
                                tmpdto.UOM = dto.UOM;
                                tmpdto.Manhours = dto.ManHours;
                                list.Add(tmpdto);
                            }
                            break;
                        case Lib.MaterialCategory.Instruments:
                            var instrument = await (new Lib.ServiceModel.ProjectModel()).GetLibInstrManhourForPaging(1, 99999, "", "", "");
                            foreach (var dto in instrument.lib)
                            {
                                Lib.LibTemplate tmpdto = new Lib.LibTemplate();
                                tmpdto.PartNumber = dto.Partnumber;
                                tmpdto.Description = dto.Description;
                                tmpdto.UOM = dto.UOM;
                                tmpdto.Manhours = dto.Manhours;
                                list.Add(tmpdto);
                            }
                            break;
                        case Lib.MaterialCategory.EHT:
                            var eht = await (new Lib.ServiceModel.ProjectModel()).GetLibEhtManhourForPaging(1, 99999, "", "", "");
                            foreach (var dto in eht.lib)
                            {
                                Lib.LibTemplate tmpdto = new Lib.LibTemplate();
                                tmpdto.PartNumber = dto.PartNo;
                                tmpdto.UOM = dto.UOM;
                                tmpdto.Manhours = dto.Manhours;
                                list.Add(tmpdto);
                            }
                            break;
                        case Lib.MaterialCategory.Cable:
                            var cable = await (new Lib.ServiceModel.ProjectModel()).GetLibCableManhourForPaging(1, 99999, "", "", "");
                            foreach (var dto in cable.lib)
                            {
                                Lib.LibTemplate tmpdto = new Lib.LibTemplate();
                                tmpdto.PartNumber = dto.PartNumber;
                                tmpdto.UOM = dto.UOM;
                                tmpdto.Manhours = dto.CablePullUnitRate;
                                list.Add(tmpdto);
                            }
                            break;
                    }
                    

                    lvLibrary.ItemsSource = list;


                  //  lvCWP.ItemsSource = _cwp.GetCWPs();

                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }

            Login.MasterPage.Loading(false, this);
        }

        #endregion

        private void lvLibrary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void btnVendor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
