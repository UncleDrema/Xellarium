using Allure.Xunit.Attributes;

namespace Xellarium.Shared.Test;

[AllureParentSuite("Shared Logic")]
[AllureSuite("Business Logic")]
[AllureSubSuite("Generic Rule")]
public class GenericRuleTests
{
    public static TheoryData<int> InvalidStatesCount => new()
    {
        -1,
        0,
        1
    };
    
    [Theory(DisplayName = "GenericRule Constructor with invalid states count throws ArgumentOutOfRangeException")]
    [MemberData(nameof(InvalidStatesCount), MemberType = typeof(GenericRuleTests))]
    public void Constructor_StatesLessThan2_ThrowsArgumentOutOfRangeException(int states)
    {
        // Act
        var action = () => new GenericRule(states);
        
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
    
    [Theory(DisplayName = "GenericRule.Builder consructor with invalid states count throws ArgumentOutOfRangeException")]
    [MemberData(nameof(InvalidStatesCount), MemberType = typeof(GenericRuleTests))]
    public void RuleBuilder_StatesLessThan2_ThrowsArgumentOutOfRangeException(int states)
    {
        // Act
        var action = () => new GenericRule.Builder(states);
        
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
    
    [Fact(DisplayName = "RuleBuilder's WithTransition call after build throws InvalidOperationException")]
    public void RuleBuilder_WithTransitionAfterBuild_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new GenericRule.Builder(2);
        builder.Build();
        
        // Act
        var action = () => builder.WithTransition(0, 1, new Dictionary<int, IList<int>>());
        
        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
    
    [Fact(DisplayName = "RuleBuilder's Build call after build throws InvalidOperationException")]
    public void RuleBuilder_BuildAfterBuild_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new GenericRule.Builder(2);
        builder.Build();
        
        // Act
        var action = () => builder.Build();
        
        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
    
    [Theory(DisplayName = "RuleBuilder's WithTransition with invalid from state throws ArgumentOutOfRangeException")]
    [InlineData(2, -1)]
    [InlineData(2, 2)]
    public void RuleBuilder_WithTransitionInvalidFromState_ThrowsArgumentOutOfRangeException(int statesCount, int fromState)
    {
        // Arrange
        var builder = new GenericRule.Builder(statesCount);
        
        // Act
        var action = () => builder.WithTransition(fromState, 1, new Dictionary<int, IList<int>>());
        
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
    
    // Проверить, что можно совершить успешный билд с несколькими установленными переходами
    [Fact(DisplayName = "RuleBuilder build success")]
    public void RuleBuilder_Build_Success()
    {
        // Arrange
        var builder = new GenericRule.Builder(2)
            .WithTransition(0, 1, new Dictionary<int, IList<int>>())
            .WithTransition(1, 0, new Dictionary<int, IList<int>>());
        
        // Act
        var rule = builder.Build();
        
        // Assert
        Assert.NotNull(rule);
    }
    
    // Протестировать успешный переход
    [Fact(DisplayName = "Transition success leads to correct state")]
    public void RuleBuilder_WithTransition_Success()
    {
        // Arrange
        var rule = new GenericRule.Builder(2)
            .WithTransition(0, 1, new Dictionary<int, IList<int>> { { 0, [1] } })
            .Build();
        
        // Act
        var nextState = rule.GetNextState(0, new Dictionary<int, int>() { {0, 1} });
        
        // Assert
        Assert.Equal(1, nextState);
    }
    
    // Протестировать переход, который не произойдет из-за нехватки соседей
    [Fact(DisplayName = "Transition with not enough neighbours leads to same state")]
    public void RuleBuilder_WithTransition_NotEnoughNeighbours()
    {
        // Arrange
        var rule = new GenericRule.Builder(2)
            .WithTransition(0, 1, new Dictionary<int, IList<int>> { { 0, [1] } })
            .Build();
        
        // Act
        var nextState = rule.GetNextState(0, new Dictionary<int, int>());
        
        // Assert
        Assert.Equal(0, nextState);
    }
    
    // Протестировать переход из состояния, из которого нет переходов
    [Fact(DisplayName = "Transition from state without transitions leads to same state")]
    public void RuleBuilder_WithTransition_NoTransition()
    {
        // Arrange
        var rule = new GenericRule.Builder(2).Build();
        
        // Act
        var nextState = rule.GetNextState(0, new Dictionary<int, int>());
        
        // Assert
        Assert.Equal(0, nextState);
    }
    
    // Протестировать переход из некорректного состояния
    [Theory(DisplayName = "GetNextState with invalid state returns same state")]
    [InlineData(2, -1)]
    [InlineData(2, 2)]
    public void RuleBuilder_GetNextState_InvalidState(int statesCount, int state)
    {
        // Arrange
        var rule = new GenericRule.Builder(statesCount).Build();
        
        // Act
        var nextState = rule.GetNextState(state, new Dictionary<int, int>());
        
        // Assert
        Assert.Equal(state, nextState);
    }
}