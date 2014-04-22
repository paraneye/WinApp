using oz.api;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Element.Reveal.TrueTask.Lib.Common
{
    public class OZReportCommandListenerImpl : OZReportCommandListener
    {

        private Discipline.Viewer.IWPGridViewer _viewerPage;
        public OZReportCommandListenerImpl(Discipline.Viewer.IWPGridViewer viewerPage)
        {
            _viewerPage = viewerPage;
        }

        public void OZCloseCommand()
        {
           //viewerPage.DisposeViewer();
        }

        public void OZPostCommand(string cmd, string msg) { }
        public void OZPrintCommand(string msg, string code, string reportname, string printername, string printcopy, string printpages, string printrange, string username, string printerdrivername, string printpagesrange) { }
        public void OZExportCommand(string code, string path, string filename, string pagecount, string filenames) { }
        public void OZProgressCommand(string step, string state, string reportname) { }
        public void OZErrorCommand(string code, string errmsg, string detailmsg, string reportname) { }
        public void OZCommand(string code, string args) { }
        public void OZExitCommand() { }
        public void OZMailCommand(string code) { }
        public void OZUserActionCommand(string type, string attrs) { }
        public void OZLinkCommand(string docIndex, string compName, string tag, string value, string mouseButton) { }
        public void OZBankBookPrintCommand(string data) { }
        public void OZPageChangeCommand(string docIndex) { }
        public void OZPageBindCommand(string docIndex, string pagecount) { }
        public void OZReportChangeCommand(string docIndex) { }

        public string OZUserEvent(string param1, string param2, string param3)
        {


            return null;
        }
        public bool OZWillChangeIndex_Paging(int newIndex, int oldIndex) { return false; }

        public void OZTextBoxCommand(OZRTextBoxCmd textBoxCmd, int mode) { } // mode: 0=OPEN, 1=CLOSE(OK), -1=CLOSE(CANCEL)
    };
}