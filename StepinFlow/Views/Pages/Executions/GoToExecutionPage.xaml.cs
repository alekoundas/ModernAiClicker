using Business.Interfaces;
using DataAccess.Repository.Interface;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    public partial class GoToExecutionPage : Page, IExecutionPage, IDetailPage
    {
        public IFlowDetailVM? FlowViewModel { get; set; }
        public IFlowStepDetailVM? FlowStepViewModel { get; set; }
        public IFlowParameterDetailVM? FlowParameterViewModel { get; set; }
        public IExecutionViewModel? ExecutionViewModel { get; set; }


        public IExecutionViewModel ViewModel { get; set; }
        public GoToExecutionPage(IDataService dataService)
        {
            ViewModel = new GoToExecutionVM(dataService);
            ExecutionViewModel = ViewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
