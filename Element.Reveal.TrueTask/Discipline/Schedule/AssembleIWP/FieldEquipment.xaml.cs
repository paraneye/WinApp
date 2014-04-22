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
using System.Collections.ObjectModel;
using DlhSoft.ProjectData.GanttChart.WinRT.Controls;
using Windows.UI;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FieldEquipment : WinAppLibrary.Controls.LayoutAwarePage
    {
        private List<DataLibrary.FieldequipmentDTO> _orgFieldequipment = new List<DataLibrary.FieldequipmentDTO>();
        private List<DataLibrary.FieldequipmentDTO> _libFieldequipment;
        private List<DataLibrary.FiwpequipDTO> _trgFiwpequip = new List<DataLibrary.FiwpequipDTO>();
        List<DataLibrary.FiwpequipDTO> _delFiwpequip = new List<DataLibrary.FiwpequipDTO>();
        DataLibrary.FiwpequipDTO _currentFiwpequip;
        Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
        List<DataLibrary.FiwpDTO> _iwps = new List<DataLibrary.FiwpDTO>();
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        Windows.UI.Xaml.Media.Animation.Storyboard _sbScheduleON, _sbScheduleOFF;
        private int _projectid, _fiwpid;
        private string _disciplineCode;
        private int selectedEquipID;

        public FieldEquipment()
        {
            this.InitializeComponent();

            this.txtQty.Value = 1;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;

            _fiwpid = Lib.IWPDataSource.selectedIWP;
            //LoadStoryBoardSwitch();
            LoadLibrary();

            Lib.WizardDataSource.SetTargetMenu(DataLibrary.Utilities.AssembleStep.EQUIPMENT, Lib.CommonDataSource.selPackageTypeLUID, true);

            this.ButtonBar.CurrentMenu = DataLibrary.Utilities.AssembleStep.EQUIPMENT;
            this.ButtonBar.Load();
        }
        

        private void LoadStoryBoardSwitch()
        {
            ////ToGridView
            //_sbScheduleOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            //_sbScheduleOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranslateSchedule, Window.Current.Bounds.Width, 0.5));

            //_sbScheduleON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            //_sbScheduleON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScheduleScale, 1, 0.5));
            //_sbScheduleON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranslateSchedule, 0, 0.5));

        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            
            //ITR <- Equipment
            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu, DataLibrary.Utilities.AssembleStep.ITR);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            _sbScheduleOFF.Begin();
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
            Login.MasterPage.Loading(true, this);

            if (lvSpec.SelectedItem != null)
            {
                try
                {
                    foreach (DataLibrary.ComboBoxDTO dto in lvSpec.SelectedItems)
                    {
                        DataLibrary.FiwpequipDTO newdto = new DataLibrary.FiwpequipDTO();

                        newdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                        newdto.Spec = GetSpec(dto);
                        newdto.SystemType = GetSystemType(dto);
                        newdto.EquipmentID = dto.DataID;
                        newdto.FIWPID = _fiwpid;
                        newdto.CWPID = Lib.CWPDataSource.selectedCWP;
                        newdto.ProjectID = _projectid;
                        newdto.DisciplineCode = _disciplineCode;
                        newdto.StartUseDate = _iwps.Count > 0 ? _iwps[0].StartDate : DateTime.Now;
                        newdto.FinishUseDate = _iwps.Count > 0 ? _iwps[0].StartDate : DateTime.Now;
                        newdto.UpdatedBy = Login.UserAccount.PersonnelId;
                        newdto.UpdatedDate = DateTime.Now;
                        newdto.EquipmentName = GetEquipmentName(dto);
                        newdto.Qty = 1;
                        _trgFiwpequip.Insert(0, newdto);
                    }

                    lvEquipment.ItemsSource = null;
                    lvEquipment.ItemsSource = _trgFiwpequip;
                
                    InitLibFieldEquipment();
                }
                catch (Exception ex)
                {

                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Add equipment", "There is a problem adding the selected item - Please try again later", "Error");
                }

            }
            Login.MasterPage.Loading(false, this);

        }

        private void RemoveItem()
        {
            Login.MasterPage.Loading(true, this);

            if (lvEquipment.SelectedItem != null)
            {
                try
                {
                    foreach (DataLibrary.FiwpequipDTO dto in lvEquipment.SelectedItems)
                    {
                        if (dto.FiwpEquipID > 0)
                        {
                            dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Delete;
                            _delFiwpequip.Add(dto);
                        }
                        _trgFiwpequip.Remove(dto);
                    }

                    lvEquipment.ItemsSource = null;
                    lvEquipment.ItemsSource = _trgFiwpequip;

                    //bindToChart(_trgFiwpequip);

                    InitLibFieldEquipment();

                }
                catch (Exception ex)
                {

                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Remove equipment", "There is a problem removing the selected item - Please try again later", "Error");
                }
            }
            Login.MasterPage.Loading(false, this);
        }

        private async void SaveDocument()
        {
            Login.MasterPage.Loading(true, this);

            List<DataLibrary.FiwpequipDTO> updatelist = new List<DataLibrary.FiwpequipDTO>();

            try
            {
                updatelist.AddRange(_trgFiwpequip.Where(x => x.DTOStatus == (int)DataLibrary.Utilities.RowStatus.Update));
                updatelist.AddRange(_trgFiwpequip.Where(x => x.DTOStatus == (int)DataLibrary.Utilities.RowStatus.New));
                updatelist.AddRange(_delFiwpequip);

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
                    fiwpdto[0].DocEstablishedLUID = DataLibrary.Utilities.AssembleStep.EQUIPMENT;

                List<DataLibrary.FiwpequipDTO> result;
                //if(updatelist.Count > 0)
                result = await (new Lib.ServiceModel.ProjectModel()).SaveFiwpequipForAssembleIWP(updatelist, fiwpdto, Login.UserAccount.PersonnelId);

                Lib.WizardDataSource.SetTargetMenu(DataLibrary.Utilities.AssembleStep.EQUIPMENT, Lib.CommonDataSource.selPackageTypeLUID, true);

                if (Lib.WizardDataSource.NextMenu != null)
                    this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Save Equipment", "There is a problem saving the selected item - Please try again later", "Error");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void lvCategory1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            lv.SelectedItem = e.ClickedItem;

            List<DataLibrary.FieldequipmentDTO> result = null;
            List<DataLibrary.ComboBoxDTO> tmp = null;

            if (lvCategory1.SelectedItem != null)
            {
                if (_libFieldequipment == null)
                    return;

                //category2
                result = _libFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as DataLibrary.ComboBoxDTO).DataName && !string.IsNullOrEmpty(x.Category2)).ToList();

                tmp = result.GroupBy(x => new { x.Category2 }).Select(x => new DataLibrary.ComboBoxDTO()
                {
                    DataName = x.Key.Category2,
                    DataID = 0
                }).ToList();

                if (tmp.Count > 0)
                {
                    lvCategory2.ItemsSource = tmp;
                    lvCategory3.ItemsSource = null;
                    lvSpec.ItemsSource = null;
                }
                else
                {
                    lvCategory2.ItemsSource = null;

                    //category3
                    result = _libFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as DataLibrary.ComboBoxDTO).DataName && !string.IsNullOrEmpty(x.Category3)).ToList();

                    tmp = result.GroupBy(x => new { x.Category3 }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.Category3,
                        DataID = 0
                    }).ToList();

                    if (tmp.Count > 0)
                    {
                        lvCategory3.ItemsSource = tmp;
                        lvSpec.ItemsSource = null;
                    }
                    else
                    {
                        lvCategory3.ItemsSource = null;

                        //category4
                        result = _libFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as DataLibrary.ComboBoxDTO).DataName).ToList();

                        tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new DataLibrary.ComboBoxDTO()
                        {
                            DataName = x.Key.Spec + (string.IsNullOrEmpty(x.Key.SystemType) ? "" : ", " + x.Key.SystemType),
                            DataID = x.Key.FieldEquipmentID
                        }).ToList();

                        if (tmp.Count > 0)
                            lvSpec.ItemsSource = tmp;
                        else
                            lvSpec.ItemsSource = null;
                    }

                }

            }
        }

        private void lvCategory2_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            lv.SelectedItem = e.ClickedItem;

            List<DataLibrary.FieldequipmentDTO> result = null;
            List<DataLibrary.ComboBoxDTO> tmp = null;

            if (lvCategory2.SelectedItem != null)
            {
                if (_libFieldequipment == null)
                    return;

                result = _libFieldequipment.Where(x => x.Category2 == (lvCategory2.SelectedItem as DataLibrary.ComboBoxDTO).DataName && !string.IsNullOrEmpty(x.Category3)).ToList();

                tmp = result.GroupBy(x => new { x.Category3 }).Select(x => new DataLibrary.ComboBoxDTO()
                {
                    DataName = x.Key.Category3,
                    DataID = 0
                }).ToList();

                if (tmp.Count > 0)
                {
                    lvCategory3.ItemsSource = tmp;
                    lvSpec.ItemsSource = null;
                }
                else
                {
                    lvCategory3.ItemsSource = null;

                    //category4
                    result = _libFieldequipment.Where(x => x.Category2 == (lvCategory2.SelectedItem as DataLibrary.ComboBoxDTO).DataName).ToList();

                    tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new DataLibrary.ComboBoxDTO()
                    {
                        DataName = x.Key.Spec + (string.IsNullOrEmpty(x.Key.SystemType) ? "" : ", " + x.Key.SystemType),
                        DataID = x.Key.FieldEquipmentID
                    }).ToList();

                    if (tmp.Count > 0)
                        lvSpec.ItemsSource = tmp;
                    else
                        lvSpec.ItemsSource = null;
                }
            }
        }

        private void lvCategory3_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            lv.SelectedItem = e.ClickedItem;

            List<DataLibrary.FieldequipmentDTO> result = null;
            List<DataLibrary.ComboBoxDTO> tmp = null;

            if (lvCategory3.SelectedItem != null)
            {
                if (_libFieldequipment == null)
                    return;

                result = _libFieldequipment.Where(x => x.Category3 == (lvCategory3.SelectedItem as DataLibrary.ComboBoxDTO).DataName).ToList();

                tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new DataLibrary.ComboBoxDTO()
                {
                    DataName = x.Key.Spec + (string.IsNullOrEmpty(x.Key.SystemType) ? "" : ", " + x.Key.SystemType),
                    DataID = x.Key.FieldEquipmentID
                }).ToList();
            }


            lvSpec.ItemsSource = tmp;
        }

        private void lvEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                selectedEquipID = ((DataLibrary.FiwpequipDTO)e.AddedItems[0]).EquipmentID;

                //txtSpec.Text = ((DataLibrary.FiwpequipDTO)e.AddedItems[0]).Spec;
                //if(!string.IsNullOrEmpty(((DataLibrary.FiwpequipDTO)e.AddedItems[0]).SystemType))
                //    txtType.Text = ((DataLibrary.FiwpequipDTO)e.AddedItems[0]).SystemType;
                if (Convert.ToDouble(((DataLibrary.FiwpequipDTO)e.AddedItems[0]).Qty) > 0)
                {
                    txtQty.Value = Convert.ToDouble(((DataLibrary.FiwpequipDTO)e.AddedItems[0]).Qty);
                }
                else
                {
                    txtQty.Value = 1;
                }
                txtStartDT.SelectedDate = ((DataLibrary.FiwpequipDTO)e.AddedItems[0]).StartUseDate;
                txtEndDT.SelectedDate = ((DataLibrary.FiwpequipDTO)e.AddedItems[0]).FinishUseDate;
            }
            else 
            {
                //txtSpec.Text = string.Empty;
                //txtType.Text = string.Empty;
                txtQty.Value = 1;
                this.txtStartDT.SelectedDate = _iwps[0].StartDate;
                this.txtEndDT.SelectedDate = _iwps[0].FinishDate;
            }
        }


        private void lvSpec_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {

        }

        private void lvEquipment_Drop(object sender, DragEventArgs e)
        {
            AddItem();
        }

        private void txtQty_TextChanged(object sender, C1.Xaml.PropertyChangedEventArgs<double> e)
        {
            if (lvEquipment.SelectedItem != null)
            {
                foreach (DataLibrary.FiwpequipDTO tdto in _trgFiwpequip)
                {
                    if (tdto.EquipmentID == selectedEquipID)
                    {
                        if ((lvEquipment.SelectedItem as DataLibrary.FiwpequipDTO).FiwpEquipID > 0)
                            tdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                        else
                            tdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                        
                        tdto.Qty = Convert.ToInt32(txtQty.Value);
                        tdto.UpdatedDate = DateTime.Now;
                    }
                }
            }
        }



        private void txtStartDT_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvEquipment.SelectedItem != null)
            {
                //시작일-종료일 날짜 유효성 체크
                CheckDate(e);

                foreach (DataLibrary.FiwpequipDTO tdto in _trgFiwpequip)
                {
                    if (tdto.EquipmentID == selectedEquipID)
                    {
                        if ((lvEquipment.SelectedItem as DataLibrary.FiwpequipDTO).FiwpEquipID > 0)
                            tdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                        else
                            tdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                        
                        tdto.StartUseDate = Convert.ToDateTime(txtStartDT.Text);
                        tdto.UpdatedDate = DateTime.Now;
                    }
                }
            }
        }

        private void txtEndDT_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvEquipment.SelectedItem != null)
            {
                //시작일-종료일 날짜 유효성 체크
                CheckDate(e);

                foreach (DataLibrary.FiwpequipDTO tdto in _trgFiwpequip)
                {
                    if (tdto.EquipmentID == selectedEquipID)
                    {
                        if ((lvEquipment.SelectedItem as DataLibrary.FiwpequipDTO).FiwpEquipID > 0)
                            tdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                        else
                            tdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;

                        tdto.FinishUseDate = Convert.ToDateTime(this.txtEndDT.Text);
                        tdto.UpdatedDate = DateTime.Now;
                    }
                }
            }
        }

        private void CheckDate(SelectionChangedEventArgs e)
        {
            DateTime _startdate = Convert.ToDateTime(txtStartDT.Text);
            DateTime _finishdate = Convert.ToDateTime(txtEndDT.Text);

            TimeSpan ts = _finishdate - _startdate;
            int diffDay = ts.Days;

            if (diffDay < 0)//종료일이 시작일보다 빠르면 경고 문
            {
                //if (e != null && e.RemovedItems.Count > 0)
                //    txtEndDT.Text = Convert.ToDateTime(e.RemovedItems[0]).ToString("MM/dd/yyyy");

                txtEndDT.Text = Convert.ToDateTime(txtStartDT.Text).ToString("MM/dd/yyyy");

                WinAppLibrary.Utilities.Helper.SimpleMessage("End date can not be exceed start date", "Warning!");
            }
        }

        #endregion

        #region "Private Method"

        private void InitLibFieldEquipment()
        {
            _libFieldequipment = new List<DataLibrary.FieldequipmentDTO>();

            if (_orgFieldequipment == null || _trgFiwpequip == null)
                return;

            foreach (DataLibrary.FieldequipmentDTO dto in _orgFieldequipment)
            {
                if (_trgFiwpequip.Where(x => x.EquipmentID == dto.FieldEquipmentID).Count() < 1)
                    _libFieldequipment.Add(dto);
            }

            //Category 1  먼저
            List<DataLibrary.ComboBoxDTO> lstCategory1 = _libFieldequipment.GroupBy(x => new { x.Category1 }).Select(x => new DataLibrary.ComboBoxDTO()
            {
                DataName = x.Key.Category1,
                DataID = 0
            }).ToList();

            lvCategory1.ItemsSource = lstCategory1;
            lvCategory2.ItemsSource = null;
            lvCategory3.ItemsSource = null;
            lvSpec.ItemsSource = null;
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
                    List<DataLibrary.FieldequipmentDTO> result;
                    result = await (new Lib.ServiceModel.CommonModel()).GetFieldequipmentByType(string.Empty);
                    if (result != null)
                        _orgFieldequipment = result;

                    List<DataLibrary.FiwpequipDTO> trgresult;
                    trgresult = await (new Lib.ServiceModel.ProjectModel()).GetFiwpEquipByFIWP(_fiwpid);
                    if (trgresult != null)
                        _trgFiwpequip = trgresult;

                    //전체 리스트에서 Category 1 먼저 세팅
                    InitLibFieldEquipment();

                    lvEquipment.ItemsSource = _trgFiwpequip;

                    await _iwp.GetFiwpByIDOnMode(_fiwpid);
                    _iwps = _iwp.GetFiwpByID();

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Load Equipment", "There is a problem loading equipment data - Please try again later", "Loading Error");
            }


            Login.MasterPage.Loading(false, this);

            SetDate();
        }

        //날짜는 iwp의 스케출 날짜 기본으로 세팅
        private void SetDate()
        {
            if (_iwps.Count > 0)
            {
                //this.txtStartDT.Text = _iwps[0].StartDate.ToString("mm/dd/yyyy");
                //this.txtEndDT.Text = _iwps[0].FinishDate.ToString("mm/dd/yyyy");

                this.txtStartDT.SelectedDate = _iwps[0].StartDate;
                this.txtEndDT.SelectedDate = _iwps[0].FinishDate;

                this.txtStartDT.DisplayDateStart = _iwps[0].StartDate;
                this.txtStartDT.DisplayDateEnd = _iwps[0].FinishDate;
                this.txtEndDT.DisplayDateStart = _iwps[0].StartDate;
                this.txtEndDT.DisplayDateEnd = _iwps[0].FinishDate;
            }
        }

        //private void Bind_Spec()
        //{
        //    List<DataLibrary.FieldequipmentDTO> result = null;
        //    List<DataLibrary.ComboBoxDTO> tmp = null;

        //    if (_libFieldequipment == null)
        //        return;

        //    if (lvCategory3.SelectedItem != null)
        //        result = _libFieldequipment.Where(x => x.Category3 == (lvCategory3.SelectedItem as DataLibrary.ComboBoxDTO).DataName).ToList();
        //    else if (lvCategory2.SelectedItem != null)
        //        result = _libFieldequipment.Where(x => x.Category2 == (lvCategory2.SelectedItem as DataLibrary.ComboBoxDTO).DataName).ToList();
        //    else
        //        result = _libFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as DataLibrary.ComboBoxDTO).DataName).ToList();

        //    tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new DataLibrary.ComboBoxDTO()
        //    {
        //        DataName = x.Key.Spec + (string.IsNullOrEmpty(x.Key.SystemType) ? "" : ", " + x.Key.SystemType),
        //        DataID = x.Key.FieldEquipmentID
        //    }).ToList();

        //    lvSpec.ItemsSource = tmp;
        //}

        private string GetSpec(DataLibrary.ComboBoxDTO dto)
        {
            string rtn = string.Empty;

            rtn = dto.DataName.Split(',')[0];

            return rtn;
        }

        private string GetSystemType(DataLibrary.ComboBoxDTO dto)
        {
            string rtn = string.Empty;

            string tmp = dto.DataName;

            if (tmp.Split(',').Count() > 1)
                rtn = tmp.Split(',')[1];

            return rtn;
        }

        private string GetEquipmentName(DataLibrary.ComboBoxDTO dto)
        {
            string rtn = string.Empty;
            string prefix = string.Empty;

            if (lvCategory3.SelectedItem != null)
            {
                prefix = (lvCategory3.SelectedItem as DataLibrary.ComboBoxDTO).DataName;
            }
            else
            {
                if (lvCategory2.SelectedItem != null)
                {
                    prefix = (lvCategory2.SelectedItem as DataLibrary.ComboBoxDTO).DataName;
                }
                else
                {
                    if (lvCategory1.SelectedItem != null)
                    {
                        prefix = (lvCategory1.SelectedItem as DataLibrary.ComboBoxDTO).DataName;
                    }
                }
            }

            rtn = prefix + "-" + dto.DataName;

            return rtn;
        }


        /* 차트
        private void bindToChart(List<DataLibrary.FiwpequipDTO> list)
        {
            DateTime dtMin = new DateTime();
            DateTime dtMax = new DateTime();

            ObservableCollection<GanttChartItem> items = new ObservableCollection<GanttChartItem>();

            gccSchedule.UseSimpleDateFormatting = true;
            gccSchedule.AreTaskDependenciesVisible = false;
            gccSchedule.IsTaskCompletedEffortVisible = false;

            ////test for altering holidays.
            //gccSchedule.WorkingWeekStart = 0;
            //gccSchedule.WorkingWeekFinish = 6;
            //gccSchedule.SpecialNonworkingDays.Clear();
            //gccSchedule.SpecialNonworkingDays.Add(new DateTimeOffset(Convert.ToDateTime("2013-06-03")));
            //gccSchedule.SpecialNonworkingDays.Add(new DateTimeOffset(Convert.ToDateTime("2013-06-04")));
            //gccSchedule.SpecialNonworkingDays.Add(new DateTimeOffset(Convert.ToDateTime("2013-06-05")));
            //gccSchedule.SpecialNonworkingDays.Add(new DateTimeOffset(Convert.ToDateTime("2013-06-06")));
            //gccSchedule.SpecialNonworkingDays.Add(new DateTimeOffset(Convert.ToDateTime("2013-06-07")));

            string barColor = "Orange";
            foreach (DataLibrary.FiwpequipDTO dto in list)
            {
                if (barColor == "Red")
                    barColor = "Orange";
                else
                    barColor = "Red";

                GanttChartItem item = new GanttChartItem();

                item.Content = "<div style='text-align: left; width: 170px; font-size: 15px; word-wrap:break-word; margin: 5px;'>" + dto.EquipmentName + "</div>";
                item.Start = new DateTimeOffset(dto.StartUseDate == null ? new DateTime() : Convert.ToDateTime(dto.StartUseDate.ToString("MMM dd, yyyy")));
                item.Finish = new DateTimeOffset(dto.FinishUseDate == null ? new DateTime() : Convert.ToDateTime(dto.FinishUseDate.ToString("MMM dd, yyyy") + " 11:59:59 PM"));
                item.Tag = dto;

                item.IsMilestone = false;
                item.TaskTemplateClientCode = @"
                var control = item.ganttChartView, settings = control.settings, document = control.ownerDocument, svgns = 'http://www.w3.org/2000/svg';

                // Create, if needed, and store a SVG group element that will represent the task bar area.
                if (typeof item.taskBarAreaContainerGroup === 'undefined')
                    item.taskBarAreaContainerGroup = document.createElementNS(svgns, 'g');

                var containerGroup = item.taskBarAreaContainerGroup;

                for (var i = containerGroup.childNodes.length; i-- > 0; )
                    containerGroup.removeChild(containerGroup.childNodes[i]);

                // Determine item positioning values.
                var itemLeft = control.getChartPosition(item.start, settings), itemRight = control.getChartPosition(item.finish, settings), itemCompletedRight = control.getChartPosition(item.completedFinish, settings);

                // SVG rectangle element that will represent the main bar of the task (blue).
                var rect = document.createElementNS(svgns, 'rect'); 
                rect.setAttribute('x', itemLeft); 
                rect.setAttribute('width', Math.max(" + gccSchedule.HourWidth * 8 + @", itemRight - itemLeft));
                rect.setAttribute('y', settings.barMargin - 0.5); 
                rect.setAttribute('height', settings.barHeight + 0.5);
                rect.setAttribute('style', 'stroke: Blue; fill: LightBlue; stroke-width: 0.65px; fill-opacity: 0.5;');
                containerGroup.appendChild(rect);

                if (settings.isTaskCompletedEffortVisible) {
                    // SVG rectangle element that will represent the completion bar of the task (gray).
                    var completionRect = document.createElementNS(svgns, 'rect');
                    completionRect.setAttribute('x', itemLeft);
                    completionRect.setAttribute('y', settings.barMargin + settings.completedBarMargin - 0.5);
                    completionRect.setAttribute('width', Math.max(0, itemCompletedRight - itemLeft));
                    completionRect.setAttribute('height', settings.completedBarHeight + 0.5);
                    completionRect.setAttribute('style', 'stroke: Gray; fill: Gray; stroke-width: 0.65px; fill-opacity: 0.5;');
                    containerGroup.appendChild(completionRect);
                }

                // SVG rectangle element that will behave as a task bar thumb, providing horizontal drag and drop operations for the main task bar.
                var thumb = document.createElementNS(svgns, 'rect');
                thumb.setAttribute('x', itemLeft); 
                thumb.setAttribute('width', Math.max(" + gccSchedule.HourWidth * 8 + @", itemRight - itemLeft));
                thumb.setAttribute('y', settings.barMargin); 
                thumb.setAttribute('height', settings.barHeight + 0.5);
                thumb.setAttribute('style', 'stroke: " + barColor + @"; stroke-width: 1px; fill: " + barColor + @"; fill-opacity: 0.9; cursor: move');
                containerGroup.appendChild(thumb);
                    
                // SVG rectangle element that will behave as a task start time editing thumb, providing horizontal drag and drop operations for the left end of the main task bar.
                var startThumb = document.createElementNS(svgns, 'rect');
                startThumb.setAttribute('x', Math.max(itemLeft, Math.min(itemRight - " + gccSchedule.HourWidth * 8 + @", itemLeft)) - 25);
                startThumb.setAttribute('y', settings.barMargin);
                startThumb.setAttribute('width', 25);
                startThumb.setAttribute('height', settings.barHeight + 0.5);
                startThumb.setAttribute('style', 'fill: " + barColor + @"; fill-opacity: 0.1; cursor: e-resize'); //'fill: Transparent; cursor: e-resize');
                containerGroup.appendChild(startThumb);

                // SVG rectangle element that will behave as a task finish time editing thumb, providing horizontal drag and drop operations for the right end of the main task bar.
                var finishThumb = document.createElementNS(svgns, 'rect');
                finishThumb.setAttribute('x', Math.max(itemLeft + " + gccSchedule.HourWidth * 8 + @", itemRight));                
                finishThumb.setAttribute('y', settings.barMargin);
                finishThumb.setAttribute('width', 25);
                finishThumb.setAttribute('height', settings.barHeight + 0.5);
                finishThumb.setAttribute('style', 'fill: " + barColor + @"; fill-opacity: 0.1; cursor: e-resize'); //'fill: Transparent; cursor: e-resize');
                containerGroup.appendChild(finishThumb);

                control.initializeTaskDraggingThumbs(thumb, startThumb, finishThumb, null, item, itemLeft, itemRight, null);                    

                if (settings.areTaskDependenciesVisible) {
                    // SVG circle element that will behave as a task dependency creation thumb, providing vertical drag and drop operations for the right end of the main task bar.
                    var dependencyThumb = document.createElementNS(svgns, 'circle');
                    dependencyThumb.setAttribute('cx', itemRight);
                    dependencyThumb.setAttribute('cy', settings.barMargin + settings.barHeight / 2);
                    dependencyThumb.setAttribute('r', settings.barHeight / 4);
                    dependencyThumb.setAttribute('style', 'fill: Transparent; cursor: pointer');
                    containerGroup.appendChild(dependencyThumb); 
                    control.initializeDependencyDraggingThumb(dependencyThumb, containerGroup, item, settings.barMargin + settings.barHeight / 2, itemRight);
                }
                return containerGroup;";

                if (_iwps.Count > 0)
                {
                    item.MinStart = _iwps[0].StartDate;
                    item.MaxStart = _iwps[0].FinishDate;
                    item.MinFinish = _iwps[0].StartDate;
                    item.MaxFinish = _iwps[0].FinishDate;
                }

                items.Add(item);
            }

            if (_iwps.Count > 0)
            {

                if (dtMin == new DateTime() || dtMin > _iwps[0].StartDate)
                    dtMin = _iwps[0].StartDate;
                if (dtMax == new DateTime() || dtMax < _iwps[0].FinishDate)
                    dtMax = _iwps[0].FinishDate;


                gccSchedule.Items = items;

                // Set the scrollable timeline to present, and the displayed and current time values to automatically scroll to a specific chart coordinate, and display a vertical bar highlighter at the specified point.
                gccSchedule.DisplayedTime = new DateTimeOffset(dtMin.AddDays(-3));
                gccSchedule.CurrentTime = new DateTimeOffset(dtMin);
                //gccSchedule.TimelineStart = new DateTimeOffset(dtMin == new DateTime() ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : dtMin.AddYears(-1));
                //gccSchedule.TimelineFinish = new DateTimeOffset(dtMax == new DateTime() ? DateTime.Now.AddYears(1) : dtMax.AddYears(1));
                gccSchedule.TimelineStart = new DateTimeOffset(dtMin == new DateTime() ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : dtMin);
                gccSchedule.TimelineFinish = new DateTimeOffset(dtMax == new DateTime() ? DateTime.Now.AddYears(1) : dtMax);

                //TimeSpan ts = _iwps[0].StartDate - _iwps[0].FinishDate;
                //gccSchedule.Width = 200 + (Math.Abs(ts.Days) * 20);
            }

            gccSchedule.IsGridReadOnly = true;
            gccSchedule.IsChartReadOnly = false;
            gccSchedule.IsCurrentTimeLineVisible = false;
            gccSchedule.IsTaskToolTipVisible = false;
            gccSchedule.IsHoldingEnabled = false;

            gccSchedule.ItemHeight = 70;
            gccSchedule.BarHeight = 50;
            gccSchedule.BarMargin = 10;

            gccSchedule.SelectedItemBackgroundColor = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#7da7d9").Color;
            gccSchedule.SelectedItemForegroundColor = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#004a80").Color;
            gccSchedule.HeaderBackgroundColor = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#004a80").Color;

            //SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Gray);
            //mySolidColorBrush.Opacity = 0.25;
            //gccSchedule.Background = mySolidColorBrush;



            gccSchedule.Columns[1].Header = "<div style='text-align: center;'>IWP</div>";
            gccSchedule.Columns[1].Width = 200;
            gccSchedule.Columns[2].IsVisible = false;
            gccSchedule.Columns[3].IsVisible = false;
            gccSchedule.Columns[4].IsVisible = false;
            gccSchedule.Columns[5].IsVisible = false;
            gccSchedule.Columns[6].IsVisible = false;
        }

        private void addItemToChart(DataLibrary.FiwpequipDTO dto)
        {
            string barColor = "Orange";

            GanttChartItem item = new GanttChartItem();

            item.Content = "<div style='text-align: left; width: 170px; font-size: 15px; word-wrap:break-word; margin: 5px;'>" + dto.EquipmentName + "</div>";
            item.Start = new DateTimeOffset(dto.StartUseDate == null ? new DateTime() : Convert.ToDateTime(dto.StartUseDate.ToString("MMM dd, yyyy")));
            item.Finish = new DateTimeOffset(dto.FinishUseDate == null ? new DateTime() : Convert.ToDateTime(dto.FinishUseDate.ToString("MMM dd, yyyy") + " 11:59:59 PM"));
            item.Tag = dto;

            item.IsMilestone = false;
            item.TaskTemplateClientCode = @"
                var control = item.ganttChartView, settings = control.settings, document = control.ownerDocument, svgns = 'http://www.w3.org/2000/svg';

                // Create, if needed, and store a SVG group element that will represent the task bar area.
                if (typeof item.taskBarAreaContainerGroup === 'undefined')
                    item.taskBarAreaContainerGroup = document.createElementNS(svgns, 'g');

                var containerGroup = item.taskBarAreaContainerGroup;

                for (var i = containerGroup.childNodes.length; i-- > 0; )
                    containerGroup.removeChild(containerGroup.childNodes[i]);

                // Determine item positioning values.
                var itemLeft = control.getChartPosition(item.start, settings), itemRight = control.getChartPosition(item.finish, settings), itemCompletedRight = control.getChartPosition(item.completedFinish, settings);

                // SVG rectangle element that will represent the main bar of the task (blue).
                var rect = document.createElementNS(svgns, 'rect'); 
                rect.setAttribute('x', itemLeft); 
                rect.setAttribute('width', Math.max(" + gccSchedule.HourWidth * 8 + @", itemRight - itemLeft));
                rect.setAttribute('y', settings.barMargin - 0.5); 
                rect.setAttribute('height', settings.barHeight + 0.5);
                rect.setAttribute('style', 'stroke: Blue; fill: LightBlue; stroke-width: 0.65px; fill-opacity: 0.5;');
                containerGroup.appendChild(rect);

                if (settings.isTaskCompletedEffortVisible) {
                    // SVG rectangle element that will represent the completion bar of the task (gray).
                    var completionRect = document.createElementNS(svgns, 'rect');
                    completionRect.setAttribute('x', itemLeft);
                    completionRect.setAttribute('y', settings.barMargin + settings.completedBarMargin - 0.5);
                    completionRect.setAttribute('width', Math.max(0, itemCompletedRight - itemLeft));
                    completionRect.setAttribute('height', settings.completedBarHeight + 0.5);
                    completionRect.setAttribute('style', 'stroke: Gray; fill: Gray; stroke-width: 0.65px; fill-opacity: 0.5;');
                    containerGroup.appendChild(completionRect);
                }

                // SVG rectangle element that will behave as a task bar thumb, providing horizontal drag and drop operations for the main task bar.
                var thumb = document.createElementNS(svgns, 'rect');
                thumb.setAttribute('x', itemLeft); 
                thumb.setAttribute('width', Math.max(" + gccSchedule.HourWidth * 8 + @", itemRight - itemLeft));
                thumb.setAttribute('y', settings.barMargin); 
                thumb.setAttribute('height', settings.barHeight + 0.5);
                thumb.setAttribute('style', 'stroke: " + barColor + @"; stroke-width: 1px; fill: " + barColor + @"; fill-opacity: 0.9; cursor: move');
                containerGroup.appendChild(thumb);
                    
                // SVG rectangle element that will behave as a task start time editing thumb, providing horizontal drag and drop operations for the left end of the main task bar.
                var startThumb = document.createElementNS(svgns, 'rect');
                startThumb.setAttribute('x', Math.max(itemLeft, Math.min(itemRight - " + gccSchedule.HourWidth * 8 + @", itemLeft)) - 25);
                startThumb.setAttribute('y', settings.barMargin);
                startThumb.setAttribute('width', 25);
                startThumb.setAttribute('height', settings.barHeight + 0.5);
                startThumb.setAttribute('style', 'fill: " + barColor + @"; fill-opacity: 0.1; cursor: e-resize'); //'fill: Transparent; cursor: e-resize');
                containerGroup.appendChild(startThumb);

                // SVG rectangle element that will behave as a task finish time editing thumb, providing horizontal drag and drop operations for the right end of the main task bar.
                var finishThumb = document.createElementNS(svgns, 'rect');
                finishThumb.setAttribute('x', Math.max(itemLeft + " + gccSchedule.HourWidth * 8 + @", itemRight));                
                finishThumb.setAttribute('y', settings.barMargin);
                finishThumb.setAttribute('width', 25);
                finishThumb.setAttribute('height', settings.barHeight + 0.5);
                finishThumb.setAttribute('style', 'fill: " + barColor + @"; fill-opacity: 0.1; cursor: e-resize'); //'fill: Transparent; cursor: e-resize');
                containerGroup.appendChild(finishThumb);

                control.initializeTaskDraggingThumbs(thumb, startThumb, finishThumb, null, item, itemLeft, itemRight, null);                    

                if (settings.areTaskDependenciesVisible) {
                    // SVG circle element that will behave as a task dependency creation thumb, providing vertical drag and drop operations for the right end of the main task bar.
                    var dependencyThumb = document.createElementNS(svgns, 'circle');
                    dependencyThumb.setAttribute('cx', itemRight);
                    dependencyThumb.setAttribute('cy', settings.barMargin + settings.barHeight / 2);
                    dependencyThumb.setAttribute('r', settings.barHeight / 4);
                    dependencyThumb.setAttribute('style', 'fill: Transparent; cursor: pointer');
                    containerGroup.appendChild(dependencyThumb); 
                    control.initializeDependencyDraggingThumb(dependencyThumb, containerGroup, item, settings.barMargin + settings.barHeight / 2, itemRight);
                }
                return containerGroup;";

            gccSchedule.Items.Add(item);
            gccSchedule.SelectedItem = item;
            gccSchedule.ScrollToBottom();
            gccSchedule.Items[0].MaxFinish = DateTime.Now;
            gccSchedule.UpdateLayout();

        }

        private void gccSchedule_ItemPropertyChanged(object sender, GanttChartItemPropertyChangedEventArgs e)
        {
            if (e.Item != null && e.IsDirect && e.IsFinal)
            {
                _currentFiwpequip = e.Item.Tag as DataLibrary.FiwpequipDTO;
                //gccSchedule.SelectedItem = e.Item;

                if (_currentFiwpequip != null)
                {
                    foreach (DataLibrary.FiwpequipDTO dto in _trgFiwpequip)
                    {
                        if (dto == _currentFiwpequip)
                        {
                            if (_currentFiwpequip.FiwpEquipID > 0)
                                dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                            else
                                dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;

                            dto.StartUseDate = Convert.ToDateTime(e.Item.Start.DateTime.ToString("MMM dd, yyyy"));
                            dto.FinishUseDate = Convert.ToDateTime(e.Item.Finish.DateTime.ToString("MMM dd, yyyy"));
                            dto.UpdatedDate = DateTime.Now;
                        }
                    }
                }
            }
        }

        private void btnScChart_Click(object sender, RoutedEventArgs e)
        {
            _sbScheduleON.Begin();
        }
        */
        #endregion


    }

    
}
