using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class AccountType
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
