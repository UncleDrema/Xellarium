using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Test.Models;

public class CollectionTests
{
    [Fact]
    public void AddRule_AddsRule()
    {
        var collection = new Collection()
        {
            Rules = new List<Rule>()
        };
        var rule = new Rule();

        collection.AddRule(rule);

        Assert.Contains(rule, collection.Rules);
    }

    [Fact]
    public void RemoveRule_RemovesRule()
    {
        var collection = new Collection()
        {
            Rules = new List<Rule>()
        };
        var rule = new Rule();
        collection.AddRule(rule);

        collection.RemoveRule(rule);

        Assert.DoesNotContain(rule, collection.Rules);
    }
}