using Model.Models;

namespace Business.Interfaces
{
    public interface ICloneService
    {
        Task<FlowStep?> GetFlowStepClone(int flowStepId);
        Flow? GetFlowClone(Flow flow);
        Task<Flow?> GetFlowClone(int flowId);
    }
}
