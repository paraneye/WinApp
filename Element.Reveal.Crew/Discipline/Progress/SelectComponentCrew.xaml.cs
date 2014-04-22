using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Element.Reveal.Crew.Discipline.Progress
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectComponentCrew : WinAppLibrary.Controls.LayoutAwarePage, INotifyPropertyChanged
    {
        public static bool RefreshData { get; set; }

        #region "Properties"
        Lib.DataSource.ComponentCrewDataSource _componentcrew = new Lib.DataSource.ComponentCrewDataSource();

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

        int SelectedRuleofCredit
        {
            get
            {
                return Lib.DataSource.CategoryDrawingSource.SelectedRuleOfCredit.DataID;
            }
        }

        int SelectedMaterial
        {
            get
            {
                return Lib.DataSource.CategoryDrawingSource.SelectedMaterial.DataID;
            }
        }

        int SelectedDrawing
        {
            get
            {
                return Convert.ToInt32(Lib.DataSource.CategoryDrawingSource.SelectedDrawing.UniqueId);
            }
        }

        public DateTime CurrentDateTime
        {
            get { return (DateTime)GetValue(CurrentDateTimeProperty); ; }
            set { SetValue(CurrentDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isFocused.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentDateTimeProperty =
            DependencyProperty.Register("CurrentDateTime", typeof(DateTime), typeof(SelectComponentCrew), new PropertyMetadata(DateTime.Today, OnValueChanged));

        System.Collections.ObjectModel.ObservableCollection<DataItem> _drawing = new System.Collections.ObjectModel.ObservableCollection<DataItem>();
        public System.Collections.ObjectModel.ObservableCollection<DataItem> Drawings
        {
            get
            {
                return _drawing;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<RevealProjectSvc.MTODTO> Components
        {
            get
            {
                return Lib.DataSource.ComponentCrewDataSource.Component;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<RevealCommonSvc.ComboBoxDTO> ForemanCrew
        {
            get
            {
                return Lib.DataSource.ComponentCrewDataSource.ForemanCrew;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectComponentCrew sender = d as SelectComponentCrew;
            if (sender.PropertyChanged != null)
            {
                sender.PropertyChanged(sender, new PropertyChangedEventArgs("CurrentDateTime"));
            }
        }
        #endregion

        public SelectComponentCrew()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("Create Crew Progress");
        }

        #region "Event Handler"       
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            try
            {
                //When page is new
                if (RefreshData)
                {
                    RefreshData = false;
                    Login.MasterPage.Loading(false, "Navigation");
                }
                else if (pageState == null)
                {
                    LoadComponentCrew(SelectedDrawing, SelectedCWP, SelectedIWP, "Navigation");
                }
                else
                    Login.MasterPage.Loading(false, "Navigation");

                _drawing.Add(Lib.DataSource.CategoryDrawingSource.SelectedDrawing);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SelectComponentCrew LoadState", "There was an error to load page. Please contact administrator", "Error");
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (sender as Grid).Width = lvComponent.ActualWidth;
        }

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //(sender as Grid).Width = grCrew.ActualWidth / 4;
            (sender as WrapGrid).ItemWidth = grCrew.ActualWidth / 4;
        }

        private void BottomBar_SubmitClick(object sender, object e)
        {
            string tag = e != null ? e.ToString() : string.Empty;
            BottomAppBar.IsOpen = false;

            switch (tag)
            {
                case "Submit":
                    Lib.ParameterDTO selection = new Lib.ParameterDTO();
                    List<RevealProjectSvc.MTODTO> components = new List<RevealProjectSvc.MTODTO>();
                    List<RevealCommonSvc.ComboBoxDTO> crews = new List<RevealCommonSvc.ComboBoxDTO>();

                    foreach (var item in lvComponent.SelectedItems)
                        components.Add(item as RevealProjectSvc.MTODTO);

                    foreach (var item in lvCrew.SelectedItems)
                        crews.Add(item as RevealCommonSvc.ComboBoxDTO);

                    selection.CustomValue1 = components;
                    selection.CustomValue2 = crews;
                    selection.DateValue1 = CurrentDateTime;

                    this.Frame.Navigate(typeof(InputTimeProgress), selection);

                    break;
                case "Unselect":
                    lvComponent.SelectedItems.Clear();
                    lvCrew.SelectedItems.Clear();
                    break;
            }
        }

        private void DatePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(sender, new PropertyChangedEventArgs("CurrentDateTime"));
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                await _componentcrew.LoadCrewAndForemanByFiwpWorkDate_Combo(SelectedCWP, SelectedIWP,
                        Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, CurrentDateTime);
            }
            catch (Exception ee)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage(ee.Message, "Error!");
            }
            Login.MasterPage.Loading(false, this);
        }
        #endregion

        #region "Private Method"
        async void LoadComponentCrew(int drawingId, int cwpId, int iwpId, object token)
        {
            try
            {
                _componentcrew.SetComponent((new Lib.DataSource.CategoryDrawingSource()).GetMTOByDrawingID(drawingId));

                await _componentcrew.LoadCrewAndForemanByFiwpWorkDate_Combo(cwpId, iwpId,
                    Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, CurrentDateTime);

            }
            catch (Exception e)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage(e.Message, "Error!");
            }

            Login.MasterPage.Loading(false, token);
        }
        #endregion

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;

            lvComponent.Height = sv.ActualHeight - 135;
            lvCrew.Height = sv.ActualHeight - 50;
        }
    }
}
