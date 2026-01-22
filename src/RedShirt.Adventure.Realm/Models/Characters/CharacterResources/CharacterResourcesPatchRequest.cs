namespace RedShirt.Adventure.Realm.Models.Characters.CharacterResources;

public class CharacterResourcesPatchRequest
{
    public ulong? HealthCurrent { get; init; }
    public ulong? HealthMaximum { get; init; }
}