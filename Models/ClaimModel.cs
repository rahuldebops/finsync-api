using finsyncapi.Helper;
using finsyncapi.Models;

public class ClaimModel
{
    public SnowFlakeId UserId { get; }
    public string? Email { get; }
    public string? PhoneNumber { get; }
    public string? GoogleId { get; }
    public int UserRole { get; }
    public string? TimeZone { get; }

    // 🔹 Email login
    public ClaimModel(SnowFlakeId userId, int userRole, string? email = null, string? phoneNumber = null, string? timeZone = null)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("At least one identity (email or phone) is required");

        UserId = userId;
        Email = email;
        UserRole = userRole;
        TimeZone = timeZone;
        PhoneNumber = phoneNumber;
    }
}



public class UserContext
{
    public SnowFlakeId UserId { get; }
    public string? PhoneNumber { get; }
    public string? Email { get; }
    public int UserRole { get; }
    public string? TimeZone { get; }
    public SnowFlakeId? ProfileId { get; }

    public UserContext(SnowFlakeId userId, int userRole, string? phoneNumber, string? email, string? timeZone, SnowFlakeId? profileId)
    {
        // 🔒 enforce at least one identity
        if (string.IsNullOrWhiteSpace(phoneNumber) && string.IsNullOrWhiteSpace(email))
        {
            throw new AppException("Invalid token: no identity present");
        }

        UserId = userId;
        UserRole = userRole;
        PhoneNumber = phoneNumber;
        Email = email;
        TimeZone = timeZone;
        ProfileId = profileId;
    }
}