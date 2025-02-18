using Model.Models;
using StepinFlow.ViewModels.UserControls;
using System.Windows.Controls;


namespace StepinFlow.Views.UserControls
{
    public partial class FlowStepFrameUserControl : UserControl
    {
        public FlowStepFrameUserControlViewModel ViewModel { get; set; }
        public FlowStepFrameUserControl()
        {
            FlowStepFrameUserControlViewModel? viewModel = App.GetService<FlowStepFrameUserControlViewModel>();

            if (viewModel == null )
                throw new InvalidOperationException("Failed to resolve from DI container.");


            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void NavigateToNewFlowStep(FlowStep flowStep) => ViewModel!.NavigateToNewFlowStep(flowStep);
        public async Task NavigateToFlowStep(int id) => await ViewModel!.NavigateToFlowStep(id);
        public async Task NavigateToFlow(int id) => await ViewModel!.NavigateToFlow(id);
    }
}
