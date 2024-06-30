using ModernAiClicker.ViewModels.Pages;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class CursorClickFlowStepPage : INavigableView<CursorClickFlowStepViewModel>
    {
        public CursorClickFlowStepViewModel ViewModel { get; }
        public CursorClickFlowStepPage(CursorClickFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
