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
public class RuleController(IRuleService _service, IMapper mapper,
    ILogger<RuleController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

        return Ok(mapper.Map<RuleDTO>(rule));
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<RuleDTO>> AddRule(PostRuleDTO rule)
    {
        var newRule = await _service.AddRule(mapper.Map<Rule>(rule));
        return CreatedAtAction(nameof(GetRule), new {id = newRule.Id},
            mapper.Map<RuleDTO>(newRule));
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRule(int id, RuleDTO rule)
    {
        if (id != rule.Id)
        {
            return BadRequest();
        }
        
        await _service.UpdateRule(mapper.Map<Rule>(rule));
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRule(int id)
    {
        if (!await _service.RuleExists(id))
        {
            return NotFound();
        }
        
        await _service.DeleteRule(id);
        return NoContent();
    }
    
    [HttpGet("{id}/owner")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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