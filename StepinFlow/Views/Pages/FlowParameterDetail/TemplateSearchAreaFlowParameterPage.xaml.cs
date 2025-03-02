using Business.Interfaces;
using StepinFlow.ViewModels.Pages.FlowParameterDetail;

namespace StepinFlow.Views.Pages.FlowParameterDetail
{
    public partial class TemplateSearchAreaFlowParameterPage : IFlowParameterDetailPage, IDetailPage
    {
        public IFlowDetailVM? FlowViewModel { get; set; }
        public IFlowStepDetailVM? FlowStepViewModel { get; set; }
        public IFlowParameterDetailVM? FlowParameterViewModel { get; set; }
        public IExecutionViewModel? FlowExecutionViewModel { get; set; }


        public IFlowParameterDetailVM ViewModel { get; set; }
        public TemplateSearchAreaFlowParameterPage(TemplateSearchAreaFlowParameterVM viewModel)
        {
            ViewModel = viewModel;
            FlowParameterViewModel = ViewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
