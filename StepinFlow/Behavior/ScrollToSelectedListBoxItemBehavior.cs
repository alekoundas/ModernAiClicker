using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace StepinFlow.Behavior
{
    public class ScrollToSelectedListBoxItemBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                ListBox listBox = (ListBox)sender ;
                if (listBox.SelectedItem != null)
                {
                    listBox.Dispatcher.BeginInvoke(() =>
                    {
                        listBox.UpdateLayout();
                        if (listBox.SelectedItem != null)
                            listBox.ScrollIntoView(listBox.SelectedItem);
                    });
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;

        }
    }
}
