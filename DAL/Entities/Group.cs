using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class Group
{
    public long Id { get; set; }

    public string GroupName { get; set; } = null!;

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public long UpdatedBy { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
