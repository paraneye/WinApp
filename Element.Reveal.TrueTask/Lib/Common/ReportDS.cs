namespace Element.Reveal.TrueTask.Lib.Common
{
    public class ReportDS
    {
        public string ServerYn
        {
            get;
            set;
        }

        public string ServerPath
        {
            get;
            set;
        }

        public string ProjectCode
        {
            get;
            set;
        }

        public string ReportName
        {
            get;
            set;
        }

        public string OdiName
        {
            get;
            set;
        }

        public string OzdName
        {
            get;
            set;
        }

        public object[] Params
        {
            get;
            set;
        }

        public string Language
        {

            get;
            set;
        }

        public bool IsNoSignZoom
        {
            get;
            set;
        }

        /// <summary>
        /// Print, Export, Preview
        /// </summary>
        public string ViewMode
        {
            get;
            set;
        }

        public string ToolBarUseYn
        {
            get;
            set;
        }

        public string ToolBarSaveYn
        {
            get;
            set;
        }

        public string ToolBarPrintYn
        {
            get;
            set;
        }

        public string ToolBarDataSaveYn
        {
            get;
            set;
        }

        public string ToolBarPageMoveYn
        {
            get;
            set;
        }

        public string ToolBarZoomYn
        {
            get;
            set;
        }

        public string ToolBarPageControlYn
        {
            get;
            set;
        }

        public string ToolBarWithControlYn
        {
            get;
            set;
        }

        public string ToolBarFindYn
        {
            get;
            set;
        }

        public string ToolBarOtherMenuYn
        {
            get;
            set;
        }

        public object[,] ValidateCheckForSave
        {
            get;
            set;
        }
    }
}