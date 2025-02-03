using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class WindowResizeFlowStepPage : INavigableView<WindowResizeFlowStepViewModel>
    {
        public WindowResizeFlowStepViewModel ViewModel { get; }
        public WindowResizeFlowStepPage(WindowResizeFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
