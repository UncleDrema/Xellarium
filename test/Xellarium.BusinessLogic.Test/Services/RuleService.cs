using Moq;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

public class RuleServiceTests : IDisposable, IClassFixture<RepositoryMocks>
{
    private readonly RepositoryMocks _mocks;
    private readonly RuleService _ruleService;

    public RuleServiceTests(RepositoryMocks mocks)
    {
        _mocks = mocks;
        _ruleService = new RuleService(_mocks.RuleRepositoryMock.Object, _mocks.CollectionRepositoryMock.Object);
    }
    
    [Fact]
    public async Task GetRules_ReturnsAllNotDeletedRules()
    {
        // Arrange
        var rule1 = new Rule();
        var rule2 = new Rule() { IsDeleted = true };
        _mocks.RuleRepositoryMock.Setup(repo => repo.GetAll(false)).ReturnsAsync([rule1]);

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
        _mocks.RuleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(rule);

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
        _mocks.RuleRepositoryMock.Setup(repo => repo.Add(rule, true)).Returns(Task.FromResult(rule));

        // Act
        await _ruleService.AddRule(rule);

        // Assert
        _mocks.RuleRepositoryMock.Verify(repo => repo.Add(rule, true), Times.Once);
    }
    
    [Fact]
    public async Task UpdateRule_UpdatesRule()
    {
        // Arrange
        var rule = new Rule();
        _mocks.RuleRepositoryMock.Setup(repo => repo.Exists(rule.Id, false)).ReturnsAsync(true);
        _mocks.RuleRepositoryMock.Setup(repo => repo.Update(rule)).Returns(Task.CompletedTask);

        // Act
        await _ruleService.UpdateRule(rule);

        // Assert
        _mocks.RuleRepositoryMock.Verify(repo => repo.Update(rule), Times.Once);
    }
    
    [Fact]
    public async Task DeleteRule_SoftDeletesRule()
    {
        // Arrange
        var rule = new Rule();
        _mocks.RuleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), true)).ReturnsAsync(rule);
        _mocks.RuleRepositoryMock.Setup(repo => repo.SoftDelete(It.IsAny<int>())).Returns(Task.CompletedTask);

        // Act
        await _ruleService.DeleteRule(1);

        // Assert
        _mocks.RuleRepositoryMock.Verify(repo => repo.SoftDelete(It.IsAny<int>()), Times.Once);
    }
    
    // test GetCollectionRules, RuleExists and GetOwner
    [Fact]
    public async Task GetCollectionRules_ReturnsAllRulesOfCollection()
    {
        // Arrange
        var rule1 = new Rule();
        var rule2 = new Rule();
        var collection = new Collection() { Rules = [rule1, rule2] };
        _mocks.CollectionRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(collection);
        
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
        _mocks.RuleRepositoryMock.Setup(repo => repo.Exists(rule.Id, false)).ReturnsAsync(true);

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
        _mocks.RuleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(rule);

        // Act
        var result = await _ruleService.GetOwner(rule.Id);

        // Assert
        Assert.Equal(rule.Owner, result);
    }
    
    public void Dispose()
    {
        _mocks.VerifyAll();
        _mocks.Reset();
    }
}