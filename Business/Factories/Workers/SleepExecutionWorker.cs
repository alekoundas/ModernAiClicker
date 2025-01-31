using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using System.Threading;

namespace Business.Factories.Workers
{
    public class SleepExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private CancellationTokenSource _cancellationToken;


        public SleepExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService, CancellationTokenSource cancellationToken) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
            _cancellationToken = cancellationToken;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            int miliseconds = 0;

            if (execution.FlowStep.SleepForMilliseconds.HasValue)
                miliseconds += execution.FlowStep.SleepForMilliseconds.Value;

            if (execution.FlowStep.SleepForSeconds.HasValue)
                miliseconds += execution.FlowStep.SleepForSeconds.Value * 1000;

            if (execution.FlowStep.SleepForMinutes.HasValue)
                miliseconds += execution.FlowStep.SleepForMinutes.Value * 60 * 1000;

            if (execution.FlowStep.SleepForHours.HasValue)
                miliseconds += execution.FlowStep.SleepForHours.Value * 60 * 60 * 1000;


            try
            {
                await Task.Delay(miliseconds, _cancellationToken.Token);
            }
            catch (TaskCanceledException) { return; }
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            // Get next sibling flow step. 
            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextSibling(execution.FlowStep.Id);
            return nextFlowStep;
        }
    }
}
