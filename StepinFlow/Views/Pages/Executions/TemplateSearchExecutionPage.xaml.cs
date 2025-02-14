using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class TemplateSearchExecutionPage : Page, IExecutionPage
    {
        public TemplateSearchExecutionViewModel ViewModel { get; set; }

        public TemplateSearchExecutionPage(TemplateSearchExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (TemplateSearchExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }
    }
}
