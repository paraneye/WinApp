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

namespace Element.Reveal.Meg.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FieldEquipment : WinAppLibrary.Controls.LayoutAwarePage
    {
        private List<RevealCommonSvc.FieldequipmentDTO> _orgFieldequipment;
        private List<RevealCommonSvc.FieldequipmentDTO> _libFieldequipment;
        private List<RevealProjectSvc.FiwpequipDTO> _trgFiwpequip;
        List<RevealProjectSvc.FiwpequipDTO> _delFiwpequip = new List<RevealProjectSvc.FiwpequipDTO>();
        RevealProjectSvc.FiwpequipDTO _currentFiwpequip;
        Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
        List<RevealProjectSvc.FiwpDTO> _iwps = new List<RevealProjectSvc.FiwpDTO>();
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        Windows.UI.Xaml.Media.Animation.Storyboard _sbScheduleON, _sbScheduleOFF;
        private int _projectid, _moduleid, _fiwpid;

        public FieldEquipment()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (Lib.IWPDataSource.isWizard)
            {
                btnNext.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnNext.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Visible;
            }

            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
                _fiwpid = Lib.IWPDataSource.selectedIWP;
            else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                _fiwpid = Lib.IWPDataSource.selectedSIWP;
            else
                _fiwpid = Lib.IWPDataSource.selectedHydro; 

            LoadStoryBoardSwitch();
            LoadLibrary();
        }

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbScheduleOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbScheduleOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranslateSchedule, Window.Current.Bounds.Width, 0.5));

            _sbScheduleON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbScheduleON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleXAnimation(ScheduleScale, 1, 0.5));
            _sbScheduleON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranslateSchedule, 0, 0.5));

        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.FieldEquipment, Lib.CommonDataSource.selPackageTypeLUID, true);

            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            _sbScheduleOFF.Begin();
        }

        private void btnAddIWP_Click(object sender, RoutedEventArgs e)
        {
            if (lvSpec.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                foreach (RevealProjectSvc.ComboBoxDTO dto in lvSpec.SelectedItems)
                {
                    RevealProjectSvc.FiwpequipDTO newdto = new RevealProjectSvc.FiwpequipDTO();

                    newdto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                    newdto.Spec = GetSpec(dto);
                    newdto.SystemType = GetSystemType(dto);
                    newdto.EquipmentID = dto.DataID;
                    newdto.FIWPID = _fiwpid;
                    newdto.CWPID = Lib.CWPDataSource.selectedCWP;
                    newdto.ProjectID = _projectid;
                    newdto.ModuleID = _moduleid;
                    newdto.StartUseDate = _iwps.Count > 0 ? _iwps[0].StartDate : DateTime.Now;
                    newdto.FinishUseDate = _iwps.Count > 0 ? _iwps[0].StartDate : DateTime.Now;
                    newdto.UpdatedDate = DateTime.Now;
                    newdto.EquipmentName = GetEquipmentName(dto);
                    _trgFiwpequip.Add(newdto);
                }

                lvEquipment.ItemsSource = null;
                lvEquipment.ItemsSource = _trgFiwpequip;
                bindToChart(_trgFiwpequip);

                //Bind_Spec();

                InitLibFieldEquipment();

                Login.MasterPage.Loading(false, this);
            }

        }

        private void btnRemoveIWP_Click(object sender, RoutedEventArgs e)
        {
            if (lvEquipment.SelectedItem != null)
            {
                Login.MasterPage.Loading(true, this);

                foreach (RevealProjectSvc.FiwpequipDTO dto in lvEquipment.SelectedItems)
                {
                    if (dto.FiwpEquipID > 0)
                    {
                        dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                        _delFiwpequip.Add(dto);
                    }
                    _trgFiwpequip.Remove(dto);
                }

                lvEquipment.ItemsSource = null;
                lvEquipment.ItemsSource = _trgFiwpequip;

                bindToChart(_trgFiwpequip);

                InitLibFieldEquipment();

                Login.MasterPage.Loading(false, this);
            }
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            List<RevealProjectSvc.FiwpequipDTO> updatelist = new List<RevealProjectSvc.FiwpequipDTO>();

            try
            {
                updatelist.AddRange(_trgFiwpequip.Where(x => x.DTOStatus == (int)WinAppLibrary.Utilities.RowStatus.Update));
                updatelist.AddRange(_trgFiwpequip.Where(x => x.DTOStatus == (int)WinAppLibrary.Utilities.RowStatus.New));
                updatelist.AddRange(_delFiwpequip);

                RevealProjectSvc.DocumentDTO document = new RevealProjectSvc.DocumentDTO();
                document.DocumentTypeLUID = Lib.DocType.ModelView;
                document.CWPID = Lib.CWPDataSource.selectedCWP;
                document.ProjectID = _projectid;
                document.ModuleID = _moduleid;
                document.ProjectScheduleID = Lib.ScheduleDataSource.selectedSchedule;
                document.FIWPID = _fiwpid;
                document.UpdatedBy = Login.UserAccount.UserName;
                document.UpdatedDate = DateTime.Now;

                List<RevealProjectSvc.DocumentDTO> listdocument = new List<RevealProjectSvc.DocumentDTO>();
                listdocument.Add(document);

                List<RevealProjectSvc.FiwpDTO> fiwpdto = new List<RevealProjectSvc.FiwpDTO>();
                fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == _fiwpid).ToList();

                fiwpdto[0].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                             
                if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.FIWP)
                {
                    if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage))
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.FieldEquipment;
                }
                else if (Lib.CommonDataSource.selPackageTypeLUID == Lib.PackageType.SIWP)
                {
                    if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.ITR))
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.FieldEquipment;
                }
                else
                {
                    if (!fiwpdto[0].DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForApp.SafetyDocument))
                        fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.FieldEquipment;
                }


                List<RevealProjectSvc.FiwpequipDTO> result = await(new Lib.ServiceModel.ProjectModel()).SaveFiwpequipForAssembleIWP(updatelist, fiwpdto, listdocument);

                //LoadLibrary();

                //MessageDialog dialog = new MessageDialog("Saved successfully.", "Completed!");
                //dialog.Commands.Add(new UICommand("Close", (command) =>
                //{
                Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.FieldEquipment, Lib.CommonDataSource.selPackageTypeLUID, true);

                    if (Lib.WizardDataSource.NextMenu != null)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
                //}));

                //await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Save to IWP", "There was an error Save to IWP. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void lvCategory1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListView lv = sender as ListView;
            lv.SelectedItem = e.ClickedItem;

            List<RevealCommonSvc.FieldequipmentDTO> result = null;
            List<RevealProjectSvc.ComboBoxDTO> tmp = null;

            if (lvCategory1.SelectedItem != null)
            {
                //category2
                result = _libFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName && !string.IsNullOrEmpty(x.Category2)).ToList();

                tmp = result.GroupBy(x => new { x.Category2 }).Select(x => new RevealProjectSvc.ComboBoxDTO()
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
                    result = _libFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName && !string.IsNullOrEmpty(x.Category3)).ToList();

                    tmp = result.GroupBy(x => new { x.Category3 }).Select(x => new RevealProjectSvc.ComboBoxDTO()
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
                        result = _libFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName).ToList();

                        tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
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

            List<RevealCommonSvc.FieldequipmentDTO> result = null;
            List<RevealProjectSvc.ComboBoxDTO> tmp = null;

            if (lvCategory2.SelectedItem != null)
            {
                result = _libFieldequipment.Where(x => x.Category2 == (lvCategory2.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName && !string.IsNullOrEmpty(x.Category3)).ToList();

                tmp = result.GroupBy(x => new { x.Category3 }).Select(x => new RevealProjectSvc.ComboBoxDTO()
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
                    result = _libFieldequipment.Where(x => x.Category2 == (lvCategory2.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName).ToList();

                    tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
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

            List<RevealCommonSvc.FieldequipmentDTO> result = null;
            List<RevealProjectSvc.ComboBoxDTO> tmp = null;

            if (lvCategory3.SelectedItem != null)
            {
                result = _libFieldequipment.Where(x => x.Category3 == (lvCategory3.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName).ToList();

                tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
                {
                    DataName = x.Key.Spec + (string.IsNullOrEmpty(x.Key.SystemType) ? "" : ", " + x.Key.SystemType),
                    DataID = x.Key.FieldEquipmentID
                }).ToList();
            }


            lvSpec.ItemsSource = tmp;
        }

        private void lvEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (lvEquipment.SelectedItem != null)
            //{
            //    txtSpec.Text = (lvEquipment.SelectedItem as RevealProjectSvc.FiwpequipDTO).Spec;
            //    txtType.Text = (lvEquipment.SelectedItem as RevealProjectSvc.FiwpequipDTO).SystemType;
            //    //txtsdate.Text = (lvEquipment.SelectedItem as RevealProjectSvc.FiwpequipDTO).StartUseDate.ToString("M/d/yyyy");
            //    //txtedate.Text = (lvEquipment.SelectedItem as RevealProjectSvc.FiwpequipDTO).FinishUseDate.ToString("M/d/yyyy");
            //}
            //else
            //{
            //    txtSpec.Text = string.Empty;
            //    txtType.Text = string.Empty;
            //    //txtsdate.Text = string.Empty;
            //    //txtedate.Text = string.Empty;
            //}
            //trgFiwpequip
        }

        #endregion

        #region "Private Method"

        private void InitLibFieldEquipment()
        {
            _libFieldequipment = new List<RevealCommonSvc.FieldequipmentDTO>();

            foreach (RevealCommonSvc.FieldequipmentDTO dto in _orgFieldequipment)
            {
                if (_trgFiwpequip.Where(x => x.EquipmentID == dto.FieldEquipmentID).Count() < 1)
                    _libFieldequipment.Add(dto);
            }

            List<RevealProjectSvc.ComboBoxDTO> lstCategory1 = _libFieldequipment.GroupBy(x => new { x.Category1 }).Select(x => new RevealProjectSvc.ComboBoxDTO()
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


            List<RevealProjectSvc.CwpDTO> source = new List<RevealProjectSvc.CwpDTO>();

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    _orgFieldequipment = await (new Lib.ServiceModel.CommonModel()).GetFieldequipmentByType(string.Empty);

                    _trgFiwpequip = await (new Lib.ServiceModel.ProjectModel()).GetFiwpEquipByFIWP(_fiwpid, _projectid, _moduleid);

                    InitLibFieldEquipment();

                    lvEquipment.ItemsSource = _trgFiwpequip;

                    await _iwp.GetFiwpByIDOnMode(_fiwpid);
                    _iwps = _iwp.GetFiwpByID();

                    bindToChart(_trgFiwpequip);

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "ComponentGrouping LoadGrouping", "There was an error load Grouping. Pleae contact administrator", "Error!");
            }


            Login.MasterPage.Loading(false, this);
        }

        private void Bind_Spec()
        {
            List<RevealCommonSvc.FieldequipmentDTO> result = null;
            List<RevealProjectSvc.ComboBoxDTO> tmp = null;

            if (lvCategory3.SelectedItem != null)
                result = _libFieldequipment.Where(x => x.Category3 == (lvCategory3.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName).ToList();
            else if (lvCategory2.SelectedItem != null)
                result = _libFieldequipment.Where(x => x.Category2 == (lvCategory2.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName).ToList();
            else
                result = _libFieldequipment.Where(x => x.Category1 == (lvCategory1.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName).ToList();

            tmp = result.GroupBy(x => new { x.FieldEquipmentID, x.Spec, x.SystemType }).Select(x => new RevealProjectSvc.ComboBoxDTO()
            {
                DataName = x.Key.Spec + (string.IsNullOrEmpty(x.Key.SystemType) ? "" : ", " + x.Key.SystemType),
                DataID = x.Key.FieldEquipmentID
            }).ToList();

            lvSpec.ItemsSource = tmp;
        }

        private string GetSpec(RevealProjectSvc.ComboBoxDTO dto)
        {
            string rtn = string.Empty;

            rtn = dto.DataName.Split(',')[0];

            return rtn;
        }

        private string GetSystemType(RevealProjectSvc.ComboBoxDTO dto)
        {
            string rtn = string.Empty;

            string tmp = dto.DataName;

            if (tmp.Split(',').Count() > 1)
                rtn = tmp.Split(',')[1];

            return rtn;
        }

        private string GetEquipmentName(RevealProjectSvc.ComboBoxDTO dto)
        {
            string rtn = string.Empty;
            string prefix = string.Empty;

            if (lvCategory3.SelectedItem != null)
            {
                prefix = (lvCategory3.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName;
            }
            else
            {
                if (lvCategory2.SelectedItem != null)
                {
                    prefix = (lvCategory2.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName;
                }
                else
                {
                    if (lvCategory1.SelectedItem != null)
                    {
                        prefix = (lvCategory1.SelectedItem as RevealProjectSvc.ComboBoxDTO).DataName;
                    }
                }
            }

            rtn = prefix + "-" + dto.DataName;

            return rtn;
        }

        private void bindToChart(List<RevealProjectSvc.FiwpequipDTO> list)
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
            foreach (RevealProjectSvc.FiwpequipDTO dto in list)
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

        private void addItemToChart(RevealProjectSvc.FiwpequipDTO dto)
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
                _currentFiwpequip = e.Item.Tag as RevealProjectSvc.FiwpequipDTO;
                //gccSchedule.SelectedItem = e.Item;

                if (_currentFiwpequip != null)
                {
                    foreach (RevealProjectSvc.FiwpequipDTO dto in _trgFiwpequip)
                    {
                        if (dto == _currentFiwpequip)
                        {
                            if (_currentFiwpequip.FiwpEquipID > 0)
                                dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                            else
                                dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;

                            dto.StartUseDate = Convert.ToDateTime(e.Item.Start.DateTime.ToString("MMM dd, yyyy"));
                            dto.FinishUseDate = Convert.ToDateTime(e.Item.Finish.DateTime.ToString("MMM dd, yyyy"));
                            dto.UpdatedDate = DateTime.Now;
                        }
                    }
                }
            }
        }

        #endregion

        private void btnScChart_Click(object sender, RoutedEventArgs e)
        {
            _sbScheduleON.Begin();
        }
    }
}
