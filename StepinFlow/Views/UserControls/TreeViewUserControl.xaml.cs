using Model.Models;
using StepinFlow.ViewModels.UserControls;
using System.Windows;
using System.Windows.Controls;
using static StepinFlow.ViewModels.Pages.FlowsViewModel;

namespace StepinFlow.Views.UserControls
{
    public partial class TreeViewUserControl : UserControl
    {
        // Define the IsLocked Dependency Property
        //public static readonly DependencyProperty IsLockedProperty =
        //    DependencyProperty.Register(
        //        nameof(IsLocked),              // Property name
        //        typeof(bool),                  // Property type
        //        typeof(TreeViewUserControl),   // Owner type
        //        new PropertyMetadata(
        //            defaultValue: false,       // Default value
        //            propertyChangedCallback: OnIsLockedChanged)); // Callback

        //// CLR wrapper for the Dependency Property
        //public bool IsLocked
        //{
        //    get => (bool)GetValue(IsLockedProperty);
        //    set => SetValue(IsLockedProperty, value);
        //}


        public event EventHandler<int>? OnSelectedFlowStepIdChange;
        public event EventHandler<int>? OnSelectedFlowIdChange;
        public event EventHandler<int>? OnSelectedFlowParameterIdChange;
        public event EventHandler<int>? OnFlowStepClone;
        public event EventHandler<FlowStep>? OnAddFlowStepClick;
        public event EventHandler<FlowParameter>? OnAddFlowParameterClick;
        public TreeViewUserControlViewModel? ViewModel { get; set; }

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
            ViewModel.OnSelectedFlowIdChangedEvent += OnSelectedFlowIdChangedEvent;
            ViewModel.OnSelectedFlowParameterIdChangedEvent += OnSelectedFlowParameterIdChangedEvent;
            ViewModel.OnFlowStepCloneEvent += OnFlowStepCloneEvent;
            ViewModel.OnAddFlowStepClickEvent += OnAddFlowStepClickEvent;
            ViewModel.OnAddFlowParameterClickEvent += OnAddFlowParameterClickEvent;

        }

        public async Task LoadFlows(int? id = 0) => await ViewModel!.LoadFlows(id);
        public void ClearCopy() => ViewModel!.ClearCopy();
        public async Task ExpandAll() => await ViewModel!.ExpandAll();
        public async Task CollapseAll() => await ViewModel!.CollapseAll();


       
        public void OnSelectedFlowStepIdChangedEvent(int id)
        {
            OnSelectedFlowStepIdChange?.Invoke(this, id);
        }
        public void OnSelectedFlowIdChangedEvent(int id)
        {
            OnSelectedFlowIdChange?.Invoke(this, id);
        }
        public void OnSelectedFlowParameterIdChangedEvent(int id)
        {
            OnSelectedFlowParameterIdChange?.Invoke(this, id);
        }

        public void OnFlowStepCloneEvent(int id)
        {
            OnFlowStepClone?.Invoke(this, id);
        }

        public void OnAddFlowStepClickEvent(FlowStep adddFlowSttep)
        {
            OnAddFlowStepClick?.Invoke(this, adddFlowSttep);
        }
        public void OnAddFlowParameterClickEvent(FlowParameter addFlowParameter)
        {
            OnAddFlowParameterClick?.Invoke(this, addFlowParameter);
        }


        //private static void OnIsLockedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = (TreeViewUserControl)d;
        //    control.ViewModel.IsLocked = (bool)e.NewValue; // Sync with ObservableProperty
        //}
    }
}
