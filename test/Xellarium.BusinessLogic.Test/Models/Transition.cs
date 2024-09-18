using Xellarium.BusinessLogic.Models;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Test.Models;

public class TransitionTests
{
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenAllRequirementsAreMet()
    {
        var transition = new Transition(1, new Dictionary<int, IList<int>>
        {
            {0, [1]},
            {1, [1]}
        });

        var neighbours = new Dictionary<int, int>
        {
            {0, 1},
            {1, 1}
        };

        Assert.True(transition.IsSatisfiedBy(neighbours));
    }
    
    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenNoRequirementsMet()
    {
        var transition = new Transition(1, new Dictionary<int, IList<int>>
        {
            {1, [1, 3]}
        });

        var neighbours = new Dictionary<int, int>
        {
            {0, 1},
            {1, 2}
        };

        Assert.False(transition.IsSatisfiedBy(neighbours));
    }
    
    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenOneOfRequirementsIsMet()
    {
        var transition = new Transition(1, new Dictionary<int, IList<int>>
        {
            {0, [1]},
            {1, [1, 2, 3]}
        });

        var neighbours = new Dictionary<int, int>
        {
            {0, 0},
            {1, 2}
        };

        Assert.True(transition.IsSatisfiedBy(neighbours));
    }
    
    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenNoRequirementsAreSet()
    {
        var transition = new Transition(1);

        var neighbours = new Dictionary<int, int>
        {
            {0, 1},
            {1, 1}
        };

        Assert.True(transition.IsSatisfiedBy(neighbours));
    }
}