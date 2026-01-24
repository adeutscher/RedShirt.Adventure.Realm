namespace RedShirt.Adventure.Realm.Models.Characters.CharacterInventory;

public class CharacterInventoryWriteRequest
{
    public List<CharacterInventoryWriteSlot> Slots { get; init; } = new();
}