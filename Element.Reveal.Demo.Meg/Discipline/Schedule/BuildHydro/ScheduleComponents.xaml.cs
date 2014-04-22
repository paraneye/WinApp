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

namespace Element.Reveal.Meg.Discipline.Schedule.BuildHydro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScheduleComponents : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid;
        private Lib.ObjectParam _obj;
        private Windows.UI.Xaml.Media.Animation.DoubleAnimation dbaniRotate = new Windows.UI.Xaml.Media.Animation.DoubleAnimation();
        private Dictionary<string, List<int>> _rotationangles = new Dictionary<string, List<int>>();
        const double ANIMATION_SPEED = WinAppLibrary.Utilities.AnimationHelper.ANIMATION_TIMEs;
        Lib.CommonDataSource _commonsource = new Lib.CommonDataSource();
        List<RevealProjectSvc.ComboBoxDTO> orgDrawingList = new List<RevealProjectSvc.ComboBoxDTO>();
        Windows.UI.Xaml.Media.Animation.Storyboard _sbGridView, _sbFlipView, _sbUnAssignSortOFF, _sbUnAssignSortON, _sbAssignSortOFF, _sbAssignSortON;
        public ScheduleComponents()
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
            // TODO: Create an appropriate data model 
            Loaddrawing(navigationParameter);
            LoadStoryBoardSwitch();

            txtScheduleLineItemName.Text = Lib.IWPDataSource.selectedHydroName;

            lvUnAssignSort.ItemsSource = _commonsource.GetSortingItems();
            lvAssignSort.ItemsSource = _commonsource.GetSortingItems();

            Lib.CommonDataSource.startPoint = 0;
            Lib.CommonDataSource.startDrawingID = 0;
            Lib.CommonDataSource.endPoint = 0;
            Lib.CommonDataSource.endDrawingID = 0;
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

                        await _commonsource.GetDrawingForAssignHydroOnMode(Lib.CWPDataSource.selectedCWP, 0, _obj.moduleList, _obj.processsystemList, _obj.lineList, _obj.pidList,
                            new List<int>(), new List<int>(), new List<int>(), new List<RevealProjectSvc.ComboBoxDTO>(), null, new List<string>(), _projectid, _moduleid, -1);

                        if (_commonsource.Drawing != null && _commonsource.Drawing.Count > 0)
                        {
                            source = _commonsource.GetGroupedDrawing();
                            orgDrawingList = _commonsource.Drawing;
                        }

                        await _commonsource.GetComponentProgressBySIWP(Lib.IWPDataSource.selectedHydro, 0, _projectid, _moduleid);
                       
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
            this.SubLayoutRoot.Visibility = Visibility.Visible;
            Login.MasterPage.Loading(false, this);
        }

        private void lvDrawing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                int drawingId = Convert.ToInt32((e.AddedItems[0] as DataItem).UniqueId);
                Lib.CommonDataSource.selectedDrawing = drawingId;
                lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId);
                //tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
                //tbUnAssignCnt.Text = "No. of components : " + _commonsource.UnAssignedCnt;
            }
            else
            {
                lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(0);
                //tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
                //tbUnAssignCnt.Text = "No. of components : " + _commonsource.UnAssignedCnt;
            }

            lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();
            tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
            tbAssignCnt.Text = "No. of components : " + _commonsource.AssignedCnt;
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

            _sbUnAssignSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbUnAssignSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransUnAssignSort, 0, ANIMATION_SPEED));


            _sbUnAssignSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbUnAssignSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransUnAssignSort, 1, ANIMATION_SPEED));

            _sbAssignSortOFF = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbAssignSortOFF.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransAssignSort, 0, ANIMATION_SPEED));

            _sbAssignSortON = new Windows.UI.Xaml.Media.Animation.Storyboard();
            _sbAssignSortON.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateScaleYAnimation(ScaleTransAssignSort, 1, ANIMATION_SPEED));

        }

        private async void LoadSchedule()
        {
            await _commonsource.GetProjectScheduleAllByProjectIDModuleID(_projectid, _moduleid);
        }
        
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Discipline.Schedule.BuildHydro.ComponentGrouping));
        }

        private void lvUnAssignSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int drawingId = Convert.ToInt32((lvDrawing.SelectedItem as DataItem).UniqueId);
            switch ((lvUnAssignSort.SelectedItem as RevealCommonSvc.ComboBoxDTO).DataName)
            {
                case "Reveal Tag Number":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId).OrderBy(x => x.TagNumber);
                    break;
                case "Man Hours":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId).OrderBy(x => x.ManhoursEstimate);
                    break;
                case "Task Type":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId).OrderBy(x => x.TaskType);
                    break;
                case "Progress Type":
                    lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId).OrderBy(x => x.ComponentTaskType);
                    break;
            }
            _sbUnAssignSortOFF.Begin();
        }

        private void lvAssignSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((lvAssignSort.SelectedItem as RevealCommonSvc.ComboBoxDTO).DataName)
            {
                case "Reveal Tag Number":
                    lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent().OrderBy(x => x.TagNumber);
                    break;
                case "Man Hours":
                    lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent().OrderBy(x => x.ManhoursEstimate);
                    break;
                case "Task Type":
                    lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent().OrderBy(x => x.TaskType);
                    break;
                case "Progress Type":
                    lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent().OrderBy(x => x.ComponentTaskType);
                    break;
            }
            _sbAssignSortOFF.Begin();
        }

        private void btnUnAssignSort_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransUnAssignSort.ScaleY > 0)
                _sbUnAssignSortOFF.Begin();
            else
                _sbUnAssignSortON.Begin();
        }

        private void btnAssignSort_Click(object sender, RoutedEventArgs e)
        {
            if (ScaleTransAssignSort.ScaleY > 0)
                _sbAssignSortOFF.Begin();
            else
                _sbAssignSortON.Begin();
        }

        //Confirm
        private async void btnSelectedAssign_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

         
            //set fiwp
            RevealProjectSvc.ProgressAssignment update = new RevealProjectSvc.ProgressAssignment();

            List<RevealProjectSvc.MTODTO> assignedPsdto = new List<RevealProjectSvc.MTODTO>();

            List<RevealProjectSvc.FiwpDTO> fiwplist = await (new Lib.ServiceModel.ProjectModel()).GetFiwpByID(Lib.IWPDataSource.selectedHydro);

            RevealProjectSvc.FiwpDTO fiwpdto = new RevealProjectSvc.FiwpDTO();

            if (fiwplist.Count > 0)
                fiwpdto = fiwplist[0];

            if (fiwpdto != null)
            {
                fiwpdto.UpdatedBy = Login.UserAccount.UserName;
                fiwpdto.UpdatedDate = DateTime.Now;
                fiwpdto.CreatedDate = fiwpdto.CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : fiwpdto.CreatedDate;
                fiwpdto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                update.fiwp = fiwpdto;

            }

            foreach (var var in lvUnAssignedComponent.SelectedItems)
            {
                RevealProjectSvc.MTODTO dto = var as RevealProjectSvc.MTODTO;
                dto.SIWPID = Lib.IWPDataSource.selectedHydro;
                dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                assignedPsdto.Add(dto);
            }

            update.progress = assignedPsdto;

            //set schedule
            LoadSchedule();
            RevealProjectSvc.ProjectscheduleDTO psdto = _commonsource.ProjectSchedule.Where(x => x.ProjectScheduleID == Lib.ScheduleDataSource.selectedSchedule).SingleOrDefault();
            if (psdto != null)
            {
                psdto.CWPID = Lib.CWPDataSource.selectedCWP;
                psdto.UpdatedBy = Login.UserAccount.UserName;
                psdto.UpdatedDate = DateTime.Now;
                psdto.CreatedDate = psdto.CreatedDate == DateTime.MinValue ? WinAppLibrary.Utilities.Helper.DateTimeMinValue : psdto.CreatedDate;
                update.schedule = psdto;
            }

            var result = await _commonsource.UpdateHydroProgressAssignmentByStartPoint(update, Lib.CommonDataSource.selectedDrawing);

            await _commonsource.GetDrawingForAssignHydroOnMode(Lib.CWPDataSource.selectedCWP, 0, _obj.moduleList, _obj.processsystemList, _obj.lineList, _obj.pidList,
                            new List<int>(), new List<int>(), new List<int>(), new List<RevealProjectSvc.ComboBoxDTO>(), null, new List<string>(), _projectid, _moduleid, -1);

            await _commonsource.GetComponentProgressBySIWP(Lib.IWPDataSource.selectedHydro, 0, _projectid, _moduleid);

            int drawingId = Convert.ToInt32((lvDrawing.SelectedItem as DataItem).UniqueId);
            lvUnAssignedComponent.ItemsSource = _commonsource.GetGroupedUnAssignedComponent(drawingId);
            lvAssignedComponent.ItemsSource = _commonsource.GetGroupedAssignedComponent();

            tbUnAssignManhours.Text = "Total Man Hours : " + _commonsource.UnAssignedManhour;
            tbUnAssignCnt.Text = "No. of components : " + _commonsource.UnAssignedCnt;
            tbAssignManhours.Text = "Total Man Hours : " + _commonsource.AssignedManhour;
            tbAssignCnt.Text = "No. of components : " + _commonsource.AssignedCnt;

            InitScope();

            Login.MasterPage.Loading(false, this);
        }

        private void InitScope()
        {
            lvUnAssignedComponent.SelectedItem = null;
        }


        private void lvUnAssignedComponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
    
}
