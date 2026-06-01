using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class AuditLog
{
    public int Id { get; set; }

    public string TableName { get; set; } = null!;

    public string RecordId { get; set; } = null!;

    public long? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string? Changes { get; set; }

    public DateTime CreatedAt { get; set; }
}
