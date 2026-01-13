using RedShirt.Adventure.Realm.Characters.Implementations.Extensions;

namespace RedShirt.Adventure.Realm.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureApiServices(this IServiceCollection serviceCollection,
        IConfigurationRoot configuration)
    {
        return serviceCollection
            .AddRealmCharacters(configuration);
    }
}