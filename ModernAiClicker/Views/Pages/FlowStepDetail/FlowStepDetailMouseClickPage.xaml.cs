using ModernAiClicker.ViewModels.Pages;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernAiClicker.Views.Pages.FlowStepDetail
{
    public partial class FlowStepDetailMouseClickPage : Page
    {
        public FlowStepDetailMouseClickViewModel ViewModel { get; }
        public FlowStepDetailMouseClickPage(FlowStepDetailMouseClickViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

        }

    }
}
