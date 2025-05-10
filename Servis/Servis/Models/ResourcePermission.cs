using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class ResourcePermission
{
    public string ResourceId { get; set; } = null!;

    public string? UserId { get; set; }

    public string? GroupId { get; set; }

    public int AccessLevel { get; set; }

    public int? MinAdvance { get; set; }

    public int? MaxAdvance { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
