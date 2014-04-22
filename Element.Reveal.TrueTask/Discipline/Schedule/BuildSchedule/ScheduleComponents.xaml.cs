using System;
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
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.BuildSchedule
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
        List<DataLibrary.ComboBoxDTO> orgDrawingList = new List<DataLibrary.ComboBoxDTO>();
        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbFlipView, _sbUnAssignSortOFF, _sbUnAssignSortON, _sbAssignSortOFF, _sbAssignSortON;
        private int pagecount = 0;
        public ScheduleComponents()
        {
            this.InitializeComponent();
           
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        /// 
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            // TODO: Create an appropriate data model 
            tbScheduleItemName.Text = Lib.ScheduleDataSource.selectedScheduleName;
            LoadP6Time();
            Loaddrawing(navigationParameter);
            LoadStoryBoardSwitch();

            lvUnAssignSort.ItemsSource = _commonsource.GetSortingItems();
            lvAssignSort.ItemsSource = _commonsource.GetSortingItems();
        }

        private void LoadP6Time()
        {
            try
            {
                if (sc.ScheduleJson.Count > 0)
                {
                    P6Date.Text = "P6 Schedule " + sc.ScheduleJson[0].ExternalStartDate.ToString("MM/dd/yyyy") + "~" + sc.ScheduleJson[0].ExternalFinishDate.ToString("MM/dd/yyyy");
                    StartDate.Text = sc.ScheduleJson[0].StartDate.ToString("MM/dd/yyyy");
                    FinishDate.Text = sc.ScheduleJson[0].FinishDate.ToString("MM/dd/yyyy");
                    tbNoCrew.Text = sc.ScheduleJson[0].CrewMembersAssigned.ToString();
                }
            }
            catch
            {
            }
        }
        
        private async void Loaddrawing(Object navigationParameter)
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.HideUserStatus();

            _obj = navigationParameter as Lib.ObjectParam;

            List<DataGroup> source = null;

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    //UnAssignedComponent Load
                    await _commonsource.GetDrawingOnMode(Lib.CWPDataSource.selectedCWP, 0, _obj.taskCategoryIdList, _obj.taskTypeLUIDList, _obj.materialIDList, _obj.progressIDList,
                        _obj.searhValue, _projectid, _disciplineCode, -1);

                    if (_commonsource.Drawing != null && _commonsource.Drawing.Count > 0)
                    {
                        source = _commonsource.GetGroupedDrawing();
                        orgDrawingList = _commonsource.Drawing;
                    }
                    //AssignedComponent Load
                    await _commonsource.GetComponentProgressByFIWP(0, Lib.ScheduleDataSource.selectedSchedule, _projectid, _disciplineCode);
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
            this.gvViewType.SelectedIndex = 0;
            this.SubLayoutRoot.Visibility = Visibility.Visible;
            LoadAssignComponent();
            Login.MasterPage.Loading(false, this);
        }

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

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbGridView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.FlipView, 0, 0, 0));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, -1200, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransLv, 0, ANIMATION_SPEED));

            //To FlipView 
            _sbFlipView = new Windows.UI.Xaml.Media.Animation.Storyboard();
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

        private async void lvDrawing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);
            if (e.AddedItems.Count > 0)
            {
                int drawingId = Convert.ToInt32((e.AddedItems[0] as DataItem).UniqueId);
                Lib.CommonDataSource.selectedDrawing = drawingId;
                lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId);
                tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
                tbUnAssignCnt.Text = "Number of Components : " + _commonsource.UnAssignedCnt;

                FlipImage.Source = ((e.AddedItems[0] as DataItem).Image);                
            }
            else
            {
                lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(0);
                tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
                tbUnAssignCnt.Text = "Number of Components : " + _commonsource.UnAssignedCnt;
                FlipImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(WinAppLibrary.Utilities.Helper.BaseUri + Lib.ContentPath.DefaultDrawing));
            }

            lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();
            tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
            tbAssignCnt.Text = "Number of Components : " + _commonsource.AssignedCnt;
            Login.MasterPage.Loading(false, this);
          
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void gvViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
                DrawingManipulation.LoadDrawing(drawing.ExtraValue1,"", UriKind.Absolute);
                DrawingManipulation.Show();
            }
        }

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildSchedule.ComponentGrouping));
        }

        private void lvUnAssignSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int drawingId = Convert.ToInt32((lvDrawing.SelectedItem as DataItem).UniqueId);
            switch ((lvUnAssignSort.SelectedItem as DataLibrary.ComboBoxDTO).DataName)
            {
                case "Man Hours":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId).OrderBy(x => x.ManhoursEstimate);
                    break;
                case "Task Type":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId).OrderBy(x => x.TaskType);
                    break;
                case "Progress Type":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId).OrderBy(x => x.ComponentTaskType);
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
            UnAssignAdd();            
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
            AssignAdd();
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

            List<DataLibrary.MTODTO> unassignedPsdto = new List<DataLibrary.MTODTO>();

            foreach (var var in lvAssignedComponent.SelectedItems)
            {
                DataLibrary.MTODTO dto = var as DataLibrary.MTODTO;
                dto.UpdatedBy = Login.UserAccount.PersonnelId;
                dto.ProjectScheduleID = 0;
                dto.FIWPID = 0;
                dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                unassignedPsdto.Add(dto);
            }

            update.progress = unassignedPsdto;

            //set schedule
            await _commonsource.GetProjectScheduleAllByProjectIDdisciplineCode(_projectid, _disciplineCode);
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

            var result = await _commonsource.UpdateProjectScheduleAssignment(update);

            //  await _commonsource.GetDrawingOnMode(Lib.CWPDataSource.selectedCWP, 0, _obj.taskCategoryCodeList, _obj.taskCategoryIdList, _obj.systemIdList, _obj.typeLUIdList,
            //                _obj.drawingtypeLUIdList, _obj.costcodeIdList, _obj.searchstringList, _obj.compsearchstringList, _obj.locationList, _obj.rfinumberList, null, _obj.searchvalueList
            //                , _projectid, _disciplineCode, -1);
            await _commonsource.GetDrawingOnMode(Lib.CWPDataSource.selectedCWP, 0, _obj.taskCategoryIdList, _obj.taskTypeLUIDList, _obj.materialIDList, _obj.progressIDList,
                        _obj.searhValue, _projectid, _disciplineCode, -1);
            await _commonsource.GetComponentProgressByFIWP(0, Lib.ScheduleDataSource.selectedSchedule, _projectid, _disciplineCode);

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
                lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId);
                lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();

                tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
                tbUnAssignCnt.Text = "Number of Components : " + _commonsource.UnAssignedCnt;
                tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
                tbAssignCnt.Text = "Number of Components : " + _commonsource.AssignedCnt;
                ChangeDateResult(null);            
            }
            else
            {
                List<DataGroup> source = _commonsource.GetGroupedDrawing();
                this.DefaultViewModel["Drawings"] = source;
            }

            Login.MasterPage.Loading(false, this);
        }

        private async void UnAssignAdd()
        {
            Login.MasterPage.Loading(true, this);
            //set MTO
            DataLibrary.ProgressAssignment update = new DataLibrary.ProgressAssignment();

            List<DataLibrary.MTODTO> assignedPsdto = new List<DataLibrary.MTODTO>();

            foreach (var var in lvUnAssignedComponent.SelectedItems)
            {
                DataLibrary.MTODTO dto = var as DataLibrary.MTODTO;
                dto.UpdatedBy = Login.UserAccount.PersonnelId;
                dto.ProjectScheduleID = Lib.ScheduleDataSource.selectedSchedule;
                dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                assignedPsdto.Add(dto);
            }

            update.progress = assignedPsdto;

            //set schedule
            await _commonsource.GetProjectScheduleAllByProjectIDdisciplineCode(_projectid, _disciplineCode);
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

            var result = await _commonsource.UpdateProjectScheduleAssignment(update);
            await _commonsource.GetDrawingOnMode(Lib.CWPDataSource.selectedCWP, 0, _obj.taskCategoryIdList, _obj.taskTypeLUIDList, _obj.materialIDList, _obj.progressIDList,
                        _obj.searhValue, _projectid, _disciplineCode, -1);
            // await _commonsource.GetDrawingOnMode(Lib.CWPDataSource.selectedCWP, 0, _obj.taskCategoryCodeList, _obj.taskCategoryIdList, _obj.systemIdList, _obj.typeLUIdList,
            //                   _obj.drawingtypeLUIdList, _obj.costcodeIdList, _obj.searchstringList, _obj.compsearchstringList, _obj.locationList, _obj.rfinumberList, null, _obj.searchvalueList
            //                   , _projectid, _disciplineCode, -1);
            await _commonsource.GetComponentProgressByFIWP(0, Lib.ScheduleDataSource.selectedSchedule, _projectid, _disciplineCode);

            //if (lvUnAssignedComponent.SelectedItems.Count == lvUnAssignedComponent.Items.Count)
            //{

            //}
            int drawingId = Convert.ToInt32((lvDrawing.SelectedItem as DataItem).UniqueId);
            lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId);
            lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();

            tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
            tbUnAssignCnt.Text = "Number of Components : " + _commonsource.UnAssignedCnt;
            tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
            tbAssignCnt.Text = "Number of Components : " + _commonsource.AssignedCnt;
            ChangeDateResult(null);            
            //}
            //else
            //{
            //    List<DataGroup> source = _commonsource.GetGroupedDrawing();
            //    this.DefaultViewModel["Drawings"] = source;
            //}

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
                            this.Frame.Navigate(typeof(SelectSchedule));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch { }
        }

        private void FinishDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeDateResult(e);
        }

        private async void ChangeDateResult(SelectionChangedEventArgs e)
        {
            try
            {
                if (pagecount > 0)
                {
                    DataLibrary.ProjectscheduleDTO dto = new DataLibrary.ProjectscheduleDTO();
                    dto = sc.ScheduleJson[0];

                    DateTime _startdate = Convert.ToDateTime(StartDate.Text);
                    DateTime _finishdate = Convert.ToDateTime(FinishDate.Text);

                    TimeSpan ts = _finishdate - _startdate;
                    int diffDay = ts.Days;


                    DateTime _schedultStartdate = Convert.ToDateTime(sc.ScheduleJson[0].ExternalStartDate);
                    DateTime _schedultFisishtdate = Convert.ToDateTime(sc.ScheduleJson[0].ExternalFinishDate);
                    TimeSpan tsschedulestart = _startdate - _schedultStartdate;
                    TimeSpan tsscheduleend = _finishdate - _schedultFisishtdate;

                    if (diffDay < 0)//종료일이 시작일보다 빠르면 경고 문
                    {
                        pagecount = 0;
                        if(e !=null && e.RemovedItems.Count>0)
                            FinishDate.Text = Convert.ToDateTime(e.RemovedItems[0]).ToString("MM/dd/yyyy");
                        WinAppLibrary.Utilities.Helper.SimpleMessage("End date can not be exceed start date", "Warning!");                        
                    }
                    else if (tsschedulestart.Days < 0 || tsscheduleend.Days > 0) //P6 ㅇ
                    {
                        if (e != null && e.RemovedItems.Count > 0)
                        {
                            pagecount = 0;
                            FinishDate.Text = Convert.ToDateTime(e.RemovedItems[0]).ToString("MM/dd/yyyy");
                        }
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Schedule start date and end date can not be exceed the P6 date on top", "Warning!");
                    }                    
                    else
                    {
                        dto.StartDate = _startdate;
                        dto.FinishDate = _finishdate;
                        dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                        dto.UpdatedBy = Login.UserAccount.UserName;
                        dto.P6CalendarID = sc.ScheduleJson[0].P6CalendarID;

                        var result = await _commonsource.UpdateProjectSchedulePeriod(dto, tbAssignManhours.Text.Replace("Total Man Hours : ", "").Trim().Length > 0 ? tbAssignManhours.Text.Replace("Total Man Hours : ", "").Trim() : "0");

                        if (result != null)
                        {
                            sc.SetSchduleInfo(result);
                            tbNoCrew.Text = result.CrewMembersAssigned.ToString();
                        }
                    }
                }
                else
                    pagecount++;

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
