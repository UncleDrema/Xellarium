using System.Security.Claims;
using System.Text.Json;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xellarium.Authentication;
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
public class RulesController(IRuleService _service, IUserService _userService, ICollectionService _collectionService,
    INeighborhoodService _neighborhoodService, IMapper mapper,
    ILogger<RulesController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.Admin)]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetRules()
    {
        var rules = await _service.GetRules();
        return Ok(rules.Select(mapper.Map<RuleDTO>));
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RuleDTO>> GetRule(int id)
    {
        var rule = await _service.GetRule(id);
        if (rule == null)
        {
            return NotFound();
        }

        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            return NotFound();
        }

        return Ok(mapper.Map<RuleDTO>(rule));
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    [EndpointSummary("Adds new rule for current user")]
    public async Task<ActionResult<RuleDTO>> AddRule(PostRuleDTO rule)
    {
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser))
        {
            return Unauthorized();
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
            Owner = user,
            Name = rule.Name,
            GenericRule = rule.GenericRule,
        };
        await _service.AddRule(ruleEntity);
        return CreatedAtAction(nameof(GetRule), new {id = ruleEntity.Id},
            mapper.Map<RuleDTO>(ruleEntity));
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<RuleDTO>> UpdateRule(int id, RuleDTO rule)
    {
        if (id != rule.Id)
        {
            return BadRequest();
        }

        var ruleEntity = await _service.GetRule(id);
        if (ruleEntity is null)
        {
            return NotFound($"Rule with id={id} is not found");
        }
        
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser) ||
            !authUser!.CanAccessResourceOfUser(ruleEntity.Owner.Id))
        {
            return Unauthorized("Only owner or admin can change rule");
        }

        if (rule.OwnerId != ruleEntity.Owner.Id &&
            authUser.Role != UserRole.Admin)
        {
            return Unauthorized("Only admin can change rule ownership");
        }

        var neighborhood = await _neighborhoodService.GetNeighborhood(rule.NeighborhoodId);

        if (neighborhood is null)
        {
            return NotFound($"Not found neighborhood with id {rule.NeighborhoodId}");
        }

        if (ruleEntity.Owner.Id != rule.OwnerId)
        {
            ruleEntity.Owner = (await _userService.GetUser(rule.OwnerId))!;
        }
        
        ruleEntity.Name = rule.Name;
        ruleEntity.GenericRule = rule.GenericRule;
        ruleEntity.Neighborhood = neighborhood;
        ruleEntity.Collections = rule.CollectionReferences.Select(c => _collectionService.GetCollection(c.Id).Result).ToList()!;
        
        await _service.UpdateRule(ruleEntity);
        
        return Ok(mapper.Map<RuleDTO>(ruleEntity));
    }
    
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<RuleDTO>> PatchRule(int id, RulePatchDTO patchDto)
    {
        var ruleEntity = await _service.GetRule(id);
        if (ruleEntity is null)
        {
            return NotFound($"Rule with id={id} is not found");
        }
        
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser) ||
            !authUser!.CanAccessResourceOfUser(ruleEntity.Owner.Id))
        {
            return Unauthorized();
        }

        if (patchDto.Name is not null)
            ruleEntity.Name = patchDto.Name;
        if (patchDto.GenericRule is not null)
            ruleEntity.GenericRule = patchDto.GenericRule;
        if (patchDto.NeighborhoodId is not null)
            ruleEntity.Neighborhood = (await _neighborhoodService.GetNeighborhood(patchDto.NeighborhoodId.Value))!;
        
        await _service.UpdateRule(ruleEntity);
        return Ok(mapper.Map<RuleDTO>(ruleEntity));
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<IActionResult> DeleteRule(int id)
    {
        var ruleEntity = await _service.GetRule(id);
        if (ruleEntity is null)
        {
            return NotFound();
        }
        
        if (!HttpContext.TryGetAuthenticatedUser(out var authUser) ||
            !authUser!.CanAccessResourceOfUser(ruleEntity.Owner.Id))
        {
            return Unauthorized();
        }
        
        await _service.DeleteRule(id);
        return NoContent();
    }
    
    [HttpGet("{id}/owner")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = JwtAuthPolicies.AdminOrUser)]
    public async Task<ActionResult<UserDTO>> GetOwner(int id)
    {
        var owner = await _service.GetOwner(id);
        if (owner == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<UserDTO>(owner));
    }
}