namespace RedShirt.Adventure.Realm.Models.Characters.CharacterInventory;

public class CharacterInventoryWriteSlot
{
    public int Slot { get; init; }
    public Guid InstanceId { get; init; }
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
}