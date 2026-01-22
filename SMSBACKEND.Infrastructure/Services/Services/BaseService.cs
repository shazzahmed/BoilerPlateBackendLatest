using AutoMapper;
using Domain.RepositoriesContracts;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.Utilities.StaticClasses;
using Domain.Entities;
using System.Threading;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class BaseService<TBusinessModel, TEntity, TKey> : IBaseService<TBusinessModel, TEntity, TKey>
        where TBusinessModel : class 
        where TEntity : class
    {
        protected readonly IBaseRepository<TEntity, TKey> repository;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;
        protected readonly ILogger _logger;
        
        public BaseService(
            IMapper mapper,
            IBaseRepository<TEntity, TKey> baseRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider = null,
            ILogger logger = null
            )
        {
            this.repository = baseRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            _cacheProvider = cacheProvider;
            _sseService = sseService;
            _logger = logger;
        }

        public virtual async Task<TBusinessModel> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null,
            bool useCache = false,
            CancellationToken cancellationToken = default)
        {
            var dataEntity = await repository.FirstOrDefaultAsync(where, orderBy, includeProperties);
            return mapper.Map<TEntity, TBusinessModel>(dataEntity);
        }
        public virtual async Task<int> GetCount(Expression<Func<TEntity, bool>> where = null)
        {
            var count = await repository.GetCountAsync(where);
            return count;
        }
        public virtual async Task<(List<TBusinessModel> items, int count,int lastId)> Get(
            Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null,
            bool useCache = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = CacheKeyBuilder<TEntity>.GetKey(where);
                if (useCache && _cacheProvider != null)
                {
                    return await _cacheProvider.GetOrCreateCacheAsync(
                        cacheKey,
                        typeof(TEntity).Name,
                        async () =>
                    {
                        var entityList = await repository.GetAsync(where, orderBy, includeProperties);
                        var query = entityList.AsQueryable();
                        var count = query.Count();
                        var entities = query.ToList();
                        var items = mapper.Map<List<TEntity>, List<TBusinessModel>>(entities);
                        var lastId = await repository.GetLastIdAsync();
                        return (items, count, lastId);
                    },
                        cancellationToken);
                }
                var entityList = await repository.GetAsync(where, orderBy, includeProperties);
                var query = entityList.AsQueryable();
                var count = query.Count();
                var entities = query.ToList();
                var items = mapper.Map<List<TEntity>, List<TBusinessModel>>(entities);
                var lastId = await repository.GetLastIdAsync();
                return (items, count, lastId);
            }
            catch (Exception ex)
            {
                // ✅ Log error with entity type context
                _logger?.LogError(ex, "Error getting {EntityType} entities", typeof(TEntity).Name);
                throw;
            }
        }
        public virtual async Task<(List<TBusinessModel> items, int count, int lastId)> Get<TResult>(
            int pageNumber = 1,
            int pageSize = 20,
            Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeProperties = null,
            Expression<Func<TEntity, TResult>> selector = null,
            bool useCache = false,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeyBuilder<TEntity>.GetKey(where);

            if (useCache && _cacheProvider != null)
            {
                return await _cacheProvider.GetOrCreateCacheAsync(
                    cacheKey,
                    typeof(TEntity).Name,
                    async () =>
                    {
                        var entityList = await repository.GetAsync(where, orderBy, includeProperties, selector);
                        var query = entityList.AsQueryable();
                        var count = query.Count();
                        var pagedEntities = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                        var items = mapper.Map<List<TResult>, List<TBusinessModel>>(pagedEntities);
                        var lastId = await repository.GetLastIdAsync();
                        return (items, count, lastId);
                    },
                    cancellationToken);
            }

            var entityList = await repository.GetAsync(where, orderBy, includeProperties, selector);
            var query = entityList.AsQueryable();
            var count = query.Count();
            var pagedEntities = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            var items = mapper.Map<List<TResult>, List<TBusinessModel>>(pagedEntities);
            var lastId = await repository.GetLastIdAsync();
            return (items, count, lastId);
        }
        public async virtual Task<List<TBusinessModel>> Search(Expression<Func<TEntity, bool>> searchExprn)
        {
            var query = repository.Get();
            if (ChecksHelper.NotNull(searchExprn))
            {
                query = query.Where(searchExprn);
            }
            return mapper.Map<List<TEntity>, List<TBusinessModel>>(query.ToList());
        }
        public virtual async Task<TBusinessModel> Add(TBusinessModel businessEntity)
        {
            var dataEntity = mapper.Map<TBusinessModel, TEntity>(businessEntity);
            dataEntity = await repository.AddAsync(dataEntity);
            await SaveChanges();
            
            var nameProp = typeof(TEntity).GetProperty("Name");
            var nameValue = nameProp?.GetValue(dataEntity)?.ToString() ?? "Unnamed";
            var idProp = typeof(TEntity).GetProperty("Id");
            var idValue = idProp?.GetValue(dataEntity)?.ToString() ?? "noId";
            
            // Use enhanced broadcasting
            await _sseService.BroadcastEntityCreatedAsync(nameValue, typeof(TEntity).Name, idValue);
            
            // Remove cache for current entity
            if (_cacheProvider!= null)
                await _cacheProvider.RemoveByTagAsync(typeof(TEntity).Name);

            // Remove cache for dependent entities
            if (EntityDependencyMap.Dependencies.TryGetValue(typeof(TEntity).Name, out var dependents))
            {
                foreach (var dependent in dependents)
                    await _cacheProvider.RemoveByTagAsync(dependent);
            }
            businessEntity = mapper.Map<TEntity, TBusinessModel>(dataEntity);
            return businessEntity;
        }

        public virtual async Task<List<TBusinessModel>> AddRange(List<TBusinessModel> businessEntities)
        {
            var dataEntities = mapper.Map<List<TBusinessModel>, List<TEntity>>(businessEntities);
            await repository.AddAsync(dataEntities);
            await SaveChanges();
            if (_cacheProvider != null)
                await _cacheProvider.RemoveByTagAsync(typeof(TEntity).Name);

            // Remove cache for dependent entities
            if (EntityDependencyMap.Dependencies.TryGetValue(typeof(TEntity).Name, out var dependents))
            {
                foreach (var dependent in dependents)
                    await _cacheProvider.RemoveByTagAsync(dependent);
            }
            businessEntities = mapper.Map<List<TEntity>, List<TBusinessModel>>(dataEntities);
            return businessEntities;
        }
        
        public virtual async Task Update(TBusinessModel businessEntity)
        {
            var dataEntity = mapper.Map<TBusinessModel, TEntity>(businessEntity);
            await repository.UpdateAsync(dataEntity);
            await SaveChanges();
            
            var nameProp = typeof(TEntity).GetProperty("Name");
            var nameValue = nameProp?.GetValue(dataEntity)?.ToString() ?? "Unnamed";
            var idProp = typeof(TEntity).GetProperty("Id");
            var idValue = idProp?.GetValue(dataEntity)?.ToString() ?? "noId";
            
            // Use enhanced broadcasting
            await _sseService.BroadcastEntityUpdatedAsync(nameValue, typeof(TEntity).Name, idValue);
            
            // Remove cache for current entity
            if (_cacheProvider != null)
                await _cacheProvider.RemoveByTagAsync(typeof(TEntity).Name);

            // Remove cache for dependent entities
            if (EntityDependencyMap.Dependencies.TryGetValue(typeof(TEntity).Name, out var dependents))
            {
                foreach (var dependent in dependents)
                    await _cacheProvider.RemoveByTagAsync(dependent);
            }
        }

        public virtual async Task Delete(TBusinessModel businessEntity)
        {
            var dataEntity = mapper.Map<TBusinessModel, TEntity>(businessEntity);
            await repository.DeleteAsync(dataEntity);
            await SaveChanges();
        }
        
        public virtual async Task SoftDelete(TBusinessModel businessEntity)
        {
            var dataEntity = mapper.Map<TBusinessModel, TEntity>(businessEntity);
            var property = dataEntity.GetType().GetProperty("IsDeleted");
            var propertyValue = (bool)property.GetValue(dataEntity);
            var syncProperty = dataEntity.GetType().GetProperty("SyncStatus");
            var syncPropertyValue = (string)syncProperty.GetValue(dataEntity);
            if (!propertyValue && syncPropertyValue != "delete")
            {
                property.SetValue(dataEntity, true);
                syncProperty.SetValue(dataEntity, "delete");
                await repository.UpdateAsync(dataEntity);
                await SaveChanges();
                
                var nameProp = typeof(TEntity).GetProperty("Name");
                var nameValue = nameProp?.GetValue(dataEntity)?.ToString() ?? "Unnamed";
                var idProp = typeof(TEntity).GetProperty("Id");
                var idValue = idProp?.GetValue(dataEntity)?.ToString() ?? "noId";
                
                // Use enhanced broadcasting
                await _sseService.BroadcastEntityDeletedAsync(nameValue, typeof(TEntity).Name, idValue);
                
                // Remove cache for current entity
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync(typeof(TEntity).Name);

                // Remove cache for dependent entities
                    if (EntityDependencyMap.Dependencies.TryGetValue(typeof(TEntity).Name, out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }

                }
            }
        }

        public virtual async Task SoftRestore(TBusinessModel businessEntity)
        {
            var dataEntity = mapper.Map<TBusinessModel, TEntity>(businessEntity);
            var property = dataEntity.GetType().GetProperty("IsDeleted");
            var propertyValue = (bool)property.GetValue(dataEntity);
            var syncProperty = dataEntity.GetType().GetProperty("SyncStatus");
            var syncPropertyValue = (string)syncProperty.GetValue(dataEntity);
            if (propertyValue && syncPropertyValue == "delete")
            {
                property.SetValue(dataEntity, false);
                syncProperty.SetValue(dataEntity, "synced");
                await repository.UpdateAsync(dataEntity);
                await SaveChanges();
                
                var nameProp = typeof(TEntity).GetProperty("Name");
                var nameValue = nameProp?.GetValue(dataEntity)?.ToString() ?? "Unnamed";
                var idProp = typeof(TEntity).GetProperty("Id");
                var idValue = idProp?.GetValue(dataEntity)?.ToString() ?? "noId";
                
                // Use enhanced broadcasting
                await _sseService.BroadcastEntityRestoredAsync(nameValue, typeof(TEntity).Name, idValue);
                
                // Remove cache for current entity
                if (_cacheProvider != null)
                    await _cacheProvider.RemoveByTagAsync(typeof(TEntity).Name);

                // Remove cache for dependent entities
                if (EntityDependencyMap.Dependencies.TryGetValue(typeof(TEntity).Name, out var dependents))
                {
                    foreach (var dependent in dependents)
                        await _cacheProvider.RemoveByTagAsync(dependent);
                }
            }
        }
        public virtual async Task Delete(TKey id)
        {
            await repository.DeleteAsync(id);
            await SaveChanges();
        }
        public virtual async Task DeleteRange(Expression<Func<TEntity, bool>> deleteExpression)
        {
            await repository.DeleteAsync(deleteExpression);
            await SaveChanges();
        }

        public async Task SaveChanges()
        {
            if (ChecksHelper.NotNull(unitOfWork))
            {
                await unitOfWork.SaveChangesAsync();
            }
        }

        private IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, FilterParams filters)
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
                    } else
                    {
                        if (constant.Type != typeof(DateTime))
                        {
                            constant = Expression.Constant(DateTime.Parse(constant.ToString()));
                        }

                    }
                } else
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