using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class GroupProfile
{
    public long Id { get; set; }

    public long GroupId { get; set; }

    public long ProfileId { get; set; }

    /// <summary>
    /// {1 : ADMIN, 2 : MEMBER}
    /// </summary>
    public short GroupRole { get; set; }

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public long UpdatedBy { get; set; }
}
