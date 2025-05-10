using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class AppointmentException
{
    public string AppointmentId { get; set; } = null!;

    public DateTime ExceptionDate { get; set; }
}
