using RedShirt.Adventure.Realm.Character.Inventory.Core.Models;
using RedShirt.Adventure.Realm.Common.Exceptions;

namespace RedShirt.Adventure.Realm.Character.Inventory.Core.Services;

public interface ICharacterInventoryService
{
    Task<CharacterInventoryModel>
        GetByCharacterIdAsync(Guid characterId, CancellationToken cancellationToken = default);

    Task SetAsync(CharacterInventoryWriteBundle bundle, CancellationToken cancellationToken = default);
}

internal class CharacterInventoryService(ICharacterInventoryRepository repository) : ICharacterInventoryService
{
    public Task<CharacterInventoryModel> GetByCharacterIdAsync(Guid characterId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetAsync(characterId, cancellationToken);
    }

    public async Task SetAsync(CharacterInventoryWriteBundle bundle, CancellationToken cancellationToken = default)
    {
        var slotTracker = new HashSet<int>();
        var instanceIdTracker = new HashSet<Guid>();
        /* Validate */
        foreach (var item in bundle.Slots)
        {
            if (item.SlotId < 0)
            {
                throw new BadRequestException("Cannot set empty slot");
            }

            if (item.InstanceId == Guid.Empty || item.ItemId == Guid.Empty)
            {
                throw new BadRequestException("Cannot send empty GUID in InstanceId or ItemId");
            }

            if (item.Quantity < 1)
            {
                throw new BadRequestException("Cannot set empty quantity");
            }

            if (!slotTracker.Add(item.SlotId))
            {
                throw new BadRequestException("Cannot update slot twice");
            }

            if (!instanceIdTracker.Add(item.InstanceId))
            {
                throw new BadRequestException("Cannot update item instance twice");
            }
        }

        /* Set */
        await repository.SetAsync(bundle, cancellationToken);
    }
}