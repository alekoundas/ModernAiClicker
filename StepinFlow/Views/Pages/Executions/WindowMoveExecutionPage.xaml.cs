using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class WindowMoveExecutionPage : Page, IExecutionPage
    {
        public WindowMoveExecutionViewModel ViewModel { get; set; }
        public WindowMoveExecutionPage()
        {
            ViewModel = new WindowMoveExecutionViewModel();
            DataContext = this;
            InitializeComponent();
            DataContext = this;
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (WindowMoveExecutionViewModel)executionViewModel;
        }
    }
}
