
namespace Theatrical.Data.Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public bool? Enabled { get; set; }
    public string? VerificationCode { get; set; }
    public bool _2FA_enabled { get; set; }
    public string? _2FA_code { get; set; }
    public string? UserSecret { get; set; }
    public List<string>? PerformerRoles { get; set; }
    public string? Facebook { get; set; }
    public string? Youtube { get; set; }
    public string? Instagram { get; set; }
    public string? BioPdfLocation { get; set; }
    public virtual List<UserImage>? UserImages { get; set; }
    
    public virtual List<UserAuthority> UserAuthorities { get; set; }
    public virtual List<Transaction> UserTransactions { get; set; }
    public virtual List<UserVenue> UserVenue { get; set; }
}