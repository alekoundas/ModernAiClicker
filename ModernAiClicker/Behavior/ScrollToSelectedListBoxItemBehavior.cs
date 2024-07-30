using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModernAiClicker.Behavior
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
                ListBox listBox = (sender as ListBox);
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
