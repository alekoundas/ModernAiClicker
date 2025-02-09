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

            miliseconds += execution.FlowStep.SleepForMilliseconds;
            miliseconds += execution.FlowStep.SleepForSeconds * 1000;
            miliseconds += execution.FlowStep.SleepForMinutes * 60 * 1000;
            miliseconds += execution.FlowStep.SleepForHours * 60 * 60 * 1000;


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
