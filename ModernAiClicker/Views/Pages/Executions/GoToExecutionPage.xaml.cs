using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class GoToExecutionPage : Page
    {
        public GoToExecutionViewModel ViewModel { get; }
        public GoToExecutionPage(GoToExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
