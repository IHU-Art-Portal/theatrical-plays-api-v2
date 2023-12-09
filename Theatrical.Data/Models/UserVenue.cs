namespace Theatrical.Data.Models;

public class UserVenue
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VenueId { get; set; }
    public DateTime DateCreated { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Venue Venue { get; set; }
}