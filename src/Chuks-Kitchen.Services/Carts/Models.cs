namespace Chuks_Kitchen.Services.Carts;

public sealed record AddCartItemCommand(Guid UserId, Guid FoodId, int Quantity);
public sealed record ClearCartCommand(Guid UserId);
public sealed record GetCartQuery(Guid UserId);

public sealed record CartItemDetail(Guid FoodId, string FoodName, decimal UnitPrice, int Quantity, decimal LineTotal, bool IsAvailable);
public sealed record CartResponse(Guid UserId, IReadOnlyList<CartItemDetail> Items, decimal Total);
