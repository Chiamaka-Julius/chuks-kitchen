namespace Chuks_Kitchen.Models;

public class User
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ReferralCode { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
