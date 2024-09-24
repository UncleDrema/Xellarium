namespace Xellarium.Shared.DTO;

public class RuleDTO
{
    public int Id { get; set; }
    public GenericRule GenericRule { get; set; }
    public string Name { get; set; }
    public int OwnerId { get; set; }
    public int NeighborhoodId { get; set; }
    public List<CollectionReferenceDTO> Collections { get; set; }
}