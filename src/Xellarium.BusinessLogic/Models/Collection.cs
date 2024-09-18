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
        Rules.Add(rule);
        rule.Collections.Add(this);
    }
    
    public void RemoveRule(Rule rule)
    {
        Rules.Remove(rule);
        rule.Collections.Remove(this);
    }
}