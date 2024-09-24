using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.Shared.DTO;

namespace Xellarium.WebApi.V2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]
public class CollectionController(ICollectionService _collectionService, IRuleService _ruleService,
    IMapper mapper, ILogger<CollectionController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CollectionDTO>>> GetCollections()
    {
        var collections = await _collectionService.GetCollections();
        return Ok(collections.Select(mapper.Map<CollectionDTO>));
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<CollectionDTO>> GetCollection(int id)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<CollectionDTO>(collection));
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<CollectionDTO>> AddCollection(PostCollectionDTO collection)
    {
        var newCollection = await _collectionService.AddCollection(mapper.Map<Collection>(collection));
        return CreatedAtAction(nameof(GetCollection), new {id = newCollection.Id},
            mapper.Map<CollectionDTO>(newCollection));
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCollection(int id, CollectionDTO collection)
    {
        if (id != collection.Id)
        {
            return BadRequest();
        }
        
        await _collectionService.UpdateCollection(mapper.Map<Collection>(collection));
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        if (!await _collectionService.CollectionExists(id))
        {
            return NotFound();
        }
        
        await _collectionService.DeleteCollection(id);
        return NoContent();
    }
    
    [HttpGet("{id}/rules")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RuleDTO>>> GetCollectionRules(int id)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }

        var rules = collection.Rules;
        return Ok(rules.Select(mapper.Map<RuleDTO>));
    }

    [HttpPost("{id}/rules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRule(int id, RuleReferenceDTO ruleReference)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }

        var rule = await _ruleService.GetRule(ruleReference.Id);
        if (rule == null)
        {
            return NotFound();
        }

        await _collectionService.AddRule(id, rule);
        return Ok(); 
    }
    
    [HttpDelete("{id}/rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRule(int id, int ruleId)
    {
        var collection = await _collectionService.GetCollection(id);
        if (collection == null)
        {
            return NotFound();
        }

        var rule = await _ruleService.GetRule(ruleId);
        if (rule == null)
        {
            return NotFound();
        }
        foreach (var r in collection.Rules)
        {
            logger.LogInformation("Rule: {Id}, {Name}", r.Id, r.Name);
        }
        logger.LogInformation("Removing rule: {Id}, {Name} {Contains}", rule.Id, rule.Name, collection.Rules.Contains(rule));
            

        await _collectionService.RemoveRule(collection.Id, rule.Id);
        return NoContent();
    }
    
    [HttpPut("{id}/privacy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrivacy(int id, bool isPrivate)
    {
        if (!await _collectionService.CollectionExists(id))
        {
            return NotFound();
        }

        await _collectionService.SetPrivacy(id, isPrivate);
        return NoContent();
    }
    
    [HttpGet("{id}/owner")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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