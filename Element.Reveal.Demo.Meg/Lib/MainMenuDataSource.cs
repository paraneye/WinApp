using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Meg.Lib
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

            //MTO
            var group0 = new DataGroup("MTO",
                    "MTO",
                    "Assets/DarkGray.png");

            group0.Items.Add(new DataItem(Lib.MainMenuList.MTO, "MTO", "Assets/truetask_icon_buildschedule.png", "", group0));

            //

            var group1 = new DataGroup("Build",
                    "Build",
                    "Assets/DarkGray.png");

            group1.Items.Add(new DataItem(Lib.MainMenuList.BuildSchedule, "Build Schedule", "Assets/truetask_icon_buildschedule.png", "", group1));
            group1.Items.Add(new DataItem(Lib.MainMenuList.BuildIWP, "Build IWP", "Assets/truetask_icon_buildiwp.png", "", group1));
            //group1.Items.Add(new DataItem(Lib.MainMenuList.BuildSIWP, "Build SIWP", "Assets/truetask_icon_buildsiwp.png", "", group1));
            group1.Items.Add(new DataItem(Lib.MainMenuList.BuildHydro, "Build Hydro", "Assets/truetask_icon_buildhydropackage.png", "", group1));

            var group2 = new DataGroup("Manage and Maintain",
                    "Manage and Maintain",
                    "Assets/DarkGray.png");

            group2.Items.Add(new DataItem(Lib.MainMenuList.ManageSchedule, "Manage Schedule", "Assets/truetask_icon_manageschedule.png", "", group2));
            group2.Items.Add(new DataItem(Lib.MainMenuList.AssembleIWP, "Assemble IWP", "Assets/truetask_icon_assembleiwp.png", "", group2));
            group2.Items.Add(new DataItem(Lib.MainMenuList.AssembleSIWP, "Assemble SIWP", "Assets/truetask_icon_assemblesiwp.png", "", group2));
            group2.Items.Add(new DataItem(Lib.MainMenuList.AssembleHydroPackage, "Assemble Hydro Package", "Assets/truetask_icon_assemblehydropackage.png", "", group2));

            var group3 = new DataGroup("Tool(Viewer)",
                   "Tool(Viewer)",
                   "Assets/DarkGray.png");

            group3.Items.Add(new DataItem(Lib.MainMenuList.DrawingViewer, "Drawing Viewer", "Assets/truetask_icon_drawingviewer.png", "", group3));
            group3.Items.Add(new DataItem(Lib.MainMenuList.IWPViewer, "IWP Viewer", "Assets/truetask_icon_iwpviewer.png", "", group3));
            group3.Items.Add(new DataItem(Lib.MainMenuList.SIWPViewer, "SIWP Viewer", "Assets/truetask_icon_siwpviewer.png", "", group3));
            group3.Items.Add(new DataItem(Lib.MainMenuList.TurnoverbinderViewer, "Turnover Binder", "Assets/truetask_icon_turnoverbinder.png", "", group3));
            group3.Items.Add(new DataItem(Lib.MainMenuList.HydroViewer, "Turnover Binder", "Assets/truetask_icon_hydropackageviewer.png", "", group3));
            group3.Items.Add(new DataItem(Lib.MainMenuList.CSUViewer, "Turnover Binder", "Assets/truetask_icon_CSUViewer.png", "", group3));

            var group4 = new DataGroup("ITR",
                   "ITR",
                   "Assets/DarkGray.png");

            group4.Items.Add(new DataItem(Lib.MainMenuList.InspectionTestRecord, "Inspection & Test Record", "Assets/truetask_icon_ITR.png", "", group4));
            group4.Items.Add(new DataItem(Lib.MainMenuList.ITRReport, "ITR Summary Report", "Assets/truetask_icon_ITRSummaryreport.png", "", group4));

            var group5 = new DataGroup("Turnover",
                   "Turnover",
                   "Assets/DarkGray.png");

            group5.Items.Add(new DataItem(Lib.MainMenuList.WalkDown, "Walk-down", "Assets/truetask_icon_walkdown.png", "", group5));
            group5.Items.Add(new DataItem(Lib.MainMenuList.AssignmentPuch, "Assignment Punch", "Assets/truetask_icon_assignmentpunch.png", "", group5));
            group5.Items.Add(new DataItem(Lib.MainMenuList.PunchListCard, "Punch List/Card", "Assets/truetask_icon_punchlistcard.png", "", group5));
            group5.Items.Add(new DataItem(Lib.MainMenuList.PunchSummary, "Punch Summary ", "Assets/truetask_icon_Punchstatusreport.png", "", group5));
            group5.Items.Add(new DataItem(Lib.MainMenuList.TurnoverSystem, "Turnover System", "Assets/truetask_icon_turnoversystem.png", "", group5));
            group5.Items.Add(new DataItem(Lib.MainMenuList.BuildCSU, "Build C&SU ", "Assets/truetask_icon_BuildCSUworkpackage.png", "", group5));
           // group5.Items.Add(new DataItem(Lib.MainMenuList.AssembleCSU, "Assemble C&SU", "Assets/truetask_icon_AssembleCSUworkpackage.png", "", group5));
            
         //   _datasource.AllGroups.Add(group0);
            _datasource.AllGroups.Add(group1);
            _datasource.AllGroups.Add(group2);
            _datasource.AllGroups.Add(group3);

            _datasource.AllGroups.Add(group4);
            _datasource.AllGroups.Add(group5);
        }

        public static void SetCurrentMenu(string name)
        {
            Login.MasterPage.SetPageTitle("");
            Login.MasterPage.ShowBackButton(false);
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
                    _selectedmenu = typeof(Discipline.Schedule.BuildHydro.SelectCWP);
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
                case MainMenuList.SIWPViewer:
                    _selectedmenu = typeof(Discipline.Viewer.SIWPGridViewer);
                    break;
                case MainMenuList.TurnoverbinderViewer:
                    _selectedmenu = typeof(Discipline.Viewer.TurnoverbinderGridViewer);
                    break;
                case MainMenuList.MTO:
                    _selectedmenu = typeof(Discipline.MTO.SelectCWP);
                    break;
                case MainMenuList.AssignmentPuch:
                    _selectedmenu = typeof(Discipline.PunchCard.AssignmentPunch);
                    break;
                    
                case MainMenuList.AssembleSIWP:
                    _selectedmenu = typeof(Discipline.Schedule.AssembleIWP.SelectIWP);
                    Lib.CommonDataSource.selPackageTypeLUID = Lib.PackageType.SIWP;
                    break;
                case MainMenuList.AssembleHydroPackage:
                    _selectedmenu = typeof(Discipline.Schedule.AssembleIWP.SelectCWP);
                    Lib.CommonDataSource.selPackageTypeLUID = Lib.PackageType.HydroTest;
                    break;
                case MainMenuList.InspectionTestRecord:
                    _selectedmenu = typeof(Discipline.ITR.ITRMenu);
                    break;
                case MainMenuList.ITRReport:
                    _selectedmenu = typeof(Discipline.ITR.ITRReportSummary);
                    break;
                case MainMenuList.WalkDown:
                    _selectedmenu = typeof(Discipline.PunchCard.FinalWalkDown);
                    break;
                case MainMenuList.PunchListCard:
                    _selectedmenu = typeof(Discipline.PunchCard.DownloadPunchList);
                    break;
                case MainMenuList.PunchSummary:
                    _selectedmenu = typeof(Discipline.PunchCard.PunchSummary);
                    break;
                case MainMenuList.TurnoverSystem:
                    _selectedmenu = typeof(Discipline.TurnOver.TurnoverSystem);
                    break;
                case MainMenuList.BuildCSU:
                    _selectedmenu = typeof(Discipline.Schedule.BuildCSU.SelectCSU);
                    break;
                case MainMenuList.AssembleCSU:
                    _selectedmenu = typeof(Discipline.MTO.SelectCWP);
                    break;
                case MainMenuList.HydroViewer:
                    _selectedmenu = typeof(Discipline.Viewer.HYDROGridViewer);
                    break;
                case MainMenuList.CSUViewer:
                    _selectedmenu = typeof(Discipline.Viewer.CSUViewer);
                    break;
            }
        }
    }
}
