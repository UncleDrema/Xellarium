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
public class NeighbourhoodsController(INeighborhoodService _service, IMapper mapper,
    ILogger<NeighbourhoodsController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NeighborhoodDTO>>> GetNeighbourhoods()
    {
        var neighborhoods = await _service.GetNeighborhoods();
        return Ok(neighborhoods.Select(mapper.Map<NeighborhoodDTO>));
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<NeighborhoodDTO>> GetNeighbourhood(int id)
    {
        var neighborhood = await _service.GetNeighborhood(id);
        if (neighborhood == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<NeighborhoodDTO>(neighborhood));
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Policy = JwtAuthPolicies.Admin)]
    public async Task<ActionResult<NeighborhoodDTO>> AddNeighbourhood(PostNeighborhoodDTO neighborhood)
    {
        var neighbourhoodEntity = mapper.Map<Neighborhood>(neighborhood);
        await _service.AddNeighborhood(neighbourhoodEntity);
        return CreatedAtAction(nameof(GetNeighbourhood), new {id = neighbourhoodEntity.Id},
            mapper.Map<NeighborhoodDTO>(neighbourhoodEntity));
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.Admin)]
    public async Task<ActionResult<NeighborhoodDTO>> UpdateNeighbourhood(int id, NeighborhoodDTO neighborhood)
    {
        if (id != neighborhood.Id)
        {
            return BadRequest();
        }

        var neighbourhoodEntity = await _service.GetNeighborhood(id);
        if (neighbourhoodEntity == null)
        {
            return NotFound();
        }

        neighbourhoodEntity.Name = neighborhood.Name;
        neighbourhoodEntity.Offsets = neighborhood.Offsets;
        await _service.UpdateNeighborhood(neighbourhoodEntity);
        
        return Ok(mapper.Map<NeighborhoodDTO>(neighbourhoodEntity));
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = JwtAuthPolicies.Admin)]
    public async Task<IActionResult> DeleteNeighbourhood(int id)
    {
        if (!await _service.NeighborhoodExists(id))
        {
            return NotFound();
        }

        await _service.DeleteNeighborhood(id);
        return NoContent();
    }
}