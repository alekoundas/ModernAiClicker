using Business.DatabaseContext;
using Business.Repository.Entities;
using Business.Repository.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Business.Repository
{
    public class BaseDatawork : IBaseDatawork, IDisposable
    {
        private InMemoryDbContext _dbContext { get; set; }
        public InMemoryDbContext Query { get; set; }

        public IFlowRepository Flows { get; set; }
        public IFlowStepRepository FlowSteps { get; set; }
        public IFlowParameterRepository FlowParameters { get; set; }
        public IExecutionRepository Executions { get; set; }

        public BaseDatawork(InMemoryDbContext baseDbContext)
        {
            Query = baseDbContext;
            _dbContext = baseDbContext;
            Flows = new FlowRepository(_dbContext);
            FlowSteps = new FlowStepRepository(_dbContext);
            FlowParameters = new FlowParameterRepository(_dbContext);
            Executions = new ExecutionRepository(_dbContext);
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public void Update<TEntity>(TEntity model)
        {
            if (model == null)
                return;

            _dbContext.Entry(model).State = EntityState.Modified;
        }

        public void UpdateRange<TEntity>(List<TEntity> models)
        {
            foreach (var model in models)
                if (model != null)
                    _dbContext.Entry(model).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
