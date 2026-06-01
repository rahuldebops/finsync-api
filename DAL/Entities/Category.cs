using System;
using System.Collections.Generic;

namespace finsyncapi.DAL.Entities;

public partial class Category
{
    public long Id { get; set; }

    public long? ProfileId { get; set; }

    public long? GroupId { get; set; }

    public string CategoryName { get; set; } = null!;

    public short TransactionTypeId { get; set; }

    public long? ParentCategoryId { get; set; }

    public string? Param1 { get; set; }

    public string? Param2 { get; set; }

    public string? Param3 { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public long UpdatedBy { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }

    public virtual TransactionType TransactionType { get; set; } = null!;
}
