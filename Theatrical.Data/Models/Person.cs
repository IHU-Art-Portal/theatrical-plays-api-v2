using System;
using System.Collections.Generic;

namespace Theatrical.Data.Models;

public class Person
{
    public int Id { get; set; }
    public string Fullname { get; set; } = null!;
    public int SystemId { get; set; }
    public DateTime Timestamp { get; set; }
    public virtual System System { get; set; } = null!;
    public virtual List<Contribution> Contributions { get; set; }
    public virtual List<Image> Images { get; set; }
    public string? HairColor { get; set; }
    public string? Height { get; set; }
    public string? EyeColor { get; set; }
    public string? Weight { get; set; }
    public List<string>? Languages { get; set; }
    public string? Description { get; set; }
    public string? Bio { get; set; }
    public DateTime? Birthdate { get; set; }
}