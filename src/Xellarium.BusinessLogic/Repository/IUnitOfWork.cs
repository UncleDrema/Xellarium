namespace Xellarium.BusinessLogic.Repository;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    
    IRuleRepository Rules { get; }
    
    ICollectionRepository Collections { get; }
    
    INeighborhoodRepository Neighborhoods { get; }
    
    Task CompleteAsync();
}