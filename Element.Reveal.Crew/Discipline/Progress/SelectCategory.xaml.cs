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
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.Progress
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectCategory : WinAppLibrary.Controls.LayoutAwarePage
    {
        #region "Private Properties"
        Lib.DataSource.CategoryDrawingSource _categorydrawing = new Lib.DataSource.CategoryDrawingSource();
        private int _projectId = 0, _moduleId = 0;
        #endregion

        public SelectCategory()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("Select Installation Work Package");
            this.DrawingList.ItemsSource = Lib.DataSource.CategoryDrawingSource.Drawings;
            this.IwpList.ItemsSource = Lib.DataSource.CategoryDrawingSource.IWPs;
            this.MaterialList.ItemsSource = Lib.DataSource.CategoryDrawingSource.Materials;
            this.RuleofCreditList.ItemsSource = Lib.DataSource.CategoryDrawingSource.RuleCredits;
        }

        #region "EventHandler"
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Lib.DataSource.ProjectModuleSource projectmodule = new Lib.DataSource.ProjectModuleSource();
            //_projectId = projectmodule.GetProjectID();
            //_moduleId = projectmodule.GetModuleID();

            _projectId = Login.UserAccount.CurProjectID;
            _moduleId = Login.UserAccount.CurModuleID;
            (new Lib.DataSource.ComponentCrewDataSource()).InitiateSource();

            await LoadCategories(pageState);
            
        }

        private async void CategoryList_Click(object sender, object e)
        {
            switch ((sender as ListView).Tag.ToString())
            {
                case "IWP":
                    _categorydrawing.SelectIWP(e);
                    LoadMaterialCategory();
                    break;
                case "Material":
                    _categorydrawing.SelectMaterial(e);
                    LoadRuleOfCredit();
                    break;
                case "RuleCredit":
                    _categorydrawing.SelectRuleOfCredit(e);
                    await LoadDrawings();
                    break;
            }
        }

        private void DrawingList_Click(object sender, object e)
        {
            try
            {
                switch (sender.ToString())
                {
                    case "Change":
                        _categorydrawing.SelectDrawing(e);
                        break;
                    case "Select":
                        LoadEditor();
                        break;
                }
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "ProgressCategory DrawingList_Click");
            }
        }

        private void DrawingEditor_Closed(object sender, object e)
        {
            Login.MasterPage.ShowTopBanner = true;
            Login.MasterPage.ShowUserStatus = true;
        }

        private async void BottomBar_SubmitClick(object sender, object e)
        {
            string tag = e != null ? e.ToString() : string.Empty;
            BottomAppBar.IsOpen = false;

            switch (tag)
            {
                case "Submit":
                    if (Lib.DataSource.CategoryDrawingSource.SelectedDrawing == null)
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Please select Drawing first", "Caution!");
                    else
                    {
                        Login.MasterPage.Loading(true, "Navigation");
                        SelectComponentCrew.RefreshData = false;

                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                        {
                            this.Frame.Navigate(typeof(SelectComponentCrew));
                        });

                        //Hard Code for Test
                        //param.Add("cwp", 2);
                        //param.Add("iwp", 35);
                        //param.Add("material", 7);
                        //param.Add("rulecredit", 14);
                    }
                    break;
                case "Unselect":
                    _categorydrawing.ClearDrawing();
                    _categorydrawing.ClearRuleOfCredit();
                    RuleofCreditList.Show(false);
                    _categorydrawing.ClearMaterials();
                    MaterialList.Show(false);
                    _categorydrawing.SelectIWP(null);
                    IwpList.Unselect();
                    break;
            }
        }
        #endregion

        #region "Private Method"
        private async Task<bool> LoadCategories(Dictionary<String, Object> pageState)
        {
            bool result = false;

            if (pageState == null)
            {
                Login.MasterPage.Loading(true, this);
                ShowAllList(false);
                _categorydrawing.InitiateSource();
                
                result = await _categorydrawing.LoadCategories(Login.UserAccount.PersonnelID, _projectId, _moduleId);
                //Hard Code for Test
                //result = await _categorydrawing.LoadCategories(3, _projectId, _moduleId);

                _categorydrawing.LoadFiwps();
                Login.MasterPage.Loading(false, this);
                IwpList.Show(true);
            }
            else
            {
                result = true;
                ShowAllList(true);
                IwpList.SetSelection(Lib.DataSource.CategoryDrawingSource.SelectedIWP, false);
                MaterialList.SetSelection(Lib.DataSource.CategoryDrawingSource.SelectedMaterial, false);
                RuleofCreditList.SetSelection(Lib.DataSource.CategoryDrawingSource.SelectedRuleOfCredit, false);
                DrawingList.SetSelection(Lib.DataSource.CategoryDrawingSource.SelectedDrawing, false);
            }

            
            return result;
        }

        private void ShowAllList(bool show)
        {
            IwpList.Show(show);
            MaterialList.Show(show);
            RuleofCreditList.Show(show);
            DrawingList.Show(show);
        }

        private void LoadMaterialCategory()
        {
            _categorydrawing.ClearRuleOfCredit();
            _categorydrawing.ClearDrawing();
            RuleofCreditList.Show(false);
            DrawingList.Show(false);

            _categorydrawing.LoadMaterials();
            MaterialList.Show(true);

            if (_categorydrawing.CountOfMaterials == 1)
                MaterialList.SetSelection(_categorydrawing.GetMaterialByIndex(0), true);
        }

        private void LoadRuleOfCredit()
        {
            _categorydrawing.ClearDrawing();
            DrawingList.Show(false);

            _categorydrawing.LoadRuleOfCredits();
            RuleofCreditList.Show(true);

            if (_categorydrawing.CountOfRuleOfCredit == 1)
                RuleofCreditList.SetSelection(_categorydrawing.GetMaterialByIndex(0), true);
        }

        private async Task<bool> LoadDrawings()
        {
            Login.MasterPage.Loading(true, this);
            //Hard Code for Test
            var result = await _categorydrawing.LoadDrawings(_categorydrawing.SelectedCWPID(Lib.DataSource.CategoryDrawingSource.SelectedIWP.DataID),
                Lib.DataSource.CategoryDrawingSource.SelectedIWP.DataID, Lib.DataSource.CategoryDrawingSource.SelectedMaterial.DataID,
                Lib.DataSource.CategoryDrawingSource.SelectedRuleOfCredit.DataID, DateTime.Now, _projectId, _moduleId);
            //var result = await _categorydrawing.LoadDrawings(2, 35, 7, 14, DateTime.Now, _projectId, _moduleId);

            DrawingList.Show(true);
            Login.MasterPage.Loading(false, this);

            if (_categorydrawing.CountOfDrawing == 1)
            {
                DrawingList.SetSelection(_categorydrawing.GetDrawingByIndex(0), true);
                _categorydrawing.SelectDrawing(DrawingList.SelectedItem);
            }

            return result;
        }

        private void LoadEditor()
        {
            Login.MasterPage.ShowTopBanner = false;
            Login.MasterPage.ShowUserStatus = false;
            DrawingEditor.LoadDrawing(Lib.DataSource.CategoryDrawingSource.SelectedDrawing.Image, Window.Current.Bounds.Width, Window.Current.Bounds.Height);
            DrawingEditor.Show();
        }
        #endregion

        
    }
}
