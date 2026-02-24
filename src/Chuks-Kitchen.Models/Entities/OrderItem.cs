namespace Chuks_Kitchen.Models;

public class OrderItem
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid FoodId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
