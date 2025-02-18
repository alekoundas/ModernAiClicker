using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class CursorScrollExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public CursorScrollExecutionPage()
        {
            ViewModel = new CursorScrollExecutionViewModel();
            InitializeComponent();
            DataContext = this;
        }
    }
}
