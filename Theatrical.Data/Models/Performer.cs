namespace Theatrical.Data.Models;

public class Performer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime Created { get; set; }
    
    public virtual List<Image> Images { get; set; } //A performer can have many Images
    public virtual List<Contribution> Contributions { get; set; } //A performer can contribute many times
}