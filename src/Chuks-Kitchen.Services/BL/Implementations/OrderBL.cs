using Chuks_Kitchen.Migrations.Data;
using Chuks_Kitchen.Models;
using Chuks_Kitchen.Models.Enums;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.Contracts;
using Chuks_Kitchen.Services.Orders;
using Microsoft.EntityFrameworkCore;

namespace Chuks_Kitchen.Services.BL.Implementations;

public sealed class OrderBL : IOrderBL
{
    private readonly AppDbContext _db;

    public OrderBL(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<OrderDto>> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            return ApiResult<OrderDto>.Fail("User not found.", 404);
        }

        if (!user.IsVerified)
        {
            return ApiResult<OrderDto>.Fail("User is not verified.", 400);
        }

        var cartItems = await _db.CartItems
            .Where(c => c.UserId == command.UserId)
            .Join(_db.Foods, c => c.FoodId, f => f.Id, (c, f) => new { c.FoodId, c.Quantity, f.Name, f.Price, f.IsAvailable })
            .ToListAsync(cancellationToken);

        if (cartItems.Count == 0)
        {
            return ApiResult<OrderDto>.Fail("Cart is empty.", 400);
        }

        var unavailable = cartItems.FirstOrDefault(c => !c.IsAvailable);
        if (unavailable is not null)
        {
            return ApiResult<OrderDto>.Fail($"Food item '{unavailable.Name}' became unavailable.", 400);
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                TotalAmount = cartItems.Sum(c => c.Price * c.Quantity),
                CreatedAtUtc = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            _db.Orders.Add(order);
            _db.OrderItems.AddRange(cartItems.Select(c => new OrderItem
            {
                OrderId = order.Id,
                FoodId = c.FoodId,
                Quantity = c.Quantity,
                UnitPrice = c.Price
            }));

            var toClear = await _db.CartItems.Where(c => c.UserId == command.UserId).ToListAsync(cancellationToken);
            _db.CartItems.RemoveRange(toClear);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await GetOrderAsync(new GetOrderQuery(order.Id), cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ApiResult<OrderDto>> GetOrderAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == query.OrderId, cancellationToken);
        if (order is null)
        {
            return ApiResult<OrderDto>.Fail("Order not found.", 404);
        }

        var items = await _db.OrderItems
            .Where(oi => oi.OrderId == query.OrderId)
            .Join(_db.Foods, oi => oi.FoodId, f => f.Id,
                (oi, f) => new OrderItemDto(oi.FoodId, f.Name, oi.Quantity, oi.UnitPrice, oi.UnitPrice * oi.Quantity))
            .OrderBy(i => i.FoodName)
            .ToListAsync(cancellationToken);

        return ApiResult<OrderDto>.Ok(new OrderDto(order.Id, order.UserId, order.TotalAmount, order.CreatedAtUtc, order.Status, items));
    }

    public async Task<ApiResult<OrderDto>> UpdateStatusAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == command.OrderId, cancellationToken);
        if (order is null)
        {
            return ApiResult<OrderDto>.Fail("Order not found.", 404);
        }

        if (!OrderStatusTransitions.IsValid(order.Status, command.Status))
        {
            return ApiResult<OrderDto>.Fail($"Invalid status transition from {order.Status} to {command.Status}.", 400);
        }

        order.Status = command.Status;
        await _db.SaveChangesAsync(cancellationToken);

        return await GetOrderAsync(new GetOrderQuery(command.OrderId), cancellationToken);
    }

    public Task<ApiResult<OrderDto>> CancelAsync(CancelOrderCommand command, CancellationToken cancellationToken = default)
    {
        return UpdateStatusAsync(new UpdateOrderStatusCommand(command.OrderId, OrderStatus.Cancelled), cancellationToken);
    }
}
