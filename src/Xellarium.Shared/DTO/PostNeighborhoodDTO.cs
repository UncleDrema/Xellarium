namespace Xellarium.Shared.DTO;

public class PostNeighborhoodDTO
{
    public string Name { get; set; } = string.Empty;
    public List<Vec2> Offsets { get; set; } = new();
}