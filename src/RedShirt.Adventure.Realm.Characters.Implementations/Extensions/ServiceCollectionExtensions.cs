using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Characters.Core.Extensions;
using RedShirt.Adventure.Realm.Characters.Core.Models;
using RedShirt.Adventure.Realm.Characters.Core.Repositories;
using RedShirt.Adventure.Realm.Characters.Implementations.Repositories;
using RedShirt.Adventure.Realm.Common.Database.Extensions;

namespace RedShirt.Adventure.Realm.Characters.Implementations.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRealmCharacters(this IServiceCollection services,
        IConfigurationRoot configuration)
    {
        return services
            .AddCharactersCore()
            .AddDatabase(configuration)
            .AddGenericDtoHandler<CharacterDto, Guid>()
            .AddSingleton<ICharacterRepository, CharacterMySqlRepository>();
    }
}