using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class CursorClickExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public CursorClickExecutionPage()
        {
            ViewModel = new CursorClickExecutionVM();
            InitializeComponent();
            DataContext = this;
        }
    }
}
