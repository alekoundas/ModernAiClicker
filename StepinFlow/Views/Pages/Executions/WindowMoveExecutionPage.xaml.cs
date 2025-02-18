using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class WindowMoveExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public WindowMoveExecutionPage(WindowMoveExecutionViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
