using Business.DatabaseContext;
using Business.Repository.Interfaces;
using Model.Models;

namespace Business.Repository.Entities
{
    public class FlowParameterRepository : BaseRepository<FlowParameter>, IFlowParameterRepository
    {
        public FlowParameterRepository(InMemoryDbContext dbContext) : base(dbContext)
        {
        }

        public InMemoryDbContext InMemoryDbContext
        {
            get { return Context as InMemoryDbContext; }
        }
    }
}
