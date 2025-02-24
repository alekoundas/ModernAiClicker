using Model.Models;
using StepinFlow.ViewModels.UserControls;
using System.Windows.Controls;

namespace StepinFlow.Views.UserControls
{
    public partial class TreeViewUserControl : UserControl
    {
        public event EventHandler<int>? OnSelectedFlowStepIdChange;
        public event EventHandler<int>? OnSelectedFlowIdChange;
        public event EventHandler<int>? OnSelectedFlowParameterIdChange;
        public event EventHandler<int>? OnFlowStepClone;
        public event EventHandler<FlowStep>? OnAddFlowStepClick;
        public event EventHandler<FlowParameter>? OnAddFlowParameterClick;


        public TreeViewUserControlVM ViewModel { get; set; }

        public TreeViewUserControl()
        {
            // Resolve the ViewModel from the DI container.
            TreeViewUserControlVM? viewModel = App.GetService<TreeViewUserControlVM>();

            if (ViewModel == null)
                throw new InvalidOperationException("Failed to resolve TreeViewUserControlViewModel from DI container.");

            DataContext = this;
            InitializeComponent();
            viewModel.OnSelectedFlowStepIdChangedEvent += OnSelectedFlowStepIdChangedEvent;
            viewModel.OnSelectedFlowIdChangedEvent += OnSelectedFlowIdChangedEvent;
            viewModel.OnSelectedFlowParameterIdChangedEvent += OnSelectedFlowParameterIdChangedEvent;
            viewModel.OnFlowStepCloneEvent += OnFlowStepCloneEvent;
            viewModel.OnAddFlowStepClickEvent += OnAddFlowStepClickEvent;
            viewModel.OnAddFlowParameterClickEvent += OnAddFlowParameterClickEvent;

            ViewModel = viewModel;
        }


        public void OnSelectedFlowStepIdChangedEvent(int id) => OnSelectedFlowStepIdChange?.Invoke(this, id);
        public void OnSelectedFlowIdChangedEvent(int id) => OnSelectedFlowIdChange?.Invoke(this, id);
        public void OnSelectedFlowParameterIdChangedEvent(int id) => OnSelectedFlowParameterIdChange?.Invoke(this, id);
        public void OnFlowStepCloneEvent(int id) => OnFlowStepClone?.Invoke(this, id);
        public void OnAddFlowStepClickEvent(FlowStep adddFlowSttep) => OnAddFlowStepClick?.Invoke(this, adddFlowSttep);
        public void OnAddFlowParameterClickEvent(FlowParameter addFlowParameter) => OnAddFlowParameterClick?.Invoke(this, addFlowParameter);
    }
}
