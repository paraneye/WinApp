using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.TurnOver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TurnoverSystem : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid; private string _disciplineCode;
        public TurnoverSystem()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadSystem();
        }

        private async void LoadSystem()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    List<DataLibrary.ComboCodeBoxDTO> _dto = new List<DataLibrary.ComboCodeBoxDTO>();
                    //lvSystem.ItemsSource = await(new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo(_projectid, _disciplineCode);
                    _dto = await (new Lib.ServiceModel.CommonModel()).GetCWPByProject_Combo(_projectid, _disciplineCode, Login.UserAccount.LoginName);

                    //todo test항목
                    List<DataLibrary.ComboCodeBoxDTO> _dtotemp = new List<DataLibrary.ComboCodeBoxDTO>();
                    _dtotemp.AddRange(_dto);
                    foreach (DataLibrary.ComboCodeBoxDTO tt in _dto)
                    {
                        _dtotemp.Add(tt);
                    }
                    lvSystem.ItemsSource = _dtotemp;
                    lvList.ItemsSource = _dtotemp;
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


        private void lvSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                /*
                if (lvCWP.SelectedItems.Count > 0)
                {
                    Lib.CWPDataSource.selectedCWP = ((DataLibrary.ComboBoxDTO)lvCWP.SelectedItem).DataID;
                    Lib.CWPDataSource.selectedCWPName = ((DataLibrary.ComboBoxDTO)lvCWP.SelectedItem).DataName;
                    this.Frame.Navigate(typeof(SelectTask));
                }*/
            }
            catch (Exception ex)
            {
            }

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }
    }
}
