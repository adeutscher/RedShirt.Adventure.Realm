namespace RedShirt.Adventure.Realm.Models.Characters;

public class CharacterPatchRequest
{
    public string? AccountId { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public int? MapId { get; init; }
    public float? PositionX { get; init; }
    public float? PositionY { get; init; }
    public float? PositionZ { get; init; }
    public float? Rotation { get; init; }
    public ulong? HealthCurrent { get; init; }
    public ulong? HealthMaximum { get; init; }
}