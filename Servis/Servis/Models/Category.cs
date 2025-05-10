using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class Category
{
    public string Id { get; set; } = null!;

    public string? ParentId { get; set; }

    public string CategoryKey { get; set; } = null!;

    public string Definition { get; set; } = null!;

    public int? ParentOrder { get; set; }
}
