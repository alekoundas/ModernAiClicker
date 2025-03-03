using Business.DatabaseContext; // Replace with your actual namespace
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Business.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly IDbContextFactory<InMemoryDbContext> _contextFactory;

        public BaseRepository(IDbContextFactory<InMemoryDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IQueryable<TEntity> Query { get => _contextFactory.CreateDbContext().Set<TEntity>(); }

        public async Task<List<TEntity>> GetAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().ToListAsync();
        }

        public List<TEntity> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<TEntity>().ToList();
        }

        public async Task<List<TResult>> SelectAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().Select(selector).ToListAsync();
        }

        public async Task<List<TResult>> SelectAllAsyncFiltered<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().Where(predicate).Select(selector).ToListAsync();
        }

        public async Task<int> CountAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().CountAsync();
        }

        public async Task<int> CountAllAsyncFiltered(Expression<Func<TEntity, bool>> selector)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().Where(selector).CountAsync();
        }

        public async Task<List<TEntity>> GetPaggingWithFilter(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderingInfo,
            Expression<Func<TEntity, bool>>? filter,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>? includes = null,
            int pageSize = 10,
            int pageIndex = 1)
        {
            using var context = _contextFactory.CreateDbContext();
            var qry = context.Set<TEntity>().AsQueryable();

            if (includes != null)
                foreach (var include in includes)
                    qry = include(qry);

            if (filter != null)
                qry = qry.Where(filter);

            if (orderingInfo != null)
                qry = orderingInfo(qry);

            if (pageSize != -1 && pageSize != 0)
                qry = qry.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return await qry.ToListAsync();
        }

        public async Task<List<TEntity>> GetWithFilter(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderingInfo,
            Expression<Func<TEntity, bool>>? filter,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>? includes = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var qry = context.Set<TEntity>().AsQueryable();

            if (includes != null)
                foreach (var include in includes)
                    qry = include(qry);

            if (filter != null)
                qry = qry.Where(filter);

            if (orderingInfo != null)
                qry = orderingInfo(qry);

            return await qry.ToListAsync();
        }

        public IQueryable<TEntity> GetWithFilterQueryable(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderingInfo,
            Expression<Func<TEntity, bool>>? filter,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>? includes = null,
            int pageSize = 10,
            int pageIndex = 1)
        {
            var context = _contextFactory.CreateDbContext(); // Note: Caller must dispose
            var qry = context.Set<TEntity>().AsQueryable();

            if (includes != null)
                foreach (var include in includes)
                    qry = include(qry);

            if (filter != null)
                qry = qry.Where(filter);

            if (orderingInfo != null)
                qry = orderingInfo(qry);

            return qry; // Warning: Caller is responsible for disposal
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<TEntity>().Count(predicate);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<TEntity>().Any(predicate);
        }

        public void Add(TEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().Add(entity);
            context.SaveChanges();
        }
        public async Task AddAsync(TEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Set<TEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public void Select(Expression<Func<TEntity, bool>> predicate)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().Select(predicate); // Note: This doesn't persist; likely a mistake
        }

        public void Select(Func<TEntity, TEntity> predicate)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().Select(predicate); // Note: This doesn't persist; likely a mistake
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().AddRange(entities);
            context.SaveChanges();
        }
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().AddRangeAsync(entities);
            context.SaveChangesAsync();
        }

        public void Remove(TEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().Remove(entity);
            context.SaveChanges();
        }
        public async Task RemoveAsync(TEntity entity)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().Remove(entity);
            context.SaveChangesAsync();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().RemoveRange(entities);
            context.SaveChanges();
        }
        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TEntity>().RemoveRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task<TEntity?> FindAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> filter,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>? includes = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var qry = context.Set<TEntity>().AsQueryable();

            if (includes != null)
                foreach (var include in includes)
                    qry = include(qry);

            return await qry.FirstAsync(filter);
        }

        public async Task<List<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<TEntity>().Where(filter).ToListAsync();
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<TEntity>().Where(expression).ToList();
        }
    }
}