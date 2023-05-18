namespace Theatrical.Data.Models;

public class Contribution
{
    public int Id { get; set; }
    public int PerformerId { get; set; }
    public virtual Performer Performer { get; set; } 
    public int ProductionId { get; set; }
    public virtual Production Production { get; set; }
    public int RoleId { get; set; }
    public virtual Role Role { get; set; }
    public string? Subrole { get; set; }
    public DateTime Created { get; set; }
}