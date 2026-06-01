using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class Transaction
{
    public long Id { get; set; }

    public long ProfileId { get; set; }

    public long? GroupId { get; set; }

    public long? AccountId { get; set; }

    public decimal Amount { get; set; }

    public short? TransactionTypeId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public long UpdatedBy { get; set; }

    public long UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public int Version { get; set; }

    public short? SplitType { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();

    public virtual Group? Group { get; set; }

    public virtual Profile Profile { get; set; } = null!;

    public virtual TransactionType? TransactionType { get; set; }
}
