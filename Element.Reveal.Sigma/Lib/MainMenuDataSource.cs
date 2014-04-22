using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Sigma.Lib
{
    class MainMenuDataSource
    {
        static Type _selectedmenu;
        public static Type CurrentMenu { get { return _selectedmenu; } }

        private GroupModel _datasource = new GroupModel();
        public GroupModel DataSource { get { return _datasource; } }

        public MainMenuDataSource()
        {
            InitiateMenu();
        }

        public void InitiateMenu()
        {
            _datasource.AllGroups.Clear();

            var group1 = new DataGroup("Sigma",
                    "Sigma",
                    "Assets/DarkGray.png");

            group1.Items.Add(new DataItem(Lib.MainMenuList.IWPViewer, "IWP Viewer", "Assets/DarkGray.png", "", group1));
            group1.Items.Add(new DataItem(Lib.MainMenuList.TimeProgress, "Time & Progress", "Assets/DarkGray.png", "", group1));
            group1.Items.Add(new DataItem(Lib.MainMenuList.Report, "Report Viewer", "Assets/DarkGray.png", "", group1));
            group1.Items.Add(new DataItem(Lib.MainMenuList.ManageSchedule, "Manage Schedule", "Assets/DarkGray.png", "", group1));

            _datasource.AllGroups.Add(group1);
        }

        public static void SetCurrentMenu(string name)
        {
            switch (name)
            {
                case MainMenuList.IWPViewer:
                    _selectedmenu = typeof(Discipline.Viewer.IWPGridViewer);
                    break;
                case MainMenuList.TimeProgress:
                    _selectedmenu = typeof(Discipline.Progress.TimeProgress);
                    break;
                case MainMenuList.ManageSchedule:
                    _selectedmenu = typeof(Discipline.Schedule.ManageSchedule);
                    break;
                case MainMenuList.Report:
                    _selectedmenu = typeof(Discipline.Report.ProjectReport);
                    break;
            }
        }
    }
}
