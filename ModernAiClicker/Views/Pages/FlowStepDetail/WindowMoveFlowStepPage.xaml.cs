using ModernAiClicker.ViewModels.Pages;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class WindowMoveFlowStepPage : INavigableView<WindowMoveFlowStepViewModel>
    {
        public WindowMoveFlowStepViewModel ViewModel { get; }
        public WindowMoveFlowStepPage(WindowMoveFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
