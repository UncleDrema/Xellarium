namespace Xellarium.Shared.Test;

public class GenericRuleTests
{
    public static TheoryData<int> InvalidStatesCount => new()
    {
        -1,
        0,
        1
    };
    
    [Theory]
    [MemberData(nameof(InvalidStatesCount), MemberType = typeof(GenericRuleTests))]
    public void Constructor_StatesLessThan2_ThrowsArgumentOutOfRangeException(int states)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GenericRule(states));
    }
    
    [Theory]
    [MemberData(nameof(InvalidStatesCount), MemberType = typeof(GenericRuleTests))]
    public void RuleBuilder_StatesLessThan2_ThrowsArgumentOutOfRangeException(int states)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GenericRule.Builder(states));
    }
    
    [Fact]
    public void RuleBuilder_WithTransitionAfterBuild_ThrowsInvalidOperationException()
    {
        var builder = new GenericRule.Builder(2);
        builder.Build();
        Assert.Throws<InvalidOperationException>(() => builder.WithTransition(0, 1, new Dictionary<int, IList<int>>()));
    }
    
    [Fact]
    public void RuleBuilder_BuildAfterBuild_ThrowsInvalidOperationException()
    {
        var builder = new GenericRule.Builder(2);
        builder.Build();
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }
    
    [Theory]
    [InlineData(2, -1)]
    [InlineData(2, 2)]
    public void RuleBuilder_WithTransitionInvalidFromState_ThrowsArgumentOutOfRangeException(int statesCount, int fromState)
    {
        var builder = new GenericRule.Builder(statesCount);
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithTransition(fromState, 1, new Dictionary<int, IList<int>>()));
    }
    
    // Проверить, что можно совершить успешный билд с несколькими установленными переходами
    [Fact]
    public void RuleBuilder_Build_Success()
    {
        var rule = new GenericRule.Builder(2)
            .WithTransition(0, 1, new Dictionary<int, IList<int>>())
            .WithTransition(1, 0, new Dictionary<int, IList<int>>())
            .Build();
        Assert.NotNull(rule);
    }
    
    // Протестировать успешный переход
    [Fact]
    public void RuleBuilder_WithTransition_Success()
    {
        // Получили правило
        var rule = new GenericRule.Builder(2)
            .WithTransition(0, 1, new Dictionary<int, IList<int>> { { 0, [1] } })
            .Build();
        
        // Проверим GetNextState
        var nextState = rule.GetNextState(0, new Dictionary<int, int>() { {0, 1} });
        Assert.Equal(1, nextState);
    }
    
    // Протестировать переход, который не произойдет из-за нехватки соседей
    [Fact]
    public void RuleBuilder_WithTransition_NotEnoughNeighbours()
    {
        // Получили правило
        var rule = new GenericRule.Builder(2)
            .WithTransition(0, 1, new Dictionary<int, IList<int>> { { 0, [1] } })
            .Build();
        
        // Проверим GetNextState
        var nextState = rule.GetNextState(0, new Dictionary<int, int>());
        Assert.Equal(0, nextState);
    }
    
    // Протестировать переход из состояния, из которого нет переходов
    [Fact]
    public void RuleBuilder_WithTransition_NoTransition()
    {
        // Получили правило
        var rule = new GenericRule.Builder(2).Build();
        
        // Проверим GetNextState
        var nextState = rule.GetNextState(0, new Dictionary<int, int>());
        Assert.Equal(0, nextState);
    }
    
    // Протестировать переход из некорректного состояния
    [Theory]
    [InlineData(2, -1)]
    [InlineData(2, 2)]
    public void RuleBuilder_GetNextState_InvalidState(int statesCount, int state)
    {
        var rule = new GenericRule.Builder(statesCount).Build();
        var nextState = rule.GetNextState(state, new Dictionary<int, int>());
        Assert.Equal(state, nextState);
    }
}