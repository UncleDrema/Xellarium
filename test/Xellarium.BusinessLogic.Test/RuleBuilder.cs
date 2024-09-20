﻿using Xellarium.BusinessLogic.Models;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Test;

public class RuleBuilder
{
    private GenericRule _genericRule = new GenericRule();
    private string _name = "Game of Life";
    private User _owner = new User();
    private int _neighborhoodId = 0;
    private ICollection<Collection> _collections = new List<Collection>();
    
    public RuleBuilder WithGenericRule(GenericRule genericRule)
    {
        _genericRule = genericRule;
        return this;
    }
    
    public RuleBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public RuleBuilder WithOwner(User owner)
    {
        _owner = owner;
        return this;
    }
    
    public RuleBuilder WithNeighborhoodId(int neighborhoodId)
    {
        _neighborhoodId = neighborhoodId;
        return this;
    }
    
    public RuleBuilder WithNeighborhood(Neighborhood neighborhood)
    {
        _neighborhoodId = neighborhood.Id;
        return this;
    }
    
    public RuleBuilder WithCollections(params Collection[] collections)
    {
        _collections = collections;
        return this;
    }
    
    public Rule Build()
    {
        return new Rule
        {
            GenericRule = _genericRule,
            Name = _name,
            Owner = _owner,
            NeighborhoodId = _neighborhoodId,
            Collections = _collections
        };
    }
}