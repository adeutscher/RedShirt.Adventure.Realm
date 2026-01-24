using Dapper;
using Microsoft.Extensions.Logging;
using RedShirt.Adventure.Realm.Character.Inventory.Core.Models;
using RedShirt.Adventure.Realm.Character.Inventory.Core.Services;
using RedShirt.Adventure.Realm.Common.Database.Services;
using RedShirt.Adventure.Realm.Common.Database.Utility;
using RedShirt.Adventure.Realm.Common.Exceptions;

namespace RedShirt.Adventure.Realm.Character.Inventory.Implementations.Services;

internal class SqlCharacterInventoryRepository(
    ISqlConnectionFactory factory,
    ILogger<SqlCharacterInventoryRepository> logger) : ICharacterInventoryRepository
{
    private const string TableName = "CharacterInventorySlot";

    /// <summary>
    ///     Retrieve a character's inventory out of the database.
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CharacterInventoryModel> GetAsync(Guid characterId, CancellationToken cancellationToken = default)
    {
        /* Parameters */

        var retryPolicy = PolicyHelper.GetRetryPolicy(logger);
        var queryBuilder =
            new SqlBuilder().Where(
                $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.CharacterId))} = @characterId",
                new {characterId});
        var template =
            queryBuilder.AddTemplate(
                $"SELECT * FROM {DatabaseUtility.QuoteResource(TableName)} /**where**/");

        /* Execute */

        IEnumerable<InventorySlotDto>? response;
        using (var connection = await factory.GetConnectionAsync())
        {
            response = await retryPolicy.ExecuteAsync(() =>
                connection.QueryAsync<InventorySlotDto>(template.RawSql, template.Parameters));
        }

        /* Return */
        return new CharacterInventoryModel
        {
            CharacterId = characterId,
            Items = (response ?? []).Select(r => new CharacterInventorySlotModel
            {
                CreatedAtUtc = r.CreatedAtUtc,
                UpdatedAtUtc = r.UpdatedAtUtc,
                Slot = r.InventorySlot,
                InstanceId = r.ItemInstance,
                ItemId = r.ItemId,
                Quantity = r.StackSize
            }).OrderBy(i => i.Slot).ToList()
        };
    }

    /// <summary>
    ///     Save a character's inventory within a transaction
    /// </summary>
    /// <param name="bundle"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="BadRequestException"></exception>
    public async Task SetAsync(CharacterInventoryWriteBundle bundle, CancellationToken cancellationToken = default)
    {
        var retryPolicy = PolicyHelper.GetRetryPolicy(logger);

        using var connection = await factory.GetConnectionAsync();
        connection.Open(); // Need to open the connection before starting a transaction
        using var transaction = connection.BeginTransaction();

        // Declare once outside of loop, even if it ends up not being used
        List<string>? insertFields = null;

        // Get the subject character's current inventory
        var dtoGetByCharacterQuery = new SqlBuilder()
            .Where(
                $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.CharacterId))} = @characterId",
                new {characterId = bundle.CharacterId})
            .AddTemplate(
                $"SELECT * FROM {DatabaseUtility.QuoteResource(TableName)} /**where**/");
        var existingInventoryEnumerable = await retryPolicy.ExecuteAsync(() =>
            connection.QueryAsync<InventorySlotDto>(dtoGetByCharacterQuery.RawSql, dtoGetByCharacterQuery.Parameters));
        var existingInventory = (existingInventoryEnumerable ?? []).ToDictionary(row => row.ItemInstance);

        /*
         * Wipe any rows that aren't mentioned in our request
         * Assumed to be depleted or otherwise deleted
         */
        {
            var dtoDeleteByCharacterQuery = new SqlBuilder()
                .Where(
                    $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.CharacterId))} = @characterId",
                    new {characterId = bundle.CharacterId})
                /*
                 * Delete everything in the current character's inventory that's not mentioned here
                 * Doing this on the assumption that the set of changed items will be a smaller
                 * set than just flat-out saying "WHERE InstanceId NOT IN @currentBundleIds"
                 */
                .Where(
                    $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.ItemInstance))} IN @instanceIds",
                    new
                    {
                        instanceIds = (existingInventoryEnumerable ?? [])
                            .Select(ei => ei.ItemInstance)
                            .Except(bundle.Slots.Select(s => s.InstanceId))
                    })
                .AddTemplate(
                    $"DELETE FROM {DatabaseUtility.QuoteResource(TableName)} /**where**/");
            await retryPolicy.ExecuteAsync(() =>
                connection.ExecuteAsync(dtoDeleteByCharacterQuery.RawSql, dtoDeleteByCharacterQuery.Parameters));
        }

        // Cycle through inventory to write/insert

        foreach (var inventorySlot in bundle.Slots)
        {
            var dto = existingInventory.GetValueOrDefault(inventorySlot.InstanceId);

            if (dto is null)
            {
                /*
                 * If the dto is null at this point, suggests that the map server making the save call either:
                 *  * Granted a brand-new item to the user
                 *  * Transferred an item from another character through a trade
                 */
                var dtoGetQuery =
                    new SqlBuilder()
                        .Where(
                            $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.ItemInstance))} = @instanceId",
                            new {instanceId = inventorySlot.InstanceId})
                        .AddTemplate(
                            $"SELECT * FROM {DatabaseUtility.QuoteResource(TableName)} /**where**/ /**orderby**/");

                dto = await retryPolicy.ExecuteAsync(() =>
                    connection.QueryFirstOrDefaultAsync<InventorySlotDto>(dtoGetQuery.RawSql, dtoGetQuery.Parameters));
            }

            if (dto is null)
            {
                // Need to insert a new record

                var createdAtUtc = DateTime.UtcNow;
                var insertParams = new
                {
                    characterId = bundle.CharacterId,
                    createdAtUtc,
                    updatedAtUtc = createdAtUtc,
                    slotId = inventorySlot.SlotId,
                    instanceId = inventorySlot.InstanceId,
                    itemId = inventorySlot.ItemId,
                    quantity = inventorySlot.Quantity
                };

                // Doing this so that we're only declaring the list once,
                //   and even then only actually populating it when it's needed
                insertFields ??=
                [
                    // Be wary of messing with the order of items in this variable
                    // If you do make changes, be mindful that order of the VALUES section in the below insert statement matches.
                    DatabaseUtility.QuoteResource(nameof(InventorySlotDto.CharacterId)),
                    DatabaseUtility.QuoteResource(nameof(InventorySlotDto.CreatedAtUtc)),
                    DatabaseUtility.QuoteResource(nameof(InventorySlotDto.UpdatedAtUtc)),
                    DatabaseUtility.QuoteResource(nameof(InventorySlotDto.InventorySlot)),
                    DatabaseUtility.QuoteResource(nameof(InventorySlotDto.ItemInstance)),
                    DatabaseUtility.QuoteResource(nameof(InventorySlotDto.ItemId)),
                    DatabaseUtility.QuoteResource(nameof(InventorySlotDto.StackSize))
                ];

                var dtoInsertQuery = new SqlBuilder()
                    .AddTemplate($"INSERT INTO {DatabaseUtility.QuoteResource(TableName)}"
                                 + $" ({string.Join(", ", insertFields)})"
                                 + " VALUES (@characterId, @createdAtUtc, @updatedAtUtc, @slotId, @instanceId, @itemId, @quantity)",
                        insertParams);
                await retryPolicy.ExecuteAsync(() =>
                    connection.ExecuteAsync(dtoInsertQuery.RawSql, dtoInsertQuery.Parameters));
            }
            else
            {
                // Need to update an existing record

                if (dto.ItemId != inventorySlot.ItemId)
                {
                    throw new BadRequestException("Unable to change item type of instance");
                }

                var updateParams = new
                {
                    characterId = bundle.CharacterId,
                    updatedAtUtc = DateTime.UtcNow,
                    slotId = inventorySlot.SlotId,
                    quantity = inventorySlot.Quantity
                };

                var updateStatements = new List<string>();

                if (dto.CharacterId != bundle.CharacterId)
                {
                    updateStatements.Add(
                        $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.CharacterId))} = @characterId");
                }

                if (dto.InventorySlot != inventorySlot.SlotId)
                {
                    updateStatements.Add(
                        $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.InventorySlot))} = @slotId");
                }

                if (dto.StackSize != inventorySlot.Quantity)
                {
                    updateStatements.Add(
                        $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.StackSize))} = @quantity");
                }

                if (updateStatements.Count == 0)
                {
                    // No fields to update
                    continue;
                }

                updateStatements.Add(
                    $"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.UpdatedAtUtc))} = @updatedAtUtc");

                var dtoUpdateQuery = new SqlBuilder()
                    .Where($"{DatabaseUtility.QuoteResource(nameof(InventorySlotDto.ItemInstance))} = @instanceId",
                        new {instanceId = inventorySlot.InstanceId})
                    .AddTemplate($"UPDATE {DatabaseUtility.QuoteResource(TableName)}"
                                 + $" SET {string.Join(", ", updateStatements)}"
                                 + " /**where**/", updateParams);
                await retryPolicy.ExecuteAsync(() =>
                    connection.ExecuteAsync(dtoUpdateQuery.RawSql, dtoUpdateQuery.Parameters));
            }
        }

        transaction.Commit();
    }

    private sealed class InventorySlotDto
    {
        public required Guid CharacterId { get; init; }
        public required DateTime CreatedAtUtc { get; init; }
        public required DateTime UpdatedAtUtc { get; init; }
        public required int InventorySlot { get; init; }
        public required Guid ItemInstance { get; init; }
        public required Guid ItemId { get; init; }
        public required int StackSize { get; init; }
    }
}