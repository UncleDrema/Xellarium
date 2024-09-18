namespace Xellarium.Shared;

public struct World
{
    public int[][] Cells { get; }
    public int Width { get; }
    public int Height { get; }

    public World(int[][] world)
    {
        Cells = world;
        Width = world.Length;
        Height = world[0].Length;
    }

    public World(int width, int height)
    {
        Cells = new int[width][];
        for (var i = 0; i < width; i++)
        {
            Cells[i] = new int[height];
        }
        
        Width = width;
        Height = height;
    }
    
    public int this[Vec2 vec2]
    {
        get => Cells[Utils.GetCyclicIndex(vec2.X, Width)][Utils.GetCyclicIndex(vec2.Y, Height)];
        set => Cells[Utils.GetCyclicIndex(vec2.X, Width)][Utils.GetCyclicIndex(vec2.Y, Height)] = value;
    }
    
    public int this[int x, int y]
    {
        get => this[new Vec2(x, y)];
        set => this[new Vec2(x, y)] = value;
    }
}