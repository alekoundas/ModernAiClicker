using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;

namespace Business.Factories.Workers
{
    public class SleepExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public SleepExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return Task.CompletedTask;

            int miliseconds = 0;

            if (execution.FlowStep.SleepForMilliseconds.HasValue)
                miliseconds += execution.FlowStep.SleepForMilliseconds.Value;

            if (execution.FlowStep.SleepForSeconds.HasValue)
                miliseconds += execution.FlowStep.SleepForSeconds.Value * 1000;

            if (execution.FlowStep.SleepForMinutes.HasValue)
                miliseconds += execution.FlowStep.SleepForMinutes.Value * 60 * 1000;

            if (execution.FlowStep.SleepForHours.HasValue)
                miliseconds += execution.FlowStep.SleepForHours.Value * 60 * 60 * 1000;


            Thread.Sleep(miliseconds);


            return Task.CompletedTask;
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
