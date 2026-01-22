using RedShirt.Adventure.Realm.Common.Analyzers.Abstractions.Attributes;

namespace RedShirt.Adventure.Realm.Character.CharacterResources;

[DbTable("CharacterResources")]
[DoNotGeneratePost]
public class CharacterResourcesDto
{
    [DbKey]
    public required Guid CharacterId { get; init; }

    [CreatedAtProperty]
    public required DateTime CreatedAtUtc { get; init; }

    [UpdatedAtProperty]
    public required DateTime UpdatedAtUtc { get; init; }

    [CannotFilterBy]
    public required ulong HealthCurrent { get; init; }

    [CannotFilterBy]
    public required ulong HealthMaximum { get; init; }
}