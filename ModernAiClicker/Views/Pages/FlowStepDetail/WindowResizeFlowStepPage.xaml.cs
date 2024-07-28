using ModernAiClicker.ViewModels.Pages;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
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
