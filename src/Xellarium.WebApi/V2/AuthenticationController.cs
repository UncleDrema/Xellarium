using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Xellarium.Authentication;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.Shared.DTO;

namespace Xellarium.WebApi.V2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AuthenticationController(IAuthenticationService _service, IMapper mapper,
    IUserService _userService,
    JwtAuthorizationConfiguration jwtConfig,
    ILogger<AuthenticationController> logger) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthenticatedTokenDTO>> Register(UserLoginDTO userLoginDto)
    {
        var (name, password) = (userLoginDto.Username, userLoginDto.Password);
        if (await _userService.UserExists(name))
        {
            logger.LogInformation("Register conflict, user already exists with name {Username}", name);
            return Conflict();
        }
        
        var user = await _service.RegisterUser(name, password);
        
        var expiration = TimeSpan.FromSeconds(jwtConfig.ExpirationSeconds);
        var token = CreateAccessToken(user, expiration);
        
        return Ok(new AuthenticatedTokenDTO()
        {
            Token = token,
            ExpirationSeconds = (int) expiration.TotalSeconds,
            Type = Jwt.AuthType
        });
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthenticatedTokenDTO>> Login(UserLoginDTO userLoginDto)
    {
        var (name, password) = (userLoginDto.Username, userLoginDto.Password);
        var user = await _service.AuthenticateUser(name, password);
        if (user == null)
        {
            return Unauthorized();
        }
        
        var expiration = TimeSpan.FromSeconds(jwtConfig.ExpirationSeconds);
        var token = CreateAccessToken(user, expiration);
        
        return Ok(new AuthenticatedTokenDTO()
        {
            Token = token,
            ExpirationSeconds = (int) expiration.TotalSeconds,
            Type = Jwt.AuthType
        });
    }

    private string CreateAccessToken(User user, TimeSpan expiration)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var keyBytes = Encoding.UTF8.GetBytes(jwtConfig.SigningKey);
        var key = new SymmetricSecurityKey(keyBytes);
        
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: DateTime.Now.Add(expiration),
            signingCredentials: signingCredentials
        );
        
        var rawToken = new JwtSecurityTokenHandler().WriteToken(token);
        return rawToken;
    }
}