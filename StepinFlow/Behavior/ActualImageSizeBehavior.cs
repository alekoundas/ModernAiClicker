using System.Windows;

namespace StepinFlow.Behavior
{
    public class ActualImageSizeBehavior
    {
        public static readonly DependencyProperty BoundWidthProperty =
              DependencyProperty.RegisterAttached("BoundWidth", typeof(double), typeof(ActualImageSizeBehavior), new PropertyMetadata(0.0));

        public static readonly DependencyProperty BoundHeightProperty =
            DependencyProperty.RegisterAttached("BoundHeight", typeof(double), typeof(ActualImageSizeBehavior), new PropertyMetadata(0.0));

        public static double GetBoundWidth(UIElement element) => (double)element.GetValue(BoundWidthProperty);
        public static void SetBoundWidth(UIElement element, double value) => element.SetValue(BoundWidthProperty, value);

        public static double GetBoundHeight(UIElement element) => (double)element.GetValue(BoundHeightProperty);
        public static void SetBoundHeight(UIElement element, double value) => element.SetValue(BoundHeightProperty, value);

        public static readonly DependencyProperty MonitorSizeProperty =
            DependencyProperty.RegisterAttached("MonitorSize", typeof(bool), typeof(ActualImageSizeBehavior), new PropertyMetadata(false, OnMonitorSizeChanged));

        public static bool GetMonitorSize(UIElement element) => (bool)element.GetValue(MonitorSizeProperty);
        public static void SetMonitorSize(UIElement element, bool value) => element.SetValue(MonitorSizeProperty, value);

        private static void OnMonitorSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.SizeChanged += OnSizeChanged;
                }
                else
                {
                    element.SizeChanged -= OnSizeChanged;
                }
            }
        }

        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                SetBoundWidth(element, e.NewSize.Width);
                SetBoundHeight(element, e.NewSize.Height);
            }
        }
    }
}