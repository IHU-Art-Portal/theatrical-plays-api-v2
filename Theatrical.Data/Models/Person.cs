namespace Theatrical.Data.Models;

public class Person
{
    public int Id { get; set; }
    public string Fullname { get; set; }
    public DateTime Timestamp { get; set; }
    public int SystemID { get; set; }
    public virtual List<Image> Images { get; set; } //A performer can have many Images
    public virtual List<Contribution> Contributions { get; set; } //A performer can contribute many times
}