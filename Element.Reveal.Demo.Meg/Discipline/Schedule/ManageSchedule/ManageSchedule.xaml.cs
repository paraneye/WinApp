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
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Schedule.ManageSchedule
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageSchedule : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbDetailON, _sbDetailOFF;

        List<RevealCommonSvc.ExtSchedulerDTO> _source;
        RevealCommonSvc.ExtSchedulerDTO _currentExtScheduler;
        string _newStartDate, _newEndDate = string.Empty;        

        public ManageSchedule()
        {   
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;    
            _moduleid = Login.UserAccount.CurModuleID;
            _source = navigationParameter as List<RevealCommonSvc.ExtSchedulerDTO>;

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

        private void gccSchedule_ItemPropertyChanged(object sender, GanttChartItemPropertyChangedEventArgs e)
        {
            if (e.Item != null && e.IsDirect && e.IsFinal)
            {   
                _newStartDate = string.Empty;
                _newEndDate = string.Empty;

                _currentExtScheduler = e.Item.Tag as RevealCommonSvc.ExtSchedulerDTO;

                if (e.PropertyName != "IsSelected")
                {
                    _newStartDate = e.Item.Start.DateTime.ToString("MMM dd, yyyy");
                    _newEndDate = e.Item.Finish.DateTime.ToString("MMM dd, yyyy");

                    txtRequestStartDate.Text = e.Item.Start.DateTime.ToString("MMM dd, yyyy");
                    txtRequestEndDate.Text = e.Item.Finish.DateTime.ToString("MMM dd, yyyy");
                }                

                LoadIWPSchedule(_currentExtScheduler, _newStartDate, _newEndDate);
                
                if (gccSchedule.Width > 915)
                {
                    Login.MasterPage.HideUserStatus();

                    if (_currentExtScheduler != null && _currentExtScheduler.Id > 0)
                        gccSchedule.SelectedItem = gccSchedule.Items.Where(x => (x.Tag as RevealCommonSvc.ExtSchedulerDTO).Id == _currentExtScheduler.Id).ToList()[0];
                    
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

        private async void _iwpScheduleDetail_btnUpdateClicked(object sender, object e)
        {
            if (string.IsNullOrEmpty(_newStartDate) || string.IsNullOrEmpty(_newEndDate))
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("There's no schedule change.", "Alert!");
            }
            else
            {
                if (await (WinAppLibrary.Utilities.Helper.YesOrNoMessage("Start Date: " + _newStartDate + "\r\nEnd Date:  " + _newEndDate, "Confirm!")))
                {
                    if (Convert.ToDateTime(_newStartDate) < Convert.ToDateTime(Lib.ScheduleDataSource.selectedScheduleStartDate))
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("The FIWP StartDate can not be eariler than the schedule StartDate.", "Alert!");
                    }
                    else
                    {
                        if (Convert.ToDateTime(_newEndDate) > Convert.ToDateTime(Lib.ScheduleDataSource.selectedScheduleEndDate))
                        {
                            WinAppLibrary.Utilities.Helper.SimpleMessage("The FIWP FinishDate can not be later than the schedule FinishDate.", "Alert!");
                        }
                        else
                        {
                            grChart.Visibility = Visibility.Collapsed;
                            Login.MasterPage.Loading(true, this);

                            var retValue = await (new Lib.ServiceModel.ProjectModel()).UpdateFiwpScheduler(_currentExtScheduler, _newStartDate, _newEndDate);

                            Login.MasterPage.Loading(false, this);
                            grChart.Visibility = Visibility.Visible;

                            if (string.IsNullOrEmpty(retValue.ExceptionMessage))
                            {
                                WinAppLibrary.Utilities.Helper.SimpleMessage("Saved successfully.", "Updated!");
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

            List<RevealProjectSvc.SigmacueDTO> list = new List<RevealProjectSvc.SigmacueDTO>();
            RevealProjectSvc.SigmacueDTO dto = new RevealProjectSvc.SigmacueDTO();

            dto.Comments = "Start Date: " + txtRequestStartDate.Text + "\r\n" + "End Date: " + txtRequestEndDate.Text + "\r\n\r\n" + "Reason for Change: \r\n" + txtRequestReason.Text;
            dto.SentBy = Login.UserAccount.UserName;
            dto.Task = WinAppLibrary.Utilities.SigmaCueTaskType.IWP;            
            dto.DataID = _currentExtScheduler.Id;
            dto.IsActive = 1;
            dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
            
            list.Add(dto);

            var retValue = await (new Lib.ServiceModel.ProjectModel()).SaveSigmacue(list, _currentExtScheduler.Id, WinAppLibrary.Utilities.SigmaCueTaskType.IWP);

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

        private void LoadSchedule(List<RevealCommonSvc.ExtSchedulerDTO> list)
        {
            Login.MasterPage.Loading(true, this);

            bindToChart(list);

            if (_currentExtScheduler != null && _currentExtScheduler.Id > 0)
            {
                gccSchedule.SelectedItem = gccSchedule.Items.Where(x => (x.Tag as RevealCommonSvc.ExtSchedulerDTO).Id == _currentExtScheduler.Id).ToList()[0];
                LoadIWPSchedule(_currentExtScheduler, string.Empty, string.Empty);
            }
            Login.MasterPage.Loading(false, this);
        }

        private void LoadScheduleInfo()
        {
            tbScheduleName.Text = Lib.ScheduleDataSource.selectedScheduleName;
            tbSchedulePeriod.Text = " Start: " + Lib.ScheduleDataSource.selectedScheduleStartDate + " ~ End: " + Lib.ScheduleDataSource.selectedScheduleEndDate;
        }

        public void LoadIWPSchedule(RevealCommonSvc.ExtSchedulerDTO dto, string startDate, string endDate)
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

        private void bindToChart(List<RevealCommonSvc.ExtSchedulerDTO> list)
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
            foreach (RevealCommonSvc.ExtSchedulerDTO dto in list)
            {
                if (barColor == "Red")
                    barColor = "Orange";
                else
                    barColor = "Red";

                GanttChartItem item = new GanttChartItem();

                item.Content = "<div style='text-align: left; width: 170px; font-size: 15px; word-wrap:break-word; margin: 5px;'>" + dto.Name + "</div>";
                item.Start = new DateTimeOffset(dto.StartDate == null ? new DateTime() : Convert.ToDateTime(dto.StartDate.ToString("MMM dd, yyyy")));
                item.Finish = new DateTimeOffset(dto.EndDate == null ? new DateTime() : Convert.ToDateTime(dto.EndDate.ToString("MMM dd, yyyy") + " 11:59:59 PM"));
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
