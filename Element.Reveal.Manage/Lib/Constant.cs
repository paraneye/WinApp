using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Reveal.Manage.Lib
{
    public sealed class ContentPath
    {
        private static Windows.Storage.StorageFolder _offfolder;
        private static Windows.Storage.StorageFolder _userfolder;
        private static Windows.Storage.StorageFolder _loginfolder;

        private static async void SetOffFolder()
        {
            _offfolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("OffMode", Windows.Storage.CreationCollisionOption.OpenIfExists);
        }

        public static Windows.Storage.StorageFolder OffModeFolder
        {
            get
            {
                SetOffFolder();
                return _offfolder;
            }
        }

        private static async void SetUserFolder()
        {
            var UserFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("OffMode", Windows.Storage.CreationCollisionOption.OpenIfExists);
            _userfolder = await UserFolder.CreateFolderAsync(Login.UserAccount.PersonnelID.ToString(), Windows.Storage.CreationCollisionOption.OpenIfExists);
        }

        public static Windows.Storage.StorageFolder OffModeUserFolder
        {
            get
            {
                SetUserFolder();
                return _userfolder;
            }
        }

        private static async void SetLoginFolder()
        {
            string foldername = string.IsNullOrEmpty(WinAppLibrary.Utilities.Helper.LoginID) ? Login.UserAccount.LoginName.ToString() : WinAppLibrary.Utilities.Helper.LoginID;
            var UserFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("OffMode", Windows.Storage.CreationCollisionOption.OpenIfExists);
            _loginfolder = await UserFolder.CreateFolderAsync(foldername, Windows.Storage.CreationCollisionOption.OpenIfExists);
        }

        public static Windows.Storage.StorageFolder OffModeLoginFolder
        {
            get
            {
                SetLoginFolder();
                return _loginfolder;
            }
        }

        public const string DefaultDrawing = "Assets/Default.JPG";
        public const string DrawingSource = "Drawing.xml";
        public const string DocumentNote = "D_N.xml";
        public const string ProjectSource = "P_.xml";
        public const string ModuleSource = "M_.xml";
        public const string GroupingCWP = "G_C.xml";
        public const string GroupingFIWP = "G_F.xml";
        public const string GroupingDrawingType = "G_GT.xml";
        public const string GroupingSelectoin = "G_S.xml";
        public const string BrassIn = "BI.xml";
        public const string BrassSignIn = "BSI.xml";
        public const string ToolBoxTalk = "TBT.xml";
        public const string LoginAccount = "UA.xml";
    }

    //Chnages are subject to committing to lose of previously stored data.
    public sealed class HashKey
    {
        //public const string Key_Reset = "Reset";
        //public const string Key_CWP = "CWP";
        //public const string Key_FIWP = "FIWP";
        //public const string Key_DrawingType = "DrawingType";
        //public const string Key_EngTag = "EngTag";
        //public const string Key_Title = "Title";
        //public const string Key_Sort = "Sorting";
        //public const string Key_Project = "Project";
        //public const string Key_Module = "Module";
        //public const string Key_ForemanBrassIn = "ForemanBrassIn";
        //public const string Key_CrewBrassIn = "CrewBrassIn";
        //public const string Key_ToolboxIn = "ToolboxIn";
        public const string Key_LoginAccount = "LoginAccount";

        ////This is aimed for XAML Element
        //public static string DrawingType { get { return Key_DrawingType; } }
        //public static string Reset { get { return Key_Reset; } }
        //public static string CWP { get { return Key_CWP; } }
        //public static string FIWP { get { return Key_FIWP; } }
    }

    public sealed class MainMenuGroup
    {
        public const string TEST = "Manage Test";

        public const string PROJECT_CONTROL = "Project Controls";
        public const string QUALITY = "Quality";
        public const string SAFETY = "Safety";
        public const string DIAGNOSTIC_CENTER = "Diagnostic Center";
        public const string TOOLS = "Tools";
    }

    public sealed class MainMenuList
    {
        ////TEST
        public const string TEST_VR = "Test - Voice Recognition";
        public const string TEST_WAKE = "Test - Wake Up";

        
        ////Project Controls
        public const string QUANTITY_SURVEY = "Quantity Survey";

        ////Quality
        public const string MAINTAIN_QA_QC_FORM = "Maintain QA/QC Form";
        public const string CHANGE_ORDER = "Change Order";
        public const string QAQC_REPORT = "QAQC Report";
        public const string TURN_OVER = "Turn Over";

        ////Safety
        public const string SAFETY_AUDIT = "Safety Audit";
        public const string INCIDENT_REPORT = "Incident Report";
        public const string SAFETY_SUMMARY = "Safety Summary";

        ////Diagnostic Center
        public const string CREW_PERFORMANCE = "Crew Performance";
        public const string PROJECT_PLANNED_VS_ACTUAL_EARNED = "Project Planned vs Actual Earned";
        public const string SYSTEM_REPORT = "System Report";
        public const string INFORMATION_REQUEST_STATUS = "Information Request Status";
        public const string IWP_REPORT = "IWP Report";
        public const string SCHEDULE_PROGRESS_N_PRODUCTIVITY = "Schedule Progress & Productivity";
        public const string MTO_SUMMARY = "MTO Summary";
        public const string EWO_REPORT = "EWO Report";
        public const string CONSTCODE_STRUCTURE = "Constcode Structure";
        public const string RFO = "RFO";

        ////Tools
        public const string DRAWING_VIEWER = "Drawing Viewer";
        public const string SIWP_VIEWER = "SIWP Viewer";
        public const string IWP_VIEWER = "IWP Viewer";
        public const string TURNOVER_BINDER = "Turnover Binder";
    }

    public sealed class GateNo
    {
        //public const int BRASSOUTLIST = 0;
        //public const int BRASSIN = 1;
        //public const int TOOLBOXTALK = 3;
        //public const int BRASSOUT = 9;
    }

    public enum NotifyType
    {
        StatusMessage,
        NdefMessage,
        PublishMessage,
        PeerMessage,
        ErrorMessage,
        ClearMessage
    };

    public sealed class Threshold
    {
        public const double ANIMATION_TIME = 0.5;
    }
    
    public sealed class QAQCGroup
    {
        public const int GROUP01 = 1074;
        public const int GROUP02 = 1075;
        public const int GROUP03 = 1076;
        public const int GROUP04 = 1077;
        public const int GROUP05 = 1078;
        public const int GROUP06 = 1751;
        public const int GROUP07 = 1752;
        public const int GROUP08 = 1753;
        public const int GROUP09 = 1754;
        public const int GROUP10 = 1755;

        public const int Header = 1749;
        public const int Grid = 1750;
    }
}
