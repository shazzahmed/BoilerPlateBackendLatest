using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceContracts
{
    public interface ICacheProvider
    {
        Task<T> GetOrCreateCacheAsync<T>(string key, string tag, Func<Task<T>> factory, CancellationToken cancellationToken);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, IEnumerable<string> tags = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);
    }
}
