using Business.DatabaseContext;
using Business.Repository.Interfaces;
using Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class DataService : IDataService, IDisposable
    {
        private readonly IDbContextFactory<InMemoryDbContext> _contextFactory;
        public InMemoryDbContext Query { get => _contextFactory.CreateDbContext(); }
        private InMemoryDbContext _dbContext { get => _contextFactory.CreateDbContext(); }

        public IFlowRepository Flows { get; set; }
        public IFlowStepRepository FlowSteps { get; set; }
        public IFlowParameterRepository FlowParameters { get; set; }
        public IExecutionRepository Executions { get; set; }
        public IAppSettingRepository AppSettings { get; set; }

        public DataService(
            IDbContextFactory<InMemoryDbContext> contextFactory,
            IFlowRepository flowRepository,
            IFlowStepRepository flowStepRepository,
            IFlowParameterRepository flowParameterRepository,
            IExecutionRepository executionRepository,
            IAppSettingRepository appSettingRepository)
        {
            _contextFactory = contextFactory;

            Flows = flowRepository;
            FlowSteps = flowStepRepository;
            FlowParameters = flowParameterRepository;
            Executions = executionRepository;
            AppSettings = appSettingRepository;
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

            using var context = _contextFactory.CreateDbContext();
            context.Entry(model).State = EntityState.Modified;
            context.SaveChanges();
        }

        public async Task UpdateAsync<TEntity>(TEntity model)
        {
            if (model == null)
                return;

            using var context = _contextFactory.CreateDbContext();
            context.Entry(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }


        public void UpdateRange<TEntity>(List<TEntity> models)
        {
            using var context = _contextFactory.CreateDbContext();
            foreach (var model in models)
                if (model != null)
                    context.Entry(model).State = EntityState.Modified;
            context.SaveChanges();
        }

        public async Task UpdateRangeAsync<TEntity>(List<TEntity> models)
        {
            using var context = _contextFactory.CreateDbContext();
            foreach (var model in models)
                if (model != null)
                    context.Entry(model).State = EntityState.Modified;

            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
