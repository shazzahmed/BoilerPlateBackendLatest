using Common.DTO.Request;
using System.Linq.Expressions;

namespace Application.ServiceContracts
{
    public interface IBaseService<TBusinessModel, TEntity, TKey>
    {
        Task<TBusinessModel> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null,
            bool useCache = false,
            CancellationToken cancellationToken = default);
        Task<(List<TBusinessModel> items, int count,int lastId)> Get(
            Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null,
            bool useCache = false,
            CancellationToken cancellationToken = default);
        Task<(List<TBusinessModel> items, int count, int lastId)> Get<TResult>(
            int pageNumber = 1,
            int pageSize = 20,
            Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null,
            Expression<Func<TEntity, TResult>> selector = null,
            bool useCache = false,
            CancellationToken cancellationToken = default);
        Task<List<TBusinessModel>> Search(Expression<Func<TEntity, bool>> searchExprn);
        Task<TBusinessModel> Add(TBusinessModel entity);
        Task<List<TBusinessModel>> AddRange(List<TBusinessModel> businessEntities);
        Task Update(TBusinessModel entity);
        Task Delete(TBusinessModel entity);
        Task SoftDelete(TBusinessModel entity);
        Task SoftRestore(TBusinessModel entity);
        Task Delete(TKey id);
        Task DeleteRange(Expression<Func<TEntity, bool>> deleteExpression);
        Task<int> GetCount(Expression<Func<TEntity, bool>> where = null);
    }
}
