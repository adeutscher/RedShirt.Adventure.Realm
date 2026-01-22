using RedShirt.Adventure.Realm.Character.Extensions;
using RedShirt.Adventure.Realm.Common.Cache.Extensions;
using RedShirt.Adventure.Realm.Common.Database.Extensions;

namespace RedShirt.Adventure.Realm.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureApiServices(this IServiceCollection serviceCollection,
        IConfigurationRoot configuration)
    {
        return serviceCollection
            .AddCaching(configuration)
            .AddDatabase(configuration)
            .AddCharacterSupport();
    }
}