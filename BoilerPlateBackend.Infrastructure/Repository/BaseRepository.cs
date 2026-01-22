using AutoMapper;
using Common.Helpers;
using System.Linq.Expressions;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Microsoft.EntityFrameworkCore;
using Common.DTO.Request;

namespace Infrastructure.Repository
{
    public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
    {
        private ISqlServerDbContext dbContext;
        private DbSet<TEntity> dbSet;
        protected readonly IMapper mapper;

        public DbSet<TEntity> DbSet
        {
            get { return dbSet; }
            private set { dbSet = value; }
        }

        public ISqlServerDbContext DbContext
        {
            get { return dbContext; }
            private set { dbContext = value; }
        }

        public BaseRepository(ISqlServerDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<TEntity>();
        }
        public BaseRepository(
            IMapper mapper,
            ISqlServerDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            dbSet = dbContext.Set<TEntity>();
        }
        public virtual async Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>>? where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking().IgnoreQueryFilters();

            if (ChecksHelper.NotNull(where))
                query = query.Where(where);
            if (includeProperties is not null)
                query = includeProperties(query);
            if (ChecksHelper.NotNull(orderBy))
                return await orderBy(query).FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync();
        }
        public virtual async Task<TOut> FirstOrDefaultAsync<TOut>(
            Expression<Func<TEntity, bool>> where,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null,
            Expression<Func<TEntity, TOut>> selector = null)
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking();

            if (ChecksHelper.NotNull(where))
                query = query.Where(where);

            if (includeProperties is not null)
                query = includeProperties(query);

            if (selector is not null)
                return await query.Select(selector).FirstOrDefaultAsync();

            return (TOut)(object)(await query.FirstOrDefaultAsync());
        }

        public virtual IQueryable<TEntity> Get()
        {
            return dbSet.AsQueryable();
        }
        public virtual async Task<List<TOut>> GetAsync<TOut>(
            Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null,
            Expression<Func<TEntity, TOut>> selector = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (ChecksHelper.NotNull(where))
                query = query.Where(where);

            if (includeProperties is not null)
                query = includeProperties(query);

            if (ChecksHelper.NotNull(orderBy))
                query = orderBy(query);

            if (selector is not null)
                return await query.AsNoTracking().Select(selector).ToListAsync();

            // If no selector is provided, return full entity cast to TOut
            return await query.AsNoTracking()
                              .Select(x => (TOut)(object)x)
                              .ToListAsync();
        }
        public virtual async Task<TEntity> GetAsync(TKey id)
        {
            var entity = await dbSet.FindAsync(id);
            dbContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }


        public virtual async Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (ChecksHelper.NotNull(where))
                query = query.Where(where);
            if (includeProperties is not null)
                query = includeProperties(query);
            if (ChecksHelper.NotNull(orderBy))              
                return await orderBy(query).AsNoTracking().ToListAsync();

            return await query.AsNoTracking().ToListAsync();
        }

        private TEntity Add(TEntity entity)
        {
            var entry = dbSet.Add(entity).Entity;
            return entry;
        }

        public virtual Task<TEntity> AddAsync(TEntity entity)
        {
            return Task.FromResult(Add(entity));
        }

        public virtual void Add(List<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual async Task AddAsync(List<TEntity> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        private void Update(TEntity entityToUpdate)
        {
            dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => Update(entity));
        }

        private void Update(List<TEntity> entitiesToUpdate)
        {
            foreach (var entity in entitiesToUpdate)
            {
                Update(entity);
            }
        }

        public virtual async Task UpdateAsync(List<TEntity> entities)
        {
            await Task.Run(() => Update(entities));
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                Delete(entityToDelete);
            }
        }

        private void Delete(TEntity entity)
        {
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            return Task.Run(() => Delete(entity));
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> where)
        {
            dbSet.RemoveRange(await dbSet.Where(where).ToListAsync());
        }
        public async Task<int> GetLastIdAsync(string idPropertyName = "Id")
        {
            // Verify that the property exists on the entity type
            var entityType = dbSet.EntityType; // works if you're in DbContext
            var prop = entityType.FindProperty(idPropertyName);

            if (prop == null)
            {
                // Property does not exist
                return 0;
            }

            return await dbSet
                .OrderByDescending(e => EF.Property<int>(e, idPropertyName))
                .Select(e => EF.Property<int>(e, idPropertyName))
                .FirstOrDefaultAsync();
        }
        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> where = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (ChecksHelper.NotNull(where))
                query = query.Where(where);

            return await query.CountAsync();
        }
        public IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, FilterParams filters)
        {
            foreach (var filter in filters.Filters)
            {
                var parameter = Expression.Parameter(typeof(TEntity), "e");
                var property = Expression.Property(parameter, filter.PropertyName);
                var constant = Expression.Constant(filter.Value);

                if (property.Type == typeof(DateTime))
                {
                    if (filter.Operator == "BetweenMonth" && DateTime.TryParse(filter.Value?.ToString(), out DateTime date))
                    {
                        // Calculate the first and last days of the month
                        var startOfMonth = DateTimeHelper.GetFirstDayOfMonth(date);
                        var endOfMonth = DateTimeHelper.GetLastDayOfMonth(date);

                        var startConstant = Expression.Constant(startOfMonth, typeof(DateTime));
                        var endConstant = Expression.Constant(endOfMonth, typeof(DateTime));

                        // Create expressions for comparison
                        var afterOrEqual = Expression.GreaterThanOrEqual(property, startConstant);
                        var beforeOrEqual = Expression.LessThanOrEqual(property, endConstant);

                        // Combine conditions
                        var combinedExpression = Expression.AndAlso(afterOrEqual, beforeOrEqual);

                        var expr = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
                        query = query.Where(expr);
                    }
                    else
                    {
                        if (constant.Type != typeof(DateTime))
                        {
                            constant = Expression.Constant(DateTime.Parse(constant.ToString()));
                        }

                    }
                }
                else
                {
                    if (property.Type == typeof(int))
                    {
                        if (constant.Type != typeof(int))
                        {
                            constant = Expression.Constant(int.Parse(constant.ToString()));
                        }
                    }
                    // Add more type checks as needed for other types


                    Expression expression = filter.Operator switch
                    {
                        "Equals" => Expression.Equal(property, constant),
                        "GreaterThan" => Expression.GreaterThan(property, constant),
                        "LessThan" => Expression.LessThan(property, constant),
                        "Contains" => Expression.Call(property, "Contains", null, constant),
                        _ => throw new NotImplementedException($"Operator '{filter.Operator}' is not implemented.")
                    };

                    var lambda = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
                    query = query.Where(lambda);

                }
            }

            return query;
        }

    }
}