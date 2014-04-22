﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

namespace Element.Reveal.TrueTask.Discipline.Schedule.BuildIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScheduleComponents : WinAppLibrary.Controls.LayoutAwarePage
    {
        private Lib.ScheduleDataSource sc = new Lib.ScheduleDataSource();
        private int _projectid; private string _disciplineCode;
        private Lib.ObjectParam _obj;
        private Windows.UI.Xaml.Media.Animation.DoubleAnimation dbaniRotate = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
        private Dictionary<string, List<int>> _rotationangles = new Dictionary<string, List<int>>();
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.IWPDataSource _iwpsource = new Lib.IWPDataSource();
        List<DataLibrary.ComboBoxDTO> orgDrawingList = new List<DataLibrary.ComboBoxDTO>();
        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbFlipView, _sbUnAssignSortOFF, _sbUnAssignSortON, _sbAssignSortOFF, _sbAssignSortON;
        private int count = 0;
        public ScheduleComponents()
        {
            this.InitializeComponent();
           
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            // TODO: Create an appropriate data model 
            tbScheduleItemName.Text = Lib.IWPDataSource.selectedIWPName;
            LoadScheduleTime();
            Loaddrawing(navigationParameter);
            LoadStoryBoardSwitch();
            lvUnAssignSort.ItemsSource = _commonsource.GetSortingItems();
            lvAssignSort.ItemsSource = _commonsource.GetSortingItems();

           
        }
        

        private async void LoadScheduleTime()
        {
            try
            {
                if (sc.ScheduleJson.Count > 0)
                {
                    scheduleDate.Text = sc.ScheduleJson[0].StartDate.ToString("MM/dd/yyyy") + "~" + sc.ScheduleJson[0].FinishDate.ToString("MM/dd/yyyy");
                    StartDate.Text = Convert.ToDateTime(Lib.IWPDataSource.selectIwpStart).ToString("MM/dd/yyyy");
                    FinishDate.Text = Convert.ToDateTime(Lib.IWPDataSource.selectIWpEnd).ToString("MM/dd/yyyy");
                    tbNoCrew.Text = Lib.IWPDataSource.selectEstimateManHour;
                }
                else
                {
                    StartDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
                    FinishDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
                }
            }
            catch
            {

            }
        }        

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void gvViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string id;
            //object selected;

            //switch (gvViewType.SelectedIndex)
            //{
            //    //Grid
            //    case 0:
            //        if (this.FlipView.SelectedItem != null)
            //        {
            //            id = (this.FlipView.SelectedItem as DataItem).UniqueId;
            //            selected = lvDrawing.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
            //            if (selected != null)
            //                lvDrawing.SelectedItem = selected;
            //        }
            //        sbShiftLeft.Begin();
            //        _sbGridView.Begin(); ;
            //        break;
            //    //"Flip"
            //    case 1:
            //        if (lvDrawing.SelectedItem != null)
            //        {
            //            id = (lvDrawing.SelectedItem as DataItem).UniqueId;
            //            selected = this.FlipView.Items.Where(x => (x as DataItem).UniqueId == id).FirstOrDefault();
            //            if (selected != null)
            //                this.FlipView.SelectedItem = selected;
            //        }

            //        sbShiftRight.Begin();
            //        _sbFlipView.Begin();
            //        break;
            //}
        }

        private void FlipViewItem_Clicked(object sender, object e)
        {

            var item = e as DataItem;
            if (item != null)
            {
                //var drawing = _commonsource.Drawing.Where(x => x.DataID.ToString() == item.UniqueId).FirstOrDefault();
                if (orgDrawingList == null)
                    return;

                var drawing = orgDrawingList.Where(x => x.DataID.ToString() == item.UniqueId).FirstOrDefault();
                //DrawingManipulation.LoadDrawing(drawing.ExtraValue1, drawing.ExtraValue2, UriKind.Absolute);
                DrawingManipulation.LoadDrawing(drawing.ExtraValue1, "", UriKind.Absolute);
                DrawingManipulation.Show();
            }
        }

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private async void Loaddrawing(Object navigationParameter)
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.HideUserStatus();
            //StretchingPanel.Stretch(false);

            _obj = navigationParameter as Lib.ObjectParam;

            List<DataGroup> source = null;

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _commonsource.GetDrawingForAssignIWPOnMode(Lib.CWPDataSource.selectedCWP, Lib.ScheduleDataSource.selectedSchedule, 0, _obj.taskCategoryIdList, _obj.taskTypeLUIDList, _obj.materialIDList, _obj.progressIDList,
                       _obj.searhValue, _projectid, _disciplineCode, -1);
                    if (_commonsource.Drawing != null && _commonsource.Drawing.Count > 0)
                    {
                        source = _commonsource.GetGroupedDrawing();
                        orgDrawingList = _commonsource.Drawing;
                    }

                    await _commonsource.GetComponentProgressByFIWP(Lib.IWPDataSource.selectedIWP, Lib.ScheduleDataSource.selectedSchedule, _projectid, _disciplineCode);
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "IWPGridViewer Loaddrawing", "There is a problem loading the drawings - Please try again later", "Error!");
            }

            this.DefaultViewModel["Drawings"] = source;
            //this.FlipView.SelectedItem = this.gvDrawing.SelectedItem = null;
            this.gvViewType.SelectedIndex = 0;
            this.SubLayoutRoot.Visibility = Visibility.Visible;
            LoadAssignComponent();
            Login.MasterPage.Loading(false, this);           
            
        }

        /// <summary>
        /// 전부 assign 되었어도 데이터 확인
        /// </summary>
        private async void LoadAssignComponent()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (lvAssignedComponent.Items.Count == 0)
                {
                    lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();
                    tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
                    tbAssignCnt.Text = "Number of Components : " + _commonsource.AssignedCnt;
                }
            });
        }

        private void lvDrawing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                int drawingId = Convert.ToInt32((e.AddedItems[0] as DataItem).UniqueId);

                lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId, Lib.IWPDataSource.selectedIWP);
                tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
                tbUnAssignCnt.Text = "Number of Components : " + _commonsource.UnAssignedCnt;
                FlipImage.Source = ((e.AddedItems[0] as DataItem).Image);
            }
            else
            {
                lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(0, Lib.IWPDataSource.selectedIWP);
                tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
                tbUnAssignCnt.Text = "Number of Components : " + _commonsource.UnAssignedCnt;
                FlipImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + Lib.ContentPath.DefaultDrawing));
            }

            lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();
            tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
            tbAssignCnt.Text = "Number of Components : " + _commonsource.AssignedCnt;
        }

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbGridView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            //_sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(lvDrawing, 1, 0, ANIMATION_SPEED));
            //_sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.SLayoutRoot, 1, 0, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.FlipView, 0, 0, 0));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, -1200, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransLv, 0, ANIMATION_SPEED));

            //To FlipView 
            _sbFlipView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            //_sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(lvDrawing, 0, 0, ANIMATION_SPEED));
            //_sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.SLayoutRoot, 0, 0, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.FlipView, 1, 0, 0));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, 0, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransLv, -400, ANIMATION_SPEED));

            _sbUnAssignSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbUnAssignSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransUnAssignSort, 0, ANIMATION_SPEED));


            _sbUnAssignSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbUnAssignSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransUnAssignSort, 1, ANIMATION_SPEED));

            _sbAssignSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbAssignSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransAssignSort, 0, ANIMATION_SPEED));

            _sbAssignSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbAssignSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransAssignSort, 1, ANIMATION_SPEED));

        }

        private async void LoadSchedule()
        {
            await _commonsource.GetProjectScheduleAllByProjectIDdisciplineCode(_projectid, _disciplineCode);
        }
        
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildIWP.ComponentGrouping));
        }

        private void lvUnAssignSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int drawingId = Convert.ToInt32((lvDrawing.SelectedItem as DataItem).UniqueId);
            switch ((lvUnAssignSort.SelectedItem as DataLibrary.ComboBoxDTO).DataName)
            {
                case "Man Hours":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId, Lib.IWPDataSource.selectedIWP).OrderBy(x => x.ManhoursEstimate);
                    break;
                case "Task Type":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId, Lib.IWPDataSource.selectedIWP).OrderBy(x => x.TaskType);
                    break;
                case "Progress Type":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId, Lib.IWPDataSource.selectedIWP).OrderBy(x => x.ComponentTaskType);
                    break;
            }
            _sbUnAssignSortOFF.Begin();
        }

        private void lvAssignSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((lvAssignSort.SelectedItem as DataLibrary.ComboBoxDTO).DataName)
            {
                case "Man Hours":
                    lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent().OrderBy(x => x.ManhoursEstimate);
                    break;
                case "Task Type":
                    lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent().OrderBy(x => x.TaskType);
                    break;
                case "Progress Type":
                    lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent().OrderBy(x => x.ComponentTaskType);
                    break;
            }
            _sbAssignSortOFF.Begin();
        }

        private void btnUnAssignSort_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransUnAssignSort.ScaleY > 0)
                _sbUnAssignSortOFF.Begin();
            else
                _sbUnAssignSortON.Begin();
        }

        private void btnAssignSort_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransAssignSort.ScaleY > 0)
                _sbAssignSortOFF.Begin();
            else
                _sbAssignSortON.Begin();
        }

        private void btnSelectedAssign_Click(object sender, RoutedEventArgs e)
        {
            AssignAdd();
        }

        private void btnAllAssign_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);
            if (lvUnAssignedComponent.Items.Count == lvUnAssignedComponent.SelectedItems.Count)
                lvUnAssignedComponent.SelectedItem = null;
            else
                lvUnAssignedComponent.SelectAll();
            Login.MasterPage.Loading(false, this);
        }
       

        private void btnSelectedUnAssign_Click(object sender, RoutedEventArgs e)
        {
            UnAssignAdd();
        }

        private void btnAllUnAssign_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);
            if (lvAssignedComponent.Items.Count == lvAssignedComponent.SelectedItems.Count)
                lvAssignedComponent.SelectedItem = null;
            else
                lvAssignedComponent.SelectAll();
            Login.MasterPage.Loading(false, this);
        }

        private void lvUnAssignedComponent_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            lvUnAssignedComponent.SelectedItem = e.Items;
        }

        private void lvUnAssignedComponent_Drop(object sender, DragEventArgs e)
        {
            AssignAdd();
        }

        private void lvAssignedComponent_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            lvAssignedComponent.SelectedItem = e.Items;
        }

        private void lvAssignedComponent_Drop(object sender, DragEventArgs e)
        {
            UnAssignAdd();            
        }

        private async void AssignAdd()
        {
            Login.MasterPage.Loading(true, this);
            //set MTO
            DataLibrary.ProgressAssignment update = new DataLibrary.ProgressAssignment();

            List<DataLibrary.MTODTO> assignedPsdto = new List<DataLibrary.MTODTO>();

            List<DataLibrary.FiwpDTO> fiwplist = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByID(Lib.IWPDataSource.selectedIWP);
            if (fiwplist == null)
            {
                Login.MasterPage.Loading(false, this);
                return;
            }

            DataLibrary.FiwpDTO fiwpdto = new DataLibrary.FiwpDTO();

            if (fiwplist.Count > 0)
                fiwpdto = fiwplist[0];

            if (fiwpdto != null)
            {
                fiwpdto.UpdatedBy = Login.UserAccount.PersonnelId;
                fiwpdto.UpdatedDate = DateTime.Now;
                fiwpdto.CreatedDate = fiwpdto.CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : fiwpdto.CreatedDate;
                fiwpdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                fiwpdto.StartDate = StartDate.SelectedDate.Value == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : StartDate.SelectedDate.Value;
                fiwpdto.FinishDate = FinishDate.SelectedDate.Value == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : FinishDate.SelectedDate.Value;
                update.fiwp = fiwpdto;
            }

            foreach (var var in lvUnAssignedComponent.SelectedItems)
            {
                DataLibrary.MTODTO dto = var as DataLibrary.MTODTO;
                dto.FIWPID = Lib.IWPDataSource.selectedIWP;
                dto.UpdatedBy = Login.UserAccount.PersonnelId;
                dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                assignedPsdto.Add(dto);
            }

            update.progress = assignedPsdto;

            //set schedule
            LoadSchedule();
            if (_commonsource.ProjectSchedule == null)
            {
                Login.MasterPage.Loading(false, this);
                return;
            }

            DataLibrary.ProjectscheduleDTO psdto = _commonsource.ProjectSchedule.Where(x => x.ProjectScheduleID == Lib.ScheduleDataSource.selectedSchedule).SingleOrDefault();
            if (psdto != null)
            {
                psdto.CWPID = Lib.CWPDataSource.selectedCWP;
                psdto.UpdatedBy = Login.UserAccount.PersonnelId;
                psdto.UpdatedDate = DateTime.Now;
                psdto.CreatedDate = psdto.CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : psdto.CreatedDate;
                update.schedule = psdto;
            }

            var result = await _commonsource.UpdateFIWPProgressAssignment(update);


            await _commonsource.GetDrawingForAssignIWPOnMode(Lib.CWPDataSource.selectedCWP, Lib.ScheduleDataSource.selectedSchedule, 0, _obj.taskCategoryIdList, _obj.taskTypeLUIDList, _obj.materialIDList, _obj.progressIDList,
                      _obj.searhValue, _projectid, _disciplineCode, -1);

            await _commonsource.GetComponentProgressByFIWP(Lib.IWPDataSource.selectedIWP, Lib.ScheduleDataSource.selectedSchedule, _projectid, _disciplineCode);

            //if (lvUnAssignedComponent.SelectedItems.Count == lvUnAssignedComponent.Items.Count)
            //{

            //}
            int drawingId = Convert.ToInt32((lvDrawing.SelectedItem as DataItem).UniqueId);
            lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId, Lib.IWPDataSource.selectedIWP);
            lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();

            tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
            tbUnAssignCnt.Text = "NoNumber of Components : " + _commonsource.UnAssignedCnt;
            tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
            tbAssignCnt.Text = "Number of Components : " + _commonsource.AssignedCnt;
            ChangeDateResult("",null);

            Login.MasterPage.Loading(false, this);
        }

        private async void UnAssignAdd()
        {
            Login.MasterPage.Loading(true, this);
            
            //set MTO
            DataLibrary.ProgressAssignment update = new DataLibrary.ProgressAssignment();

            List<DataLibrary.FiwpDTO> fiwplist = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByID(Lib.IWPDataSource.selectedIWP);
            if (fiwplist == null)
            {
                Login.MasterPage.Loading(false, this);
                return;
            }

            DataLibrary.FiwpDTO fiwpdto = new DataLibrary.FiwpDTO();

            if (fiwplist.Count > 0)
                fiwpdto = fiwplist[0];

            if (fiwpdto != null)
            {
                fiwpdto.UpdatedBy = Login.UserAccount.PersonnelId;
                fiwpdto.UpdatedDate = DateTime.Now;
                fiwpdto.CreatedDate = fiwpdto.CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : fiwpdto.CreatedDate;
                fiwpdto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                fiwpdto.StartDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;
                fiwpdto.FinishDate = WinAppLibrary.Utilities.Helper.DateTimeMinValue;
                update.fiwp = fiwpdto;
            }

            List<DataLibrary.MTODTO> unassignedPsdto = new List<DataLibrary.MTODTO>();

            foreach (var var in lvAssignedComponent.SelectedItems)
            {
                DataLibrary.MTODTO dto = var as DataLibrary.MTODTO;
                dto.UpdatedBy = Login.UserAccount.PersonnelId;
                dto.FIWPID = 0;
                dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                unassignedPsdto.Add(dto);
            }

            update.progress = unassignedPsdto;

            //set schedule
            LoadSchedule();
            if (_commonsource.ProjectSchedule == null)
            {
                Login.MasterPage.Loading(false, this);
                return;
            }

            DataLibrary.ProjectscheduleDTO psdto = _commonsource.ProjectSchedule.Where(x => x.ProjectScheduleID == Lib.ScheduleDataSource.selectedSchedule).SingleOrDefault();
            if (psdto != null)
            {
                psdto.CWPID = Lib.CWPDataSource.selectedCWP;
                psdto.UpdatedBy = Login.UserAccount.PersonnelId;
                psdto.UpdatedDate = DateTime.Now;
                psdto.CreatedDate = psdto.CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : psdto.CreatedDate;
                update.schedule = psdto;
            }

            var result = await _commonsource.UpdateFIWPProgressAssignment(update);
           
            await _commonsource.GetDrawingForAssignIWPOnMode(Lib.CWPDataSource.selectedCWP, Lib.ScheduleDataSource.selectedSchedule, 0, _obj.taskCategoryIdList, _obj.taskTypeLUIDList, _obj.materialIDList, _obj.progressIDList,
                      _obj.searhValue, _projectid, _disciplineCode, -1);
            await _commonsource.GetComponentProgressByFIWP(Lib.IWPDataSource.selectedIWP, Lib.ScheduleDataSource.selectedSchedule, _projectid, _disciplineCode);


            //unasign 하려는 assigned component의 drawingid가 drawinglist에 없으면 drawing rebind
            bool state = false;
            foreach (DataItem dto in lvDrawing.Items)
            {
                if (lvAssignedComponent.Items.Count > 0)
                {
                    if (dto.UniqueId == (lvAssignedComponent.Items[0] as DataLibrary.MTODTO).DrawingID.ToString())
                        state = true;
                }
            }

            if (state)
            {
                int drawingId = Convert.ToInt32((lvDrawing.SelectedItem as DataItem).UniqueId);
                lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId, Lib.IWPDataSource.selectedIWP);
                lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();

                tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
                tbUnAssignCnt.Text = "Number of Components : " + _commonsource.UnAssignedCnt;
                tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
                tbAssignCnt.Text = "Number of Components : " + _commonsource.AssignedCnt;
                ChangeDateResult("", null);
            }
            else
            {
                List<DataGroup> source = _commonsource.GetGroupedDrawing();
                this.DefaultViewModel["Drawings"] = source;
            }
            Login.MasterPage.Loading(false, this);
        }

        private async void btnSelectBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> selectType = new List<string>();
                selectType.Add("CWP");
                selectType.Add("SLI");
                selectType.Add("Cancel");
                object objresult = await WinAppLibrary.Utilities.Helper.BuildMessage("Go back to 'Select Construction Work Package(CWP)' or 'Select Schedule Line Item(SLI)'?", "Select", selectType);
                if (objresult != null)
                {
                    switch (Convert.ToInt32(objresult))
                    {
                        case 0:
                            this.Frame.Navigate(typeof(SelectCWP));
                            break;
                        case 1:
                            this.Frame.Navigate(typeof(SelectIWP));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch { }

        }

        void FinishDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeDateResult("end", e);
        }

        void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeDateResult("start",e);
        }

        private async void ChangeDateResult(string type, SelectionChangedEventArgs e)
        {
            try
            {
                if (count > 1)
                {
                    DataLibrary.FiwpDTO dto = new DataLibrary.FiwpDTO();

                    DateTime _startdate = Convert.ToDateTime(StartDate.Text);
                    DateTime _finishdate = Convert.ToDateTime(FinishDate.Text);

                    TimeSpan ts = _finishdate - _startdate;
                    int diffDay = ts.Days;

                    DateTime _schedultStartdate = Convert.ToDateTime(sc.ScheduleJson[0].StartDate);
                    DateTime _schedultFisishtdate = Convert.ToDateTime(sc.ScheduleJson[0].FinishDate);
                    TimeSpan tsschedulestart = _startdate - _schedultStartdate;
                    TimeSpan tsscheduleend = _finishdate - _schedultFisishtdate;



                    if (tsschedulestart.Days < 0)
                    {
                        if (e != null && e.RemovedItems.Count > 0)
                        {
                            count = 1;
                            StartDate.Text = Convert.ToDateTime(e.RemovedItems[0]).ToString("MM/dd/yyyy");
                        }
                        WinAppLibrary.Utilities.Helper.SimpleMessage("IWP start date and end date can not be exceed the scheduled date on top", "Warning!");
                    }
                    else if (tsscheduleend.Days > 0)
                    {
                        if (e != null && e.RemovedItems.Count > 0)
                        {
                            count = 1;
                            FinishDate.Text = Convert.ToDateTime(e.RemovedItems[0]).ToString("MM/dd/yyyy");
                        }
                        WinAppLibrary.Utilities.Helper.SimpleMessage("IWP start date and end date can not be exceed the scheduled date on top", "Warning!");
                    }

                    else if (diffDay < 0)//종료일이 시작일보다 빠르면 경고 문
                    {
                        if (e != null && e.RemovedItems.Count > 0)
                        {
                            count = 1;
                            switch (type.ToLower())
                            {
                                case "start":
                                    StartDate.Text = Convert.ToDateTime(e.RemovedItems[0]).ToString("MM/dd/yyyy");
                                    break;
                                case "end":
                                    FinishDate.Text = Convert.ToDateTime(e.RemovedItems[0]).ToString("MM/dd/yyyy");
                                    break;
                            }
                        }

                        WinAppLibrary.Utilities.Helper.SimpleMessage("End date can not be exceed start date", "Warning!");
                    }
                    else
                    {
                        dto.FiwpID = Lib.IWPDataSource.selectedIWP;
                        dto.StartDate = _startdate;
                        dto.FinishDate = _finishdate;
                        dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                        dto.UpdatedBy = Login.UserAccount.UserName;
                        dto.P6CalendarID = sc.ScheduleJson[0].P6CalendarID;
                        var result = await _iwpsource.UpdateIwpPeriod(dto, tbAssignManhours.Text.Replace("Total Man Hours : ", "").Trim().Length > 0 ? tbAssignManhours.Text.Replace("Total Man Hours : ", "").Trim() : "0");

                        if (result != null)
                        {
                            tbNoCrew.Text = result.CrewMembersAssigned.ToString();

                            Lib.IWPDataSource.selectIwpStart = result.StartDate.ToString("MM/dd/yyyy");
                            Lib.IWPDataSource.selectIWpEnd = result.FinishDate.ToString("MM/dd/yyyy");
                            Lib.IWPDataSource.selectEstimateManHour = result.CrewMembersAssigned.ToString();
                        }
                    }
                }
                else
                    count++;
            }
            catch (Exception ex)
            {
            }
        }

        private void FlipImage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            DrawingManipulation.LoadDrawing(((Windows.UI.Xaml.Media.Imaging.BitmapImage)FlipImage.Source).UriSource.ToString(), "", UriKind.Absolute);
            DrawingManipulation.Show();
        }

        private void FlipImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            FlipImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + Lib.ContentPath.DefaultDrawing));
        }
    }    
}
