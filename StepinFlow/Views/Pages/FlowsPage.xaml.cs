using Model.Models;
using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;
using StepinFlow.Views.UserControls;
using static StepinFlow.ViewModels.Pages.FlowsVM;

namespace StepinFlow.Views.Pages
{
    public partial class FlowsPage : INavigableView<FlowsVM>
    {
        public FlowsVM ViewModel { get; }

        public FlowsPage(FlowsVM viewModel, TreeViewUserControl treeViewUserControl)
        {
            viewModel.IsLockedChanged += OnIsLockedChangedEvent;

            viewModel.LoadFlows += InvokeLoadFlowsAction;
            viewModel.ClearCopy += InvokeClearCopyAction;
            viewModel.ExpandAll += InvokeExpandAllAction;
            viewModel.CollapseAll += InvokeCollapseAllAction;

            viewModel.NavigateToFlow += InvokeNavigateToFlowAction;
            viewModel.NavigateToFlowStep += InvokeNavigateToFlowStepAction;
            viewModel.NavigateToFlowParameter += InvokeNavigateToFlowParameterAction;
            viewModel.NavigateToNewFlowStep += InvokeNavigateToNewFlowStepAction;
            viewModel.NavigateToNewFlowParameter += InvokeNavigateToNewFlowParameter;


            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private void OnIsLockedChangedEvent(bool isLocked)
        {
            TreeViewControl.ViewModel.IsLocked = isLocked;     
        }

        private void OnAddFlowStepClick(object sender, FlowStep newFlowStep)
        {
            ViewModel.OnAddFlowStepClick(newFlowStep);
        }
        private void OnAddFlowParameterClick(object sender, FlowParameter newFlowParameter)
        {
            ViewModel.OnAddFlowParameterClick(newFlowParameter);

        }

        private void OnFlowStepClone(object sender, int id)
        {
            ViewModel.OnFlowStepCopy(id);
        }

        private void OnSelectedFlowStepIdChange(object sender, int id)
        {
            ViewModel.OnTreeViewItemFlowStepSelected(id);
        }

        private void OnSelectedFlowIdChange(object sender, int id)
        {
            ViewModel.OnTreeViewItemFlowSelected(id);
        }
        private void OnSelectedFlowParameterIdChange(object sender, int id)
        {
            ViewModel.OnTreeViewItemFlowParameterSelected(id);
        }

        public async Task InvokeLoadFlowsAction(int? id = 0)
        {
            await TreeViewControl.ViewModel.LoadFlows(id);
        }

        public void InvokeClearCopyAction()
        {
            TreeViewControl.ViewModel.ClearCopy();
        }


        public async Task InvokeExpandAllAction()
        {
            await TreeViewControl.ViewModel.ExpandAll();
        }

        public async Task InvokeCollapseAllAction()
        {
            await TreeViewControl.ViewModel.CollapseAll();
        }

        public async Task InvokeNavigateToFlowAction(int id)
        {
            await FrameDetailUserControl.ViewModel.NavigateToFlow(id);
        }

        public async Task InvokeNavigateToFlowStepAction(int id)
        {
            await FrameDetailUserControl.ViewModel.NavigateToFlowStep(id);
        }
        public async Task InvokeNavigateToFlowParameterAction(int id)
        {
            await FrameDetailUserControl.ViewModel.NavigateToFlowParameter(id);
        }

        public void InvokeNavigateToNewFlowStepAction(FlowStep flowStep)
        {
            FrameDetailUserControl.ViewModel.NavigateToNewFlowStep(flowStep);
        }

        public void InvokeNavigateToNewFlowParameter(FlowParameter flowParameter)
        {
            FrameDetailUserControl.ViewModel.NavigateToNewFlowParameter(flowParameter);
        }

    }
}
