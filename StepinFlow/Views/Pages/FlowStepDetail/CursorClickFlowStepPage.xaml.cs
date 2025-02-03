using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class CursorClickFlowStepPage : INavigableView<CursorClickFlowStepViewModel>
    {
        public CursorClickFlowStepViewModel ViewModel { get; }
        public CursorClickFlowStepPage(CursorClickFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
