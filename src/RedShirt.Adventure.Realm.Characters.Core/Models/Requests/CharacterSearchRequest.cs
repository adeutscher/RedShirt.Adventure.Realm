namespace RedShirt.Adventure.Realm.Characters.Core.Models.Requests;

public class CharacterSearchRequest
{
    public required string? AccountId { get; init; }
    public required string? FirstName { get; init; }
    public required string? LastName { get; init; }
    public required int? MapId { get; init; }
    public required string? ContinuationToken { get; init; }
}