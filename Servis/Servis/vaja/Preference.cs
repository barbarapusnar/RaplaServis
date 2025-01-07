using System;
using System.Collections.Generic;

namespace Servis.vaja;

public partial class Preference
{
    public string? UserId { get; set; }

    public string Role { get; set; } = null!;

    public string? StringValue { get; set; }

    public string? XmlValue { get; set; }
}
