using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Theatrical.Data.Models;

namespace Theatrical.Data.Context;

public class TheatricalPlaysDbContext : DbContext
{
    public TheatricalPlaysDbContext(DbContextOptions<TheatricalPlaysDbContext> options, IConfiguration configuration) : base(options)
    {
    }

    public virtual DbSet<Person> persons { get; set; }
    public virtual DbSet<Organizer> Organizers { get; set; }
    public virtual DbSet<Roles> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Venue> Venues { get; set; }
    public virtual DbSet<Image> Images { get; set; }
    public virtual DbSet<Production> Productions { get; set; }
    public virtual DbSet<Contribution> Contributions { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    
}