namespace Xellarium.Shared.DTO;

public class PostRuleDTO
{
    public int Id { get; set; }
    public GenericRule GenericRule { get; set; }
    public int NeighborhoodId { get; set; }
    public string Name { get; set; }
}