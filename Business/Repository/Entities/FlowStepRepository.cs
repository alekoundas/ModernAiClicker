using Business.DatabaseContext;
using Business.Repository.Interfaces;
using DataAccess.Repository;
using Model.Models;

namespace Business.Repository.Entities
{
    public class FlowStepRepository : BaseRepository<FlowStep>, IFlowStepRepository
    {
        public FlowStepRepository(InMemoryDbContext dbContext) : base(dbContext)
        {
        }

        public InMemoryDbContext InMemoryDbContext
        {
            get { return Context as InMemoryDbContext; }
        }
    }
}
