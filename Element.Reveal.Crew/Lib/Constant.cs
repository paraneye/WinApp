using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Reveal.Crew.Lib
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
        public const string Key_Reset = "Reset";
        public const string Key_CWP = "CWP";
        public const string Key_FIWP = "FIWP";
        public const string Key_DrawingType = "DrawingType";
        public const string Key_EngTag = "EngTag";
        public const string Key_Title = "Title";
        public const string Key_Sort = "Sorting";
        public const string Key_Project = "Project";
        public const string Key_Module = "Module";
        public const string Key_ForemanBrassIn = "ForemanBrassIn";
        public const string Key_CrewBrassIn = "CrewBrassIn";
        public const string Key_ToolboxIn = "ToolboxIn";
        public const string Key_LoginAccount = "LoginAccount";
        public const string Key_ITRList = "ITRList";

        //This is aimed for XAML Element
        public static string DrawingType { get { return Key_DrawingType; } }
        public static string Reset { get { return Key_Reset; } }
        public static string CWP { get { return Key_CWP; } }
        public static string FIWP { get { return Key_FIWP; } }
    }


    public sealed class MainMenuGroup
    {
        public const string CREWTASK = "Crew Task";
        public const string ITR = "ITR";
        public const string REPORT = "Report";
        public const string VISUAL = "Visual";

    }

    public sealed class MainMenuList
    {
        //Crew Tasks
        public const string BRASSIN = "Crew Brass In";
        public const string BRASSOUT = "Crew Brass Out";
        public const string TIMEPROGRESS = "Timt & Progress";
        public const string SAFETY = "Safety";
        public const string MANAGETIMESHEET = "Manage Timesheet";
        public const string ToolBoxTalkSelection = "ToolBoxTalk Selection";

        //Report
        public const string RFISTATUS = "RFI Status";
        public const string IWPREPORT = "IWP Report";
        public const string SIWPREPORT = "SIWP Report";
        public const string MATERIALREPORT = "Material Report";

        //Visual
        public const string IWPVIEWER = "IWP Viewer";
        public const string SIWPVIEWER = "SIWP Viewer";
        public const string DRAWINGVIEWER = "Drawing Viewer";

        //ITR
        //public const string DownloadITR = "Download ITR";
        //public const string FilloutITR = "FillOut ITR";
        //public const string SubmitITR = "Submit ITR";
        public const string ITRMenu = "ITR Menu";
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

    public sealed class GateNo
    {
        public const int BRASSOUTLIST = 0;
        public const int BRASSIN = 1;
        public const int TOOLBOXTALK = 3;
        public const int BRASSOUT = 9;
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

    public sealed class Department
    {
        public const int Owner = 21;
        public const int Piping = 1;
        public const int Electrical = 2;
        public const int Instrumentation = 3;
        public const int Scaffold = 4;
        public const int Insulation = 5;
        public const int Steel = 6;
        public const int Mechanical = 7;
        public const int Civil = 8;
        public const int MAGS = 9;
        public const int ConstructionManagement = 10;
        public const int Superintendant = 11;
        public const int ProjectManagement = 12;
        public const int GeneralForeman = 13;
        public const int Foreman = 14;
        public const int Crew = 15;
        public const int SupportStaff = 16;
        public const int MaterialManagement = 17;
        public const int QualityManagement = 18;
        public const int FieldEngineering = 19;
        public const int Administration = 20;
        public const int ProjectControl = 22;
        public const int EquipmentManagement = 23;
        public const int SafetyEnvironment = 25;
        public const int DocumentControl = 27;
        public const int Planner = 28;
        public const int Coordinator = 30;
    }

    public sealed class ITRList
    {
        public const string DownloadList = "ITR_Download_List.xml";
        public const int CableTrayInspection = 117;
        public const int LightingDeviceInstallation = 118;
        public const int LightingDeviceCircuit = 119;
        public const int PowerCableReelReceiving = 120;
        public const int ControlCableReelReciving = 121;
        public const int InstrumentCableReelReceiving = 122;
        public const int PowerCableInspection = 123;
        public const int ControlCableInspection = 124;
        public const int InstrumentCableInspection = 125;
        public const int MIReceiving = 126;
        public const int SRPLReceiving = 127;
        public const int MIInspection = 128;
        public const int SRPLInspection = 129;
    }

    public sealed class UcTitle
    {
        public const string CableTrayInspectionTitle = "Field Inspection of Cable Tray";
        public const string LightingDeviceInstallationTitle = "Lighting & Device Installation";
        public const string LightingDeviceCircuitTitle = "Lighting & Device Circuit";
        public const string PowerCableReelReceivingTitle = "Power Cable Receiving";
        public const string ControlCableReelRecivingTitle = "Control Cable Receiving";
        public const string InstrumentCableReelReceivingTitle = "Instrumentation Cable Receiving";
        public const string PowerCableInspectionTitle = "Power Cable Inspection";
        public const string ControlCableInspectionTitle = "Control Cable Inspection";
        public const string InstrumentCableInspectionTitle = "Instrumentation Cable Inspection";
        public const string MIReceivingTitle = "MI Receiving";
        public const string SRPLReceivingTitle = "SR/PL Receiving";
        public const string MIInspectionTitle = "MI Inspection";
        public const string SRPLInspectionTitle = "SR/PL Inspection";
    }
}
