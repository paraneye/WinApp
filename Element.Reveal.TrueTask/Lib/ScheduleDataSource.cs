using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

namespace Element.Reveal.TrueTask.Lib
{
    public class ScheduleDataSource
    {
        private List<DataLibrary.ProjectscheduleDTO> _schedule = new List<DataLibrary.ProjectscheduleDTO>();
        public List<DataLibrary.ProjectscheduleDTO> Schedule { get { return _schedule; } }

        #region DataLibrary사용시 사용(임시)
        //todo 아직 DataLibrary 확정 된거 아님
        private static List<DataLibrary.ProjectscheduleDTO> _scheduleJson = new List<DataLibrary.ProjectscheduleDTO>();
        public List<DataLibrary.ProjectscheduleDTO> ScheduleJson { get { return _scheduleJson; } }


        #endregion

        public static int selectedSchedule { get; set; }
        public static string selectedScheduleName { get; set; }
        public static string selectedScheduleStartDate { get; set; }
        public static string selectedScheduleEndDate { get; set; }
        public static string selectedGeneralForeman { get; set; }

        public async Task<bool> GetProjectScheduleByProjectWithWBSOnMode(int projectId, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleByProjectWithWBS(projectId, disciplineCode);
                _scheduleJson = result;

                if (_scheduleJson != null)
                    retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> GetProjectScheduleByCwpProjectIDWithWBSOnMode(int cwpId, int projectId)//, string disciplineCode)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleByCwpProjectIDWithWBS(cwpId, projectId);//, disciplineCode);
                _scheduleJson = result;

                if (_scheduleJson != null)
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

        public async Task<bool> GetProjectScheduleByCwpProjectPackageTypeWithWBS(int cwpId, int projectId, string packagetypeLuid)
        {
            bool retValue = false;
            try
            {
                var result = await (new Lib.ServiceModel.ProjectModel()).GetProjectScheduleByCwpProjectPackageTypeWithWBS(cwpId, projectId, packagetypeLuid);
                _scheduleJson = result;

                if (_scheduleJson != null)
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
            List<DataLibrary.ProjectscheduleDTO> titles = new List<DataLibrary.ProjectscheduleDTO>();
            List<DataGroup> grouplist = new List<DataGroup>();

            DataGroup group;

            if (_scheduleJson != null)
            {
                titles = _scheduleJson.Where(x => x.IsWBS == DataLibrary.Utilities.IsWBS.Level3).ToList();
            }

            try
            {
                for (int i = 0; i < titles.Count(); i++)
                {
                    group = new DataGroup("Group" + i.ToString(), titles[i].ProjectScheduleName, "");

                    //group.Items = _scheduleJson.Where(y => y.IsWBS == 3 && titles[i].P6WBSCode == y.P6WBSCode.Split('.')[0]).Select(y =>
                    group.Items = _scheduleJson.Where(y => y.IsWBS == DataLibrary.Utilities.IsWBS.Level4 && titles[i].P6ActivityObjectID == y.P6ParentObjectID).Select(y =>
                            new DataItem(y.ProjectScheduleID.ToString(), y.P6ActivityID + " - " + y.ProjectScheduleName, y.StartDate + "~" + y.FinishDate, y.LeaderId, group) { }).ToObservableCollection();

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

        public bool SetSchduleInfo(DataLibrary.ProjectscheduleDTO dto)
        {
            try
            {
                List<DataLibrary.ProjectscheduleDTO> ldto = new List<DataLibrary.ProjectscheduleDTO>();
                ldto.Add(dto);
                _scheduleJson = ldto;
            }
            catch( Exception e)
            {
            }
            return true;
        }
        
    }
}
