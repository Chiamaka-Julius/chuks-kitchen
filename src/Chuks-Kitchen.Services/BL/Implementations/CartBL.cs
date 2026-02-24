using Chuks_Kitchen.Migrations.Data;
using Chuks_Kitchen.Models;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.Carts;
using Chuks_Kitchen.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Chuks_Kitchen.Services.BL.Implementations;

public sealed class CartBL : ICartBL
{
    private readonly AppDbContext _db;

    public CartBL(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<CartResponse>> AddItemAsync(AddCartItemCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Quantity <= 0)
        {
            return ApiResult<CartResponse>.Fail("Quantity must be at least 1.", 400);
        }

        var userExists = await _db.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
        if (!userExists)
        {
            return ApiResult<CartResponse>.Fail("User not found.", 404);
        }

        var food = await _db.Foods.FirstOrDefaultAsync(f => f.Id == command.FoodId, cancellationToken);
        if (food is null)
        {
            return ApiResult<CartResponse>.Fail("Food item not found.", 404);
        }

        if (!food.IsAvailable)
        {
            return ApiResult<CartResponse>.Fail("Food item is currently unavailable.", 400);
        }

        var existing = await _db.CartItems.FirstOrDefaultAsync(c => c.UserId == command.UserId && c.FoodId == command.FoodId, cancellationToken);
        if (existing is null)
        {
            _db.CartItems.Add(new CartItem { UserId = command.UserId, FoodId = command.FoodId, Quantity = command.Quantity });
        }
        else
        {
            existing.Quantity += command.Quantity;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return await GetCartAsync(new GetCartQuery(command.UserId), cancellationToken);
    }

    public async Task<ApiResult<CartResponse>> GetCartAsync(GetCartQuery query, CancellationToken cancellationToken = default)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == query.UserId, cancellationToken);
        if (!userExists)
        {
            return ApiResult<CartResponse>.Fail("User not found.", 404);
        }

        var items = await _db.CartItems
            .Where(c => c.UserId == query.UserId)
            .Join(
                _db.Foods,
                c => c.FoodId,
                f => f.Id,
                (c, f) => new
                {
                    c.FoodId,
                    FoodName = f.Name,
                    UnitPrice = f.Price,
                    c.Quantity,
                    f.IsAvailable
                })
            .OrderBy(x => x.FoodName)
            .Select(x => new CartItemDetail(
                x.FoodId,
                x.FoodName,
                x.UnitPrice,
                x.Quantity,
                x.UnitPrice * x.Quantity,
                x.IsAvailable))
            .ToListAsync(cancellationToken);

        return ApiResult<CartResponse>.Ok(new CartResponse(query.UserId, items, items.Sum(i => i.LineTotal)));
    }

    public async Task<ApiResult<string>> ClearCartAsync(ClearCartCommand command, CancellationToken cancellationToken = default)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
        if (!userExists)
        {
            return ApiResult<string>.Fail("User not found.", 404);
        }

        var cartItems = await _db.CartItems.Where(c => c.UserId == command.UserId).ToListAsync(cancellationToken);
        _db.CartItems.RemoveRange(cartItems);
        await _db.SaveChangesAsync(cancellationToken);

        return ApiResult<string>.Ok("Cart cleared.");
    }
}
