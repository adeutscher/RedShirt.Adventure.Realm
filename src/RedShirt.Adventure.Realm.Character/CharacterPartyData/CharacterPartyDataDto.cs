using RedShirt.Adventure.Realm.Common.Analyzers.Abstractions.Attributes;

namespace RedShirt.Adventure.Realm.Character.CharacterPartyData;

[DbTable("CharacterPartyData")]
[DoNotGeneratePost]
public class CharacterPartyDataDto
{
    [DbKey]
    public required Guid CharacterId { get; init; }

    [CreatedAtProperty]
    public required DateTime CreatedAtUtc { get; init; }

    [UpdatedAtProperty]
    public required DateTime UpdatedAtUtc { get; init; }

    public required Guid PartyId { get; init; }
    public required bool IsInParty { get; init; }
}