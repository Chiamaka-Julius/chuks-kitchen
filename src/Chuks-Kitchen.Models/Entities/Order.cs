using System.ComponentModel.DataAnnotations.Schema;
using Chuks_Kitchen.Models.Enums;

namespace Chuks_Kitchen.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public OrderStatus Status { get; set; }

    [NotMapped]
    public List<CartItem> Items { get; set; } = [];
}
