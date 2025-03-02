using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class CursorRelocateFlowStepPage : Page, IFlowStepDetailPage, IDetailPage
    {
        public IFlowDetailVM? FlowViewModel { get; set; }
        public IFlowStepDetailVM? FlowStepViewModel { get; set; }
        public IFlowParameterDetailVM? FlowParameterViewModel { get; set; }
        public IExecutionViewModel? ExecutionViewModel { get; set; }


        public IFlowStepDetailVM ViewModel { get; set; }
        public CursorRelocateFlowStepPage(CursorRelocateFlowStepVM viewModel)
        {
            ViewModel = viewModel;
            FlowStepViewModel = ViewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
