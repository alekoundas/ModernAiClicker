using Business.Interfaces;
using StepinFlow.ViewModels.Pages;

namespace StepinFlow.Views.Pages.FlowDetail
{
    public partial class FlowPage : IFlowDetailPage, IDetailPage
    {
        public IFlowDetailVM? FlowViewModel { get; set; }
        public IFlowStepDetailVM? FlowStepViewModel { get; set; }
        public IFlowParameterDetailVM? FlowParameterViewModel { get; set; }
        public IExecutionViewModel? ExecutionViewModel { get; set; }


        public IFlowDetailVM ViewModel { get; set; }
        public FlowPage(FlowVM viewModel)
        {
            ViewModel = viewModel;
            FlowViewModel = ViewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
