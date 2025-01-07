using System;
using System.Collections.Generic;

namespace Servis.vaja;

public partial class Event
{
    public string Id { get; set; } = null!;

    public string TypeKey { get; set; } = null!;

    public string OwnerId { get; set; } = null!;

    public DateTime? CreationTime { get; set; }

    public DateTime? LastChanged { get; set; }

    public string? LastChangedBy { get; set; }
}
