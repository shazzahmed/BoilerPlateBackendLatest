using Application.ServiceContracts;
using IdentityModel;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Services
{
    public class DistributedMemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _memoryCache;

        public DistributedMemoryCacheProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task<T> GetOrCreateCacheAsync<T>(string key, string tag, Func<Task<T>> factory, CancellationToken cancellationToken)
        {
            if (_memoryCache.TryGetValue(key, out T cached))
            {
                Console.WriteLine($"[MEMORY CACHE] HIT for key: {key}");
                return cached!;
            }
            Console.WriteLine($"[MEMORY CACHE] MISS for key: {key}. Executing factory...");
            var result = await factory();
            string json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var cleanedValue = JsonConvert.DeserializeObject<T>(json);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            _memoryCache.Set(key, cleanedValue, cacheEntryOptions);
            return cleanedValue;
        }
        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, IEnumerable<string> tags = null, CancellationToken cancellationToken = default)
        {
            string json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var cleanedValue = JsonConvert.DeserializeObject<T>(json);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(60)
            };

            _memoryCache.Set(key, cleanedValue, cacheEntryOptions);
            TrackKey(typeof(T).Name, key);
            return Task.CompletedTask;
        }
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[CACHE] Removing key: {key}");
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
        public Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
        {
            _memoryCache.Get(tag);
            Console.WriteLine($"[CACHE] RemoveByTag is not supported in IMemoryCache.");
            return Task.CompletedTask;
        }
        public void TrackKey(string entityName, string cacheKey)
        {
            var indexKey = $"cache_keys:{entityName}";
            List<string> keys = _memoryCache.Get<List<string>>(indexKey) ?? new List<string>();

            if (!keys.Contains(cacheKey))
            {
                keys.Add(cacheKey);
                _memoryCache.Set(indexKey, keys);
            }
        }

    }

}
