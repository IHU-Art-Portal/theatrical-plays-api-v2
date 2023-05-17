using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Theatrical.Data.Models;

namespace Theatrical.Data.Context;

public class TheatricalPlaysDbContext : DbContext
{
    private readonly string _schema;

    public TheatricalPlaysDbContext(DbContextOptions<TheatricalPlaysDbContext> options, IConfiguration configuration) : base(options)
    {
        _schema = configuration.GetConnectionString(name: "Schema");
    }
    
    public virtual DbSet<Performer> Performers { get; set; }
    public virtual DbSet<Organizer> Organizers { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Venue> Venues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrWhiteSpace(_schema))
            modelBuilder.HasDefaultSchema(_schema);
    }
}