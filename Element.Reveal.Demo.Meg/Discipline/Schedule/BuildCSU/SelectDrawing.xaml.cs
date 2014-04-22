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

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Converters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Schedule.BuildCSU
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectDrawing : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid;
        private Lib.ObjectParam _obj;
        private bool _isExist = false;
        private Windows.UI.Xaml.Media.Animation.DoubleAnimation dbaniRotate = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
        private Dictionary<string, List<int>> _rotationangles = new Dictionary<string, List<int>>();
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        List<RevealProjectSvc.ComboBoxDTO> orgDrawingList = new List<RevealProjectSvc.ComboBoxDTO>();
        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbFlipView, _sbUnAssignSortOFF, _sbUnAssignSortON, _sbAssignSortOFF, _sbAssignSortON;
        public SelectDrawing()
        {
            this.InitializeComponent();
           
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            if (Lib.IWPDataSource.isWizard)
            {
                btnNext.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnSave.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                btnNext.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnSave.Visibility = Windows.UI.Xaml.Visibility.Visible;
            } 

            // TODO: Create an appropriate data model 
            tbScheduleItemName.Text = Lib.IWPDataSource.selectedHydroName;
            Loaddrawing(navigationParameter);
            LoadStoryBoardSwitch();
            GetAllassignedList();
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void gvViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void FlipViewItem_Clicked(object sender, object e)
        {

            var item = e as DataItem;
            if (item != null)
            {
                var drawing = orgDrawingList.Where(x => x.DataID.ToString() == item.UniqueId).FirstOrDefault();
                DrawingManipulation.LoadDrawing(drawing.ExtraValue1, drawing.ExtraValue2, UriKind.Absolute);
                DrawingManipulation.Show();
            }
        }

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }
        private async void GetAllassignedList()
        {
           RevealProjectSvc.FiwpDocument doc = await (new Lib.ServiceModel.ProjectModel()).GetFIWPDocDrawingsByCSU(Lib.IWPDataSource.selectedHydro, _projectid, _moduleid);

           if (doc.drawing.Count() > 0)
           {
               _isExist = true;
               object selected = lvDrawing.Items.Where(x => (x as DataItem).UniqueId == doc.drawing[0].DrawingID.ToString()).FirstOrDefault();
                if (selected != null)
                    lvDrawing.SelectedItem = selected;
           }
        }
        private async void Loaddrawing(Object navigationParameter)
        {
            Login.MasterPage.Loading(true, this);
            Login.MasterPage.HideUserStatus();

            _obj = navigationParameter as Lib.ObjectParam;

            List<DataGroup> source = null;

            try
            {
                if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
                {
                    bool login = WinAppLibrary.Utilities.SPDocument.IsLogin ? true :
                                await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                    if (login)
                    {
                        if (!WinAppLibrary.Utilities.SPDocument.IsLogin)
                            await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                       
                        await _commonsource.GetDrawingForCSUOnMode(Lib.DrawingType.PANDID, _projectid, _moduleid);
                        //await _commonsource.GetDrawingForCSUOnMode(0, _projectid, _moduleid);

                        if (_commonsource.Drawing != null && _commonsource.Drawing.Count > 0)
                        {
                            source = _commonsource.GetGroupedDrawing();
                            orgDrawingList = _commonsource.Drawing;
                        }

                        
                       
                    }
                    else
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Alert!", "We couldn't sign in Sharepoint Server. Please check your authentication.");
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "IWPGridViewer Loaddrawing", "There was an error load drawing. Pleae contact administrator", "Error!");
            }

            this.DefaultViewModel["Drawings"] = source;
            this.gvViewType.SelectedIndex = 0;
            Login.MasterPage.Loading(false, this);
        }

        private void lvDrawing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Lib.CommonDataSource.selectedDrawing = Convert.ToInt32((e.AddedItems[0] as DataItem).UniqueId);

            }
            else
            {
            }

        }

        private void LoadStoryBoardSwitch()
        {
            //ToGridView
            _sbGridView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.FlipView, 0, 0, 0));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, -1200, ANIMATION_SPEED));
            _sbGridView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransLv, 0, ANIMATION_SPEED));

            //To FlipView 
            _sbFlipView = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(this.FlipView, 1, 0, 0));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransFlip, 0, ANIMATION_SPEED));
            _sbFlipView.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateTranslateXAnimation(TranTransLv, -400, ANIMATION_SPEED));

        }

        private async void LoadSchedule()
        {
            await _commonsource.GetProjectScheduleAllByProjectIDModuleID(_projectid, _moduleid);
        }
        
        private void backButton_Click(object sender, RoutedEventArgs e)
        { 
            Lib.WizardDataSource.SetTargetMenuForCSU(WinAppLibrary.Utilities.DocEstablishedForCSU.PnIDDrawing);

            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                List<RevealProjectSvc.FiwpDTO> fiwps = new List<RevealProjectSvc.FiwpDTO>();
                RevealProjectSvc.FiwpDTO fiwp = Lib.IWPDataSource.csulist.Where(x => x.FiwpID == Lib.IWPDataSource.selectedHydro).FirstOrDefault();

                 if (!fiwp.DocEstablishedLUID.Equals(WinAppLibrary.Utilities.DocEstablishedForCSU.AssociatedDoc))
                        fiwp.DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForCSU.PnIDDrawing;

                fiwp.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;

                fiwps.Add(fiwp);

                List<RevealProjectSvc.ComboBoxDTO> drawings = new List<RevealProjectSvc.ComboBoxDTO>();
                RevealProjectSvc.ComboBoxDTO drawing = _commonsource.Drawing.Where(x => x.DataID == Lib.CommonDataSource.selectedDrawing).FirstOrDefault();
                drawing.DTOStatus = _isExist ? (int)WinAppLibrary.Utilities.RowStatus.Update : (int)WinAppLibrary.Utilities.RowStatus.New;
                drawings.Add(drawing);

                var result = await (new Lib.ServiceModel.ProjectModel()).SavePnIDDrawingForBuildCSU(drawings, fiwps);

                Lib.WizardDataSource.SetTargetMenuForCSU(WinAppLibrary.Utilities.DocEstablishedForCSU.PnIDDrawing);

                if (Lib.WizardDataSource.NextMenu != null)
                    this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Build CSU Work Package", "There was an error Save P&ID into CSU Package. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.Loading(false, this);
        }

    }
    
}
