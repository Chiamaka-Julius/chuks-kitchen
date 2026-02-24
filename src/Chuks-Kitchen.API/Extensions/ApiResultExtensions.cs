using Chuks_Kitchen.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Chuks_Kitchen.API.Extensions;

public static class ApiResultExtensions
{
    public static IActionResult ToActionResult<T>(this ApiResult<T> result)
    {
        if (result.Success)
        {
            return result.StatusCode switch
            {
                201 => new CreatedResult(string.Empty, result.Data),
                204 => new NoContentResult(),
                _ => new OkObjectResult(result.Data)
            };
        }

        var payload = new { message = result.Error };
        return result.StatusCode switch
        {
            400 => new BadRequestObjectResult(payload),
            404 => new NotFoundObjectResult(payload),
            409 => new ConflictObjectResult(payload),
            _ => new ObjectResult(payload) { StatusCode = result.StatusCode }
        };
    }
}
