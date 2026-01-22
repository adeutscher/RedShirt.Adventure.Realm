namespace RedShirt.Adventure.Realm.Common.Cache.Configuration;

public class CacheConfigurationModel
{
    public required int ExpiryTimeSeconds { get; init; }
    public int EffectiveExpiryTimeSeconds => Math.Max(ExpiryTimeSeconds, 60);
}