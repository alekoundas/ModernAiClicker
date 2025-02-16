using Model.Models;

namespace Business.Interfaces
{
    public interface IFlowStepDetailVM
    {
        public Task LoadFlowStepId(int flowStepId);
        public Task LoadNewFlowStep(FlowStep newFlowStep);
    }
}
