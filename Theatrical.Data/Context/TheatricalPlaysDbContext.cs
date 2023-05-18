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
    public virtual DbSet<Image> Images { get; set; }
    public virtual DbSet<Production> Productions { get; set; }
    public virtual DbSet<Contribution> Contributions { get; set; }
    public virtual DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrWhiteSpace(_schema))
            modelBuilder.HasDefaultSchema(_schema);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Image>()
            .HasOne(i => i.Performer)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.PerformerId);

        modelBuilder.Entity<Production>()
            .HasOne(p => p.Organizer)
            .WithMany(o => o.Productions)
            .HasForeignKey(p => p.OrganizerId);

        modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Performer)
            .WithMany(p => p.Contributions)
            .HasForeignKey(c => c.PerformerId);

        modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Production)
            .WithMany(p => p.Contributions)
            .HasForeignKey(c => c.ProductionId);

        modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Role)
            .WithMany(r => r.Contributions)
            .HasForeignKey(c => c.RoleId);

        modelBuilder.Entity<Event>()
            .HasOne(e => e.Production)
            .WithMany(p => p.Events)
            .HasForeignKey(e => e.ProductionId);

        modelBuilder.Entity<Event>()
            .HasOne(e => e.Venue)
            .WithMany(v => v.Events)
            .HasForeignKey(e => e.VenueId);
    }
}