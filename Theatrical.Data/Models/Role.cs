namespace Theatrical.Data.Models;

public class Role
{
    public int Id { get; set; }
    public string Role1 { get; set; } = null!;
    public int SystemId { get; set; }
    public DateTime Timestamp { get; set; }

    //Navigational Properties
    public virtual System System { get; set; } = null!;
    public virtual List<Contribution> Contributions { get; set; }
}