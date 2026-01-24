namespace RedShirt.Adventure.Realm.Character.Inventory.Core.Models;

public sealed class CharacterInventorySlotModel
{
    public required DateTime CreatedAtUtc { get; init; }
    public required DateTime UpdatedAtUtc { get; init; }
    public required int Slot { get; init; }
    public required Guid InstanceId { get; init; }
    public required Guid ItemId { get; init; }
    public required int Quantity { get; init; }
}