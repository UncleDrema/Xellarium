using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Test.Models;

public class UserTests
{
    // Проверим блокировку пользователя после трёх предупреждений
    [Fact]
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
    [Fact]
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
    
    [Fact]
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
    
    [Fact]
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
    
    [Fact]
    public void AddCollection_AddCollectionToUser()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var collection = new Collection();
        
        // Act
        user.AddCollection(collection);
        
        // Assert
        Assert.Contains(collection, user.Collections);
        Assert.Equal(user, collection.Owner);
    }
    
    [Fact]
    public void AddCollection_NullCollection_ThrowArgumentNullException()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        // Act
        Action act = () => user.AddCollection(null);
        
        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
    
    [Fact]
    public void AddRule_AddRuleToUser()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var rule = new Rule();
        
        // Act
        user.AddRule(rule);
        
        // Assert
        Assert.Contains(rule, user.Rules);
        Assert.Equal(user, rule.Owner);
    }
    
    [Fact]
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