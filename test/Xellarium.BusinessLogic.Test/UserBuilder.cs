using Xellarium.BusinessLogic.Models;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Test;

public class UserBuilder
{
    private int _id = 0;
    private int _warningsCount = 0;
    private string _name = "";
    private string _passwordHash = "";
    private UserRole _role = UserRole.User;
    private bool _isBlocked = false;
    private ICollection<Rule> _rules = new List<Rule>();
    private ICollection<Collection> _collections = new List<Collection>();
    
    public UserBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public UserBuilder WithWarnings(int warningsCount)
    {
        _warningsCount = warningsCount;
        return this;
    }
    
    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;
        return this;
    }
    
    public UserBuilder WithRole(UserRole role)
    {
        _role = role;
        return this;
    }
    
    public UserBuilder WithIsBlocked(bool isBlocked)
    {
        _isBlocked = isBlocked;
        return this;
    }
    
    public UserBuilder WithRules(params Rule[] rules)
    {
        _rules = rules;
        return this;
    }
    
    public UserBuilder WithCollections(params Collection[] collections)
    {
        _collections = collections;
        return this;
    }
    
    public User Build()
    {
        var res =  new User
        {
            Id = _id,
            WarningsCount = _warningsCount,
            Name = _name,
            PasswordHash = _passwordHash,
            Role = _role,
            IsBlocked = _isBlocked || _warningsCount >= 3,
            Rules = _rules,
            Collections = _collections
        };
        
        foreach (var rule in _rules)
        {
            rule.Owner = res;
        }
        
        foreach (var collection in _collections)
        {
            collection.Owner = res;
        }
        
        return res;
    }
}