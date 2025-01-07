using System;
using System.Collections.Generic;

namespace Servis.vaja;

public partial class Allocation
{
    public string AppointmentId { get; set; } = null!;

    public string ResourceId { get; set; } = null!;

    public int? ParentOrder { get; set; }
}
