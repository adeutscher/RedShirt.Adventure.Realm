namespace RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;

public class CharacterActionBarWriteBundle
{
    public required Guid CharacterId { get; init; }
    public required List<Slot> Slots { get; init; }

    public sealed class Slot
    {
        public required int SlotId { get; init; }
        public required CharacterActionBarActionType ActionType { get; init; }
        public required Guid SubjectId { get; init; }
        public required Guid ItemId { get; init; }
    }
}