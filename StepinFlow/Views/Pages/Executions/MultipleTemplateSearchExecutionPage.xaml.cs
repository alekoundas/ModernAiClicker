using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class MultipleTemplateSearchExecutionPage : Page, IExecutionPage
    {
        public MultipleTemplateSearchExecutionViewModel ViewModel { get; set; }

        public MultipleTemplateSearchExecutionPage(MultipleTemplateSearchExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (MultipleTemplateSearchExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }
    }
}
