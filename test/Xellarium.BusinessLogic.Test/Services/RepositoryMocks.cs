using Moq;
using Xellarium.BusinessLogic.Repository;

namespace Xellarium.BusinessLogic.Test.Services;

public class RepositoryMocks
{
    public Mock<IUserRepository> UserRepositoryMock { get; }
    public Mock<ICollectionRepository> CollectionRepositoryMock { get; }
    public Mock<IRuleRepository> RuleRepositoryMock { get; }
    public Mock<INeighborhoodRepository> NeighborhoodRepositoryMock { get; }
    
    public RepositoryMocks()
    {
        UserRepositoryMock = new Mock<IUserRepository>();
        CollectionRepositoryMock = new Mock<ICollectionRepository>();
        RuleRepositoryMock = new Mock<IRuleRepository>();
        NeighborhoodRepositoryMock = new Mock<INeighborhoodRepository>();
    }
    
    public void VerifyAll()
    {
        UserRepositoryMock.VerifyAll();
        CollectionRepositoryMock.VerifyAll();
        RuleRepositoryMock.VerifyAll();
        NeighborhoodRepositoryMock.VerifyAll();
    }

    public void Reset()
    {
        UserRepositoryMock.Reset();
        CollectionRepositoryMock.Reset();
        RuleRepositoryMock.Reset();
        NeighborhoodRepositoryMock.Reset();
    }
}