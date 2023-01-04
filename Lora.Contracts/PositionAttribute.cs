using System;

namespace MCP.DeviceDiscovery.Contracts;


[AttributeUsage(AttributeTargets.Property)]
internal class ArrayLengthAttribute : Attribute
{
    private readonly string[] _names;

    public ArrayLengthAttribute(params string[] names)
    {
        _names = names;
    }

    public bool RelatedTo(string memberName)
    {
        if (string.IsNullOrEmpty(memberName))
            throw new ArgumentException("Value cannot be null or empty.", nameof(memberName));
        
        foreach (var name in _names)
        {
            if (string.Compare(name, memberName, 
                    StringComparison.InvariantCultureIgnoreCase) == 0)
                return true;
        }
        return false;
    }
}


[AttributeUsage(AttributeTargets.All)]
public class PositionAttribute : Attribute
{
    public int Position { get; }
    public int ArrayElemCount { get; } 

    public PositionAttribute(int position)
    {
        Position = position;
    }
    public PositionAttribute(int position, int arrayElemCount)
    {
        ArrayElemCount = arrayElemCount;
        Position = position;
    }
}