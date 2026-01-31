using RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;

namespace RedShirt.Adventure.Realm.Models.Characters.CharacterActionBar;

public class CharacterActionBarWriteSlot
{
    public int Slot { get; init; }
    public CharacterActionBarActionType ActionType { get; init; }
    public Guid SubjectId { get; init; }
}