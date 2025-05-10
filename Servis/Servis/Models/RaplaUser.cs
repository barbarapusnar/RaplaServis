using System;
using System.Collections.Generic;

namespace Servis.Models;

public partial class RaplaUser
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Isadmin { get; set; }

    public DateTime? CreationTime { get; set; }

    public DateTime? LastChanged { get; set; }
}
