using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class WindowResizeExecutionPage : Page, IExecutionPage
    {
        public WindowResizeExecutionViewModel ViewModel { get; set; }
        public WindowResizeExecutionPage()
        {
            DataContext = new WindowResizeExecutionViewModel();
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            DataContext = (WindowResizeExecutionViewModel)executionViewModel;
        }
    }
}
