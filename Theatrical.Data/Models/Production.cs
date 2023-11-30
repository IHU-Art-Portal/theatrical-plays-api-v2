using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Theatrical.Data.Models;

public class Production
{
    public int Id { get; set; }
    public int? OrganizerId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Producer { get; set; } = null!;
    public string MediaUrl { get; set; } = null!;
    public string? Duration { get; set; } = null!;
    public int SystemId { get; set; }
    public DateTime Timestamp { get; set; }
    public virtual Organizer? Organizer { get; set; }
    public virtual System System { get; set; } = null!;
    public virtual List<Contribution> Contributions { get; set; }
    public virtual List<Event> Events { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}