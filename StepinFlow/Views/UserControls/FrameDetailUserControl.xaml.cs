using Model.Models;
using StepinFlow.ViewModels.UserControls;
using System.Windows.Controls;
using static StepinFlow.ViewModels.UserControls.TreeViewUserControlVM;


namespace StepinFlow.Views.UserControls
{
    public partial class FrameDetailUserControl : UserControl
    {
        public event EventHandler<int>? OnSaveFlow;
        public event EventHandler<int>? OnSaveFlowStep;
        public event EventHandler<int>? OnSaveFlowParameter;

        public FrameDetailUserControlVM ViewModel { get; set; }
        public FrameDetailUserControl()
        {
            FrameDetailUserControlVM? viewModel = App.GetService<FrameDetailUserControlVM>();

            if (viewModel == null )
                throw new InvalidOperationException("Failed to resolve from DI container.");

            viewModel.OnSaveFlowEvent += OnSaveFlowEvent;
            viewModel.OnSaveFlowStepEvent += OnSaveFlowStepEvent;
            viewModel.OnSaveFlowParameterEvent += OnSaveFlowParameterEvent;

            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        public void NavigateToNewFlowStep(FlowStep flowStep) => ViewModel!.NavigateToNewFlowStep(flowStep);
        public async Task NavigateToFlowStep(int id) => await ViewModel!.NavigateToFlowStep(id);
        public async Task NavigateToFlow(int id) => await ViewModel!.NavigateToFlow(id);

        public void OnSaveFlowEvent(int id)
        {
            OnSaveFlow?.Invoke(this, id);
        }

        public void OnSaveFlowStepEvent(int id)
        {
            OnSaveFlowStep?.Invoke(this, id);
        }

        public void OnSaveFlowParameterEvent(int id)
        {
            OnSaveFlowParameter?.Invoke(this, id);
        }
    }
}
