using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Manage.Lib.DataSource
{
    class MainMenuDataSource
    {
        static Type _selectedmenu;
        public static Type CurrentMenu { get { return _selectedmenu; } }

        private GroupModel _datasource = new GroupModel();
        public GroupModel DataSource { get { return _datasource; } }

        public static bool offDataUpdate { get; set; }

        public MainMenuDataSource()
        {
            InitiateMenu();
        }

        public void InitiateMenu()
        {
            _datasource.AllGroups.Clear();

            //var group = new DataGroup(Lib.MainMenuGroup.CREWTASK,
            //        Lib.MainMenuGroup.CREWTASK,
            //        "Assets/DarkGray.png");

            //group.Items.Add(new DataItem(Lib.MainMenuList.BRASSIN, Lib.MainMenuList.BRASSIN, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_brassin.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.BRASSOUT, Lib.MainMenuList.BRASSOUT, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_brassout.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.TIMEPROGRESS, Lib.MainMenuList.TIMEPROGRESS, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_timeandprogress.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.SAFETY, Lib.MainMenuList.SAFETY, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_safety.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.MANAGETIMESHEET, Lib.MainMenuList.MANAGETIMESHEET, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_managetimeandprogress.png", "", group));
            //_datasource.AllGroups.Add(group);

            //group = new DataGroup(Lib.MainMenuGroup.REPORT,
            //        Lib.MainMenuGroup.REPORT,
            //        "Assets/DarkGray.png");

            //group.Items.Add(new DataItem(Lib.MainMenuList.RFISTATUS, Lib.MainMenuList.RFISTATUS, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_rfistatus.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.IWPREPORT, Lib.MainMenuList.IWPREPORT, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_iwpreport.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.SIWPREPORT, Lib.MainMenuList.SIWPREPORT, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_siwpreport.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.MATERIALREPORT, Lib.MainMenuList.MATERIALREPORT, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_materialreport.png", "", group));
            //_datasource.AllGroups.Add(group);

            //group = new DataGroup(Lib.MainMenuGroup.VISUAL,
            //        Lib.MainMenuGroup.VISUAL,
            //        "Assets/DarkGray.png");

            //group.Items.Add(new DataItem(Lib.MainMenuList.IWPVIEWER, Lib.MainMenuList.IWPVIEWER, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_iwpviewer.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.SIWPVIEWER, Lib.MainMenuList.SIWPVIEWER, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_siwpviewer.png", "", group));
            //group.Items.Add(new DataItem(Lib.MainMenuList.DRAWINGVIEWER, Lib.MainMenuList.DRAWINGVIEWER, WinAppLibrary.Utilities.Helper.BaseUri + "Assets/Menu/crew_drawingviewer.png", "", group));
            //_datasource.AllGroups.Add(group);

            //_datasource.AllGroups.Add(group2);
        }

        public static void SetCurrentMenu(string name)
        {
            switch (name)
            {
                //case MainMenuList.BRASSIN:
                //    _selectedmenu = typeof(Discipline.Administrator.CrewBrassIn);
                //    break;
                //case MainMenuList.BRASSOUT:
                //    _selectedmenu = typeof(Discipline.Administrator.CrewBrassOut);
                //    break;
                //case MainMenuList.TIMEPROGRESS:
                //    _selectedmenu = typeof(Discipline.Progress.SelectCategory);
                //    break;
                ////case MainMenuList.SAFETY:
                ////    _selectedmenu = typeof(MainMenu);
                ////    break;
                ////case MainMenuList.MANAGETIMESHEET:
                ////    _selectedmenu = typeof(MainMenu);
                ////    break;
                //case MainMenuList.IWPVIEWER:
                //    _selectedmenu = typeof(Discipline.IWP.IWPGridViewer);
                //    break;
                //case MainMenuList.DRAWINGVIEWER:
                //    _selectedmenu = typeof(Discipline.Drawing.DrawingGridViewer);
                //    break;
                default:
                    _selectedmenu = typeof(MainMenu);
                    break;

            }
        }
    }
}