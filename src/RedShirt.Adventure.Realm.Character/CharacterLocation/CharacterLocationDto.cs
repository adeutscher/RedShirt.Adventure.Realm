using RedShirt.Adventure.Realm.Common.Analyzers.Abstractions.Attributes;

namespace RedShirt.Adventure.Realm.Character.CharacterLocation;

[DbTable("CharacterLocation")]
[DoNotGeneratePost]
public class CharacterLocationDto
{
    [DbKey]
    public required Guid CharacterId { get; init; }

    [CreatedAtProperty]
    public required DateTime CreatedAtUtc { get; init; }

    [UpdatedAtProperty]
    public required DateTime UpdatedAtUtc { get; init; }

    public required int MapId { get; init; }
    public required float PositionX { get; init; }
    public required float PositionY { get; init; }
    public required float PositionZ { get; init; }
    public required float Rotation { get; init; }
}