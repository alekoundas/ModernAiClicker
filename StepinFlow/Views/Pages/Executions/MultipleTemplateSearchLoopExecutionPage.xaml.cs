using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class MultipleTemplateSearchLoopExecutionPage : Page, IExecutionPage
    {
        public MultipleTemplateSearchLoopExecutionViewModel ViewModel { get; set; }

        public MultipleTemplateSearchLoopExecutionPage(MultipleTemplateSearchLoopExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (MultipleTemplateSearchLoopExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }
    }
}
