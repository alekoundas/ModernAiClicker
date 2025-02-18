using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class TemplateSearchLoopExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public TemplateSearchLoopExecutionPage(TemplateSearchLoopExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
