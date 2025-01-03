﻿using CommunityToolkit.Mvvm.ComponentModel;
using Model.Models;
using System.Collections.ObjectModel;
using DataAccess.Repository.Interface;
using Business.Factories;

namespace ModernAiClicker.ViewModels.Pages.Executions
{
    public partial class FrameExecutionFlowViewModel : ObservableObject
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly IExecutionFactory _executionFactory;

        public Flow? Flow;

        public ObservableCollection<Execution> Executions;
        public bool StopExecution { get; set; }



        public FrameExecutionFlowViewModel(IBaseDatawork baseDatawork, IExecutionFactory executionFactory)
        {
            _baseDatawork = baseDatawork;
            _executionFactory = executionFactory;
        }

        //[RelayCommand]
        //private async Task OnButtonStartClick()
        //{
        //    if (Flow == null)
        //        return;


        //    // Create new thread so UI doesnt freeze.
        //    await Task.Run(async () =>
        //    {
        //        IExecutionWorker flowWorker = _executionFactory.GetWorker(null);
        //        Execution flowExecution = await flowWorker.CreateExecutionModel(Flow.Id, null);

        //        //Application.Current.Dispatcher.Invoke((Action)delegate
        //        //{
        //        //    Executions.Add(flowExecution);
        //        //});


        //        // Start execution.
        //        await flowWorker.ExpandAndSelectFlowStep(flowExecution);
        //        //AllowUIToUpdate();
        //        await flowWorker.SetExecutionModelStateRunning(flowExecution);

        //        // Get next flow step and recursively execute every other step.
        //        FlowStep? nextFlowStep = await flowWorker.GetNextChildFlowStep(flowExecution);
        //        await ExecuteStepRecursion(nextFlowStep, flowExecution);

        //        // Complete execution.
        //        await flowWorker.SetExecutionModelStateComplete(flowExecution);
        //    });

        //    StopExecution = false;
        //}

        //private async Task<Execution?> ExecuteStepRecursion(FlowStep? flowStep, Execution parentExecution)
        //{
        //    // Recursion ends here.
        //    if (flowStep == null || StopExecution == true)
        //        return await Task.FromResult<Execution?>(null);

        //    IExecutionWorker factoryWorker = _executionFactory.GetWorker(flowStep.FlowStepType);
        //    Execution flowStepExecution = await factoryWorker.CreateExecutionModel(flowStep.Id, parentExecution);
        //    Application.Current.Dispatcher.Invoke((Action)delegate
        //    {
        //        Executions.Add(flowStepExecution);
        //    });

        //    await factoryWorker.ExpandAndSelectFlowStep(flowStepExecution);

        //    factoryWorker.RefreshUI();
        //    AllowUIToUpdate();

        //    await factoryWorker.SetExecutionModelStateRunning(flowStepExecution);
        //    await factoryWorker.ExecuteFlowStepAction(flowStepExecution);
        //    await factoryWorker.SetExecutionModelStateComplete(flowStepExecution);
        //    await factoryWorker.SaveToJson();

        //    FlowStep? nextFlowStep;
        //    nextFlowStep = await factoryWorker.GetNextChildFlowStep(flowStepExecution);


        //    // If step contains children, execute recursion for children first.
        //    // Else if no executable children are found, continue recursion for siblings.
        //    if (nextFlowStep != null)
        //        await ExecuteStepRecursion(nextFlowStep, flowStepExecution);
        //    else
        //        nextFlowStep = await factoryWorker.GetNextSiblingFlowStep(flowStepExecution);

        //    return await ExecuteStepRecursion(nextFlowStep, flowStepExecution);
        //}

        // Refresh UI.
        //private static void AllowUIToUpdate()
        //{
        //    DispatcherFrame frame = new();
        //    // DispatcherPriority set to Input, the highest priority
        //    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object parameter)
        //    {
        //        frame.Continue = false;
        //        Thread.Sleep(100); // Stop all processes to make sure the UI update is perform
        //        return null;
        //    }), null);
        //    Dispatcher.PushFrame(frame);
        //    // DispatcherPriority set to Input, the highest priority
        //    Application.Current.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate { }));
        //}



        //[RelayCommand]
        //private void OnButtonStopClick()
        //{
        //    StopExecution = true;
        //}

    }
}
