using RedShirt.Adventure.Realm.Character.Inventory.Core.Models;

namespace RedShirt.Adventure.Realm.Character.Inventory.Core.Services;

public interface ICharacterInventoryRepository
{
    Task<CharacterInventoryModel> GetAsync(Guid characterId, CancellationToken cancellationToken = default);
    Task SetAsync(CharacterInventoryWriteBundle bundle, CancellationToken cancellationToken = default);
}