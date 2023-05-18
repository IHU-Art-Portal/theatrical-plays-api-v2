
namespace Theatrical.Data.Models;

public class Production
{
    public int Id { get; set; }
    public int OrganizerId { get; set; }
    public virtual Organizer Organizer { get; set; } //A production belongs to one Organizer
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Url { get; set; }
    public string? Producer { get; set; }
    public string? MediaUrl { get; set; }
    public string? Duration { get; set; }
    public DateTime Created { get; set; }
    public virtual List<Contribution> Contributions { get; set; } //A production can have many contributions
    public virtual List<Event> Events { get; set; }
}