using Chuks_Kitchen.Models.Enums;

namespace Chuks_Kitchen.Services.Orders;

public static class OrderStatusTransitions
{
    public static bool IsValid(OrderStatus from, OrderStatus to)
    {
        if (from == to)
        {
            return true;
        }

        var transitions = new Dictionary<OrderStatus, HashSet<OrderStatus>>
        {
            [OrderStatus.Pending] = [OrderStatus.Confirmed, OrderStatus.Cancelled],
            [OrderStatus.Confirmed] = [OrderStatus.Preparing, OrderStatus.Cancelled],
            [OrderStatus.Preparing] = [OrderStatus.OutForDelivery, OrderStatus.Cancelled],
            [OrderStatus.OutForDelivery] = [OrderStatus.Completed, OrderStatus.Cancelled],
            [OrderStatus.Completed] = [],
            [OrderStatus.Cancelled] = []
        };

        return transitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }
}
