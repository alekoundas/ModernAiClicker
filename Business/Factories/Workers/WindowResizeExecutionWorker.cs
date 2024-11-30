using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using Model.Structs;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class WindowResizeExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public WindowResizeExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService)
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

            newWindowRect.Left = windowRect.Left;
            newWindowRect.Top = windowRect.Top;
            newWindowRect.Right = windowRect.Left + execution.FlowStep.WindowWidth;
            newWindowRect.Bottom = windowRect.Top + execution.FlowStep.WindowHeight;

            bool result = _systemService.MoveWindow(execution.FlowStep.ProcessName, newWindowRect);

            return Task.CompletedTask;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            // Get next sibling flow step. 
            Expression<Func<FlowStep, bool>> nextStepFilter;

            if (execution.FlowStep.ParentFlowStepId != null)
                nextStepFilter = (x) =>
                       x.FlowStepType != FlowStepTypesEnum.IS_NEW
                    && x.OrderingNum > execution.FlowStep.OrderingNum
                    && x.ParentFlowStepId == execution.FlowStep.ParentFlowStepId;
            else
                nextStepFilter = (x) =>
                       x.FlowStepType != FlowStepTypesEnum.IS_NEW
                    && x.OrderingNum > execution.FlowStep.OrderingNum
                    && x.FlowId == execution.FlowStep.FlowId;

            List<FlowStep>? nextFlowSteps = await _baseDatawork.Query.FlowSteps.AsNoTracking()
                .Where(nextStepFilter)
                .ToListAsync();

            FlowStep? nextFlowStep = null;
            if (nextFlowSteps.Any())
                nextFlowStep = nextFlowSteps.Aggregate((currentMin, x) => x.OrderingNum < currentMin.OrderingNum ? x : currentMin);

            //TODO return error message 
            if (nextFlowStep == null)
                return null;

            return nextFlowStep;
        }
    }
}
