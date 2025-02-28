using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class SubFlowStepExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }

        public SubFlowStepExecutionPage(SubFlowStepExecutionVM viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
