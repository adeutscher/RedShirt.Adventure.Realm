using Microsoft.Extensions.Options;
using RedShirt.Adventure.Realm.Common.Cache.Configuration;

namespace RedShirt.Adventure.Realm.Common.Cache.Services;

internal interface IInMemoryCacheService
{
    void Set(Guid key, object objectValue);
    bool TryTake<T>(Guid key, out T value) where T : class;
}

internal class InMemoryCacheService(IOptions<CacheConfigurationModel> options) : IInMemoryCacheService
{
    private readonly List<ListEntry> _listOfKeys = [];
    private readonly Dictionary<Guid, object> _objects = new();

    private void Clean()
    {
        var threshold = TimeSpan.FromSeconds(options.Value.EffectiveExpiryTimeSeconds);

        while (_listOfKeys.Count > 0)
        {
            var entry = _listOfKeys[0]; // shorthand
            if (entry.DateEstablishedUtc + threshold < DateTime.UtcNow)
            {
                _listOfKeys.RemoveAt(0);
                _objects.Remove(entry.Key);
            }
            else
            {
                /*
                 * Each entry in the list is known to be newer than the one before it,
                 * so there isn't any point in continuing once we've found one non-expired record
                 */
                break;
            }
        }
    }

    public void Set(Guid key, object objectValue)
    {
        Clean();
        _objects[key] = objectValue;
        _listOfKeys.Add(new ListEntry
        {
            DateEstablishedUtc = DateTime.UtcNow,
            Key = key
        });
    }

    public bool TryTake<T>(Guid key, out T value) where T : class
    {
        Clean();
        var success = _objects.Remove(key, out var objectValue);
#pragma warning disable CS8601 // Possible null reference assignment.
        value = objectValue as T;
#pragma warning restore CS8601 // Possible null reference assignment.
        return success && value is not null;
    }

    private class ListEntry
    {
        public required DateTime DateEstablishedUtc { get; init; }
        public required Guid Key { get; init; }
    }
}