using RedShirt.Adventure.Realm.Characters.Core.Models;
using RedShirt.Adventure.Realm.Characters.Core.Models.Requests;
using RedShirt.Adventure.Realm.Characters.Core.Models.Responses;

namespace RedShirt.Adventure.Realm.Characters.Core.Repositories;

public interface ICharacterRepository
{
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CharacterDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CharacterDto> PutAsync(CharacterDto character, CancellationToken cancellationToken = default);

    Task<CharacterSearchResponse> SearchAsync(CharacterSearchRequest request,
        CancellationToken cancellationToken = default);
}