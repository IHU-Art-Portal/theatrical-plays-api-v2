using System;
using System.Collections.Generic;

namespace Theatrical.Data.Models;

public class Authority
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual List<User> Users { get; set; }
}