namespace Theatrical.Data.Models;

public class Contribution
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public int ProductionId { get; set; }
    public int RoleId { get; set; }
    public string? SubRole { get; set; }
    public int SystemId { get; set; }
    public DateTime Timestamp { get; set; }

    public virtual Person People { get; set; } = null!;
    public virtual Production Production { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    public virtual System System { get; set; } = null!;
}