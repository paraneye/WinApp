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

namespace Element.Reveal.Manage.Discipline.PunchCard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PunchListDashboard : WinAppLibrary.Controls.LayoutAwarePage
    {
        List<DataItem> Items = new List<DataItem>{ 
            new DataItem { Name = "Item1", Value=30 },
            new DataItem { Name = "Item2", Value=10 },
            new DataItem { Name = "Item3", Value=80 }
        };
        public PunchListDashboard()
        {
            this.InitializeComponent();

            //chart.DataContext = Items;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
    public class DataItem
    {
      public string Name { get; set; }
      public double Value { get; set; }
    }
}
