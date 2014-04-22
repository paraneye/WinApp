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

namespace Element.Reveal.Meg.Discipline.ITR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ITRReportSummary : WinAppLibrary.Controls.LayoutAwarePage
    {
        string top = @"<html> <head> <!--Load the AJAX API--> <script type='text/javascript' src='https://www.google.com/jsapi'></script> <script type='text/javascript'> google.load('visualization', '1.0', { 'packages': ['corechart'] }); google.setOnLoadCallback(drawChart); function drawChart() { var data = google.visualization.arrayToDataTable([ ";
        string bottom = @" ]); var options = { 'title': '', 'backgroundColor': { fill:'transparent' },hAxis: {textStyle: {color: 'white'}}, 'width': 580, 'height': 380, legend: { position: 'none' }, isStacked: true,  }; var chart = new google.visualization._CHARTKIND_(document.getElementById('chart_div')); chart.draw(data, options); window.external.notify('a'); } </script> </head> <body style='background-color:#1A242C'> <div style='background-color:#1A242C' id='chart_div'></div> </body> </html>";
        bool blShow1 = false; bool blShow2 = false;
        Lib.ServiceModel.ProjectModel pModel = new Lib.ServiceModel.ProjectModel();
        private int backtype = 0; 
        public ITRReportSummary()
        {
            this.InitializeComponent();
            web.ScriptNotify += web_ScriptNotify;
            web1.ScriptNotify += web1_ScriptNotify;
        }
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            backtype = navigationParameter != null ? 1 : 0;
            Login.MasterPage.Loading(true, this);
            getData();
        }
        void web1_ScriptNotify(object sender, NotifyEventArgs e)
        {
            web1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            blShow1 = true;
            if (blShow2)
                Login.MasterPage.Loading(false, this);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (backtype.Equals(1))
                this.Frame.Navigate(typeof(Discipline.TurnOver.TurnoverSystem));
            else
            this.Frame.Navigate(typeof(MainMenu));
        }

        void web_ScriptNotify(object sender, NotifyEventArgs e)
        {
            web.Visibility = Windows.UI.Xaml.Visibility.Visible;
            blShow2 = true;
            if (blShow1)
                Login.MasterPage.Loading(false, this);
        }

        private string SetDataCol(List<RevealProjectSvc.rptQAQCformDTO> data, bool isSys)
        {
            string r = "";
            r += " ['Name', 'Val1', 'Val2', 'Val3', 'Val4'],";
            List<string> items = new List<string>();
            foreach (RevealProjectSvc.rptQAQCformDTO d in data)
            {
                string s = "['" + ((isSys) ? d.SystemName : d.CWPName) + "', ";
                s += d.CompletedCnt + ",";
                s += d.RejectedCnt + ",";
                s += d.PartialCnt + ",";
                s += d.AssembledCnt + "]";
                //"['" + d.Name + "', " + string.Join(",", d.Values.Select(x => x.ToString()).ToArray()) + "]"
                items.Add(s);
            }
            r += string.Join(",", items);
            return r;
        }

        private string SetDataPie(List<RevealProjectSvc.rptPunchDTO> data, bool isDisc)
        {
            return "";
        }

        private async void getData()
        {
            List<RevealProjectSvc.rptQAQCformDTO> dataSys = await pModel.GetITRReportBySystem(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);
            List<RevealProjectSvc.rptQAQCformDTO> dataCwp = await pModel.GetITRReportByCWP(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);

            lvDisc.ItemsSource = dataSys;
            lvCat.ItemsSource = dataCwp;

            web.NavigateToString(top + SetDataCol(dataSys, false) + bottom.Replace("_CHARTKIND_", "ColumnChart"));
            web1.NavigateToString(top + SetDataCol(dataCwp, true) + bottom.Replace("_CHARTKIND_", "ColumnChart"));
        }
    }
}
