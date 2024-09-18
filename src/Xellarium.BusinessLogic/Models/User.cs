using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Models;
/**
 * Пользователь
 * Содержит информацию о пользователе: имя, данные авторизации, число предупреждений, заблокирован ли пользователь
 */
public class User : BaseModel
{
    public string Name { get; init; } = "";
    public string PasswordHash { get; set; } = "";
    public UserRole Role { get; set; } = UserRole.User;
    public int WarningsCount { get; set; } = 0;
    public bool IsBlocked { get; set; } = false;
    public virtual ICollection<Rule> Rules { get; set; } = new List<Rule>();
    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
    
    public void AddWarning()
    {
        WarningsCount++;
        if (WarningsCount >= 3)
        {
            IsBlocked = true;
        }
    }
    
    public void RemoveWarning()
    {
        WarningsCount--;
        if (WarningsCount < 3)
        {
            IsBlocked = false;
        }
    }

    public void AddCollection(Collection collection)
    {
        collection.Owner = this;
        Collections.Add(collection);
    }

    public void AddRule(Rule rule)
    {
        rule.Owner = this;
        Rules.Add(rule);
    }
}