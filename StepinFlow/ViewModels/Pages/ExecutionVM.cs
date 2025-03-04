﻿using Business.Extensions;
using Business.Factories;
using Business.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using StepinFlow.ViewModels.UserControls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace StepinFlow.ViewModels.Pages
{
    public partial class ExecutionVM : ObservableObject, INavigationAware
    {
        private readonly IDataService _dataService;
        private readonly ISystemService _systemService;
        private readonly IExecutionFactory _executionFactory;

        public event LoadFlowsEvent? LoadFlows;
        public delegate Task LoadFlowsEvent(int id = 0);

        public event NavigateToExecutionEvent? NavigateToExecution;
        public delegate void NavigateToExecutionEvent(Execution execution);
        public event ExpandAndSelectFlowStepEvent? ExpandAndSelectFlowStep;
        public delegate Task ExpandAndSelectFlowStepEvent(int id);

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
        public string _runFor = "";
        [ObservableProperty]
        public string _currentStep = "";
        [ObservableProperty]
        public bool _isLocked = true;


        private bool _stopExecution = false;
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        private readonly DispatcherTimer _timer;
        private TimeSpan _timeElapsed;


        public ExecutionVM(
            IDataService dataService,
            ISystemService systemService,
            IExecutionFactory executionFactory)
        {
            _dataService = dataService;
            _systemService = systemService;
            _executionFactory = executionFactory;

            ComboBoxFlows = new ObservableCollection<Flow>(_dataService.Query.Flows.ToList());

            _executionFactory.SetCancellationToken(_cancellationToken);

            // Update every second
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        }


        [RelayCommand]
        private async Task OnButtonStartClick()
        {
            if (ComboBoxSelectedFlow == null)
                return;

            _timeElapsed = TimeSpan.Zero;
            void UpdateTimer(object sender, EventArgs e)
            {
                _timeElapsed = _timeElapsed.Add(TimeSpan.FromSeconds(1));
                RunFor = _timeElapsed.ToString(@"hh\:mm\:ss");
            }
            _timer.Tick += UpdateTimer;
            _timer.Start();

            Status = ExecutionStatusEnum.RUNNING.ToString();

            // Create new thread so UI doesnt freeze.
            await Task.Run(async () =>
                {
                    IExecutionWorker flowWorker = _executionFactory.GetWorker(null);
                    Execution flowExecution = await flowWorker.CreateExecutionModelFlow(ComboBoxSelectedFlow.Id, null);
                    FlowStep? nextFlowStep;

                    // Add Execution to listbox and select it
                    List<Execution> executions = ComboBoxExecutionHistories.ToList();
                    executions.Add(flowExecution);
                    ComboBoxExecutionHistories = new ObservableCollection<Execution>(executions);
                    ComboBoxSelectedExecutionHistory = flowExecution;

                    await flowWorker.SetExecutionModelStateRunning(flowExecution);
                    await flowWorker.SaveToDisk(flowExecution);

                    nextFlowStep = await flowWorker.GetNextChildFlowStep(flowExecution);
                    await ExecuteStepLoop(nextFlowStep, flowExecution);
                    await flowWorker.SetExecutionModelStateComplete(flowExecution);
                    _timer.Stop();
                    Status = ExecutionStatusEnum.COMPLETED.ToString();

                });

            _stopExecution = false;
            _cancellationToken = new CancellationTokenSource();
            _executionFactory.SetCancellationToken(_cancellationToken);
            _executionFactory.DestroyWorkers();

        }

        private async Task ExecuteStepLoop(FlowStep? initialFlowStep, Execution initialParentExecution)
        {
            Execution? latestParentExecution = initialParentExecution;
            var stack = new Stack<(FlowStep? flowStep, Execution parentExecution)>();
            stack.Push((initialFlowStep, initialParentExecution));

            while (stack.Count > 0)
            {
                var (flowStep, parentExecution) = stack.Pop();

                if (flowStep == null || _stopExecution == true)
                    return;

                IExecutionWorker factoryWorker = _executionFactory.GetWorker(flowStep.Type);
                factoryWorker.ClearEntityFrameworkChangeTracker();
                Execution flowStepExecution = await factoryWorker.CreateExecutionModel(flowStep, parentExecution, latestParentExecution);
                parentExecution.ResultImage = null;// TODO test if needed.

                // Add execution to history listbox.
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentStep = flowStep.Type.ToString();
                    ListBoxExecutions.Add(flowStepExecution);
                });

                //await ExpandAndSelectFlowStep?.Invoke(flowStepExecution.FlowStepId ?? -1);
                //await factoryWorker.ExpandAndSelectFlowStep(flowStepExecution, _treeViewUserControlViewModel.FlowsList);
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

        [RelayCommand]
        private void OnButtonStopClick()
        {
            _stopExecution = true;
            _cancellationToken.Cancel();
        }


        [RelayCommand]
        private async Task OnComboBoxSelectionChangedFlow(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            if (ComboBoxSelectedFlow == null)
                return;

            List<Execution> executions = await _dataService.Executions.Query
                .Where(x => x.FlowId == ComboBoxSelectedFlow.Id)
                .ToListAsync();

            ComboBoxExecutionHistories = new ObservableCollection<Execution>(executions);
            await LoadFlows?.Invoke(ComboBoxSelectedFlow.Id);
        }


        private async Task LoadExecutionChild(Execution execution)
        {
            Execution? executionChild = await _dataService.Query.Executions
                        .Where(x => x.Id == execution.Id)
                        .Select(x => x.ChildExecution)
                        .FirstOrDefaultAsync();

            if (executionChild == null)
                return;

            execution.ChildExecution = executionChild;
            await LoadExecutionChild(execution.ChildExecution);
        }


        [RelayCommand]
        private async Task OnButtonDeleteClick()
        {

            //Delete all.
            var aa = _dataService.Executions.GetAll();
            await _dataService.Executions.RemoveRangeAsync(aa);

            // Reclaim free space in database file.
            await _dataService.Query.Database.ExecuteSqlRawAsync("VACUUM;");

            //ComboBoxExecutionHistories.Remove(ComboBoxSelectedExecutionHistory);
            ComboBoxSelectedExecutionHistory = null;
            ListBoxExecutions.Clear();

        }



        [RelayCommand]
        private void OnListBoxSelectedItemChanged(SelectionChangedEventArgs routedPropertyChangedEventArgs)
        {
            if (routedPropertyChangedEventArgs?.AddedItems.Count > 0)
            {
                object? selectedItem = routedPropertyChangedEventArgs.AddedItems[0];
                if (selectedItem is not Execution)
                    return;

                Execution selectedExecution = (Execution)selectedItem;
                ListboxSelectedExecution = selectedExecution;

                if (selectedExecution.Flow != null)
                    selectedExecution.Flow.IsSelected = true;

                if (selectedExecution.FlowStep != null)
                {
                    selectedExecution.FlowStep.IsSelected = true;
                    NavigateToExecution?.Invoke(selectedExecution);
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

            Execution? execution = await _dataService.Executions.Query
                .Include(x => x.ChildExecution)
                .FirstOrDefaultAsync(x => x.Id == ComboBoxSelectedExecutionHistory.Id);

            if(execution !=null)
            await LoadExecutionChild(execution);

            List<Execution> executions = execution
                .SelectRecursive(x => x.ChildExecution)
                .Where(x => x != null)
                .ToList();

            executions.Add(execution);
            executions.OrderBy(x => x.Id);

            ListBoxExecutions = new ObservableCollection<Execution>(executions);
        }
        [RelayCommand]
        private void OnImageFailed(ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("Image failed to load: " + e.ErrorException?.Message);
        }


        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }

        public Task OnNavigatedToAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync()
        {
            return Task.CompletedTask;
        }
    }
}
