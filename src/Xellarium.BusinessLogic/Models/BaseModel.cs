using Xellarium.Tracing;

namespace Xellarium.BusinessLogic.Models;

/**
 * Базовая модель для всех моделей
 * Предоставляет Id и мягкое удаление
 */
public abstract class BaseModel
{
    public int Id { get; init; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public void MarkCreated()
    {
        using var activity = XellariumTracing.StartActivity();
        if (CreatedAt != default(DateTime))
            throw new InvalidOperationException("Entity is already marked created");
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkUpdated()
    {
        using var activity = XellariumTracing.StartActivity();
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Delete()
    {
        using var activity = XellariumTracing.StartActivity();
        if (IsDeleted)
            throw new InvalidOperationException("Cannot delete a deleted entity.");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}