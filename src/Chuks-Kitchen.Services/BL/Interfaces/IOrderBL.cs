using Chuks_Kitchen.Services.Contracts;
using Chuks_Kitchen.Services.Orders;

namespace Chuks_Kitchen.Services.BL.Interfaces;

public interface IOrderBL
{
    Task<ApiResult<OrderDto>> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult<OrderDto>> GetOrderAsync(GetOrderQuery query, CancellationToken cancellationToken = default);
    Task<ApiResult<OrderDto>> UpdateStatusAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default);
    Task<ApiResult<OrderDto>> CancelAsync(CancelOrderCommand command, CancellationToken cancellationToken = default);
}
