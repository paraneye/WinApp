using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Meg.Lib.DataSource
{
    public class DownloadITRDataSource
    {
        private List<RevealProjectSvc.FiwpqaqcDTO> _ITRList = new List<RevealProjectSvc.FiwpqaqcDTO>();
        public List<RevealProjectSvc.FiwpqaqcDTO> ITRList { get { return _ITRList; } }

        private List<RevealProjectSvc.QaqcformDTO> _QaqcFormList = new List<RevealProjectSvc.QaqcformDTO>();
        public List<RevealProjectSvc.QaqcformDTO> QaqcFormList { get { return _QaqcFormList; } }

        public async Task<bool> GetITRListByFiwp(int fiwpId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetITRListByFiwp(fiwpId);
                _ITRList = result.Where(x => x.Downloaded < 1).ToList();   ///Downloaded 0:Download Not yet, 1: Download Complete

                if (_ITRList != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<RevealProjectSvc.FiwpqaqcDTO> GetITRList()
        {
            return _ITRList;
        }

        public List<DataGroup> GetGroupITRList()
        {
            //List<RevealProjectSvc.FiwpqaqcDTO> titles = new List<RevealProjectSvc.FiwpqaqcDTO>();
            List<string> titles = new List<string>();
            List<DataGroup> grouplist = new List<DataGroup>();

            DataGroup group;
            //titles = _ITRList.Select(x => x.Group).Distinct().ToList();
            
            group = new DataGroup("Group" + 1, titles[0], "");

            group.Items = _ITRList.Select(y =>
                    new DataItem(y.QAQCFormTemplateID.ToString(), y.QAQCFormCode, y.QAQCFormTypeLUID.ToString(), y.QAQCFormTemplateName.ToString(), group) { }).ToObservableCollection();

            grouplist.Add(group);


            return grouplist;
        }

        public async Task<bool> GetDownloadQaqc(int projectId, int moduleId, int cwpId, int fiwpId, List<RevealProjectSvc.QaqcformtemplateDTO> qaqcformtemplate, string updatedBy, int qaqcType)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).SaveQaqcformForDownload(projectId, moduleId, cwpId, fiwpId, qaqcformtemplate, updatedBy, qaqcType);
                _QaqcFormList = result;

                if (_QaqcFormList != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<RevealProjectSvc.QaqcformDTO> GetDownloadQaqcList()
        {
            return _QaqcFormList;
        }

    }
}
