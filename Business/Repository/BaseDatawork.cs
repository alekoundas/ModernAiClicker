using Business.DatabaseContext;
using Business.Repository.Entities;
using Business.Repository.Interfaces;
using DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataAccess
{
    public class BaseDatawork : IBaseDatawork
    {
        public InMemoryDbContext Query { get; }
        private InMemoryDbContext _dbContext { get; }

        public IFlowRepository Flows { get; set; }
        public IFlowStepRepository FlowSteps { get; set; }

        public BaseDatawork(InMemoryDbContext baseDbContext)
        {
            Query = baseDbContext;
            _dbContext = baseDbContext;
            Flows = new FlowRepository(_dbContext);
            FlowSteps = new FlowStepRepository(_dbContext);
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
            _dbContext.Entry(model).State = EntityState.Modified;
        }

        public void UpdateRange<TEntity>(List<TEntity> models)
        {
            foreach (var model in models)
                _dbContext.Entry(model).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
