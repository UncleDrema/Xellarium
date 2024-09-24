using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.BusinessLogic.Services;
using Xellarium.Shared.DTO;

namespace Xellarium.WebApi.V2;


[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]
public class NeighbourhoodController(INeighborhoodService _service, IMapper mapper,
    ILogger<NeighbourhoodController> logger) : ControllerBase
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
    public async Task<ActionResult<NeighborhoodDTO>> AddNeighbourhood(PostNeighborhoodDTO neighborhood)
    {
        var newNeighborhood = await _service.AddNeighborhood(mapper.Map<Neighborhood>(neighborhood));
        return CreatedAtAction(nameof(GetNeighbourhood), new {id = newNeighborhood.Id},
            mapper.Map<NeighborhoodDTO>(newNeighborhood));
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateNeighbourhood(int id, NeighborhoodDTO neighborhood)
    {
        if (id != neighborhood.Id)
        {
            return BadRequest();
        }

        try
        {
            await _service.UpdateNeighborhood(mapper.Map<Neighborhood>(neighborhood));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating neighborhood");
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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