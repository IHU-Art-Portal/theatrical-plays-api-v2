namespace Theatrical.Data.Models;

public class Roles
{
    public int Id { get; set; }
    public string Role { get; set; }
    public virtual List<Contribution> Contributions { get; set; } //A role takes part in many contributions.
}