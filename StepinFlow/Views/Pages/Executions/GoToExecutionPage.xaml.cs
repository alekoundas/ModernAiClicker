using Business.Interfaces;
using DataAccess.Repository.Interface;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class GoToExecutionPage : Page, IExecutionPage
    {
        public GoToExecutionViewModel ViewModel { get; set; }
        public GoToExecutionPage(IBaseDatawork baseDatawork)
        {
            ViewModel = new GoToExecutionViewModel(baseDatawork);
            DataContext = this;
            InitializeComponent();
        }

        public void SetViewModel(IExecutionViewModel executionViewModel)
        {
            ViewModel = (GoToExecutionViewModel)executionViewModel;
            DataContext = this;
        }
    }
}
