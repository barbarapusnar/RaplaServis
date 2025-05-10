using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class EventAttributeValue
{
    public string EventId { get; set; } = null!;

    public string? AttributeKey { get; set; }

    public string? AttributeValue { get; set; }
}
