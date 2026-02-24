namespace Chuks_Kitchen.Models;

public class CartItem
{
    public Guid UserId { get; set; }
    public Guid FoodId { get; set; }
    public int Quantity { get; set; }
}
