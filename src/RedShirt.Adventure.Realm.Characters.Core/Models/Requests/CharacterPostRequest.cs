namespace RedShirt.Adventure.Realm.Characters.Core.Models.Requests;

public class CharacterPostRequest
{
    public required string AccountId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}