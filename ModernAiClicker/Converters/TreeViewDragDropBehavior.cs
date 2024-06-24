using Microsoft.Xaml.Behaviors;
using Model.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernAiClicker.Converters
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

    public class TreeViewDragDropBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            //AssociatedObject.PreviewMouseLeftButtonDown += tv_PreviewMouseLeftButtonDown;
            //AssociatedObject.MouseMove += tv_MouseMove;
            //AssociatedObject.DragOver += tv_DragOver;
            //AssociatedObject.Drop += Tv_Drop;
            //AssociatedObject.DragLeave += tv_DragLeave;
        }

        // for saving TreeViewItem to drag
        TreeViewItem draggedTVI = null;

        // save TreeViewItem to drag
        private void tv_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedTVI = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);
        }

        // start Drag&Drop when mouse is moved and there's a saved TreeViewItem
        private void tv_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedTVI != null)
            {
                // Find the data behind the TreeViewItem
                var dragData = draggedTVI.DataContext;
                // Initialize the drag & drop operation
                DragDrop.DoDragDrop(draggedTVI, dragData, DragDropEffects.Move);
                // reset saved TreeViewItem
                draggedTVI = null;
            }
        }

        // highlight target
        private void tv_DragOver(object sender, DragEventArgs e)
        {
            TreeViewItem tvi = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (tvi != null) tvi.Background = Brushes.LightBlue;
        }

        // 
        private void Tv_Drop(object sender, DragEventArgs e)
        {
            // if no data to drop return
            if (e.Data.GetDataPresent(typeof(FlowStep)))
            {

                // store the drop target
                FlowStep flowStepDrop = (e.OriginalSource as TextBlock)?.DataContext as FlowStep;
                if (flowStepDrop == null)
                    return;

                FlowStep flowStep = (FlowStep)e.Data.GetData(typeof(FlowStep));
                flowStepDrop.ChildrenFlowSteps.Add(flowStep);

            // change parent collection
            flowStep.ParentFlowStep = flowStepDrop;
            }

            //// reset background on target TreeViewItem
            TreeViewItem tvi = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (tvi != null) tvi.Background = Brushes.White;
        }

        // reset background on leaved possible target TreeViewItem
        private void tv_DragLeave(object sender, DragEventArgs e)
        {
            TreeViewItem tvi = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (tvi != null) tvi.Background = Brushes.White;
        }

        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T) return (T)current;
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);
            return null;
        }
    }
}