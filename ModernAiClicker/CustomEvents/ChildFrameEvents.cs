using System.Windows;
using System.Windows.Controls;

namespace ModernAiClicker.CustomEvents
{
    public class ChildFrameEvents : UserControl
    {
        // This defines the custom event
        public static readonly RoutedEvent MyCustomEvent = EventManager.RegisterRoutedEvent(
            "MyCustom", // Event name
            RoutingStrategy.Bubble, // Bubble means the event will bubble up through the tree
            typeof(RoutedEventHandler), // The event type
            typeof(ChildFrameEvents)); // Belongs to ChildControlBase

        // Allows add and remove of event handlers to handle the custom event
        public event RoutedEventHandler MyCustom
        {
            add { AddHandler(MyCustomEvent, value); }
            remove { RemoveHandler(MyCustomEvent, value); }
        }
    }
}
