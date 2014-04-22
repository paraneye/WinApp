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

namespace Element.Reveal.TrueTask.Discipline.Schedule.BuildSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComponentGrouping : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        private int _projectid; private string _disciplineCode;
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
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadGrouping();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildSchedule.SelectSchedule));
        }

        /// <summary>
        /// 가상 키보드 엔터 누를 시 다음 페이지 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                MoveNextPage();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            MoveNextPage();
        }

        private void MoveNextPage()
        {
            _objectparam.taskCategoryIdList = new List<string>();
            _objectparam.taskTypeLUIDList = new List<string>();
            _objectparam.systemIdList = new List<int>();
            _objectparam.typeLUIdList = new List<int>();
            _objectparam.drawingtypeLUIdList = new List<int>();
            _objectparam.costcodeIdList = new List<int>();
            _objectparam.taskCategoryCodeList = new List<string>();
            _objectparam.materialIDList = new List<string>();
            _objectparam.progressIDList = new List<string>();
            _objectparam.rfinumberList = new List<string>();
            _objectparam.searchcolumn = string.Empty;
            _objectparam.searchvalueList = new List<string>();

            foreach (DataLibrary.ComboCodeBoxDTO dto in lvTaskCategory.SelectedItems)
            {
                _objectparam.taskCategoryIdList.Add(dto.DataID);
            }

            foreach (DataLibrary.ComboCodeBoxDTO dto in lvTaskType.SelectedItems)
            {
                _objectparam.taskTypeLUIDList.Add(dto.DataID);
            }

            foreach (DataLibrary.ComboCodeBoxDTO dto in lvMaterial.SelectedItems)
            {
                _objectparam.materialIDList.Add(dto.DataID);
            }

            foreach (DataLibrary.ComboCodeBoxDTO dto in lvProgress.SelectedItems)
            {
                _objectparam.progressIDList.Add(dto.DataID);
            }

            _objectparam.searhValue = txtSearch.Text;

            this.Frame.Navigate(typeof(Discipline.Schedule.BuildSchedule.ScheduleComponents), _objectparam);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lvTaskCategory.SelectedItems.Clear();
            lvTaskType.SelectedItems.Clear();
            lvMaterial.SelectedItems.Clear();
            lvProgress.SelectedItems.Clear();

            txtSearch.Text = "";
        }


        #endregion

        #region "Private Method"

        private async void LoadGrouping()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();
            //StretchingPanel.Stretch(false);

            List<DataLibrary.CwpDTO> source = new List<DataLibrary.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _commonsource.GetAvailableCollectionForScheduling(Lib.CWPDataSource.selectedCWP, 0, _projectid, _disciplineCode, 0);

                    if (_commonsource.orgCollection == null)
                    {
                        Login.MasterPage.Loading(false, this);
                        return;
                    }

                    List<DataLibrary.ComboCodeBoxDTO> lstTaskCategory = _commonsource.orgCollection.GroupBy(x => new { x.TaskCategoryCode, x.TaskCategoryName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                    {
                        DataName = x.Key.TaskCategoryName,
                        DataID = x.Key.TaskCategoryCode
                    }).ToList();
                    lvTaskCategory.ItemsSource = lstTaskCategory.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                    List<DataLibrary.ComboCodeBoxDTO> lstTaskType = _commonsource.orgCollection.GroupBy(x => new { x.TaskTypeId, x.TaskTypeName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                    {
                        DataName = x.Key.TaskTypeName,
                        DataID = x.Key.TaskTypeId.ToString()
                    }).ToList();
                    lvTaskType.ItemsSource = lstTaskType.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                    List<DataLibrary.ComboCodeBoxDTO> lstMaterial = _commonsource.orgCollection.GroupBy(x => new { x.MaterialID, x.MaterialName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                    {
                        DataName = x.Key.MaterialName,
                        DataID = x.Key.MaterialID.ToString()
                    }).ToList();
                    lvMaterial.ItemsSource = lstMaterial.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                    List<DataLibrary.ComboCodeBoxDTO> lstProgress = _commonsource.orgCollection.GroupBy(x => new { x.ProgressTypeId, x.ProgressTypeName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                    {
                        DataName = x.Key.ProgressTypeName,
                        DataID = x.Key.ProgressTypeId.ToString()
                    }).ToList();
                    lvProgress.ItemsSource = lstProgress.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                    /*
                    List<DataLibrary.ComboBoxDTO> lstTaskCategory = _commonsource.orgCollection.GroupBy(x => new { x.ProgressTypeId, x.ProgressTypeName }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.ProgressTypeName,
                        DataID = x.Key.ProgressTypeId
                    }).ToList();
                    lvTaskCategory.ItemsSource = lstTaskCategory.Where(x => x.DataID > 0).ToList();

                    List<DataLibrary.ComboBoxDTO> lstTaskType = _commonsource.orgCollection.GroupBy(x => new { x.TaskTypeId, x.TaskTypeName }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.TaskTypeName,
                        DataID = x.Key.TaskTypeId
                    }).ToList();
                    lvTaskType.ItemsSource = lstTaskType.Where(x => x.DataID > 0).ToList();

                    List<DataLibrary.ComboCodeBoxDTO> lstMaterial = _commonsource.orgCollection.GroupBy(x => new { x.TaskCategoryCode, x.TaskCategoryCodeName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                    {
                        DataName = x.Key.TaskCategoryCodeName,
                        DataID = x.Key.TaskCategoryCode
                    }).ToList();
                    lvMaterial.ItemsSource = lstMaterial.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                    List<DataLibrary.ComboBoxDTO> lstProgress = _commonsource.orgCollection.GroupBy(x => new { x.ProgressID, x.Progress }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.ProgressID,
                        DataID = x.Key.Progresss
                    }).ToList();
                    lvProgress.ItemsSource = lstProgress.Where(x => x.DataID > 0).ToList();*/
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "ComponentGrouping LoadGrouping", "There is a problem loading component grouping data - Please try again later", "Loading Error");
            }
            //todo 이전부터 주석
            //this.DefaultViewModel["CWPs"] = source;
            //this.gvCWP.SelectedItem = null;

            //this.gvViewType.SelectedIndex = 0;
            Login.MasterPage.Loading(false, this);
        }

        #endregion

        private void InitSelIDList()
        {
            Lib.CommonDataSource.selTaskCategoryIDList = new List<string>();
            Lib.CommonDataSource.selTaskTypeIDList = new List<string>();
            Lib.CommonDataSource.selTaskCategoryCodeList = new List<string>();
            Lib.CommonDataSource.selProgressIDList = new List<string>();
            Lib.CommonDataSource.selMaterialIDList = new List<string>();
        }

        private void SetCategoryListBySelected()
        {
            _totalselectedcnt = 0;
            InitSelIDList();

            List<DataLibrary.CollectionDTO> list = _commonsource.orgCollection;
            if (list == null)
                return;

            string pars = "";

            //task
            foreach (DataLibrary.ComboCodeBoxDTO dto in lvTaskCategory.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selTaskCategoryIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.TaskCategoryCode.ToString())).ToList();


            //componenttype
            pars = "";
            foreach (DataLibrary.ComboCodeBoxDTO dto in lvTaskType.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selTaskTypeIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.TaskTypeId.ToString())).ToList();

            //system
            pars = "";
            foreach (DataLibrary.ComboCodeBoxDTO dto in lvMaterial.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selMaterialIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.MaterialID.ToString())).ToList();


            //progress
            pars = "";
            foreach (DataLibrary.ComboCodeBoxDTO dto in lvProgress.SelectedItems)
            {
                pars += dto.DataID + ";";
                Lib.CommonDataSource.selProgressIDList.Add(dto.DataID);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.ProgressTypeId.ToString())).ToList();

            Lib.CommonDataSource.curCollection = list;
        }

        private void RebindAnotherCategory(int flag)
        {
            List<DataLibrary.CollectionDTO> list = new List<DataLibrary.CollectionDTO>();

            if (flag != 0)
            {
                if (_totalselectedcnt == 1 && lvTaskCategory.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;

                //taskcategory
                List<DataLibrary.ComboCodeBoxDTO> tmpTaskcategory = list.GroupBy(x => new { x.TaskCategoryCode, x.TaskCategoryName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                {
                    DataName = x.Key.TaskCategoryName,
                    DataID = x.Key.TaskCategoryCode
                }).ToList();

                if (lvTaskCategory.SelectedItems.Count > 0)
                    _chk = false;

                lvTaskCategory.ItemsSource = tmpTaskcategory.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                foreach (DataLibrary.ComboCodeBoxDTO dto in lvTaskCategory.Items)
                {
                    if (Lib.CommonDataSource.selTaskCategoryIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvTaskCategory.SelectedValue = dto;
                    }
                }
            }
            //tasktype
            if (flag != 1)
            {
                if (_totalselectedcnt == 1 && lvTaskType.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;
                //progress
                List<DataLibrary.ComboCodeBoxDTO> tmptasktype = list.GroupBy(x => new { x.TaskTypeId, x.TaskTypeName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                {
                    DataName = x.Key.TaskTypeName,
                    DataID = x.Key.TaskTypeId.ToString()
                }).ToList();

                if (lvTaskType.SelectedItems.Count > 0)
                    _chk = false;

                lvTaskType.ItemsSource = tmptasktype.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                foreach (DataLibrary.ComboCodeBoxDTO dto in lvTaskType.Items)
                {
                    if (Lib.CommonDataSource.selTaskTypeIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvTaskType.SelectedValue = dto;
                    }
                }
            }
            //material
            if (flag != 2)
            {
                if (_totalselectedcnt == 1 && lvMaterial.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;
                //componenttype
                List<DataLibrary.ComboCodeBoxDTO> tmpmaterial = list.GroupBy(x => new { x.MaterialID, x.MaterialName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                {
                    DataName = x.Key.MaterialName,
                    DataID = x.Key.MaterialID.ToString()
                }).ToList();

                if (lvMaterial.SelectedItems.Count > 0)
                    _chk = false;

                lvMaterial.ItemsSource = tmpmaterial.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                foreach (DataLibrary.ComboCodeBoxDTO dto in lvMaterial.Items)
                {
                    if (Lib.CommonDataSource.selMaterialIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvMaterial.SelectedValue = dto;
                    }
                }
            }
            //progress

            if (flag != 3)
            {
                if (_totalselectedcnt == 1 && lvProgress.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;


                List<DataLibrary.ComboCodeBoxDTO> tmpprogress = list.GroupBy(x => new { x.ProgressTypeId, x.ProgressTypeName }).Select(x => new DataLibrary.ComboCodeBoxDTO()
                {
                    DataName = x.Key.ProgressTypeName,
                    DataID = x.Key.ProgressTypeId.ToString()
                }).ToList();

                if (lvProgress.SelectedItems.Count > 0)
                    _chk = false;

                lvProgress.ItemsSource = tmpprogress.Where(x => !string.IsNullOrEmpty(x.DataID)).ToList();

                foreach (DataLibrary.ComboCodeBoxDTO dto in lvProgress.Items)
                {
                    if (Lib.CommonDataSource.selProgressIDList.Where(x => x == dto.DataID).ToList().Count > 0)
                    {
                        _chk = false;
                        lvProgress.SelectedValue = dto;
                    }
                }
            }
        }

        private void lvTaskCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(0);
            }
            else
                _chk = true;
        }

        private void lvTaskType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(1);
            }
            else
                _chk = true;
        }

        private void lvMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(2);
            }
            else
                _chk = true;
        }

        private void lvProgress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(3);
            }
            else
                _chk = true;
        }
    }
}