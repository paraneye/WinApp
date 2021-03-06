﻿using System;
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

namespace Element.Reveal.TrueTask.Discipline.Schedule.BuildHydro
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
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildHydro.SelectHydro));
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            _objectparam.moduleList = new List<DataLibrary.ComboBoxDTO>();
            _objectparam.pidList = new List<DataLibrary.ComboBoxDTO>();
            _objectparam.processsystemList = new List<DataLibrary.ComboBoxDTO>();
            _objectparam.lineList = new List<DataLibrary.ComboBoxDTO>();

            foreach (DataLibrary.ComboBoxDTO dto in lvModule.SelectedItems)
            {
                _objectparam.moduleList.Add(new DataLibrary.ComboBoxDTO()
                {
                    DataName = Lib.MaterialField.StringVar20,
                    ExtraValue1 = "like",
                    ExtraValue2 = "%" + dto.DataName.ToLower() + "%"
                });
            }

            foreach (DataLibrary.ComboBoxDTO dto in lvPID.SelectedItems)
            {

                _objectparam.pidList.Add(new DataLibrary.ComboBoxDTO()
                {
                    ExtraValue1 = "like",
                    ExtraValue2 = "%" + dto.DataName.ToLower() + "%"
                });
            }

            foreach (DataLibrary.ComboBoxDTO dto in lvProcessSystem.SelectedItems)
            {
                _objectparam.processsystemList.Add(new DataLibrary.ComboBoxDTO()
                {
                    DataName = Lib.MaterialField.StringVar21,
                    ExtraValue1 = "like",
                    ExtraValue2 = "%" + dto.DataName.ToLower() + "%"
                });
            }

            foreach (DataLibrary.ComboBoxDTO dto in lvLine.SelectedItems)
            {

                _objectparam.lineList.Add(new DataLibrary.ComboBoxDTO()
                {
                    DataName = Lib.ComponentField.LineNo,
                    ExtraValue1 = "like",
                    ExtraValue2 = "%" + dto.DataName.ToLower() + "%"
                });
            }

            this.Frame.Navigate(typeof(Discipline.Schedule.BuildHydro.ScheduleComponents), _objectparam);
        }


     
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lvModule.SelectedItems.Clear();
            lvPID.SelectedItems.Clear();
            lvProcessSystem.SelectedItems.Clear();
            lvLine.SelectedItems.Clear();
        }

        #endregion

        #region "Private Method"

        private async void LoadGrouping()
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.ShowUserStatus();

            List<DataLibrary.CwpDTO> source = new List<DataLibrary.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _commonsource.GetAvailableCollectionForHydroScheduling(Lib.CWPDataSource.selectedCWP, Lib.ScheduleDataSource.selectedSchedule, _projectid, _disciplineCode);

                    List<DataLibrary.ComboBoxDTO> lstModule = _commonsource.orgCollection.GroupBy(x => new { x.Module }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.Module
                    }).ToList();

                    lvModule.ItemsSource = lstModule.Where(x => !String.IsNullOrEmpty(x.DataName)).ToList();

                    List<DataLibrary.ComboBoxDTO> lstPID = _commonsource.orgCollection.GroupBy(x => new { x.PANDID }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.PANDID
                    }).ToList();

                    lvPID.ItemsSource = lstPID.Where(x => !String.IsNullOrEmpty(x.DataName)).ToList();

                    List<DataLibrary.ComboBoxDTO> lstProcessSystem = _commonsource.orgCollection.GroupBy(x => new { x.ProcessSystem }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.ProcessSystem
                    }).ToList();

                    lvProcessSystem.ItemsSource = lstProcessSystem.Where(x => !String.IsNullOrEmpty(x.DataName)).ToList();

                    List<DataLibrary.ComboBoxDTO> lstLine = _commonsource.orgCollection.GroupBy(x => new { x.ISOLineNo }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.ISOLineNo
                    }).ToList();

                    lvLine.ItemsSource = lstLine.Where(x => !String.IsNullOrEmpty(x.DataName)).ToList();

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "ComponentGrouping LoadGrouping", "There is a problem loading component grouping data - Please try again later", "Loading Error");
            }

            Login.MasterPage.Loading(false, this);
        }

        #endregion

        private void InitSelIDList()
        {
            Lib.CommonDataSource.selModuleList = new List<string>();
            Lib.CommonDataSource.selPIDList = new List<string>();
            Lib.CommonDataSource.selProcessSystemList = new List<string>();
            Lib.CommonDataSource.selLineList = new List<string>();
        }

        private void SetCategoryListBySelected()
        {
            _totalselectedcnt = 0;
            InitSelIDList();

            List<DataLibrary.CollectionDTO> list = _commonsource.orgCollection;

            string pars = "";

            //module
            foreach (DataLibrary.ComboBoxDTO dto in lvModule.SelectedItems)
            {
                pars += dto.DataName + ";";
                Lib.CommonDataSource.selModuleList.Add(dto.DataName);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.Module)).ToList();

            //p&id
            pars = "";
            foreach (DataLibrary.ComboBoxDTO dto in lvPID.SelectedItems)
            {
                pars += dto.DataName + ";";
                Lib.CommonDataSource.selPIDList.Add(dto.DataName);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.PANDID)).ToList();

            //process system
            pars = "";
            foreach (DataLibrary.ComboBoxDTO dto in lvProcessSystem.SelectedItems)
            {
                pars += dto.DataName + ";";
                Lib.CommonDataSource.selProcessSystemList.Add(dto.DataName);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.ProcessSystem)).ToList();

            //line
            pars = "";
            foreach (DataLibrary.ComboBoxDTO dto in lvLine.SelectedItems)
            {
                pars += dto.DataName + ";";
                Lib.CommonDataSource.selLineList.Add(dto.DataName);
                _totalselectedcnt++;
            }
            if (!string.IsNullOrEmpty(pars))
                list = list.Where(x => pars.Contains(x.ISOLineNo)).ToList();

         
            Lib.CommonDataSource.curCollection = list;
        }

        private void RebindAnotherCategory(int flag)
        {
            List<DataLibrary.CollectionDTO> list = new List<DataLibrary.CollectionDTO>();

            if (flag != 0)
            {
                if (_totalselectedcnt == 1 && lvModule.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;

                //module
                List<DataLibrary.ComboBoxDTO> tmpModule = list.GroupBy(x => new { x.Module }).Select(x => new DataLibrary.ComboBoxDTO()
                {
                    DataName = x.Key.Module
                }).ToList();

                if (lvModule.SelectedItems.Count > 0)
                    _chk = false;

                lvModule.ItemsSource = tmpModule.Where(x => !String.IsNullOrEmpty(x.DataName)).ToList();

                foreach (DataLibrary.ComboBoxDTO dto in lvModule.Items)
                {
                    if (Lib.CommonDataSource.selModuleList.Where(x => !String.IsNullOrEmpty(dto.DataName)).ToList().Count > 0)
                    {
                        _chk = false;
                        lvModule.SelectedValue = dto;
                    }
                }
            }
            if (flag != 1)
            {
                if (_totalselectedcnt == 1 && lvPID.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;
                //p&id
                List<DataLibrary.ComboBoxDTO> tmpPID = list.GroupBy(x => new { x.PANDID }).Select(x => new DataLibrary.ComboBoxDTO()
                {
                    DataName = x.Key.PANDID
                }).ToList();

                if (lvPID.SelectedItems.Count > 0)
                    _chk = false;

                lvPID.ItemsSource = tmpPID.Where(x => !String.IsNullOrEmpty(x.DataName)).ToList();

                foreach (DataLibrary.ComboBoxDTO dto in lvPID.Items)
                {
                    if (Lib.CommonDataSource.selPIDList.Where(x => !String.IsNullOrEmpty(dto.DataName)).ToList().Count > 0)
                    {
                        _chk = false;
                        lvPID.SelectedValue = dto;
                    }
                }
            }
            if (flag != 2)
            {
                if (_totalselectedcnt == 1 && lvProcessSystem.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;
                //process system
                List<DataLibrary.ComboBoxDTO> tmpProcessSystem = list.GroupBy(x => new { x.ProcessSystem }).Select(x => new DataLibrary.ComboBoxDTO()
                {
                    DataName = x.Key.ProcessSystem
                }).ToList();

                if (lvProcessSystem.SelectedItems.Count > 0)
                    _chk = false;

                lvProcessSystem.ItemsSource = tmpProcessSystem.Where(x => !String.IsNullOrEmpty(x.DataName)).ToList();

                foreach (DataLibrary.ComboBoxDTO dto in lvProcessSystem.Items)
                {
                    if (Lib.CommonDataSource.selProcessSystemList.Where(x => !String.IsNullOrEmpty(dto.DataName)).ToList().Count > 0)
                    {
                        _chk = false;
                        lvProcessSystem.SelectedValue = dto;
                    }
                }
            }
            if (flag != 3)
            {
                if (_totalselectedcnt == 1 && lvLine.SelectedItems.Count > 0)
                    list = _commonsource.orgCollection;
                else
                    list = Lib.CommonDataSource.curCollection;

                //line
                List<DataLibrary.ComboBoxDTO> tmpLine = list.GroupBy(x => new { x.ISOLineNo}).Select(x => new DataLibrary.ComboBoxDTO()
                {
                    DataName = x.Key.ISOLineNo
                }).ToList();

                if (lvLine.SelectedItems.Count > 0)
                    _chk = false;

                lvLine.ItemsSource = tmpLine.Where(x => !String.IsNullOrEmpty(x.DataName)).ToList();

                foreach (DataLibrary.ComboBoxDTO dto in lvLine.Items)
                {
                    if (Lib.CommonDataSource.selLineList.Where(x => !String.IsNullOrEmpty(dto.DataName)).ToList().ToList().Count > 0)
                    {
                        _chk = false;
                        lvLine.SelectedValue = dto;
                    }
                }
            }
          
        }


        private void lvModule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(0);
            }
            else
                _chk = true;
        }

        private void lvPID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(1);
            }
            else
                _chk = true;
        }

        private void lvProcessSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_chk)
            {
                SetCategoryListBySelected();
                RebindAnotherCategory(2);
            }
            else
                _chk = true;
        }

        private void lvLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
