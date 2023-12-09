namespace Theatrical.Data.Models;

public class Venue
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Address { get; set; }
    public int SystemId { get; set; }
    public DateTime Timestamp { get; set; }
    public virtual System System { get; set; } = null!;
    public virtual List<Event> Events { get; set; }
    public virtual List<UserVenue> UserVenues { get; set; }
}