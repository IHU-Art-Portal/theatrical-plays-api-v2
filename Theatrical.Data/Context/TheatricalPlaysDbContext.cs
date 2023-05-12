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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrWhiteSpace(_schema))
            modelBuilder.HasDefaultSchema(_schema);
    }
}