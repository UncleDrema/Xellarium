namespace Xellarium.BusinessLogic.Models;

/**
 * Базовая модель для всех моделей
 * Предоставляет Id и мягкое удаление
 */
public abstract class BaseModel
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public void MarkCreated()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}