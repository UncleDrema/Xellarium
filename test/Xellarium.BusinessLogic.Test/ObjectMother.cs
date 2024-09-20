using Xellarium.BusinessLogic.Models;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Test;

public static class ObjectMother
{
    private class TestBaseModel : BaseModel { }
    
    public static Rule SimpleRule()
    {
        return new Rule
        {
            Id = 0,
            IsDeleted = false,
            DeletedAt = null,
            CreatedAt = default,
            UpdatedAt = default,
            GenericRule = new GenericRule(),
            Name = "Simple Rule",
            Owner = null,
            NeighborhoodId = 0,
            Collections = new List<Collection>()
        };
    }
    
    public static Rule SimpleRule(int nId)
    {
        return new Rule
        {
            Id = 0,
            IsDeleted = false,
            DeletedAt = null,
            CreatedAt = default,
            UpdatedAt = default,
            GenericRule = new GenericRule(),
            Name = "Simple Rule",
            Owner = null,
            NeighborhoodId = nId,
            Collections = new List<Collection>()
        };
    }

    public static Collection EmptyCollection()
    {
        return new Collection();
    }
    
    public static User SimpleUser()
    {
        return new User();
    }
    
    public static Neighborhood SimpleNeighborhood()
    {
        return new Neighborhood();
    }

    public static BaseModel SimpleBaseModel()
    {
        return new TestBaseModel();
    }
    
    public static World SimpleWorld()
    {
        return WorldOfSize(10, 10);
    }
    
    public static World WorldOfSize(int width, int height)
    {
        return new World(width, height);
    }
    
    public static World WorldOfCells(int[][] cells)
    {
        return new World(cells);
    }
}