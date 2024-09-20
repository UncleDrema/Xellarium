using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Test.Models;

[AllureParentSuite("Business Logic")]
[AllureSuite("Models")]
[AllureSubSuite("Collection")]
public class CollectionTests
{
    private readonly CollectionBuilder _builder;
    
    [AllureBefore("Create collection builder")]
    public CollectionTests()
    {
        _builder = new CollectionBuilder();
    }
    
    [Fact(DisplayName = "Add rule adds rule to collection")]
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

    [Fact(DisplayName = "Remove rule removes rule from collection")]
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

    [Fact(DisplayName = "Add rule throws exception if rule is null")]
    public void AddRule_ThrowsIfRuleIsNull()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        
        // Act
        var action = () => collection.AddRule(null);
        
        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact(DisplayName = "Remove rule throws exception if rule is null")]
    public void RemoveRule_ThrowsIfRuleIsNull()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        
        // Act
        var action = () => collection.RemoveRule(null);
        
        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact(DisplayName = "Remove rule throws exception if rule is not in collection")]
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