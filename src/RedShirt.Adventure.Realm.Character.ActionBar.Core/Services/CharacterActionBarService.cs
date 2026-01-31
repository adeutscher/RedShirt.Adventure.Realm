using RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;
using RedShirt.Adventure.Realm.Common.Exceptions;

namespace RedShirt.Adventure.Realm.Character.ActionBar.Core.Services;

public interface ICharacterActionBarService
{
    Task<CharacterActionBarModel>
        GetByCharacterIdAsync(Guid characterId, CancellationToken cancellationToken = default);

    Task SetAsync(CharacterActionBarWriteBundle bundle, CancellationToken cancellationToken = default);
}

internal class CharacterActionBarService(ICharacterActionBarRepository repository) : ICharacterActionBarService
{
    public Task<CharacterActionBarModel> GetByCharacterIdAsync(Guid characterId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetAsync(characterId, cancellationToken);
    }

    public async Task SetAsync(CharacterActionBarWriteBundle bundle, CancellationToken cancellationToken = default)
    {
        var slotTracker = new HashSet<int>();

        foreach (var action in bundle.Slots)
        {
            if (action.SlotId < 0)
            {
                throw new BadRequestException("Cannot set empty slot");
            }

            if (action.SubjectId == Guid.Empty)
            {
                throw new BadRequestException("Cannot send empty GUID in SubjectId");
            }

            if (!slotTracker.Add(action.SlotId))
            {
                throw new BadRequestException("Cannot update slot twice");
            }
        }

        await repository.SetAsync(bundle, cancellationToken);
    }
}