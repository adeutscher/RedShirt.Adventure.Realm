using RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;

namespace RedShirt.Adventure.Realm.Character.ActionBar.Core.Services;

public interface ICharacterActionBarRepository
{
    Task<CharacterActionBarModel> GetAsync(Guid characterId, CancellationToken cancellationToken = default);
    Task SetAsync(CharacterActionBarWriteBundle bundle, CancellationToken cancellationToken = default);
}