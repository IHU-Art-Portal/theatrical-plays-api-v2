namespace Theatrical.Data.Models;

public class Venue
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public DateTime Created { get; set; }
    public virtual List<Event> Events { get; set; }
}