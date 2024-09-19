namespace Xellarium.BusinessLogic.Test.Models;

public class BaseModel
{
    [Fact]
    public void MarkCreated_MakesCreatedAtSet()
    {
        // Arrange
        var baseModel = ObjectMother.SimpleBaseModel();
        
        // Act
        baseModel.MarkCreated();
        
        // Assert
        Assert.NotEqual(default, baseModel.CreatedAt);
    }
    
    [Fact]
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
    
    [Fact]
    public void MarkUpdated_MakesUpdatedAtSet()
    {
        // Arrange
        var baseModel = ObjectMother.SimpleBaseModel();
        
        // Act
        baseModel.MarkUpdated();
        
        // Assert
        Assert.NotEqual(default, baseModel.UpdatedAt);
    }
    
    [Fact]
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
    
    [Fact]
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
    
    [Fact]
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