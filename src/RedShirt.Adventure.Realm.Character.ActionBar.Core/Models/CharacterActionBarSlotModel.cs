namespace RedShirt.Adventure.Realm.Character.ActionBar.Core.Models;

public class CharacterActionBarSlotModel
{
    public required DateTime CreatedAtUtc { get; init; }
    public required DateTime UpdatedAtUtc { get; init; }
    public required int Slot { get; init; }
    public required CharacterActionBarActionType ActionType { get; init; }
    public required Guid SubjectId { get; init; }
}