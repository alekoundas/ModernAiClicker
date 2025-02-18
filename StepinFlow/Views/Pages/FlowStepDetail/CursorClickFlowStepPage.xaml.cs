using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class CursorClickFlowStepPage : Page, IFlowStepDetailPage
    {
        public IFlowStepDetailVM ViewModel { get; set; }
        public CursorClickFlowStepPage(CursorClickFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
