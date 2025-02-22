using Business.DatabaseContext;
using Business.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
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

        public async Task<FlowParameter> GetIsNewSibling(int flowParameterId)
        {
            return await InMemoryDbContext.FlowParameters
                .Where(x => x.Id == flowParameterId)
                .Select(x => x.ChildrenFlowParameters.First(y => y.Type == FlowParameterTypesEnum.NEW))
                .FirstAsync();
        }
    }
}
