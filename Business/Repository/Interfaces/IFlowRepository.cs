using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.Repository.Interfaces
{
    public interface IFlowRepository : IBaseRepository<Flow>
    {
        Task<FlowStep> GetIsNewSibling(int id);
    }
}
