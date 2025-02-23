using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class WaitExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public WaitExecutionPage()
        {
            ViewModel = new WaitExecutionVM();
            InitializeComponent();
            DataContext = this;
        }
    }
}
