using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class MultipleTemplateSearchExecutionPage : Page, IExecutionPage, IDetailPage
    {
        public IFlowDetailVM? FlowViewModel { get; set; }
        public IFlowStepDetailVM? FlowStepViewModel { get; set; }
        public IFlowParameterDetailVM? FlowParameterViewModel { get; set; }
        public IExecutionViewModel? ExecutionViewModel { get; set; }


        public IExecutionViewModel ViewModel { get; set; }
        public MultipleTemplateSearchExecutionPage(MultipleTemplateSearchExecutionVM viewModel)
        {
            ViewModel = viewModel;
            ExecutionViewModel = ViewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
