using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Business;
using Model.Enums;
using Model.Models;
using System.Drawing;
using System.Linq.Expressions;

namespace Business.Factories.Workers
{
    public class LoopExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;
        private readonly ISystemService _systemService;

        public LoopExecutionWorker(
              IBaseDatawork baseDatawork
            , ISystemService systemService
            ) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
            _systemService = systemService;
        }

        public async override Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution? parentExecution)
        {
            if (parentExecution == null)
                throw new ArgumentNullException(nameof(parentExecution));

            Execution execution = new Execution();
            execution.FlowStepId = flowStep.Id;
            execution.ParentExecutionId = parentExecution.Id;// TODO This is wrong!
            execution.ParentLoopExecutionId = parentExecution.Id;

            execution.ExecutionFolderDirectory = parentExecution.ExecutionFolderDirectory;
            execution.LoopCount = parentExecution?.LoopCount == null ? 0 : parentExecution.LoopCount + 1;


            _baseDatawork.Executions.Add(execution);
            await _baseDatawork.SaveChangesAsync();

            parentExecution.ChildExecutionId = execution.Id;
            await _baseDatawork.SaveChangesAsync();

            execution.FlowStep = flowStep;
            return execution;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            return Task.CompletedTask;
        }

        public async override Task<FlowStep?> GetNextChildFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            FlowStep? nextFlowStep;

            // Get next executable child.
            nextFlowStep = await _baseDatawork.Query.FlowSteps.AsNoTracking()
                .Include(x => x.ChildrenFlowSteps)
                .ThenInclude(x => x.ChildrenFlowSteps)
                .FirstOrDefaultAsync(x => x.Id == execution.FlowStepId);


            if (execution.ExecutionResultEnum == ExecutionResultEnum.SUCCESS)
                nextFlowStep = nextFlowStep.ChildrenFlowSteps
                    .First(x => x.FlowStepType == FlowStepTypesEnum.IS_SUCCESS)
                    .ChildrenFlowSteps
                    .FirstOrDefault(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW && x.OrderingNum == 0);
            else
                nextFlowStep = nextFlowStep.ChildrenFlowSteps
                    .First(x => x.FlowStepType == FlowStepTypesEnum.IS_FAILURE)
                    .ChildrenFlowSteps
                    .FirstOrDefault(x => x.FlowStepType != FlowStepTypesEnum.IS_NEW && x.OrderingNum == 0);

            return nextFlowStep;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);

            // If MaxLoopCount is 0 or CurrentLoopCount < MaxLoopCount, return te same flow step.
            if (execution.FlowStep.MaxLoopCount == 0 || execution.LoopCount < execution.FlowStep.MaxLoopCount)
                return execution.FlowStep;


            // If not, get next sibling flow step. 
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

            return nextFlowStep;
        }
    }
}