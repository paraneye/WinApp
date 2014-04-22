using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace WinAppLibrary.Controls
{
    sealed class ManagedBlockSizes
    {
        public static Size WideBigBlock = new Size(4, 2);
        public static Size WideSmallBlock = new Size(2, 1);
        public static Size PrimaryBlock = new Size(2, 2);
        public static Size TallBlock = new Size(1, 2);
        public static Size NormalBlock = new Size(1, 1);
    }

    public class RandomSizedItemGrid : GridView
    {
        Dictionary<int, List<Size>> _diclayout = new Dictionary<int, List<Size>>();

        private int _sequence = 0;
        private Random _rand;

        public RandomSizedItemGrid()
        {
            this.DefaultStyleKey = typeof(RandomSizedItemGrid);
            SetLayoutType();
        }

        protected override void OnApplyTemplate()
        {
            
            base.OnApplyTemplate();
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            int index = Math.Min(base.Items.IndexOf(item), _diclayout[_sequence].Count - 1);
            Size blocklayout = ManagedBlockSizes.NormalBlock;

            try
            {
                blocklayout = _diclayout[_sequence][index];
            }
            catch { }
            
            //element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, blocklayout.Width);
            //element.SetValue(VariableSizedWrapGrid.RowSpanProperty, blocklayout.Height);

            VariableSizedWrapGrid.SetRowSpan(element as UIElement, (int)blocklayout.Height);
            VariableSizedWrapGrid.SetColumnSpan(element as UIElement, (int)blocklayout.Width);
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return base.GetContainerForItemOverride();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            return size;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return base.IsItemItsOwnContainerOverride(item);
        }

        private void SetLayoutType()
        {
            List<Size> layoutlist = new List<Size>();
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.WideSmallBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.TallBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            _diclayout.Add(0, layoutlist);

            layoutlist = new List<Size>();
            layoutlist.Add(ManagedBlockSizes.WideSmallBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.TallBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            _diclayout.Add(1, layoutlist);

            layoutlist = new List<Size>();
            layoutlist.Add(ManagedBlockSizes.TallBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.TallBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            layoutlist.Add(ManagedBlockSizes.TallBlock);
            layoutlist.Add(ManagedBlockSizes.NormalBlock);
            _diclayout.Add(2, layoutlist);

            _rand = new Random(DateTime.Now.Millisecond);
            _sequence = _rand.Next(_diclayout.Count - 1);
        }
    }
}
