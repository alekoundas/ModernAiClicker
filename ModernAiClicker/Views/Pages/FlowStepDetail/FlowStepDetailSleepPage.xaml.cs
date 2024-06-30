using ModernAiClicker.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class FlowStepDetailSleepPage : INavigableView<FlowStepDetailSleepViewModel>
    {
        public FlowStepDetailSleepViewModel ViewModel { get; }
        public FlowStepDetailSleepPage(FlowStepDetailSleepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
