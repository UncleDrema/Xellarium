using Xellarium.Shared;
using Xellarium.Tracing;

namespace Xellarium.BusinessLogic.Models;
/**
 * Пользователь
 * Содержит информацию о пользователе: имя, данные авторизации, число предупреждений, заблокирован ли пользователь
 */
public class User : BaseModel
{
    public string Name { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string? TwoFactorSecret { get; set; } = "";
    public UserRole Role { get; set; } = UserRole.User;
    public int WarningsCount { get; set; } = 0;
    public bool IsBlocked { get; set; } = false;
    public virtual ICollection<Rule> Rules { get; set; } = new List<Rule>();
    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
    
    public void AddWarning()
    {
        using var activity = XellariumTracing.StartActivity();
        WarningsCount++;
        if (WarningsCount >= 3)
        {
            IsBlocked = true;
        }
    }
    
    public void RemoveWarning()
    {
        using var activity = XellariumTracing.StartActivity();
        if (WarningsCount == 0)
        {
            throw new InvalidOperationException("No warnings to remove");
        }
        WarningsCount--;
        if (WarningsCount < 3)
        {
            IsBlocked = false;
        }
    }

    public void AddCollection(Collection collection)
    {
        using var activity = XellariumTracing.StartActivity();
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));
        collection.Owner = this;
        Collections.Add(collection);
    }

    public void AddRule(Rule rule)
    {
        using var activity = XellariumTracing.StartActivity();
        ArgumentNullException.ThrowIfNull(rule, nameof(rule));
        rule.Owner = this;
        Rules.Add(rule);
    }
}