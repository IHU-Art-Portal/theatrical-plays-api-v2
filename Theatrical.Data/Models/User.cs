using System;
using System.Collections.Generic;

namespace Theatrical.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public bool? Enabled { get; set; }

    public virtual List<UserAuthority> UserAuthorities { get; set; }
}