using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Net.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class TemplateSearchLoopExecutionPage : Page, IExecutionPage
    {
        public TemplateSearchLoopExecutionViewModel ViewModel { get; set; }
        public TemplateSearchLoopExecutionPage(TemplateSearchLoopExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (TemplateSearchLoopExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }
    }
}
