using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class TransactionType
{
    public short Id { get; set; }

    public string TransactionTypeName { get; set; } = null!;

    public short DebitCredit { get; set; }

    public bool AllowGroup { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
