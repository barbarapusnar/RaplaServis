using System;
using System.Collections.Generic;

namespace Servis.vaja;

public partial class ResourceAttributeValue
{
    public string ResourceId { get; set; } = null!;

    public string? AttributeKey { get; set; }

    public string? AttributeValue { get; set; }
}
