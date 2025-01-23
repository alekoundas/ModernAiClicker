using Business.Extensions;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using Model.Structs;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class MouseMoveExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public MouseMoveExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return ;

            Point pointToMove;
            Execution? parentExecution = null;

            // Get point from result of parent template search.
            if (execution.ParentExecutionId != null)
            {
                Execution? currentExecution = execution;
                while (currentExecution.ParentExecutionId != null)
                {
                    currentExecution = await _baseDatawork.Executions.Query
                        .Include(x=>x.FlowStep)
                        .FirstAsync(x=>x.Id==currentExecution.ParentExecutionId.Value);

                    if(currentExecution.FlowStepId == execution.FlowStep.ParentTemplateSearchFlowStepId)
                    {
                        parentExecution = currentExecution;
                        break;
                    }
                }


                // If parentExecution exists get value from result.
                // Else get point from flow step.
                if (parentExecution?.ResultLocationX != null && parentExecution?.ResultLocationY != null)
                    pointToMove = new Point(parentExecution.ResultLocationX.Value, parentExecution.ResultLocationY.Value);
                else   
                    pointToMove = new Point(execution.FlowStep.LocationX, execution.FlowStep.LocationY);

                _systemService.SetCursorPossition(pointToMove);
                execution.ResultLocationX = pointToMove.X;
                execution.ResultLocationY = pointToMove.Y;
                await _baseDatawork.SaveChangesAsync();
            }
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
