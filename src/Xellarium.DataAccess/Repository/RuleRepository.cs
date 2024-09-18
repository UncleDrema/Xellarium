using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;

namespace Xellarium.DataAccess.Repository;

public class RuleRepository : GenericRepository<Rule>, IRuleRepository
{
    public RuleRepository(XellariumContext context) : base(context)
    {
    }
}