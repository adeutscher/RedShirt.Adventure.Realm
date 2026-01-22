using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace RedShirt.Adventure.Realm.Common.Cache.Services;

internal interface IRedisConnectionSource
{
    IDatabase GetDatabase();
}

internal class RedisConnectionSource(IOptions<RedisConnectionSource.ConfigurationModel> options)
    : IRedisConnectionSource
{
    private readonly Lazy<ConnectionMultiplexer> _lazyConnection = new(() =>
        ConnectionMultiplexer.Connect($"{options.Value.EndpointAddress}:{options.Value.EndpointPort}"));

    public IDatabase GetDatabase()
    {
        return _lazyConnection.Value.GetDatabase();
    }

    public class ConfigurationModel
    {
        public required string EndpointAddress { get; init; }
        public required int EndpointPort { get; init; }
    }
}