using Business.Interfaces;
using StepinFlow.ViewModels.Pages.FlowParameterDetail;

namespace StepinFlow.Views.Pages.FlowParameterDetail
{
    public partial class TemplateSearchAreaFlowParameterPage : IFlowParameterDetailPage
    {
        public IFlowParameterDetailVM ViewModel { get; set; }
        public TemplateSearchAreaFlowParameterPage(TemplateSearchAreaFlowParameterVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
