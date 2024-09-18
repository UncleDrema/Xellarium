namespace Xellarium.Shared.DTO;

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public int WarningsCount { get; set; }
    public bool IsBlocked { get; set; }
    public List<RuleDTO> Rules { get; set; }
    public List<CollectionDTO> Collections { get; set; }
}