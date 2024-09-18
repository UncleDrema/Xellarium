namespace Xellarium.Shared.DTO;

public class NeighborhoodDTO
{
    public int Id { get; set; } = -1;
    public string Name { get; set; } = string.Empty;
    public List<Vec2> Offsets { get; set; } = new();
}