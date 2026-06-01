using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class Ledger
{
    public long Id { get; set; }

    public long TransactionId { get; set; }

    public long? GroupId { get; set; }

    public long UserId { get; set; }

    public long ProfileId { get; set; }

    public long? AccountId { get; set; }

    public short Type { get; set; }

    public string Description { get; set; } = null!;

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public DateTime CreatedAt { get; set; }
}
