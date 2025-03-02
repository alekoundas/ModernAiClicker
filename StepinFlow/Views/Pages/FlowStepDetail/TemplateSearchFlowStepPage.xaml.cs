using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class TemplateSearchFlowStepPage : Page, IFlowStepDetailPage, IDetailPage
    {
        public IFlowDetailVM? FlowViewModel { get; set; }
        public IFlowStepDetailVM? FlowStepViewModel { get; set; }
        public IFlowParameterDetailVM? FlowParameterViewModel { get; set; }
        public IExecutionViewModel? ExecutionViewModel { get; set; }


        public IFlowStepDetailVM ViewModel { get; set; }

        public TemplateSearchFlowStepPage(TemplateSearchFlowStepVM viewModel)
        {
            ViewModel = viewModel;
            FlowStepViewModel = ViewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
