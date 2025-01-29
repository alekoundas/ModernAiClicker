using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Model.Structs;

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
            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextSibling(execution.FlowStep.Id);
            return nextFlowStep;
        }
    }
}
