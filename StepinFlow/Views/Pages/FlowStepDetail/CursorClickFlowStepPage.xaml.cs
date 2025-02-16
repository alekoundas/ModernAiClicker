using Business.Interfaces;
using StepinFlow.ViewModels.Pages;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class CursorClickFlowStepPage : IFlowStepDetailPage
    {
        public IFlowStepDetailVM ViewModel { get; set; }
        public CursorClickFlowStepPage(CursorClickFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
