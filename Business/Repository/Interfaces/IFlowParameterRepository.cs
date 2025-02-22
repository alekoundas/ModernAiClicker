using Model.Enums;
using Model.Models;

namespace Business.Repository.Interfaces
{
    public interface IFlowParameterRepository : IBaseRepository<FlowParameter>
    {
        //Task<FlowStep> GetIsNewSibling(int flowStepId);
        //Task<List<FlowStep>> GetSiblings(int flowStepId);
        //Task<FlowStep?> GetNextSibling(int flowStepId);
        //Task<FlowStep?> GetNextChild(int flowStepId, ExecutionResultEnum? resultEnum);
        //Task<FlowStep?> GetFlowStepClone(int flowStepId);
        //Task<FlowStep> LoadAllExpandedChildren(FlowStep flowStep);
    }
}
