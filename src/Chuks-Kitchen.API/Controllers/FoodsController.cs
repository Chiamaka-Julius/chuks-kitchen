using Chuks_Kitchen.API.Contracts;
using Chuks_Kitchen.API.Extensions;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.CQRS;
using Chuks_Kitchen.Services.Foods;
using Microsoft.AspNetCore.Mvc;

namespace Chuks_Kitchen.API.Controllers;

[ApiController]
[Route("api/foods")]
public sealed class FoodsController : ControllerBase
{
    private readonly IFoodBL _foodBl;
    private readonly ICqrsDispatcher _cqrs;

    public FoodsController(IFoodBL foodBl, ICqrsDispatcher cqrs)
    {
        _foodBl = foodBl;
        _cqrs = cqrs;
    }

    [HttpGet]
    public async Task<IActionResult> GetFoods(bool includeUnavailable, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteQueryAsync(
            ct => _foodBl.GetFoodsAsync(new GetFoodsQuery(includeUnavailable), ct),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> AddFood(AddFoodRequest request, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _foodBl.AddFoodAsync(new AddFoodCommand(request.Name, request.Price, request.IsAvailable), ct),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{foodId:guid}/availability")]
    public async Task<IActionResult> UpdateAvailability(Guid foodId, UpdateAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await _cqrs.ExecuteCommandAsync(
            ct => _foodBl.UpdateAvailabilityAsync(new UpdateFoodAvailabilityCommand(foodId, request.IsAvailable), ct),
            cancellationToken);

        return result.ToActionResult();
    }
}
