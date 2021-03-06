﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAppLibrary.Utilities
{
    public class InputProcessor
    {
        protected Windows.UI.Input.GestureRecognizer _gestureRecognizer;

        // Element being manipulated
        protected Windows.UI.Xaml.FrameworkElement _target;
        public Windows.UI.Xaml.FrameworkElement Target
        {
            get { return _target; }
        }

        protected Windows.UI.Xaml.Controls.Canvas _reference;
        public Windows.UI.Xaml.FrameworkElement Reference
        {
            get { return _reference; }
        }

        internal InputProcessor(Windows.UI.Xaml.FrameworkElement element, Windows.UI.Xaml.Controls.Canvas reference)
        {
            this._target = element;
            this._reference = reference;

            // Setup pointer event handlers for the element.
            this._target.PointerCanceled += OnPointerCanceled;
            this._target.PointerMoved += OnPointerMoved;
            this._target.PointerPressed += OnPointerPressed;
            this._target.PointerReleased += OnPointerReleased;
            this._target.PointerWheelChanged += OnPointerWheelChanged;

            // Create the gesture recognizer
            this._gestureRecognizer = new Windows.UI.Input.GestureRecognizer();
            this._gestureRecognizer.GestureSettings = Windows.UI.Input.GestureSettings.None;
        }

        #region Pointer event handlers

        private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            // Obtain current point in the coordinate system of the reference element
            Windows.UI.Input.PointerPoint currentPoint = args.GetCurrentPoint(this._reference);
            this._gestureRecognizer.ProcessDownEvent(currentPoint);
            this._target.CapturePointer(args.Pointer);

            args.Handled = true;
        }

        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this._gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints(this._reference));
            args.Handled = true;
        }

        private void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            Windows.UI.Input.PointerPoint currentPoint = args.GetCurrentPoint(this._reference);
            this._gestureRecognizer.ProcessUpEvent(currentPoint);
            this._target.ReleasePointerCapture(args.Pointer);

            // Mark event handled, to prevent execution of default event handlers
            args.Handled = true;
        }

        private void OnPointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            Windows.UI.Input.PointerPoint currentPoint = args.GetCurrentPoint(this._reference);

            bool shift = (args.KeyModifiers & Windows.System.VirtualKeyModifiers.Shift) == Windows.System.VirtualKeyModifiers.Shift;
            bool ctrl = (args.KeyModifiers & Windows.System.VirtualKeyModifiers.Control) == Windows.System.VirtualKeyModifiers.Control;

            // Route the event to the gesture recognizer
            this._gestureRecognizer.ProcessMouseWheelEvent(currentPoint, shift, ctrl);
            args.Handled = true;
        }

        private void OnPointerCanceled(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this._gestureRecognizer.CompleteGesture();
            this._target.ReleasePointerCapture(args.Pointer);

            args.Handled = true;
        }

        #endregion Pointer event handlers
    }
}
