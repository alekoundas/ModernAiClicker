using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.Factories.Workers
{
    public class MouseScrollExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public MouseScrollExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null || !execution.FlowStep.MouseLoopTimes.HasValue)
                return Task.CompletedTask;


            _systemService.CursorScroll(execution.FlowStep.MouseScrollDirectionEnum, execution.FlowStep.MouseLoopTimes.Value);

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
