using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Services;

public readonly record struct GetNeighborhoodsResult(ResultCode Result, NeighborhoodDTO[]? Response);
public readonly record struct GetNeighborhoodResult(ResultCode Result, NeighborhoodDTO? Response);

public interface INeighborhoodService
{
    Task<GetNeighborhoodsResult> GetNeighborhoods();

    Task<GetNeighborhoodResult> GetNeighborhood(int id);
}