using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class TransactionPayment
{
    public long Id { get; set; }

    public long TransactionId { get; set; }

    public long UserId { get; set; }

    public long ProfileId { get; set; }

    public long AccountId { get; set; }

    public decimal Amount { get; set; }

    public short DebitCredit { get; set; }

    public long? CategoryId { get; set; }

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public long UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
}
