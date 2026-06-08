using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class ExpenseSplit
{
    public long Id { get; set; }

    public long TransactionId { get; set; }

    public long? GroupId { get; set; }

    public long UserId { get; set; }

    public long ProfileId { get; set; }

    public decimal OwedAmount { get; set; }

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Percentage { get; set; }

    public decimal? Share { get; set; }

    public virtual Profile Profile { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
