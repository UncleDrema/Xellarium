namespace Xellarium.Shared.DTO;

public class CollectionDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsPrivate { get; set; }
    public int OwnerId { get; set; }
    public List<RuleReferenceDTO> RuleReferences { get; set; }
}