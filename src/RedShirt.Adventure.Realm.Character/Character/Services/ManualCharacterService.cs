using RedShirt.Adventure.Realm.Character.Character.Generated;
using RedShirt.Adventure.Realm.Common.Exceptions;

namespace RedShirt.Adventure.Realm.Character.Character.Services;

internal class ManualCharacterService(ICharacterRepository repository, ICharacterNameValidator characterNameValidator)
    : ICharacterService
{
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await repository.DeleteAsync(id, cancellationToken))
        {
            throw new ResourceNotFoundException();
        }
    }

    public async Task<CharacterDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await repository.GetByIdAsync(id, cancellationToken) is not { } entry)
        {
            throw new ResourceNotFoundException();
        }

        return entry;
    }

    public async Task<CharacterDto> PatchAsync(CharacterServicePatchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!request.AreChangesRequested())
        {
            throw new NoChangesToModifyException();
        }

        /* Validate */

        if (!string.IsNullOrEmpty(request.FirstName))
        {
            var validationResult =
                await characterNameValidator.ValidateAsync(request.FirstName, "First name", cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.ErrorMessage);
            }
        }

        if (!string.IsNullOrEmpty(request.LastName))
        {
            var validationResult =
                await characterNameValidator.ValidateAsync(request.LastName, "Last name", cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.ErrorMessage);
            }
        }

        /* Get Existing */

        if (await repository.GetByIdAsync(request.Id, cancellationToken) is not { } existing)
        {
            throw new ResourceNotFoundException();
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
        {
            var existingCharacters = await repository.SearchAsync(new CharacterServiceSearchRequest
            {
                AccountId = null,
                FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? existing.FirstName : request.FirstName,
                LastName = string.IsNullOrWhiteSpace(request.LastName) ? existing.LastName : request.LastName,
                PageSize = 0,
                CreatedBeforeUtc = null,
                CreatedAfterUtc = null,
                UpdatedBeforeUtc = null,
                UpdatedAfterUtc = null,
                AccountIdContains = null,
                FirstNameContains = null,
                LastNameContains = null
            }, null, cancellationToken);

            if ((existingCharacters.Records.Count > 0 && existingCharacters.Records[0].Id != request.Id)
                || existingCharacters.Records.Count > 1)
            {
                throw new BadRequestException("Character with this name already exists");
            }
        }

        var candidate = new CharacterDto
        {
            Id = request.Id,
            CreatedAtUtc = existing.CreatedAtUtc,
            UpdatedAtUtc = DateTime.UtcNow,
            AccountId = string.IsNullOrWhiteSpace(request.AccountId) ? existing.AccountId : request.AccountId,
            FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? existing.FirstName : request.FirstName,
            LastName = string.IsNullOrWhiteSpace(request.LastName) ? existing.LastName : request.LastName
        };

        if (candidate.IsTheSameAs(existing))
        {
            throw new NoChangesToModifyException();
        }

        return await repository.UpsertAsync(candidate, cancellationToken);
    }

    public async Task<CharacterDto> PostAsync(CharacterServicePostRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.AccountId))
        {
            throw new BadRequestException("AccountId cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            throw new BadRequestException("FirstName cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            throw new BadRequestException("LastName cannot be empty.");
        }

        var firstNameResult =
            await characterNameValidator.ValidateAsync(request.FirstName, "First name", cancellationToken);
        if (!firstNameResult.IsValid)
        {
            throw new BadRequestException(firstNameResult.ErrorMessage);
        }

        var lastNameResult =
            await characterNameValidator.ValidateAsync(request.LastName, "Last name", cancellationToken);
        if (!lastNameResult.IsValid)
        {
            throw new BadRequestException(lastNameResult.ErrorMessage);
        }

        var existingCharacters = await repository.SearchAsync(new CharacterServiceSearchRequest
        {
            AccountId = null,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PageSize = 0,
            CreatedBeforeUtc = null,
            CreatedAfterUtc = null,
            UpdatedBeforeUtc = null,
            UpdatedAfterUtc = null,
            AccountIdContains = null,
            FirstNameContains = null,
            LastNameContains = null
        }, null, cancellationToken);

        if (existingCharacters.Records.Count > 0)
        {
            throw new BadRequestException("Character with this name already exists");
        }

        var createdAt = DateTime.UtcNow;
        var dto = new CharacterDto
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = createdAt,
            UpdatedAtUtc = createdAt,
            AccountId = request.AccountId,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        return await repository.UpsertAsync(dto, cancellationToken);
    }

    public async Task<CharacterDto> PutAsync(CharacterServicePutRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.AccountId))
        {
            throw new BadRequestException("AccountId cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            throw new BadRequestException("FirstName cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            throw new BadRequestException("LastName cannot be empty.");
        }

        var existingCharacters = await repository.SearchAsync(new CharacterServiceSearchRequest
        {
            AccountId = null,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PageSize = 0,
            CreatedBeforeUtc = null,
            CreatedAfterUtc = null,
            UpdatedBeforeUtc = null,
            UpdatedAfterUtc = null,
            AccountIdContains = null,
            FirstNameContains = null,
            LastNameContains = null
        }, null, cancellationToken);

        if ((existingCharacters.Records.Count > 0 && existingCharacters.Records[0].Id != request.Id)
            || existingCharacters.Records.Count > 1)
        {
            throw new BadRequestException("Character with this name already exists");
        }

        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        var createdAt = DateTime.UtcNow;
        var dto = new CharacterDto
        {
            Id = request.Id,
            CreatedAtUtc = existing?.CreatedAtUtc ?? createdAt,
            UpdatedAtUtc = createdAt,
            AccountId = request.AccountId,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        if (existing is not null && existing.IsTheSameAs(dto))
        {
            throw new NoChangesToModifyException();
        }

        return await repository.UpsertAsync(dto, cancellationToken);
    }

    public Task<CharacterSearchResponse> SearchAsync(CharacterServiceSearchRequest parameters, Guid? continuationToken,
        CancellationToken cancellationToken = default)
    {
        return repository.SearchAsync(parameters, continuationToken, cancellationToken);
    }
}