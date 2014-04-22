using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Crew.Lib.DataSource
{
    public class ToolBoxTalkSelectionDataSource
    {
        public static string strFileName;

        private List<RevealProjectSvc.DocumentDTO> _Document = new List<RevealProjectSvc.DocumentDTO>();
        public List<RevealProjectSvc.DocumentDTO> Doument { get { return _Document; } }

        private static List<RevealProjectSvc.DailytoolboxDTO> _dailytoolboxtalk = new List<RevealProjectSvc.DailytoolboxDTO>();
        public static List<RevealProjectSvc.DailytoolboxDTO> DailytoolBoxTalk { get { return _dailytoolboxtalk; } }

        private static List<RevealProjectSvc.ToolboxsignDTO> _toolboxsign;

        public static List<RevealProjectSvc.ToolboxsignDTO> ToolBoxSign
        {
            get
            {
                if (_toolboxsign == null)
                    return new List<RevealProjectSvc.ToolboxsignDTO>();
                else
                    return _toolboxsign;
            }
            set
            {
                _toolboxsign = value;
            }
        }

        public async Task<bool> GetSafetyDocumentsList(int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetSafetyDocumentsList(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);
                _Document = result;

                if (_Document != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public List<DataGroup> GetGroupSharePoint()
        {
            //List<RevealProjectSvc.DocumentDTO> titles = new List<RevealProjectSvc.DocumentDTO>();
            List<string> titles = new List<string>();
            List<DataGroup> grouplist = new List<DataGroup>();

            DataGroup group;
            titles = _Document.Select(x => x.Group).Distinct().ToList();

            for (int i = 0; i < titles.Count(); i++)
            {
                group = new DataGroup("Group" + i.ToString(), titles[i], "");

                group.Items = _Document.Where(x => x.Group == titles[i]).Select(y =>
                        new DataItem(y.SPCollectionID.ToString(), y.Group, y.LocationURL.ToString(), y.Description.ToString(), group) { }).ToObservableCollection();

                grouplist.Add(group);
            }

            return grouplist;
        }






    }
}
