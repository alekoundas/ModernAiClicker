using Business.Interfaces;
using StepinFlow.ViewModels.Pages;

namespace StepinFlow.Views.Pages.FlowDetail
{
    public partial class FlowPage : IFlowDetailPage
    {
        public IFlowDetailVM ViewModel { get; set; }
        public FlowPage(FlowVM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
