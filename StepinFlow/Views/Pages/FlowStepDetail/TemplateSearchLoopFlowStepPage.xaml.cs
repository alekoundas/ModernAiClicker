using Business.Interfaces;
using StepinFlow.ViewModels.Pages;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.FlowStepDetail
{
    public partial class TemplateSearchLoopFlowStepPage : Page, IPage
    {
        public IFlowStepViewModel ViewModel { get; set; }

        public TemplateSearchLoopFlowStepPage(TemplateSearchLoopFlowStepViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
