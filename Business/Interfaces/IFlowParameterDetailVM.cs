using Model.Models;

namespace Business.Interfaces
{
    public interface IFlowParameterDetailVM
    {
        public Task LoadFlowParameterId(int flowParameterId);
        public Task LoadNewFlowParameter(FlowParameter newFlowParameter);
        int GetCurrentEntityId();
        void OnPageExit();
        Task OnSave();
        Task OnCancel();
    }
}
