using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class WindowMoveFlowStepPage : Page,IFlowStepDetailPage
    {
        public IFlowStepDetailVM ViewModel { get; set; }
        public WindowMoveFlowStepPage(WindowMoveFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
