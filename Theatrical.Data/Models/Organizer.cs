using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Theatrical.Data.Models;
public class Organizer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Town { get; set; } = null!;
    public string Postcode { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Doy { get; set; } = null!;
    public string Afm { get; set; } = null!;
    public int SystemId { get; set; }
    public DateTime Timestamp { get; set; }

    public virtual System System { get; set; } = null!;
    public virtual List<Production> Productions { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}