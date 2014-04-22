using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinAppLibrary.ServiceModels;
using WinAppLibrary.Converters;
using DlhSoft.ProjectData.GanttChart.WinRT.Controls;
using System.Threading.Tasks;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.ManageSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageSchedule : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _id = 0; private string _disciplineCode;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbDetailON, _sbDetailOFF;

        List<DataLibrary.ExtSchedulerDTO> _source;
        DataLibrary.ExtSchedulerDTO _currentExtScheduler, _extScheduler;
        string _newStartDate, _newEndDate = string.Empty;

        bool isUpdate = false;
        int updateCount = 0;

        public ManageSchedule()
        {   
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;    
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            _source = navigationParameter as List<DataLibrary.ExtSchedulerDTO>;

            changeScales(Lib.ChartScale.Month);
            LoadScheduleInfo();
            LoadSchedule(_source);            
            LoadStoryBoardSwitch();
        }

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbDetailOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbDetailOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(detailPanelTrans, Window.Current.Bounds.Width, ANIMATION_SPEED));
            _sbDetailOFF.Begin();

            _sbDetailON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbDetailON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(detailPanelScale, 1, 0));
            _sbDetailON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(detailPanelTrans, 0, ANIMATION_SPEED));
        }

        #region "Event Handler"

        private async void gccSchedule_ItemPropertyChanged(object sender, GanttChartItemPropertyChangedEventArgs e)
        {
            if (e.Item != null && e.IsDirect && e.IsFinal)
            {
                _currentExtScheduler = e.Item.Tag as DataLibrary.ExtSchedulerDTO;

                if (e.PropertyName == "IsSelected")
                {
                    if (_id > 0 && _id != _currentExtScheduler.Id)
                    {
                        if (_newStartDate != e.Item.Start.DateTime.ToString("MM/dd/yyyy") ||
                            _newEndDate != e.Item.Finish.DateTime.ToString("MM/dd/yyyy"))
                        {
                            if (updateCount < 1)
                            {
                                updateCount++;
                                await UpdateSchedule(false);
                            }
                        }
                    }
                }
                else
                {
                    if (isUpdate == false)
                    {
                        _id = _currentExtScheduler.Id;
                        _extScheduler = _currentExtScheduler;

                        _newStartDate = e.Item.Start.DateTime.ToString("MM/dd/yyyy");
                        _newEndDate = e.Item.Finish.DateTime.ToString("MM/dd/yyyy");
                    }

                    txtRequestStartDate.Text = e.Item.Start.DateTime.ToString("MM/dd/yyyy");
                    txtRequestEndDate.Text = e.Item.Finish.DateTime.ToString("MM/dd/yyyy");
                }

                LoadIWPSchedule(_currentExtScheduler, e.Item.Start.DateTime.ToString("MM/dd/yyyy"), e.Item.Finish.DateTime.ToString("MM/dd/yyyy"));
                
                if (gccSchedule.Width > 915)
                {
                    Login.MasterPage.HideUserStatus();

                    if (_currentExtScheduler != null && _currentExtScheduler.Id > 0)
                        gccSchedule.SelectedItem = gccSchedule.Items.Where(x => (x.Tag as DataLibrary.ExtSchedulerDTO).Id == _currentExtScheduler.Id).ToList()[0];
                    
                    gccSchedule.Width = 915;
                    gccSchedule.ChartWidthPercent = 80;
                    gccSchedule.GridWidthPercent = 20;
                }
                
                detailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _sbDetailON.Begin();
            }

            //if (e.IsDirect && e.IsFinal && e.PropertyName != "IsSelected" && e.PropertyName != "IsExpanded")
            //{
            //    MessageDialog dialog = new MessageDialog(string.Format("{1} property has changed for {0}.", e.Item.Content, e.PropertyName));
            //    await dialog.ShowAsync();
            //}
        }

        private async Task UpdateSchedule(bool isPushed)
        {
            isUpdate = true;

            // 처음 선택하는 경우
            if (string.IsNullOrEmpty(_newStartDate) == true || string.IsNullOrEmpty(_newEndDate) == true)
            {
                updateCount = 0;
                isUpdate = false;

                WinAppLibrary.Utilities.Helper.SimpleMessage("Can't save. There is no schedule change.", "Warning");
                return;
            }

            // 변경 schedule이 없는 경우
            if (_extScheduler.StartDate.ToString("MM/dd/yyyy") == _newStartDate && _extScheduler.EndDate.ToString("MM/dd/yyyy") == _newEndDate)
            {
                updateCount = 0;
                isUpdate = false;

                if (isPushed == true)
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Can't save. There is no schedule change.", "Warning");
                return;
            }
            else
            {
                if (await (WinAppLibrary.Utilities.Helper.YesOrNoMessage("If you want to save the change, please select [Yes], Otherwise, please select [No]." +
                    "\r\n\r\nStart Date: " + _newStartDate + "\r\nEnd Date:  " + _newEndDate, "Save Schedule")))
                {
                    if (Convert.ToDateTime(_newStartDate) < Convert.ToDateTime(Lib.ScheduleDataSource.selectedScheduleStartDate))
                    {
                        updateCount = 0;
                        WinAppLibrary.Utilities.Helper.SimpleMessage("IWP start date and end date can not be exceed the scheduled date on top", "Alert!");
                    }
                    else
                    {
                        if (Convert.ToDateTime(_newEndDate) > Convert.ToDateTime(Lib.ScheduleDataSource.selectedScheduleEndDate))
                        {
                            updateCount = 0;
                            WinAppLibrary.Utilities.Helper.SimpleMessage("IWP start date and end date can not be exceed the scheduled date on top", "Alert!");
                        }
                        else
                        {
                            grChart.Visibility = Visibility.Collapsed;
                            Login.MasterPage.Loading(true, this);

                            _extScheduler.UpdatedBy = Login.UserAccount.UserName;

                            // update!
                            var retValue = await (new Lib.ServiceModel.ProjectModel()).UpdateFiwpScheduler(_extScheduler, _newStartDate, _newEndDate);

                            updateCount = 0;

                            Login.MasterPage.Loading(false, this);
                            grChart.Visibility = Visibility.Visible;

                            if (string.IsNullOrEmpty(retValue.ExceptionMessage))
                            {
                                WinAppLibrary.Utilities.Helper.SimpleMessage("Successfully Saved", "Confirm");
                                LoadSchedule(_source);
                            }
                            else
                            {
                                bool confirmRequest = await (WinAppLibrary.Utilities.Helper.YesOrNoMessage("Installation Work Package schedule change is out of schedule scope.\r\nCannnot update your schedule change. Send a schedule request to PM.", "Confirm!"));

                                if (confirmRequest)
                                {
                                    grChart.Visibility = Visibility.Collapsed;
                                    grRequest.Visibility = Visibility.Visible;
                                }
                                else
                                    LoadSchedule(_source);
                            }
                        }
                    }
                }
                else
                {
                    _id = 0;

                    updateCount = 0;
                    LoadSchedule(_source);
                }
            }

            isUpdate = false;
        }
        private async void _iwpScheduleDetail_btnUpdateClicked(object sender, object e)
        {
            if (updateCount < 1)
            {
                updateCount++;
                await UpdateSchedule(true);
            }
        }
                
        private void _iwpScheduleDetail_btnPanelCollapseClicked(object sender, object e)
        {
            Login.MasterPage.ShowUserStatus();

            this.gccSchedule.Width = 1220;
            gccSchedule.ChartWidthPercent = 85;
            gccSchedule.GridWidthPercent = 15;

            detailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbDetailOFF.Begin();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.ManageSchedule.SubMenu));
        }

        private void btnDay_Click(object sender, RoutedEventArgs e)
        {
            changeScales(Lib.ChartScale.Day);
            LoadSchedule(_source);
        }

        private void btnWeek_Click(object sender, RoutedEventArgs e)
        {
            changeScales(Lib.ChartScale.Week);
            LoadSchedule(_source);
        }

        private void btnMonth_Click(object sender, RoutedEventArgs e)
        {
            changeScales(Lib.ChartScale.Month);
            LoadSchedule(_source);
        }

        private void btnYear_Click(object sender, RoutedEventArgs e)
        {
            changeScales(Lib.ChartScale.Year);
            LoadSchedule(_source);
        }
        
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            List<DataLibrary.SigmacueDTO> list = new List<DataLibrary.SigmacueDTO>();
            DataLibrary.SigmacueDTO dto = new DataLibrary.SigmacueDTO();

            dto.Comments = "Start Date: " + txtRequestStartDate.Text + "\r\n" + "End Date: " + txtRequestEndDate.Text + "\r\n\r\n" + "Reason for Change: \r\n" + txtRequestReason.Text;
            dto.SentBy = Login.UserAccount.UserName;
            dto.Task = DataLibrary.Utilities.SigmaCueTaskType.IWP;            
            dto.DataID = _currentExtScheduler.Id;
            dto.IsActive = 1;
            dto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
            
            list.Add(dto);

            var retValue = await (new Lib.ServiceModel.ProjectModel()).SaveSigmacue(list, _currentExtScheduler.Id, DataLibrary.Utilities.SigmaCueTaskType.IWP);

            Login.MasterPage.Loading(false, this);

            grChart.Visibility = Visibility.Visible;
            grRequest.Visibility = Visibility.Collapsed;
            LoadSchedule(_source);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            grChart.Visibility = Visibility.Visible;
            grRequest.Visibility = Visibility.Collapsed;
            LoadSchedule(_source);
        }

        #endregion

        #region "Private Method"

        private void LoadSchedule(List<DataLibrary.ExtSchedulerDTO> list)
        {
            Login.MasterPage.Loading(true, this);

            bindToChart(list);

            if (_currentExtScheduler != null && _currentExtScheduler.Id > 0)
            {
                if (gccSchedule.Items == null)
                {
                    Login.MasterPage.Loading(false, this);
                    return;
                }

                gccSchedule.SelectedItem = gccSchedule.Items.Where(x => (x.Tag as DataLibrary.ExtSchedulerDTO).Id == _currentExtScheduler.Id).ToList()[0];
                LoadIWPSchedule(_currentExtScheduler, string.Empty, string.Empty);
            }
            Login.MasterPage.Loading(false, this);
        }

        private void LoadScheduleInfo()
        {
            tbScheduleName.Text = Lib.ScheduleDataSource.selectedScheduleName;
            tbSchedulePeriod.Text = " Start: " + Lib.ScheduleDataSource.selectedScheduleStartDate + " ~ End: " + Lib.ScheduleDataSource.selectedScheduleEndDate;
        }

        public void LoadIWPSchedule(DataLibrary.ExtSchedulerDTO dto, string startDate, string endDate)
        {
            Login.MasterPage.Loading(true, this);
            
            ucIWPScheduleDetail.btnUpdateClicked += _iwpScheduleDetail_btnUpdateClicked;
            ucIWPScheduleDetail.btnPanelCollapseClicked += _iwpScheduleDetail_btnPanelCollapseClicked;

            if (dto != null)
            {
                ucIWPScheduleDetail.BindIWPScheduleDetail(dto, startDate, endDate);                
            }

            Login.MasterPage.Loading(false, this);
        }

        private void changeScales(string scale)
        {
            SolidColorBrush brBackSel = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#92d050");
            SolidColorBrush brForeSel = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#1a242c");
            SolidColorBrush brBackUn = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#1a242c");
            SolidColorBrush brForeUn = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#ffffff");
            
            //initialize
            btnDay.Background = brBackUn;
            btnDay.Foreground = brForeUn;
            btnMonth.Background = brBackUn;
            btnMonth.Foreground = brForeUn;
            btnYear.Background = brBackUn;
            btnYear.Foreground = brForeUn;
            btnWeek.Background = brBackUn;
            btnWeek.Foreground = brForeUn;

            var upperScale = new DlhSoft.ProjectData.GanttChart.WinRT.Controls.Scale();
            var lowerScale = new DlhSoft.ProjectData.GanttChart.WinRT.Controls.Scale();
            upperScale.HeaderBackgroundColor = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#004a80").Color;
            lowerScale.HeaderBackgroundColor = (new WinAppLibrary.Utilities.Helper()).GetColorFromHexa("#7da7d9").Color;

            switch (scale)
            {
                case Lib.ChartScale.Day:
                    upperScale.ScaleType = ScaleType.Days;
                    upperScale.HeaderTextFormat = ScaleHeaderTextFormat.DayOfWeekAbbreviation;
                    lowerScale.ScaleType = ScaleType.Days;
                    lowerScale.HeaderTextFormat = ScaleHeaderTextFormat.Day;
                    gccSchedule.HourWidth = 2.5;
                    btnDay.Background = brBackSel;
                    btnDay.Foreground = brForeSel;
                    break;                
                case Lib.ChartScale.Month:
                    upperScale.ScaleType = ScaleType.Months;
                    upperScale.HeaderTextFormat = ScaleHeaderTextFormat.MonthAbbreviation;
                    lowerScale.ScaleType = ScaleType.Weeks;
                    lowerScale.HeaderTextFormat = ScaleHeaderTextFormat.Date;
                    gccSchedule.HourWidth = 1.5;
                    btnMonth.Background = brBackSel;
                    btnMonth.Foreground = brForeSel;
                    break;
                case Lib.ChartScale.Year:
                    upperScale.ScaleType = ScaleType.Years;
                    upperScale.HeaderTextFormat = ScaleHeaderTextFormat.Year;
                    lowerScale.ScaleType = ScaleType.Months;
                    lowerScale.HeaderTextFormat = ScaleHeaderTextFormat.MonthAbbreviation;
                    gccSchedule.HourWidth = 1;
                    btnYear.Background = brBackSel;
                    btnYear.Foreground = brForeSel;
                    break;
                default:
                    upperScale.ScaleType = ScaleType.Weeks;
                    upperScale.HeaderTextFormat = ScaleHeaderTextFormat.Date;
                    lowerScale.ScaleType = ScaleType.Days;
                    lowerScale.HeaderTextFormat = ScaleHeaderTextFormat.DayOfWeekAbbreviation;
                    gccSchedule.HourWidth = 2;
                    btnWeek.Background = brBackSel;
                    btnWeek.Foreground = brForeSel;
                    break;
            }            

            gccSchedule.Scales.Clear();
            gccSchedule.Scales.Add(upperScale);
            gccSchedule.Scales.Add(lowerScale);
        }

        private void bindToChart(List<DataLibrary.ExtSchedulerDTO> list)
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
            foreach (DataLibrary.ExtSchedulerDTO dto in list)
            {
                if (barColor == "Red")
                    barColor = "Orange";
                else
                    barColor = "Red";

                GanttChartItem item = new GanttChartItem();

                item.Content = "<div style='text-align: left; width: 170px; font-size: 15px; word-wrap:break-word; margin: 5px;'>" + dto.Name + "</div>";
                item.Start = new DateTimeOffset(dto.StartDate == null ? new DateTime() : Convert.ToDateTime(dto.StartDate.ToString("MM/dd/yyyy")));
                item.Finish = new DateTimeOffset(dto.EndDate == null ? new DateTime() : Convert.ToDateTime(dto.EndDate.ToString("MM/dd/yyyy") + " 11:59:59 PM"));
                item.Tag = dto;

                item.IsMilestone = false;
                item.TaskTemplateClientCode = @"
                var control = item.ganttChartView, settings = control.settings, document = control.ownerDocument, svgns = 'http://www.w3.org/2000/svg', xlinkns = 'http://www.w3.org/1999/xlink';

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

                items.Add(item);

                if (dtMin == new DateTime() || dtMin > dto.StartDate)
                    dtMin = dto.StartDate;
                if (dtMax == new DateTime() || dtMax < dto.EndDate)
                    dtMax = dto.EndDate;
            }
            
            gccSchedule.Items = items;
            
            // Set the scrollable timeline to present, and the displayed and current time values to automatically scroll to a specific chart coordinate, and display a vertical bar highlighter at the specified point.
            gccSchedule.DisplayedTime = new DateTimeOffset(dtMin.AddDays(-3));
            gccSchedule.CurrentTime = new DateTimeOffset(dtMin);
            gccSchedule.TimelineStart = new DateTimeOffset(dtMin == new DateTime() ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : dtMin.AddYears(-1));
            gccSchedule.TimelineFinish = new DateTimeOffset(dtMax == new DateTime() ? DateTime.Now.AddYears(1) : dtMax.AddYears(1));
            
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
           
            gccSchedule.Columns[1].Header = "<div style='text-align: center;'>IWP</div>";
            gccSchedule.Columns[1].Width = 200;
            gccSchedule.Columns[2].IsVisible = false;
            gccSchedule.Columns[3].IsVisible = false;
            gccSchedule.Columns[4].IsVisible = false;
            gccSchedule.Columns[5].IsVisible = false;
            gccSchedule.Columns[6].IsVisible = false;
        }

        #endregion
    }
}
