using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WinAppLibrary.ServiceModels
{
    #region "Definition"
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class DataCommon : WinAppLibrary.Controls.BindableBase
    {
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
                    this._image = new BitmapImage(new Uri(Utilities.Helper.BaseUri, this._imagePath));
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

        public void SetImage(BitmapImage source)
        {
            this._image = source;
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

        [DataMember]
        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        [DataMember]
        private DataGroup _group;
        public DataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }

        [DataMember]
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
        private ObservableCollection<DataGroup> _allGroups = new ObservableCollection<DataGroup>();
        public ObservableCollection<DataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public IEnumerable<DataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return this.AllGroups;
        }

        public DataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public DataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
    }
}
