using System;
using System.Collections.Generic;

namespace Servis.vaja;

public partial class AppointmentException
{
    public string AppointmentId { get; set; } = null!;

    public DateTime ExceptionDate { get; set; }
}
