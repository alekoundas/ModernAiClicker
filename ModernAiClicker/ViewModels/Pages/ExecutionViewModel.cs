using Business.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class ExecutionViewModel : ObservableObject, INavigationAware
    {
        public event FrameNavigateToFlowEvent? FrameNavigateToFlow;
        public delegate void FrameNavigateToFlowEvent(Flow flowStep, ObservableCollection<Execution> Executions);

        public event NavigateToExecutionDetailEvent? NavigateToExecutionDetail;
        public delegate void NavigateToExecutionDetailEvent(FlowStepTypesEnum flowStepTypes, Execution? execution);



        [ObservableProperty]
        private ObservableCollection<Flow> _treeviewFlows = new ObservableCollection<Flow>();

        [ObservableProperty]
        public ObservableCollection<Execution> _listBoxExecutions = new ObservableCollection<Execution>();

        [ObservableProperty]
        private ObservableCollection<Flow> _comboBoxFlows = new ObservableCollection<Flow>();

        [ObservableProperty]
        private ObservableCollection<Execution> _comboBoxExecutions = new ObservableCollection<Execution>();


        [ObservableProperty]
        private Flow? _comboBoxSelectedFlow;

        [ObservableProperty]
        private Execution? _comboBoxSelectedExecution;

        private readonly IBaseDatawork _baseDatawork;

        public bool IsLocked = true;
        public ExecutionViewModel(IBaseDatawork baseDatawork)
        {
            _baseDatawork = baseDatawork;
            ComboBoxFlows = GetFlows();
            IsLocked = true;
        }

        private ObservableCollection<Flow> GetFlows()
        {
            List<Flow> flows = _baseDatawork.Flows.GetAll();

            return new ObservableCollection<Flow>(flows);
        }


        [RelayCommand]
        private async Task OnComboBoxSelectionChangedFlow(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            if (ComboBoxSelectedFlow == null)
                return;

            Flow flow = await _baseDatawork.Flows
                .FirstOrDefaultAsync(x => x.Id == ComboBoxSelectedFlow.Id);

            List<Execution> executions = await _baseDatawork.Executions.Query
                .Where(x => x.FlowId == ComboBoxSelectedFlow.Id)
                .ToListAsync();

            ComboBoxExecutions = new ObservableCollection<Execution>(executions);

            TreeviewFlows.Clear();
            TreeviewFlows.Add(flow);

            FrameNavigateToFlow?.Invoke(flow, ListBoxExecutions);
        }


        [RelayCommand]
        private void OnTreeViewSelectedItemChanged(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            object selectedItem = routedPropertyChangedEventArgs.NewValue;
            if (selectedItem is not FlowStep)
                return;

            FlowStepTypesEnum flowStepType = ((FlowStep)selectedItem).FlowStepType;

            if (ComboBoxSelectedExecution != null)
                NavigateToExecutionDetail?.Invoke(flowStepType, ComboBoxSelectedExecution);

            NavigateToExecutionDetail?.Invoke(flowStepType, null);
        }

        [RelayCommand]
        private void OnListBoxSelectedItemChanged(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            object selectedItem = routedPropertyChangedEventArgs?.AddedItems[0];
            if (selectedItem is not Execution)
                return;

            Execution selectedExecution = selectedItem as Execution;
            if (selectedExecution.Flow != null)
                selectedExecution.Flow.IsSelected = true;

            if (selectedExecution.FlowStep != null)
                selectedExecution.FlowStep.IsSelected = true;
        }

        [RelayCommand]
        private async Task OnComboBoxSelectionChangedExecution(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            if (ComboBoxSelectedFlow == null || ComboBoxSelectedExecution == null)
                return;

            // Get recursively all parents.
            Execution execution = await _baseDatawork.Executions.Query

                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .Include(x => x.ChildExecution).ThenInclude(x => x.FlowStep)
                .FirstAsync(x => x.Id == ComboBoxSelectedExecution.Id);


            List<Execution> executions = execution
                .SelectRecursive(x => x.ChildExecution)
                .Where(x => x != null)
                .ToList();

            executions.Add(execution);
            executions.OrderBy(x => x.Id);

            ListBoxExecutions = new ObservableCollection<Execution>(executions);
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom() { }

    }
}
