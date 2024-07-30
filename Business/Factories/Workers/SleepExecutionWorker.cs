using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using Model.Structs;
using System.Linq.Expressions;

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

        public async Task<Execution> CreateExecutionModel(int flowStepId, Execution parentExecution)
        {
            Execution execution = new Execution();
            execution.FlowStepId = flowStepId;
            execution.ParentExecutionId = parentExecution.Id;

            _baseDatawork.Executions.Add(execution);
            parentExecution.ChildExecutionId = execution.Id;
            await _baseDatawork.SaveChangesAsync();

            return execution;
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
                miliseconds += execution.FlowStep.SleepForMinutes.Value * 60 * 100;

            if (execution.FlowStep.SleepForHours.HasValue)
                miliseconds += execution.FlowStep.SleepForHours.Value * 60 * 60 * 100;


            Thread.Sleep(miliseconds);


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

            List<FlowStep>? nextFlowSteps = await _baseDatawork.Query.FlowSteps
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

        public async Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            // Cursor move doesnt contain any children.
            return await Task.FromResult<FlowStep?>(null);
        }

        public async Task SetExecutionModelStateRunning(Execution execution)
        {
            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;

            if (execution.ParentExecution != null)
                execution.ExecutionFolderDirectory = execution.ParentExecution.ExecutionFolderDirectory;

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

            execution.FlowStep.IsExpanded = true;
            execution.FlowStep.IsSelected = true;
        }


    }
}
