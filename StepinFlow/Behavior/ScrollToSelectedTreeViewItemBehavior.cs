using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using TreeViewItem = System.Windows.Controls.TreeViewItem;

namespace StepinFlow.Behavior
{
    public class ScrollToSelectedTreeviewItemBehavior
    {
        public static bool GetIsBroughtIntoViewWhenSelected(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        }

        public static void SetIsBroughtIntoViewWhenSelected(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
         DependencyProperty.RegisterAttached(
         "IsBroughtIntoViewWhenSelected",
         typeof(bool),
         typeof(ScrollToSelectedTreeviewItemBehavior),
         new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

        static void OnIsBroughtIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)depObj;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                item.Selected += OnTreeViewItemSelected;
            else
                item.Selected -= OnTreeViewItemSelected;
        }

        static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            // Only react to the Selected event raised by the TreeViewItem
            // whose IsSelected property was modified. Ignore all ancestors
            // who are merely reporting that a descendant's Selected fired.
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
                return;
            if (e.OriginalSource is TreeViewItem)
            {

                TreeViewItem item = (TreeViewItem)e.OriginalSource;
                if (item != null)
                    item.BringIntoView();
            }
        }
    }
}
