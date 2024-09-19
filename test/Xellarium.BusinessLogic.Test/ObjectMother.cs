using Xellarium.BusinessLogic.Models;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Test;

public static class ObjectMother
{
    private class TestBaseModel : BaseModel { }
    
    public static Rule SimpleRule()
    {
        return new Rule();
    }

    public static Collection EmptyCollection()
    {
        return new Collection();
    }
    
    public static User SimpleUser()
    {
        return new User();
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