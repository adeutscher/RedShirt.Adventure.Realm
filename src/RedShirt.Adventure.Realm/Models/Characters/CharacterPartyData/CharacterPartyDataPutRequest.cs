namespace RedShirt.Adventure.Realm.Models.Characters.CharacterPartyData;

public class CharacterPartyDataPutRequest
{
    public Guid PartyId { get; init; }
    public bool IsInParty { get; init; }
}