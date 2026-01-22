using System.Text.Json;

namespace RedShirt.Adventure.Realm.Common.Cache.Services;

internal interface IRedisCacheService
{
    Task<string?> GetAsync(Guid key, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(Guid key, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(Guid key, CancellationToken cancellationToken = default);
    Task SetAsync(Guid key, string value, TimeSpan duration, CancellationToken cancellationToken = default);

    Task SetAsync<T>(Guid key, T value, TimeSpan duration, CancellationToken cancellationToken = default)
        where T : class;
}

internal class RedisCacheService(IRedisConnectionSource redisConnectionSource) : IRedisCacheService
{
    public async Task<string?> GetAsync(Guid key, CancellationToken cancellationToken = default)
    {
        var cache = redisConnectionSource.GetDatabase();
        var data = await cache.StringGetAsync(key.ToString());
        return data;
    }

    public async Task<T?> GetAsync<T>(Guid key, CancellationToken cancellationToken = default) where T : class
    {
        var keyString = key.ToString();
        var cache = redisConnectionSource.GetDatabase();
        var data = await cache.StringGetAsync(keyString);
        return data.HasValue ? JsonSerializer.Deserialize<T>(data.ToString()) : null;
    }

    public Task RemoveAsync(Guid key, CancellationToken cancellationToken = default)
    {
        var cache = redisConnectionSource.GetDatabase();
        return cache.KeyDeleteAsync(key.ToString());
    }

    public Task SetAsync(Guid key, string value, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        var cache = redisConnectionSource.GetDatabase();

        return cache.StringSetAsync(key.ToString(), value, duration);
    }

    public Task SetAsync<T>(Guid key, T value, TimeSpan duration, CancellationToken cancellationToken = default)
        where T : class
    {
        return SetAsync(key, JsonSerializer.Serialize(value), duration, cancellationToken);
    }
}