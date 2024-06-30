using DataAccess.Repository.Interface;
using ModernAiClicker.ViewModels.Pages;
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
