using System;
using System.Collections.Generic;

namespace Theatrical.Data.Models;

public partial class System
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual List<Contribution> Contributions { get; set; }
    public virtual List<Event> Events { get; set; }
    public virtual List<Organizer> Organizers { get; set; }
    public virtual List<Person> People { get; set; }
    public virtual List<Production> Productions { get; set; }
    public virtual List<Role> Roles { get; set; }
    public virtual List<Venue> Venues { get; set; }
}