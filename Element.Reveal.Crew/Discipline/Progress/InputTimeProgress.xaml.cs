using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Element.Reveal.Crew.Lib.DataSource;
using WinAppLibrary.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.Progress
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InputTimeProgress : WinAppLibrary.Controls.LayoutAwarePage, INotifyPropertyChanged
    {
        #region "Properties"
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime selectedDateTime;

        public String SelectedDateTimeString
        {
            get
            {
                return selectedDateTime.ToString("MMM dd, yyyy");
            }

        }

        int SelectedCWP
        {
            get
            {
                return Lib.DataSource.CategoryDrawingSource.SelectedCWPID();
            }
        }

        int SelectedIWP
        {
            get
            {
                return Lib.DataSource.CategoryDrawingSource.SelectedIWP.DataID;
            }
        }

        int SelectedMaterial
        {
            get
            {
                return Lib.DataSource.CategoryDrawingSource.SelectedMaterial.DataID;
            }
        }

        int SelectedRuleCredit
        {
            get
            {
                return Lib.DataSource.CategoryDrawingSource.SelectedRuleOfCredit.DataID;
            }
        }

        int SelectedDrawing
        {
            get
            {
                return Convert.ToInt32(Lib.DataSource.CategoryDrawingSource.SelectedDrawing.UniqueId);
            }
        }

        Popup _pop;
        #endregion

        #region "Private Properties"
        

        //const string Web_Server = "http://dev.elementindustrial.com/Reveal.PreDemo/Discipline";
        //const string Web_Server = "http://localhost:3957/Element.Reveal.UI/Discipline";
        //const string Web_Sub_Timesheet = "Report/Electrical/RPTTimetable.aspx?param0={0}&param1={1}&param2={2}";
        //const string Web_Sub_Submit = "Report/Electrical/RPTCostCodeTimetable.aspx?param1={0}&param2={1}&param3={2}&param4={3}&param5={4}&param6={5}&param7={6}&param8={7}";
        const double Margin = 50;
        static Lib.UI.InputTemplate.OperationType OperationType;
        static int SelectedGroupIndex = -1;

        Lib.UI.InputTemplate _selectedgroup;
        int _departstructure;
        
        InputProgressDataSource _datasource = new InputProgressDataSource();
        #endregion

        public InputTimeProgress()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("Input Progress");
            this.DefaultViewModel["TimesheetAndProgress"] = InputProgressDataSource.TimpeSheetProgress;  
        }

        #region "Event Handler"
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.Loading(true, this);

            Login.MasterPage.DoBeforeBack += MasterPage_DoBeforeBack;

            Lib.ParameterDTO selection = navigationParameter as Lib.ParameterDTO;
            var components = selection.CustomValue1 as List<RevealProjectSvc.MTODTO>;
            var crews = selection.CustomValue2 as List<RevealCommonSvc.ComboBoxDTO>;
            selectedDateTime = selection.DateValue1;
            _departstructure = (new Lib.DataSource.ComponentCrewDataSource()).FindCrewByPersonnelID(Login.UserAccount.PersonnelID).DataID;
            
            if (!await LoadTimeSheetAndProgress(_departstructure, selectedDateTime, components, crews))
                WinAppLibrary.Utilities.Helper.SimpleMessage("There was an error to load page", "Error!");

            Login.MasterPage.Loading(false, this);
        }

        private void MasterPage_DoBeforeBack(object sender, object e)
        {
            DoBeforeBack();
        }

        private void grRightPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid rightpanel = sender as Grid;
            
            //lvTotal.Height = lvTimeSheet.Height = rightpanel.ActualHeight / 2 - Margin;
            //dbaniSlideUp.To = -lvTotal.Height - Margin * 2;
            dbaniSlideUp.To = -LayoutRoot.ActualHeight;
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid layoutroot = sender as Grid;
            gvInputTimeProgress.Height = layoutroot.ActualHeight - layoutroot.RowDefinitions[0].ActualHeight;
            gvInputTimeProgress.Width = layoutroot.ActualWidth - layoutroot.ColumnDefinitions[0].ActualWidth;
        }

        #region "Center Panel"
        private void InputTemplate_DeleteItemClicked(object sender, object e)
        {
            OperationType = Lib.UI.InputTemplate.OperationType.DeleteItem;

            try
            {
                switch (sender.ToString())
                {
                    case "Component":
                        _datasource.DeleteProgress(SelectedGroupIndex, e as RevealProjectSvc.MTODTO);
                        break;
                    case "Crew":
                        _datasource.DeleteTimeSheet(SelectedGroupIndex, e as RevealProjectSvc.TimesheetDTO);
                        break;
                }
            }
            catch (Exception ee)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage(ee.Message, "Caution!");
            }
        }

        private async void InputTemplate_OperationClicked(object sender, object e)
        {
            var type = (Lib.UI.InputTemplate.OperationType)e;
            var selectedgroup = sender as Lib.UI.InputTemplate;

            switch (type)
            {
                case Lib.UI.InputTemplate.OperationType.Edit:
                    EditTimeSheetAndProgress(selectedgroup, type);
                    break;
                case Lib.UI.InputTemplate.OperationType.Add:
                    OpenAddingTimeSheetAndProgress(type);
                    break;
                case Lib.UI.InputTemplate.OperationType.DeleteAll:
                    if (SelectedGroupIndex > -1)
                    {
                        if (await WinAppLibrary.Utilities.Helper.YesOrNoMessage("You can't recover once removing all data. Would you like to proceed?", "Caution!") == true)
                            DeleteTimeSheetAndProgress(SelectedGroupIndex);
                    }
                    break;
                case Lib.UI.InputTemplate.OperationType.Save:
                    if (SelectedGroupIndex > -1)
                    {
                        OperationType = type;
                        SaveTimeSheetAndProgress(SelectedGroupIndex);
                    }
                    break;
                case Lib.UI.InputTemplate.OperationType.Cancel:
                    if (selectedgroup != null)
                    {
                        if ((selectedgroup.DataContext as TimesheetAndProgress).DataID > 0)
                        {
                            _datasource.RollBack(SelectedGroupIndex);
                            selectedgroup.EnableInputTemplate(false);
                            SelectedGroupIndex = 0;
                            _selectedgroup = null;
                        }

                        OperationType = Lib.UI.InputTemplate.OperationType.Normal;
                    }
                    break;
            }
        }

        private void ProgressCrewAdd_OperationClick(object sender, object e)
        {
            Lib.UI.ProgressCrewAdd progresscrew = sender as Lib.UI.ProgressCrewAdd;

            if (e.ToString().Equals("Add"))
            {
                var selectedcomp = progresscrew.GetSelectedComponents();
                var selectedcrew = progresscrew.GetSelectedCrews();

                AddTimeSheetAndProgress(SelectedGroupIndex, selectedcomp, selectedcrew);
                OperationType = Lib.UI.InputTemplate.OperationType.Edit;
            }

            progresscrew.ClearItemsSource();
            progresscrew.OperationClick -= ProgressCrewAdd_OperationClick;
            _pop.IsOpen = false;
        }

        private void BottomBar_SubmitClick(object sender, object e)
        {
            string tag = e != null ? e.ToString() : string.Empty;
            BottomBarSetting.IsOpen = false;

            switch (tag)
            {
                case "Submit":
                    SlideUp();
                    break;
                case "Upload":                    
                    break;
            }
        }

        private void TimeSheetTotal_OperationClick(object sender, object e)
        {
            switch (e.ToString())
            {
                case "Submit":
                    SubmitTimeSheet();
                    break;
                case "Cancel":
                    SlideDown();
                    break;
            }
        }
        #endregion

        #endregion

        #region "Private Method"
        private async Task<bool> LoadTimeSheetAndProgress(int departstructureId, DateTime selectedDate, List<RevealProjectSvc.MTODTO> components, List<RevealCommonSvc.ComboBoxDTO> crews)
        {
            bool retValue = false;
            int groupindex = -1;

            if(OperationType == Lib.UI.InputTemplate.OperationType.Normal)
                retValue = await _datasource.LoadTimeSheetAndProgress(selectedDate, departstructureId, 0, Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);

            switch (OperationType)
            {
                case Lib.UI.InputTemplate.OperationType.Add:
                    groupindex = (_selectedgroup.DataContext as TimesheetAndProgress).DataID;
                    break;
                case Lib.UI.InputTemplate.OperationType.Normal:
                    if (retValue)
                        groupindex = 0;
                    break;
            }

            if (groupindex > -1 && crews != null && components != null && crews.Count > 0 && components.Count > 0)
            {
                TimesheetAndProgress group = new TimesheetAndProgress();
                group.TimesheetList = new ObservableCollection<RevealProjectSvc.TimesheetDTO>();
                group.progresseslist = new ObservableCollection<RevealProjectSvc.MTODTO>();

                foreach (var item in crews.Select(x => new RevealProjectSvc.TimesheetDTO()
                {
                    DepartStructureID = x.DataID,
                    PersonnelID = Convert.ToInt32(x.ExtraValue2),
                    EmployeeFullName = x.DataName,
                    DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New
                }).ToList())
                {
                    group.TimesheetList.Add(item);
                }

                foreach (var item in components)
                {
                    item.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                    group.progresseslist.Add(item);
                }

                group.Updated = true;
                _datasource.Insert(groupindex, group);
            }

            SelectedGroupIndex = groupindex;
            return retValue;
        }

        private async void EditTimeSheetAndProgress(Lib.UI.InputTemplate selectedgroup, Lib.UI.InputTemplate.OperationType type)
        {
            if (OperationType == Lib.UI.InputTemplate.OperationType.Normal)
            {
                _selectedgroup = selectedgroup;

                if (_selectedgroup != null)
                {
                    SelectedGroupIndex = (_selectedgroup.DataContext as TimesheetAndProgress).DataID;
                    _selectedgroup.EnableInputTemplate(true);
                    OperationType = type;
                }
                else
                    SelectedGroupIndex = -1;
            }
            else
            {
                bool excute = false;
                if (_selectedgroup == null)
                    excute = true;
                else if (await WinAppLibrary.Utilities.Helper.YesOrNoMessage("If you don't save previous work you will lose all of them. Would you like to proceed?", "Caution!") == true)
                {
                    _datasource.RollBack(SelectedGroupIndex);
                    _selectedgroup.EnableInputTemplate(false);
                    excute = true;
                }

                if (excute)
                {
                    _selectedgroup = selectedgroup;
                    if (_selectedgroup != null)
                    {
                        SelectedGroupIndex = (_selectedgroup.DataContext as TimesheetAndProgress).DataID;
                        _selectedgroup.EnableInputTemplate(true);
                        OperationType = type;
                    }
                    else
                        SelectedGroupIndex = -1;
                }
            }
        }

        private void OpenAddingTimeSheetAndProgress(Lib.UI.InputTemplate.OperationType type)
        {
            if (SelectedGroupIndex > -1)
            {
                OperationType = type;

                var components = _datasource.GetAddingComponents(SelectedGroupIndex, Lib.DataSource.ComponentCrewDataSource.Component.ToList());
                var crews = _datasource.GetAddingCrews(SelectedGroupIndex, Lib.DataSource.ComponentCrewDataSource.ForemanCrew.ToList());

                _pop = WinAppLibrary.Utilities.Helper.GetPopupPane();

                Lib.UI.ProgressCrewAdd progresscrew = new Lib.UI.ProgressCrewAdd();
                progresscrew.OperationClick += ProgressCrewAdd_OperationClick;

                progresscrew.SetLayout(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
                progresscrew.SetItemsSource(components, crews);

                _pop.Child = progresscrew;
                _pop.IsOpen = true;
            }
        }

        private void AddTimeSheetAndProgress(int groupindex, List<RevealProjectSvc.MTODTO> components, List<RevealCommonSvc.ComboBoxDTO> crews)
        {
            if (crews != null && components != null)
            {
                var timesheets = crews.Select(x => new RevealProjectSvc.TimesheetDTO()
                    {
                        DepartStructureID = x.DataID,
                        PersonnelID = Convert.ToInt32(x.ExtraValue2),
                        EmployeeFullName = x.DataName
                    }).ToList();

                _datasource.AddTimeSheetAndProgress(groupindex, components, timesheets);
            }
        }

        private async void DeleteTimeSheetAndProgress(int groupindex)
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                await _datasource.DeleteTimeSheetAndProgress(groupindex);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgress DeleteTimeSheetAndProgress",
                    "There was an error to update time and progress. \n Please contact Administrator.", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private async void SaveTimeSheetAndProgress(int groupindex)
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                await _datasource.SaveTimeSheet(groupindex, SelectedIWP, SelectedRuleCredit, SelectedMaterial,
                    Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, selectedDateTime, Login.UserAccount.UserName);
                
                await InitiateList();
                DoBeforeBack();
                WinAppLibrary.Utilities.Helper.SimpleMessage("Timesheet has been saved to server.", "Success!");
            }
            catch (NotImplementedException ne)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage(ne.Message, "Caution!");
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "InputTimeProgress SaveTimeSheet",
                    "There was an error to save time and progress.Please contact Administrator.", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        async void SubmitTimeSheet()
        {
            try
            {
                Login.MasterPage.Loading(true, this);
                Lib.ServiceModel.ProjectModel project = new Lib.ServiceModel.ProjectModel();
                await project.SaveDailyTimehseet(_departstructure, Login.UserAccount.UserName, selectedDateTime,
                    Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeSheetTotal SubmitTimeSheet",
                    "There was an error on submitting timesheet. Please contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void SlideDown()
        {
            sbSLideUp.Pause();
            sbSlideOff.Begin();
        }

        private void SlideUp()
        {
            LoadTotalTimeSheet();
            sbSlideOff.Pause();
            sbSLideUp.Begin();
        }

        private async void LoadTotalTimeSheet()
        {
            bool retValue = false;

            if (_departstructure > 0)
            {
                Login.MasterPage.Loading(true, this);

                var result = await _datasource.LoadTotalTimeSheet( _departstructure, selectedDateTime, Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);

                this.TimeSheetTotal.BindTimeSheetTotal(result);
                this.TimeSheetTotal.SetOperationType(WinAppLibrary.Utilities.TrackTimeSheetStatus.Submit, selectedDateTime, 
                    "Submitted by " + Login.UserAccount.FirstName + ", " + Login.UserAccount.LastName);

                Login.MasterPage.Loading(false, this);
            }
        }

        private async void LoadTimeAllocation()
        {
            bool result = false;

            if (SelectedCWP > 0 && SelectedIWP > 0 && SelectedMaterial > 0)
            {
                var source = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressByFIWPDone(SelectedCWP, SelectedMaterial,
                    SelectedIWP, Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, selectedDateTime, SelectedRuleCredit);

                //lvTimeSheet.ItemsSource = source;
                result = true;
            }

            //return result;
        }

        private void DoBeforeBack()
        {
            OperationType = Lib.UI.InputTemplate.OperationType.Normal;
            Login.MasterPage.DoBeforeBack -= MasterPage_DoBeforeBack;
            Login.MasterPage.GoBack();
        }

        private async Task<bool> InitiateList()
        {
            bool retValue = false;

            try
            {
                retValue = await _datasource.ReloadTimeSheetProgress(selectedDateTime, _departstructure, 0, Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "InputTimeProgress _InitiateList", 
                    "There was an error to reload timesheet and component. \n Please contact administrator", "Error");
            }

            _selectedgroup = null;
            SelectedGroupIndex = -1;
            OperationType = Lib.UI.InputTemplate.OperationType.Normal;
            SelectComponentCrew.RefreshData = true;

            return retValue;
        }
        #endregion
    }
}
