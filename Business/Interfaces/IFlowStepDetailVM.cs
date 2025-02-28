using Model.Models;

namespace Business.Interfaces
{
    public interface IFlowStepDetailVM
    {
        event Action<int> OnSave;

        Task LoadFlowStepId(int flowStepId);
        Task LoadNewFlowStep(FlowStep newFlowStep);
        void OnPageExit();

    }
}
