using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xellarium.Authentication;
using Xellarium.BusinessLogic;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.Shared.DTO;
using static Xellarium.Authentication.CookiesAuthPolicies;
using IAuthenticationService = Xellarium.BusinessLogic.Services.IAuthenticationService;

namespace Xellarium.WebApi.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
public class UserController(IUserService service, ICollectionService _collectionService, IAuthenticationService authenticationService, IMapper mapper,
    ILogger<UserController> logger, BusinessLogicConfiguration configuration) : ControllerBase
{
    #region AuthEndpoints
    
    // POST: api/User/register
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> RegisterUser([FromBody] UserLoginDTO userLoginDto)
    {
        logger.LogInformation("Requested registration of {@UserLogin}", userLoginDto);
        var (username, password) = (userLoginDto.Username, userLoginDto.Password);
        if (await service.UserExists(username))
        {
            logger.LogInformation("Register conflict, user already exists with name {Username}", username);
            return Conflict();
        }

        var user = await authenticationService.RegisterUser(username, password);
        var userDto = mapper.Map<BusinessLogic.Models.User, UserDTO>(user);
        logger.LogInformation("Successfully registered user {Username}", username);
        
        await Cookies.SignInUser(HttpContext, user);
        return CreatedAtAction(nameof(GetUser), new {id = userDto.Id}, userDto);
    }
    
    // POST: api/User/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> LoginUser([FromBody] UserLoginDTO userLoginDto)
    {
        logger.LogInformation("Requested login of {@UserLogin}", userLoginDto);
        var (username, password) = (userLoginDto.Username, userLoginDto.Password);
        var user = await authenticationService.AuthenticateUser(username, password);
        if (user == null)
        {
            logger.LogInformation("Failed authorization of {@UserLogin}", userLoginDto);
            return Unauthorized();
        }
        
        await Cookies.SignInUser(HttpContext, user);
        logger.LogInformation("User successfully authorized: {@UserLogin}", userLoginDto);
        return Ok(mapper.Map<BusinessLogic.Models.User, UserDTO>(user));
    }
    
    // POST: api/User/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> LogoutUser()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }
    
    #endregion
    
    // GET: api/User/5
    [HttpGet("{id}")]
    [Authorize(Policy = AdminOrOwner)]
    public async Task<ActionResult<UserDTO>> GetUser(int id)
    {
        logger.LogInformation("Requested user {Id}", id);
        var user = await service.GetUser(id);

        if (user == null)
        {
            logger.LogInformation("User not found");
            return NotFound();
        }

        var userDto = mapper.Map<BusinessLogic.Models.User, UserDTO>(user);
        logger.LogInformation("Found user: {@UserDto}", userDto);
        return userDto;
    }
        
    // GET: api/User/5/collections
    [HttpGet("{id}/collections")]
    [Authorize(Policy = AdminOrOwner)]
    public async Task<ActionResult<IEnumerable<CollectionDTO>>> GetUserCollections(int id)
    {
        logger.LogInformation("Requested collections of user {Id}", id);
        var collections = await service.GetUserCollections(id);
        return Ok(collections.Select(mapper.Map<Collection, CollectionDTO>));
    }
        
    // GET: api/User/5/rules
    [HttpGet("{id}/rules")]
    [Authorize(Policy = AdminOrOwner)]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetUserRules(int id)
    {
        logger.LogInformation("Requested rules of user {Id}", id);
        var rules = await service.GetUserRules(id);
        return Ok(rules.Select(mapper.Map<Rule, RuleDTO>));
    }
    
    // GET: api/User/neighborhood
    [HttpGet("neighborhood")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<NeighborhoodDTO>>> GetNeighborhoods()
    {
        logger.LogInformation("Requested all neighborhoods");
        var neighborhoods = await service.GetNeighborhoods();
        return Ok(neighborhoods.Select(mapper.Map<Neighborhood, NeighborhoodDTO>));
    }
    
    // GET: api/User/neighborhood/{id}
    [HttpGet("neighborhood/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<NeighborhoodDTO>> GetNeighborhood(int id)
    {
        logger.LogInformation("Requested neighborhood {Id}", id);
        var neighborhood = await service.GetNeighborhood(id);
        if (neighborhood == null)
        {
            logger.LogInformation("Neighborhood {Id} not found", id);
            return NotFound();
        }
        
        return Ok(mapper.Map<Neighborhood, NeighborhoodDTO>(neighborhood));
    }

        
    // POST: api/User/5/warn
    [HttpPost("{id}/warn")]
    [Authorize(Policy = Admin)]
    public async Task<IActionResult> WarnUser(int id)
    {
        logger.LogInformation("Requested warn of user {Id}", id);
        await service.WarnUser(id);
        logger.LogInformation("Warned user {Id}", id);
        return NoContent();
    }
        
    // GET: api/User/5/collections/1
    [HttpGet("{id}/collections/{collectionId}")]
    [Authorize(Policy = CanAccessCollection)]
    public async Task<ActionResult<CollectionDTO>> GetCollection(int id, int collectionId)
    {
        logger.LogInformation("Requested collection {CollectionId} of user {Id}", collectionId, id);
        var collection = await service.GetCollection(collectionId);
        if (collection == null)
        {
            logger.LogInformation("Collection {CollectionId} of user {Id} not found", collectionId, id);
            return NotFound();
        }

        logger.LogInformation("Found collection {CollectionId} of user {Id}", collectionId, id);
        return Ok(mapper.Map<Collection, CollectionDTO>(collection));
    }
    
    // GET: api/User/5/rules/1
    [HttpGet("{id}/rules/{ruleId}")]
    [Authorize(Policy = AdminOrOwner)]
    public async Task<ActionResult<RuleDTO>> GetRule(int id, int ruleId)
    {
        logger.LogInformation("Requested rule {RuleId} of user {Id}", ruleId, id);
        var rule = await service.GetRule(id, ruleId);
        if (rule == null)
        {
            logger.LogInformation("Rule {RuleId} of user {Id} not found", ruleId, id);
            return NotFound();
        }

        logger.LogInformation("Found rule {RuleId} of user {Id}", ruleId, id);
        return Ok(mapper.Map<Rule, RuleDTO>(rule));
    }
    
    // GET: api/User/5/collections/1/rules
    [HttpGet("{id}/collections/{collectionId}/rules")]
    [Authorize(Policy = CanAccessCollection)]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetCollectionRules(int id, int collectionId)
    {
        logger.LogInformation("Requested rules of collection {CollectionId} of user {Id}", collectionId, id);
        var collection = await service.GetCollection(collectionId);
        if (collection == null)
        {
            logger.LogInformation("Collection {CollectionId} of user {Id} not found", collectionId, id);
            return NotFound();
        }

        return Ok(collection.Rules.Select(mapper.Map<Rule, RuleDTO>));
    }

    #region SelfEndpoints
    
    // GET: api/User/collections/1/rules
    [HttpGet("collections/{collectionId}/rules")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetCollectionRules(int collectionId)
    {
        if (!TryGetAuthorizedId(out var id))
        {
            return Unauthorized();
        }
        return await GetCollectionRules(id, collectionId);
    }
    
    // GET: api/User/collections
    [HttpGet("collections")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CollectionDTO>>> GetCollections()
    {
        if (!TryGetAuthorizedId(out var id))
        {
            return Unauthorized();
        }
        return await GetUserCollections(id);
    }
    
    // GET: api/User/rules
    [HttpGet("rules")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetRules()
    {
        if (!TryGetAuthorizedId(out var id))
        {
            return Unauthorized();
        }
        return await GetUserRules(id);
    }

    // GET: api/User/collections/1
    [HttpGet("collections/{collectionId}")]
    [Authorize(Policy = CanAccessCollection)]
    public async Task<ActionResult<CollectionDTO>> GetCollection(int collectionId)
    {
        if (!TryGetAuthorizedId(out var id))
        {
            return Unauthorized();
        }
        return await GetCollection(id, collectionId);
    }
    
    // api/User/collections
    [HttpPost("collections")]
    [Authorize]
    public async Task<ActionResult<CollectionDTO>> AddCollection([FromBody] PostCollectionDTO collectionPostDto)
    {
        if (!TryGetAuthorizedId(out var id))
        {
            return Unauthorized();
        }
        return await AddCollection(id, collectionPostDto);
    }
    
    // POST: api/User/collections/1/rule
    [HttpPost("collections/{collectionId}/rule")]
    [Authorize]
    public async Task<ActionResult<RuleDTO>> AddRule(int collectionId, [FromBody] PostRuleDTO ruleDto)
    {
        if (!TryGetAuthorizedId(out var id))
        {
            return Unauthorized();
        }
        return await AddRule(id, collectionId, ruleDto);
    }
    
    // GET: api/User/profile
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserDTO>> CurrentUser()
    {
        if (!TryGetAuthorizedId(out var id))
        {
            return Unauthorized();
        }
        var user = await service.GetUser(id);
        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(mapper.Map<BusinessLogic.Models.User, UserDTO>(user));
    }
    
    // PUT: api/User/public_collections
    [HttpGet("public_collections")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CollectionDTO>>> GetPublicCollections()
    {
        if (!TryGetAuthorizedId(out var id))
        {
            return Unauthorized();
        }
        return await GetAccessibleCollections(id);
    }

    #endregion

    #region AdminOnlyEndpoints
    
    // GET: api/User
    [HttpGet]
    [Authorize(Policy = Admin)]
    public async Task<ActionResult<IQueryable<UserDTO>>> GetUsers()
    {
        logger.LogInformation("Requested all users");
        var users = await service.GetUsers();
        return Ok(users.Select(mapper.Map<BusinessLogic.Models.User, UserDTO>));
    }

    // POST: api/User
    [HttpPost]
    [Authorize(Policy = Admin)]
    public async Task<ActionResult<UserDTO>> PostUser([FromBody] PostUserDTO user)
    {
        logger.LogInformation("Requested post of user {@PostUserDto}", user);

        var realUser = new BusinessLogic.Models.User()
        {
            Name = user.Name,
            PasswordHash = authenticationService.HashPassword(user.Password),
            Role = user.Role,
        };
        await service.AddUser(realUser);
        logger.LogInformation("Created new user with id {Id}", realUser.Id);
        return CreatedAtAction(nameof(GetUser), new {id = realUser.Id}, mapper.Map<BusinessLogic.Models.User, UserDTO>(realUser));
    }
    
    // GET: api/User/5/public_collections
    [HttpGet("{id}/public_collections")]
    [Authorize(Policy = Admin)]
    public async Task<ActionResult<IEnumerable<CollectionDTO>>> GetAccessibleCollections(int id)
    {
        logger.LogInformation("Requested accessible collections of user {Id}", id);
        var collections = await _collectionService.GetPublicAndOwnedCollections(id);
        return Ok(collections.Select(mapper.Map<Collection, CollectionDTO>));
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    [Authorize(Policy = Admin)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        logger.LogInformation("Requested delete of user {Id}", id);
        if (!await service.UserExists(id))
        {
            logger.LogInformation("User with {Id} does not exist", id);
            return NotFound();
        }

        await service.DeleteUser(id);
        logger.LogInformation("Deleted user {Id}", id);
        return NoContent();
    }
    
    // PUT: api/User/5
    [HttpPut("{id}")]
    [Authorize(Policy = Admin)]
    public async Task<IActionResult> PutUser(int id, [FromBody] UserDTO userDto)
    {
        logger.LogInformation("Requested put of user {Id} with value {@UserDto}", id, userDto);
        if (id != userDto.Id)
        {
            logger.LogInformation("Id in path {Id} != id in value {@UserDto}", id, userDto);
            return BadRequest();
        }

        if (!await service.UserExists(id))
        {
            logger.LogInformation("User with {Id} does not exist", id);
            return NotFound();
        }

        var storedUser = await service.GetUser(id);

        var user = mapper.Map<UserDTO, BusinessLogic.Models.User>(userDto);
        user.CreatedAt = storedUser!.CreatedAt;
        user.DeletedAt = storedUser.DeletedAt;
        user.PasswordHash = storedUser.PasswordHash;
        
        await service.UpdateUser(user);
        logger.LogInformation("Updated user {Id}", id);
        return NoContent();
    }

    // POST: api/User/5/collections/1/rule
    [HttpPost("{id}/collections/{collectionId}/rule")]
    [Authorize(Policy = Admin)]
    public async Task<ActionResult<RuleDTO>> AddRule(int id, int collectionId, [FromBody] PostRuleDTO postRuleDto)
    {
        logger.LogInformation("Requested add of rule {@PostRuleDto} to collection {CollectionId} of user {Id}", postRuleDto, collectionId, id);
        var realRule = new Rule()
        {
            GenericRule = postRuleDto.GenericRule,
            Name = postRuleDto.Name,
            Neighborhood = (await service.GetNeighborhood(postRuleDto.NeighborhoodId))!
        };
        await service.AddNewRuleToCollection(id, collectionId, realRule);
        var ruleDto = mapper.Map<Rule, RuleDTO>(realRule!);
        logger.LogInformation("Added rule {@RuleDto} to collection {CollectionId} of user {Id}", ruleDto, collectionId, id);
        return CreatedAtAction(nameof(GetRule), new {id, ruleId = ruleDto.Id}, ruleDto);
    }
    
    // POST: api/User/5/collections
    [HttpPost("{id}/collections")]
    [Authorize(Policy = Admin)]
    public async Task<ActionResult<CollectionDTO>> AddCollection(int id, [FromBody] PostCollectionDTO collectionPostDto)
    {
        logger.LogInformation("Requested add of collection {@PostCollectionDto} to user {Id}", collectionPostDto, id);
        var user = await service.GetUser(id);
        if (user == null)
        {
            logger.LogInformation("User {Id} not found", id);
            return NotFound();
        }
        var realCollection = new Collection()
        {
            Name = collectionPostDto.Name,
            IsPrivate = collectionPostDto.IsPrivate ?? configuration.CollectionsPrivateByDefault
        };
        await service.AddCollection(id, realCollection);
        var collectionDto = mapper.Map<Collection, CollectionDTO>(realCollection!);
        logger.LogInformation("Added collection {@CollectionDto} to user {Id}", collectionDto, id);
        return CreatedAtAction(nameof(GetCollection), new {id, collectionId = collectionDto.Id}, collectionDto);
    }
    
    #endregion
    
    private bool TryGetAuthorizedId(out int id)
    {
        var identityUser = HttpContext.User;
        var idClaim = identityUser.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null)
        {
            id = 0;
            return false;
        }
        
        id = int.Parse(idClaim.Value);
        return true;
    }
}