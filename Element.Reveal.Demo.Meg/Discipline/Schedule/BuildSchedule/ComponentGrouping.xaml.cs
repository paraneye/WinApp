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

namespace Element.Reveal.Meg.Discipline.Schedule.BuildSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComponentGrouping : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        private int _projectid, _moduleid;
        private int _totalselectedcnt;
        private bool _chk = true;
        public ComponentGrouping()
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
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildSchedule.SelectSchedule));
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

            this.Frame.Navigate(typeof(Discipline.Schedule.BuildSchedule.ScheduleComponents), _objectparam);
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
            Login.MasterPage.ShowUserStatus();
            //StretchingPanel.Stretch(false);

            List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _commonsource.GetAvailableCollectionForScheduling(Lib.CWPDataSource.selectedCWP, 0, _projectid, _moduleid);

                    List<RevealProjectSvc.ComboBoxDTO> lstTask = _commonsource.orgCollection.GroupBy(x => new { x.MaterialCategoryID, x.MaterialCategory }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = x.Key.MaterialCategory,
                        DataID = x.Key.MaterialCategoryID
                    }).ToList();

                    lvTask.ItemsSource = lstTask.Where(x => x.DataID > 0).ToList();

                    List<RevealProjectSvc.ComboBoxDTO> lstProgress = _commonsource.orgCollection.GroupBy(x => new { x.TaskCategoryID, x.TaskCategory }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = x.Key.TaskCategory,
                        DataID = x.Key.TaskCategoryID
                    }).ToList();

                    lvProgress.ItemsSource = lstProgress.Where(x => x.DataID > 0).ToList();

                    List<RevealProjectSvc.ComboBoxDTO> lstComponentType = _commonsource.orgCollection.GroupBy(x => new { x.TaskTypeLUID, x.TaskType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = x.Key.TaskType,
                        DataID = x.Key.TaskTypeLUID
                    }).ToList();

                    lvComponentType.ItemsSource = lstComponentType.Where(x => x.DataID > 0).ToList();

                    List<RevealProjectSvc.ComboBoxDTO> lstSystem = _commonsource.orgCollection.GroupBy(x => new { x.SystemID, x.SystemName }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = x.Key.SystemName,
                        DataID = x.Key.SystemID
                    }).ToList();

                    lvSystem.ItemsSource = lstSystem.Where(x => x.DataID > 0).ToList();

                    List<RevealProjectSvc.ComboBoxDTO> lstDrawingType = _commonsource.orgCollection.GroupBy(x => new { x.DrawingTypeLUID, x.DrawingType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = x.Key.DrawingType,
                        DataID = x.Key.DrawingTypeLUID
                    }).ToList();

                    lvDrawingType.ItemsSource = lstDrawingType.Where(x => x.DataID > 0).ToList();


                    List<RevealProjectSvc.ComboBoxDTO> lstCostCode = _commonsource.orgCollection.GroupBy(x => new { x.CostCodeID, x.CostCode }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                    {
                        DataName = x.Key.CostCode,
                        DataID = x.Key.CostCodeID
                    }).ToList();

                    lvCostcode.ItemsSource = lstCostCode.Where(x => x.DataID > 0).ToList();
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

        private void InitSelIDList(){
            Lib.CommonDataSource.selMaterialCategoryIDList = new List<int>();
            Lib.CommonDataSource.selTaskCategoryIDList = new List<int>();
            Lib.CommonDataSource.selComponentTypeIDList = new List<int>();
            Lib.CommonDataSource.selSystemIDList = new List<int>();
            Lib.CommonDataSource.selDrawingTypeIDList = new List<int>();
            Lib.CommonDataSource.selCostCodeIDList = new List<int>();

        }

        private void SetCategoryListBySelected()
        {
            _totalselectedcnt = 0;
            InitSelIDList();

            List<RevealProjectSvc.CollectionDTO> list = _commonsource.orgCollection;

            string pars = "";

            //task
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvTask.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selMaterialCategoryIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.MaterialCategoryID.ToString())).ToList();

            //progress
            pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvProgress.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selTaskCategoryIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.TaskCategoryID.ToString())).ToList();

            //componenttype
            pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvComponentType.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selComponentTypeIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.TaskTypeLUID.ToString())).ToList();

            //system
            pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvSystem.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selSystemIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.SystemID.ToString())).ToList();

            //drawingtype
            pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvDrawingType.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selDrawingTypeIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.DrawingTypeLUID.ToString())).ToList();

            //costcode
            pars = "";
            foreach (RevealProjectSvc.ComboBoxDTO dto in lvCostcode.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selCostCodeIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.CostCodeID.ToString())).ToList();

            Lib.CommonDataSource.curCollection = list;
        }

        private void RebindAnotherCategory(int flag)
        {
            List<RevealProjectSvc.CollectionDTO> list = new List<RevealProjectSvc.CollectionDTO>();

            if (flag != 0)
            {
                if (_totalselectedcnt == 1 && lvTask.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;

                //task
                List<RevealProjectSvc.ComboBoxDTO> tmpTask = list.GroupBy(x => new { x.MaterialCategoryID, x.MaterialCategory }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.MaterialCategory,
                    DataID = x.Key.MaterialCategoryID
                }).ToList();

                if (lvTask.SelectedItems.Count > 0)
                    _chk = false;

                lvTask.ItemsSource = tmpTask.Where(x => x.DataID > 0).ToList();

                foreach (RevealProjectSvc.ComboBoxDTO dto in lvTask.Items)
                {
                    if (Lib.CommonDataSource.selMaterialCategoryIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvTask.SelectedValue = dto;
                    }
                }
            }
            if (flag != 1)
            {
                if (_totalselectedcnt == 1 && lvProgress.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;
                //progress
                List<RevealProjectSvc.ComboBoxDTO> tmpProgress = list.GroupBy(x => new { x.TaskCategoryID, x.TaskCategory }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.TaskCategory,
                    DataID = x.Key.TaskCategoryID
                }).ToList();

                if (lvProgress.SelectedItems.Count > 0)
                    _chk = false;

                lvProgress.ItemsSource = tmpProgress.Where(x => x.DataID > 0).ToList();

                foreach (RevealProjectSvc.ComboBoxDTO dto in lvProgress.Items)
                {
                    if (Lib.CommonDataSource.selTaskCategoryIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvProgress.SelectedValue = dto;
                    }
                }
            }
            if (flag != 2)
            {
                if (_totalselectedcnt == 1 && lvComponentType.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;
                //componenttype
                List<RevealProjectSvc.ComboBoxDTO> tmpComponentType = list.GroupBy(x => new { x.TaskTypeLUID, x.TaskType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.TaskType,
                    DataID = x.Key.TaskTypeLUID
                }).ToList();

                if (lvComponentType.SelectedItems.Count > 0)
                    _chk = false;

                lvComponentType.ItemsSource = tmpComponentType.Where(x => x.DataID > 0).ToList();

                foreach (RevealProjectSvc.ComboBoxDTO dto in lvComponentType.Items)
                {
                    if (Lib.CommonDataSource.selComponentTypeIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvComponentType.SelectedValue = dto;
                    }
                }
            }
            if (flag != 3)
            {
                if (_totalselectedcnt == 1 && lvSystem.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;

                //system
                List<RevealProjectSvc.ComboBoxDTO> tmpSystem = list.GroupBy(x => new { x.SystemID, x.SystemName }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.SystemName,
                    DataID = x.Key.SystemID
                }).ToList();

                if (lvSystem.SelectedItems.Count > 0)
                    _chk = false;

                lvSystem.ItemsSource = tmpSystem.Where(x => x.DataID > 0).ToList();

                foreach (RevealProjectSvc.ComboBoxDTO dto in lvSystem.Items)
                {
                    if (Lib.CommonDataSource.selSystemIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvSystem.SelectedValue = dto;
                    }
                }
            }
            if (flag != 4)
            {
                if (_totalselectedcnt == 1 && lvDrawingType.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;

                //drawingtype
                List<RevealProjectSvc.ComboBoxDTO> tmpDrawingType = list.GroupBy(x => new { x.DrawingTypeLUID, x.DrawingType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.DrawingType,
                    DataID = x.Key.DrawingTypeLUID
                }).ToList();

                if (lvDrawingType.SelectedItems.Count > 0)
                    _chk = false;

                lvDrawingType.ItemsSource = tmpDrawingType.Where(x => x.DataID > 0).ToList();

                foreach (RevealProjectSvc.ComboBoxDTO dto in lvDrawingType.Items)
                {
                    if (Lib.CommonDataSource.selDrawingTypeIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvDrawingType.SelectedValue = dto;
                    }
                }
            }
            if (flag != 5)
            {
                if (_totalselectedcnt == 1 && lvCostcode.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;

                //costcode
                List<RevealProjectSvc.ComboBoxDTO> tmpCostcode = list.GroupBy(x => new { x.CostCodeID, x.CostCode }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.CostCode,
                    DataID = x.Key.CostCodeID
                }).ToList();

                if (lvCostcode.SelectedItems.Count > 0)
                    _chk = false;

                lvCostcode.ItemsSource = tmpCostcode.Where(x => x.DataID > 0).ToList();

                foreach (RevealProjectSvc.ComboBoxDTO dto in lvCostcode.Items)
                {
                    if (Lib.CommonDataSource.selCostCodeIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvCostcode.SelectedValue = dto;
                    }
                }
            }
        }


        private void lvTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(0);
            }
            else
                _chk = true;
        }

        private void lvProgress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
            SetCategoryListBySelected();
            RebindAnotherCategory(1);
            }
            else
                _chk = true;
        }

        private void lvComponentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(2);
            }
            else
                _chk = true;
        }

        private void lvSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(3);
            }
            else
                _chk = true;
        }

        private void lvDrawingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(4);
            }
            else
                _chk = true;
        }

        private void lvCostcode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(5);
            }
            else
                _chk = true;
        }

        
    }
}
