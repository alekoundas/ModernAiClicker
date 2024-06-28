using Business.Helpers;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Model.Business;
using Model.Enums;
using Model.Models;
using Model.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories.Workers
{
    public class MouseClickExecutionWorker : IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public MouseClickExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task<Execution> CreateExecutionModel(int flowStepId, int? parentExecutionId)
        {
            Execution execution = new Execution();
            execution.FlowStepId = flowStepId;
            execution.ParentExecutionId = parentExecutionId;

            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            return execution;
        }

        public async Task ExecuteFlowStepAction(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            switch (execution.FlowStep.MouseAction)
            {
                case MouseActionsEnum.SINGLE_CLICK:
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    return;
                case MouseActionsEnum.DOUBLE_CLICK:
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    _systemService.CursorClick(execution.FlowStep.MouseButton);
                    return;
                // TODO
                case MouseActionsEnum.LOOP_CLICK:
                    do
                    {
                        _systemService.CursorClick(execution.FlowStep.MouseButton);
                    } while (true);
                default:
                    return;
            }
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            Expression<Func<FlowStep, bool>> firstFlowStepFilter;

            firstFlowStepFilter = PredicateHelper.Create<FlowStep>(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW);
            firstFlowStepFilter = firstFlowStepFilter.And(x => x.OrderingNum == execution.FlowStep.OrderingNum + 1);

            if (execution.FlowStep.ParentFlowStepId != null)
                firstFlowStepFilter = firstFlowStepFilter.And(x => x.ParentFlowStepId == execution.FlowStep.ParentFlowStepId);
            else
                firstFlowStepFilter = firstFlowStepFilter.And(x => x.FlowId == execution.FlowStep.FlowId);

            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.FirstOrDefaultAsync(firstFlowStepFilter);

            //TODO return error message 
            if (nextFlowStep == null)
                return null;

            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            // Cursor move doesnt contain any children.
            return await Task.FromResult<FlowStep?>(null);
        }

        public async Task SetExecutionModelStateRunning(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;

            await _baseDatawork.SaveChangesAsync();
        }

        public async Task SetExecutionModelStateComplete(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.COMPLETED;
            execution.EndedOn = DateTime.Now;

            await _baseDatawork.SaveChangesAsync();
        }
        public void ExpandAndSelectFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            execution.FlowStep.IsSelected = true;
            execution.FlowStep.IsExpanded = true;
        }

    }
}
