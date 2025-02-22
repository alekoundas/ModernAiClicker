using Model.Models;

namespace Business.Factories
{
    public interface IExecutionWorker
    {
        Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution parentExecution, Execution latestParentExecution);
        Task<Execution> CreateExecutionModelFlow(int id, Execution? parentExecution);
        Task ExecuteFlowStepAction(Execution execution);
        Task<FlowStep?> GetNextChildFlowStep(Execution execution);
        Task<FlowStep?> GetNextSiblingFlowStep(Execution execution);
        Task SetExecutionModelStateRunning(Execution execution);
        Task SetExecutionModelStateComplete(Execution execution);
        Task SaveToDisk(Execution execution);
        void ClearEntityFrameworkChangeTracker();
    }
}
