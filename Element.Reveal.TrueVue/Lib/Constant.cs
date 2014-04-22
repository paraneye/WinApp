using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Reveal.TrueVue.Lib
{    
    public sealed class ContentPath
    {
        private static Windows.Storage.StorageFolder _offfolder;
       
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
        public const string DefaultDrawing = "Assets/Default.JPG";
        public const string DrawingSource = "Drawing.xml";
        public const string DocumentNote = "D_N.xml";
        public const string ProjectSource = "P_.xml";
        public const string ModuleSource = "M_.xml";
        public const string GroupingCWP = "G_C.xml";
        public const string GroupingFIWP = "G_F.xml";
        public const string GroupingDrawingType = "G_GT.xml";
        public const string GroupingSelectoin = "G_S.xml";
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

        //This is aimed for XAML Element
        public static string DrawingType { get { return Key_DrawingType; } }
        public static string Reset { get { return Key_Reset; } }
        public static string CWP { get { return Key_CWP; } }
        public static string FIWP { get { return Key_FIWP; } }
    }
}
