using Model.Models;

namespace Business.Interfaces
{
    public interface IFlowStepDetailVM
    {
        public event Action<int> OnSave;

        public Task LoadFlowStepId(int flowStepId);
        public Task LoadNewFlowStep(FlowStep newFlowStep);
    }
}
