using Moq;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

public class UserServiceTests : IDisposable, IClassFixture<RepositoryMocks>
{
    private readonly RepositoryMocks _mocks;
    private readonly UserService _userService;
    
    public UserServiceTests(RepositoryMocks mocks)
    {
        _mocks = mocks;
        
        _userService = new UserService(_mocks.UserRepositoryMock.Object,
            _mocks.CollectionRepositoryMock.Object,
            _mocks.RuleRepositoryMock.Object,
            _mocks.NeighborhoodRepositoryMock.Object);
    }
    
    [Fact]
    public async Task GetUserById_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId };
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUser(userId);
        
        // Assert
        Assert.Equal(user, result);
    }
    
    [Fact]
    public async Task GetUserById_WhenUserNotExists_ShouldReturnNull()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var result = await _userService.GetUser(userId);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetUserById_WhenUserDeleted_ShouldReturnNull()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, IsDeleted = true };

        _mocks.UserRepositoryMock.Setup(x => x.Add(user, true));
        
        // Act
        await _userService.AddUser(user);
        
        // Assert
        var result = await _userService.GetUser(userId);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task AddUser_WhenUserNotExists_ShouldAddUser()
    {
        // Arrange
        var user = new User { Id = 1 };
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(user.Id, false)).ReturnsAsync(false);
        _mocks.UserRepositoryMock.Setup(x => x.Add(user, true));
        
        // Act
        await _userService.AddUser(user);
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Add(user, true), Times.Once);
    }
    
    [Fact]
    public async Task AddUser_WhenUserExists_ShouldNotAddUser()
    {
        // Arrange
        var user = new User { Id = 1 };
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(user.Id, false)).ReturnsAsync(true);
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.AddUser(user));
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Add(user, true), Times.Never);
    }
    
    [Fact]
    public async Task UpdateUser_WhenUserExists_ShouldUpdateUser()
    {
        // Arrange
        var user = new User { Id = 1 };
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(user.Id, false)).ReturnsAsync(true);
        _mocks.UserRepositoryMock.Setup(x => x.Update(user));
        
        // Act
        await _userService.UpdateUser(user);
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Update(user), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUser_WhenUserNotExists_ShouldNotUpdateUser()
    {
        // Arrange
        var user = new User { Id = 1 };
        
        _mocks.UserRepositoryMock.Setup(x => x.Exists(user.Id, false)).ReturnsAsync(false);
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUser(user));
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Update(user), Times.Never);
    }
    
    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldSoftDeleteUser()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, IsDeleted = false };
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, true)).ReturnsAsync(user);
        _mocks.UserRepositoryMock.Setup(x => x.SoftDelete(userId));
        
        // Act
        await _userService.DeleteUser(userId);
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.SoftDelete(userId), Times.Once);
        _mocks.UserRepositoryMock.Verify(x => x.HardDelete(userId), Times.Never);
    }
    
    [Fact]
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
    
    [Fact]
    public async Task DeleteUser_WhenUserDeleted_ShouldNotDeleteUser()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, IsDeleted = true };
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, true)).ReturnsAsync(user);
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.DeleteUser(userId));
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.SoftDelete(userId), Times.Never);
        _mocks.UserRepositoryMock.Verify(x => x.HardDelete(userId), Times.Never);
    }
    
    [Fact]
    public async Task DeleteUser_WhenUserDoesNotExists_ShouldNotDeleteUser()
    {
        // Arrange
        var userId = 1;
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.DeleteUser(userId));
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.SoftDelete(userId), Times.Never);
        _mocks.UserRepositoryMock.Verify(x => x.HardDelete(userId), Times.Never);
    }
    
    [Fact]
    public async Task GetUserCollections_WhenUserExists_ShouldReturnUserCollections()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Collections = new List<Collection> { new Collection() } };
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUserCollections(userId);
        
        // Assert
        Assert.Equal(user.Collections, result);
    }
    
    [Fact]
    public async Task GetUserCollections_WhenUserNotExists_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var action = () => _userService.GetUserCollections(userId);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
    
    [Fact]
    public async Task GetUserRules_WhenUserExists_ShouldReturnUserRules()
    {
        // Arrange
        var userId = 1;
        var rule = new Rule();
        var user = new User
        {
            Id = userId,
            Collections = new List<Collection>
            {
                new Collection
                {
                    Rules = new List<Rule> { rule }
                }
            },
            Rules = new List<Rule>() {rule}
        };
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUserRules(userId);
        
        // Assert
        Assert.Equal(user.Collections.SelectMany(c => c.Rules), result);
    }
    
    [Fact]
    public async Task GetUserRule_ReturnsOnlyUniqueRules()
    {
        // Arrange
        var userId = 1;
        var rule = new Rule();
        var user = new User
        {
            Id = userId,
            Collections = new List<Collection>
            {
                new Collection
                {
                    Rules = new List<Rule> { rule }
                },
                new Collection
                {
                    Rules = new List<Rule> { rule }
                }
            },
            Rules = new List<Rule>() {rule}
        };
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetUserRules(userId);
        
        // Assert
        Assert.Equal(user.Collections.SelectMany(c => c.Rules).Distinct(), result);
    }
    
    [Fact]
    public async Task GetUserRules_WhenUserNotExists_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUserRules(userId));
    }
    
    [Fact]
    public async Task WarnUser_WhenUserExists_ShouldAddWarning()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId };
        
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
    
    [Fact]
    public async Task WarnUser_WhenUserNotExists_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        
        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.WarnUser(userId));
    }
    
    [Fact]
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
    
    [Fact]
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
    
    [Fact]
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
    
    // test GetUsers(), NameExists(), GetCollection(), AddRule(), RegisterUser(), AuthenticateUser(), HashPassword(), VerifyPassword()
    [Fact]
    public async Task GetUsers_WhenUsersExists_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<User> { new User(), new User() };
        
        _mocks.UserRepositoryMock.Setup(x => x.GetAll(false)).ReturnsAsync(users);
        
        // Act
        var result = await _userService.GetUsers();
        
        // Assert
        Assert.Equal(users, result);
    }
    
    [Fact]
    public async Task GetUsers_WhenUsersNotExists_ShouldReturnEmptyList()
    {
        // Arrange
        _mocks.UserRepositoryMock.Setup(x => x.GetAll(false)).ReturnsAsync(new List<User>());
        
        // Act
        var result = await _userService.GetUsers();
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task NameExists_WhenNameExists_ShouldReturnTrue()
    {
        // Arrange
        var name = "name";
        var user = new User { Name = name };
        
        _mocks.UserRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.NameExists(name);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task NameExists_WhenNameNotExists_ShouldReturnFalse()
    {
        // Arrange
        var name = "name";
        
        // Act
        var result = await _userService.NameExists(name);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task GetCollection_WhenCollectionExists_ShouldReturnCollection()
    {
        // Arrange
        var userId = 1;
        var collectionId = 1;
        var collection = new Collection { Id = collectionId };
        var user = new User { Id = userId, Collections = new List<Collection> { collection } };
        collection.Owner = user;
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        _mocks.CollectionRepositoryMock.Setup(x => x.Get(collectionId, false)).ReturnsAsync(collection);
        
        // Act
        var result = await _userService.GetCollection(userId, collectionId);
        
        // Assert
        Assert.Equal(collection.Id, result.Id);
    }
    
    [Fact]
    public async Task GetCollection_WhenCollectionNotExists_ShouldReturnNull()
    {
        // Arrange
        var userId = 1;
        var collectionId = 1;
        var user = new User { Id = userId, Collections = new List<Collection>() };
        
        _mocks.UserRepositoryMock.Setup(x => x.Get(userId, false)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.GetCollection(userId, collectionId);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task RegisterUserWhenUserExists_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "name";
        var user = new User { Name = name };
        
        _mocks.UserRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(user);
        
        // Act
        var action = () => _userService.RegisterUser(name, "password");
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
    
    [Fact]
    public async Task RegisterUserWhenPasswordIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "name";
        
        // Act
        var action = () => _userService.RegisterUser(name, "");
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
    
    [Fact]
    public async Task RegisterUserWhenUserNotExists_ShouldAddUser()
    {
        // Arrange
        var name = "name";
        var password = "password";
        
        _mocks.UserRepositoryMock.Setup(x => x.Add(It.IsAny<User>(), true));
        
        // Act
        await _userService.RegisterUser(name, password);
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Add(It.IsAny<User>(), true), Times.Once);
    }
    
    [Fact]
    public async Task AuthenticateUserWhenUserNotExists_ShouldReturnNull()
    {
        // Arrange
        var name = "name";
        
        // Act
        var result = await _userService.AuthenticateUser(name, "password");
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task AuthenticateUserWhenPasswordNotMatch_ShouldReturnNull()
    {
        // Arrange
        var name = "name";
        var password = "password";
        var user = await _userService.RegisterUser(name, password);
        
        _mocks.UserRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.AuthenticateUser(name, "notMyPassword");
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task AuthenticateUserWhenPasswordMatch_ShouldReturnUser()
    {
        // Arrange
        var name = "name";
        var password = "password";
        var user = await _userService.RegisterUser(name, password);
        
        _mocks.UserRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(user);
        
        // Act
        var result = await _userService.AuthenticateUser(name, password);
        
        // Assert
        Assert.Equal(user, result);
    }
    
    [Fact]
    public void HashPassword_ShouldReturnHash()
    {
        // Arrange
        var password = "password";
        
        // Act
        var result = _userService.HashPassword(password);
        
        // Assert
        Assert.NotEqual(password, result);
    }
    
    [Fact]
    public void VerifyPasswordWhenPasswordMatch_ShouldReturnTrue()
    {
        // Arrange
        var password = "password";
        var hash = _userService.HashPassword(password);
        
        // Act
        var result = _userService.VerifyPassword(password, hash);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void VerifyPasswordWhenPasswordNotMatch_ShouldReturnFalse()
    {
        // Arrange
        var password = "password";
        var hash = _userService.HashPassword(password);
        
        // Act
        var result = _userService.VerifyPassword("password1", hash);
        
        // Assert
        Assert.False(result);
    }


    public void Dispose()
    {
        _mocks.VerifyAll();
        _mocks.Reset();
    }
}