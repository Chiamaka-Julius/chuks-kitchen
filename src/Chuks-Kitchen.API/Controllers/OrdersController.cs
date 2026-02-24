using Chuks_Kitchen.API.Contracts;
using Chuks_Kitchen.API.Extensions;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.CQRS;
using Chuks_Kitchen.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Chuks_Kitchen.API.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderBL _orderBl;
    private readonly ICqrsDispatcher _cqrs;

    public OrdersController(IOrderBL orderBl, ICqrsDispatcher cqrs)
    {
        _orderBl = orderBl;
        _cqrs = cqrs;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _orderBl.CreateOrderAsync(new CreateOrderCommand(request.UserId), ct),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteQueryAsync(
            ct => _orderBl.GetOrderAsync(new GetOrderQuery(orderId), ct),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPatch("{orderId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _orderBl.UpdateStatusAsync(new UpdateOrderStatusCommand(orderId, request.Status), ct),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _orderBl.CancelAsync(new CancelOrderCommand(orderId), ct),
            cancellationToken);

        return result.ToActionResult();
    }
}
