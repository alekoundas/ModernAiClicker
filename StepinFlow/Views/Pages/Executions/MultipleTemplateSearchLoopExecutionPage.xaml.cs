using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class MultipleTemplateSearchLoopExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public MultipleTemplateSearchLoopExecutionPage(MultipleTemplateSearchLoopExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
