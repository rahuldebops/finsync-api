using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class Currency
{
    public short Id { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
