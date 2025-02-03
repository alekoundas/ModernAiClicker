using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class CursorScrollFlowStepPage : Page
    {
        public CursorScrollFlowStepViewModel ViewModel { get; }
        public CursorScrollFlowStepPage(CursorScrollFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }

    }
}
