using ModernAiClicker.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class FrameExecutionFlowPage : Page
    {
        public FrameExecutionFlowViewModel ViewModel { get; }

        public FrameExecutionFlowPage(FrameExecutionFlowViewModel viewModel)
        {

            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
