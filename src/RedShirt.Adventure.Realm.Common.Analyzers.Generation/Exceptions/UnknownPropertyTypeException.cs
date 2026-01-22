using System;

namespace RedShirt.Adventure.Realm.Common.Analyzers.Generation.Exceptions;

public class UnknownPropertyTypeException : Exception
{
    public UnknownPropertyTypeException(string message) : base(message)
    {
    }
}