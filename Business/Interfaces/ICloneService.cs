using Model.Models;

namespace Business.Interfaces
{
    public interface ICloneService
    {
        Task<FlowStep?> GetFlowStepClone(int flowStepId);
        Task<Flow?> GetFlowClone(int flowId);
    }
}
