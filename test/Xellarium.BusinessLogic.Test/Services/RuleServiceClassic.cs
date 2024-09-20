using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

[AllureParentSuite("Business Logic")]
[AllureSuite("Services")]
[AllureSubSuite("RuleService Classic")]
public class RuleServiceClassic : IDisposable
{
    private readonly DatabaseFixture _databaseFixture;
    private readonly RuleService _ruleService;
    
    [AllureBefore("Connect to database")]
    public RuleServiceClassic()
    {
        _databaseFixture = new DatabaseFixture();
        
        _ruleService = new RuleService(
            _databaseFixture.RuleRepository,
            _databaseFixture.CollectionRepository);
    }
    
    [Fact(DisplayName = "GetRules returns all not deleted rules")]
    public async Task GetRules_ReturnsAllNotDeletedRules()
    {
        // Arrange
        var neighborhood = ObjectMother.SimpleNeighborhood();
        await _databaseFixture.NeighborhoodRepository.Add(neighborhood);
        var rule1 = new RuleBuilder()
            .WithNeighborhood(neighborhood)
            .Build();
        var rule2 = new RuleBuilder()
            .WithNeighborhood(neighborhood)
            .Build();
        rule2.Delete();
        var user = new UserBuilder()
            .WithRules(rule1, rule2)
            .Build();
        await _databaseFixture.UserRepository.Add(user, false);
        await _databaseFixture.RuleRepository.Add(rule1, false);
        await _databaseFixture.RuleRepository.Add(rule2, true);
        await _databaseFixture.Context.SaveChangesAsync();
        
        // Act
        var result = await _ruleService.GetRules();
        
        // Assert
        Assert.Equal([rule1], result);
    }
    
    [Fact(DisplayName = "GetRule returns rule when exists")]
    public async Task GetRule_ReturnsRule_WhenExists()
    {
        // Arrange
        var neighborhood = ObjectMother.SimpleNeighborhood();
        await _databaseFixture.NeighborhoodRepository.Add(neighborhood);
        var rule = new RuleBuilder()
            .WithNeighborhood(neighborhood)
            .Build();
        await _databaseFixture.RuleRepository.Add(rule);
        
        // Act
        var result = await _ruleService.GetRule(rule.Id);

        // Assert
        Assert.Equal(rule, result);
    }

    [Fact(DisplayName = "AddRule adds new rule")]
    public async Task AddRule_AddsNewRule()
    {
        var neighborhood = ObjectMother.SimpleNeighborhood();
        await _databaseFixture.NeighborhoodRepository.Add(neighborhood);
        var rule = new RuleBuilder()
            .WithNeighborhood(neighborhood)
            .Build();
        
        // Act
        await _ruleService.AddRule(rule);
        
        // Assert
        Assert.Single((await _databaseFixture.RuleRepository.GetAll()));
    }

    [AllureAfter("Clear database")]
    public void Dispose()
    {
        _databaseFixture.Dispose();
    }
}