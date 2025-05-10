using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class RaplaConflict
{
    public string ResourceId { get; set; } = null!;

    public string Appointment1 { get; set; } = null!;

    public string Appointment2 { get; set; } = null!;

    public int App1enabled { get; set; }

    public int App2enabled { get; set; }

    public DateTime? LastChanged { get; set; }
}
