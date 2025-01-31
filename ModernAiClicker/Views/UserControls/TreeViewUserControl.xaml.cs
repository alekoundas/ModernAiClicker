using Model.Models;
using ModernAiClicker.ViewModels.UserControls;
using System.Windows.Controls;

namespace ModernAiClicker.Views.UserControls
{
    public partial class TreeViewUserControl : UserControl
    {
        public event EventHandler<int>? OnSelectedFlowStepIdChange;
        public event EventHandler<int>? OnFlowStepClone;
        public event EventHandler<FlowStep>? OnAddFlowStepClick;
        public TreeViewUserControlViewModel ViewModel { get; set; }

        public TreeViewUserControl()
        {
            // Resolve the ViewModel from the DI container.
            ViewModel = App.GetService<TreeViewUserControlViewModel>();

            if (ViewModel == null)
            {
                throw new InvalidOperationException("Failed to resolve TreeViewUserControlViewModel from DI container.");
            }

            DataContext = this;
            InitializeComponent();
            ViewModel.OnSelectedFlowStepIdChangedEvent += OnSelectedFlowStepIdChangedEvent;
            ViewModel.OnFlowStepCloneEvent += OnFlowStepCloneEvent;
            ViewModel.OnAddFlowStepClickEvent += OnAddFlowStepClickEvent;

        }

        public void OnSelectedFlowStepIdChangedEvent(int id)
        {
            OnSelectedFlowStepIdChange?.Invoke(this,id);
        }

        public void OnFlowStepCloneEvent(int id)
        {
            OnFlowStepClone?.Invoke(this, id);
        }

        public void OnAddFlowStepClickEvent(FlowStep adddFlowSttep)
        {
            OnAddFlowStepClick?.Invoke(this, adddFlowSttep);
        }


    }
}
