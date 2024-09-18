using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;

namespace Xellarium.BusinessLogic.Services;

public class NeighborhoodService : INeighborhoodService
{
    private readonly INeighborhoodRepository _neighborhoodRepository;

    public NeighborhoodService(INeighborhoodRepository neighborhoodRepository)
    {
        _neighborhoodRepository = neighborhoodRepository;
    }

    public async Task<IEnumerable<Neighborhood>> GetNeighborhoods()
    {
        return await _neighborhoodRepository.GetAll();
    }

    public async Task<Neighborhood?> GetNeighborhood(int id)
    {
        return await _neighborhoodRepository.Get(id);
    }

    public async Task<Neighborhood> AddNeighborhood(Neighborhood rule)
    {
        if (await _neighborhoodRepository.Exists(rule.Id)) throw new ArgumentException("Neighborhood already exists");
        return await _neighborhoodRepository.Add(rule);
    }

    public async Task UpdateNeighborhood(Neighborhood rule)
    {
        if (!await _neighborhoodRepository.Exists(rule.Id)) throw new ArgumentException("Neighborhood not found");
        await _neighborhoodRepository.Update(rule);
    }

    public async Task DeleteNeighborhood(int id)
    {
        var rule = await _neighborhoodRepository.Get(id, true);
        if (rule == null || rule.IsDeleted) throw new ArgumentException("Neighborhood not found");
        await _neighborhoodRepository.SoftDelete(id);
    }

    public Task<bool> NeighborhoodExists(int id)
    {
        return _neighborhoodRepository.Exists(id);
    }
}