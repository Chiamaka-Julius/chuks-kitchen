using Chuks_Kitchen.Models.Enums;

namespace Chuks_Kitchen.API.Contracts;

public sealed record SignupRequest(string? Email, string? PhoneNumber, string? ReferralCode);
public sealed record VerifyRequest(Guid UserId, string Otp);
public sealed record AddFoodRequest(string Name, decimal Price, bool IsAvailable = true);
public sealed record UpdateAvailabilityRequest(bool IsAvailable);
public sealed record AddCartItemRequest(Guid UserId, Guid FoodId, int Quantity);
public sealed record CreateOrderRequest(Guid UserId);
public sealed record UpdateOrderStatusRequest(OrderStatus Status);
