using Dapper;
using RedShirt.Adventure.Realm.Characters.Core.Models;
using RedShirt.Adventure.Realm.Characters.Core.Models.Requests;
using RedShirt.Adventure.Realm.Characters.Core.Models.Responses;
using RedShirt.Adventure.Realm.Characters.Core.Repositories;
using RedShirt.Adventure.Realm.Common.Database.Services;

namespace RedShirt.Adventure.Realm.Characters.Implementations.Repositories;

internal class CharacterMySqlRepository(
    IGenericDtoStorage<CharacterDto, Guid> genericDtoStorage,
    ISqlConnectionFactory connectionFactory) : ICharacterRepository
{
    public Task<CharacterDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return genericDtoStorage.GetByKeyAsync(id, cancellationToken);
    }

    public Task<CharacterDto> PutAsync(CharacterDto character, CancellationToken cancellationToken = default)
    {
        return genericDtoStorage.UpsertAsync(character, cancellationToken);
    }

    public async Task<CharacterSearchResponse> SearchAsync(CharacterSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var connection = await connectionFactory.GetConnectionAsync();

        var builder = new SqlBuilder();

        if (!string.IsNullOrWhiteSpace(request.AccountId))
        {
            builder = builder.Where("`AccountId` = @accountId", new {accountId = request.AccountId});
        }

        if (request.MapId.HasValue)
        {
            builder = builder.Where("`MapId` = @mapId", new {mapId = request.MapId.Value});
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            builder = builder.Where("`FirstName` = @firstName", new {firstName = request.FirstName});
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            builder = builder.Where("`LastName` = @lastName", new {lastName = request.LastName});
        }

        var query = builder.AddTemplate("SELECT * FROM `Character` /**where**/");

        var results = await connection.QueryAsync<CharacterDto>(query.RawSql, query.Parameters);

        return new CharacterSearchResponse
        {
            Items = results.ToList()
        };
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return genericDtoStorage.DeleteByKeyAsync(id, cancellationToken);
    }
}