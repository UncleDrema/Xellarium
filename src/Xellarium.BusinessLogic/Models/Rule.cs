using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Models;

/**
 * Правило
 * Содержит информацию о правиле:
 * - Generic Rule этого правила
 * - Тип соседства
 * - Название
 * - Начальное состояние (матрица чисел - состояний клеток)
 * - Владелец (User)
 * - Коллекция, к которой относится правило (Collection, может не быть)
 */
public class Rule : BaseModel
{
    public GenericRule GenericRule { get; set; }
    public string Name { get; set; }
    public virtual User Owner { get; set; }
    public int NeighborhoodId { get; set; }
    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();

    public World NextState(World w, IList<Vec2> offsets)
    {
        return GenericRule.NextState(w, offsets);
    }
}