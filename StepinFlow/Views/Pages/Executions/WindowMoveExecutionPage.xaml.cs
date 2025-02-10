using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class WindowMoveExecutionPage : Page, IExecutionPage
    {
        public WindowMoveExecutionPage()
        {
            DataContext = new WindowMoveExecutionViewModel();
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            DataContext = (WindowMoveExecutionViewModel)executionViewModel;
        }
    }
}
