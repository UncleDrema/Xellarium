using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xellarium.Authentication;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.Shared.DTO;

namespace Xellarium.WebApi.V2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]
[Produces("application/json")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class UsersController(IUserService _service, IMapper mapper,
    IRuleService _ruleService, ICollectionService _collectionService,
    ILogger<UsersController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        var users = await _service.GetUsers();
        return Ok(users.Select(mapper.Map<UserDTO>));
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<UserDTO>> GetUser(int id)
    {
        var user = await _service.GetUser(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<UserDTO>(user));
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = JwtAuthPolicies.Admin)]
    public async Task<ActionResult<UserDTO>> AddUser(PostUserDTO user)
    {
        if (await _service.UserExists(user.Name))
        {
            return BadRequest("User already exists");
        }
        var newUser = mapper.Map<User>(user);
        
        await _service.AddUser(newUser);
        return CreatedAtAction(nameof(GetUser), new {id = newUser.Id},
            mapper.Map<UserDTO>(newUser));
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = JwtAuthPolicies.Admin)]
    public async Task<ActionResult<UserDTO>> UpdateUser(int id, UserDTO user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }
        
        var userEntity = await _service.GetUser(id);
        if (userEntity == null)
        {
            return NotFound();
        }
        
        userEntity.Name = user.Name;
        userEntity.Role = user.Role;
        userEntity.WarningsCount = user.WarningsCount;
        userEntity.IsBlocked = user.IsBlocked;
        userEntity.Rules = user.Rules.Select(r => _ruleService.GetRule(r.Id).Result).ToList()!;
        userEntity.Collections = user.Collections.Select(c => _collectionService.GetCollection(c.Id).Result).ToList()!;
        
        await _service.UpdateUser(userEntity);
        
        return Ok(mapper.Map<UserDTO>(userEntity));
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.Admin)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (!await _service.UserExists(id))
        {
            return NotFound();
        }
        
        await _service.DeleteUser(id);
        return NoContent();
    }
    
    [HttpGet("{id}/collections")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<IEnumerable<CollectionDTO>>> GetUserCollections(int id)
    {
        var collections = await _service.GetUserCollections(id);
        return Ok(collections.Select(mapper.Map<CollectionDTO>));
    }
    
    [HttpGet("{id}/rules")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetUserRules(int id)
    {
        var rules = await _service.GetUserRules(id);
        return Ok(rules.Select(mapper.Map<RuleDTO>));
    }
    
    [HttpPost("{id}/warn")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.Admin)]
    public async Task<IActionResult> WarnUser(int id)
    {
        if (!await _service.UserExists(id))
        {
            return NotFound();
        }
        
        await _service.WarnUser(id);
        return NoContent();
    }
    
    [HttpGet("{id}/collections/{collectionId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<CollectionDTO>> GetCollection(int id, int collectionId)
    {
        var collection = await _service.GetCollection(id, collectionId);
        if (collection == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<CollectionDTO>(collection));
    }
    
    [HttpGet("{id}/rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<RuleDTO>> GetRule(int id, int ruleId)
    {
        var rule = await _service.GetRule(id, ruleId);
        if (rule == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<RuleDTO>(rule));
    }

    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<UserDTO>> GetCurrentUser()
    {
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            return Unauthorized();
        }

        return await GetUser(authUser.Id);
    }
    
    [HttpGet("current/collections")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<IEnumerable<CollectionDTO>>>
        GetCurrentUserCollections()
    {
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            return Unauthorized();
        }

        return await GetUserCollections(authUser!.Id);
    }
    
    [HttpGet("current/rules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetCurrentUserRules()
    {
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            return Unauthorized();
        }

        return await GetUserRules(authUser!.Id);
    }
    
    [HttpGet("current/collections/{collectionId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<CollectionDTO>> GetCurrentCollection(int collectionId)
    {
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            return Unauthorized();
        }

        return await GetCollection(authUser.Id, collectionId);
    }
    
    [HttpGet("current/rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<RuleDTO>> GetCurrentRule(int ruleId)
    {
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            return Unauthorized();
        }

        return await GetRule(authUser.Id, ruleId);
    }
}