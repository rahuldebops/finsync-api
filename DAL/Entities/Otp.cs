using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class Otp
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public short Purpose { get; set; }

    public string OtpHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public int Attempts { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }
}
