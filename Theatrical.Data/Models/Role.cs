namespace Theatrical.Data.Models;

public class Role
{
    public int Id { get; set; }
    public string Value { get; set; }
    public virtual List<Contribution> Contributions { get; set; } //A role takes part in many contributions.
}