using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class WindowMoveFlowStepPage : INavigableView<WindowMoveFlowStepViewModel>
    {
        public WindowMoveFlowStepViewModel ViewModel { get; }
        public WindowMoveFlowStepPage(WindowMoveFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
