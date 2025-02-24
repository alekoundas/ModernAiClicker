using Model.Models;
using StepinFlow.ViewModels.UserControls;
using System.Windows.Controls;
using static StepinFlow.ViewModels.UserControls.TreeViewUserControlVM;


namespace StepinFlow.Views.UserControls
{
    public partial class FrameDetailUserControl : UserControl
    {
        public event EventHandler<int>? OnSaveFlowStep;

        public FrameDetailUserControlVM ViewModel { get; set; }
        public FrameDetailUserControl()
        {
            FrameDetailUserControlVM? viewModel = App.GetService<FrameDetailUserControlVM>();

            if (viewModel == null )
                throw new InvalidOperationException("Failed to resolve from DI container.");

            viewModel.OnSaveFlowStepEvent += OnSaveFlowStepEvent;

            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void NavigateToNewFlowStep(FlowStep flowStep) => ViewModel!.NavigateToNewFlowStep(flowStep);
        public async Task NavigateToFlowStep(int id) => await ViewModel!.NavigateToFlowStep(id);
        public async Task NavigateToFlow(int id) => await ViewModel!.NavigateToFlow(id);

        public void OnSaveFlowStepEvent(int id)
        {
            OnSaveFlowStep?.Invoke(this, id);
        }
    }
}
