using RedShirt.Adventure.Realm.Characters.Core.Models.Requests;

namespace RedShirt.Adventure.Realm.Characters.Core.Extensions;

public static class CharacterDtoPatchRequestExtensions
{
    public static bool AreChangesRequested(this CharacterDtoPatchRequest obj)
    {
        return !string.IsNullOrWhiteSpace(obj.FirstName)
               || !string.IsNullOrWhiteSpace(obj.LastName)
               || !string.IsNullOrWhiteSpace(obj.AccountId)
               || obj.MapId.HasValue
               || obj.PositionX.HasValue
               || obj.PositionY.HasValue
               || obj.PositionZ.HasValue
               || obj.Rotation.HasValue
               || obj.HealthCurrent.HasValue
               || obj.HealthMaximum.HasValue;
    }
}