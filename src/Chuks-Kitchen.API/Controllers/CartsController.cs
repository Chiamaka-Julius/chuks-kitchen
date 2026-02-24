using Chuks_Kitchen.API.Contracts;
using Chuks_Kitchen.API.Extensions;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.CQRS;
using Chuks_Kitchen.Services.Carts;
using Microsoft.AspNetCore.Mvc;

namespace Chuks_Kitchen.API.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartsController : ControllerBase
{
    private readonly ICartBL _cartBl;
    private readonly ICqrsDispatcher _cqrs;

    public CartsController(ICartBL cartBl, ICqrsDispatcher cqrs)
    {
        _cartBl = cartBl;
        _cqrs = cqrs;
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddCartItem(AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _cartBl.AddItemAsync(new AddCartItemCommand(request.UserId, request.FoodId, request.Quantity), ct),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetCart(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteQueryAsync(
            ct => _cartBl.GetCartAsync(new GetCartQuery(userId), ct),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> ClearCart(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _cartBl.ClearCartAsync(new ClearCartCommand(userId), ct),
            cancellationToken);

        return result.ToActionResult();
    }
}
