using RedShirt.Adventure.Realm.Characters.Core.Extensions;
using RedShirt.Adventure.Realm.Characters.Core.Models;
using RedShirt.Adventure.Realm.Characters.Core.Models.Requests;
using RedShirt.Adventure.Realm.Characters.Core.Models.Responses;
using RedShirt.Adventure.Realm.Characters.Core.Repositories;
using RedShirt.Adventure.Realm.Common.Exceptions;

namespace RedShirt.Adventure.Realm.Characters.Core.Services;

public interface ICharacterService
{
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CharacterDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CharacterDto> PatchAsync(CharacterDtoPatchRequest request,
        CancellationToken cancellationToken = default);

    Task<CharacterDto> PostAsync(CharacterPostRequest request, CancellationToken cancellationToken = default);
    Task<CharacterDto> PutAsync(CharacterDto character, CancellationToken cancellationToken = default);

    Task<CharacterSearchResponse> SearchAsync(CharacterSearchRequest request,
        CancellationToken cancellationToken = default);
}

internal class CharacterService(ICharacterNameValidator nameValidator, ICharacterRepository characterRepository)
    : ICharacterService
{
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await characterRepository.DeleteAsync(id, cancellationToken))
        {
            throw new ResourceNotFoundException();
        }
    }

    public async Task<CharacterDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await characterRepository.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            throw new ResourceNotFoundException();
        }

        return result;
    }

    public Task<CharacterSearchResponse> SearchAsync(CharacterSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        return characterRepository.SearchAsync(request, cancellationToken);
    }

    public async Task<CharacterDto> PostAsync(CharacterPostRequest request,
        CancellationToken cancellationToken = default)
    {
        var firstNameResult = await nameValidator.ValidateAsync(request.FirstName, "First name", cancellationToken);
        if (!firstNameResult.IsValid)
        {
            throw new BadRequestException(firstNameResult.ErrorMessage);
        }

        var lastNameResult = await nameValidator.ValidateAsync(request.LastName, "Last name", cancellationToken);
        if (!lastNameResult.IsValid)
        {
            throw new BadRequestException(lastNameResult.ErrorMessage);
        }

        var existingCharacters = await characterRepository.SearchAsync(new CharacterSearchRequest
        {
            AccountId = null,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MapId = null,
            ContinuationToken = null
        }, cancellationToken);

        if (existingCharacters.Items.Count > 0)
        {
            throw new BadRequestException("Character with this name already exists");
        }

        var dto = new CharacterDto
        {
            Id = Guid.NewGuid(),
            AccountId = request.AccountId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MapId = 0,
            PositionX = 0,
            PositionY = 0,
            PositionZ = 0,
            Rotation = 0,
            CreatedAt = DateTime.UtcNow,
            HealthCurrent = 0,
            HealthMaximum = 0
        };

        return await PutAsync(dto, cancellationToken);
    }

    public async Task<CharacterDto> PatchAsync(CharacterDtoPatchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!request.AreChangesRequested())
        {
            throw new NoChangesToModifyException();
        }

        var existing = await characterRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existing is null)
        {
            throw new ResourceNotFoundException();
        }

        /* Validate */

        if (!string.IsNullOrEmpty(request.FirstName))
        {
            var validationResult =
                await nameValidator.ValidateAsync(request.FirstName, "First name", cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.ErrorMessage);
            }
        }

        if (!string.IsNullOrEmpty(request.LastName))
        {
            var validationResult =
                await nameValidator.ValidateAsync(request.LastName, "Last name", cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.ErrorMessage);
            }
        }

        /* Put */

        var candidate = new CharacterDto
        {
            Id = existing.Id,
            CreatedAt = existing.CreatedAt,
            AccountId = request.AccountId ?? existing.AccountId,
            FirstName = request.FirstName ?? existing.FirstName,
            LastName = request.LastName ?? existing.LastName,
            PositionX = request.PositionX ?? existing.PositionX,
            PositionY = request.PositionY ?? existing.PositionY,
            PositionZ = request.PositionZ ?? existing.PositionZ,
            Rotation = request.Rotation ?? existing.Rotation,
            MapId = request.MapId ?? existing.MapId,
            HealthCurrent = request.HealthCurrent ?? existing.HealthCurrent,
            HealthMaximum = request.HealthMaximum ?? existing.HealthMaximum
        };

        if (candidate.IsTheSameAs(existing))
        {
            throw new NoChangesToModifyException();
        }

        // ReSharper disable once InvertIf
        if (!string.IsNullOrEmpty(request.FirstName) || !string.IsNullOrEmpty(request.LastName))
        {
            // Name changes requested. Check that the name isn't already taken
            var existingCharacters = await characterRepository.SearchAsync(new CharacterSearchRequest
            {
                AccountId = null,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                MapId = null,
                ContinuationToken = null
            }, cancellationToken);

            if (existingCharacters.Items.Count > 0 && existingCharacters.Items[0].Id != existing.Id)
            {
                throw new BadRequestException(
                    $"Character with this name already exists ({existingCharacters.Items[0].Id} != {existing.Id})");
            }
        }

        return await PutAsync(candidate, cancellationToken);
    }

    public Task<CharacterDto> PutAsync(CharacterDto character, CancellationToken cancellationToken = default)
    {
        return characterRepository.PutAsync(character, cancellationToken);
    }
}