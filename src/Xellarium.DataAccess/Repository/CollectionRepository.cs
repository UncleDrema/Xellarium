using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;

namespace Xellarium.DataAccess.Repository;

public class CollectionRepository : GenericRepository<Collection>, ICollectionRepository
{
    public CollectionRepository(XellariumContext context) : base(context)
    {
    }
}