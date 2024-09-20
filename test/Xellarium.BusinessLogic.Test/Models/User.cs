using Allure.Xunit.Attributes;
using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Test.Models;

[AllureParentSuite("Business Logic")]
[AllureSuite("Models")]
[AllureSubSuite("User")]
public class UserTests
{
    // Проверим блокировку пользователя после трёх предупреждений
    [Fact(DisplayName = "Warning after two warnings blocks user")]
    public void AddWarning_ThreeWarnings_BlockUser()
    {
        // Arrange
        var user = new UserBuilder()
            .WithWarnings(2)
            .Build();
        
        // Act
        user.AddWarning();
        
        // Assert
        Assert.True(user.IsBlocked);
    }
    
    // Проверим, что после снятия предупреждения пользователь разблокируется
    [Fact(DisplayName = "Remove warning unblocks user if warnings count is less than 3")]
    public void RemoveWarning_ThreeWarnings_UnblockUser()
    {
        // Arrange
        var user = new UserBuilder()
            .WithWarnings(3)
            .Build();
        
        // Act
        user.RemoveWarning();
        
        // Assert
        Assert.False(user.IsBlocked);
    }
    
    [Fact(DisplayName = "Remove warning after four warnings remains user still blocked")]
    public void RemoveWarning_AfterFourWarnings_StillBlocked()
    {
        // Arrange
        var user = new UserBuilder()
            .WithWarnings(4)
            .Build();
        
        // Act
        user.RemoveWarning();
        
        // Assert
        Assert.Equal(3, user.WarningsCount);
        Assert.True(user.IsBlocked);
    }
    
    [Fact(DisplayName = "Remove warning when zero warnings throws exception")]
    public void RemoveWarning_WhenZeroWarnings_ThrowException()
    {
        // Arrange
        var user = new UserBuilder()
            .WithWarnings(0)
            .Build();
        
        // Act
        Action act = () => user.RemoveWarning();
        
        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
    
    [Fact(DisplayName = "Add collection to user makes user owner of the collection")]
    public void AddCollection_AddCollectionToUser()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var collection = ObjectMother.EmptyCollection();
        
        // Act
        user.AddCollection(collection);
        
        // Assert
        Assert.Contains(collection, user.Collections);
        Assert.Equal(user, collection.Owner);
    }
    
    [Fact(DisplayName = "Add null collection to user throws exception")]
    public void AddCollection_NullCollection_ThrowArgumentNullException()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        // Act
        Action act = () => user.AddCollection(null);
        
        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
    
    [Fact(DisplayName = "Add rule to user makes user owner of the rule")]
    public void AddRule_AddRuleToUser()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var rule = ObjectMother.SimpleRule();
        
        // Act
        user.AddRule(rule);
        
        // Assert
        Assert.Contains(rule, user.Rules);
        Assert.Equal(user, rule.Owner);
    }
    
    [Fact(DisplayName = "Add null rule to user throws exception")]
    public void AddRule_NullRule_ThrowArgumentNullException()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        // Act
        Action act = () => user.AddRule(null);
        
        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
}