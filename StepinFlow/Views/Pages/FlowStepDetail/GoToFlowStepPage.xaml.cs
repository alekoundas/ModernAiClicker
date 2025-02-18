using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class GoToFlowStepPage : Page, IFlowStepDetailPage
    {
        public IFlowStepDetailVM ViewModel { get; set; }
        public GoToFlowStepPage(GoToFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
