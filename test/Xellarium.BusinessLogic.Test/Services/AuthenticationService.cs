using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Moq;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;

namespace Xellarium.BusinessLogic.Test.Services;

[AllureParentSuite("Business Logic")]
[AllureSuite("Services")]
[AllureSubSuite("AuthenticationService")]
public class AuthenticationServiceTests : IDisposable, IClassFixture<RepositoryMocks>
{
    private readonly RepositoryMocks _mocks;
    private readonly IAuthenticationService _authenticationService;
    
    [AllureBefore("Create authentication service and mocks")]
    public AuthenticationServiceTests(RepositoryMocks mocks)
    {
        _mocks = mocks;
        
        var userService = new UserService(_mocks.UserRepositoryMock.Object,
            _mocks.CollectionRepositoryMock.Object,
            _mocks.RuleRepositoryMock.Object,
            _mocks.NeighborhoodRepositoryMock.Object);
        
        _authenticationService = new AuthenticationService(userService);
    }
    
    [Fact(DisplayName = "RegisterUser when user exists should throw ArgumentException")]
    public async Task RegisterUserWhenUserExists_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "name";
        var user = new UserBuilder()
            .WithName(name)
            .Build();
        
        _mocks.UserRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(user);
        
        // Act
        var action = () => _authenticationService.RegisterUser(name, "password");
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
    
    [Fact(DisplayName = "RegisterUser when password is empty should throw ArgumentException")]
    public async Task RegisterUserWhenPasswordIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "name";
        
        // Act
        var action = () => _authenticationService.RegisterUser(name, "");
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
    
    [Fact(DisplayName = "RegisterUser when user not exists should add user")]
    public async Task RegisterUserWhenUserNotExists_ShouldAddUser()
    {
        // Arrange
        var name = "name";
        var password = "password";
        
        _mocks.UserRepositoryMock.Setup(x => x.Add(It.IsAny<User>(), true));
        
        // Act
        await _authenticationService.RegisterUser(name, password);
        
        // Assert
        _mocks.UserRepositoryMock.Verify(x => x.Add(It.IsAny<User>(), true), Times.Once);
    }
    
    [Fact(DisplayName = "AuthenticateUser when user not exists should return null")]
    public async Task AuthenticateUserWhenUserNotExists_ShouldReturnNull()
    {
        // Arrange
        var name = "name";
        
        // Act
        var result = await _authenticationService.AuthenticateUser(name, "password");
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact(DisplayName = "AuthenticateUser when password does not match should return null")]
    public async Task AuthenticateUserWhenPasswordNotMatch_ShouldReturnNull()
    {
        // Arrange
        var name = "name";
        var password = "password";
        var user = await _authenticationService.RegisterUser(name, password);
        
        _mocks.UserRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(user);
        
        // Act
        var result = await _authenticationService.AuthenticateUser(name, "notMyPassword");
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact(DisplayName = "AuthenticateUser when password match should return user")]
    public async Task AuthenticateUserWhenPasswordMatch_ShouldReturnUser()
    {
        // Arrange
        var name = "name";
        var password = "password";
        var user = await _authenticationService.RegisterUser(name, password);
        
        _mocks.UserRepositoryMock.Setup(x => x.GetByName(name)).ReturnsAsync(user);
        
        // Act
        var result = await _authenticationService.AuthenticateUser(name, password);
        
        // Assert
        Assert.Equal(user, result);
    }
    
    [Fact(DisplayName = "HashPassword returns different from password hash string")]
    public void HashPassword_ShouldReturnHash()
    {
        // Arrange
        var password = "password";
        
        // Act
        var result = _authenticationService.HashPassword(password);
        
        // Assert
        Assert.NotEqual(password, result);
    }
    
    [Fact(DisplayName = "VerifyPassword when password match should return true")]
    public void VerifyPasswordWhenPasswordMatch_ShouldReturnTrue()
    {
        // Arrange
        var password = "password";
        var hash = _authenticationService.HashPassword(password);
        
        // Act
        var result = _authenticationService.VerifyPassword(password, hash);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact(DisplayName = "VerifyPassword when password not match should return false")]
    public void VerifyPasswordWhenPasswordNotMatch_ShouldReturnFalse()
    {
        // Arrange
        var password = "password";
        var hash = _authenticationService.HashPassword(password);
        
        // Act
        var result = _authenticationService.VerifyPassword("password1", hash);
        
        // Assert
        Assert.False(result);
    }
    
    [AllureAfter("Verify and reset all mocks")]
    public void Dispose()
    {
        _mocks.VerifyAll();
        _mocks.Reset();
    }
}