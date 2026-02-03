namespace RedShirt.Adventure.Realm.Models.Characters.CharacterLocation;

public class CharacterLocationPatchRequest
{
    public int? MapId { get; init; }
    public Guid? MapInstanceId { get; init; }
    public float? PositionX { get; init; }
    public float? PositionY { get; init; }
    public float? PositionZ { get; init; }
    public float? Rotation { get; init; }
}