using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class CursorClickExecutionPage : Page, IExecutionPage
    {
        public CursorClickExecutionViewModel ViewModel { get; set; }
        public CursorClickExecutionPage()
        {
            ViewModel = new CursorClickExecutionViewModel();
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (CursorClickExecutionViewModel)executionViewModel;
            DataContext = this;
        }
    }
}
