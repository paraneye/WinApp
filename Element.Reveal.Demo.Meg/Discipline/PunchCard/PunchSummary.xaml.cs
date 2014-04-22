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

namespace Element.Reveal.Meg.Discipline.PunchCard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PunchSummary : WinAppLibrary.Controls.LayoutAwarePage
    {
        string top = @"<html> <head> <!--Load the AJAX API--> <script type='text/javascript' src='https://www.google.com/jsapi'></script> <script type='text/javascript'> google.load('visualization', '1.0', { 'packages': ['corechart'] }); google.setOnLoadCallback(drawChart); function drawChart() { var data = google.visualization.arrayToDataTable([ ";
        string bottom = @" ]); var options = { 'title': '', 'backgroundColor': { fill:'transparent' }, 'width': 480, 'height': 480, is3D: true, legend: { position: 'none' }, isStacked: true,  }; var chart = new google.visualization._CHARTKIND_(document.getElementById('chart_div')); chart.draw(data, options); window.external.notify('a'); } </script> </head> <body style='background-color:#1A242C'> <div style='background-color:#1A242C' id='chart_div'></div> </body> </html>";
        bool blShow1 = false; bool blShow2 = false;
        Lib.ServiceModel.ProjectModel pModel = new Lib.ServiceModel.ProjectModel();

        private int backtype = 0;
        public PunchSummary()
        {
            this.InitializeComponent();
            web.ScriptNotify += web_ScriptNotify;
            web1.ScriptNotify += web1_ScriptNotify;
        }

        void web1_ScriptNotify(object sender, NotifyEventArgs e)
        {
            web1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            blShow1 = true;
            if(blShow2)
                Login.MasterPage.Loading(false, this);
        }

        void web_ScriptNotify(object sender, NotifyEventArgs e)
        {
            web.Visibility = Windows.UI.Xaml.Visibility.Visible;
            blShow2 = true;
            if(blShow1)
                Login.MasterPage.Loading(false, this);
        }

        private string SetDataCol()
        {
            /*
            string r = "";
            r += " ['Name', 'Val1', 'Val2', 'Val3', 'Val4', 'Val5'],";
            List<string> items = new List<string>();
            foreach (DArray d in col)
                items.Add("['" + d.Name + "', " + string.Join(",", d.Values.Select(x => x.ToString()).ToArray()) + "]");
            r += string.Join(",", items);
            return r;
            */
            return "";
        }

        private string SetDataPie(List<RevealProjectSvc.rptPunchDTO> data,  bool isDisc)
        {
            string r = "";
            List<string> items = new List<string>();
            r += "['Name', 'Value'], ";
            foreach (RevealProjectSvc.rptPunchDTO d in data)
            {
                items.Add("['" + ((isDisc) ? d.Discipline : d.CAT) + "', " + d.Cnt + "]");
            }
            r += string.Join(",", items);
            return r;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if(backtype.Equals(1))
                this.Frame.Navigate(typeof(Discipline.TurnOver.TurnoverSystem));
            else
                this.Frame.Navigate(typeof(MainMenu));
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            backtype = navigationParameter != null ? 1 : 0;
            Login.MasterPage.Loading(true, this);
            getData();
        }
        private async void getData()
        {
            List<RevealProjectSvc.rptPunchDTO> dataDisc = await pModel.GetPunchReportByDisc(Login.UserAccount.CurProjectID);
            List<RevealProjectSvc.rptPunchDTO> dataCat = await pModel.GetPunchReportByCat(Login.UserAccount.CurProjectID);

            lvDisc.ItemsSource = dataDisc;
            lvCat.ItemsSource = dataCat;

            web.NavigateToString(top + SetDataPie(dataDisc, false) + bottom.Replace("_CHARTKIND_", "PieChart"));
            web1.NavigateToString(top + SetDataPie(dataCat, true) + bottom.Replace("_CHARTKIND_", "PieChart"));
        }
    }
}
