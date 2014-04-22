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
    public sealed partial class SelectType : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        Lib.ObjectParam _objectparam = new Lib.ObjectParam();
        List<DataLibrary.MaterialcategoryDTO> _materialcategory = new List<DataLibrary.MaterialcategoryDTO>();
        private int _projectid; private string _disciplineCode;

        public SelectType()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;

            Lib.CommonDataSource.selectedMaterialCategory = 2;

            LoadLibrary();
            LoadType();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        #endregion

        #region "Private Method"

        private string TaskTypeName(int TaskTypeId)
        {
            string rtn = string.Empty;

            switch (TaskTypeId)
            {
                case Lib.MaterialCategory.Grounding:
                    rtn = Lib.TaskTypeName.Grounding;
                    break;
                case Lib.MaterialCategory.Equipment:
                    rtn = Lib.TaskTypeName.Equipment;
                    break;
                case Lib.MaterialCategory.LightingREC:
                    rtn = Lib.TaskTypeName.LightingREC;
                    break;
                case Lib.MaterialCategory.Raceway:
                    rtn = Lib.TaskTypeName.Raceway;
                    break;
                case Lib.MaterialCategory.Instruments:
                    rtn = Lib.TaskTypeName.Instruments;
                    break;
                case Lib.MaterialCategory.EHT:
                    rtn = Lib.TaskTypeName.EHT;
                    break;
                case Lib.MaterialCategory.Cable:
                    rtn = Lib.TaskTypeName.Cable;
                    break;
            }

            return rtn;
        }

        private async void LoadLibrary()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {   
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

        private async void LoadType()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    lvTaskType.ItemsSource = await (new Lib.ServiceModel.CommonModel()).GetLookupByLookupType_Combo(DataLibrary.Utilities.LOOKUPTYPE.ComponentTaskType, TaskTypeName(Lib.CommonDataSource.selectedMaterialCategory));
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

        private void lvTaskType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void lvLibType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
