using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Services;

public interface INeighborhoodService
{
    Task<IEnumerable<Neighborhood>> GetNeighborhoods();
    Task<Neighborhood?> GetNeighborhood(int id);
    Task<Neighborhood> AddNeighborhood(Neighborhood rule);
    Task UpdateNeighborhood(Neighborhood rule);
    Task DeleteNeighborhood(int id);
    Task<bool> NeighborhoodExists(int id);
}