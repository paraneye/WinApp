using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
namespace Element.Reveal.Manage.Lib
{
    class MainMenuDataSource
    {
        static Type _selectedmenu;
        public static Type CurrentMenu { get { return _selectedmenu; } }

        private GroupModel _datasource = new GroupModel();
        public GroupModel DataSource { get { return _datasource; } }
        public MainMenuDataSource(string name)
        {
            InitiateMenu(name);
        }

        public void InitiateMenu(string name)
        {
            _datasource.AllGroups.Clear();

            // Project Controls
            var group = new DataGroup(Lib.MainMenuGroup.PROJECT_CONTROL, Lib.MainMenuGroup.PROJECT_CONTROL, "Assets/DarkGray.png");

            group.Items.Add(new DataItem(Lib.MainMenuList.QUANTITY_SURVEY, Lib.MainMenuList.QUANTITY_SURVEY, "Assets/Menu/Manage_icon_QuantitySurvey.png", "", group));
            _datasource.AllGroups.Add(group);

            // Quality / Safety / Diagnostic Center
            AddMenu(name, _datasource);

            // Tools
            group = new DataGroup(Lib.MainMenuGroup.TOOLS, Lib.MainMenuGroup.TOOLS, "Assets/DarkGray.png");

            group.Items.Add(new DataItem(Lib.MainMenuList.DRAWING_VIEWER, Lib.MainMenuList.DRAWING_VIEWER, "Assets/Menu/Manage_icon_drawingviewer.png", "", group));
            group.Items.Add(new DataItem(Lib.MainMenuList.SIWP_VIEWER, Lib.MainMenuList.SIWP_VIEWER, "Assets/Menu/Manage_icon_siwpviewer.png", "", group));
            group.Items.Add(new DataItem(Lib.MainMenuList.IWP_VIEWER, Lib.MainMenuList.IWP_VIEWER, "Assets/Menu/Manage_icon_iwpviewer.png", "", group));
            group.Items.Add(new DataItem(Lib.MainMenuList.TURNOVER_BINDER, Lib.MainMenuList.TURNOVER_BINDER, "Assets/Menu/Manage_icon_TurnoverBinder.png", "", group));
            _datasource.AllGroups.Add(group);
        }

        private void AddMenu(string name, GroupModel data)
        {
            var group = new DataGroup(string.Empty, string.Empty, "Assets/DarkGray.png");;
            switch (name)
            {
                case MainMenuGroup.QUALITY:
                    group = new DataGroup(Lib.MainMenuGroup.QUALITY, Lib.MainMenuGroup.QUALITY, "Assets/DarkGray.png");
                    
                    group.Items.Add(new DataItem(Lib.MainMenuList.MAINTAIN_QA_QC_FORM, Lib.MainMenuList.MAINTAIN_QA_QC_FORM, "Assets/Menu/Manage_icon_MaintainQAQCFrom.png", "", group));

                    // icon 없음
                    group.Items.Add(new DataItem(Lib.MainMenuList.CHANGE_ORDER, Lib.MainMenuList.CHANGE_ORDER, "Assets/Menu/Manage_icon_informationrequeststatus.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.QAQC_REPORT, Lib.MainMenuList.QAQC_REPORT, "Assets/Menu/Manage_icon_informationrequeststatus.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.TURN_OVER, Lib.MainMenuList.TURN_OVER, "Assets/Menu/Manage_icon_informationrequeststatus.png", "", group));
                    _datasource.AllGroups.Add(group);
                    break;
                case MainMenuGroup.SAFETY:
                    group = new DataGroup(Lib.MainMenuGroup.SAFETY, Lib.MainMenuGroup.SAFETY, "Assets/DarkGray.png");

                    group.Items.Add(new DataItem(Lib.MainMenuList.SAFETY_AUDIT, Lib.MainMenuList.SAFETY_AUDIT, "Assets/Menu/Manage_icon_SafetyAudit.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.INCIDENT_REPORT, Lib.MainMenuList.INCIDENT_REPORT, "Assets/Menu/Manage_icon_IncidentReport.png", "", group));

                    // icon 없음
                    group.Items.Add(new DataItem(Lib.MainMenuList.SAFETY_SUMMARY, Lib.MainMenuList.SAFETY_SUMMARY, "Assets/Menu/Manage_icon_informationrequeststatus.png", "", group));
                    _datasource.AllGroups.Add(group);
                    break;
                case MainMenuGroup.DIAGNOSTIC_CENTER:
                    group = new DataGroup(Lib.MainMenuGroup.DIAGNOSTIC_CENTER, Lib.MainMenuGroup.DIAGNOSTIC_CENTER, "Assets/DarkGray.png");

                    group.Items.Add(new DataItem(Lib.MainMenuList.CREW_PERFORMANCE, Lib.MainMenuList.CREW_PERFORMANCE, "Assets/Menu/Manage_icon_crewperformance.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.PROJECT_PLANNED_VS_ACTUAL_EARNED, Lib.MainMenuList.PROJECT_PLANNED_VS_ACTUAL_EARNED, "Assets/Menu/Manage_icon_projectplanedvsactualearned.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.SYSTEM_REPORT, Lib.MainMenuList.SYSTEM_REPORT, "Assets/Menu/Manage_icon_systemreport.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.INFORMATION_REQUEST_STATUS, Lib.MainMenuList.INFORMATION_REQUEST_STATUS, "Assets/Menu/Manage_icon_informationrequeststatus.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.IWP_REPORT, Lib.MainMenuList.IWP_REPORT, "Assets/Menu/Manage_icon_iwpreport.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.SCHEDULE_PROGRESS_N_PRODUCTIVITY, Lib.MainMenuList.SCHEDULE_PROGRESS_N_PRODUCTIVITY, "Assets/Menu/Manage_icon_scheduleprogressandproductivity.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.MTO_SUMMARY, Lib.MainMenuList.MTO_SUMMARY, "Assets/Menu/Manage_icon_mtosummary.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.EWO_REPORT, Lib.MainMenuList.EWO_REPORT, "Assets/Menu/Manage_icon_EWOReport.png", "", group));
                    group.Items.Add(new DataItem(Lib.MainMenuList.CONSTCODE_STRUCTURE, Lib.MainMenuList.CONSTCODE_STRUCTURE, "Assets/Menu/Manage_icon_Costcode_Structure.png", "", group));

                    // icon 없음
                    group.Items.Add(new DataItem(Lib.MainMenuList.RFO, Lib.MainMenuList.RFO, "Assets/Menu/Manage_icon_informationrequeststatus.png", "", group));
                    _datasource.AllGroups.Add(group);
                    break;
            }
        }

        public static void SetCurrentMenu(string name)
        {
            switch (name)
            {
                case MainMenuList.QUANTITY_SURVEY:
                    _selectedmenu = typeof(Discipline.Survey.QS_SelectDisplineNCWP);
                    break;
                case MainMenuList.DRAWING_VIEWER:
                    _selectedmenu = typeof(Discipline.Survey.QS_VerifyComponents);  // 개발 때문에 임시로 연결
                    break;
                case MainMenuList.CREW_PERFORMANCE:
                    _selectedmenu = typeof(Discipline.Survey.TEST_Verify);  // 개발 때문에 임시로 연결 - SBang
                    break;
                case MainMenuList.PROJECT_PLANNED_VS_ACTUAL_EARNED:
                    _selectedmenu = typeof(Discipline.PunchCard.PunchListDashboard);
                    break;
            }
        }
    }
}
