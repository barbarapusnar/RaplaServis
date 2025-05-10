using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class Appointment
{
    public string Id { get; set; } = null!;

    public string EventId { get; set; } = null!;

    public DateTime AppointmentStart { get; set; }

    public DateTime AppointmentEnd { get; set; }

    public string? RepetitionType { get; set; }

    public int? RepetitionNumber { get; set; }

    public DateTime? RepetitionEnd { get; set; }

    public int? RepetitionInterval { get; set; }
}
