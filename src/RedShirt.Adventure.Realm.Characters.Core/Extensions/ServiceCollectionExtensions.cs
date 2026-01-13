using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Characters.Core.Services;

namespace RedShirt.Adventure.Realm.Characters.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharactersCore(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICharacterService, CharacterService>()
            .AddSingleton<ICharacterNameValidator, CharacterNameValidator>();
    }
}