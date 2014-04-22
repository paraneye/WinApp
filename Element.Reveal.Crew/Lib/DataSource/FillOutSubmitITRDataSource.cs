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
    public class FillOutSubmitITRDataSource
    {
        private List<RevealProjectSvc.QaqcformDTO> _QaqcFormList = new List<RevealProjectSvc.QaqcformDTO>();
        public List<RevealProjectSvc.QaqcformDTO> QaqcFormList { get { return _QaqcFormList; } }


        public async Task<bool> SaveQaqcformForSubmit(List<RevealProjectSvc.QaqcformDTO> qaqcforms)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).SaveQaqcformForSubmit(qaqcforms);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveQaqcformForSubmit");
            }

            return retValue;
        }


    }
}
