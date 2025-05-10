using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class Allocationtest
{
    public int Id { get; set; }

    public string AppointmentId { get; set; } = null!;

    public string ResourceId { get; set; } = null!;

    public int? ParentOrder { get; set; }
}
