using Microsoft.Extensions.DependencyInjection;
using RedShirt.Adventure.Realm.Character.Character.Generated;
using RedShirt.Adventure.Realm.Character.CharacterLocation.Generated;
using RedShirt.Adventure.Realm.Character.CharacterResources.Generated;

namespace RedShirt.Adventure.Realm.Character.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharacterSupport(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddGeneratedCharacter()
            .AddGeneratedCharacterLocation()
            .AddGeneratedCharacterResources();
    }
}