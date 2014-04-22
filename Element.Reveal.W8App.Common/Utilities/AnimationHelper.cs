using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace WinAppLibrary.Utilities
{
    public enum AnimationFlow
    {
        None = 0,
        ForwardToBack = 1,
        BackToForward = 2,
        ToNext = 3,
        ToPrevious = 4
    }

    public class AnimationHelper
    {
        public const double ScaleThreshold = 0.5;
        public const double TransThreshold = 80.0;
        public const double SecondsThreshold = 0.8;
        public const double VelocityThreshold = 0.6;
        public const double ANIMATION_TIMEs = 0.5;
        /// <summary>
        /// Helper to create double animation</summary>
        /// 
        public static DoubleAnimation CreateDoubleAnimation(DependencyObject element, string property,
            double from, double to, int milliseconds)
        {
            DoubleAnimation da;

            da = new DoubleAnimation();
            da.To = to;
            da.From = from;

            da.Duration = new Duration(new TimeSpan(0, 0, 0, 0, milliseconds));

            Storyboard.SetTargetProperty(da, property);
            Storyboard.SetTarget(da, element);
            return da;
        }

        /// <summary>
        /// Helper to create double animation</summary>
        /// 
        public static DoubleAnimation CreateDoubleAnimation(DependencyObject element, string property,
            double from, double to, double seconds)
        {
            DoubleAnimation da;

            da = new DoubleAnimation();
            da.To = to;
            da.From = from;

            da.Duration = new Duration(TimeSpan.FromSeconds(seconds));

            Storyboard.SetTargetProperty(da, property);
            Storyboard.SetTarget(da, element);
            return da;
        }

        public static DoubleAnimation CreateScaleXAnimation(ScaleTransform scale, double to, double seconds)
        {
            var scallingXAnimation = new DoubleAnimation
            {
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                EasingFunction = new QuinticEase()

            };
            Storyboard.SetTarget(scallingXAnimation, scale);
            Storyboard.SetTargetProperty(scallingXAnimation, "ScaleX");

            return scallingXAnimation;
        }

        public static DoubleAnimation CreateScaleYAnimation(ScaleTransform scale, double to, double seconds)
        {
            var scallingYAnimation = new DoubleAnimation
            {
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                EasingFunction = new QuinticEase()
            };
            Storyboard.SetTarget(scallingYAnimation, scale);
            Storyboard.SetTargetProperty(scallingYAnimation, "ScaleY");

            return scallingYAnimation;
        }

        public static DoubleAnimation CreateTranslateXAnimation(TranslateTransform translate, double to, double seconds)
        {
            var translateXAnimation = new DoubleAnimation
            {
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };
            Storyboard.SetTarget(translateXAnimation, translate);
            Storyboard.SetTargetProperty(translateXAnimation, "X");

            return translateXAnimation;
        }

        public static DoubleAnimation CreateXAnimation(UIElement element, double to, double secDuration)
        {
            var doubAni = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(secDuration)),
                To = to,
                EasingFunction = new QuinticEase()
            };

            Storyboard.SetTarget(doubAni, element);
            Storyboard.SetTargetProperty(doubAni, "(Canvas.Left)");
            return doubAni;
        }

        public static DoubleAnimationUsingKeyFrames CreateElasticXAnimation(UIElement element, double to, double seconds)
        {
            var elasticAni = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };

            var easing = new EasingDoubleKeyFrame
            {
                Value = to,
                EasingFunction = new ElasticEase
                {
                    EasingMode = EasingMode.EaseInOut,
                    Oscillations = 3,
                    Springiness = 8,
                }
            };

            easing.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            elasticAni.KeyFrames.Add(easing);
            Storyboard.SetTargetProperty(elasticAni, @"(Canvas.Left)");
            Storyboard.SetTarget(elasticAni, element);
            return elasticAni;
        }

        public static DoubleAnimationUsingKeyFrames CreateElasticXAnimation(TranslateTransform translate, double to, double seconds)
        {
            var elasticAni = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };

            var easing = new EasingDoubleKeyFrame
            {
                Value = to,
                EasingFunction = new ElasticEase
                {
                    EasingMode = EasingMode.EaseInOut,
                    Oscillations = 3,
                    Springiness = 8,
                }
            };

            easing.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            elasticAni.KeyFrames.Add(easing);
            Storyboard.SetTargetProperty(elasticAni, "X");
            Storyboard.SetTarget(elasticAni, translate);
            return elasticAni;
        }

        public static DoubleAnimation CreateTranslateYAnimation(TranslateTransform translate, double to, double seconds)
        {
            var translateYAnimation = new DoubleAnimation
            {
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };
            Storyboard.SetTarget(translateYAnimation, translate);
            Storyboard.SetTargetProperty(translateYAnimation, "Y");

            return translateYAnimation;
        }

        public static DoubleAnimationUsingKeyFrames CreateElasticYAnimation(UIElement element, double to, double seconds)
        {
            var elasticAni = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };

            var easing = new EasingDoubleKeyFrame
            {
                Value = to,
                EasingFunction = new ElasticEase
                {
                    EasingMode = EasingMode.EaseInOut,
                    Oscillations = 3,
                    Springiness = 8,
                }
            };

            easing.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            elasticAni.KeyFrames.Add(easing);
            Storyboard.SetTargetProperty(elasticAni, @"(Canvas.Top)");
            Storyboard.SetTarget(elasticAni, element);
            return elasticAni;
        }

        public static DoubleAnimationUsingKeyFrames CreateElasticYAnimation(TranslateTransform translate, double to, double seconds)
        {
            var elasticAni = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };

            var easing = new EasingDoubleKeyFrame
            {
                Value = to,
                EasingFunction = new ElasticEase
                {
                    EasingMode = EasingMode.EaseInOut,
                    Oscillations = 3,
                    Springiness = 8,
                }
            };

            easing.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            elasticAni.KeyFrames.Add(easing);
            Storyboard.SetTargetProperty(elasticAni, "Y");
            Storyboard.SetTarget(elasticAni, translate);
            return elasticAni;
        }

        public static DoubleAnimation CreateYAnimation(UIElement element, double to, double secDuration)
        {
            var doubAni = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(secDuration)),
                To = to,
                EasingFunction = new QuinticEase()
            };

            Storyboard.SetTarget(doubAni, element);
            Storyboard.SetTargetProperty(doubAni, "(Canvas.Top)");
            return doubAni;
        }

        public static DoubleAnimation CreateWidthAnimation(UIElement element, double to, double secDuration, EasingFunctionBase func)
        {
            var doubAni = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(secDuration)),
                To = to,
                EasingFunction = func
            };

            doubAni.EasingFunction.EasingMode = EasingMode.EaseOut;
            Storyboard.SetTarget(doubAni, element);
            Storyboard.SetTargetProperty(doubAni, "Width");
            return doubAni;
        }

        public static DoubleAnimation CreateHeightAnimation(UIElement element, double to, double secDuration, EasingFunctionBase func)
        {
            var doubAni = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(secDuration)),
                To = to,
                EasingFunction = func
            };

            doubAni.EasingFunction.EasingMode = EasingMode.EaseOut;
            doubAni.EnableDependentAnimation = true;
            Storyboard.SetTarget(doubAni, element);
            Storyboard.SetTargetProperty(doubAni, "Height");
            return doubAni;
        }

        public static DoubleAnimation CreateOpacityAnimation(UIElement element, double to, double beginTime, double duration)
        {
            var opacityAnimation = new DoubleAnimation
            {
                BeginTime = TimeSpan.FromSeconds(beginTime),
                Duration = new Duration(TimeSpan.FromSeconds(duration)),
                To = to,
            };

            Storyboard.SetTarget(opacityAnimation, element);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            return opacityAnimation;
        }

        public static DoubleAnimation CreateRotationAnimation(RotateTransform rotate, double angle, double seconds)
        {
            var rotateAnimation = new DoubleAnimation
            {
                To = angle,
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(rotateAnimation, rotate);
            Storyboard.SetTargetProperty(rotateAnimation, "Angle");
            return rotateAnimation;
        }

        public static DoubleAnimation CreateProjectionXAnimation(PlaneProjection projection, double to, double seconds)
        {
            return CreateProjectionAnimation(projection, to, seconds, "RotationX");
        }

        public static DoubleAnimation CreateProjectionYAnimation(PlaneProjection projection, double to, double seconds)
        {
            return CreateProjectionAnimation(projection, to, seconds, "RotationY");
        }

        public static DoubleAnimation CreateProjectionZAnimation(PlaneProjection projection, double to, double seconds)
        {
            return CreateProjectionAnimation(projection, to, seconds, "RotationZ");
        }

        private static DoubleAnimation CreateProjectionAnimation(PlaneProjection projection, double to, double seconds,
                                                          string projectionRotation)
        {
            var projectionAnimation = new DoubleAnimation
            {
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
            };
            Storyboard.SetTarget(projectionAnimation, projection);
            Storyboard.SetTargetProperty(projectionAnimation, projectionRotation);

            return projectionAnimation;
        }
    }
}
