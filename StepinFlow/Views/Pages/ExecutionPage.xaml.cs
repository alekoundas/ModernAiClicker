using Model.Models;
using StepinFlow.ViewModels.Pages;
using StepinFlow.Views.UserControls;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages
{
    public partial class ExecutionPage : INavigableView<ExecutionVM>
    {
        public ExecutionVM ViewModel { get; }
        public ExecutionPage(ExecutionVM viewModel)
        {
            viewModel.LoadFlows += InvokeLoadFlowsAction;
            viewModel.ExpandAndSelectFlowStep += InvokeExpandAndSelectFlowStepAction;
            viewModel.NavigateToExecution += InvokeNavigateToExecutionAction;
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }


        public async Task InvokeLoadFlowsAction(int? id = 0)
        {
            await TreeViewControl.ViewModel.LoadFlows(id);
        }
        public void InvokeNavigateToExecutionAction(Execution execution)
        {
            FrameDetailUserControl.ViewModel.NavigateToExecution(execution);
        }

        public async Task InvokeExpandAndSelectFlowStepAction(int id)
        {
            await TreeViewControl.ViewModel.ExpandAndSelectFlowStep(id);
        }
    }
}
