using Business.Extensions;
using Business.Factories;
using Business.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.ConverterModels;
using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;
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

        [ObservableProperty]
        public bool _isLocked = true;
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
            List<Flow> flows = _baseDatawork.Query.Flows.ToList();
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
                Execution flowExecution = await flowWorker.CreateExecutionModelFlow(ComboBoxSelectedFlow.Id, null);

                // Add Execution to listbox and select it
                List<Execution> executions = ComboBoxExecutionHistories.ToList();
                executions.Add(flowExecution);
                ComboBoxExecutionHistories = new ObservableCollection<Execution>(executions);
                ComboBoxSelectedExecutionHistory = flowExecution;


                //await flowWorker.ExpandAndSelectFlowStep(flowExecution);
                await flowWorker.SetExecutionModelStateRunning(flowExecution);
                await flowWorker.SaveToDisk(flowExecution);

                FlowStep? nextFlowStep = await flowWorker.GetNextChildFlowStep(flowExecution);
                await ExecuteStepLoop(nextFlowStep, flowExecution);
                await flowWorker.SetExecutionModelStateComplete(flowExecution);
            });

            StopExecution = false;
        }



        private async Task ExecuteStepLoop(FlowStep? initialFlowStep, Execution initialParentExecution)
        {
            var stack = new Stack<(FlowStep? flowStep, Execution parentExecution)>();
            stack.Push((initialFlowStep, initialParentExecution));

            while (stack.Count > 0)
            {
                var (flowStep, parentExecution) = stack.Pop();

                if (flowStep == null || StopExecution == true)
                    return;

                IExecutionWorker factoryWorker = _executionFactory.GetWorker(flowStep.FlowStepType);
                factoryWorker.ClearEntityFrameworkChangeTracker();
                Execution flowStepExecution = await factoryWorker.CreateExecutionModel(flowStep, parentExecution);
                parentExecution.ResultImage = null;// TODO test if needed.


                // Add execution to history listbox.
                Application.Current.Dispatcher.Invoke(() => ListBoxExecutions.Add(flowStepExecution));

                await factoryWorker.ExpandAndSelectFlowStep(flowStepExecution, TreeviewFlows);
                await factoryWorker.SetExecutionModelStateRunning(flowStepExecution);
                await factoryWorker.ExecuteFlowStepAction(flowStepExecution);
                await factoryWorker.SetExecutionModelStateComplete(flowStepExecution);
                await factoryWorker.SaveToDisk(flowStepExecution);

                // If step has a sibling, push it first in stack.
                FlowStep? nextFlowStep;
                nextFlowStep = await factoryWorker.GetNextSiblingFlowStep(flowStepExecution);
                if (nextFlowStep != null)
                    stack.Push((nextFlowStep, flowStepExecution));

                // If child is found, push it to stack last so it can be executed firtst.
                nextFlowStep = await factoryWorker.GetNextChildFlowStep(flowStepExecution);
                if (nextFlowStep != null)
                    stack.Push((nextFlowStep, flowStepExecution));
            }
        }

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

            var flow = await _baseDatawork.Query.Flows
                .Include(x => x.FlowSteps)
                .FirstOrDefaultAsync(x => x.Id == ComboBoxSelectedFlow.Id);

            foreach (FlowStep flowStep in flow.FlowSteps)
                await LoadFlowStepChildren(flowStep);


            List<Execution> executions = await _baseDatawork.Executions.Query
                .Where(x => x.FlowId == ComboBoxSelectedFlow.Id)
                .ToListAsync();

            ComboBoxExecutionHistories = new ObservableCollection<Execution>(executions);

            TreeviewFlows.Clear();
            TreeviewFlows.Add(flow);
        }

        private async Task LoadFlowStepChildren(FlowStep flowStep)
        {
            List<FlowStep> flowSteps = await _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .ThenInclude(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStep.Id)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .ToListAsync();

            flowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);

            foreach (var childFlowStep in flowStep.ChildrenFlowSteps)
            {
                if (childFlowStep.IsExpanded)
                    await LoadFlowStepChildren(childFlowStep);
            }
        }

        private async Task LoadExecutionChild(Execution execution)
        {
            Execution? executionChild = await _baseDatawork.Query.Executions
                        .Include(x => x.ChildExecution)
                        .FirstOrDefaultAsync(x => x.Id == execution.Id);

            executionChild = executionChild.ChildExecution;

            if (executionChild == null)
                return;

            execution.ChildExecution = executionChild;
            await LoadExecutionChild(execution.ChildExecution);
        }

        [RelayCommand]
        private async Task OnTreeViewItemExpanded(EventParammeters eventParameters)
        {
            if (eventParameters == null)
                return;

            if (eventParameters.FlowId is FlowStep)
            {

                FlowStep flowStep = (FlowStep)eventParameters.FlowId;

                if (flowStep.ChildrenFlowSteps == null)
                    return;

                foreach (var childrenFlowStep in flowStep.ChildrenFlowSteps)
                {
                    if (childrenFlowStep.ChildrenFlowSteps == null || childrenFlowStep.ChildrenFlowSteps.Count == 0)
                    {
                        List<FlowStep> flowSteps = await _baseDatawork.Query.FlowSteps
                            .Include(x => x.ChildrenFlowSteps)
                            .Where(x => x.Id == childrenFlowStep.Id)
                            .SelectMany(x => x.ChildrenFlowSteps)
                            .ToListAsync();

                        childrenFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
                    }
                }
            }

            else if (eventParameters.FlowId is Flow)
            {

                Flow flow = (Flow)eventParameters.FlowId;

                foreach (var childrenFlowStep in flow.FlowSteps)
                {
                    if (childrenFlowStep.ChildrenFlowSteps == null || childrenFlowStep.ChildrenFlowSteps.Count == 0)
                    {
                        List<FlowStep> flowSteps = await _baseDatawork.Query.FlowSteps
                            .Include(x => x.ChildrenFlowSteps)
                            .Where(x => x.Id == childrenFlowStep.Id)
                            .SelectMany(x => x.ChildrenFlowSteps)
                            .ToListAsync();

                        childrenFlowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);
                    }
                }
            }
        }

        [RelayCommand]
        private async Task OnButtonDeleteClick()
        {

            // Export.
            //var flow = await _baseDatawork.Query.Flows
            //    .Include(x => x.FlowSteps)
            //    .FirstOrDefaultAsync(x => x.Id == 2);
            //foreach (FlowStep flowStep in flow.FlowSteps)
            //    await LoadFlowStepChildrenExport(flowStep);


            //Import
            var aaaaa = _systemService.LoadFlowsJSON();
            _baseDatawork.Flows.Add(aaaaa[0]);
            await _baseDatawork.SaveChangesAsync();


            //await _baseDatawork.Query.Database.ExecuteSqlRawAsync("CREATE TABLE TempExecutions AS SELECT * FROM Executions WHERE 1=0;");
            //await _baseDatawork.Query.Database.ExecuteSqlRawAsync("DROP TABLE Executions;");
            //await _baseDatawork.Query.Database.ExecuteSqlRawAsync("ALTER TABLE TempExecutions RENAME TO Executions;");

            //if (ComboBoxSelectedExecutionHistory == null)
            //    return;
            //_baseDatawork.Query.ChangeTracker.Clear();

            //await _baseDatawork.Query.Database.ExecuteSqlRawAsync("DELETE FROM Executions;");

            var aa = _baseDatawork.Executions.GetAll();
            _baseDatawork.Executions.RemoveRange(aa);
            _baseDatawork.SaveChanges();
            //// Reclaim free space in database file.
            await _baseDatawork.Query.Database.ExecuteSqlRawAsync("VACUUM;");

            //ComboBoxExecutionHistories.Remove(ComboBoxSelectedExecutionHistory);
            //ComboBoxSelectedExecutionHistory = null;
            //ListBoxExecutions.Clear();

        }

        private async Task LoadFlowStepChildrenExport(FlowStep flowStep)
        {
            List<FlowStep> flowSteps = await _baseDatawork.Query.FlowSteps
                        .Include(x => x.ChildrenFlowSteps)
                        .ThenInclude(x => x.ChildrenFlowSteps)
                        .Where(x => x.Id == flowStep.Id)
                        .SelectMany(x => x.ChildrenFlowSteps)
                        .ToListAsync();

            flowStep.ChildrenFlowSteps = new ObservableCollection<FlowStep>(flowSteps);

            foreach (var childFlowStep in flowStep.ChildrenFlowSteps)
            {
                await LoadFlowStepChildren(childFlowStep);
            }
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

            Execution execution = await _baseDatawork.Executions.Query
                .Include(x => x.ChildExecution)
                .FirstAsync(x => x.Id == ComboBoxSelectedExecutionHistory.Id);

            await LoadExecutionChild(execution);

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
