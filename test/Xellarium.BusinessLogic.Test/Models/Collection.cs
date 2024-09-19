using Allure.Xunit.Attributes;
using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Test.Models;

public class CollectionTests
{
    private readonly CollectionBuilder _builder;
    
    public CollectionTests()
    {
        _builder = new CollectionBuilder();
    }
    
    [Fact]
    public void AddRule_AddsRule()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        var rule = ObjectMother.SimpleRule();
        
        // Act
        collection.AddRule(rule);

        // Assert
        Assert.Contains(rule, collection.Rules);
    }

    [Fact]
    public void RemoveRule_RemovesRule()
    {
        // Arrange
        var rule = ObjectMother.SimpleRule();
        var collection = _builder
            .WithRules(rule)
            .Build();

        // Act
        collection.RemoveRule(rule);

        // Assert
        Assert.DoesNotContain(rule, collection.Rules);
    }

    [Fact]
    public void AddRule_ThrowsIfRuleIsNull()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        
        // Act
        var action = () => collection.AddRule(null);
        
        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void RemoveRule_ThrowsIfRuleIsNull()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        
        // Act
        var action = () => collection.RemoveRule(null);
        
        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void RemoveRule_ThrowsIfRuleNotInCollection()
    {
        // Arrange
        var collection = _builder
            .WithRules(new Rule())
            .Build();
        
        // Act
        var action = () => collection.RemoveRule(new Rule());
        
        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
}