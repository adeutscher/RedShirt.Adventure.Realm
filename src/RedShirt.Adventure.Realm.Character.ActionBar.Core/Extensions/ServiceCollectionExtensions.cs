using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Character.ActionBar.Core.Services;

namespace RedShirt.Adventure.Realm.Character.ActionBar.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharacterActionBarCore(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICharacterActionBarService, CharacterActionBarService>();
    }
}