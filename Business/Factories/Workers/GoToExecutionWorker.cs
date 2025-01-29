using Business.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Business.Factories.Workers
{
    public class GoToExecutionWorker : CommonExecutionWorker, IExecutionWorker
    {
        private readonly IBaseDatawork _baseDatawork;

        public GoToExecutionWorker(IBaseDatawork baseDatawork, ISystemService systemService) : base(baseDatawork, systemService)
        {
            _baseDatawork = baseDatawork;
        }

        public Task ExecuteFlowStepAction(Execution execution)
        {
            return Task.CompletedTask;
        }

        public async Task<FlowStep?> GetNextSiblingFlowStep(Execution execution)
        {
            if (execution.FlowStep == null)
                return await Task.FromResult<FlowStep?>(null);
            if (!execution.FlowStep.ParentTemplateSearchFlowStepId.HasValue)
                return await Task.FromResult<FlowStep?>(null);

            FlowStep? nextFlowStep = await _baseDatawork.Query.FlowSteps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == execution.FlowStep.ParentTemplateSearchFlowStepId.Value);

            //TODO return error message 
            if (nextFlowStep == null)
                return null;

            return nextFlowStep;
        }
    }
}
