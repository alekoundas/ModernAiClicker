using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class SleepFlowStepPage : INavigableView<SleepFlowStepViewModel>
    {
        public SleepFlowStepViewModel ViewModel { get; }
        public SleepFlowStepPage(SleepFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
