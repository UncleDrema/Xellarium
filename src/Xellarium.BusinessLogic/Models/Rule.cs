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
    public Neighborhood Neighborhood { get; set; }
    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();

    public World NextState(World w, IList<Vec2> offsets, int times = 1)
    {
        if (times < 1)
            throw new ArgumentOutOfRangeException(nameof(times), times, "Times should be greater than 0");
        ArgumentNullException.ThrowIfNull(w);
        ArgumentNullException.ThrowIfNull(offsets);
        World result = w;
        for (int i = 0; i < times; i++)
        {
            result = GenericRule.NextState(result, offsets);
        }
        return result;
    }
}