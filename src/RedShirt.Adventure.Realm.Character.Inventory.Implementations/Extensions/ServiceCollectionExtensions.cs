using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Character.Inventory.Core.Services;
using RedShirt.Adventure.Realm.Character.Inventory.Implementations.Services;

namespace RedShirt.Adventure.Realm.Character.Inventory.Implementations.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharacterInventoryImplementations(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICharacterInventoryRepository, SqlCharacterInventoryRepository>();
    }
}