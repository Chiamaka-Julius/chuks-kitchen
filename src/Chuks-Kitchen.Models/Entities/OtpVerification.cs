namespace Chuks_Kitchen.Models;

public class OtpVerification
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
