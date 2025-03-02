using Model.Models;
using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;
using StepinFlow.Views.UserControls;
using Wpf.Ui.Abstractions.Controls;

namespace StepinFlow.Views.Pages
{
    public partial class SubFlowsPage : INavigableView<SubFlowsVM>
    {
        public SubFlowsVM ViewModel { get; }

        public SubFlowsPage(SubFlowsVM viewModel, TreeViewUserControl treeViewUserControl)
        {
            viewModel.IsLockedChanged += OnIsLockedChangedEvent;

            viewModel.LoadFlowsAndSelectFlow += InvokeLoadFlowsAndSelectFlowAction;
            viewModel.LoadFlowsAndSelectFlowStep += InvokeLoadFlowsAndSelectFlowStepAction;
            viewModel.LoadFlowsAndSelectFlowParameter += InvokeLoadFlowsAndSelectFlowParameterAction;
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

        private void OnSaveFlow(object sender, int id) => ViewModel.OnSaveFlow(id);
        private void OnSaveFlowStep(object sender, int id) => ViewModel.OnSaveFlowStep(id);
        private void OnSaveFlowParameter(object sender, int id) => ViewModel.OnSaveFlowParameter(id);
        private void OnAddFlowStepClick(object sender, FlowStep newFlowStep) => ViewModel.OnAddFlowStepClick(newFlowStep);
        private void OnAddFlowParameterClick(object sender, FlowParameter newFlowParameter) => ViewModel.OnAddFlowParameterClick(newFlowParameter);
        private void OnFlowStepClone(object sender, int id) => ViewModel.OnFlowStepCopy(id);

        private void OnIsLockedChangedEvent(bool isLocked)
        {
            if (TreeViewControl.ViewModel != null)
                TreeViewControl.ViewModel.IsLocked = isLocked;
        }

        private async void OnSelectedFlowStepIdChange(object sender, int id)
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

        public void InvokeClearCopyAction() => TreeViewControl.ViewModel.ClearCopy();
        public async Task InvokeExpandAllAction() => await TreeViewControl.ViewModel.ExpandAll();
        public async Task InvokeCollapseAllAction() => await TreeViewControl.ViewModel.CollapseAll();
        public async Task InvokeLoadFlowsAndSelectFlowAction(int id) => await TreeViewControl.ViewModel.LoadFlowsAndSelectFlow(id);
        public async Task InvokeLoadFlowsAndSelectFlowStepAction(int id) => await TreeViewControl.ViewModel.LoadFlowsAndSelectFlowStep(id);
        public async Task InvokeLoadFlowsAndSelectFlowParameterAction(int id) => await TreeViewControl.ViewModel.LoadFlowsAndSelectFlowParameter(id);
        public async Task InvokeLoadFlowsAction(int flowId = 0, bool isSubFlow = false) => await TreeViewControl.ViewModel.LoadFlows(flowId, isSubFlow);
        public async Task InvokeNavigateToFlowAction(int id) => await FrameDetailUserControl.ViewModel.NavigateToFlow(id);
        public async Task InvokeNavigateToFlowStepAction(int id) => await FrameDetailUserControl.ViewModel.NavigateToFlowStep(id);
        public async Task InvokeNavigateToFlowParameterAction(int id) => await FrameDetailUserControl.ViewModel.NavigateToFlowParameter(id);
        public void InvokeNavigateToNewFlowStepAction(FlowStep flowStep) => FrameDetailUserControl.ViewModel.NavigateToNewFlowStep(flowStep);
        public void InvokeNavigateToNewFlowParameter(FlowParameter flowParameter) => FrameDetailUserControl.ViewModel.NavigateToNewFlowParameter(flowParameter);

    }
}
