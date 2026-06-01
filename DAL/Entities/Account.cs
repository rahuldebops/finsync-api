using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class Account
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Name { get; set; } = null!;

    public short AccountTypeId { get; set; }

    public decimal Balance { get; set; }

    public DateTime BalanceAsOf { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public long UpdatedBy { get; set; }

    public virtual ICollection<AccountProfile> AccountProfiles { get; set; } = new List<AccountProfile>();

    public virtual AccountType AccountType { get; set; } = null!;

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
