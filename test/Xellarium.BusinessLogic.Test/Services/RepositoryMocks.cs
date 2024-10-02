using Moq;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Repository;

namespace Xellarium.BusinessLogic.Test.Services;

public class RepositoryMocks
{
    public Mock<IUserRepository> UserRepositoryMock { get; }
    public Mock<ICollectionRepository> CollectionRepositoryMock { get; }
    public Mock<IRuleRepository> RuleRepositoryMock { get; }
    public Mock<INeighborhoodRepository> NeighborhoodRepositoryMock { get; }
    private IUnitOfWork _unitOfWorkMock;
    
    private class UnitOfWorkMocked : IUnitOfWork
    {
        public UnitOfWorkMocked(IUserRepository users, IRuleRepository rules, ICollectionRepository collections, INeighborhoodRepository neighborhoods)
        {
            Users = users;
            Rules = rules;
            Collections = collections;
            Neighborhoods = neighborhoods;
        }

        public IUserRepository Users { get; }
        public IRuleRepository Rules { get; }
        public ICollectionRepository Collections { get; }
        public INeighborhoodRepository Neighborhoods { get; }
        public Task CompleteAsync()
        {
            return Task.CompletedTask;
        }
    }

    public RepositoryMocks()
    {
        UserRepositoryMock = new Mock<IUserRepository>();
        CollectionRepositoryMock = new Mock<ICollectionRepository>();
        RuleRepositoryMock = new Mock<IRuleRepository>();
        NeighborhoodRepositoryMock = new Mock<INeighborhoodRepository>();
    }

    public IUnitOfWork GetUnitOfWork()
    {
        return new UnitOfWorkMocked(UserRepositoryMock.Object, RuleRepositoryMock.Object,
            CollectionRepositoryMock.Object, NeighborhoodRepositoryMock.Object);
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