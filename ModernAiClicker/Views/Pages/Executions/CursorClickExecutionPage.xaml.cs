using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class CursorClickExecutionPage : Page
    {
        public CursorClickExecutionViewModel ViewModel { get; }
        public CursorClickExecutionPage(CursorClickExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
        }

    }
}
