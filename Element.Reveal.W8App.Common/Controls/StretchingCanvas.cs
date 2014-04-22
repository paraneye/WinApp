using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinAppLibrary.Controls
{
    public class StretchingCanvas : Canvas, INotifyPropertyChanged
    {
        public event EventHandler<object> OnStretchOut;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isloaded = false;
        private bool _isstretchout = false;
        public bool IsStretchOut { get { return _isstretchout; } }
        public string Title { get; set; }
        public int UniqueID { get; set; }

        #region "Private Properties"
        private Windows.UI.Xaml.Media.Animation.Storyboard sbLayoutOn = new Windows.UI.Xaml.Media.Animation.Storyboard();
        private Windows.UI.Xaml.Media.Animation.Storyboard sbLayoutOff = new Windows.UI.Xaml.Media.Animation.Storyboard();
        private const double ANIMATIONSPEED = 0.5;
        #endregion

        public StretchingCanvas()
        {
            this.Loaded += StretchingCanvas_Loaded;
            sbLayoutOff.Completed += sbLayoutOff_Completed;
            sbLayoutOn.Completed += sbLayoutOn_Completed;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }

        #region "Public Method"
        public void Stretch(bool isout)
        {
            if (isout)
            {
                this.Visibility = Windows.UI.Xaml.Visibility.Visible;
                sbLayoutOn.Pause();
                sbLayoutOn.Begin();
            }
            else
            {
                sbLayoutOff.Pause();
                sbLayoutOff.Begin();
            }

            _isstretchout = isout;
        }

        public void OrientationBeforeStretch()
        {
            foreach (FrameworkElement child in this.Children)
                child.Opacity = 0;
        }

        public void UpdateSize()
        {
            if (_isloaded)
                this.UpdateElementLayout();
        }
        #endregion

        #region "Members"

        #region ActiveOpacity (DependencyProperty) (l)

        public double ActiveOpacity
        {
            get { return (double)GetValue(ActiveOpacityProperty); }
            set { SetValue(ActiveOpacityProperty, value); }
        }
        public static readonly DependencyProperty ActiveOpacityProperty =
            DependencyProperty.Register("ActiveOpacity", typeof(double), typeof(StretchingCanvas), new PropertyMetadata(0.6, new PropertyChangedCallback(StretchingCanvas.OnValuesChanged)));

        #endregion

        #region ContentMargin (DependencyProperty) (2)
        public double ContentMargin
            {
            get { return (double)GetValue(ContentMarginProperty); }
            set { SetValue(ContentMarginProperty, value); }
        }

        public static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.Register("ContentMargin", typeof(double), typeof(StretchingCanvas), new PropertyMetadata(10d, new PropertyChangedCallback(StretchingCanvas.OnValuesChanged)));
        #endregion

        #region Orientation (DependencyProperty) (3)
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        public static readonly DependencyProperty OrientationProperty =
           DependencyProperty.Register("Orientation", typeof(Orientation),
           typeof(StretchingCanvas), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(StretchingCanvas.OnValuesChanged)));
        #endregion

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateElementLayout();
        }

        protected void UpdateElementLayout()
        {
            var collection = this.Children.Where(x => (x as FrameworkElement).Visibility == Visibility.Visible);
            if (collection != null && collection.Count() > 0)
            {
                double starty = 0;
                double totalheight = collection.
                    Where(x => (x as FrameworkElement).VerticalAlignment != VerticalAlignment.Stretch).
                    Sum(x => double.IsNaN((x as FrameworkElement).Height) ? 0 : (x as FrameworkElement).Height);

                sbLayoutOff.Stop();
                sbLayoutOn.Stop();
                sbLayoutOff.Children.Clear();
                sbLayoutOn.Children.Clear();

                int count = collection.Where(x => (x as FrameworkElement).VerticalAlignment == VerticalAlignment.Stretch).Count();
                var zeroheight = count > 0 ? Math.Max(0, (this.ActualHeight - totalheight - this.ContentMargin * collection.Count()) / count) : 0;

                double margin = zeroheight == 0 && collection.Count() > 0 ? Math.Max(0, (this.ActualHeight - totalheight) / collection.Count()) : this.ContentMargin;
                foreach (FrameworkElement child in collection)
                {
                    child.Width = this.ActualWidth > this.ContentMargin * 2 ? this.ActualWidth - this.ContentMargin * 2 : this.ActualWidth;
                    if (child.VerticalAlignment == VerticalAlignment.Stretch)
                        child.Height = zeroheight;

                    if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        child.SetValue(Canvas.LeftProperty, this.ContentMargin);
                        sbLayoutOff.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateYAnimation(child, 0, ANIMATIONSPEED / 4));
                        sbLayoutOff.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(child, 0, 0, ANIMATIONSPEED / 4));
                        sbLayoutOn.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateYAnimation(child, starty, ANIMATIONSPEED * 2));
                        sbLayoutOn.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(child, ActiveOpacity, 0, ANIMATIONSPEED * 2));
                    }
                    else
                    {
                        child.SetValue(Canvas.TopProperty, starty);
                        sbLayoutOff.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateXAnimation(child, -this.ActualWidth, ANIMATIONSPEED / 4));
                        sbLayoutOff.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(child, 0, 0, ANIMATIONSPEED / 4));
                        sbLayoutOn.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateXAnimation(child, 0, ANIMATIONSPEED * 2));
                        sbLayoutOn.Children.Add(WinAppLibrary.Utilities.AnimationHelper.CreateOpacityAnimation(child, ActiveOpacity, 0, ANIMATIONSPEED * 2));
                    }

                    starty += double.IsNaN(child.Height) ? 0 : child.Height + margin;
                }

            }
            Stretch(_isstretchout);
        }

        private static void OnValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StretchingCanvas).UpdateElementLayout();
        }
        #endregion

        #region "EventHandler"
        private void StretchingCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _isloaded = true;
            this.UpdateElementLayout();
        }

        private void sbLayoutOff_Completed(object sender, object e)
        {
            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void sbLayoutOn_Completed(object sender, object e)
        {
            if (OnStretchOut != null)
                OnStretchOut(this, this.ActualHeight);
        }
        #endregion
    }
}
