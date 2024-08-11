using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class CursorScrollExecutionPage : Page
    {
        public CursorScrollExecutionViewModel ViewModel { get; }
        public CursorScrollExecutionPage(CursorScrollExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
        }

    }
}
