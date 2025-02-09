using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class SleepExecutionPage : Page, IExecutionPage
    {
        public SleepExecutionViewModel ViewModel { get; set; }
        public SleepExecutionPage()
        {
            ViewModel = new SleepExecutionViewModel();
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (SleepExecutionViewModel)executionViewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
