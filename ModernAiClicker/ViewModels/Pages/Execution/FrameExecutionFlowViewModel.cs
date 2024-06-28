using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Model.Models;
using Business.Interfaces;
using System.Collections.ObjectModel;
using DataAccess.Repository.Interface;
using Business.Factories;
using System.Windows.Threading;

namespace ModernAiClicker.ViewModels.Pages
{
    public partial class FrameExecutionFlowViewModel : ObservableObject
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly IExecutionFactory _executionFactory;

        [ObservableProperty]
        private Flow? _flow;


        public FrameExecutionFlowViewModel(IBaseDatawork baseDatawork, IExecutionFactory executionFactory)
        {
            _baseDatawork = baseDatawork;
            _executionFactory = executionFactory;
        }


        [RelayCommand]
        private async Task OnButtonStartClick()
        {
            if (Flow == null)
                return;

            // Create new thread so UI doesnt freeze.
            await Task.Run(async () =>
            {
                IExecutionWorker flowWorker = _executionFactory.GetWorker(null);
                Execution flowExecution = await flowWorker.CreateExecutionModel(Flow.Id, null);

                // Start execution.
                await flowWorker.SetExecutionModelStateRunning(flowExecution);

                // Get next flow step and recursively execute every other step.
                FlowStep? nextFlowStep = await flowWorker.GetNextChildFlowStep(flowExecution);
                await ExecuteStepRecursion(nextFlowStep, flowExecution.Id);

                // Complete execution.
                await flowWorker.SetExecutionModelStateComplete(flowExecution);
            });
        }

        private async Task<Execution?> ExecuteStepRecursion(FlowStep? flowStep, int parentExecutionId)
        {
            if (flowStep == null)
                return await Task.FromResult<Execution?>(null);

            IExecutionWorker factoryWorker = _executionFactory.GetWorker(flowStep.FlowStepType);
            Execution flowStepExecution = await factoryWorker.CreateExecutionModel(flowStep.Id, parentExecutionId);

            factoryWorker.ExpandAndSelectFlowStep(flowStepExecution);
            await factoryWorker.SetExecutionModelStateRunning(flowStepExecution);
            await factoryWorker.ExecuteFlowStepAction(flowStepExecution);
            await factoryWorker.SetExecutionModelStateComplete(flowStepExecution);

            FlowStep? nextFlowStep;
            nextFlowStep = await factoryWorker.GetNextChildFlowStep(flowStepExecution);

            // If no executable children are found check for siblings
            if (nextFlowStep == null)
                nextFlowStep = await factoryWorker.GetNextSiblingFlowStep(flowStepExecution);


            return await ExecuteStepRecursion(nextFlowStep, flowStepExecution.Id);
        }



        [RelayCommand]
        private void OnButtonStopClick()
        {
        }

    }
}
