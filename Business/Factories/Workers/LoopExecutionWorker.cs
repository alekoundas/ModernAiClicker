using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;

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

        public async override Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution parentExecution, Execution latestParentExecution)
        {
            Execution execution = new Execution();
            execution.FlowStepId = flowStep.Id;
            execution.ParentExecutionId = latestParentExecution.Id;
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
            if (execution.FlowStepId == null)
                return await Task.FromResult<FlowStep?>(null);

            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextChild(execution.FlowStepId.Value, null);
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
            FlowStep? nextFlowStep = await _baseDatawork.FlowSteps.GetNextSibling(execution.FlowStep.Id);
            return nextFlowStep;
        }
    }
}