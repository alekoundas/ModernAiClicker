using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class GoToFlowStepPage : INavigableView<GoToFlowStepViewModel>
    {
        public GoToFlowStepViewModel ViewModel { get; }
        public GoToFlowStepPage(GoToFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
