using Business.DatabaseContext;
using Business.Repository.Interfaces;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Business.Repository.Entities
{
    public class ExecutionRepository : BaseRepository<Execution>, IExecutionRepository
    {
        public ExecutionRepository(InMemoryDbContext dbContext) : base(dbContext)
        {
        }

        public InMemoryDbContext InMemoryDbContext
        {
            get { return Context as InMemoryDbContext; }
        }

        public async Task<List<Execution>> GetAllParentLoopExecutions(int executionId)
        {
            List<Execution> parentLoopExecutions = new List<Execution>();
            Execution? currentExecution = await InMemoryDbContext.Executions.AsNoTracking().FirstAsync(x => x.Id == executionId);

            while (currentExecution.ParentLoopExecutionId != null)
            {
                parentLoopExecutions.Add(currentExecution);

                currentExecution = await InMemoryDbContext.Executions
                    .AsNoTracking()
                    .Include(x => x.FlowStep)
                    .FirstAsync(x => x.Id == currentExecution.ParentLoopExecutionId.Value);
            }

            return parentLoopExecutions;
        }
    }
}
