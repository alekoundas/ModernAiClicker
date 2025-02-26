using Business.DatabaseContext;
using Business.Repository.Interfaces;

namespace DataAccess.Repository.Interface
{
    public interface IDataService : IDisposable
    {
        InMemoryDbContext Query { get; }

        IFlowRepository Flows { get; set; }
        IFlowStepRepository FlowSteps { get; set; }
        IFlowParameterRepository FlowParameters { get; set; }
        IExecutionRepository Executions { get; set; }

        Task<int> SaveChangesAsync();
        int SaveChanges();
        void Update<TEntity>(TEntity model);
        void UpdateRange<TEntity>(List<TEntity> models);

    }
}
