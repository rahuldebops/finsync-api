using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class UserRole
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
