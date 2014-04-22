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

namespace Element.Reveal.Meg.Discipline.Schedule.BuildIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OLD_ComponentGrouping : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        private int _projectid, _moduleid;

        public OLD_ComponentGrouping()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {   
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            LoadGrouping();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildIWP.SelectIWP));
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            _objectparam.materialCategoryIdList = new List<int>();
            _objectparam.taskCategoryIdList = new List<int>();
            _objectparam.typeLUIdList = new List<int>();
            _objectparam.systemIdList = new List<int>();
            _objectparam.drawingtypeLUIdList = new List<int>();
            _objectparam.costcodeIdList = new List<int>();
            _objectparam.searchstringList = new List<RevealProjectSvc.ComboBoxDTO>();
            _objectparam.compsearchstringList = new List<RevealProjectSvc.ComboBoxDTO>();
            _objectparam.locationList = new List<RevealProjectSvc.ComboBoxDTO>();
            _objectparam.rfinumberList = new List<string>();
            _objectparam.searchcolumn = string.Empty;
            _objectparam.searchvalueList = new List<string>();

            foreach (RevealProjectSvc.ComboBoxDTO dto in lvTask.SelectedItems)
            {   
                _objectparam.materialCategoryIdList.Add(dto.DataID);
            }

            foreach (RevealProjectSvc.ComboBoxDTO dto in lvProgress.SelectedItems)
            {

                _objectparam.taskCategoryIdList.Add(dto.DataID);
            }

            foreach (RevealProjectSvc.ComboBoxDTO dto in lvComponentType.SelectedItems)
            {

                _objectparam.typeLUIdList.Add(dto.DataID);
            }

            foreach (RevealProjectSvc.ComboBoxDTO dto in lvSystem.SelectedItems)
            {

                _objectparam.systemIdList.Add(dto.DataID);
            }

            foreach (RevealProjectSvc.ComboBoxDTO dto in lvDrawingType.SelectedItems)
            {

                _objectparam.drawingtypeLUIdList.Add(dto.DataID);
            }

            foreach (RevealProjectSvc.ComboBoxDTO dto in lvCostcode.SelectedItems)
            {

                _objectparam.costcodeIdList.Add(dto.DataID);
            }

            _objectparam.searchstringList = getSearchStringList(_objectparam.materialCategoryIdList);

            _objectparam.compsearchstringList = getCompSearchStringList();
            _objectparam.locationList = getLocationList();

            if (!string.IsNullOrEmpty(txtRFINumber.Text))//Search Value - RFI Number
                _objectparam.rfinumberList.Add(txtRFINumber.Text);

            this.Frame.Navigate(typeof(Discipline.Schedule.BuildIWP.ScheduleComponents), _objectparam);
        }


        protected List<RevealProjectSvc.ComboBoxDTO> getSearchStringList(List<int> materialCategoryIdList)
        {
            List<RevealProjectSvc.ComboBoxDTO> list = new List<RevealProjectSvc.ComboBoxDTO>();

            if (!string.IsNullOrEmpty(txtTagNumber.Text))
            {
                list.Add(new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = Lib.MaterialField.TagNumber,
                    ExtraValue1 = "like",
                    ExtraValue2 = "%" + txtTagNumber.Text.ToLower() + "%"
                });
            }

            if (!string.IsNullOrEmpty(txtLocation.Text))
            {
                list.Add(new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = Lib.MaterialField.StringVar11,
                    ExtraValue1 = "like",
                    ExtraValue2 = "%" + txtLocation.Text.ToLower() + "%"
                });
            }

            //Cable
            if (materialCategoryIdList.Contains(Lib.MaterialCategory.Cable))
            {
                //FromTag
                if (!string.IsNullOrEmpty(txtFromTag.Text))
                {
                    list.Add(new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = Lib.MaterialField.StringVar7,
                        ExtraValue1 = "like",
                        ExtraValue2 = "%" + txtFromTag.Text.ToLower() + "%"
                    });
                }
                //ToTag
                if (!string.IsNullOrEmpty(txtToTag.Text))
                {
                    list.Add(new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = Lib.MaterialField.StringVar8,
                        ExtraValue1 = "like",
                        ExtraValue2 = "%" + txtToTag.Text.ToLower() + "%"
                    });
                }
            }

            return list;
        }

        protected List<RevealProjectSvc.ComboBoxDTO> getCompSearchStringList()
        {
            List<RevealProjectSvc.ComboBoxDTO> list = new List<RevealProjectSvc.ComboBoxDTO>();

            if (!string.IsNullOrEmpty(txtLineNumber.Text))
            {
                list.Add(new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = Lib.ComponentField.LineNo,
                    ExtraValue1 = "like",
                    ExtraValue2 = "%" + txtLineNumber.Text.ToLower() + "%"
                });
            }

            if (!string.IsNullOrEmpty(txtEWONumber.Text))
            {
                list.Add(new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = Lib.ComponentField.EWO,
                    ExtraValue1 = "like",
                    ExtraValue2 = "%" + txtEWONumber.Text.ToLower() + "%"
                });
            }

            return list;
        }

        protected List<RevealProjectSvc.ComboBoxDTO> getLocationList()
        {
            List<RevealProjectSvc.ComboBoxDTO> list = new List<RevealProjectSvc.ComboBoxDTO>();

            if (cbEast.SelectedIndex > 0)
            {
                list.Add(new RevealProjectSvc.ComboBoxDTO()
                {
                    DataID = Convert.ToInt32(((ComboBoxItem)this.cbEast.SelectedItem).Tag.ToString()), 
                    DataName = Lib.MaterialDimension.FEast,
                    ExtraValue3 = Lib.MaterialDimension.TEast,
                    ExtraValue1 = txtFromEast.Text,
                    ExtraValue2 = txtToEast.Text
                });
            }

            if (cbNorth.SelectedIndex > 0)
            {
                list.Add(new RevealProjectSvc.ComboBoxDTO()
                {
                    DataID = Convert.ToInt32(((ComboBoxItem)this.cbNorth.SelectedItem).Tag.ToString()), 
                    DataName = Lib.MaterialDimension.FNorth,
                    ExtraValue3 = Lib.MaterialDimension.TNorth,
                    ExtraValue1 = txtFromNorth.Text,
                    ExtraValue2 = txtToNorth.Text
                });
            }

            if (cbElevation.SelectedIndex > 0)
            {
                list.Add(new RevealProjectSvc.ComboBoxDTO()
                {
                    DataID = Convert.ToInt32(((ComboBoxItem)this.cbElevation.SelectedItem).Tag.ToString()),
                    DataName = Lib.MaterialDimension.FElevation,
                    ExtraValue3 = Lib.MaterialDimension.TElevation,
                    ExtraValue1 = txtFromElevation.Text,
                    ExtraValue2 = txtToElevation.Text
                });
            }

            return list;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lvTask.SelectedItems.Clear();
            lvProgress.SelectedItems.Clear();
            lvComponentType.SelectedItems.Clear();
            lvSystem.SelectedItems.Clear();
            lvDrawingType.SelectedItems.Clear();
            lvCostcode.SelectedItems.Clear();
            txtFromTag.Text = "";
            txtToTag.Text = "";
            txtLineNumber.Text = "";
            txtTagNumber.Text = "";
            txtRFINumber.Text = "";
            txtEWONumber.Text = "";
            txtLocation.Text = "";            
            cbEast.SelectedIndex = 0;
            cbNorth.SelectedIndex = 0;
            cbElevation.SelectedIndex = 0;            
        }

        private void cbEast_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEast != null)
            {
                switch (cbEast.SelectedIndex)
                {   
                    case 1:
                        txtToEast.Text = "";
                        txtFromEast.IsEnabled = true;
                        txtToEast.IsEnabled = false;
                        break;
                    case 2:                        
                        txtFromEast.IsEnabled = true;
                        txtToEast.IsEnabled = true;
                        break;
                    case 3:
                        txtFromEast.Text = "";                        
                        txtFromEast.IsEnabled = false;
                        txtToEast.IsEnabled = true;
                        break;
                    default:
                        txtFromEast.Text = "";
                        txtToEast.Text = "";
                        txtFromEast.IsEnabled = false;
                        txtToEast.IsEnabled = false;
                        break;
                }
            }
        }

        private void cbNorth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbNorth != null)
            {
                switch (cbNorth.SelectedIndex)
                {
                    case 1:
                        txtToNorth.Text = "";
                        txtFromNorth.IsEnabled = true;
                        txtToNorth.IsEnabled = false;
                        break;
                    case 2:
                        txtFromNorth.IsEnabled = true;
                        txtToNorth.IsEnabled = true;
                        break;
                    case 3:
                        txtFromNorth.Text = "";
                        txtFromNorth.IsEnabled = false;
                        txtToNorth.IsEnabled = true;
                        break;
                    default:
                        txtFromNorth.Text = "";
                        txtToNorth.Text = "";
                        txtFromNorth.IsEnabled = false;
                        txtToNorth.IsEnabled = false;
                        break;
                }
            }
        }

        private void cbElevation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbElevation != null)
            {
                switch (cbElevation.SelectedIndex)
                {
                    case 1:
                        txtToElevation.Text = "";
                        txtFromElevation.IsEnabled = true;
                        txtToElevation.IsEnabled = false;
                        break;
                    case 2:
                        txtFromElevation.IsEnabled = true;
                        txtToElevation.IsEnabled = true;
                        break;
                    case 3:
                        txtFromElevation.Text = "";
                        txtFromElevation.IsEnabled = false;
                        txtToElevation.IsEnabled = true;
                        break;
                    default:
                        txtFromElevation.Text = "";
                        txtToElevation.Text = "";
                        txtFromElevation.IsEnabled = false;
                        txtToElevation.IsEnabled = false;
                        break;
                }
            }
        }

        #endregion

        #region "Private Method"

        private async void LoadGrouping()
        {
            Login.MasterPage.Loading(true, this);
            //StretchingPanel.Stretch(false);

            List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _commonsource.GetAvailableCollectionForScheduling(Lib.CWPDataSource.selectedCWP, Lib.ScheduleDataSource.selectedSchedule, _projectid, _moduleid);

                    List<RevealProjectSvc.ComboBoxDTO> list = _commonsource.orgCollection.GroupBy(x=> new {x.MaterialCategoryID, x.MaterialCategory}).Select(x => new RevealProjectSvc.ComboBoxDTO()
                          {
                              DataName = x.Key.MaterialCategory,
                              DataID = x.Key.MaterialCategoryID
                          }).ToList();

                    lvTask.ItemsSource = list;
                   
                    //var taskSource = await (new Lib.ServiceModel.CommonModel()).GetMaterialCategoryByModule_Combo(_moduleid);
                    //lvTask.ItemsSource = taskSource;

                    //var progressSource = await (new Lib.ServiceModel.CommonModel()).GetTaskcategoryByMaterialCategory(_moduleid, 0);
                    //lvProgress.ItemsSource = progressSource;

                    //var componenttypeSource = await (new Lib.ServiceModel.CommonModel()).GetLookupByLookupType(WinAppLibrary.Utilities.LOOKUPTYPE.ComponentTaskType);
                    //lvComponentType.ItemsSource = componenttypeSource;

                    //var systemSource = await (new Lib.ServiceModel.CommonModel()).GetSystemByProject_Combo(_projectid, _moduleid);
                    //lvSystem.ItemsSource = systemSource;

                    //var drawingtypeSource = await (new Lib.ServiceModel.CommonModel()).GetLookupByLookupType(WinAppLibrary.Utilities.LOOKUPTYPE.DrawingType);
                    //lvDrawingType.ItemsSource = drawingtypeSource;

                    //var costcodeSource = await (new Lib.ServiceModel.CommonModel()).GetCostCodeByProject_Combo(_projectid, _moduleid);
                    //lvCostcode.ItemsSource = costcodeSource;

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "ComponentGrouping LoadGrouping", "There was an error load Grouping. Pleae contact administrator", "Error!");
            }

            //this.DefaultViewModel["CWPs"] = source;
            //this.gvCWP.SelectedItem = null;

            //this.gvViewType.SelectedIndex = 0;
            Login.MasterPage.Loading(false, this);
        }

        #endregion

        private void lvTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<RevealProjectSvc.CollectionDTO> list = _commonsource.orgCollection;
            string pars = "";


            foreach (RevealProjectSvc.ComboBoxDTO dto in lvTask.SelectedItems)
            {
                pars += dto.DataID + ";";
            }

            var result = list.Where(x => pars.Contains(x.MaterialCategoryID.ToString())).ToList();

            Lib.CommonDataSource.selectedTaskCategory = result;

            List<RevealProjectSvc.ComboBoxDTO> tmp = Lib.CommonDataSource.selectedTaskCategory.GroupBy(x => new { x.TaskCategoryID, x.TaskCategory }).Select(x => new RevealProjectSvc.ComboBoxDTO()
            {
                DataName = x.Key.TaskCategory,
                DataID = x.Key.TaskCategoryID
            }).ToList();


            lvProgress.ItemsSource = tmp;
        }

        private void lvProgress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<RevealProjectSvc.CollectionDTO> list = Lib.CommonDataSource.selectedTaskCategory;
            string pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvProgress.SelectedItems)
            {
                pars += dto.DataID + ";";
            }
            var result = list.Where(x => pars.Contains(x.TaskCategoryID.ToString())).ToList();

            Lib.CommonDataSource.selectedComponentType = result;

            List<RevealProjectSvc.ComboBoxDTO> tmp = Lib.CommonDataSource.selectedComponentType.GroupBy(x => new { x.TaskTypeLUID, x.TaskType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
            {
                DataName = x.Key.TaskType,
                DataID = x.Key.TaskTypeLUID
            }).ToList();


            lvComponentType.ItemsSource = tmp;
        }

        private void lvComponentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<RevealProjectSvc.CollectionDTO> list = Lib.CommonDataSource.selectedComponentType;
            string pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvComponentType.SelectedItems)
            {
                pars += dto.DataID + ";";
            }
            var result = list.Where(x => pars.Contains(x.TaskTypeLUID.ToString())).ToList();

            Lib.CommonDataSource.selectedSystem = result;

            List<RevealProjectSvc.ComboBoxDTO> tmp = Lib.CommonDataSource.selectedSystem.GroupBy(x => new { x.SystemID, x.SystemName }).Select(x => new RevealProjectSvc.ComboBoxDTO()
            {
                DataName = x.Key.SystemName,
                DataID = x.Key.SystemID
            }).ToList();

            lvSystem.ItemsSource = tmp;
        }

        private void lvSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<RevealProjectSvc.CollectionDTO> list = Lib.CommonDataSource.selectedSystem;
            string pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvSystem.SelectedItems)
            {
                pars += dto.DataID + ";";
            }
            var result = list.Where(x => pars.Contains(x.SystemID.ToString())).ToList();

            Lib.CommonDataSource.selectedDrawingType = result;

            List<RevealProjectSvc.ComboBoxDTO> tmp = Lib.CommonDataSource.selectedDrawingType.GroupBy(x => new { x.DrawingTypeLUID, x.DrawingType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
            {
                DataName = x.Key.DrawingType,
                DataID = x.Key.DrawingTypeLUID
            }).ToList();

            lvDrawingType.ItemsSource = tmp;
        }

        private void lvDrawingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<RevealProjectSvc.CollectionDTO> list = Lib.CommonDataSource.selectedDrawingType;
            string pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvDrawingType.SelectedItems)
            {
                pars += dto.DataID + ";";
            }
            var result = list.Where(x => pars.Contains(x.DrawingTypeLUID.ToString())).ToList();

            Lib.CommonDataSource.selectedCostCode = result;

            List<RevealProjectSvc.ComboBoxDTO> tmp = Lib.CommonDataSource.selectedCostCode.GroupBy(x => new { x.CostCodeID, x.CostCode }).Select(x => new RevealProjectSvc.ComboBoxDTO()
            {
                DataName = x.Key.CostCode,
                DataID = x.Key.CostCodeID
            }).ToList();

            lvCostcode.ItemsSource = tmp;
        }
    }
}
