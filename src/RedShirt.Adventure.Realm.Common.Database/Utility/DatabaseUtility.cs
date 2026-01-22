namespace RedShirt.Adventure.Realm.Common.Database.Utility;

public static class DatabaseUtility
{
    public static string QuoteResource(string input)
    {
        return $"`{input}`";
    }
}