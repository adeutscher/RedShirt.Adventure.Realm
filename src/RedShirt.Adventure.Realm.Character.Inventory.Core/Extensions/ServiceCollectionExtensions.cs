using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Character.Inventory.Core.Services;

namespace RedShirt.Adventure.Realm.Character.Inventory.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharacterInventoryCore(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICharacterInventoryService, CharacterInventoryService>();
    }
}