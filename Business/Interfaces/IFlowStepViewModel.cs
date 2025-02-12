using Model.Models;

namespace Business.Interfaces
{
    public interface IFlowStepViewModel
    {
        public Task LoadFlowStepId(int flowStepId);
        public void LoadNewFlowStep(FlowStep newFlowStep);
    }
}
