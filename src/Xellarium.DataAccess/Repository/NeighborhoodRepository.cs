using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;

namespace Xellarium.DataAccess.Repository;

public class NeighborhoodRepository : GenericRepository<Neighborhood>, INeighborhoodRepository
{
    public NeighborhoodRepository(XellariumContext context) : base(context)
    {
    }
}