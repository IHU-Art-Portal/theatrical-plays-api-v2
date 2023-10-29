using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Theatrical.Services.Caching;

public interface ICaching
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> retrieve);
}

public class Caching : ICaching
{
    private readonly IMemoryCache _memoryCache;

    public Caching(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> retrieve)
    {
        if (!_memoryCache.TryGetValue(key, out T cacheValue))
        {
            cacheValue = await retrieve();

            if (cacheValue is not null)
            {
                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

                _memoryCache.Set(key, cacheValue, cacheOptions);
            }
        }

        return cacheValue;
    }
}

