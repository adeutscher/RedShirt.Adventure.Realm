using RedShirt.Adventure.Realm.Common.Analyzers.Abstractions.Attributes;

namespace RedShirt.Adventure.Realm.Character.Character;

[DbTable("Character")]
[DoNotGenerateService]
public class CharacterDto
{
    [DbKey]
    public required Guid Id { get; init; }

    [CreatedAtProperty]
    public required DateTime CreatedAtUtc { get; init; }

    [UpdatedAtProperty]
    public required DateTime UpdatedAtUtc { get; init; }

    public required string AccountId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}