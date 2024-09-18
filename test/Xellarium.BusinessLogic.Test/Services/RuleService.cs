using Moq;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

public class RuleServiceTests : IDisposable
{
    private readonly Mock<ICollectionRepository> _collectionRepositoryMock;
    private readonly Mock<IRuleRepository> _ruleRepositoryMock;
    private readonly RuleService _ruleService;

    public RuleServiceTests()
    {
        _collectionRepositoryMock = new Mock<ICollectionRepository>();
        _ruleRepositoryMock = new Mock<IRuleRepository>();
        _ruleService = new RuleService(_ruleRepositoryMock.Object, _collectionRepositoryMock.Object);
    }
    
    [Fact]
    public async Task GetRules_ReturnsAllNotDeletedRules()
    {
        // Arrange
        var rule1 = new Rule();
        var rule2 = new Rule() { IsDeleted = true };
        _ruleRepositoryMock.Setup(repo => repo.GetAll(false)).ReturnsAsync([rule1]);

        // Act
        var result = await _ruleService.GetRules();

        // Assert
        Assert.Equal([rule1], result);
    }
    
    [Fact]
    public async Task GetRule_ReturnsRule_WhenExists()
    {
        // Arrange
        var rule = new Rule();
        _ruleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(rule);

        // Act
        var result = await _ruleService.GetRule(1);

        // Assert
        Assert.Equal(rule, result);
    }
    
    [Fact]
    public async Task AddRule_AddsNewRule()
    {
        // Arrange
        var rule = new Rule();
        _ruleRepositoryMock.Setup(repo => repo.Add(rule, true)).Returns(Task.FromResult(rule));

        // Act
        await _ruleService.AddRule(rule);

        // Assert
        _ruleRepositoryMock.Verify(repo => repo.Add(rule, true), Times.Once);
    }
    
    [Fact]
    public async Task UpdateRule_UpdatesRule()
    {
        // Arrange
        var rule = new Rule();
        _ruleRepositoryMock.Setup(repo => repo.Exists(rule.Id, false)).ReturnsAsync(true);
        _ruleRepositoryMock.Setup(repo => repo.Update(rule)).Returns(Task.CompletedTask);

        // Act
        await _ruleService.UpdateRule(rule);

        // Assert
        _ruleRepositoryMock.Verify(repo => repo.Update(rule), Times.Once);
    }
    
    [Fact]
    public async Task DeleteRule_SoftDeletesRule()
    {
        // Arrange
        var rule = new Rule();
        _ruleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), true)).ReturnsAsync(rule);
        _ruleRepositoryMock.Setup(repo => repo.SoftDelete(It.IsAny<int>())).Returns(Task.CompletedTask);

        // Act
        await _ruleService.DeleteRule(1);

        // Assert
        _ruleRepositoryMock.Verify(repo => repo.SoftDelete(It.IsAny<int>()), Times.Once);
    }
    
    // test GetCollectionRules, RuleExists and GetOwner
    [Fact]
    public async Task GetCollectionRules_ReturnsAllRulesOfCollection()
    {
        // Arrange
        var rule1 = new Rule();
        var rule2 = new Rule();
        var collection = new Collection() { Rules = [rule1, rule2] };
        _collectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);
        
        // Act
        var result = await _ruleService.GetCollectionRules(collection.Id);
        
        // Assert
        Assert.Equal(collection.Rules, result);
    }
    
    [Fact]
    public async Task RuleExists_ReturnsTrue_WhenRuleExists()
    {
        // Arrange
        var rule = new Rule();
        _ruleRepositoryMock.Setup(repo => repo.Exists(rule.Id, false)).ReturnsAsync(true);

        // Act
        var result = await _ruleService.RuleExists(rule.Id);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task GetOwner_ReturnsOwner()
    {
        // Arrange
        var rule = new Rule() { Owner = new User() { Id = 1 }};
        _ruleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(rule);

        // Act
        var result = await _ruleService.GetOwner(rule.Id);

        // Assert
        Assert.Equal(rule.Owner, result);
    }
    
    public void Dispose()
    {
        _collectionRepositoryMock.VerifyAll();
        _ruleRepositoryMock.VerifyAll();
        _collectionRepositoryMock.Reset();
        _ruleRepositoryMock.Reset();
    }
}