using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppLibrary.Utilities
{
    #region FilterManipulation
    public class FilterManipulationDelta
    {
        internal FilterManipulationDelta(Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs args)
        {
            Delta = args.Delta;
            Pivot = args.Position;
            Cumulative = args.Cumulative;
            Velocities = args.Velocities;
        }

        internal FilterManipulationDelta(Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs args)
        {
            Pivot = args.Position;
            Cumulative = args.Cumulative;
            Velocities = args.Velocities;
        }

        public Windows.UI.Input.ManipulationDelta Delta
        {
            get;
            set;
        }

        public Windows.Foundation.Point Pivot
        {
            get;
            set;
        }

        public Windows.UI.Input.ManipulationDelta Cumulative
        {
            get;
            set;
        }

        public Windows.UI.Input.ManipulationVelocities Velocities
        {
            get;
            set;
        }
    }

    #endregion

    #region FilterManipulation

    public delegate void FilterManipulation(object sender, FilterManipulationEventArgs args);

    public class FilterManipulationEventArgs
    {
        internal FilterManipulationEventArgs(Windows.UI.Input.ManipulationUpdatedEventArgs args)
        {
            Delta = args.Delta;
            Pivot = args.Position;
        }

        public Windows.UI.Input.ManipulationDelta Delta
        {
            get;
            set;
        }

        public Windows.Foundation.Point Pivot
        {
            get;
            set;
        }
    }

    #endregion

    public class ManipulationManager : InputProcessor
    {
        private bool _handlersRegistered;

        private Windows.UI.Xaml.Media.MatrixTransform _initialTransform;
        private Windows.UI.Xaml.Media.MatrixTransform _previousTransform;
        private Windows.UI.Xaml.Media.CompositeTransform _deltaTransform;
        private Windows.UI.Xaml.Media.TransformGroup _transform;

        /// <summary>
        /// Gets or sets the filter that is applied to each manipulation update.
        /// </summary>
        public FilterManipulation OnFilterManipulation
        {
            get;
            set;
        }

        public ManipulationManager(Windows.UI.Xaml.FrameworkElement element, Windows.UI.Xaml.Controls.Canvas parent)
            : base(element, parent)
        {
            this._handlersRegistered = false;
            this.InitialTransform = this._target.RenderTransform;
            this.ResetManipulation();
        }

        public Windows.UI.Xaml.Media.Transform InitialTransform
        {
            get { return this._initialTransform; }
            set
            {
                this._initialTransform = new Windows.UI.Xaml.Media.MatrixTransform()
                {
                    Matrix = new Windows.UI.Xaml.Media.TransformGroup()
                    {
                        Children = { value }
                    }.Value
                };
            }
        }

        /// <summary>
        /// Configures the manipulations that are enabled.
        /// </summary>
        public void Configure(bool scale, bool rotate, bool translate, bool inertia)
        {
            var settings = new Windows.UI.Input.GestureSettings();

            if (scale)
            {
                settings |= Windows.UI.Input.GestureSettings.ManipulationScale;
                if (inertia)
                {
                    settings |= Windows.UI.Input.GestureSettings.ManipulationScaleInertia;
                }
            }
            if (rotate)
            {
                settings |= Windows.UI.Input.GestureSettings.ManipulationRotate;
                if (inertia)
                {
                    settings |= Windows.UI.Input.GestureSettings.ManipulationRotateInertia;
                }
            }
            if (translate)
            {
                settings |= Windows.UI.Input.GestureSettings.ManipulationTranslateX |
                    Windows.UI.Input.GestureSettings.ManipulationTranslateY;
                if (inertia)
                {
                    settings |= Windows.UI.Input.GestureSettings.ManipulationTranslateInertia;
                }
            }
            this._gestureRecognizer.GestureSettings = settings;

            this.ConfigureHandlers(scale || rotate || translate);
        }

        public void ResetManipulation()
        {
            // Reset previous transform to the initial transform of the element
            this._previousTransform = new Windows.UI.Xaml.Media.MatrixTransform()
            {
                Matrix = this._initialTransform.Matrix
            };

            this._deltaTransform = new Windows.UI.Xaml.Media.CompositeTransform();

            this._transform = new Windows.UI.Xaml.Media.TransformGroup()
            {
                Children = { this._previousTransform, this._deltaTransform }
            };

            // Set the element's transform
            this._target.RenderTransform = this._transform;
        }

        public void SetElementScale(double scalex, double scaley)
        {
            this._deltaTransform.ScaleX = scalex;
            this._deltaTransform.ScaleY = scaley;
        }

        private void OnManipulationUpdated(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.ManipulationUpdatedEventArgs args)
        {
            var filteredArgs = new FilterManipulationEventArgs(args);
            if (OnFilterManipulation != null)
            {
                OnFilterManipulation(this, filteredArgs);
            }

            // Update the transform            
            this._previousTransform.Matrix = _transform.Value;
            this._deltaTransform.CenterX = filteredArgs.Pivot.X;
            this._deltaTransform.CenterY = filteredArgs.Pivot.Y;
            this._deltaTransform.Rotation = filteredArgs.Delta.Rotation;
            this._deltaTransform.ScaleX = _deltaTransform.ScaleY = filteredArgs.Delta.Scale;
            this._deltaTransform.TranslateX = filteredArgs.Delta.Translation.X;
            this._deltaTransform.TranslateY = filteredArgs.Delta.Translation.Y;
        }

        private void ConfigureHandlers(bool register)
        {
            if (register && !_handlersRegistered)
            {
                this._gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;

                this._handlersRegistered = true;
            }
            else if (!register && _handlersRegistered)
            {
                this._gestureRecognizer.ManipulationUpdated -= OnManipulationUpdated;

                this._handlersRegistered = false;
            }
        }
    }
}
