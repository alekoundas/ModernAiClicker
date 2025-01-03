using Business.Interfaces;
using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace ModernAiClicker.Views.Pages.Executions
{
    public partial class LoopExecutionPage : Page, IExecutionPage
    {
        public LoopExecutionViewModel ViewModel { get; set; }

        public LoopExecutionPage()
        {
            ViewModel = new LoopExecutionViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (LoopExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }
    }
}
