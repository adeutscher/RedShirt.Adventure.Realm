namespace RedShirt.Adventure.Realm.Characters.Core.Models.Requests;

public class CharacterDtoPatchRequest
{
    public Guid Id { get; init; }
    public required string? FirstName { get; init; }
    public required string? LastName { get; init; }
    public required string? AccountId { get; init; }
    public required int? MapId { get; init; }
    public required float? PositionX { get; init; }
    public required float? PositionY { get; init; }
    public required float? PositionZ { get; init; }
    public required float? Rotation { get; init; }
    public required ulong? HealthCurrent { get; init; }
    public required ulong? HealthMaximum { get; init; }
}