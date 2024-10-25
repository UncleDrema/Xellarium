using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Moq;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

[AllureParentSuite("Business Logic")]
[AllureSuite("Services")]
[AllureSubSuite("CollectionService")]
public class CollectionServiceTests : IDisposable, IClassFixture<RepositoryMocks>
{
    private readonly RepositoryMocks _mocks;
    private readonly CollectionService _collectionService;

    [AllureBefore("Create collection service and mocks")]
    public CollectionServiceTests(RepositoryMocks mocks)
    {
        _mocks = mocks;
        _collectionService = new CollectionService(_mocks.GetUnitOfWork());
    }

    [Fact(DisplayName = "GetCollections returns all not deleted collections")]
    public async Task GetCollections_ReturnsAllNotDeletedCollections()
    {
        throw new Exception("Test is broken");
        // Arrange
        var col1 = ObjectMother.EmptyCollection();
        var col2 = ObjectMother.EmptyCollection();
        col2.Delete();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.GetAllInclude()).ReturnsAsync([col1]);

        // Act
        var result = await _collectionService.GetCollections();

        // Assert
        Assert.Equal([col1], result);
    }

    [Fact(DisplayName = "GetCollection returns collection when exists")]
    public async Task GetCollection_ReturnsCollection_WhenExists()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.GetInclude(It.IsAny<int>())).ReturnsAsync(collection);

        // Act
        var result = await _collectionService.GetCollection(It.IsAny<int>());

        // Assert
        Assert.Equal(collection, result);
    }

    [Fact(DisplayName = "AddCollection adds new collection")]
    public async Task AddCollection_AddsNewCollection()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Add(collection)).Returns(Task.FromResult(collection));

        // Act
        await _collectionService.AddCollection(collection);

        // Assert
        _mocks.CollectionRepositoryMock.Verify(repo => repo.Add(collection), Times.Once);
    }
    
    [Fact(DisplayName = "UpdateCollection updates collection")]
    public async Task UpdateCollection_UpdatesCollection()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Exists(collection.Id, false)).ReturnsAsync(true);
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Update(collection)).Returns(Task.CompletedTask);

        // Act
        await _collectionService.UpdateCollection(collection);

        // Assert
        _mocks.CollectionRepositoryMock.Verify(repo => repo.Update(collection), Times.Once);
    }
    
    [Fact(DisplayName = "DeleteCollection soft deletes collection")]
    public async Task DeleteCollection_SoftDeletesCollection()
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);
        _mocks.CollectionRepositoryMock.Setup(repo => repo.SoftDelete(It.IsAny<int>())).Returns(Task.CompletedTask);

        // Act
        await _collectionService.DeleteCollection(1);

        // Assert
        _mocks.CollectionRepositoryMock.Verify(repo => repo.SoftDelete(It.IsAny<int>()), Times.Once);
    }
    
    [Fact(DisplayName = "AddRule adds new rule")]
    public async Task AddRule_AddsNewRule()
    {
        // Arrange
        var rule = ObjectMother.SimpleRule();
        var collection = ObjectMother.EmptyCollection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.GetInclude(It.IsAny<int>())).ReturnsAsync(collection);

        // Act
        await _collectionService.AddRule(collection.Id, rule);

        // Assert
        Assert.Contains(rule, collection.Rules);
    }
    
    [Fact(DisplayName = "RemoveRule removes rule")]
    public async Task RemoveRule_RemovesRule()
    {
        // Arrange
        var rule = ObjectMother.SimpleRule();
        var collection = ObjectMother.EmptyCollection();
        collection.AddRule(rule);
        _mocks.CollectionRepositoryMock.Setup(repo => repo.GetInclude(It.IsAny<int>())).ReturnsAsync(collection);

        // Act
        await _collectionService.RemoveRule(collection.Id, rule.Id);

        // Assert
        Assert.DoesNotContain(rule, collection.Rules);
    }
    
    [Theory(DisplayName = "SetPrivacy sets corresponding privacy")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SetPrivacy_SetsPrivacy(bool isPrivate)
    {
        // Arrange
        var collection = ObjectMother.EmptyCollection();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Update(collection)).Returns(Task.CompletedTask);

        // Act
        await _collectionService.SetPrivacy(collection.Id, isPrivate);

        // Assert
        Assert.Equal(isPrivate, collection.IsPrivate);
    }
    
    [Fact(DisplayName = "GetCollectionRules returns all rules of collection")]
    public async Task CollectionExists_ReturnsTrue_WhenCollectionExists()
    {
        // Arrange
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Exists(It.IsAny<int>(), false)).ReturnsAsync(true);

        // Act
        var result = await _collectionService.CollectionExists(1);

        // Assert
        Assert.True(result);
    }
    
    [Fact(DisplayName = "CollectionExists returns false when collection does not exist")]
    public async Task CollectionExists_ReturnsFalse_WhenCollectionDoesNotExist()
    {
        // Arrange
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Exists(It.IsAny<int>(), false)).ReturnsAsync(false);

        // Act
        var result = await _collectionService.CollectionExists(1);

        // Assert
        Assert.False(result);
    }
    
    [Fact(DisplayName = "GetOwner returns owner of collection")]
    public async Task GetOwner_ReturnsOwner()
    {
        // Arrange
        var collection = new Collection() { Owner = new User() { Id = 1 }};
        _mocks.CollectionRepositoryMock.Setup(repo => repo.GetInclude(It.IsAny<int>())).ReturnsAsync(collection);

        // Act
        var result = await _collectionService.GetOwner(collection.Id);

        // Assert
        Assert.Equal(collection.Owner, result);
    }
    
    [Fact(DisplayName = "GetCollectionRules returns all rules of collection")]
    public async Task GetCollectionRules_ReturnsAllRulesOfCollection()
    {
        // Arrange
        var rule1 = ObjectMother.SimpleRule();
        var rule2 = ObjectMother.SimpleRule();
        var collection = new CollectionBuilder()
            .WithRules(rule1, rule2)
            .Build();
        _mocks.CollectionRepositoryMock.Setup(repo => repo.GetInclude(It.IsAny<int>())).ReturnsAsync(collection);
        
        // Act
        var result = await _collectionService.GetCollectionRules(collection.Id);
        
        // Assert
        Assert.Equal(collection.Rules, result);
    }
    
    [AllureAfter("Verify and reset all mocks")]
    public void Dispose()
    {
        _mocks.VerifyAll();
        _mocks.Reset();
    }
}