using Model.Models;

namespace Business.Interfaces
{
    public interface IDetailPage
    {
        public IFlowDetailVM? FlowViewModel { get; set; }
        public IFlowStepDetailVM? FlowStepViewModel { get; set; }
        public IFlowParameterDetailVM? FlowParameterViewModel { get; set; }
        public IExecutionViewModel? ExecutionViewModel { get; set; }
    }
}
