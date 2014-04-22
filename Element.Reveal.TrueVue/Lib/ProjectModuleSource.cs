using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.ServiceModels;

namespace Element.Reveal.TrueVue.Lib
{
    public class ProjectModuleSource
    {
        private static GroupModel _datasource = new GroupModel();
        public static GroupModel DataSource { get { return _datasource; } }
        public static bool RequestUpdate { get; set; }

        public void InitiateProjectModule()
        {
            _datasource.AllGroups.Clear();

            var group1 = new DataGroup("Project",
                    "Project",
                    "Assets/DarkGray.png");

            _datasource.AllGroups.Add(group1);

            var group2 = new DataGroup("Module",
                    "Module",
                    "Assets/LightGray.png");

            _datasource.AllGroups.Add(group2);
        }

        public IEnumerable<DataItem> SelectedItems()
        {
            ObservableCollection<DataItem> retValue = new ObservableCollection<DataItem>();
            foreach (var datagroup in _datasource.AllGroups)
            {
                var items = datagroup.Items.Where(x => x.Selected);
                foreach (var item in items)
                    retValue.Add(item);
            }
            return retValue;
        }

        public int GetProjectID()
        {
            int retValue = 0;
            try
            {
                var project = _datasource.GetGroup("Project").Items.Where(x => x.Selected).FirstOrDefault();

                if (project != null)
                    retValue = Convert.ToInt32(project.UniqueId);
            }
            catch { }

            return retValue;
        }

        public int GetModuleID()
        {
            int retValue = 0;
            try
            {
                var module = _datasource.GetGroup("Module").Items.Where(x => x.Selected).FirstOrDefault(); ;

                if (module != null)
                    retValue = Convert.ToInt32(module.UniqueId);
            }
            catch { }

            return retValue;
        }

        public List<DataItem> GetAllProject()
        {
            List<DataItem> retValue = new List<DataItem>();
            DataGroup project = _datasource.GetGroup("Project");
            if (project != null)
                retValue = project.Items.ToList();

            return retValue;
        }

        public List<DataItem> GetAllModule()
        {
            List<DataItem> retValue = new List<DataItem>();
            DataGroup module = _datasource.GetGroup("Module");
            if (module != null)
                retValue = module.Items.ToList();

            return retValue;
        }

        public async Task<IEnumerable<DataGroup>> GetProjectModule(WinAppLibrary.UI.LogMode onoff)
        {
            ObservableCollection<DataGroup> retValue = new ObservableCollection<DataGroup>();

            switch (onoff)
            {
                case WinAppLibrary.UI.LogMode.OnMode:
                    retValue = await GetProjectModuleOnMode();
                    break;
                case WinAppLibrary.UI.LogMode.OffMode:
                    retValue = await GetProjectModuleOffMode();
                    break;
            }

            return retValue;
        }

        private async Task<ObservableCollection<DataGroup>> GetProjectModuleOnMode()
        {
            try
            {
                var projects = await (new ServiceModel.ProjectModel()).GetAllProject();
                var modules = await (new ServiceModel.CommonModel()).GetAllModule();
                SetProjectModuleGroup(projects, modules);
            }
            catch(Exception e) 
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "GetProjectModuleOnMode");
                throw e;
            }

            return _datasource.AllGroups;
        }

        private async Task<ObservableCollection<DataGroup>> GetProjectModuleOffMode()
        {
            try
            {
                WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
                var stream = await helper.GetFileStream(ContentPath.OffModeFolder, Lib.ContentPath.ProjectSource);
                var projects = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealProjectSvc.ProjectDTO>>(stream);

                stream = await helper.GetFileStream(Lib.ContentPath.OffModeFolder, Lib.ContentPath.ModuleSource);
                var modules = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealCommonSvc.ModuleDTO>>(stream);

                SetProjectModuleGroup(projects, modules);

                DataGroup datagroup = _datasource.GetGroup("Project");
                string selected = WinAppLibrary.Utilities.Helper.GetValueFromStorage(Lib.HashKey.Key_Project);
                var item = datagroup.Items.Where(x => x.UniqueId == selected).FirstOrDefault();
                if (item != null)
                    item.Selected = true;

                datagroup = _datasource.GetGroup("Module");
                selected = WinAppLibrary.Utilities.Helper.GetValueFromStorage(Lib.HashKey.Key_Module);
                item = datagroup.Items.Where(x => x.UniqueId == selected).FirstOrDefault();
                if (item != null)
                    item.Selected = true;

                //DataGroup group = _datasource.GetGroup("Project");
                //group.Items.Clear();

                //foreach (var item in projects)
                //    group.Items.Add(item);
                //retValue.Add(group);

                //group = _datasource.GetGroup("Module");
                //group.Items.Clear();

                //foreach (var item in modules)
                //    group.Items.Add(item);
                //retValue.Add(group);

            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "GetProjectModuleOffMode");
                throw e;
            }

            return _datasource.AllGroups;
        }

        private void SetProjectModuleGroup(List<RevealProjectSvc.ProjectDTO> projects, List<RevealCommonSvc.ModuleDTO> modules)
        {
            DataGroup group = _datasource.GetGroup("Project");
            IEnumerable<DataItem> items;

            if (group != null)
            {
                group.Items.Clear();

                items = projects.Select(x =>
                                new DataItem(x.ProjectID.ToString(), x.ProjectName, string.Format("Assets/Project/project{0}.png", (x.ProjectID % 6)),
                                    "Job No. : " + x.JobNumber + Environment.NewLine + "Client Name : " +
                                    x.ClientName, group) { Title = x.ProjectName });
                foreach (var item in items)
                    group.Items.Add(item);
            }

            group = _datasource.GetGroup("Module");
            if (group != null)
            {
                group.Items.Clear();

                items = modules.Select(x =>
                            new DataItem(x.ModuleID.ToString(), x.ModuleName, string.Format("Assets/Module/{0}.png", x.ModuleName.ToLower()),
                                "Module Name: " + x.ModuleName, group) { Title = x.ModuleName });
                foreach (var item in items)
                    group.Items.Add(item);
            }
        }

        public async Task<bool> SaveProjectModule(int curproject, int curmodule)
        {
             bool retValue = false;
             WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

             try
             {
                 var projects = this.GetAllProject();
                 var modules = this.GetAllModule();

                 //Save project
                 var xmlstream = helper.EncryptHashSerializeTo<List<DataItem>>(projects);
                 await helper.SaveFileStream(ContentPath.OffModeFolder, Lib.ContentPath.ProjectSource, xmlstream);

                 //Save Module
                 xmlstream = helper.EncryptHashSerializeTo<List<DataItem>>(modules);
                 await helper.SaveFileStream(ContentPath.OffModeFolder, Lib.ContentPath.ModuleSource, xmlstream);

                 WinAppLibrary.Utilities.Helper.SetValueInStorage(Lib.HashKey.Key_Project, curproject);
                 WinAppLibrary.Utilities.Helper.SetValueInStorage(Lib.HashKey.Key_Module, curmodule);
             }
             catch (Exception e)
             {
                 helper.ExceptionHandler(e, "SaveProjectModule");
                 throw e;
             }

             return retValue;
        }

        public async Task<bool> SaveProjectModuleFull(int curproject, int curmodule)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                var projects = await (new ServiceModel.ProjectModel()).GetAllProject();
                var modules = await (new ServiceModel.CommonModel()).GetAllModule();

                //Save project
                var xmlstream = helper.EncryptSerializeTo<List<RevealProjectSvc.ProjectDTO>>(projects);
                await helper.SaveFileStream(ContentPath.OffModeFolder, Lib.ContentPath.ProjectSource, xmlstream);

                //Save Module
                xmlstream = helper.EncryptSerializeTo<List<RevealCommonSvc.ModuleDTO>>(modules);
                await helper.SaveFileStream(ContentPath.OffModeFolder, Lib.ContentPath.ModuleSource, xmlstream);

                WinAppLibrary.Utilities.Helper.SetValueInStorage(Lib.HashKey.Key_Project, curproject);
                WinAppLibrary.Utilities.Helper.SetValueInStorage(Lib.HashKey.Key_Module, curmodule);
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "SaveProjectModuleFull");
                throw e;
            }

            return retValue;
        }
    }
}
