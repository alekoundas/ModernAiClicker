using Business.DatabaseContext;
using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class MouseScrollExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;
        private readonly IDbContextFactory<InMemoryDbContext> _contextFactory;

        public MouseScrollExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService):base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async Task<Execution> CreateExecutionModel(int flowStepId, Execution parentExecution)
        {
            var context = _contextFactory.CreateDbContext();

            Execution execution = new Execution();
            execution.FlowStepId = flowStepId;
            execution.ParentExecutionId = parentExecution.Id;
            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;

            context.Executions.Add(execution);
            await context.SaveChangesAsync();

            parentExecution.ChildExecutionId = execution.Id;
            await context.SaveChangesAsync();

            if (execution.FlowStep == null && execution.FlowStepId != null)
            execution.FlowStep = context.FlowSteps.Include(x => x.ChildrenFlowSteps).ThenInclude(x => x.ChildrenFlowSteps).First(x => x.Id == flowStepId);

            return execution;
        }


        public Task ExecuteFlowStepAction(Execution execution)
        {
            _systemService.CursorScroll();

            return Task.CompletedTask;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            var context = _contextFactory.CreateDbContext();

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

            List<FlowStep>? nextFlowSteps = await context.FlowSteps
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
            var context = _contextFactory.CreateDbContext();
            execution.Status = ExecutionStatusEnum.RUNNING;
            execution.StartedOn = DateTime.Now;

            if (execution.ParentExecution != null)
                execution.ExecutionFolderDirectory = execution.ParentExecution.ExecutionFolderDirectory;

            await context.SaveChangesAsync();
        }

        public async Task SetExecutionModelStateComplete(Execution execution)
        {
            var context = _contextFactory.CreateDbContext();
            execution.Status = ExecutionStatusEnum.COMPLETED;
            execution.EndedOn = DateTime.Now;

            await context.SaveChangesAsync();
        }

        public async Task ExpandAndSelectFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return;

            execution.FlowStep.IsExpanded = true;
            execution.FlowStep.IsSelected = true;
        }

    }
}
