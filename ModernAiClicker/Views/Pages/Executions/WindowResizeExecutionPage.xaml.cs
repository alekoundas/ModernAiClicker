using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class WindowResizeExecutionPage : Page
    {
        public WindowResizeExecutionViewModel ViewModel { get; }
        public WindowResizeExecutionPage(WindowResizeExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
        }

    }
}
