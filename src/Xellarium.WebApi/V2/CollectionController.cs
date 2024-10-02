using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xellarium.Authentication;
using Xellarium.BusinessLogic;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.Shared;
using Xellarium.Shared.DTO;

namespace Xellarium.WebApi.V2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]
[Produces("application/json")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class CollectionsController(ICollectionService _collectionService, IRuleService _ruleService,
    INeighborhoodService _neighborhoodService,
    IUserService _userService, BusinessLogicConfiguration businessLogicConfig,
    IMapper mapper, ILogger<CollectionsController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CollectionDTO>>> GetCollections([FromQuery] bool includePrivate = false)
    {
        IEnumerable<Collection> collections;
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            if (includePrivate)
            {
                return Unauthorized();
            }
            collections = await _collectionService.GetPublicCollections();
        }
        else
        {
            var user = await _userService.GetUser(authUser.Id);
            if (user == null)
            {
                throw new InvalidOperationException("Current user not found");
            }
            
            if (includePrivate && user.Role != UserRole.Admin)
            {
                return Unauthorized();
            }
            collections = includePrivate
                ? await _collectionService.GetCollections()
                : await _collectionService.GetPublicAndOwnedCollections(authUser!.Id);
        }
        
        return Ok(collections.Select(mapper.Map<CollectionDTO>));
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<CollectionDTO>> GetCollection(int id)
    {
        logger.LogInformation("Requested collection {Id}", id);
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }
        
        if (collection.IsPrivate)
        {
            if (!HttpContext.TryGetAuthenticatedUser(out var authUser) || 
                !authUser.CanAccessResourceOfUser(collection.Owner.Id))
            {
                return NotFound();
            }
        }

        return Ok(mapper.Map<CollectionDTO>(collection));
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    [EndpointSummary("Adds new collection for current user")]
    public async Task<ActionResult<CollectionDTO>> AddCollection(PostCollectionDTO collection)
    {
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            return Unauthorized();
        }
        
        var user = await _userService.GetUser(authUser.Id);
        if (user == null)
        {
            throw new InvalidOperationException("Current user not found");
        }
        
        var newCollection = new Collection
        {
            Name = collection.Name,
            IsPrivate = collection.IsPrivate ?? businessLogicConfig.CollectionsPrivateByDefault,
            Owner = user
        };
        await _collectionService.AddCollection(newCollection);
        return CreatedAtAction(nameof(GetCollection), new {id = newCollection.Id},
            mapper.Map<CollectionDTO>(newCollection));
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<CollectionDTO>> UpdateCollection(int id, CollectionDTO collection)
    {
        if (id != collection.Id)
        {
            return BadRequest("Id in path and body do not match");
        }
        
        var collectionEntity = await _collectionService.GetCollection(id);
        if (collectionEntity == null)
        {
            return NotFound();
        }
        
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser) ||
            !authUser!.CanAccessResourceOfUser(collectionEntity.Owner.Id))
        {
            return Unauthorized("Only owner or admin can change collection");
        }

        if (collection.OwnerId != collectionEntity.Owner.Id &&
            authUser.Role != UserRole.Admin)
        {
            return Unauthorized("Only admin can change collection ownership");
        }

        if (collectionEntity.Owner.Id != collection.OwnerId)
        {
            collectionEntity.Owner = (await _userService.GetUser(collection.OwnerId))!;
        }

        collectionEntity.Name = collection.Name;
        collectionEntity.IsPrivate = collection.IsPrivate;
        collectionEntity.Rules = collection.RuleReferences.Select(r => _ruleService.GetRule(r.Id).Result).ToList()!;
        await _collectionService.UpdateCollection(collectionEntity);
        return Ok(mapper.Map<CollectionDTO>(collectionEntity));
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection is null)
        {
            return NotFound();
        }
        
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser) ||
            !authUser!.CanAccessResourceOfUser(collection.Owner.Id))
        {
            return Unauthorized("Only owner or admin can delete collection");
        }
        
        await _collectionService.DeleteCollection(id);
        return NoContent();
    }
    
    [HttpGet("{id}/rules")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetRules(int id)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }
        
        if (collection.IsPrivate)
        {
            if (!HttpContext.TryGetAuthenticatedUser(out var authUser) || 
                !authUser.CanAccessResourceOfUser(collection.Owner.Id))
            {
                return NotFound();
            }
        }

        var rules = collection.Rules;
        return Ok(rules.Select(mapper.Map<RuleDTO>));
    }

    [HttpPost("{id}/rules")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    [EndpointSummary("Adds new rule into collection of current user")]
    public async Task<ActionResult<RuleDTO>> AddRuleToCollection(int id, PostRuleDTO rule)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }
        
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser) || 
            !authUser.CanAccessResourceOfUser(collection.Owner.Id))
        {
            return NotFound();
        }
        
        var user = await _userService.GetUser(authUser.Id);
        if (user == null)
        {
            return Unauthorized("User not found");
        }
        
        var neighborhood = await _neighborhoodService.GetNeighborhood(rule.NeighborhoodId);
        if (neighborhood == null)
        {
            return BadRequest("Neighborhood not found");
        }
        
        var ruleEntity = new Rule()
        {
            Neighborhood = neighborhood,
            Name = rule.Name,
            GenericRule = rule.GenericRule,
        };
        await _userService.AddNewRuleToCollection(user.Id, collection.Id, ruleEntity);
        return Created($"/api/v2/rules/{ruleEntity.Id}", mapper.Map<RuleDTO>(ruleEntity));
    }
    
    [HttpGet("{id}/contained_rules")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RuleReferenceDTO>>> GetCollectionRules(int id)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }
        
        if (collection.IsPrivate)
        {
            if (!HttpContext.TryGetAuthenticatedUser(out var authUser) || 
                !authUser.CanAccessResourceOfUser(collection.Owner.Id))
            {
                return NotFound();
            }
        }

        var rules = collection.Rules;
        return Ok(rules.Select(mapper.Map<RuleReferenceDTO>));
    }

    [HttpPost("{id}/contained_rules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<IActionResult> AddRule(int id, RuleReferenceDTO ruleReference)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }
        
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser) ||
            !authUser!.CanAccessResourceOfUser(collection.Owner.Id))
        {
            return Unauthorized("Only owner or admin can change collection");
        }

        var rule = await _ruleService.GetRule(ruleReference.Id);
        if (rule == null)
        {
            return NotFound();
        }

        await _collectionService.AddRule(id, rule);
        return Ok(); 
    }
    
    [HttpDelete("{id}/contained_rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRule(int id, int ruleId)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }
        
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser) ||
            !authUser!.CanAccessResourceOfUser(collection.Owner.Id))
        {
            return Unauthorized("Only owner or admin can change collection");
        }

        var rule = await _ruleService.GetRule(ruleId);
        if (rule == null)
        {
            return NotFound();
        }

        await _collectionService.RemoveRule(collection.Id, rule.Id);
        return NoContent();
    }
    
    [HttpGet("{id}/owner")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<UserDTO>> GetOwner(int id)
    {
        var owner = await _collectionService.GetOwner(id);
        if (owner == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<UserDTO>(owner));
    }
}