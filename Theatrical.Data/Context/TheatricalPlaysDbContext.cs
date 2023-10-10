using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Theatrical.Data.Models;

namespace Theatrical.Data.Context;

public class TheatricalPlaysDbContext : DbContext
{
    private readonly IConfiguration _config;

    public TheatricalPlaysDbContext(DbContextOptions<TheatricalPlaysDbContext> options, IConfiguration configuration) :
        base(options)
    {
        _config = configuration;
    }

    public virtual DbSet<Authority> Authorities { get; set; } = null!;
    public virtual DbSet<UserAuthority> UserAuthorities { get; set; } = null!;
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
    public virtual DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_config.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

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
            
            entity.Property(e => e.HairColor).HasColumnName("HairColor");
            entity.Property(e => e.Height).HasColumnName("Height");
            entity.Property(e => e.EyeColor).HasColumnName("EyeColor");
            entity.Property(e => e.Weight).HasColumnName("Weight");
            entity.Property(e => e.Languages).HasColumnName("Languages");
            entity.Property(e => e.Description).HasColumnName("Description");
            entity.Property(e => e.Bio).HasColumnName("Bio");
            entity.Property(e => e.Birthdate).HasColumnName("Birthdate");
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

            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Email, "email")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Email).HasColumnName("email");

            entity.Property(e => e.Enabled).HasColumnName("enabled");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.Property(e => e.VerificationCode).HasColumnName("verification_code");

            entity.Property(u => u._2FA_enabled).HasColumnName("2FA_enabled");

            entity.Property(u => u._2FA_code).HasColumnName("2FA_code");
            
            entity.Property(u => u.UserSecret).HasColumnName("user_secret");

            entity.Property(u => u.Facebook).HasColumnName("facebook");
            entity.Property(u => u.Youtube).HasColumnName("youtube");
            entity.Property(u => u.Instagram).HasColumnName("instagram");
        });
        
        modelBuilder.Entity<Authority>(entity =>
        {
            entity.ToTable("authorities");

            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Name, "name")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name).HasColumnName("name");
        });
        
        modelBuilder.Entity<UserAuthority>(entity =>
        {
            entity.ToTable("user_authorities");
            
            entity.HasKey(ua => new { ua.UserId, ua.AuthorityId });
            
            entity.Property(ua => ua.UserId).HasColumnName("user_id");
            entity.Property(ua => ua.AuthorityId).HasColumnName("authority_id");

            entity.HasOne(ua => ua.User)
                .WithMany(u => u.UserAuthorities)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ua => ua.Authority)
                .WithMany()
                .HasForeignKey(ua => ua.AuthorityId)
                .OnDelete(DeleteBehavior.Cascade);
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

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.HasOne(t => t.User)
                .WithMany(u => u.UserTransactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("usertransactions_ut1");

            entity.Property(t => t.TransactionId).HasColumnName("TransactionID");
            entity.Property(t => t.NetworkTransactionId).HasColumnName("NetworkTransactionID");
        });

    }
}