using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Character.ActionBar.Core.Services;
using RedShirt.Adventure.Realm.Character.ActionBar.Implementations.Services;

namespace RedShirt.Adventure.Realm.Character.ActionBar.Implementations.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharacterActionBarImplementations(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICharacterActionBarRepository, SqlCharacterActionBarRepository>();
    }
}