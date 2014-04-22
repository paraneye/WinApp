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

namespace Element.Reveal.Sigma.Discipline.Progress
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeProgress : WinAppLibrary.Controls.LayoutAwarePage, INotifyPropertyChanged
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
        #endregion

        #region "Private Properties"
        const double Margin = 50;
        Lib.TimeProgressDataSource _datasource = new Lib.TimeProgressDataSource();
        private int _projectId = 0, _moduleId = 0;

        RevealCommonSvc.ComboBoxDTO SelectedCWP
        {
            get
            {
                if (cbCwp.SelectedItem == null)
                    return new RevealCommonSvc.ComboBoxDTO();
                else
                    return cbCwp.SelectedItem as RevealCommonSvc.ComboBoxDTO;
            }
        }

        RevealCommonSvc.ComboBoxDTO SelectedIWP
        {
            get
            {
                if (cbIwp.SelectedItem == null)
                    return new RevealCommonSvc.ComboBoxDTO();
                else
                    return cbIwp.SelectedItem as RevealCommonSvc.ComboBoxDTO;
            }
        }

        RevealCommonSvc.ComboBoxDTO SelectedMaterial
        {
            get
            {
                if (cbMaterial.SelectedItem == null)
                    return new RevealCommonSvc.ComboBoxDTO();
                else
                    return cbMaterial.SelectedItem as RevealCommonSvc.ComboBoxDTO;
            }
        }

        RevealCommonSvc.ComboBoxDTO SelectedRuleOfCredit
        {
            get
            {
                if (cbRuleCredit.SelectedItem == null)
                    return new RevealCommonSvc.ComboBoxDTO();
                else
                    return cbRuleCredit.SelectedItem as RevealCommonSvc.ComboBoxDTO;
            }
        }
        #endregion

        public TimeProgress()
        {
            this.InitializeComponent();
            this.CurrentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            this.DataContext = this;
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

            Lib.ProjectModuleSource projectmodule = new Lib.ProjectModuleSource();
            _projectId = projectmodule.GetProjectID();
            _moduleId = projectmodule.GetModuleID();
            
            cbCwp.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo_Mobile(_projectId, _moduleId);
            cbMaterial.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetMaterialCategoryByModule_Combo(_moduleId);
            lvComponent.ItemsSource = _datasource.Component;
            lvCrew.ItemsSource = _datasource.ForemanCrew;

            Login.MasterPage.Loading(false, this);
        }

        private void grRightPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid rightpanel = sender as Grid;

            sclvBottom.Width = sclvTop.Width = rightpanel.ActualWidth - Margin * 3;
            lvComponent.Height = lvSelectedComponent.Height = lvCrew.Height = lvSelectedCrew.Height = rightpanel.ActualHeight / 2 - Margin * 3;
            lvTotal.Height = lvTimeSheet.Height = rightpanel.ActualHeight / 2 - Margin;
            dbaniSlideUp.To = -lvTotal.Height - Margin * 2;
        }

        #region "Left Panel"
        private async void cbCwp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);
            cbIwp.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetFIWPByCwp_Combo(SelectedCWP.DataID, _projectId, _moduleId);
            Login.MasterPage.Loading(false, this);
        }

        private async void cbMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);
            cbRuleCredit.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetRuleofCreditByMaterialCategory_Combo(_projectId, _moduleId, SelectedMaterial.DataID);
            Login.MasterPage.Loading(false, this);
        }

        private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            sbDateShow.Pause();
            sbDateShow.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sbDateHide.Begin();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);
            bool result = false;

            InitiateList();
            sbSLideUp.Pause();
            sbSlideOff.Begin();

            result = await LoadComponentProgressGrid();
            result = result ? await LoadTimeAllocation() : false;
            result = result ? await LoadTotalTimeSheet() : false;
            result = result ? await LoadTreeView() : false;
            
            if (result)
            {
                sbShiftOff.Pause();
                sbShiftRight.Begin();
            }
            else
            {
                sbShiftRight.Pause();
                sbShiftOff.Begin();
            }

            Login.MasterPage.Loading(false, this);
        }
        #endregion

        #region "Center Panel"

        #region "ListView"
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

        private void lvSelectedComponent_Drop(object sender, DragEventArgs e)
        {
            DataPackageView view = e.Data.GetView();
            List<object> items = null;
            if (view.Contains("Component") && view.RequestedOperation == DataPackageOperation.Move)
            {
                items = lvComponent.SelectedItems.ToList();
                foreach (var item in items)
                {
                    _datasource.RemoveComponent(item);
                    lvSelectedComponent.Items.Add(item);

                }
            }
            else if (view.Contains("Crew") && view.RequestedOperation == DataPackageOperation.Move)
            {
                items = lvCrew.SelectedItems.ToList();
                foreach (var item in items)
                {
                    var timesheet = _datasource.RemoveCrew(item);
                    lvSelectedCrew.Items.Add(timesheet);
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            List<object> items = lvComponent.SelectedItems.ToList();
            foreach (var item in items)
            {
                _datasource.RemoveComponent(item);
                lvSelectedComponent.Items.Add(item);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            List<object> items = lvCrew.SelectedItems.ToList();
            foreach (var item in items)
            {
                var timesheet = _datasource.RemoveCrew(item);
                lvSelectedCrew.Items.Add(timesheet);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;
            var mto = chk.DataContext as RevealProjectSvc.MTODTO;          
            var txt = chk.FindName("tbInstalledQty") as TextBox;
            var tborigin = chk.FindName("tbOriginal") as Windows.UI.Xaml.Documents.Run;

            mto.IsComplete = 1;
            mto.InstalledQtyRatio = 100;
            txt.Text = Math.Max(100 - Convert.ToDecimal(tborigin.Text), 0).ToString();
            txt.IsEnabled = false;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;
            var mto = chk.DataContext as RevealProjectSvc.MTODTO;
            var txt = chk.FindName("tbInstalledQty") as TextBox;
            var tborigin = chk.FindName("tbOriginal") as Windows.UI.Xaml.Documents.Run;

            mto.IsComplete = 0;
            mto.InstalledQtyRatio = 100 - Convert.ToDecimal(tborigin.Text);
            txt.Text = "0";
            txt.IsEnabled = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            decimal input = 0;

            if (decimal.TryParse(tb.Text, out input))
            {
                switch (tb.Tag.ToString())
                {
                    case "InstalledQtyRatio":
                        var mto = tb.DataContext as RevealProjectSvc.MTODTO;
                        var tborigin = tb.FindName("tbOriginal") as Windows.UI.Xaml.Documents.Run;
                        mto.InstalledQtyRatio = Convert.ToDecimal(tborigin.Text);

                        if(mto.InstalledQtyRatio + input > 100)
                        {
                            WinAppLibrary.Utilities.Helper.SimpleMessage("Total installed qty can not be over 100%", "Caution!");
                            tb.Text = "0";
                        }
                        else
                            mto.InstalledQtyRatio += input;
                        break;
                    case "ActualInstalled":
                        (tb.DataContext as RevealProjectSvc.MTODTO).ActualInstalled = input;
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
            }
            else
                tb.Text = "0";
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
                    _datasource.AddComponent(btn.DataContext);
                    lvSelectedComponent.Items.Remove(btn.DataContext);
                    break;
                case "Crew":
                    _datasource.AddCrew(btn.DataContext);
                    lvSelectedCrew.Items.Remove(btn.DataContext);
                    break;
            }
        }

        private void Slide_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            switch (btn.Tag.ToString())
            {
                case "Up":
                    sbSlideOff.Pause();
                    sbSLideUp.Begin();
                    break;
                case "Down":
                    sbSLideUp.Pause();
                    sbSlideOff.Begin();
                    break;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            switch (btn.Tag.ToString())
            {
                case "On":
                    sbShiftOff.Pause();
                    sbShiftRight.Begin();                   
                    break;
                case "Off":
                    sbShiftRight.Pause();
                    sbShiftOff.Begin();
                    break;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            List<RevealProjectSvc.MTODTO> progresses = new List<RevealProjectSvc.MTODTO>();
            List<RevealProjectSvc.TimesheetDTO> timesheets = new List<RevealProjectSvc.TimesheetDTO>();

            RevealProjectSvc.MTODTO pgdto;
            RevealProjectSvc.TimesheetDTO tdto;

            decimal ManhoursEstimate = 0;

            foreach (var item in lvSelectedComponent.Items)
            {
                pgdto = item as RevealProjectSvc.MTODTO;
                ManhoursEstimate += pgdto.ManhoursEstimate * pgdto.InstalledQtyRatio;
                progresses.Add(pgdto);
            }

            foreach (var item in lvSelectedCrew.Items)
            {
                tdto = item as RevealProjectSvc.TimesheetDTO;
                tdto.RuleOFCreditWeightID = SelectedRuleOfCredit.DataID;
                tdto.FiwpID = SelectedIWP.DataID;
                tdto.MaterialCategoryID = SelectedMaterial.DataID;
                tdto.WorkDate = currentDateTime;
                tdto.ProjectID = _projectId;
                tdto.ModuleID = _moduleId;
                tdto.IsComplete = 1;
                tdto.AmountInstalled = 0;
                tdto.CreatedBy = Login.UserAccount.UserName;
                tdto.CreatedDate = DateTime.Now;
                timesheets.Add(tdto);
            }

            var result = await _datasource.SaveTimeSheet(timesheets, progresses, 0);

            if (result)
            {
                await LoadTimeAllocation();
                await LoadTotalTimeSheet();

                InitiateTimeProgress();
                sbSlideOff.Pause();
                sbSLideUp.Begin();
            }
            else
                WinAppLibrary.Utilities.Helper.SimpleMessage("There was an error to save time and progress.Please contact Administrator.", "Error!");

            Login.MasterPage.Loading(false, this);
        }
        #endregion

        #endregion

        #region "Private Method"
        private async Task<bool> LoadComponentProgressGrid()
        {
            string msg = string.Empty;
            
            if (SelectedCWP.DataID == 0)
                msg = "Please select CWP.";
            else if (SelectedIWP.DataID == 0)
                msg = "Please select IWP";
            else if (SelectedMaterial.DataID == 0)
                msg = "Please select TaskCategory";
            else if (SelectedRuleOfCredit.DataID == 0)
                msg = "Please select Rule of Credit";

            if (!string.IsNullOrEmpty(msg))
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage(msg, "Caution!");
                return false;
            }
            else
            {
                lvSelectedComponent.ItemsSource = null;

                var retValue = await _datasource.GetComponent(SelectedCWP.DataID, SelectedIWP.DataID, SelectedMaterial.DataID, SelectedRuleOfCredit.DataID,
                                            currentDateTime, _projectId, _moduleId);
                return retValue;
            }
        }

        private async Task<bool> LoadTotalTimeSheet()
        {
            bool result = false;

            lvTotal.ItemsSource = await _datasource.GetTimesheetByCrewForMultiPool(SelectedCWP.DataID, SelectedIWP.DataID, SelectedMaterial.DataID,
                                               currentDateTime, _projectId, _moduleId);
            result = true;

            return result;
        }

        private async Task<bool> LoadTimeAllocation()
        {
            bool result = false;

            lvTimeSheet.ItemsSource = await _datasource.GetComponentProgressByFIWPDone(SelectedCWP.DataID, SelectedIWP.DataID, SelectedMaterial.DataID,
                                                SelectedRuleOfCredit.DataID, currentDateTime, _projectId, _moduleId); 
            result = true;

            return result;
        }

        private async Task<bool> LoadTreeView()
        {
            bool result = await _datasource.GetCrewAndForemanByFiwpWorkDate_Combo(SelectedCWP.DataID, SelectedIWP.DataID, _projectId, _moduleId, currentDateTime);
            return result;
        }

        private void InitiateTimeProgress()
        {
            cbRuleCredit.SelectedItem = null;
            cbIwp.SelectedItem = null;
            cbMaterial.SelectedItem = null;

            InitiateList();
        }

        private void InitiateList()
        {
            lvSelectedComponent.Items.Clear();
            lvSelectedCrew.Items.Clear();
            _datasource.InitiateSource();
        }
        #endregion
    }
}
