namespace Xellarium.Shared;

public record struct Vec2(int X, int Y)
{
    public static implicit operator (int, int)(Vec2 vec)
    {
        return (vec.X, vec.Y);
    }

    public static implicit operator Vec2((int, int) tuple)
    {
        return new Vec2(tuple.Item1, tuple.Item2);
    }
    
    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        return new Vec2(a.X + b.X, a.Y + b.Y);
    }
    
    public static Vec2 operator -(Vec2 a, Vec2 b)
    {
        return new Vec2(a.X - b.X, a.Y - b.Y);
    }
}