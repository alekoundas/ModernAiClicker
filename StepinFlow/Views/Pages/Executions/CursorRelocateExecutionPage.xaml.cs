using Business.Interfaces;
using Business.Services.Interfaces;
using StepinFlow.ViewModels.Pages.Executions;
using System.Windows.Controls;

namespace StepinFlow.Views.Pages.Executions
{
    
    public partial class CursorRelocateExecutionPage : Page, IExecutionPage, IDetailPage
    {
        public IFlowDetailVM? FlowViewModel { get; set; }
        public IFlowStepDetailVM? FlowStepViewModel { get; set; }
        public IFlowParameterDetailVM? FlowParameterViewModel { get; set; }
        public IExecutionViewModel? FlowExecutionViewModel { get; set; }


        public IExecutionViewModel ViewModel { get; set; }
        public CursorRelocateExecutionPage(IDataService dataService)
        {
            ViewModel = new CursorRelocateExecutionVM(dataService);
            FlowExecutionViewModel = ViewModel;
            InitializeComponent();
            DataContext = this;
        }
    }
}
