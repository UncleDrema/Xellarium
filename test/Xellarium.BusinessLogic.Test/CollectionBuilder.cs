using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Test;

public class CollectionBuilder
{
    private List<Rule> _rules = new();
    private User _user = ObjectMother.SimpleUser();
    private bool _isPrivate = false;
    private int _id = 0;
    private string _name = string.Empty;

    public CollectionBuilder WithRules(params Rule[] rules)
    {
        _rules.AddRange(rules);
        return this;
    }

    public CollectionBuilder WithUser(User user)
    {
        _user = user;
        return this;
    }

    public CollectionBuilder WithPrivateStatus(bool isPrivate)
    {
        _isPrivate = isPrivate;
        return this;
    }

    public CollectionBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CollectionBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public Collection Build()
    {
        return new Collection
        {
            Id = _id,
            Name = _name,
            IsPrivate = _isPrivate,
            Owner = _user,
            Rules = _rules
        };
    }
}