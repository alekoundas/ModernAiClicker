using Business.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class LoopExecutionPage : Page, IExecutionPage
    {
        public IExecutionViewModel ViewModel { get; set; }
        public LoopExecutionPage()
        {
            ViewModel = new LoopExecutionVM();
            InitializeComponent();
            DataContext = this;
        }
    }
}
