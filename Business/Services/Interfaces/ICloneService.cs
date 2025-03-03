using Model.Models;

namespace Business.Services.Interfaces
{
    public interface ICloneService
    {
        Task<FlowStep?> GetFlowStepClone(int flowStepId);
        Flow? GetFlowClone(Flow flow);
        Task<Flow?> GetFlowClone(int flowId);
    }
}
