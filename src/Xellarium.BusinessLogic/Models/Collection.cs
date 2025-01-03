﻿using Xellarium.Tracing;

namespace Xellarium.BusinessLogic.Models;

/**
 * Коллекция
 * Содержит информацию о коллекции правил
 * - Название
 * - Является ли коллекция приватной
 * - Владелец (User)
 * - Правила, входящие в коллекцию (Rule)
 */
public class Collection : BaseModel
{
    public string Name { get; set; }
    public bool IsPrivate { get; set; }
    public virtual User Owner { get; set; }
    public virtual ICollection<Rule> Rules { get; set; } = new List<Rule>();
    
    public void AddRule(Rule rule)
    {
        using var activity = XellariumTracing.StartActivity();
        ArgumentNullException.ThrowIfNull(rule);
        Rules.Add(rule);
        rule.Collections.Add(this);
    }
    
    public void RemoveRule(Rule rule)
    {
        using var activity = XellariumTracing.StartActivity();
        ArgumentNullException.ThrowIfNull(rule);
        if (!Rules.Contains(rule))
            throw new InvalidOperationException($"Rule {rule.Name} is not in collection {Name}.");
        Rules.Remove(rule);
        rule.Collections.Remove(this);
    }
}