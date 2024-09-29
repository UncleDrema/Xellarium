using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Moq;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

[AllureParentSuite("Business Logic")]
[AllureSuite("Services")]
[AllureSubSuite("RuleService")]
public class RuleServiceTests : IDisposable, IClassFixture<RepositoryMocks>
{
    private readonly RepositoryMocks _mocks;
    private readonly RuleService _ruleService;

    [AllureBefore("Create rule service and mocks")]
    public RuleServiceTests(RepositoryMocks mocks)
    {
        _mocks = mocks;
        _ruleService = new RuleService(_mocks.RuleRepositoryMock.Object, _mocks.CollectionRepositoryMock.Object);
    }
    
    [Fact(DisplayName = "GetRules returns all not deleted rules")]
    public async Task GetRules_ReturnsAllNotDeletedRules()
    {
        // Arrange
        var rule1 = ObjectMother.SimpleRule();
        var rule2 = ObjectMother.SimpleRule();
        rule2.Delete();
        _mocks.RuleRepositoryMock.Setup(repo => repo.GetAll(false)).ReturnsAsync([rule1]);

        // Act
        var result = await _ruleService.GetRules();

        // Assert
        Assert.Equal([rule1], result);
    }
    
    [Fact(DisplayName = "GetRule returns rule when exists")]
    public async Task GetRule_ReturnsRule_WhenExists()
    {
        // Arrange
        var rule = ObjectMother.SimpleRule();
        _mocks.RuleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(rule);

        // Act
        var result = await _ruleService.GetRule(1);

        // Assert
        Assert.Equal(rule, result);
    }
    
    [Fact(DisplayName = "AddRule adds new rule")]
    public async Task AddRule_AddsNewRule()
    {
        // Arrange
        var rule = ObjectMother.SimpleRule();
        _mocks.RuleRepositoryMock.Setup(repo => repo.Add(rule, true)).Returns(Task.FromResult(rule));

        // Act
        await _ruleService.AddRule(rule);

        // Assert
        _mocks.RuleRepositoryMock.Verify(repo => repo.Add(rule, true), Times.Once);
    }
    
    [Fact(DisplayName = "UpdateRule updates rule")]
    public async Task UpdateRule_UpdatesRule()
    {
        // Arrange
        var rule = ObjectMother.SimpleRule();
        _mocks.RuleRepositoryMock.Setup(repo => repo.Exists(rule.Id, false)).ReturnsAsync(true);
        _mocks.RuleRepositoryMock.Setup(repo => repo.Update(rule)).Returns(Task.CompletedTask);

        // Act
        await _ruleService.UpdateRule(rule);

        // Assert
        _mocks.RuleRepositoryMock.Verify(repo => repo.Update(rule), Times.Once);
    }
    
    [Fact(DisplayName = "DeleteRule soft deletes rule")]
    public async Task DeleteRule_SoftDeletesRule()
    {
        // Arrange
        var rule = ObjectMother.SimpleRule();
        _mocks.RuleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), true)).ReturnsAsync(rule);
        _mocks.RuleRepositoryMock.Setup(repo => repo.SoftDelete(It.IsAny<int>())).Returns(Task.CompletedTask);

        // Act
        await _ruleService.DeleteRule(1);

        // Assert
        _mocks.RuleRepositoryMock.Verify(repo => repo.SoftDelete(It.IsAny<int>()), Times.Once);
    }
    
    [Fact(DisplayName = "GetRuleCollections returns collections with rule")]
    public async Task GetRuleCollections_ReturnsCollectionsWithRule()
    {
        // Arrange
        var col1 = ObjectMother.EmptyCollection();
        var col2 = ObjectMother.EmptyCollection();
        var rule = ObjectMother.SimpleRule();
        col1.AddRule(rule);
        col2.AddRule(rule);
        
        _mocks.RuleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(rule);
        
        // Act
        var result = await _ruleService.GetRuleCollections(rule.Id);
        
        // Assert
        Assert.Contains(col1, result);
    }
    
    [Fact(DisplayName = "RuleExists returns true when rule exists")]
    public async Task RuleExists_ReturnsTrue_WhenRuleExists()
    {
        // Arrange
        var rule = ObjectMother.SimpleRule();
        _mocks.RuleRepositoryMock.Setup(repo => repo.Exists(rule.Id, false)).ReturnsAsync(true);

        // Act
        var result = await _ruleService.RuleExists(rule.Id);

        // Assert
        Assert.True(result);
    }
    
    [Fact(DisplayName = "GetOwner returns owner of rule")]
    public async Task GetOwner_ReturnsOwner()
    {
        // Arrange
        var rule = new RuleBuilder()
            .WithOwner(new UserBuilder()
                .WithId(1)
                .Build())
            .Build();
        _mocks.RuleRepositoryMock.Setup(repo => repo.Get(It.IsAny<int>(), false)).ReturnsAsync(rule);

        // Act
        var result = await _ruleService.GetOwner(rule.Id);

        // Assert
        Assert.Equal(rule.Owner, result);
    }
    
    [AllureAfter("Verify and reset all mocks")]
    public void Dispose()
    {
        _mocks.VerifyAll();
        _mocks.Reset();
    }
}