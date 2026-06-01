using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class User
{
    public long Id { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? GoogleId { get; set; }

    public string? FullName { get; set; }

    public short UserRole { get; set; }

    public string? PasswordHash { get; set; }

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsVerified { get; set; }

    public long CreatedBy { get; set; }

    public long UpdatedBy { get; set; }

    public virtual UserRole UserRoleNavigation { get; set; } = null!;
}
