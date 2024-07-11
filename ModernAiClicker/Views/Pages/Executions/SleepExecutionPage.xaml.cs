using ModernAiClicker.ViewModels.Pages;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class SleepExecutionPage : INavigableView<SleepExecutionViewModel>
    {
        public SleepExecutionViewModel ViewModel { get; }
        public SleepExecutionPage(SleepExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
