using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.DataAccess.Models;
using Xellarium.DataAccess.Repository;

namespace Xellarium.IntegrationTests;

public class UserTests
{
    
    [Fact]
    public async Task TestRegistrationWorks()
    {
        await using var context = new TestDatabaseBuilder()
            .Build();
        
        var service = GetService(context);
        
        var user = await service.RegisterUser("user", "password");
        
        Assert.NotNull(user);
        
        var allUsers = (await service.GetUsers()).ToList();
        
        Assert.Single(allUsers);
        Assert.Equal("user", allUsers.First().Name);
        
        var userById = await service.GetUser(user.Id);
        
        Assert.NotNull(userById);
        Assert.Equal("user", userById.Name);
    }
    
    [Fact]
    public async Task TestRegistrationFailsWithDuplicateName()
    {
        await using var context = new TestDatabaseBuilder()
            .WithUsers(new List<User>
            {
                new User
                {
                    Name = "user",
                    PasswordHash = "password"
                }
            })
            .Build();
        
        var service = GetService(context);
        
        await Assert.ThrowsAsync<ArgumentException>(() => service.RegisterUser("user", "password"));
    }
    
    [Fact]
    public async Task TestAuthenticateWorks()
    {
        await using var context = new TestDatabaseBuilder()
            .Build();
        
        var service = GetService(context);
        
        await service.RegisterUser("user", "password");
        
        var user = await service.AuthenticateUser("user", "password");
        
        Assert.NotNull(user);
        Assert.Equal("user", user.Name);
    }
    
    [Fact]
    public async Task TestAuthenticateFailsWithWrongPassword()
    {
        await using var context = new TestDatabaseBuilder()
            .Build();
        
        var service = GetService(context);
        
        await service.RegisterUser("user", "password");
        
        var user = await service.AuthenticateUser("user", "wrongpassword");
        
        Assert.Null(user);
    }

    private UserService GetService(XellariumContext context)
    {
        var userRepository = new UserRepository(context);
        var collectionRepository = new CollectionRepository(context);
        var ruleRepository = new RuleRepository(context);
        var neighborhoodRepository = new NeighborhoodRepository(context);
        var service = new UserService(userRepository, collectionRepository, ruleRepository, neighborhoodRepository);
        return service;
    }
}