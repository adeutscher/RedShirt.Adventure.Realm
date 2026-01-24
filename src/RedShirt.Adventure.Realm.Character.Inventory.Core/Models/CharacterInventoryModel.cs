namespace RedShirt.Adventure.Realm.Character.Inventory.Core.Models;

public class CharacterInventoryModel
{
    public required Guid CharacterId { get; init; }
    public required List<CharacterInventorySlotModel> Items { get; init; }
}