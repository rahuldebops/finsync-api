using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class RefreshToken
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }
}
