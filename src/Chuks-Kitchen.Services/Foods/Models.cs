using Chuks_Kitchen.Models;

namespace Chuks_Kitchen.Services.Foods;

public sealed record GetFoodsQuery(bool IncludeUnavailable);
public sealed record AddFoodCommand(string Name, decimal Price, bool IsAvailable = true);
public sealed record UpdateFoodAvailabilityCommand(Guid FoodId, bool IsAvailable);
public sealed record FoodDto(Guid Id, string Name, decimal Price, bool IsAvailable)
{
    public static FoodDto FromEntity(Food f) => new(f.Id, f.Name, f.Price, f.IsAvailable);
}
