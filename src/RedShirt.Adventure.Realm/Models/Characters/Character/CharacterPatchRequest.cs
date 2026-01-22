namespace RedShirt.Adventure.Realm.Models.Characters.Character;

public class CharacterPatchRequest
{
    public string? AccountId { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}