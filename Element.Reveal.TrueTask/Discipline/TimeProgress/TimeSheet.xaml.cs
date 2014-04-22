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

namespace Element.Reveal.TrueTask.Discipline.TimeProgress
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimeSheet : WinAppLibrary.Controls.LayoutAwarePage
    {
        public TimeSheet()
        {
            this.InitializeComponent();
        }

        #region "Inheritant"
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            //IntergerValue1 = DepartStructureID
            //IntergerValue2 = ProjectID
            //IntergerValue3 = ModuleID
            //StringValue1 = UpdatedBy
            //CustomValue1 = WorkDate
            DataLibrary.SigmacueDTO param = navigationParameter as DataLibrary.SigmacueDTO;

            await this.TimeSheetTotal.LoadTotalTimeSheet(param);
        }
        #endregion

        #region "Event Handler"
        void TimeSheetTotal_Completed(object sender, object e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private void TimeSheetTotal_CancelClick(object sender, object e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }
        #endregion
    }
}
