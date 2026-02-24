using Chuks_Kitchen.Services.Carts;
using Chuks_Kitchen.Services.Contracts;

namespace Chuks_Kitchen.Services.BL.Interfaces;

public interface ICartBL
{
    Task<ApiResult<CartResponse>> AddItemAsync(AddCartItemCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult<CartResponse>> GetCartAsync(GetCartQuery query, CancellationToken cancellationToken = default);
    Task<ApiResult<string>> ClearCartAsync(ClearCartCommand command, CancellationToken cancellationToken = default);
}
