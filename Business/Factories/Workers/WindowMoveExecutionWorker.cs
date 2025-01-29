using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Enums;
using Model.Models;
using Model.Structs;

namespace Business.Factories.Workers
{
    public class WindowMoveExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public WindowMoveExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService)
            : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep?.ProcessName.Length <= 1 || execution.FlowStep == null)
                return Task.CompletedTask;

            Rectangle windowRect = _systemService.GetWindowSize(execution.FlowStep.ProcessName);
            Rectangle newWindowRect = new Rectangle();

            int x = execution.FlowStep.LocationX;
            int y = execution.FlowStep.LocationY;
            int height = Math.Abs(windowRect.Bottom - windowRect.Top);
            int width = Math.Abs(windowRect.Left - windowRect.Right);

            newWindowRect.Left = x;
            newWindowRect.Top = y;
            newWindowRect.Right = x + width;
            newWindowRect.Bottom = y + height;

            bool result = _systemService.MoveWindow(execution.FlowStep.ProcessName, newWindowRect);

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
