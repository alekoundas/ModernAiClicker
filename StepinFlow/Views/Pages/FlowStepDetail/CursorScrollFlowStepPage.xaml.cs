using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class CursorScrollFlowStepPage : Page, IPage
    {
        public IFlowStepViewModel ViewModel { get; set; }
        public CursorScrollFlowStepPage(CursorScrollFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }

    }
}
