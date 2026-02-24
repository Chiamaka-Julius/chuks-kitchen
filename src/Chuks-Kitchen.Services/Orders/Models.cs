using Chuks_Kitchen.Models.Enums;

namespace Chuks_Kitchen.Services.Orders;

public sealed record CreateOrderCommand(Guid UserId);
public sealed record GetOrderQuery(Guid OrderId);
public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status);
public sealed record CancelOrderCommand(Guid OrderId);

public sealed record OrderItemDto(Guid FoodId, string FoodName, int Quantity, decimal UnitPrice, decimal LineTotal);
public sealed record OrderDto(Guid Id, Guid UserId, decimal TotalAmount, DateTime CreatedAtUtc, OrderStatus Status, IReadOnlyList<OrderItemDto> Items);
