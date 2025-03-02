using Business.DatabaseContext;
using Business.Repository.Interfaces;
using Model.Enums;
using Model.Models;

namespace Business.Repository.Entities
{
    public class AppSettingRepository : BaseRepository<AppSetting>, IAppSettingRepository
    {
        public AppSettingRepository(InMemoryDbContext dbContext) : base(dbContext)
        {
        }

        public InMemoryDbContext InMemoryDbContext
        {
            get { return Context as InMemoryDbContext; }
        }
    }
}
