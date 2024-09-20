using Allure.Xunit.Attributes;

namespace Xellarium.Shared.Test;

[AllureParentSuite("Shared Logic")]
[AllureSuite("Business Logic")]
[AllureSubSuite("Transition")]
public class TransitionTests
{
    [Fact(DisplayName = "IsSatisfiedBy returns true when all requirements are met")]
    public void IsSatisfiedBy_ReturnsTrue_WhenAllRequirementsAreMet()
    {
        // Arrange
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

        // Act
        var result = transition.IsSatisfiedBy(neighbours);

        // Assert
        Assert.True(result);
    }
    
    [Fact(DisplayName = "IsSatisfiedBy returns false when no requirements are met")]
    public void IsSatisfiedBy_ReturnsFalse_WhenNoRequirementsMet()
    {
        // Arrange
        var transition = new Transition(1, new Dictionary<int, IList<int>>
        {
            {1, [1, 3]}
        });
        
        var neighbours = new Dictionary<int, int>
        {
            {0, 1},
            {1, 2}
        };

        // Act
        var result = transition.IsSatisfiedBy(neighbours);

        
        // Assert
        Assert.False(result);
    }
    
    [Fact(DisplayName = "IsSatisfiedBy returns true when one of requirements is met")]
    public void IsSatisfiedBy_ReturnsTrue_WhenOneOfRequirementsIsMet()
    {
        // Arrange
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
        
        // Act
        var result = transition.IsSatisfiedBy(neighbours);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact(DisplayName = "IsSatisfiedBy returns true when no requirements are set")]
    public void IsSatisfiedBy_ReturnsTrue_WhenNoRequirementsAreSet()
    {
        // Arrange
        var transition = new Transition(1);

        var neighbours = new Dictionary<int, int>
        {
            {0, 1},
            {1, 1}
        };
        
        // Act
        var result = transition.IsSatisfiedBy(neighbours);
        
        // Assert
        Assert.True(result);
    }
}