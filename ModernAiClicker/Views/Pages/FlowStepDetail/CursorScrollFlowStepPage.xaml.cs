using ModernAiClicker.ViewModels.Pages;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
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
