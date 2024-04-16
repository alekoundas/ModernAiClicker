using Business.DatabaseContext;
using Business.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository.Interface
{
    public interface IBaseDatawork : IDisposable
    {
        InMemoryDbContext Query { get; }

        IFlowRepository Flows { get; set; }
        IFlowStepRepository FlowSteps { get; set; }

        Task<int> SaveChangesAsync();
        int SaveChanges();
        void Update<TEntity>(TEntity model);
        void UpdateRange<TEntity>(List<TEntity> models);
    }
}
