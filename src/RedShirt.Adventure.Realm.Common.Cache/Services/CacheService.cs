using Microsoft.Extensions.Options;
using RedShirt.Adventure.Realm.Common.Cache.Configuration;

namespace RedShirt.Adventure.Realm.Common.Cache.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(Guid key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync(Guid key, object value, CancellationToken cancellationToken = default);
}

internal class CacheService(
    IInMemoryCacheService inMemoryCacheService,
    IRedisCacheService redisCacheService,
    IOptions<CacheConfigurationModel> options) : ICacheService
{
    public async Task SetAsync(Guid key, object value, CancellationToken cancellationToken = default)
    {
        inMemoryCacheService.Set(key, value);
        await redisCacheService.SetAsync(key, value, TimeSpan.FromSeconds(options.Value.EffectiveExpiryTimeSeconds),
            cancellationToken);
    }

    public async Task<T?> GetAsync<T>(Guid key, CancellationToken cancellationToken = default) where T : class
    {
        if (inMemoryCacheService.TryTake<T>(key, out var value))
        {
            return value;
        }

        return await redisCacheService.GetAsync<T>(key, cancellationToken);
    }
}