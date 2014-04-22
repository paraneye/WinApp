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
    public class QaqcDataSource
    {
        private List<RevealProjectSvc.QaqcformdetailDTO> _QaqcFormDetailList = new List<RevealProjectSvc.QaqcformdetailDTO>();
        public List<RevealProjectSvc.QaqcformdetailDTO> QaqcFormDetailList { get { return _QaqcFormDetailList; } }

        private RevealProjectSvc.QaqcformDTO _QaqcformDTO = new RevealProjectSvc.QaqcformDTO();
        public RevealProjectSvc.QaqcformDTO QaqcformDTO { get { return _QaqcformDTO; } }

        private RevealProjectSvc.PunchDTOSet _PunchDTOSet = new RevealProjectSvc.PunchDTOSet();
        public RevealProjectSvc.PunchDTOSet PunchDTOSet { get { return _PunchDTOSet; } }

        //Download PunchTicket List
        public async Task<bool> GetPunchListByPersonnelDepartment(int projectId, int moduleId, int personnelId, int departmentId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetQaqcformByPersonnelDepartment(projectId, moduleId, personnelId, departmentId);
                _QaqcFormDetailList = result;

                if (_QaqcFormDetailList != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public List<RevealProjectSvc.QaqcformdetailDTO> GetPunchListByPersonnelDepartment()
        {
            return _QaqcFormDetailList;
        }

        public async Task<bool> GetQaqcformbyID(int qaqcformId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetQaqcformbyID(qaqcformId);
                _QaqcformDTO = result;

                if (_QaqcformDTO != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public RevealProjectSvc.QaqcformDTO GetQaqcformbyID()
        {
            return _QaqcformDTO;
        }

        //Get Punch Ticket
        public async Task<bool> GetPunchTicketByQaqcform(int qaqcformId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetPunchListByQaqcform(qaqcformId);
                _PunchDTOSet = result;

                if (_PunchDTOSet != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;

        }

        public RevealProjectSvc.PunchDTOSet GetPunchTicketByQaqcform()
        {
            return _PunchDTOSet;
        }

        //Save Punch Ticket
        public async Task<bool> SaveQaqcformWithSharePoint(RevealProjectSvc.WalkdownDTOSet qaqcforms)
        {
            bool retValue = false;

            try
            {
                await (new Lib.ServiceModel.ProjectModel()).SaveQaqcformWithSharePoint(qaqcforms);
                retValue = true;
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "SaveQaqcformWithSharePoint");
            }

            return retValue;
        }
    }
}
