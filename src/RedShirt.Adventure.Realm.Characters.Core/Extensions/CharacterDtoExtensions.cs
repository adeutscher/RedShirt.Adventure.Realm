using RedShirt.Adventure.Realm.Characters.Core.Models;

namespace RedShirt.Adventure.Realm.Characters.Core.Extensions;

public static class CharacterDtoExtensions
{
    private const float Tolerance = 0.0001f;

    public static bool IsTheSameAs(this CharacterDto a, CharacterDto b)
    {
        return a.Id == b.Id
               && a.CreatedAt == b.CreatedAt
               && a.AccountId == b.AccountId
               && a.MapId == b.MapId
               && a.FirstName == b.FirstName
               && a.LastName == b.LastName
               && Math.Abs(a.PositionX - b.PositionX) < Tolerance
               && Math.Abs(a.PositionY - b.PositionY) < Tolerance
               && Math.Abs(a.PositionZ - b.PositionZ) < Tolerance
               && Math.Abs(a.Rotation - b.Rotation) < Tolerance
               && a.HealthCurrent == b.HealthCurrent
               && a.HealthMaximum == b.HealthMaximum;
    }
}