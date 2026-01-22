using Application.ServiceContracts;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;

namespace Infrastructure.Services.Services
{
    public class DistributedHybridCacheProvider : ICacheProvider
    {
        private readonly HybridCache _hybridCache;

        public DistributedHybridCacheProvider(HybridCache hybridCache)
        {
            _hybridCache = hybridCache;
        }
        public async Task<T> GetOrCreateCacheAsync<T>(string key, string tag, Func<Task<T>> factory, CancellationToken cancellationToken)
        {
            var options = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(60),
                LocalCacheExpiration = TimeSpan.FromMinutes(60)
            };

            return await _hybridCache.GetOrCreateAsync<object, T>(
                key,
                state: null,
                async (_, token) => {
                    Console.WriteLine($"[CACHE] MISS - Executing factory for key: {key}");
                    var result = await factory();
                    
                    // Handle reference loops by cleaning the object with System.Text.Json
                    if (result != null && HasNavigationProperties(typeof(T)))
                    {
                        try 
                        {
                            var jsonOptions = new JsonSerializerOptions
                            {
                                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                                MaxDepth = 32
                            };
                            
                            var json = JsonSerializer.Serialize(result, jsonOptions);
                            return JsonSerializer.Deserialize<T>(json, jsonOptions);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[CACHE] JSON serialization warning for {typeof(T).Name}: {ex.Message}");
                            // If serialization fails, return the original result
                            return result;
                        }
                    }
                    
                    return result;
                },
                options: options,
                tags: new[] { tag },
                CancellationToken.None
            );
        }
        
        private static bool HasNavigationProperties(Type type)
        {
            // Check if it's likely an entity type that could have reference loops
            var typeName = type.Name.ToLower();
            return typeName.Contains("student") || typeName.Contains("user") || typeName.Contains("basemodel") ||
                   typeName.Contains("category") || typeName.Contains("section") || typeName.Contains("class") || 
                   typeName.Contains("school") || type.Namespace?.Contains("Domain.Entities") == true ||
                   type.Namespace?.Contains("Common.DTO") == true;
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, IEnumerable<string> tags = null, CancellationToken cancellationToken = default)
        {
            var options = new HybridCacheEntryOptions
            {
                Expiration = expiration ?? TimeSpan.FromHours(24),
                LocalCacheExpiration = TimeSpan.FromHours(24)
            };

            // Clean the value if it has navigation properties
            T cleanValue = value;
            if (value != null && HasNavigationProperties(typeof(T)))
            {
                try 
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        MaxDepth = 32
                    };
                    
                    var json = JsonSerializer.Serialize(value, jsonOptions);
                    cleanValue = JsonSerializer.Deserialize<T>(json, jsonOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CACHE] JSON serialization warning in SetAsync for {typeof(T).Name}: {ex.Message}");
                    // If serialization fails, use the original value
                }
            }

            await _hybridCache.SetAsync(key, cleanValue, options, tags, cancellationToken);
        }
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[CACHE] Removing key: {key}");
            await _hybridCache.RemoveAsync(key, cancellationToken);
        }
        public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[CACHE] Evicting by tag: {tag}");
            await _hybridCache.RemoveByTagAsync(tag, cancellationToken);
        }

    }

}
