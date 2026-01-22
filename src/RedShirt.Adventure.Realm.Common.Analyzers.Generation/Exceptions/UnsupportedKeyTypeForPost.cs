using System;

namespace RedShirt.Adventure.Realm.Common.Analyzers.Generation.Exceptions;

public class UnsupportedKeyTypeForPost : Exception
{
    public UnsupportedKeyTypeForPost(string message) : base(message)
    {
    }
}