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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Element.Reveal.TrueTask.Discipline.MTO
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class DrawingViewer : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid; private string _disciplineCode;
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Windows.UI.Xaml.Media.Animation.Storyboard _sbDetailON, _sbDetailOFF;

        DataLibrary.MTOPageTotal mtolist = new DataLibrary.MTOPageTotal();
        public DrawingViewer()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadDrawing();
            LoadMtoList();
            LoadIssue();
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

        private async void LoadDrawing()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
            }
        }

        private async void LoadMtoList()
        {
            try 
            {
                mtolist = await(new Lib.ServiceModel.ProjectModel()).GetMaterialTakeOff(Lib.CWPDataSource.selectedCWP, Lib.CommonDataSource.selectedDrawing,
                    Lib.CommonDataSource.selectedMaterialCategory, _projectid, _disciplineCode);

                lvMtoList.ItemsSource = mtolist.mto;
            }
            catch (Exception ex)
            {
 
            }
        }

        private async void LoadIssue()
        {
            try
            {

            }
            catch (Exception ex)
            { 

            }
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ThumbnailViewer));            
        }

        #endregion

        #region button event

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {                       

            }
            catch (Exception ex)
            {
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvMtoList.SelectedItems.Count > 0)
                {
                    DataLibrary.MTOPageTotal deletemto = new DataLibrary.MTOPageTotal();
                    foreach (DataLibrary.MTODTO mto in mtolist.mto)
                    {
                        //todo equals 항목 확인
                        if (mto.ProgressID.Equals(((DataLibrary.MTODTO)lvMtoList.SelectedItem).ProgressID))
                        {
                            mto.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Delete;
                            deletemto.mto = new List<DataLibrary.MTODTO>();
                            deletemto.mto.Add(mto);
                        }
                    }
                    //todo 확인이 필요해서 주석처리
                    // await(new Lib.ServiceModel.ProjectModel().DeleteMto(deletemto.mto, Login.UserAccount.UserName, Login.UserAccount.LoginName, Login.UserAccount.WDPassword, 0));
                }
            }
            catch (Exception ex)
            { 
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddMaterial_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReferenceDrawing_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Login.MasterPage.HideUserStatus();
                grreference.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _sbDetailON.Begin();
            }
            catch (Exception ex)
            {
            }

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.ShowUserStatus();
            imgThumb.UriSource = null;
            grreference.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _sbDetailOFF.Begin();
        }

        private void grReferenceDrawingList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // imageReference.Source = "http://reveal.elementindustrial.com/Element.Reveal.Server.WS/IFC_Images/PL21-P-8391-1%20REV%201B.jpg";
            //todo 아직 웹서비스 만들어 진게 없
            imgThumb.UriSource = new Uri("http://reveal.elementindustrial.com/Element.Reveal.Server.WS/IFC_Images/PL21-P-8391-1%20REV%201B.jpg");
        }

        private void btnMarkUP_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DrawingViewerMarkup));
        }

        #endregion

       
        private void lvMtoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lvMtoList.SelectedItems.Count > 0)
                {
                    btnDelete.IsEnabled = true;
                    btnEdit.IsEnabled = true;
                }
                else
                {
                    btnDelete.IsEnabled = false; 
                    btnEdit.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        
    }
}
