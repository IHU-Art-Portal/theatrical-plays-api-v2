using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Theatrical.Data.Models;

namespace Theatrical.Data.Context;

public class TheatricalPlaysDbContext : DbContext
{
    public TheatricalPlaysDbContext(DbContextOptions<TheatricalPlaysDbContext> options, IConfiguration configuration) :
        base(options)
    {
    }

    public virtual DbSet<Authority> Authorities { get; set; } = null!;
    public virtual DbSet<ChangeLog> ChangeLogs { get; set; } = null!;
    public virtual DbSet<Contribution> Contributions { get; set; } = null!;
    public virtual DbSet<Event> Events { get; set; } = null!;
    public virtual DbSet<Image> Images { get; set; } = null!;
    public virtual DbSet<Organizer> Organizers { get; set; } = null!;
    public virtual DbSet<Person> Persons { get; set; } = null!;
    public virtual DbSet<Production> Productions { get; set; } = null!;
    public virtual DbSet<Role> Roles { get; set; } = null!;
    public virtual DbSet<Models.System> Systems { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Venue> Venues { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL(
                "Server=195.251.123.174; Port=3306; Database=theatrical_plays_api_db; Uid=aris; Pwd=aris;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Authority>(entity =>
        {
            entity.ToTable("authorities");

            entity.HasIndex(e => e.Name, "name")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<ChangeLog>(entity =>
        {
            entity.ToTable("changeLog");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.CollumnName).HasMaxLength(100);

            entity.Property(e => e.EventType).HasMaxLength(100);

            entity.Property(e => e.TableName).HasMaxLength(100);

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp")
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Value).HasMaxLength(200);
        });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.ToTable("contributions");

            entity.HasIndex(e => e.PeopleId, "PeopleID");

            entity.HasIndex(e => e.ProductionId, "ProductionID");

            entity.HasIndex(e => e.RoleId, "RoleID");

            entity.HasIndex(e => e.SystemId, "SystemID");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.PeopleId).HasColumnName("PeopleID");

            entity.Property(e => e.ProductionId).HasColumnName("ProductionID");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.Property(e => e.SubRole)
                .HasMaxLength(100)
                .HasColumnName("subRole");

            entity.Property(e => e.SystemId).HasColumnName("SystemID");

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.People)
                .WithMany(p => p.Contributions)
                .HasForeignKey(d => d.PeopleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contributions_ibfk_2");

            entity.HasOne(d => d.Production)
                .WithMany(p => p.Contributions)
                .HasForeignKey(d => d.ProductionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contributions_ibfk_3");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Contributions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contributions_ibfk_1");

            entity.HasOne(d => d.System)
                .WithMany(p => p.Contributions)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contributions_ibfk_4");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");

            entity.HasIndex(e => e.ProductionId, "ProductionID");

            entity.HasIndex(e => e.SystemId, "SystemID");

            entity.HasIndex(e => e.VenueId, "VenueID");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.DateEvent).HasColumnType("datetime");

            entity.Property(e => e.PriceRange).HasMaxLength(30);

            entity.Property(e => e.ProductionId).HasColumnName("ProductionID");

            entity.Property(e => e.SystemId).HasColumnName("SystemID");

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.VenueId).HasColumnName("VenueID");

            entity.HasOne(d => d.Production)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.ProductionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("events_ibfk_1");

            entity.HasOne(d => d.System)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("events_ibfk_3");

            entity.HasOne(d => d.Venue)
                .WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("events_ibfk_2");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("images");

            entity.HasIndex(e => e.PersonId, "personID");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(100)
                .HasColumnName("imageURL");

            entity.Property(e => e.PersonId).HasColumnName("personID");
        });

        modelBuilder.Entity<Organizer>(entity =>
        {
            entity.ToTable("organizer");

            entity.HasIndex(e => e.SystemId, "SystemID");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Address).HasMaxLength(50);

            entity.Property(e => e.Afm).HasMaxLength(30);

            entity.Property(e => e.Doy).HasMaxLength(30);

            entity.Property(e => e.Email).HasMaxLength(100);

            entity.Property(e => e.Name).HasMaxLength(80);

            entity.Property(e => e.Phone).HasMaxLength(30);

            entity.Property(e => e.Postcode)
                .HasMaxLength(20)
                .HasColumnName("postcode");

            entity.Property(e => e.SystemId).HasColumnName("SystemID");

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Town).HasMaxLength(100);

            entity.HasOne(d => d.System)
                .WithMany(p => p.Organizers)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organizer_ibfk_1");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("persons");

            entity.HasIndex(e => e.SystemId, "SystemID");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Fullname).HasMaxLength(600);

            entity.Property(e => e.SystemId).HasColumnName("SystemID");

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.System)
                .WithMany(p => p.People)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("persons_ibfk_1");
        });

        modelBuilder.Entity<Production>(entity =>
        {
            entity.ToTable("production");

            entity.HasIndex(e => e.OrganizerId, "OrganizerID");

            entity.HasIndex(e => e.SystemId, "SystemID");

            entity.HasIndex(e => e.SystemId, "SystemID_2");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Duration).HasMaxLength(30);

            entity.Property(e => e.MediaUrl)
                .HasMaxLength(500)
                .HasColumnName("MediaURL");

            entity.Property(e => e.OrganizerId).HasColumnName("OrganizerID");

            entity.Property(e => e.Producer).HasMaxLength(255);

            entity.Property(e => e.SystemId).HasColumnName("SystemID");

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Title).HasMaxLength(255);

            entity.Property(e => e.Url)
                .HasMaxLength(256)
                .HasColumnName("URL");

            entity.HasOne(d => d.Organizer)
                .WithMany(p => p.Productions)
                .HasForeignKey(d => d.OrganizerId)
                .HasConstraintName("production_ibfk_1");

            entity.HasOne(d => d.System)
                .WithMany(p => p.Productions)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("production_ibfk_2");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasIndex(e => e.SystemId, "SystemID");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Role1)
                .HasMaxLength(150)
                .HasColumnName("Role");

            entity.Property(e => e.SystemId).HasColumnName("SystemID");

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.System)
                .WithMany(p => p.Roles)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_ibfk_1");
        });

        modelBuilder.Entity<Models.System>(entity =>
        {
            entity.ToTable("system");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Email).HasColumnName("email");

            entity.Property(e => e.Enabled).HasColumnName("enabled");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.HasMany(d => d.Authorities)
                .WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserAuthority",
                    l => l.HasOne<Authority>().WithMany().HasForeignKey("AuthorityId")
                        .OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("user_roles_authority_id_fk"),
                    r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_roles_user_id_fk"),
                    j =>
                    {
                        j.HasKey("UserId", "AuthorityId").HasName("PRIMARY");

                        j.ToTable("user_authorities");

                        j.HasIndex(new[] { "AuthorityId" }, "user_roles_authority_id_fk");

                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");

                        j.IndexerProperty<int>("AuthorityId").HasColumnName("authority_id");
                    });
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("venue");

            entity.HasIndex(e => e.SystemId, "SystemID");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Address).HasMaxLength(60);

            entity.Property(e => e.SystemId).HasColumnName("SystemID");

            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp")
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnName("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Title).HasMaxLength(60);

            entity.HasOne(d => d.System)
                .WithMany(p => p.Venues)
                .HasForeignKey(d => d.SystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("venue_ibfk_1");
        });

        
    }
}