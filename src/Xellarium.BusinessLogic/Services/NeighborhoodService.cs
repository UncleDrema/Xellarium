using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;

namespace Xellarium.BusinessLogic.Services;

public class NeighborhoodService(IUnitOfWork unitOfWork) : INeighborhoodService
{
    public async Task<IEnumerable<Neighborhood>> GetNeighborhoods()
    {
        return await unitOfWork.Neighborhoods.GetAll();
    }

    public async Task<Neighborhood?> GetNeighborhood(int id)
    {
        return await unitOfWork.Neighborhoods.Get(id);
    }

    public async Task AddNeighborhood(Neighborhood rule)
    {
        if (await unitOfWork.Neighborhoods.Exists(rule.Id)) throw new ArgumentException("Neighborhood already exists");
        await unitOfWork.Neighborhoods.Add(rule);
        await unitOfWork.CompleteAsync();
    }

    public async Task UpdateNeighborhood(Neighborhood rule)
    {
        if (!await unitOfWork.Neighborhoods.Exists(rule.Id)) throw new ArgumentException("Neighborhood not found");
        await unitOfWork.Neighborhoods.Update(rule);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteNeighborhood(int id)
    {
        var rule = await unitOfWork.Neighborhoods.Get(id);
        if (rule == null) throw new ArgumentException("Neighborhood not found");
        await unitOfWork.Neighborhoods.SoftDelete(id);
        await unitOfWork.CompleteAsync();
    }

    public Task<bool> NeighborhoodExists(int id)
    {
        return unitOfWork.Neighborhoods.Exists(id);
    }
}