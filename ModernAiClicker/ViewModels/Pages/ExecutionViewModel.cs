using Business.Extensions;
using Business.Factories;
using Business.Interfaces;
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
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class ExecutionViewModel : ObservableObject, INavigationAware
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private readonly IExecutionFactory _executionFactory;

        public event NavigateToExecutionDetailEvent? NavigateToExecutionDetail;
        public delegate void NavigateToExecutionDetailEvent(FlowStepTypesEnum flowStepTypes, Execution? execution);

        // Treeview
        [ObservableProperty]
        private ObservableCollection<Flow> _treeviewFlows = new ObservableCollection<Flow>();


        // Combobox Flows
        [ObservableProperty]
        private Flow? _comboBoxSelectedFlow;

        [ObservableProperty]
        private ObservableCollection<Flow> _comboBoxFlows = new ObservableCollection<Flow>();


        // Combobox Execution history
        [ObservableProperty]
        private Execution? _comboBoxSelectedExecutionHistory;

        [ObservableProperty]
        private ObservableCollection<Execution> _comboBoxExecutionHistories = new ObservableCollection<Execution>();


        // Listbox executions
        [ObservableProperty]
        private Execution? _listboxSelectedExecution;

        [ObservableProperty]
        public ObservableCollection<Execution> _listBoxExecutions = new ObservableCollection<Execution>();



        [ObservableProperty]
        public string _status = "-";

        [ObservableProperty]
        public int _runFor = 0;

        [ObservableProperty]
        public int _currentStep = 0;

        public bool IsLocked = true;
        public bool StopExecution { get; set; }

        public ExecutionViewModel(IBaseDatawork baseDatawork, ISystemService systemService, IExecutionFactory executionFactory)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _executionFactory = executionFactory;

            ComboBoxFlows = GetFlows();
            IsLocked = true;
        }

        private ObservableCollection<Flow> GetFlows()
        {
            List<Flow> flows = _baseDatawork.Flows.GetAll();

            return new ObservableCollection<Flow>(flows);
        }


        [RelayCommand]
        private async Task OnButtonStartClick()
        {
            if (ComboBoxSelectedFlow == null)
                return;


            // Create new thread so UI doesnt freeze.
            await Task.Run(async () =>
            {
                IExecutionWorker flowWorker = _executionFactory.GetWorker(null);
                Execution flowExecution = await flowWorker.CreateExecutionModel(ComboBoxSelectedFlow.Id, null);

                // Add Execution to listbox and select it
                List<Execution> executions = ComboBoxExecutionHistories.ToList(); 
                executions.Add(flowExecution);
                ComboBoxExecutionHistories = new ObservableCollection<Execution>(executions);
                ComboBoxSelectedExecutionHistory = flowExecution; 

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    ListBoxExecutions.Add(flowExecution);
                });


                // Start execution.
                flowWorker.ExpandAndSelectFlowStep(flowExecution);
                AllowUIToUpdate();
                await flowWorker.SetExecutionModelStateRunning(flowExecution);

                // Get next flow step and recursively execute every other step.
                FlowStep? nextFlowStep = await flowWorker.GetNextChildFlowStep(flowExecution);
                await ExecuteStepRecursion(nextFlowStep, flowExecution);

                // Complete execution.
                await flowWorker.SetExecutionModelStateComplete(flowExecution);
            });

            StopExecution = false;
        }

        private async Task<Execution?> ExecuteStepRecursion(FlowStep? flowStep, Execution parentExecution)
        {
            // Recursion ends here.
            if (flowStep == null || StopExecution == true)
                return await Task.FromResult<Execution?>(null);

            IExecutionWorker factoryWorker = _executionFactory.GetWorker(flowStep.FlowStepType);
            Execution flowStepExecution = await factoryWorker.CreateExecutionModel(flowStep.Id, parentExecution);
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ListBoxExecutions.Add(flowStepExecution);
            });

            factoryWorker.ExpandAndSelectFlowStep(flowStepExecution);
            factoryWorker.RefreshUI();
            await factoryWorker.SetExecutionModelStateRunning(flowStepExecution);
            await factoryWorker.ExecuteFlowStepAction(flowStepExecution);
            await factoryWorker.SetExecutionModelStateComplete(flowStepExecution);
            await factoryWorker.SaveToJson();

            FlowStep? nextFlowStep;
            nextFlowStep = await factoryWorker.GetNextChildFlowStep(flowStepExecution);


            // If step contains children, execute recursion for children first.
            // Else if no executable children are found, continue recursion for siblings.
            if (nextFlowStep != null)
                await ExecuteStepRecursion(nextFlowStep, flowStepExecution);
                
            nextFlowStep = await factoryWorker.GetNextSiblingFlowStep(flowStepExecution);

            return await ExecuteStepRecursion(nextFlowStep, flowStepExecution);
        }

        // Refresh UI.
        private static void AllowUIToUpdate()
        {
            DispatcherFrame frame = new();
            // DispatcherPriority set to Input, the highest priority
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                Thread.Sleep(100); // Stop all processes to make sure the UI update is perform
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
            // DispatcherPriority set to Input, the highest priority
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate { }));
        }



        [RelayCommand]
        private void OnButtonStopClick()
        {
            StopExecution = true;
        }


        [RelayCommand]
        private async Task OnComboBoxSelectionChangedFlow(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            if (ComboBoxSelectedFlow == null)
                return;

            var flow = _baseDatawork.Query.Flows
                .FirstOrDefault(x => x.Id == ComboBoxSelectedFlow.Id);

            List<Execution> executions = await _baseDatawork.Executions.Query
                .Where(x => x.FlowId == ComboBoxSelectedFlow.Id)
                .ToListAsync();

            ComboBoxExecutionHistories = new ObservableCollection<Execution>(executions);

            TreeviewFlows.Clear();
            TreeviewFlows.Add(flow);
            //FrameNavigateToFlow?.Invoke(flow, ListBoxExecutions);
        }


        //[RelayCommand]
        //private void OnTreeViewSelectedItemChanged(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        //{
        //    object selectedItem = routedPropertyChangedEventArgs.NewValue;
        //    if (selectedItem is not FlowStep)
        //        return;

        //    FlowStepTypesEnum flowStepType = ((FlowStep)selectedItem).FlowStepType;

        //    if (ListboxSelectedExecution != null)
        //        NavigateToExecutionDetail?.Invoke(flowStepType, ListboxSelectedExecution);

        //    NavigateToExecutionDetail?.Invoke(flowStepType, null);
        //}


        [RelayCommand]
        private async Task OnButtonDeleteClick()
        {
            if (ComboBoxSelectedExecutionHistory == null)
                return;

            _baseDatawork.Executions.Remove(ComboBoxSelectedExecutionHistory);
            _baseDatawork.SaveChanges();
            await _systemService.UpdateFlowsJSON(await _baseDatawork.Flows.GetAllAsync());

            ComboBoxExecutionHistories.Remove(ComboBoxSelectedExecutionHistory);
            ComboBoxSelectedExecutionHistory = null;
            ListBoxExecutions.Clear();
        }

        [RelayCommand]
        private void OnListBoxSelectedItemChanged(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            if (routedPropertyChangedEventArgs?.AddedItems.Count > 0)
            {

                object selectedItem = routedPropertyChangedEventArgs?.AddedItems[0];
                if (selectedItem is not Execution)
                    return;

                Execution selectedExecution = selectedItem as Execution;
                ListboxSelectedExecution = selectedExecution;


                if (selectedExecution.Flow != null)
                    selectedExecution.Flow.IsSelected = true;

                if (selectedExecution.FlowStep != null)
                {
                    selectedExecution.FlowStep.IsSelected = true;

                    NavigateToExecutionDetail?.Invoke(selectedExecution.FlowStep.FlowStepType, ListboxSelectedExecution);
                }
            }
        }

        [RelayCommand]
        private async Task OnComboBoxSelectionChangedExecution(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            if (ComboBoxSelectedFlow == null || ComboBoxSelectedExecutionHistory == null)
            {
                ListBoxExecutions.Clear();
                return;
            }

            // Get recursively all parents.
            Execution execution = await _baseDatawork.Executions.Query

                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .Include(x => x.ChildExecution)
                .FirstAsync(x => x.Id == ComboBoxSelectedExecutionHistory.Id);


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
