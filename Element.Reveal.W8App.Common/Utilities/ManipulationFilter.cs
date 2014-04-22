using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppLibrary.Utilities
{
    public class ManipulationFilter
    {
        private static float TargetMinSize = 100F;
        private static float TargetMaxSize = 10000F;
        private static float TargetMinInside = 50F;

        //Forces the rotation to be about the center of the manipulation target.
        public static void RotateAboutCenter(object sender, FilterManipulationEventArgs args)
        {
            var inputProcessor = sender as InputProcessor;
            var target = inputProcessor.Target;

            var rect = target.RenderTransform.TransformBounds(
                new Windows.Foundation.Rect(0, 0, target.ActualWidth, target.ActualHeight));

            args.Pivot = new Windows.Foundation.Point
            {
                X = rect.Left + (rect.Width / 2),
                Y = rect.Top + (rect.Height / 2)
            };
        }

        // Forces pixels of the manipulation target to remain inside its container.
        public static void Clamp(object sender, FilterManipulationEventArgs args)
        {
            var inputProcessor = sender as InputProcessor;
            var target = inputProcessor.Target;
            var container = inputProcessor.Reference;

            // Get the bounding box of the manipulation target, expressed in the coordinate system of its container
            var rect = target.RenderTransform.TransformBounds(
                new Windows.Foundation.Rect(0, 0, target.ActualWidth, target.ActualHeight));

            var translate = new Windows.Foundation.Point
            {
                X = args.Delta.Translation.X,
                Y = args.Delta.Translation.Y
            };
            if ((args.Delta.Translation.X > 0 && args.Delta.Translation.X > container.ActualWidth - rect.Left - ManipulationFilter.TargetMinInside) ||
                (args.Delta.Translation.X < 0 && args.Delta.Translation.X < ManipulationFilter.TargetMinInside - rect.Right) ||
                (args.Delta.Translation.Y > 0 && args.Delta.Translation.Y > container.ActualHeight - rect.Top - ManipulationFilter.TargetMinInside) ||
                (args.Delta.Translation.Y < 0 && args.Delta.Translation.Y < ManipulationFilter.TargetMinInside - rect.Bottom))
            {
                translate.X = 0;
                translate.Y = 0;
            }

            float scale = args.Delta.Scale < 1F ?
                (float)System.Math.Max(ManipulationFilter.TargetMinSize / System.Math.Min(rect.Width, rect.Height), args.Delta.Scale) :
                (float)System.Math.Min(ManipulationFilter.TargetMaxSize / System.Math.Max(rect.Width, rect.Height), args.Delta.Scale);

            args.Delta = new Windows.UI.Input.ManipulationDelta
            {
                Expansion = args.Delta.Expansion,
                Rotation = args.Delta.Rotation,
                Scale = scale,
                Translation = translate
            };
        }

        public static void ClampCenterOfMass(object sender, FilterManipulationEventArgs args)
        {
            var inputProcessor = sender as InputProcessor;
            var target = inputProcessor.Target;
            var container = inputProcessor.Reference;

            var rect = target.RenderTransform.TransformBounds(
                new Windows.Foundation.Rect(0, 0, target.ActualWidth, target.ActualHeight));

            var centerOfMass = new Windows.Foundation.Point
            {
                X = rect.Left + (rect.Width / 2),
                Y = rect.Top + (rect.Height / 2)
            };

            var transform = new Windows.UI.Xaml.Media.CompositeTransform
            {
                CenterX = args.Pivot.X,
                CenterY = args.Pivot.Y,
                Rotation = args.Delta.Rotation,
                ScaleX = args.Delta.Scale,
                ScaleY = args.Delta.Scale,
                TranslateX = args.Delta.Translation.X,
                TranslateY = args.Delta.Translation.Y
            };

            var transformedCenterOfMass = transform.TransformPoint(centerOfMass);

            if (transformedCenterOfMass.X < 0 || transformedCenterOfMass.X > container.ActualWidth ||
                transformedCenterOfMass.Y < 0 || transformedCenterOfMass.Y > container.ActualHeight)
            {
                args.Delta = new Windows.UI.Input.ManipulationDelta
                {
                    Rotation = 0F,
                    Scale = 1F,
                    Translation = new Windows.Foundation.Point(0, 0)
                };
            }
        }
    }
}
