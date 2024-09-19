using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Test;

public static class ObjectMother
{
    public static Rule SimpleRule()
    {
        return new Rule();
    }

    public static Collection EmptyCollection()
    {
        return new Collection();
    }
}