using Chuks_Kitchen.Services.Contracts;
using Chuks_Kitchen.Services.Foods;

namespace Chuks_Kitchen.Services.BL.Interfaces;

public interface IFoodBL
{
    Task<ApiResult<IReadOnlyList<FoodDto>>> GetFoodsAsync(GetFoodsQuery query, CancellationToken cancellationToken = default);
    Task<ApiResult<FoodDto>> AddFoodAsync(AddFoodCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult<FoodDto>> UpdateAvailabilityAsync(UpdateFoodAvailabilityCommand command, CancellationToken cancellationToken = default);
}
