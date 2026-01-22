using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Common.Cache.Configuration;
using RedShirt.Adventure.Realm.Common.Cache.Services;

namespace RedShirt.Adventure.Realm.Common.Cache.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services,
        IConfigurationRoot configurationRoot)
    {
        return services
            // General
            .AddSingleton<ICacheService, CacheService>()
            .Configure<CacheConfigurationModel>(configurationRoot.GetSection("Cache"))
            // Local
            .AddSingleton<IInMemoryCacheService, InMemoryCacheService>()
            // Redis
            .AddSingleton<IRedisConnectionSource, RedisConnectionSource>()
            .Configure<RedisConnectionSource.ConfigurationModel>(configurationRoot.GetSection("Cache:Redis"))
            .AddSingleton<IRedisCacheService, RedisCacheService>();
    }
}