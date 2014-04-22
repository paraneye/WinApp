using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.TrueTask.Lib
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

            _datasource = WinAppLibrary.Menu.MenuDataSource.GetDataSrouce();
        }

        public static void SetCurrentMenu(string name)
        {
            switch (name)
            {
                case MainMenuList.BuildSchedule:
                    _selectedmenu = typeof(Discipline.Schedule.BuildSchedule.SelectCWP);
                    break;
                case MainMenuList.BuildIWP:
                    _selectedmenu = typeof(Discipline.Schedule.BuildIWP.SelectCWP);
                    break;
                case MainMenuList.BuildSIWP:
                    _selectedmenu = typeof(Discipline.Schedule.BuildSIWP.SelectSIWP);
                    break;
                case MainMenuList.BuildHydro:
                    _selectedmenu = typeof(Discipline.Schedule.BuildHydro.SelectHydro);
                    break;
                case MainMenuList.ManageSchedule:
                    _selectedmenu = typeof(Discipline.Schedule.ManageSchedule.SelectCWP);
                    break;
                case MainMenuList.AssembleIWP:                    
                    _selectedmenu = typeof(Discipline.Schedule.AssembleIWP.SelectCWP);
                    Lib.CommonDataSource.selPackageTypeLUID = Lib.PackageType.FIWP;
                    break;
                case MainMenuList.DrawingViewer:
                    _selectedmenu = typeof(Discipline.Viewer.DrawingGridViewer);
                    break;
                case MainMenuList.IWPViewer:
                    _selectedmenu = typeof(Discipline.Viewer.IWPGridViewer);
                    break;
                //case MainMenuList.SIWPViewer:
                //    _selectedmenu = typeof(Discipline.Viewer.SIWPGridViewer);
                //    break;
             /*   case MainMenuList.TurnoverbinderViewer:
                    _selectedmenu = typeof(Discipline.Viewer.TurnoverbinderGridViewer);
                    break;*/
                case MainMenuList.MTO:
                    _selectedmenu = typeof(Discipline.MTO.SelectCWP);
                    break;

                case MainMenuList.AssembleSIWP:
                    _selectedmenu = typeof(Discipline.Schedule.AssembleIWP.SelectIWP);
                    Lib.CommonDataSource.selPackageTypeLUID = Lib.PackageType.SIWP;
                    break;
                case MainMenuList.AssembleHydroPackage:
                    _selectedmenu = typeof(Discipline.Schedule.AssembleIWP.SelectCWP);
                    Lib.CommonDataSource.selPackageTypeLUID = Lib.PackageType.HydroTest;
                    break;
               
                case MainMenuList.BuildCSU:
                    _selectedmenu = typeof(Discipline.Schedule.BuildCSU.SelectCSU);
                    break;
                case MainMenuList.AssembleCSU:
                    _selectedmenu = typeof(Discipline.MTO.SelectCWP);
                    break;
                //case MainMenuList.HydroViewer:
                //    _selectedmenu = typeof(Discipline.Viewer.HYDROGridViewer);
                //    break;
                //case MainMenuList.CSUViewer:
                //    _selectedmenu = typeof(Discipline.Viewer.CSUViewer);
                //    break;

                case MainMenuList.SignOffStatus:
                    _selectedmenu = typeof(Discipline.IWPSignoff.IWPSignoffStatus);
                    break;
            }
        }
    }
}
