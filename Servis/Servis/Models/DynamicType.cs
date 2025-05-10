using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class DynamicType
{
    public string Id { get; set; } = null!;

    public string TypeKey { get; set; } = null!;

    public string Definition { get; set; } = null!;
}
