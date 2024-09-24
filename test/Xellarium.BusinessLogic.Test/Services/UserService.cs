using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Moq;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

[AllureParentSuite("Business Logic")]
[AllureSuite("Services")]
[AllureSubSuite("UserService")]
public class UserServiceTests : IDisposable, IClassFixture<RepositoryMocks>
{
    private readonly RepositoryMocks _mocks;
    private readonly UserService _userService;
    
    [AllureBefore("Create user service and mocks")]
    public UserServiceTests(RepositoryMocks mocks)
    {
        _mocks = mocks;
        
        _userService = new UserService(_mocks.UserRepositoryMock.Object,
            _mocks.CollectionRepositoryMock.Object,
            _mocks.RuleRepositoryMock.Object,
            _mocks.NeighborhoodRepositoryMock.Object);
    }
    
    [Fact(DisplayName = "GetUserById returns user when user exists")]
    public async Task GetUserById_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var userId = 1;
        var user = new UserBuilder()
            .WithId(userId)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUser(userId);
        
        // Assert
        Assert.Equal(user, result);
    }
    
    [Fact(DisplayName = "GetUserById returns null when user not exists")]
    public async Task GetUserById_WhenUserNotExists_ShouldReturnNull()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var result = await _userService.GetUser(userId);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact(DisplayName = "GetUserById returns null when user deleted")]
    public async Task GetUserById_WhenUserDeleted_ShouldReturnNull()
    {
        // Arrange
        var userId = 1;
        var user = new UserBuilder()
            .WithId(userId)
            .Build();
        user.Delete();

        _mocks.UserRepositoryMock.Setup(x => x.Add(user, true));
        
        // Act
        await _userService.AddUser(user);
        
        // Assert
        var result = await _userService.GetUser(userId);
        Assert.Null(result);
    }
    
    [Fact(DisplayName = "AddUser adds new user when user not exists")]
    public async Task AddUser_WhenUserNotExists_ShouldAddUser()
    {
        // Arrange
        var user = new UserBuilder()
            .WithId(1)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(user.Id, false)).ReturnsAsync(false);
        _mocks.UserRepositoryMock.Setup(x => x.Add(user, true));
        
        // Act
        await _userService.AddUser(user);
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Add(user, true), Times.Once);
    }
    
    [Fact(DisplayName = "AddUser throws exception when user exists")]
    public async Task AddUser_WhenUserExists_ShouldNotAddUser()
    {
        // Arrange
        var user = new UserBuilder()
            .WithId(1)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(user.Id, false)).ReturnsAsync(true);
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.AddUser(user));
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Add(user, true), Times.Never);
    }
    
    [Fact(DisplayName = "UpdateUser updates user when user exists")]
    public async Task UpdateUser_WhenUserExists_ShouldUpdateUser()
    {
        // Arrange
        var user = new UserBuilder()
            .WithId(1)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(user.Id, false)).ReturnsAsync(true);
        _mocks.UserRepositoryMock.Setup(x => x.Update(user));
        
        // Act
        await _userService.UpdateUser(user);
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Update(user), Times.Once);
    }
    
    [Fact(DisplayName = "UpdateUser throws exception when user not exists")]
    public async Task UpdateUser_WhenUserNotExists_ShouldNotUpdateUser()
    {
        // Arrange
        var user = new UserBuilder()
            .WithId(1)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(user.Id, false)).ReturnsAsync(false);
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUser(user));
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Update(user), Times.Never);
    }
    
    [Fact(DisplayName = "DeleteUser soft deletes user when user exists")]
    public async Task DeleteUser_WhenUserExists_ShouldSoftDeleteUser()
    {
        // Arrange
        var userId = 1;
        var user = new UserBuilder()
            .WithId(1)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, true)).ReturnsAsync(user);
        _mocks.UserRepositoryMock.Setup(x => x.SoftDelete(userId));
        
        // Act
        await _userService.DeleteUser(userId);
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.SoftDelete(userId), Times.Once);
        _mocks.UserRepositoryMock.Verify(x => x.HardDelete(userId), Times.Never);
    }
    
    [Fact(DisplayName = "DeleteUser does not tries to delete user when user not exists")]
    public async Task DeleteUser_WhenUserNotExists_ShouldNotDeleteUser()
    {
        // Arrange
        var userId = 1;
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.DeleteUser(userId));
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.SoftDelete(userId), Times.Never);
        _mocks.UserRepositoryMock.Verify(x => x.HardDelete(userId), Times.Never);
    }
    
    [Fact(DisplayName = "DeleteUser does not tries to delete user when user deleted")]
    public async Task DeleteUser_WhenUserDeleted_ShouldNotDeleteUser()
    {
        // Arrange
        var userId = 1;
        var user = new UserBuilder()
            .WithId(1)
            .Build();
        user.Delete();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, true)).ReturnsAsync(user);
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.DeleteUser(userId));
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.SoftDelete(userId), Times.Never);
        _mocks.UserRepositoryMock.Verify(x => x.HardDelete(userId), Times.Never);
    }
    
    [Fact(DisplayName = "GetUserCollections returns user collections when user exists")]
    public async Task GetUserCollections_WhenUserExists_ShouldReturnUserCollections()
    {
        // Arrange
        var userId = 1;
        var user =new UserBuilder()
            .WithId(userId)
            .WithCollections(ObjectMother.EmptyCollection())
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUserCollections(userId);
        
        // Assert
        Assert.Equal(user.Collections, result);
    }
    
    [Fact(DisplayName = "GetUserCollections throws exception when user not exists")]
    public async Task GetUserCollections_WhenUserNotExists_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var action = () => _userService.GetUserCollections(userId);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
    
    [Fact(DisplayName = "GetUserRules returns user rules when user exists")]
    public async Task GetUserRules_WhenUserExists_ShouldReturnUserRules()
    {
        // Arrange
        var userId = 1;
        var rule = ObjectMother.SimpleRule();
        var user = new UserBuilder()
            .WithId(userId)
            .WithCollections(new CollectionBuilder()
                .WithRules(rule)
                .Build())
            .WithRules(rule)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUserRules(userId);
        
        // Assert
        Assert.Equal(user.Collections.SelectMany(c => c.Rules), result);
    }
    
    [Fact(DisplayName = "GetUserRules returns only unique rules")]
    public async Task GetUserRule_ReturnsOnlyUniqueRules()
    {
        // Arrange
        var userId = 1;
        var rule = ObjectMother.SimpleRule();
        var user = new UserBuilder()
            .WithId(userId)
            .WithCollections(new CollectionBuilder()
                    .WithRules(rule)
                    .Build(),
                new CollectionBuilder()
                    .WithRules(rule)
                    .Build())
            .WithRules(rule)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUserRules(userId);
        
        // Assert
        Assert.Equal(user.Collections.SelectMany(c => c.Rules).Distinct(), result);
    }
    
    [Fact(DisplayName = "GetUserRules throws exception when user not exists")]
    public async Task GetUserRules_WhenUserNotExists_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var action = () => _userService.GetUserRules(userId);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
    
    [Fact(DisplayName = "WarnUser adds warning when user exists")]
    public async Task WarnUser_WhenUserExists_ShouldAddWarning()
    {
        // Arrange
        var userId = 1;
        var user = new UserBuilder()
            .WithId(userId)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        _mocks.UserRepositoryMock.Setup(x => x.Update(user));
        
        // Act
        await _userService.WarnUser(userId);
        
        // Assert
        var result = await _userService.GetUser(userId);
        Assert.NotNull(result);
        Assert.Equal(1, result.WarningsCount);
        _mocks.UserRepositoryMock.Verify(x => x.Update(user), Times.Once);
    }
    
    [Fact(DisplayName = "WarnUser throws exception when user not exists")]
    public async Task WarnUser_WhenUserNotExists_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var action = () => _userService.WarnUser(userId);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
    
    [Fact(DisplayName = "UserExists when user exists should return true")]
    public async Task UserExists_WhenUserExists_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1;
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(userId, false)).ReturnsAsync(true);
        
        // Act
        var result = await _userService.UserExists(userId);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact(DisplayName = "UserExists when user not exists should return false")]
    public async Task UserExists_WhenUserNotExists_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(userId, false)).ReturnsAsync(false);
        
        // Act
        var result = await _userService.UserExists(userId);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact(DisplayName = "UserExists when user deleted should return false")]
    public async Task UserExists_WhenUserDeleted_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(userId, false)).ReturnsAsync(false);
        
        // Act
        var result = await _userService.UserExists(userId);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact(DisplayName = "GetUsers returns users when users exists")]
    public async Task GetUsers_WhenUsersExists_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<User> { ObjectMother.SimpleUser(), ObjectMother.SimpleUser() };
        
        _mocks.UserRepositoryMock.Setup(x => x.GetAll(false)).ReturnsAsync(users);
        
        // Act
        var result = await _userService.GetUsers();
        
        // Assert
        Assert.Equal(users, result);
    }
    
    [Fact(DisplayName = "GetUsers returns empty list when users not exists")]
    public async Task GetUsers_WhenUsersNotExists_ShouldReturnEmptyList()
    {
        // Arrange
        _mocks.UserRepositoryMock.Setup(x => x.GetAll(false)).ReturnsAsync(new List<User>());
        
        // Act
        var result = await _userService.GetUsers();
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact(DisplayName = "UserExists when name exists should return true")]
    public async Task UserExists_WhenNameExists_ShouldReturnTrue()
    {
        // Arrange
        var name = "name";
        var user = new UserBuilder()
            .WithName(name)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.UserExists(name);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact(DisplayName = "UserExists when name not exists should return false")]
    public async Task UserExists_WhenNameNotExists_ShouldReturnFalse()
    {
        // Arrange
        var name = "name";
        
        // Act
        var result = await _userService.UserExists(name);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact(DisplayName = "GetCollection returns collection when collection exists")]
    public async Task GetCollection_WhenCollectionExists_ShouldReturnCollection()
    {
        // Arrange
        var userId = 1;
        var collectionId = 1;
        var collection = new CollectionBuilder()
            .WithId(collectionId)
            .Build();
        var user = new UserBuilder()
            .WithId(userId)
            .WithCollections(collection)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        _mocks.CollectionRepositoryMock.Setup(x => x.Get(collectionId, false)).ReturnsAsync(collection);
        
        // Act
        var result = await _userService.GetCollection(userId, collectionId);
        
        // Assert
        Assert.Equal(collection.Id, result.Id);
    }
    
    [Fact(DisplayName = "GetCollection returns null when collection not exists")]
    public async Task GetCollection_WhenCollectionNotExists_ShouldReturnNull()
    {
        // Arrange
        var userId = 1;
        var collectionId = 1;
        var user = new UserBuilder()
            .WithId(userId)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetCollection(userId, collectionId);
        
        // Assert
        Assert.Null(result);
    }


    [AllureAfter("Verify and reset all mocks")]
    public void Dispose()
    {
        _mocks.VerifyAll();
        _mocks.Reset();
    }
}