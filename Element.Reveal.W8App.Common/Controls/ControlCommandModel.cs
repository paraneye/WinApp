using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace WinAppLibrary.Controls
{
    public static class ControlCommandModel
    {
        #region Command

        /// <summary>
        /// Command attached property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(Object), typeof(ControlCommandModel), new PropertyMetadata(DependencyProperty.UnsetValue));

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }
        #endregion

        #region RoutedEvent

        /// <summary>
        /// RoutedEvent property
        /// </summary>
        public static readonly DependencyProperty RoutedEventProperty =
            DependencyProperty.RegisterAttached("RoutedEvent", typeof(string), typeof(ControlCommandModel),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnRoutedEventChanged)));

        public static string GetRoutedEvent(DependencyObject d)
        {
            return (string)d.GetValue(RoutedEventProperty);
        }
        public static void SetRoutedEvent(DependencyObject d, string value)
        {
            d.SetValue(RoutedEventProperty, value);
        }

        private static void OnRoutedEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string routedEvent = (string)e.NewValue;

            if (!string.IsNullOrEmpty(routedEvent))
            {
                EventHooker eventHooker = new EventHooker();
                eventHooker.ControlCommandObject = d;
                EventInfo eventInfo = GetEventInfo(d.GetType(), routedEvent);

                if (eventInfo != null)
                {
                    eventInfo.AddEventHandler(d, eventHooker.GetEventHandler(eventInfo));
                    //System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable<RoutedEventArgs>
                    //Info: http://www.microsoft.com/en-us/download/details.aspx?id=29058
                    //var eventpattern = System.Reactive.Linq.Observable.FromEventPattern<RoutedEventArgs>(d, routedEvent);
                }

            }
        }

        /// <summary>
        /// Search the EventInfo from the type and its base types
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        private static EventInfo GetEventInfo(Type type, string eventName)
        {
            EventInfo eventInfo = null;
            eventInfo = type.GetTypeInfo().GetDeclaredEvent(eventName);
            if (eventInfo == null)
            {
                Type baseType = type.GetTypeInfo().BaseType;
                if (baseType != null)
                    return GetEventInfo(type.GetTypeInfo().BaseType, eventName);
                else
                    return eventInfo;
            }
            return eventInfo;
        }
        #endregion

        #region ExceptionRoutedEvent
        /// <summary>
        /// ExceptionRoutedEvent property
        /// </summary>
        public static readonly DependencyProperty ExceptionRoutedEventProperty =
            DependencyProperty.RegisterAttached("ExceptionRoutedEvent", typeof(string), typeof(ControlCommandModel),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnExceptionRoutedEventChanged)));

        public static string GetExceptionRoutedEvent(DependencyObject d)
        {
            return (string)d.GetValue(ExceptionRoutedEventProperty);
        }
        public static void SetExceptionRoutedEvent(DependencyObject d, string value)
        {
            d.SetValue(ExceptionRoutedEventProperty, value);
        }

        private static void OnExceptionRoutedEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string routedEvent = (string)e.NewValue;

            if (!string.IsNullOrEmpty(routedEvent))
            {
                EventHooker eventHooker = new EventHooker();
                eventHooker.ControlCommandObject = d;
                EventInfo eventInfo = GetEventInfo(d.GetType(), routedEvent);

                if (eventInfo != null)
                {
                    eventInfo.AddEventHandler(d, eventHooker.GetEventHandler(eventInfo));
                }

            }
        }
        #endregion
    }

    internal sealed class EventHooker
    {
        public DependencyObject ControlCommandObject { get; set; }

        public Delegate GetEventHandler(EventInfo eventInfo)
        {
            Delegate del = null;
            if (eventInfo == null)
                throw new ArgumentNullException("eventInfo");

            if (eventInfo.EventHandlerType == null)
                throw new ArgumentNullException("eventInfo.EventHandlerType");

            if (del == null)
                del = this.GetType().GetTypeInfo().GetDeclaredMethod("OnEventRaised").CreateDelegate(eventInfo.EventHandlerType, this);

            return del;
        }

        private void OnEventRaised(object sender, object e)
        {
            ICommand command = (ICommand)(sender as DependencyObject).GetValue(ControlCommandModel.CommandProperty);

            if (command != null)
                command.Execute(null);
        }
    }
}
