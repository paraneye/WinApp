using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Extensions;
using WinAppLibrary.Utilities;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Element.Reveal.Manage.Lib
{
    public sealed class ScheduleDataSource
    {
        private List<RevealProjectSvc.ProjectscheduleDTO> _schedule = new List<RevealProjectSvc.ProjectscheduleDTO>();

        public List<DataGroup> GetGroupedSchedule()
        {
            List<RevealProjectSvc.ProjectscheduleDTO> titles = new List<RevealProjectSvc.ProjectscheduleDTO>();
            List<DataGroup> grouplist = new List<DataGroup>();

            DataGroup group;
            titles = _schedule.Where(x => x.IsWBS == 1).ToList();

            for (int i = 0; i < titles.Count(); i++)
            {
                group = new DataGroup("Group" + i.ToString(), titles[i].ProjectScheduleName, "");

                group.Items = _schedule.Where(y => y.IsWBS == 3
                    && titles[i].P6WBSCode == y.P6WBSCode.Substring(0, y.P6WBSCode.LastIndexOf("."))).Select(y =>
                        new DataItem(y.ProjectScheduleID.ToString(), y.P6ActivityID + " - " + y.ProjectScheduleName, y.StartDate + "~" + y.FinishDate, y.DepartStructureID.ToString(), group) { }).ToObservableCollection();

                if (group.Items.Count > 0)
                    grouplist.Add(group);
            }

            return grouplist;
        }
    }
}
