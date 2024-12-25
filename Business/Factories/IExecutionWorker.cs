using Model.Enums;
using Model.Models;
using System.Collections.ObjectModel;

namespace Business.Factories
{
    public interface IExecutionWorker
    {
        Task<Execution> CreateExecutionModel(FlowStep flowStep, Execution? parentExecution);
        Task<Execution> CreateExecutionModelFlow(int id, Execution? parentExecution);
        Task ExecuteFlowStepAction(Execution execution);
        Task<FlowStep?> GetNextChildFlowStep(Execution execution);
        Task<FlowStep?> GetNextSiblingFlowStep(Execution execution);
        Task SetExecutionModelStateRunning(Execution execution);
        Task SetExecutionModelStateComplete(Execution execution);
        Task ExpandAndSelectFlowStep(Execution execution, ObservableCollection<Flow> treeviewFlows);
        Task SaveToJson();
        //void RefreshUI();
        Task SaveToDisk(Execution execution);
        void ClearEntityFrameworkChangeTracker();

    }
}
