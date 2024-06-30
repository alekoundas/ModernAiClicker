using ModernAiClicker.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class FlowStepDetailGoToPage : INavigableView<FlowStepDetailGoToViewModel>
    {
        public FlowStepDetailGoToViewModel ViewModel { get; }
        public FlowStepDetailGoToPage(FlowStepDetailGoToViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
