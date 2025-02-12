using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class CursorClickFlowStepPage : IPage
    {
        public IFlowStepViewModel ViewModel { get; set; }
        public CursorClickFlowStepPage(CursorClickFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;

        }

    }
}
