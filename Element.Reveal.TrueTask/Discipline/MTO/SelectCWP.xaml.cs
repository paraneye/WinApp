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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.MTO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectCWP : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        List<DataLibrary.MaterialcategoryDTO> _materialcategory = new List<DataLibrary.MaterialcategoryDTO>();
        private int _projectid; private string _disciplineCode;

        public SelectCWP()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadCWP();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        #endregion

        #region "Private Method"

        private async void LoadCWP()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    await _cwp.GetCWPsByProjectIDOnMode(_projectid, _disciplineCode);

                    //_iwp = await (new Lib.ServiceModel.CommonModel()).GetFIWPByCwp_Combo(0, _projectid, _disciplineCode);
                    lvCWP.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo(_projectid, _disciplineCode, Login.UserAccount.LoginName); 
                    
                  //  lvCWP.ItemsSource = _cwp.GetCWPs();

                }
                else
                {

                }
            }
            catch (Exception e)
            {
            }

            Login.MasterPage.Loading(false, this);
        }

        #endregion

        private void lvCWP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lvCWP.SelectedItems.Count > 0)
                {
                    Lib.CWPDataSource.selectedCWP = ((DataLibrary.ComboBoxDTO)lvCWP.SelectedItem).DataID;
                    Lib.CWPDataSource.selectedCWPName = ((DataLibrary.ComboBoxDTO)lvCWP.SelectedItem).DataName;
                    this.Frame.Navigate(typeof(SelectTask));
                }
            }
            catch (Exception ex)
            { 
            }
             
            /*
            var cwp = e.AddedItems[0] as RevealProjectSvc.CwpDTO;
            Lib.CWPDataSource.selectedCWP = cwp.CWPID;
            Lib.CWPDataSource.selectedCWPName = cwp.CWPName;*/
            
        }
    }
}
