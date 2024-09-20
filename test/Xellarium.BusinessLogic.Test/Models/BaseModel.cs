using Allure.Xunit.Attributes;

namespace Xellarium.BusinessLogic.Test.Models;

[AllureParentSuite("Business Logic")]
[AllureSuite("Models")]
[AllureSubSuite("Base Model")]
public class BaseModel
{
    [Fact(DisplayName = "Mark created makes CreatedAt set")]
    public void MarkCreated_MakesCreatedAtSet()
    {
        // Arrange
        var baseModel = ObjectMother.SimpleBaseModel();
        
        // Act
        baseModel.MarkCreated();
        
        // Assert
        Assert.NotEqual(default, baseModel.CreatedAt);
    }
    
    [Fact(DisplayName = "Double mark created throws exception")]
    public void DoubleMarkCreated_ThrowsException()
    {
        // Arrange
        var baseModel = ObjectMother.SimpleBaseModel();
        baseModel.MarkCreated();
        
        // Act
        var action = () => baseModel.MarkCreated();
        
        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
    
    [Fact(DisplayName = "Mark updated makes UpdatedAt set")]
    public void MarkUpdated_MakesUpdatedAtSet()
    {
        // Arrange
        var baseModel = ObjectMother.SimpleBaseModel();
        
        // Act
        baseModel.MarkUpdated();
        
        // Assert
        Assert.NotEqual(default, baseModel.UpdatedAt);
    }
    
    [Fact(DisplayName = "Second mark updated makes new UpdatedAt later in time")]
    public void SecondMarkUpdated_MakesNewUpdatedAtBigger()
    {
        // Arrange
        var baseModel = ObjectMother.SimpleBaseModel();
        baseModel.MarkUpdated();
        var oldUpdatedAt = baseModel.UpdatedAt;
        
        // Act
        baseModel.MarkUpdated();
        
        // Assert
        Assert.True(baseModel.UpdatedAt > oldUpdatedAt);
    }
    
    [Fact(DisplayName = "Double delete throws exception")]
    public void DoubleDelete_ThrowsException()
    {
        // Arrange
        var baseModel = ObjectMother.SimpleBaseModel();
        baseModel.Delete();
        
        // Act
        var action = () => baseModel.Delete();
        
        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
    
    [Fact(DisplayName = "Delete makes IsDeleted set")]
    public void Delete_MakesIsDeletedSet()
    {
        // Arrange
        var baseModel = ObjectMother.SimpleBaseModel();
        
        // Act
        baseModel.Delete();
        
        // Assert
        Assert.True(baseModel.IsDeleted);
    }
}