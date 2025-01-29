using DataAccess.Repository.Interface;
using Model.Models;

namespace Business.Repository.Interfaces
{
    public interface IExecutionRepository : IBaseRepository<Execution>
    {
        Task<List<Execution>> GetAllParentLoopExecutions(int executionId);
    }
}
