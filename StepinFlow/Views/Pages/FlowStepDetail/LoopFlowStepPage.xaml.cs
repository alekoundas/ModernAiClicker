using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class LoopFlowStepPage : Page
    {
        public LoopFlowStepViewModel ViewModel { get; }

        public LoopFlowStepPage(LoopFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
