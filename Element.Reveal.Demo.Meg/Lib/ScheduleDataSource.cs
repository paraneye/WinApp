using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

namespace Element.Reveal.Meg.Lib
{
    public class ScheduleDataSource
    {
        private List<RevealProjectSvc.ProjectscheduleDTO> _schedule = new List<RevealProjectSvc.ProjectscheduleDTO>();
        public List<RevealProjectSvc.ProjectscheduleDTO> Schedule { get { return _schedule; } }
        public static int selectedSchedule { get; set; }
        public static string selectedScheduleName { get; set; }
        public static string selectedScheduleStartDate { get; set; }
        public static string selectedScheduleEndDate { get; set; }
        public static int selectedGeneralForeman { get; set; }        

        public async Task<bool> GetProjectScheduleByProjectWithWBSOnMode(int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleByProjectWithWBS(projectId, moduleId);
                _schedule = result;

                if (_schedule != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetProjectScheduleByCwpProjectIDWithWBSOnMode(int cwpId, int projectId, int moduleId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleByCwpProjectIDWithWBS(cwpId, projectId, moduleId);
                _schedule = result;

                if (_schedule != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetProjectScheduleByCwpProjectWithWBS(int cwpId, int projectId)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleByCwpProjectWithWBS(cwpId, projectId);
                _schedule = result;

                if (_schedule != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetProjectScheduleByCwpProjectPackageTypeWithWBS(int cwpId, int projectId, int packagetypeLuid)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleByCwpProjectPackageTypeWithWBS(cwpId, projectId, packagetypeLuid);
                _schedule = result;

                if (_schedule != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }
        

        public List<DataGroup> GetGroupedSchedule()
        {
            List<RevealProjectSvc.ProjectscheduleDTO> titles = new List<RevealProjectSvc.ProjectscheduleDTO>();
            List<DataGroup> grouplist = new List<DataGroup>();

            DataGroup group;
            titles = _schedule.Where(x => x.IsWBS == 1).ToList();

            try
            {
                for (int i = 0; i < titles.Count(); i++)
                {
                    group = new DataGroup("Group" + i.ToString(), titles[i].ProjectScheduleName, "");

                    group.Items = _schedule.Where(y => y.IsWBS == 3 && titles[i].P6WBSCode == y.P6WBSCode.Split('.')[0]).Select(y =>
                            new DataItem(y.ProjectScheduleID.ToString(), y.P6ActivityID + " - " + y.ProjectScheduleName, y.StartDate + "~" + y.FinishDate, y.DepartStructureID.ToString(), group) { }).ToObservableCollection();

                    //group.Items = _schedule.Where(y => y.IsWBS == 3
                    //    && titles[i].P6WBSCode == y.P6WBSCode.Substring(0, y.P6WBSCode.LastIndexOf("."))).Select(y =>
                    //        new DataItem(y.ProjectScheduleID.ToString(), y.P6ActivityID + " - " + y.ProjectScheduleName, y.StartDate + "~" + y.FinishDate, y.DepartStructureID.ToString(), group) { }).ToObservableCollection();

                    if (group.Items.Count > 0)
                        grouplist.Add(group);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return grouplist;
        }
    }
}
