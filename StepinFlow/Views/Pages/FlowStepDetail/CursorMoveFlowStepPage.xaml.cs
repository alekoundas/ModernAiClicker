using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    
    public partial class CursorMoveFlowStepPage : Page, IPage
    {
        public IFlowStepViewModel ViewModel { get; set; }

        public CursorMoveFlowStepPage(CursorMoveFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            DataContext = this;

        }

    }
}
