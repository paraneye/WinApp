using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WinAppLibrary.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.Progress
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InputTimeProgress_Old : WinAppLibrary.Controls.LayoutAwarePage, INotifyPropertyChanged
    {
        #region "Properties"
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime currentDateTime;
        public DateTime CurrentDateTime
        {
            get
            {
                return currentDateTime;
            }
            set
            {
                if (value == currentDateTime)
                    return;

                currentDateTime = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("CurrentDateTime"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("CurrentDateTimeString"));

                }
            }
        }

        public String CurrentDateTimeString
        {
            get
            {
                return currentDateTime.ToString("MMM dd, yyyy");
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
        #endregion

        #region "Private Properties"
        System.Collections.ObjectModel.ObservableCollection<RevealProjectSvc.TimesheetDTO> _timesheet = new System.Collections.ObjectModel.ObservableCollection<RevealProjectSvc.TimesheetDTO>();
        System.Collections.ObjectModel.ObservableCollection<RevealProjectSvc.MTODTO> _component = new System.Collections.ObjectModel.ObservableCollection<RevealProjectSvc.MTODTO>();

        //const string Web_Server = "http://dev.elementindustrial.com/Reveal.PreDemo/Discipline";
        const string Web_Server = "http://localhost:3957/Element.Reveal.UI/Discipline";
        const string Web_Sub_Timesheet = "Report/Electrical/RPTTimetable.aspx?param0={0}&param1={1}&param2={2}";
        const string Web_Sub_Submit = "Report/Electrical/RPTCostCodeTimetable.aspx?param1={0}&param2={1}&param3={2}&param4={3}&param5={4}&param6={5}&param7={6}&param8={7}";
        const double Margin = 50; 

        int _departstructure;
        #endregion

        public InputTimeProgress_Old()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("Input Progress");
            currentDateTime = DateTime.Now;
            this.DefaultViewModel["Timesheet"] = _timesheet;
            this.DefaultViewModel["Component"] = _component;
        }

        #region "Event Handler"
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Login.MasterPage.Loading(true, this);

            Lib.ParameterDTO selection = navigationParameter as Lib.ParameterDTO;
            var components = selection.CustomValue1 as List<RevealProjectSvc.MTODTO>;
            var crews = selection.CustomValue2 as List<RevealCommonSvc.ComboBoxDTO>;

            _timesheet.Clear();
            _component.Clear();

            if (crews != null)
            {

                foreach (var item in crews.Select(x => new RevealProjectSvc.TimesheetDTO()
                                            {
                                                DepartStructureID = x.DataID,
                                                PersonnelID = Convert.ToInt32(x.ExtraValue2),
                                                EmployeeFullName = x.DataName
                                            }).ToList())
                {
                    _timesheet.Add(item);
                }
            }

            if (components != null)
            {
                foreach (var item in components)
                    _component.Add(item);
            }

            _departstructure = (new Lib.DataSource.ComponentCrewDataSource()).FindCrewByPersonnelID(Login.UserAccount.PersonnelID).DataID;
           
            Login.MasterPage.Loading(false, this);
        }

        private void grRightPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid rightpanel = sender as Grid;
            
            //lvTotal.Height = lvTimeSheet.Height = rightpanel.ActualHeight / 2 - Margin;
            //dbaniSlideUp.To = -lvTotal.Height - Margin * 2;
            dbaniSlideUp.To = -(LayoutRoot.ActualHeight - 50);
        }

        #region "Center Panel"

        #region "ListView"
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid gr = sender as Grid;
            switch (gr.Tag.ToString())
            {
                case "Component":
                    gr.Width = lvSelectedComponent.ActualWidth;
                    break;
                case "Crew":
                    gr.Width = lvSelectedCrew.ActualWidth;
                    break;
            }
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WrapGrid sp = sender as WrapGrid;
            switch (sp.Tag.ToString())
            {
                case "Component":
                    sp.Width = lvSelectedComponent.ActualWidth;
                    break;
                case "Crew":
                    sp.Width = lvSelectedCrew.ActualWidth;
                    break;
            }
        }

        private void lvComponent_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var items = e.Items.ToObservableCollection();
            e.Data.RequestedOperation = DataPackageOperation.Move;
            e.Data.SetDataProvider("Component", request => request.SetData(items));
        }

        private void lvCrew_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var items = e.Items.ToObservableCollection();
            e.Data.RequestedOperation = DataPackageOperation.Move;
            e.Data.SetDataProvider("Crew", request => request.SetData(items));
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            decimal input = 0;
            bool validate = decimal.TryParse(tb.Text, out input);

            switch (tb.Tag.ToString())
            {
                case "InstalledQtyRatio":
                    var mto = tb.DataContext as RevealProjectSvc.MTODTO;
                    var tborigin = tb.FindName("tbOriginal") as Windows.UI.Xaml.Documents.Run;
                    mto.InstalledQtyRatio = Convert.ToDecimal(tborigin.Text);

                    if (mto.InstalledQtyRatio + input > 100)
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Total installed qty can not be over 100%", "Caution!");
                        tb.Text = "";
                    }
                    else
                        mto.InstalledQtyRatio += input;
                    break;
                case "ActualInstalled":
                    (tb.DataContext as RevealProjectSvc.MTODTO).AmountInstalled = input;
                    break;
                case "StraightTime":
                    (tb.DataContext as RevealProjectSvc.TimesheetDTO).StraightTime = input;
                    break;
                case "DoubleTime":
                    (tb.DataContext as RevealProjectSvc.TimesheetDTO).DoubleTime = input;
                    break;
                case "TimeAndHalf":
                    (tb.DataContext as RevealProjectSvc.TimesheetDTO).TimeAndHalf = input;
                    break;
            }

            if (!validate)
                tb.Text = "";
        }
        #endregion

        private void Detail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            switch (btn.Tag.ToString())
            {
                case "Component":
                    _component.Remove(btn.DataContext as RevealProjectSvc.MTODTO);
                    break;
                case "Crew":
                    _timesheet.Remove(btn.DataContext as RevealProjectSvc.TimesheetDTO);
                    break;
            }
        }

        private void TimeSheetTotal_CancelClick(object sender, object e)
        {
            SlideDown();
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
                case "Save":
                    if (lvSelectedComponent.Items.Count == 0)
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Please add component first.", "Alert");
                    else if (lvSelectedCrew.Items.Count == 0)
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Please add crew first.", "Alert");
                    else
                        SaveTimeSheet();
                    break;
                case "Upload":                    
                    break;
            }
        }
        #endregion

        #endregion

        #region "Private Method"
        private async void SaveTimeSheet()
        {
            Login.MasterPage.Loading(true, this);

            decimal ManhoursEstimate = 0;

            try
            {
                foreach (var item in _component)
                {
                    item.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                    ManhoursEstimate += item.ManhoursEstimate * item.InstalledQtyRatio;
                }

                foreach (var item in _timesheet)
                {
                    item.RuleOFCreditWeightID = SelectedRuleCredit;
                    item.FiwpID = SelectedIWP;
                    item.MaterialCategoryID = SelectedMaterial;
                    item.WorkDate = currentDateTime;
                    item.ProjectID = Login.UserAccount.CurProjectID;
                    item.ModuleID = Login.UserAccount.CurModuleID;
                    item.IsComplete = 1;
                    item.AmountInstalled = 0;
                    item.TimeAllocationID = -1;
                    item.CreatedBy = Login.UserAccount.UserName;
                    item.CreatedDate = currentDateTime;
                    item.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;

                    if (item.DoubleTime + item.TimeAndHalf + item.StraightTime <= 0)
                        throw new NotImplementedException("Please check " + item.EmployeeFullName + "'s working time");
                }


                await (new Lib.ServiceModel.ProjectModel()).SaveTimesheet(_timesheet.ToList(), _component.ToList(), 0, 0);

                //await LoadTimeAllocation();
                //await LoadTotalTimeSheet();

                InitiateList();
                SlideUp();
            }
            catch (NotImplementedException ne)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage(ne.Message, "Caution!");
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "TimeProgress SaveTimeSheet",
                    "There was an error to save time and progress.Please contact Administrator.", "Error!");
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
            bool result = false;

            if (_departstructure > 0)
            {
                //result = await this.TimeSheetTotal.LoadTotalTimeSheet(Login.UserAccount.FirstName + ", " + Login.UserAccount.LastName, _departstructure,
                //                    DateTime.Now, Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);
            }
        }

        private async void LoadTimeAllocation()
        {
            bool result = false;

            if (SelectedCWP > 0 && SelectedIWP > 0 && SelectedMaterial > 0)
            {
                var source = await (new Lib.ServiceModel.ProjectModel()).GetComponentProgressByFIWPDone(SelectedCWP, SelectedMaterial,
                    SelectedIWP, Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, currentDateTime, SelectedRuleCredit);

                //lvTimeSheet.ItemsSource = source;
                result = true;
            }

            //return result;
        }

        private void InitiateList()
        {
            try
            {
                (new Lib.DataSource.ComponentCrewDataSource()).RemoveSource(_component.ToList());
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "InputTimeProgress _InitiateList");
                this.Frame.Navigate(typeof(SelectCategory));
            }

            _component.Clear();
            _timesheet.Clear();
            SelectComponentCrew.RefreshData = true;
        }
        #endregion

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid layoutroot = sender as Grid;
            lvSelectedComponent.Height = lvSelectedCrew.Height = layoutroot.ActualHeight - (layoutroot.RowDefinitions[0].ActualHeight + 
                layoutroot.RowDefinitions[2].ActualHeight + grCrew.RowDefinitions[0].ActualHeight + grCrew.RowDefinitions[1].ActualHeight);
        }
    }
}
