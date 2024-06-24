using ModernAiClicker.ViewModels.Pages;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class FlowStepDetailMouseClickPage : INavigableView<FlowStepDetailMouseClickViewModel>
    {
        public FlowStepDetailMouseClickViewModel ViewModel { get; }
        public FlowStepDetailMouseClickPage(FlowStepDetailMouseClickViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
