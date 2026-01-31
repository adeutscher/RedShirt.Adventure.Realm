namespace RedShirt.Adventure.Realm.Models.Characters.CharacterActionBar;

public class CharacterActionBarWriteRequest
{
    public List<CharacterActionBarWriteSlot> Slots { get; init; } = new();
}