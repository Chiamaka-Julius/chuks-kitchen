using Chuks_Kitchen.Migrations.Data;
using Chuks_Kitchen.Models;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.Contracts;
using Chuks_Kitchen.Services.Foods;
using Microsoft.EntityFrameworkCore;

namespace Chuks_Kitchen.Services.BL.Implementations;

public sealed class FoodBL : IFoodBL
{
    private readonly AppDbContext _db;

    public FoodBL(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<IReadOnlyList<FoodDto>>> GetFoodsAsync(GetFoodsQuery query, CancellationToken cancellationToken = default)
    {
        var foods = await _db.Foods
            .Where(f => query.IncludeUnavailable || f.IsAvailable)
            .OrderBy(f => f.Name)
            .Select(f => new FoodDto(f.Id, f.Name, f.Price, f.IsAvailable))
            .ToListAsync(cancellationToken);

        return ApiResult<IReadOnlyList<FoodDto>>.Ok(foods);
    }

    public async Task<ApiResult<FoodDto>> AddFoodAsync(AddFoodCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return ApiResult<FoodDto>.Fail("Food name is required.", 400);
        }

        if (command.Price <= 0)
        {
            return ApiResult<FoodDto>.Fail("Price must be greater than zero.", 400);
        }

        var food = new Food
        {
            Id = Guid.NewGuid(),
            Name = command.Name.Trim(),
            Price = command.Price,
            IsAvailable = command.IsAvailable
        };

        _db.Foods.Add(food);
        await _db.SaveChangesAsync(cancellationToken);

        return ApiResult<FoodDto>.Ok(new FoodDto(food.Id, food.Name, food.Price, food.IsAvailable), 201);
    }

    public async Task<ApiResult<FoodDto>> UpdateAvailabilityAsync(UpdateFoodAvailabilityCommand command, CancellationToken cancellationToken = default)
    {
        var food = await _db.Foods.FirstOrDefaultAsync(f => f.Id == command.FoodId, cancellationToken);
        if (food is null)
        {
            return ApiResult<FoodDto>.Fail("Food item not found.", 404);
        }

        food.IsAvailable = command.IsAvailable;
        await _db.SaveChangesAsync(cancellationToken);

        return ApiResult<FoodDto>.Ok(new FoodDto(food.Id, food.Name, food.Price, food.IsAvailable));
    }
}
