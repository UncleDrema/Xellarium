using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;

namespace Xellarium.DataAccess.Repository;

public class NeighborhoodRepository(XellariumContext context, ILogger logger)
    : GenericRepository<Neighborhood>(context, logger), INeighborhoodRepository;