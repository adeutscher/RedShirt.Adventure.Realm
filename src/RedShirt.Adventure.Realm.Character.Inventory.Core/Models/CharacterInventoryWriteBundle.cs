namespace RedShirt.Adventure.Realm.Character.Inventory.Core.Models;

public sealed class CharacterInventoryWriteBundle
{
    public required Guid CharacterId { get; init; }
    public required List<Slot> Slots { get; init; }

    public sealed class Slot
    {
        public required int SlotId { get; init; }
        public required Guid InstanceId { get; init; }
        public required Guid ItemId { get; init; }
        public required int Quantity { get; init; }
    }
}