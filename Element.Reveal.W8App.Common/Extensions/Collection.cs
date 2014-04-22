using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppLibrary.Extensions
{
    public static class CollectionExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            if (enumerableList != null)
            {
                // Create an emtpy observable collection object
                var observableCollection = new ObservableCollection<T>();

                // Loop through all the records and add to observable collection object
                foreach (var item in enumerableList)
                {
                    observableCollection.Add(item);
                }

                // Return the populated observable collection
                return observableCollection;
            }
            return null;
        }
    }

    public class SortedObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Func<T, int> func_int;
        private readonly Func<T, string> func_string;

        public SortedObservableCollection(Func<T, int> func)
        {
            this.func_int = func;
        }

        public SortedObservableCollection(Func<T, int> func, IEnumerable<T> collection) :
            base(collection)
        {
            this.func_int = func;
        }

        public SortedObservableCollection(Func<T, int> func, List<T> list) :
            base(list)
        {
            this.func_int = func;
        }

        public SortedObservableCollection(Func<T, string> func)
        {
            this.func_string = func;
        }

        public SortedObservableCollection(Func<T, string> func, IEnumerable<T> collection) :
            base(collection)
        {
            this.func_string = func;
        }

        public SortedObservableCollection(Func<T, string> func, List<T> list) :
            base(list)
        {
            this.func_string = func;
        }

        protected override void InsertItem(int index, T item)
        {
            bool added = false;
            if (func_int != null)
            {
                for (int idx = 0; idx < Count; idx++)
                {
                    if (func_int(item) < func_int(Items[idx]))
                    {
                        base.InsertItem(idx, item); added = true; break;
                    }
                }

            }
            else
            {
                for (int idx = 0; idx < Count; idx++)
                {
                    if (string.Compare(func_string(item), func_string(Items[idx])) < 0)
                    {
                        base.InsertItem(idx, item); added = true; break;
                    }
                }
            }

            if (!added)
            {
                base.InsertItem(index, item);
            }
        }
    }
}
