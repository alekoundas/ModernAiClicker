using Business.DatabaseContext;
using Business.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Business.Repository.Entities
{
    public class AppSettingRepository : BaseRepository<AppSetting>, IAppSettingRepository
    {
        private readonly IDbContextFactory<InMemoryDbContext> _contextFactory;
        private InMemoryDbContext _dbContext { get => _contextFactory.CreateDbContext(); }

        public AppSettingRepository(IDbContextFactory<InMemoryDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

    }
}
