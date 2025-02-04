using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Business.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        //Excell reflection call
        Task<List<TEntity>> GetAllAsync();
        List<TEntity> GetAll();
        IQueryable<TEntity> Query { get; }
        Task<TEntity?> FindAsync(int id);
        Task<int> CountAllAsync();
        Task<int> CountAllAsyncFiltered(Expression<Func<TEntity, bool>> filter);
        Task<List<TResult>> SelectAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector);
        Task<List<TResult>> SelectAllAsyncFiltered<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);

        Task<List<TEntity>> GetPaggingWithFilter(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderingInfo,
            Expression<Func<TEntity, bool>>? filter = null,
            List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>? includes = null,
            int pageSize = 10,
            int pageIndex = 1);
        Task<List<TEntity>> GetWithFilter(
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderingInfo,
           Expression<Func<TEntity, bool>>? filter = null,
           List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>? includes = null);
        IQueryable<TEntity> GetWithFilterQueryable(
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderingInfo,
           Expression<Func<TEntity, bool>>? filter = null,
           List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>? includes = null,
           int pageSize = 10,
           int pageIndex = 1);

        Task<List<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter);

        int Count(Expression<Func<TEntity, bool>> predicate);

        bool Any(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> expression);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> filter, List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>? includes = null);
        void Select(Func<TEntity, TEntity> predicate);
        void Select(Expression<Func<TEntity, bool>> predicate);
    }
}
