using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace WinAppLibrary.Controls
{
    [TemplatePart(Name = TransformRootName, Type = typeof(Grid))]
    [TemplatePart(Name = PresenterName, Type = typeof(ContentPresenter))]
    public sealed class LayoutTransformer : ContentControl
    {
        #region "Properties"
        /// <summary>
        /// Name of the TransformRoot template part.
        /// </summary>
        private const string TransformRootName = "TransformRoot";

        /// <summary>
        /// Name of the Presenter template part.
        /// </summary>
        private const string PresenterName = "Presenter";

        // Acceptable difference between two doubles.
        private const double AcceptableDelta = 0.0001;

        // Number of decimals to round the Matrix to.
        private const int DecimalsAfterRound = 4;

        // Root element for performing transformations.
        private Panel _transformRoot;

        // ContentPresenter element for displaying the content.
        private ContentPresenter _contentPresenter;
        private MatrixTransform _matrixTransform;
        private Matrix _transformation;
        private Size _childActualSize = Size.Empty;
        #endregion

        #region LayoutTransform "DependencyProperty (1)"

        public Transform LayoutTransform
        {
            get { return (Transform)GetValue(LayoutTransformProperty); }
            set { SetValue(LayoutTransformProperty, value); }
        }

        /// <summary>
        /// Identifies the LayoutTransform DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty LayoutTransformProperty = DependencyProperty.Register(
            "LayoutTransform", typeof(Transform), typeof(LayoutTransformer), new PropertyMetadata(null, LayoutTransformChanged));

        #endregion

        private FrameworkElement Child
        {
            get
            {
                // Preferred child is the content; fall back to the presenter itself
                return (null != _contentPresenter) ?
                    (_contentPresenter.Content as FrameworkElement ?? _contentPresenter) :
                    null;
            }
        }

        public LayoutTransformer()
        {
            // Associated default style
            DefaultStyleKey = typeof(LayoutTransformer);
            IsTabStop = false;

            #if SILVERLIGHT
            // Disable layout rounding because its rounding of values confuses things
            UseLayoutRounding = false;
            #endif
        }

        protected override void OnApplyTemplate()
        {
            // Apply new template
            base.OnApplyTemplate();
            // Find template parts
            _transformRoot = GetTemplateChild(TransformRootName) as Grid;
            _contentPresenter = GetTemplateChild(PresenterName) as ContentPresenter;
            _matrixTransform = new MatrixTransform();
            if (null != _transformRoot)
            {
                _transformRoot.RenderTransform = _matrixTransform;
            }

            // Apply the current transform
            ApplyLayoutTransform();
        }

        private static void LayoutTransformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Casts are safe because Silverlight is enforcing the types
            ((LayoutTransformer)o).ProcessTransform((Transform)e.NewValue);
        }

        public void ApplyLayoutTransform()
        {
            ProcessTransform(LayoutTransform);
        }

        private void ProcessTransform(Transform transform)
        {
            // Get the transform matrix and apply it
            _transformation = RoundMatrix(GetTransformMatrix(transform), DecimalsAfterRound);
            if (null != _matrixTransform)
            {
                _matrixTransform.Matrix = _transformation;
            }
            // New transform means re-layout is necessary
            InvalidateMeasure();
        }

        private Matrix GetTransformMatrix(Transform transform)
        {
            if (null != transform)
            {
                // WPF equivalent of this entire method:
                // return transform.Value;

                // Process the TransformGroup
                TransformGroup transformGroup = transform as TransformGroup;
                if (null != transformGroup)
                {
                    Matrix groupMatrix = Matrix.Identity;
                    foreach (Transform child in transformGroup.Children)
                    {
                        groupMatrix = MatrixMultiply(groupMatrix, GetTransformMatrix(child));
                    }
                    return groupMatrix;
                }

                // Process the RotateTransform
                RotateTransform rotateTransform = transform as RotateTransform;
                if (null != rotateTransform)
                {
                    double angle = rotateTransform.Angle;
                    double angleRadians = (2 * Math.PI * angle) / 360;
                    double sine = Math.Sin(angleRadians);
                    double cosine = Math.Cos(angleRadians);
                    return new Matrix(cosine, sine, -sine, cosine, 0, 0);
                }

                // Process the ScaleTransform
                ScaleTransform scaleTransform = transform as ScaleTransform;
                if (null != scaleTransform)
                {
                    double scaleX = scaleTransform.ScaleX;
                    double scaleY = scaleTransform.ScaleY;
                    return new Matrix(scaleX, 0, 0, scaleY, 0, 0);
                }

                // Process the SkewTransform
                SkewTransform skewTransform = transform as SkewTransform;
                if (null != skewTransform)
                {
                    double angleX = skewTransform.AngleX;
                    double angleY = skewTransform.AngleY;
                    double angleXRadians = (2 * Math.PI * angleX) / 360;
                    double angleYRadians = (2 * Math.PI * angleY) / 360;
                    return new Matrix(1, angleYRadians, angleXRadians, 1, 0, 0);
                }

                // Process the MatrixTransform
                MatrixTransform matrixTransform = transform as MatrixTransform;
                if (null != matrixTransform)
                {
                    return matrixTransform.Matrix;
                }

                // TranslateTransform has no effect in LayoutTransform
            }

            // Fall back to no-op transformation
            return Matrix.Identity;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            FrameworkElement child = Child;
            if ((null == _transformRoot) || (null == child))
            {
                // No content, no size
                return Size.Empty;
            }

            //DiagnosticWriteLine("MeasureOverride < " + availableSize);
            Size measureSize;
            if (_childActualSize == Size.Empty)
            {
                // Determine the largest size after the transformation
                measureSize = ComputeLargestTransformedSize(availableSize);
            }
            else
            {
                measureSize = _childActualSize;
            }

            _transformRoot.Measure(measureSize);
            //DiagnosticWriteLine("  _transformRoot.DesiredSize = " + _transformRoot.DesiredSize);

           
            // Transform DesiredSize to find its width/height
            Rect transformedDesiredRect = RectTransform(new Rect(0, 0, _transformRoot.DesiredSize.Width, _transformRoot.DesiredSize.Height), _transformation);
            Size transformedDesiredSize = new Size(transformedDesiredRect.Width, transformedDesiredRect.Height);

            // Return result to allocate enough space for the transformation
            return transformedDesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            FrameworkElement child = Child;
            if ((null == _transformRoot) || (null == child))
            {
                // No child, use whatever was given
                return finalSize;
            }

            Size finalSizeTransformed = ComputeLargestTransformedSize(finalSize);
            if (IsSizeSmaller(finalSizeTransformed, _transformRoot.DesiredSize))
            {
                finalSizeTransformed = _transformRoot.DesiredSize;
            }

            // Transform the working size to find its width/height
            Rect transformedRect = RectTransform(new Rect(0, 0, finalSizeTransformed.Width, finalSizeTransformed.Height), _transformation);
            // Create the Arrange rect to center the transformed content
            Rect finalRect = new Rect(
                -transformedRect.Left + ((finalSize.Width - transformedRect.Width) / 2),
                -transformedRect.Top + ((finalSize.Height - transformedRect.Height) / 2),
                finalSizeTransformed.Width,
                finalSizeTransformed.Height);

            _transformRoot.Arrange(finalRect);

            // This is the first opportunity under Silverlight to find out the Child's true DesiredSize
            if (IsSizeSmaller(finalSizeTransformed, child.RenderSize) && (Size.Empty == _childActualSize))
            {
                _childActualSize = new Size(child.ActualWidth, child.ActualHeight);
                InvalidateMeasure();
            }
            else
            {
                // Clear the "need to measure/arrange again" flag
                _childActualSize = Size.Empty;
            }
            
            return finalSize;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Closely corresponds to WPF's FrameworkElement.FindMaximalAreaLocalSpaceRect.")]
        private Size ComputeLargestTransformedSize(Size arrangeBounds)
        {
            //DiagnosticWriteLine("  ComputeLargestTransformedSize < " + arrangeBounds);

            // Computed largest transformed size
            Size computedSize = Size.Empty;

            // Detect infinite bounds and constrain the scenario
            bool infiniteWidth = double.IsInfinity(arrangeBounds.Width);
            if (infiniteWidth)
            {
                arrangeBounds.Width = arrangeBounds.Height;
            }
            bool infiniteHeight = double.IsInfinity(arrangeBounds.Height);
            if (infiniteHeight)
            {
                arrangeBounds.Height = arrangeBounds.Width;
            }

            // Capture the matrix parameters
            double a = _transformation.M11;
            double b = _transformation.M12;
            double c = _transformation.M21;
            double d = _transformation.M22;

            double maxWidthFromWidth = Math.Abs(arrangeBounds.Width / a);
            double maxHeightFromWidth = Math.Abs(arrangeBounds.Width / c);
            double maxWidthFromHeight = Math.Abs(arrangeBounds.Height / b);
            double maxHeightFromHeight = Math.Abs(arrangeBounds.Height / d);

            double idealWidthFromWidth = maxWidthFromWidth / 2;
            double idealHeightFromWidth = maxHeightFromWidth / 2;
            double idealWidthFromHeight = maxWidthFromHeight / 2;
            double idealHeightFromHeight = maxHeightFromHeight / 2;

            // Compute slope of both constraint lines
            double slopeFromWidth = -(maxHeightFromWidth / maxWidthFromWidth);
            double slopeFromHeight = -(maxHeightFromHeight / maxWidthFromHeight);

            if ((0 == arrangeBounds.Width) || (0 == arrangeBounds.Height))
            {
                // Check for empty bounds
                computedSize = new Size(arrangeBounds.Width, arrangeBounds.Height);
            }
            else if (infiniteWidth && infiniteHeight)
            {
                // Check for completely unbound scenario
                computedSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            }
            else if (!MatrixHasInverse(_transformation))
            {
                // Check for singular matrix
                computedSize = new Size(0, 0);
            }
            else if ((0 == b) || (0 == c))
            {
                // Check for 0/180 degree special cases
                double maxHeight = (infiniteHeight ? double.PositiveInfinity : maxHeightFromHeight);
                double maxWidth = (infiniteWidth ? double.PositiveInfinity : maxWidthFromWidth);
                if ((0 == b) && (0 == c))
                {
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (0 == b)
                {
                    double computedHeight = Math.Min(idealHeightFromWidth, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs((c * computedHeight) / a),
                        computedHeight);
                }
                else if (0 == c)
                {
                    double computedWidth = Math.Min(idealWidthFromHeight, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs((b * computedWidth) / d));
                }
            }
            else if ((0 == a) || (0 == d))
            {
                double maxWidth = (infiniteHeight ? double.PositiveInfinity : maxWidthFromHeight);
                double maxHeight = (infiniteWidth ? double.PositiveInfinity : maxHeightFromWidth);
                if ((0 == a) && (0 == d))
                {
                    computedSize = new Size(maxWidth, maxHeight);
                }
                else if (0 == a)
                {
                    double computedHeight = Math.Min(idealHeightFromHeight, maxHeight);
                    computedSize = new Size(
                        maxWidth - Math.Abs((d * computedHeight) / b),
                        computedHeight);
                }
                else if (0 == d)
                {
                    double computedWidth = Math.Min(idealWidthFromWidth, maxWidth);
                    computedSize = new Size(
                        computedWidth,
                        maxHeight - Math.Abs((a * computedWidth) / c));
                }
            }
            else if (idealHeightFromWidth <= ((slopeFromHeight * idealWidthFromWidth) + maxHeightFromHeight))
            {
                // Check the width midpoint for viability (by being below the height constraint line)
                computedSize = new Size(idealWidthFromWidth, idealHeightFromWidth);
            }
            else if (idealHeightFromHeight <= ((slopeFromWidth * idealWidthFromHeight) + maxHeightFromWidth))
            {
                // Check the height midpoint for viability (by being below the width constraint line)
                computedSize = new Size(idealWidthFromHeight, idealHeightFromHeight);
            }
            else
            {
                double computedWidth = (maxHeightFromHeight - maxHeightFromWidth) / (slopeFromWidth - slopeFromHeight);
                
                computedSize = new Size(
                    computedWidth,
                    (slopeFromWidth * computedWidth) + maxHeightFromWidth);
            }
           
            //DiagnosticWriteLine("  ComputeLargestTransformedSize > " + computedSize);
            return computedSize;
        }

        private static bool IsSizeSmaller(Size a, Size b)
        {
            return ((a.Width + AcceptableDelta < b.Width) || (a.Height + AcceptableDelta < b.Height));
        }

        private static Matrix RoundMatrix(Matrix matrix, int decimals)
        {
            return new Matrix(
                Math.Round(matrix.M11, decimals),
                Math.Round(matrix.M12, decimals),
                Math.Round(matrix.M21, decimals),
                Math.Round(matrix.M22, decimals),
                matrix.OffsetX,
                matrix.OffsetY);
        }

        private static Rect RectTransform(Rect rect, Matrix matrix)
        {
            Point leftTop = matrix.Transform(new Point(rect.Left, rect.Top));
            Point rightTop = matrix.Transform(new Point(rect.Right, rect.Top));
            Point leftBottom = matrix.Transform(new Point(rect.Left, rect.Bottom));
            Point rightBottom = matrix.Transform(new Point(rect.Right, rect.Bottom));
            double left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            double top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            double right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            double bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));
            Rect rectTransformed = new Rect(left, top, right - left, bottom - top);
            return rectTransformed;
        }

        private static Matrix MatrixMultiply(Matrix matrix1, Matrix matrix2)
        {
            // WPF equivalent of following code:
            // return Matrix.Multiply(matrix1, matrix2);
            return new Matrix(
                (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21),
                (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22),
                (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21),
                (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22),
                ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX,
                ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);
        }

        private static bool MatrixHasInverse(Matrix matrix)
        {
            // WPF equivalent of following code:
            // return matrix.HasInverse;
            return (0 != ((matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21)));
        }
    }
}
