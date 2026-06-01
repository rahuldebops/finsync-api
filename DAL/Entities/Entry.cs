using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class Entry
{
    public long Id { get; set; }

    public long ProfileId { get; set; }

    public long? FromProfileId { get; set; }

    public long? GroupId { get; set; }

    public long? AccountId { get; set; }

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public string BaseCurrencyCode { get; set; } = null!;

    public short? TransactionTypeId { get; set; }

    public short DebitCredit { get; set; }

    public long? CategoryId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public bool IsShared { get; set; }

    public long? SharedWith { get; set; }

    public short? SharedStatus { get; set; }

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public long UpdatedBy { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Currency BaseCurrencyCodeNavigation { get; set; } = null!;

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public virtual Group? Group { get; set; }

    public virtual TransactionType? TransactionType { get; set; }
}
