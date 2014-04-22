using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Element.Reveal.TrueVue.Lib.Common
{
    #region "Definition"
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class DataCommon : WinAppLibrary.Controls.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");
        public static Uri BaseUri { get { return _baseUri; } }

        public DataCommon(String uniqueId, String title, String imagePath)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public String ImagePath
        {
            get { return _imagePath; }
        }
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(DataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class DataItem : DataCommon
    {
        public DataItem(String uniqueId, String title, String imagePath, String content, DataGroup group)
            : base(uniqueId, title, imagePath)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private DataGroup _group;
        public DataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }

        private bool _selected = false;
        public bool Selected
        {
            get { return this._selected; }
            set { this.SetProperty(ref this._selected, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class DataGroup : DataCommon
    {
        public DataGroup(String uniqueId, String title, String imagePath)
            : base(uniqueId, title, imagePath)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                    break;
                case NotifyCollectionChangedAction.Move:
                    TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    TopItems.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    break;
            }
        }

        private ObservableCollection<DataItem> _items = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> Items
        {
            get { return this._items; }
            set
            {
                _items.Clear();
                foreach (var item in value)
                    _items.Add(item);
            }
        }

        private ObservableCollection<DataItem> _topItem = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> TopItems
        {
            get {return this._topItem; }
        }
    }
    #endregion

    public sealed class GroupModel
    {
        private static GroupModel _projectmodulsSource = new GroupModel();

        private ObservableCollection<DataGroup> _allGroups = new ObservableCollection<DataGroup>();
        public ObservableCollection<DataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<DataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _projectmodulsSource.AllGroups;
        }

        public static DataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _projectmodulsSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static DataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _projectmodulsSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public async static Task<IEnumerable<DataGroup>> GetProjectGroup()
        {
            ObservableCollection<DataGroup> retValue = new ObservableCollection<DataGroup>();
            try
            {
                var projects = await (new WinAppLibrary.ServiceModels.ProjectModel()).GetAllProject();
                var modules = await (new WinAppLibrary.ServiceModels.CommonModel()).GetAllModule();
                DataGroup project = GetGroup("Project");
                project.Items.Clear();

                var items = projects.Select(x =>
                                new DataItem(x.ProjectID.ToString(), x.ProjectName, string.Format("Assets/Project/project{0}.png", (x.ProjectID % 6)),
                                    "Job No. : " + x.JobNumber + Environment.NewLine + "Client Name : " +
                                    x.ClientName, project) { Title = x.ProjectName });
                foreach (var item in items)
                    project.Items.Add(item);
                retValue.Add(project);

                project = GetGroup("Module");
                project.Items.Clear();

                items = modules.Select(x =>
                            new DataItem(x.ModuleID.ToString(), x.ModuleName, string.Format("Assets/Module/{0}.png", x.ModuleName.ToLower()),
                                "Module Name: " + x.ModuleName, project) { Title = x.ModuleName });
                foreach (var item in items)
                    project.Items.Add(item);
                retValue.Add(project);
            }
            catch { }

            return retValue;
        }

        public static  IEnumerable<DataItem> SelectedItems()
        {
            ObservableCollection<DataItem> retValue = new ObservableCollection<DataItem>();
            foreach (var datagroup in _projectmodulsSource.AllGroups)
            {
                var items = datagroup.Items.Where(x => x.Selected);
                foreach (var item in items)
                    retValue.Add(item);
            }
            return retValue;
        }

        public static int GetProjectID()
        {
            int retValue = 0;
            try
            {
                var project = GetGroup("Project").Items.Where(x => x.Selected).FirstOrDefault();

                if (project != null)
                    retValue = Convert.ToInt32(project.UniqueId);
            }
            catch { }

            return retValue;
        }

        public static int GetModuleID()
        {
            int retValue = 0;
            try
            {
                var module = GetGroup("Module").Items.Where(x => x.Selected).FirstOrDefault(); ;

                if (module != null)
                    retValue = Convert.ToInt32(module.UniqueId);
            }
            catch { }

            return retValue;
        }

        public GroupModel()
        {
            var group1 = new DataGroup("Project",
                    "Project",
                    "Assets/DarkGray.png");

            this.AllGroups.Add(group1);

            var group2 = new DataGroup("Module",
                    "Module",
                    "Assets/LightGray.png");

            this.AllGroups.Add(group2);
        }
    }
}
