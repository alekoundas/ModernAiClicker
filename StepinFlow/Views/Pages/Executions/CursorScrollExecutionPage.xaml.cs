using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class CursorScrollExecutionPage : Page, IExecutionPage
    {
        public CursorScrollExecutionViewModel ViewModel { get; set; }
        public CursorScrollExecutionPage()
        {
            ViewModel = new CursorScrollExecutionViewModel();
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (CursorScrollExecutionViewModel)executionViewModel;
            DataContext = ViewModel;
        }

    }
}
