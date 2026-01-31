using Dapper;
using Microsoft.Extensions.Logging;
using RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;
using RedShirt.Adventure.Realm.Character.ActionBar.Core.Services;
using RedShirt.Adventure.Realm.Common.Database.Services;
using RedShirt.Adventure.Realm.Common.Database.Utility;

namespace RedShirt.Adventure.Realm.Character.ActionBar.Implementations.Services;

public class SqlCharacterActionBarRepository(
    ISqlConnectionFactory factory,
    ILogger<SqlCharacterActionBarRepository> logger) : ICharacterActionBarRepository
{
    private const string TableName = "CharacterActionBarSlot";

    /// <summary>
    ///     Retrieve a character's action bar configuration out of the database.
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CharacterActionBarModel> GetAsync(Guid characterId, CancellationToken cancellationToken = default)
    {
        /* Parameters */

        var retryPolicy = PolicyHelper.GetRetryPolicy(logger);
        var queryBuilder =
            new SqlBuilder().Where(
                $"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.CharacterId))} = @characterId",
                new {characterId});
        var template =
            queryBuilder.AddTemplate(
                $"SELECT * FROM {DatabaseUtility.QuoteResource(TableName)} /**where**/");

        /* Execute */

        IEnumerable<ActionBarSlotDto>? response;
        using (var connection = await factory.GetConnectionAsync())
        {
            response = await retryPolicy.ExecuteAsync(() =>
                connection.QueryAsync<ActionBarSlotDto>(template.RawSql, template.Parameters));
        }

        /* Return */
        return new CharacterActionBarModel
        {
            CharacterId = characterId,
            Actions = (response ?? []).Select(r => new CharacterActionBarSlotModel
            {
                CreatedAtUtc = r.CreatedAtUtc,
                UpdatedAtUtc = r.UpdatedAtUtc,
                Slot = r.SlotId,
                ActionType = r.ActionType,
                SubjectId = r.SubjectId,
                ItemId = r.ItemId
            }).OrderBy(i => i.Slot).ToList()
        };
    }

    /// <summary>
    ///     Save a character's action bar configuration within a transaction.
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SetAsync(CharacterActionBarWriteBundle bundle, CancellationToken cancellationToken = default)
    {
        var retryPolicy = PolicyHelper.GetRetryPolicy(logger);

        using var connection = await factory.GetConnectionAsync();
        connection.Open(); // Need to open the connection before starting a transaction
        using var transaction = connection.BeginTransaction();

        // Declare once outside of loop, even if it ends up not being used
        List<string>? insertFields = null;

        // Get the subject character's current ActionBar
        var dtoGetByCharacterQuery = new SqlBuilder()
            .Where(
                $"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.CharacterId))} = @characterId",
                new {characterId = bundle.CharacterId})
            .AddTemplate(
                $"SELECT * FROM {DatabaseUtility.QuoteResource(TableName)} /**where**/");
        var existingActionBarEnumerable = await retryPolicy.ExecuteAsync(() =>
            connection.QueryAsync<ActionBarSlotDto>(dtoGetByCharacterQuery.RawSql, dtoGetByCharacterQuery.Parameters));
        var existingActionBar = (existingActionBarEnumerable ?? []).ToDictionary(row => row.SlotId);

        /*
         * Wipe any rows that aren't mentioned in our request
         * Assumed to be removed
         */
        {
            var dtoDeleteByCharacterQuery = new SqlBuilder()
                .Where(
                    $"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.CharacterId))} = @characterId",
                    new {characterId = bundle.CharacterId})
                /*
                 * Delete everything in the current character's action bar that's not mentioned in the write bundle
                 * Doing this on the assumption that the set of changed records will be a smaller
                 * set than just flat-out saying "WHERE SlotId NOT IN @currentSlotIds"
                 */
                .Where(
                    $"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.SlotId))} IN @slotIds",
                    new
                    {
                        slotIds = (existingActionBarEnumerable ?? [])
                            .Select(ei => ei.SlotId)
                            .Except(bundle.Slots.Select(s => s.SlotId))
                    })
                .AddTemplate(
                    $"DELETE FROM {DatabaseUtility.QuoteResource(TableName)} /**where**/");
            await retryPolicy.ExecuteAsync(() =>
                connection.ExecuteAsync(dtoDeleteByCharacterQuery.RawSql, dtoDeleteByCharacterQuery.Parameters));
        }

        // Cycle through ActionBar to write/insert

        foreach (var actionBarSlot in bundle.Slots)
        {
            var dto = existingActionBar.GetValueOrDefault(actionBarSlot.SlotId);

            if (dto is null)
            {
                // Need to insert a new record

                var createdAtUtc = DateTime.UtcNow;
                var insertParams = new
                {
                    characterId = bundle.CharacterId,
                    createdAtUtc,
                    updatedAtUtc = createdAtUtc,
                    slotId = actionBarSlot.SlotId,
                    actionType = actionBarSlot.ActionType,
                    subjectId = actionBarSlot.SubjectId,
                    itemId = actionBarSlot.ItemId
                };

                // Doing this so that we're only declaring the list once,
                //   and even then only actually populating it when it's needed
                insertFields ??=
                [
                    // Be wary of messing with the order of items in this variable
                    // If you do make changes, be mindful that order of the VALUES section in the below insert statement matches.
                    DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.CharacterId)),
                    DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.SlotId)),
                    DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.CreatedAtUtc)),
                    DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.UpdatedAtUtc)),
                    DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.ActionType)),
                    DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.SubjectId)),
                    DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.ItemId))
                ];

                var dtoInsertQuery = new SqlBuilder()
                    .AddTemplate($"INSERT INTO {DatabaseUtility.QuoteResource(TableName)}"
                                 + $" ({string.Join(", ", insertFields)})"
                                 + " VALUES (@characterId, @slotId, @createdAtUtc, @updatedAtUtc, @actionType, @subjectId, @itemId)",
                        insertParams);
                await retryPolicy.ExecuteAsync(() =>
                    connection.ExecuteAsync(dtoInsertQuery.RawSql, dtoInsertQuery.Parameters));
            }
            else
            {
                // Need to update an existing record

                var updateParams = new
                {
                    updatedAtUtc = DateTime.UtcNow,
                    actionType = actionBarSlot.ActionType,
                    subjectId = actionBarSlot.SubjectId,
                    itemId = actionBarSlot.ItemId
                };

                var updateStatements = new List<string>();

                if (dto.ActionType != actionBarSlot.ActionType)
                {
                    updateStatements.Add(
                        $"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.ActionType))} = @actionType");
                }

                if (dto.SubjectId != actionBarSlot.SubjectId)
                {
                    updateStatements.Add(
                        $"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.SubjectId))} = @subjectId");
                }

                if (dto.ItemId != actionBarSlot.ItemId)
                {
                    updateStatements.Add(
                        $"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.ItemId))} = @itemId");
                }

                if (updateStatements.Count == 0)
                {
                    // No fields to update
                    continue;
                }

                updateStatements.Add(
                    $"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.UpdatedAtUtc))} = @updatedAtUtc");

                var dtoUpdateQuery = new SqlBuilder()
                    .Where($"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.CharacterId))} = @characterId",
                        new {characterId = bundle.CharacterId})
                    .Where($"{DatabaseUtility.QuoteResource(nameof(ActionBarSlotDto.SlotId))} = @slotId",
                        new {slotId = actionBarSlot.SlotId})
                    .AddTemplate($"UPDATE {DatabaseUtility.QuoteResource(TableName)}"
                                 + $" SET {string.Join(", ", updateStatements)}"
                                 + " /**where**/", updateParams);
                await retryPolicy.ExecuteAsync(() =>
                    connection.ExecuteAsync(dtoUpdateQuery.RawSql, dtoUpdateQuery.Parameters));
            }
        }

        transaction.Commit();
    }

    private sealed class ActionBarSlotDto
    {
        public required Guid CharacterId { get; init; }
        public required int SlotId { get; init; }
        public required DateTime CreatedAtUtc { get; init; }
        public required DateTime UpdatedAtUtc { get; init; }
        public required CharacterActionBarActionType ActionType { get; init; }
        public required Guid SubjectId { get; init; }
        public required Guid ItemId { get; init; }
    }
}