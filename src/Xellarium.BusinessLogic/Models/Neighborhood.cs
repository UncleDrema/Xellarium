using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Models;

public class Neighborhood : BaseModel
{
    public string Name { get; init; } = "";
    public Vec2[] Offsets { get; init; } = Array.Empty<Vec2>();
    
    public static Vec2[] MooreOffsets =>
    [
        (-1, -1), (0, -1), (1, -1),
        (-1,  0),          (1,  0),
        (-1,  1), (0,  1), (1,  1)
    ];
    
    public static Vec2[] NeumannOffsets =>
    [
        (0, -1),
        (-1,  0),          (1,  0),
        (0,  1)
    ];
    
    public static Vec2[] LeftRightOffsets =>
    [
        (-1,  0),          (1,  0)
    ];
}