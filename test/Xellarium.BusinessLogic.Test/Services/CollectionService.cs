using Moq;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

public class CollectionServiceTests : IDisposable, IClassFixture<RepositoryMocks>
{
    private readonly RepositoryMocks _mocks;
    private readonly CollectionService _collectionService;

    public CollectionServiceTests(RepositoryMocks mocks)
    {
        _mocks = mocks;
        _collectionService = new CollectionService(_mocks.CollectionRepositoryMock.Object, _mocks.RuleRepositoryMock.Object);
    }

    [Fact]
    public async Task GetCollections_ReturnsAllNotDeletedCollections()
    {
        // Arrange
        var col1 = new Collection();
        var col2 = new Collection() { IsDeleted = true };
        _mocks.CollectionRepositoryMock.Setup(repo => repo.GetAll(false)).ReturnsAsync([col1]);

        // Act
        var result = await _collectionService.GetCollections();

        // Assert
        Assert.Equal([col1], result);
    }

    [Fact]
    public async Task GetCollection_ReturnsCollection_WhenExists()
    {
        // Arrange
        var collection = new Collection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);

        // Act
        var result = await _collectionService.GetCollection(1);

        // Assert
        Assert.Equal(collection, result);
    }

    [Fact]
    public async Task AddCollection_AddsNewCollection()
    {
        // Arrange
        var collection = new Collection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Add(collection, true)).Returns(Task.FromResult(collection));

        // Act
        await _collectionService.AddCollection(collection);

        // Assert
        _mocks.CollectionRepositoryMock.Verify(repo => repo.Add(collection, true), Times.Once);
    }
    
    [Fact]
    public async Task UpdateCollection_UpdatesCollection()
    {
        // Arrange
        var collection = new Collection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Exists(collection.Id, false)).ReturnsAsync(true);
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Update(collection)).Returns(Task.CompletedTask);

        // Act
        await _collectionService.UpdateCollection(collection);

        // Assert
        _mocks.CollectionRepositoryMock.Verify(repo => repo.Update(collection), Times.Once);
    }
    
    [Fact]
    public async Task DeleteCollection_SoftDeletesCollection()
    {
        // Arrange
        var collection = new Collection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), true)).ReturnsAsync(collection);
        _mocks.CollectionRepositoryMock.Setup(repo => repo.SoftDelete(It.IsAny<int>())).Returns(Task.CompletedTask);

        // Act
        await _collectionService.DeleteCollection(1);

        // Assert
        _mocks.CollectionRepositoryMock.Verify(repo => repo.SoftDelete(It.IsAny<int>()), Times.Once);
    }
    
    [Fact]
    public async Task AddRule_AddsNewRule()
    {
        // Arrange
        var rule = new Rule();
        var collection = new Collection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);

        // Act
        await _collectionService.AddRule(collection.Id, rule);

        // Assert
        _mocks.RuleRepositoryMock.Verify(repo => repo.Add(rule, true), Times.Once);
        Assert.Contains(rule, collection.Rules);
    }
    
    [Fact]
    public async Task RemoveRule_RemovesRule()
    {
        // Arrange
        var rule = new Rule();
        var collection = new Collection();
        collection.AddRule(rule);
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);

        // Act
        await _collectionService.RemoveRule(collection.Id, rule);

        // Assert
        Assert.DoesNotContain(rule, collection.Rules);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SetPrivacy_SetsPrivacy(bool isPrivate)
    {
        // Arrange
        var collection = new Collection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Update(collection)).Returns(Task.CompletedTask);

        // Act
        await _collectionService.SetPrivacy(collection.Id, isPrivate);

        // Assert
        Assert.Equal(isPrivate, collection.IsPrivate);
    }
    
    [Fact]
    public async Task CollectionExists_ReturnsTrue_WhenCollectionExists()
    {
        // Arrange
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Exists(It.IsAny<int>(), false)).ReturnsAsync(true);

        // Act
        var result = await _collectionService.CollectionExists(1);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task CollectionExists_ReturnsFalse_WhenCollectionDoesNotExist()
    {
        // Arrange
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Exists(It.IsAny<int>(), false)).ReturnsAsync(false);

        // Act
        var result = await _collectionService.CollectionExists(1);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task GetOwner_ReturnsOwner()
    {
        // Arrange
        var collection = new Collection() { Owner = new User() { Id = 1 }};
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);

        // Act
        var result = await _collectionService.GetOwner(collection.Id);

        // Assert
        Assert.Equal(collection.Owner, result);
    }
    
    [Fact]
    public async Task GetRuleCollections_ReturnsCollectionsWithRule()
    {
        // Arrange
        var col1 = new Collection();
        var col2 = new Collection();
        var rule = new Rule();
        col1.AddRule(rule);
        col2.AddRule(rule);
        
        _mocks.RuleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(rule);
        
        // Act
        var result = await _collectionService.GetRuleCollections(rule.Id);
        
        // Assert
        Assert.Contains(col1, result);
    }
    
    public void Dispose()
    {
        _mocks.VerifyAll();
        _mocks.Reset();
    }
}