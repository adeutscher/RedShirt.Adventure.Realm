namespace RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;

public class CharacterActionBarModel
{
    public required Guid CharacterId { get; init; }
    public required List<CharacterActionBarSlotModel> Actions { get; init; }
}