using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;

namespace Business.Factories.Workers
{
    public class MouseClickExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public MouseClickExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            switch (execution.FlowStep?.MouseAction)
            {
                case MouseActionsEnum.SINGLE_CLICK:
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    break;
                case MouseActionsEnum.DOUBLE_CLICK:
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    break;
                // TODO
                case MouseActionsEnum.LOOP_CLICK:
                    do
                    {
                        _systemService.CursorClick(execution.FlowStep.MouseButton);
                    } while (true);
                default:
                    break;
            }

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
