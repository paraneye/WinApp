using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.TrueTask.Lib
{
    class SubMenuDataSource
    {
        static Type _selectedmenu;
        public static Type CurrentMenu { get { return _selectedmenu; } }

        private GroupModel _datasource = new GroupModel();
        public GroupModel DataSource { get { return _datasource; } }

        public SubMenuDataSource()
        {
            InitiateMenu();
        }

        public void InitiateMenu()
        {
            _datasource.AllGroups.Clear();

            var group1 = new DataGroup("Manage Schedule",
                    "Manage Schedule",
                    "Assets/DarkGray.png");

            group1.Items.Add(new DataItem(Lib.SubMenuList.IWPSchedule, "Installation Work Package Schedule", "../../../Assets/manageshedule_default_packageschedule.png", "", group1));
            group1.Items.Add(new DataItem(Lib.SubMenuList.ManpowerLoading, "Manpower Loading", "../../../Assets/manageshedule_default_manpowerloading.png", "", group1));
          //  group1.Items.Add(new DataItem(Lib.SubMenuList.ProjectSchedule, "Project Schedule", "../../../Assets/manageshedule_default_projectschedule.png", "", group1));

            _datasource.AllGroups.Add(group1);            
        }

        public static void SetCurrentMenu(string name)
        {
            switch (name)
            {
                case SubMenuList.IWPSchedule:
                    _selectedmenu = typeof(Discipline.Schedule.ManageSchedule.ManageSchedule);
                    break;
                case SubMenuList.ManpowerLoading:
                    _selectedmenu = typeof(Discipline.Schedule.ManageSchedule.ManpowerLoading);
                    break;
                case SubMenuList.ProjectSchedule:
                    _selectedmenu = typeof(Discipline.Schedule.ManageSchedule.ProjectSchedule);
                    break;
            }
        }
    }
}
