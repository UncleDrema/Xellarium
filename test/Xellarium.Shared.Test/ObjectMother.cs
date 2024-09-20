namespace Xellarium.Shared.Test;

public static class ObjectMother
{
    public static DefaultDictionary<TK, TV> CreateDefaultDictionary<TK, TV>()
    {
        return new DefaultDictionary<TK, TV>();
    }
    
    public static DefaultDictionary<TK, TV> CreateDefaultDictionary<TK, TV>(TV defaultValue)
    {
        return new DefaultDictionary<TK, TV>(defaultValue);
    }
    
    public static DefaultDictionary<TK, TV> CreateDefaultDictionary<TK, TV>(Func<TK, TV> factory)
    {
        return new DefaultDictionary<TK, TV>(factory);
    }
}